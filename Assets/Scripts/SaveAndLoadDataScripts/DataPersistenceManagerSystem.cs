using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistenceManagerSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;
    //private List<IDataPersistence> dataPersistencesObjects;
    //private EcsPoolInject<DataComponent> _dataComponentsPool;
    private EcsPoolInject<LoadGameEvent> _loadGameEventsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<SaveGameEvent> _saveGameEventsPool;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentsPool;
    private EcsPoolInject<InventoryCellComponent> _inventoryCellsComponentsPool;
    private EcsPoolInject<StorageCellTag> _storageCellTagsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<SpecialInventoryCellTag> _specialInventoryCellTagsPool;
    private EcsPoolInject<InventoryComponent> _inventoryComponent;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<FlashLightInInventoryComponent> _flashLightInInventoryComponentsPool;
    private EcsPoolInject<NowUsedWeaponTag> _nowUsedWeaponTagsPool;
    private EcsPoolInject<PlayerMeleeWeaponComponent> _playerMeleeWeaponComponentsPool;
    private EcsPoolInject<MeleeWeaponComponent> _meleeWeaponComponentsPool;
    private EcsPoolInject<AttackComponent> _attackComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;

    private EcsFilterInject<Inc<SaveGameEvent>> _saveGameEventsFilter;
    private EcsFilterInject<Inc<InventoryItemComponent>> _inventoryItemComponentsFilter;
    private EcsFilterInject<Inc<InventoryCellComponent>> _inventoryCellComponentsFilter;
    private EcsFilterInject<Inc<InventoryComponent>> _inventoryComponentsFilter;

    private GameData gameData;
    private string fileName = "bestgamewithfishevernumtwo";
    private bool useEncryption = true;
    private FileDataHandler dataHandler;
    private int dataComponentEntity;
    //public static DataPersistenceManager instance { get; private set; }

    /* private void Awake()
     {
         instance = this;
     }*/
    public void Init(IEcsSystems systems)
    {
        _sceneData.Value.inventoryEntity = _world.Value.NewEntity();
        _sceneData.Value.storageEntity = _world.Value.NewEntity();
        //  Debug.Log(_sceneData.Value.inventoryEntity + "inv" + _sceneData.Value.storageEntity + "str");
        dataComponentEntity = _world.Value.NewEntity();
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        LoadGame();
    }
    public void Run(IEcsSystems systems)
    {
        foreach (var save in _saveGameEventsFilter.Value)
        {
            SaveGame();
            _saveGameEventsPool.Value.Del(save);
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        List<InventoryCellView> cells = new List<InventoryCellView>();

        #region -special cells setup-
        int firstCell = _world.Value.NewEntity();
        _sceneData.Value.firstGunCellView.Construct(firstCell, _world.Value);
        ref var invCellCmpFirstWeapon = ref _inventoryCellsComponentsPool.Value.Add(firstCell);
        invCellCmpFirstWeapon.isEmpty = true;
        invCellCmpFirstWeapon.cellView = _sceneData.Value.firstGunCellView;
        _specialInventoryCellTagsPool.Value.Add(firstCell);
        //_playerWeaponsInInventoryComponentsPool.Value.Add(firstCell);
        cells.Add(invCellCmpFirstWeapon.cellView);

        int secondCell = _world.Value.NewEntity();
        _sceneData.Value.secondGunCellView.Construct(secondCell, _world.Value);
        ref var invCellCmpSecondWeapon = ref _inventoryCellsComponentsPool.Value.Add(secondCell);
        invCellCmpSecondWeapon.isEmpty = true;
        invCellCmpSecondWeapon.cellView = _sceneData.Value.secondGunCellView;
        //_playerWeaponsInInventoryComponentsPool.Value.Add(secondCell);
        _specialInventoryCellTagsPool.Value.Add(secondCell);
        cells.Add(invCellCmpSecondWeapon.cellView);

        int meleeCell = _world.Value.NewEntity();
        _sceneData.Value.meleeWeaponCellView.Construct(meleeCell, _world.Value);
        ref var invCellCmpMeleeWeapon = ref _inventoryCellsComponentsPool.Value.Add(meleeCell);
        invCellCmpMeleeWeapon.isEmpty = true;
        invCellCmpMeleeWeapon.cellView = _sceneData.Value.meleeWeaponCellView;
        //_playerWeaponsInInventoryComponentsPool.Value.Add(meleeCell);
        _specialInventoryCellTagsPool.Value.Add(meleeCell);
        cells.Add(invCellCmpMeleeWeapon.cellView);
        _nowUsedWeaponTagsPool.Value.Add(meleeCell);

        int healingItemCell = _world.Value.NewEntity();
        _sceneData.Value.healingItemCellView.Construct(healingItemCell, _world.Value);
        ref var invCellCmpHealingItem = ref _inventoryCellsComponentsPool.Value.Add(healingItemCell);
        invCellCmpHealingItem.isEmpty = true;
        invCellCmpHealingItem.cellView = _sceneData.Value.healingItemCellView;
        //_playerWeaponsInInventoryComponentsPool.Value.Add(healingItemCell);
        _specialInventoryCellTagsPool.Value.Add(healingItemCell);
        cells.Add(invCellCmpHealingItem.cellView);

        int flashlightItemCell = _world.Value.NewEntity();
        _sceneData.Value.flashlightItemCellView.Construct(flashlightItemCell, _world.Value);
        ref var invCellCmpFlashlightItem = ref _inventoryCellsComponentsPool.Value.Add(flashlightItemCell);
        invCellCmpFlashlightItem.isEmpty = true;
        invCellCmpFlashlightItem.cellView = _sceneData.Value.flashlightItemCellView;
        _specialInventoryCellTagsPool.Value.Add(flashlightItemCell);
        cells.Add(invCellCmpFlashlightItem.cellView);
        #endregion


        // _nowUsedWeaponTagsPool.Value.Add(_sceneData.Value.meleeWeaponCellView._entity);

        //5 спец клеток
        for (int i = 0; i < _sceneData.Value.inventoryCellsCount; i++)
        {
            var cellEntity = _world.Value.NewEntity();

            ref var inventoryCellsCmp = ref _inventoryCellsComponentsPool.Value.Add(cellEntity);
            inventoryCellsCmp.isEmpty = true;
            inventoryCellsCmp.cellView = _sceneData.Value.GetInventoryCell(cellEntity, _world.Value, _sceneData.Value.inventoryCellsContainer);
            cells.Add(inventoryCellsCmp.cellView);
        }

        for (int i = 0; i < _sceneData.Value.storageCellsCount; i++)
        {
            var cellEntity = _world.Value.NewEntity();

            ref var storageCellsCmp = ref _inventoryCellsComponentsPool.Value.Add(cellEntity);
            _storageCellTagsPool.Value.Add(cellEntity);
            storageCellsCmp.isEmpty = true;
            storageCellsCmp.cellView = _sceneData.Value.GetInventoryCell(cellEntity, _world.Value, _sceneData.Value.storageCellsContainer);
            cells.Add(storageCellsCmp.cellView);
        }

        _sceneData.Value.idItemslist.cells = new InventoryCellView[cells.Count];
        int j = 0;
        foreach (var cell in cells)
        {
            _sceneData.Value.idItemslist.cells[j++] = cell;
        }


        this.gameData = dataHandler.Load();

        if (this.gameData == null)
            NewGame();


        ref var invCmp = ref _inventoryComponent.Value.Add(_sceneData.Value.inventoryEntity);
        ref var storageCmp = ref _inventoryComponent.Value.Add(_sceneData.Value.storageEntity);

        Dictionary<int, int> durabilityItemsForSaveDataList = new Dictionary<int, int>();
        Dictionary<int, int> bulletsWeaponForSaveDataList = new Dictionary<int, int>();

        foreach (var item in gameData.durabilityItemsForSaveData)
            durabilityItemsForSaveDataList.Add(item.cellId, item.num);

        foreach (var item in gameData.bulletsWeaponForSaveData)
            bulletsWeaponForSaveDataList.Add(item.cellId, item.num);

        int playerEntity = _sceneData.Value.playerEntity;
        ref var playerWeaponsInInvCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(playerEntity);

        foreach (var item in this.gameData.itemsCellinfo)
        {
            //Debug.Log(item.itemCellId + "cell id");
            int cellEntity = _sceneData.Value.idItemslist.cells[item.itemCellId]._entity;

            ref var itemCmp = ref _inventoryItemComponentsPool.Value.Add(cellEntity);
            ref var invCellCmp = ref _inventoryCellsComponentsPool.Value.Get(cellEntity);
            invCellCmp.isEmpty = false;


            itemCmp.currentItemsCount = item.itemCount;
            itemCmp.itemInfo = _sceneData.Value.idItemslist.items[item.itemId];
            invCellCmp.inventoryItemComponent = itemCmp;

            invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);

            invCellCmp.cellView.ChangeCellItemSprite(itemCmp.itemInfo.itemSprite);

            if (!_storageCellTagsPool.Value.Has(cellEntity))
                invCmp.weight += itemCmp.currentItemsCount * itemCmp.itemInfo.itemWeight;
            else
                storageCmp.weight += itemCmp.currentItemsCount * itemCmp.itemInfo.itemWeight;

            Debug.Log(itemCmp.currentItemsCount + " count " + itemCmp.itemInfo.itemWeight + "kg " + itemCmp.itemInfo.itemId + "id");

            if (itemCmp.itemInfo.type == ItemInfo.itemType.gun)
            {
                ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Add(cellEntity);
                gunInInvCmp.currentAmmo = bulletsWeaponForSaveDataList[item.itemCellId];
                gunInInvCmp.gunDurability = durabilityItemsForSaveDataList[item.itemCellId];
                if (cellEntity == _sceneData.Value.firstGunCellView._entity)
                {
                    Debug.Log("gun added" + cellEntity);
                    playerWeaponsInInvCmp.gunFirstObject = itemCmp.itemInfo.gunInfo;
                    playerWeaponsInInvCmp.curFirstWeaponDurability = gunInInvCmp.gunDurability;
                    playerWeaponsInInvCmp.curFirstWeaponAmmo = gunInInvCmp.currentAmmo;
                    playerWeaponsInInvCmp.curEquipedWeaponsCount++;
                    gunInInvCmp.isEquipedWeapon = true;
                }
                else if (cellEntity == _sceneData.Value.secondGunCellView._entity)
                {
                    playerWeaponsInInvCmp.gunSecondObject = itemCmp.itemInfo.gunInfo;
                    playerWeaponsInInvCmp.curSecondWeaponDurability = gunInInvCmp.gunDurability;
                    playerWeaponsInInvCmp.curSecondWeaponAmmo = gunInInvCmp.currentAmmo;
                    playerWeaponsInInvCmp.curEquipedWeaponsCount++;
                    gunInInvCmp.isEquipedWeapon = true;
                }
            }
            else if (itemCmp.itemInfo.type == ItemInfo.itemType.flashlight)
            {
                _flashLightInInventoryComponentsPool.Value.Add(cellEntity).currentChargeRemainigTime = durabilityItemsForSaveDataList[item.itemCellId];
            }
            //если будут добавляться клетки нвентаря, то в списке их добавлять до клеток склада
        }
        ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Get(cells[2]._entity);
        Debug.Log(cells[2]._entity + "ent " + _inventoryItemComponentsPool.Value.Has(cells[2]._entity));
        ref var meleeCmp = ref _meleeWeaponComponentsPool.Value.Get(playerEntity);
        ref var curAttackCmp = ref _attackComponentsPool.Value.Get(playerEntity);
        ref var meleeInvCellCmp = ref _inventoryCellsComponentsPool.Value.Get(cells[2]._entity);
        ref var playerMeleeCmp = ref _playerMeleeWeaponComponentsPool.Value.Add(playerEntity);
        //_gunComponentsPool.Value.Add(playerEntity);
        //_playerGunComponentsPool.Value.Add(playerEntity);

        _sceneData.Value.statsInventoryText.text = invCmp.weight + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
        _sceneData.Value.statsStorageText.text = storageCmp.weight + "kg/ " + _sceneData.Value.maxInStorageWeight + "kg \n max cells " + _sceneData.Value.storageCellsCount;

        _playerComponentsPool.Value.Get(playerEntity).money = this.gameData.money;

        _globalTimeComponentsPool.Value.Get(playerEntity).currentDay = this.gameData.currentDay;
        _globalTimeComponentsPool.Value.Get(playerEntity).currentDayTime = this.gameData.currentDayTime;

        //  List<ItemInfoForSaveData> items = new List<ItemInfoForSaveData>();

        Debug.Log("time" + this.gameData.currentDayTime + " " + this.gameData.itemsCellinfo[0].itemCount);
        meleeInvCellCmp.isEmpty = false;
        // invItemCmp.currentItemsCount++;
        //_sceneData.Value.meleeWeaponCellView.ChangeCellItemCount(invItemCmp.currentItemsCount);
        // invItemCmp.itemInfo = _sceneData.Value.meleeWeaponItemInfoStarted;
        playerWeaponsInInvCmp.curEquipedWeaponsCount++;
        playerWeaponsInInvCmp.curWeapon = 2;//номер клетки с милишкой
        curAttackCmp.changeWeaponTime = invItemCmp.itemInfo.meleeWeaponInfo.weaponChangeSpeed;
        curAttackCmp.attackCouldown = invItemCmp.itemInfo.meleeWeaponInfo.attackCouldown;
        curAttackCmp.damage = invItemCmp.itemInfo.meleeWeaponInfo.damage;
        playerMeleeCmp.weaponInfo = invItemCmp.itemInfo.meleeWeaponInfo;
        playerWeaponsInInvCmp.meleeWeaponObject = playerMeleeCmp.weaponInfo;

        var playerView = _playerComponentsPool.Value.Get(playerEntity).view;
        playerView.weaponSpriteRenderer.sprite = playerMeleeCmp.weaponInfo.weaponSprite;
        playerView.weaponTransform.localScale = Vector3.one * playerMeleeCmp.weaponInfo.spriteScaleMultiplayer;
        playerView.weaponTransform.localEulerAngles = new Vector3(0, 0, playerMeleeCmp.weaponInfo.spriteRotation);

        _sceneData.Value.ammoInfoText.text = "";
        // _sceneData.Value.meleeWeaponCellView.ChangeCellItemSprite(invItemCmp.itemInfo.itemSprite);
        // _gunInventoryCellComponentsPool.Value.Add(_sceneData.Value.firstGunCellView._entity);
        _loadGameEventsPool.Value.Add(dataComponentEntity);
    }

    public void SaveGame()
    {
        int playerEntity = _sceneData.Value.playerEntity;
        this.gameData.money = _playerComponentsPool.Value.Get(playerEntity).money;

        this.gameData.currentDay = _globalTimeComponentsPool.Value.Get(playerEntity).currentDay;
        this.gameData.currentDayTime = _globalTimeComponentsPool.Value.Get(playerEntity).currentDayTime;

        //сохранение инвентаря
        List<ItemInfoForSaveData> items = new List<ItemInfoForSaveData>();


        this.gameData.itemsCellinfo = new ItemInfoForSaveData[_inventoryItemComponentsFilter.Value.GetEntitiesCount()];

        ref var cellsInventory = ref _sceneData.Value.idItemslist.cells;

        List<NumAndIdForSafeData> bulletsWeaponForSaveData = new List<NumAndIdForSafeData>();
        List<NumAndIdForSafeData> durabilityItemsForSaveData = new List<NumAndIdForSafeData>();

        for (int i = 0; i < cellsInventory.Length; i++)
        {
            if (_inventoryItemComponentsPool.Value.Has(cellsInventory[i]._entity))
            {
                var invItemCmp = _inventoryItemComponentsPool.Value.Get(cellsInventory[i]._entity);

                items.Add(new ItemInfoForSaveData(invItemCmp.itemInfo.itemId, invItemCmp.currentItemsCount, i));
                // Debug.Log(invItemCmp.currentItemsCount + "saved items count");
                if (invItemCmp.itemInfo.type == ItemInfo.itemType.gun)
                {
                    if (_specialInventoryCellTagsPool.Value.Has(cellsInventory[i]._entity))
                    {
                        if (_nowUsedWeaponTagsPool.Value.Has(cellsInventory[i]._entity))
                        {
                            bulletsWeaponForSaveData.Add(new NumAndIdForSafeData(i, _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity).currentMagazineCapacity));
                            durabilityItemsForSaveData.Add(new NumAndIdForSafeData(i, _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity).durabilityPoints));
                        }
                        else if(cellsInventory[i]._entity == _sceneData.Value.firstGunCellView._entity)
                        {
                            var playerWeaponsInInvCmp = _playerWeaponsInInventoryComponentsPool.Value.Get(playerEntity);
                            durabilityItemsForSaveData.Add(new NumAndIdForSafeData(i, playerWeaponsInInvCmp.curFirstWeaponDurability));
                            bulletsWeaponForSaveData.Add(new NumAndIdForSafeData(i, playerWeaponsInInvCmp.curFirstWeaponAmmo));
                        }
                        else if (cellsInventory[i]._entity == _sceneData.Value.secondGunCellView._entity)
                        {
                            var playerWeaponsInInvCmp = _playerWeaponsInInventoryComponentsPool.Value.Get(playerEntity);
                            durabilityItemsForSaveData.Add(new NumAndIdForSafeData(i, playerWeaponsInInvCmp.curSecondWeaponDurability));
                            bulletsWeaponForSaveData.Add(new NumAndIdForSafeData(i, playerWeaponsInInvCmp.curSecondWeaponAmmo));
                        }
                    }
                    else
                    {
                        ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(cellsInventory[i]._entity);
                        bulletsWeaponForSaveData.Add(new NumAndIdForSafeData(i, gunInInvCmp.currentAmmo));
                        durabilityItemsForSaveData.Add(new NumAndIdForSafeData(i, gunInInvCmp.gunDurability));

                    }
                }
                else if (invItemCmp.itemInfo.type == ItemInfo.itemType.flashlight)
                {
                    durabilityItemsForSaveData.Add(new NumAndIdForSafeData(i, (int)_flashLightInInventoryComponentsPool.Value.Get(cellsInventory[i]._entity).currentChargeRemainigTime));//возможно когда то поправить этот незначительный недочёт
                }
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            gameData.itemsCellinfo[i] = items[i];
        }
        gameData.bulletsWeaponForSaveData = new NumAndIdForSafeData[bulletsWeaponForSaveData.Count];
        for (int i = 0; i < bulletsWeaponForSaveData.Count; i++)
        {
            gameData.bulletsWeaponForSaveData[i] = bulletsWeaponForSaveData[i];
        }

        gameData.durabilityItemsForSaveData = new NumAndIdForSafeData[durabilityItemsForSaveData.Count];
        for (int i = 0; i < durabilityItemsForSaveData.Count; i++)
        {
            gameData.durabilityItemsForSaveData[i] = durabilityItemsForSaveData[i];
        }
        // Debug.Log("time" + this.gameData.currentDayTime);

        dataHandler.Save(this.gameData);
    }


}
