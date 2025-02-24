using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManagerSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;
    //private List<IDataPersistence> dataPersistencesObjects;
    //private EcsPoolInject<DataComponent> _dataComponentsPool;
    private EcsPoolInject<LoadGameEvent> _loadGameEventsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<SaveGameEvent> _saveGameEventsPool;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentsPool { get; set; }
    private EcsPoolInject<InventoryCellComponent> _inventoryCellsComponentsPool;
    private EcsPoolInject<StorageCellTag> _storageCellTagsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<SpecialInventoryCellTag> _specialInventoryCellTagsPool;
    private EcsPoolInject<InventoryComponent> _inventoryComponent;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<DurabilityInInventoryComponent> _flashLightInInventoryComponentsPool;
    private EcsPoolInject<NowUsedWeaponTag> _nowUsedWeaponTagsPool;
    private EcsPoolInject<PlayerMeleeWeaponComponent> _playerMeleeWeaponComponentsPool;
    private EcsPoolInject<MeleeWeaponComponent> _meleeWeaponComponentsPool;
    private EcsPoolInject<AttackComponent> _attackComponentsPool;
    private EcsPoolInject<HidedObjectOutsideFOVComponent> _hidedObjectOutsideFOVComponentsPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<QuestComponent> _questComponentsPool;
    private EcsPoolInject<CellsListComponent> _cellsListComponentsPool;
    private EcsPoolInject<PlayerUpgradedStats> _playerUpgradedStatsPool;
    private EcsPoolInject<DeathEvent> _deathEventsPool;
    private EcsPoolInject<LaserPointerForGunComponent> _laserPointerForGunComponentsPool;
    private EcsPoolInject<SolarPanelElectricGeneratorComponent> _solarPanelElectricGeneratorComponentsPool;
    private EcsPoolInject<InventoryCellTag> _inventoryCellTagsPool;
    private EcsPoolInject<SecondDurabilityComponent> _shieldComponentsPool;
    private EcsPoolInject<BuildingCheckerComponent> _buildingCheckerComponentsPool;
    private EcsPoolInject<CraftingTableComponent> _craftingTableComponentsPool;
    private EcsPoolInject<PlayerMoveComponent> _playerMoveComponentsPool;
    private EcsPoolInject<FieldOfViewComponent> _fieldOfViewComponentsPool;
    private EcsPoolInject<QuestNPCComponent> _questNPCComponentsPool;
    private EcsPoolInject<MenuStatesComponent> _menuStatesComponentsPool;
    private EcsPoolInject<ShopCharacterComponent> _shopCharacterComponentsPool;

    private EcsFilterInject<Inc<SaveGameEvent>> _saveGameEventsFilter;
    private EcsFilterInject<Inc<InventoryItemComponent>> _inventoryItemComponentsFilter;
    private EcsFilterInject<Inc<DeathEvent, PlayerComponent>> _playerDeathEventsFilter;

    private GameData gameData;
    private string fileName = "bestgamewithfishevernumtwo";
    private bool useEncryption = true;
    private FileDataHandler dataHandler;
    private int dataComponentEntity;

    public void Init(IEcsSystems systems)
    {
        Cursor.visible = false;
        _sceneData.Value.inventoryEntity = _world.Value.NewEntity();
        _sceneData.Value.storageEntity = _world.Value.NewEntity();
        dataComponentEntity = _world.Value.NewEntity();
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        _sceneData.Value.dropedItemsUIView.restartGameButton.onClick.AddListener(StartNewGame);
        LoadGame();
    }
    public void Run(IEcsSystems systems)
    {
        foreach (var save in _saveGameEventsFilter.Value)
        {
            SaveGame(_saveGameEventsPool.Value.Get(save).type);
            _saveGameEventsPool.Value.Del(save);
        }
        foreach (var playerDeath in _playerDeathEventsFilter.Value)
        {
            ref var fOVCmp = ref _fieldOfViewComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            ref var playerCmp = ref _playerComponentsPool.Value.Get(playerDeath);
            var playerView = playerCmp.view;
            playerCmp.hasForestGuide = false;
            playerCmp.canDeffuseMines = false;
            fOVCmp.fieldOfView = playerView.defaultFOV;
            fOVCmp.viewDistance = playerView.viewDistance;
            Cursor.visible = true;
            _deathEventsPool.Value.Del(playerDeath);
            _buildingCheckerComponentsPool.Value.Get(_sceneData.Value.playerEntity).isHideRoof = false;
        }
        if (_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).restartGameButtonIsPressed)
        {
            ref var menuStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            menuStatesCmp.timeScincePressRestartGameButton += Time.deltaTime;
            _sceneData.Value.dropedItemsUIView.restartGameButtonText.text = "Press the button after " + (int)(5f - menuStatesCmp.timeScincePressRestartGameButton) + " seconds to delete progress";
            if (menuStatesCmp.timeScincePressRestartGameButton >= 5)
            {
                menuStatesCmp.restartGameButtonIsPressed = false;
                _sceneData.Value.dropedItemsUIView.restartGameButtonText.text = "Press button to restart game";
                //after exit make set params
            }
        }
    }

    public void StartNewGame()
    {
        ref var menuStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        if (menuStatesCmp.timeScincePressRestartGameButton == 0)
            menuStatesCmp.restartGameButtonIsPressed = true;
        else if (menuStatesCmp.timeScincePressRestartGameButton >= 5)
        {
            this.gameData = null;
            dataHandler.Save(this.gameData);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame()
    {
        List<InventoryCellView> cells = new List<InventoryCellView>();
        ref var cellsCmp = ref _cellsListComponentsPool.Value.Add(_sceneData.Value.playerEntity);

        for (int i = 0; i < _sceneData.Value.storageCellsCount; i++)
        {
            var cellEntity = _world.Value.NewEntity();

            ref var storageCellsCmp = ref _inventoryCellsComponentsPool.Value.Add(cellEntity);
            _storageCellTagsPool.Value.Add(cellEntity);
            storageCellsCmp.isEmpty = true;
            storageCellsCmp.cellView = _sceneData.Value.GetItemCell(_sceneData.Value.storageCellsContainer);
            storageCellsCmp.cellView.Construct(cellEntity, _world.Value);
            cells.Add(storageCellsCmp.cellView);
        }

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
        // int meleeCellId = cells.Count;
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

        int grenadeItemCell = _world.Value.NewEntity();
        _sceneData.Value.grenadeCellView.Construct(grenadeItemCell, _world.Value);
        ref var invCellCmpGrenadeItem = ref _inventoryCellsComponentsPool.Value.Add(grenadeItemCell);
        invCellCmpGrenadeItem.isEmpty = true;
        invCellCmpGrenadeItem.cellView = _sceneData.Value.grenadeCellView;
        _specialInventoryCellTagsPool.Value.Add(grenadeItemCell);
        cells.Add(invCellCmpGrenadeItem.cellView);

        int backpackItemCell = _world.Value.NewEntity();
        _sceneData.Value.backpackCellView.Construct(backpackItemCell, _world.Value);
        ref var invCellCmpBackpackItem = ref _inventoryCellsComponentsPool.Value.Add(backpackItemCell);
        invCellCmpBackpackItem.isEmpty = true;
        invCellCmpBackpackItem.cellView = _sceneData.Value.backpackCellView;
        _specialInventoryCellTagsPool.Value.Add(backpackItemCell);
        cells.Add(invCellCmpBackpackItem.cellView);

        int shieldItemCell = _world.Value.NewEntity();
        _sceneData.Value.shieldCellView.Construct(shieldItemCell, _world.Value);
        ref var invCellCmpShieldItem = ref _inventoryCellsComponentsPool.Value.Add(shieldItemCell);
        invCellCmpShieldItem.isEmpty = true;
        invCellCmpShieldItem.cellView = _sceneData.Value.shieldCellView;
        _specialInventoryCellTagsPool.Value.Add(shieldItemCell);
        cells.Add(invCellCmpShieldItem.cellView);

        int bodyArmorItemCell = _world.Value.NewEntity();
        _sceneData.Value.bodyArmorCellView.Construct(bodyArmorItemCell, _world.Value);
        ref var invCellCmpBodyArmorItem = ref _inventoryCellsComponentsPool.Value.Add(bodyArmorItemCell);
        invCellCmpBodyArmorItem.isEmpty = true;
        invCellCmpBodyArmorItem.cellView = _sceneData.Value.bodyArmorCellView;
        _specialInventoryCellTagsPool.Value.Add(bodyArmorItemCell);
        cells.Add(invCellCmpBodyArmorItem.cellView);

        int helmetItemCell = _world.Value.NewEntity();
        _sceneData.Value.helmetCellView.Construct(helmetItemCell, _world.Value);
        ref var invCellCmpHelmetItem = ref _inventoryCellsComponentsPool.Value.Add(helmetItemCell);
        invCellCmpHelmetItem.isEmpty = true;
        invCellCmpHelmetItem.cellView = _sceneData.Value.helmetCellView;
        _specialInventoryCellTagsPool.Value.Add(helmetItemCell);
        cells.Add(invCellCmpHelmetItem.cellView);
        #endregion


        // _nowUsedWeaponTagsPool.Value.Add(_sceneData.Value.meleeWeaponCellView._entity);

        //5 спец клеток


        this.gameData = dataHandler.Load();
        // this.gameData = null;

        int playerEntity = _sceneData.Value.playerEntity;
        ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(playerEntity);
        ref var invCmp = ref _inventoryComponent.Value.Add(_sceneData.Value.inventoryEntity);
        ref var healthCmp = ref _healthComponentsPool.Value.Get(playerEntity);
        ref var playerCmp = ref _playerComponentsPool.Value.Get(playerEntity);
        healthCmp.healthView = playerCmp.view.healthView;
        healthCmp.healthView.Construct(playerEntity);
        healthCmp.maxHealthPoint = healthCmp.healthView.maxHealth;
        var playerView = playerCmp.view;


        if (this.gameData == null || this.gameData != null && this.gameData.lastGameVersion != Application.version)
        {
            NewGame();
            this.gameData.lastGameVersion = Application.version;
            if (this.gameData.itemsCellinfo == null)
            {

                int meleeCellId = 0;
                int armorCellId = 0;
                for (int i = 0; i < cells.Count; i++)
                {
                    if (cells[i]._entity == meleeCell)
                    {
                        meleeCellId = i;
                        if (armorCellId != 0)
                            break;
                    }
                    else if (cells[i]._entity == bodyArmorItemCell)
                    {
                        armorCellId = i;
                        if (meleeCellId != 0)
                            break;
                    }
                }
                this.gameData.itemsCellinfo = new ItemInfoForSaveData[]
                {
                new ItemInfoForSaveData(25,1, meleeCellId),
                 new ItemInfoForSaveData(90,1, armorCellId)
                };
                this.gameData.durabilityItemsForSaveData = new NumAndIdForSafeData[]
                {
                      new NumAndIdForSafeData(armorCellId, _sceneData.Value.idItemslist.items[90].bodyArmorInfo.armorDurability)
                };
            }

            gameData.currentNightLightIntensity = 0.3f;
            gameData.currentDayTime = 3;
            gameData.changeToRain = true;
            gameData.roundsToWeaterChange = Random.Range(10, 20);
            gameData.playerHP = healthCmp.maxHealthPoint;
            gameData.invCellsCount = _sceneData.Value.dropedItemsUIView.startBackpackInfo.cellsCount;
            gameData.playerHunger = (int)playerView.maxHungerPoints;

            List<NumAndIdForSafeData> weaponsList = new List<NumAndIdForSafeData>();
            foreach (var weapon in _sceneData.Value.idItemslist.items)
                if (weapon != null && (weapon.type == ItemInfo.itemType.gun || weapon.type == ItemInfo.itemType.meleeWeapon))
                    weaponsList.Add(new NumAndIdForSafeData(weapon.itemId, 0));

            gameData.weaponsCurrentExpForSaveData = new NumAndIdForSafeData[weaponsList.Count];
            for (int i = 0; i < weaponsList.Count; i++)
                gameData.weaponsCurrentExpForSaveData[i] = weaponsList[i];

            gameData.shoppersInfoForSafeData = new QuestInfoForSafeData[_sceneData.Value.startShoppers.Length];
            for (int i = 0; i < _sceneData.Value.startShoppers.Length; i++)
            {
                gameData.shoppersInfoForSafeData[i].questNPCId = _sceneData.Value.startShoppers[i];
                ref var needShopper = ref _shopCharacterComponentsPool.Value.Get(_sceneData.Value.interactCharacters[gameData.shoppersInfoForSafeData[i].questNPCId]._entity);

                gameData.shoppersInfoForSafeData[i].currentQuest = needShopper.characterView.startMoneyToBuy;
                gameData.shoppersInfoForSafeData[i].collectedItems = new int[needShopper.characterView.shopItems.Length];
                for (int j = 0; j < needShopper.characterView.shopItems.Length; j++)
                    gameData.shoppersInfoForSafeData[i].collectedItems[j] = needShopper.characterView.shopItems[j].itemsCountToBuy;
                Debug.Log("rem shop items" + gameData.shoppersInfoForSafeData[i].collectedItems.Length);
            }
        }

        for (int i = 0; i < gameData.shoppersInfoForSafeData.Length; i++)
        {
            ref var needShopper = ref _shopCharacterComponentsPool.Value.Get(_sceneData.Value.interactCharacters[gameData.shoppersInfoForSafeData[i].questNPCId]._entity);
            Debug.Log("rem shop items" + gameData.shoppersInfoForSafeData[i].collectedItems.Length);

            needShopper.remainedMoneyToBuy = gameData.shoppersInfoForSafeData[i].currentQuest;
            if (Application.isEditor)
            {
                int needCount = needShopper.characterView.shopItems.Length < gameData.shoppersInfoForSafeData[i].collectedItems.Length ? needShopper.characterView.shopItems.Length : gameData.shoppersInfoForSafeData[i].collectedItems.Length;
                needShopper.remainedShopItems = new int[needShopper.characterView.shopItems.Length];
                for (int j = 0; j < needCount; j++)
                    needShopper.remainedShopItems[j] = gameData.shoppersInfoForSafeData[i].collectedItems[j];
            }
            else
            {
                needShopper.remainedShopItems = new int[gameData.shoppersInfoForSafeData[i].collectedItems.Length];
                for (int j = 0; j < needShopper.remainedShopItems.Length; j++)
                    needShopper.remainedShopItems[j] = gameData.shoppersInfoForSafeData[i].collectedItems[j];
            }
        }

        int gunCount = 0;
        foreach (var weapon in _sceneData.Value.idItemslist.items)
            if (weapon != null && (weapon.type == ItemInfo.itemType.gun || weapon.type == ItemInfo.itemType.meleeWeapon))
                gunCount++;

        if (gunCount != gameData.weaponsCurrentExpForSaveData.Length)
        {
            Dictionary<int, int> curWeaponsExp = new Dictionary<int, int>();
            foreach (var weaponExpInfo in gameData.weaponsCurrentExpForSaveData)
                curWeaponsExp.Add(weaponExpInfo.cellId, weaponExpInfo.num);

            foreach (var weapon in _sceneData.Value.idItemslist.items)
                if (weapon != null && (weapon.type == ItemInfo.itemType.gun || weapon.type == ItemInfo.itemType.meleeWeapon) && !curWeaponsExp.ContainsKey(weapon.itemId))
                {
                    curWeaponsExp.Add(weapon.itemId, 0);
                    Debug.Log("addd gun to exp list" +  weapon.itemId);
                }

            gameData.weaponsCurrentExpForSaveData = new NumAndIdForSafeData[curWeaponsExp.Count];
            int i = 0;
            foreach(var weaponExp in curWeaponsExp)
                gameData.weaponsCurrentExpForSaveData[i++] = new NumAndIdForSafeData(weaponExp.Key, weaponExp.Value);
        }


        _sceneData.Value.dropedItemsUIView.uiScalerSlider.value = this.gameData.currentUIScaleMultiplayer;
        // _sceneData.Value.dropedItemsUIView.uiScalerSlider.value = 0.7f;
        for (int i = 0; i < _sceneData.Value.mainMenuView.uiMenusToScale.Length; i++)
            _sceneData.Value.mainMenuView.uiMenusToScale[i].localScale = Vector3.one * this.gameData.currentUIScaleMultiplayer;

        invCmp.currentCellCount = gameData.invCellsCount;
        for (int i = 0; i < 16; i++)
        {
            var cellEntity = _world.Value.NewEntity();

            _inventoryCellTagsPool.Value.Add(cellEntity).inventoryCellId = i;

            if (i < gameData.invCellsCount)
            {
                ref var inventoryCellsCmp = ref _inventoryCellsComponentsPool.Value.Add(cellEntity);
                inventoryCellsCmp.cellView = _sceneData.Value.GetInventoryCell(i);
                inventoryCellsCmp.cellView.Construct(cellEntity, _world.Value);
                inventoryCellsCmp.isEmpty = true;
                cells.Add(inventoryCellsCmp.cellView);
            }
        }
        cellsCmp.cells = cells;

        ref var playerStats = ref _playerUpgradedStatsPool.Value.Add(_sceneData.Value.playerEntity);
        playerStats.weaponsExp = new Dictionary<int, GunLevelInfoElement>();
        foreach (var weaponExp in gameData.weaponsCurrentExpForSaveData)
        {
            float curExp = weaponExp.num;
            int curLevel = 0;
            while (curExp >= _sceneData.Value.levelExpCounts[curLevel])
                curLevel++;
            playerStats.weaponsExp.Add(weaponExp.cellId, new GunLevelInfoElement((int)curExp, curLevel));
            Debug.Log(weaponExp.cellId + " weapon in exp cmp"+ curExp);
        }

        playerStats.statLevels = new int[3];
        playerStats.currentStatsExp = new float[3];
        for (int i = 0; i < playerStats.statLevels.Length; i++)
        {
            float curExp = gameData.currentStatsExp[i];
            int curLevel = 0;
            while (curExp >= _sceneData.Value.levelExpCounts[curLevel])
                curLevel++;
            playerStats.statLevels[i] = curLevel;
            playerStats.currentStatsExp[i] = curExp;
        }

        invCmp.currentMaxWeight = _sceneData.Value.maxInInventoryWeight + playerStats.statLevels[0] * _sceneData.Value.maxInInventoryWeight / 50f;
        Debug.Log("max inv veidht def " + _sceneData.Value.maxInInventoryWeight + " buffed " + invCmp.currentMaxWeight);
        ref var storageCmp = ref _inventoryComponent.Value.Add(_sceneData.Value.storageEntity);
        storageCmp.currentMaxWeight = _sceneData.Value.maxInStorageWeight;
        storageCmp.currentCellCount = _sceneData.Value.storageCellsCount;
        storageCmp.moneyCount = gameData.moneyInStorage;

        _sceneData.Value.dropedItemsUIView.storageMoneyCountText.text = storageCmp.moneyCount + "$";
        _sceneData.Value.storageInteractView.Construct(_world.Value, _sceneData.Value.storageEntity);
        ref var hidedOBjCmp = ref _hidedObjectOutsideFOVComponentsPool.Value.Add(_sceneData.Value.storageEntity);
        hidedOBjCmp.hidedObjects = _sceneData.Value.storageInteractView.gameObject.GetComponent<HidedOutsidePlayerFovView>().objectsToHide;
        hidedOBjCmp.isSetActiveObject = true;

        int craftingTableEntity = _world.Value.NewEntity();
        _sceneData.Value.craftingTableInteractView.Construct(_world.Value, craftingTableEntity);
        ref var hidedOBjCraftingTableCmp = ref _hidedObjectOutsideFOVComponentsPool.Value.Add(craftingTableEntity);
        hidedOBjCraftingTableCmp.hidedObjects = _sceneData.Value.craftingTableInteractView.gameObject.GetComponent<HidedOutsidePlayerFovView>().objectsToHide;
        hidedOBjCraftingTableCmp.isSetActiveObject = true;


        Dictionary<int, int> durabilityItemsForSaveDataList = new Dictionary<int, int>();
        Dictionary<int, int> bulletsWeaponForSaveDataList = new Dictionary<int, int>();
        Dictionary<int, int> laserPoinerRemainingTimeForSaveDataList = new Dictionary<int, int>();

        Dictionary<int, int> buttGunPartsWithId = new Dictionary<int, int>();
        Dictionary<int, int> scopeGunPartsWithId = new Dictionary<int, int>();
        Dictionary<int, int> downGunPartsWithId = new Dictionary<int, int>();
        Dictionary<int, int> barrelGunPartsWithId = new Dictionary<int, int>();

        Dictionary<int, int> weaponCurrentExpForSaveDataList = new Dictionary<int, int>();

        if (gameData.durabilityItemsForSaveData != null)
            foreach (var item in gameData.durabilityItemsForSaveData)
                durabilityItemsForSaveDataList.Add(item.cellId, item.num);

        if (gameData.bulletsWeaponForSaveData != null)
            foreach (var item in gameData.bulletsWeaponForSaveData)
                bulletsWeaponForSaveDataList.Add(item.cellId, item.num);

        if (gameData.laserPoinerRemainingTimeForSaveData != null)
            foreach (var item in gameData.laserPoinerRemainingTimeForSaveData)
                laserPoinerRemainingTimeForSaveDataList.Add(item.cellId, item.num);

        if (gameData.buttGunPartsForSaveData != null)
            foreach (var item in gameData.buttGunPartsForSaveData)
                buttGunPartsWithId.Add(item.cellId, item.num);

        if (gameData.scopeGunPartsForSaveData != null)
            foreach (var item in gameData.scopeGunPartsForSaveData)
                scopeGunPartsWithId.Add(item.cellId, item.num);

        if (gameData.downGunPartsForSaveData != null)
            foreach (var item in gameData.downGunPartsForSaveData)
                downGunPartsWithId.Add(item.cellId, item.num);

        if (gameData.barrelGunPartsForSaveData != null)
            foreach (var item in gameData.barrelGunPartsForSaveData)
                barrelGunPartsWithId.Add(item.cellId, item.num);


        ref var playerWeaponsInInvCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(playerEntity);

        if (this.gameData.itemsCellinfo != null)
            foreach (var item in this.gameData.itemsCellinfo)
            {
                // if (item.itemId == 0) continue;
                Debug.Log(item.itemCellId + " ned cell id");
                int cellEntity = cellsCmp.cells[item.itemCellId]._entity;
                Debug.Log(cellEntity + " add item from gamedata" + item.itemId);
                ref var itemCmp = ref _inventoryItemComponentsPool.Value.Add(cellEntity);
                ref var invCellCmp = ref _inventoryCellsComponentsPool.Value.Get(cellEntity);
                invCellCmp.isEmpty = false;


                itemCmp.currentItemsCount = item.itemCount;
                itemCmp.itemInfo = _sceneData.Value.idItemslist.items[item.itemId];

                invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                invCellCmp.cellView.ChangeCellItemSprite(itemCmp.itemInfo.itemSprite);

                if (itemCmp.itemInfo.type == ItemInfo.itemType.gun)
                {
                    ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Add(cellEntity);
                    gunInInvCmp.gunPartsId = new int[4];
                    gunInInvCmp.currentGunWeight = itemCmp.itemInfo.itemWeight;
                    float gunPartsWeight = 0;

                    if (buttGunPartsWithId.ContainsKey(item.itemCellId))
                    {
                        gunInInvCmp.gunPartsId[0] = buttGunPartsWithId[item.itemCellId];
                        gunPartsWeight += _sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[0]].itemWeight;
                    }
                    if (scopeGunPartsWithId.ContainsKey(item.itemCellId))
                    {
                        gunInInvCmp.gunPartsId[1] = scopeGunPartsWithId[item.itemCellId];
                        gunPartsWeight += _sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[1]].itemWeight;
                    }
                    if (downGunPartsWithId.ContainsKey(item.itemCellId))
                    {
                        gunInInvCmp.gunPartsId[2] = downGunPartsWithId[item.itemCellId];
                        gunPartsWeight += _sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[2]].itemWeight;
                        if (_sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[2]].gunPartInfo.laserMaxLenght != 0)
                            _laserPointerForGunComponentsPool.Value.Add(cellEntity).remainingLaserPointerTime = laserPoinerRemainingTimeForSaveDataList[item.itemCellId];
                    }
                    if (barrelGunPartsWithId.ContainsKey(item.itemCellId))
                    {
                        gunInInvCmp.gunPartsId[3] = barrelGunPartsWithId[item.itemCellId];
                        gunPartsWeight += _sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[3]].itemWeight;
                    }

                    if (gunPartsWeight != 0)
                        gunInInvCmp.currentGunWeight += gunPartsWeight;


                    //выгрузка айдишников в компонент из сэйва
                    Debug.Log(itemCmp.itemInfo.itemName + " gun " + itemCmp.itemInfo.itemId + "id");
                    gunInInvCmp.currentAmmo = bulletsWeaponForSaveDataList[item.itemCellId];
                    gunInInvCmp.gunDurability = durabilityItemsForSaveDataList[item.itemCellId];

                    gunInInvCmp.currentGunWeight += _sceneData.Value.idItemslist.items[itemCmp.itemInfo.gunInfo.bulletTypeId].itemWeight * gunInInvCmp.currentAmmo;

                    if (cellEntity == _sceneData.Value.firstGunCellView._entity)
                    {
                        Debug.Log("gun added" + cellEntity);
                        playerWeaponsInInvCmp.curEquipedWeaponsCount++;
                        gunInInvCmp.isEquipedWeapon = true;
                    }
                    else if (cellEntity == _sceneData.Value.secondGunCellView._entity)
                    {
                        playerWeaponsInInvCmp.curEquipedWeaponsCount++;
                        gunInInvCmp.isEquipedWeapon = true;
                    }


                    Debug.Log(itemCmp.currentItemsCount + " count " + itemCmp.itemInfo.itemWeight + "kg " + itemCmp.itemInfo.itemId + "id " + cellEntity + "cellEntity");
                }
                else if (itemCmp.itemInfo.type == ItemInfo.itemType.flashlight || itemCmp.itemInfo.type == ItemInfo.itemType.bodyArmor || itemCmp.itemInfo.type == ItemInfo.itemType.helmet)
                {
                    _flashLightInInventoryComponentsPool.Value.Add(cellEntity).currentDurability = durabilityItemsForSaveDataList[item.itemCellId];

                    if (itemCmp.itemInfo.type == ItemInfo.itemType.helmet && itemCmp.itemInfo.helmetInfo.addedLightIntancity != 0)
                        _shieldComponentsPool.Value.Add(cellEntity).currentDurability = durabilityItemsForSaveDataList[item.itemCellId];
                }

                else if (itemCmp.itemInfo.type == ItemInfo.itemType.sheild)
                {
                    _shieldComponentsPool.Value.Add(cellEntity).currentDurability = durabilityItemsForSaveDataList[item.itemCellId];

                    if (cellEntity == _sceneData.Value.shieldCellView._entity)
                    {
                        playerCmp.view.shieldView.shieldSpriteRenderer.sprite = itemCmp.itemInfo.sheildInfo.sheildSprite;
                        playerCmp.view.shieldView.shieldCollider.size = itemCmp.itemInfo.sheildInfo.sheildColliderScale;
                        playerCmp.view.shieldView.shieldCollider.offset = itemCmp.itemInfo.sheildInfo.sheildColliderPositionOffset;
                        playerCmp.view.shieldView.shieldCollider.gameObject.transform.SetParent(playerCmp.view.shieldView.shieldContainer);//playerCmp.view.movementView.nonWeaponContainer
                        playerCmp.view.shieldView.shieldObject.localPosition = Vector2.zero;
                        if (_shieldComponentsPool.Value.Get(cellEntity).currentDurability > 0)//сделать поставку разных спрайтов щита в засимости от его прочности
                            playerCmp.view.shieldView.shieldCollider.enabled = true;
                        playerCmp.view.shieldView.shieldSpriteRenderer.sortingOrder = 2;
                    }
                }

                if (!_storageCellTagsPool.Value.Has(cellEntity))
                {
                    if (itemCmp.itemInfo.type == ItemInfo.itemType.gun)
                        invCmp.weight += _gunInventoryCellComponentsPool.Value.Get(cellEntity).currentGunWeight;
                    else
                        invCmp.weight += itemCmp.itemInfo.itemWeight * itemCmp.currentItemsCount;
                }
                else
                {
                    if (itemCmp.itemInfo.type == ItemInfo.itemType.gun)
                        storageCmp.weight += _gunInventoryCellComponentsPool.Value.Get(cellEntity).currentGunWeight;
                    else
                        storageCmp.weight += itemCmp.itemInfo.itemWeight * itemCmp.currentItemsCount;
                }
            }
        int meleeCellNum = _sceneData.Value.storageCellsCount + 2;
        ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Get(cells[meleeCellNum]._entity);
        Debug.Log(cells[2]._entity + "ent " + _inventoryItemComponentsPool.Value.Has(cells[meleeCellNum]._entity));
        ref var meleeCmp = ref _meleeWeaponComponentsPool.Value.Get(playerEntity);
        ref var curAttackCmp = ref _attackComponentsPool.Value.Get(playerEntity);
        ref var meleeInvCellCmp = ref _inventoryCellsComponentsPool.Value.Get(cells[meleeCellNum]._entity);
        ref var playerMeleeCmp = ref _playerMeleeWeaponComponentsPool.Value.Add(playerEntity);

        _sceneData.Value.statsInventoryText.text = invCmp.weight + "kg/ " + invCmp.currentMaxWeight + "kg \n max cells " + invCmp.currentCellCount;
        _sceneData.Value.statsStorageText.text = storageCmp.weight + "kg/ " + storageCmp.currentMaxWeight + "kg \n max cells " + storageCmp.currentCellCount;

        _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity).moneyCount = this.gameData.moneyInInventory;


        if (this.gameData.itemsCellinfo != null)
        {
            globalTimeCmp.nightLightIntensity = this.gameData.currentNightLightIntensity;
            globalTimeCmp.goToLightNight = this.gameData.goToLightNight;
            globalTimeCmp.levelsToRain = this.gameData.roundsToWeaterChange;
            globalTimeCmp.changedToRain = this.gameData.changeToRain;
            globalTimeCmp.currentDay = this.gameData.currentDay;
            globalTimeCmp.currentDayTime = this.gameData.currentDayTime;

        }

        meleeInvCellCmp.isEmpty = false;
        playerWeaponsInInvCmp.curEquipedWeaponsCount++;
        playerWeaponsInInvCmp.curWeapon = 2;//номер клетки с милишкой
        playerWeaponsInInvCmp.curEquipedWeaponCellEntity = _sceneData.Value.meleeWeaponCellView._entity;
        curAttackCmp.changeWeaponTime = invItemCmp.itemInfo.meleeWeaponInfo.weaponChangeSpeed;
        curAttackCmp.attackCouldown = invItemCmp.itemInfo.meleeWeaponInfo.attackCouldown;
        curAttackCmp.damage = invItemCmp.itemInfo.meleeWeaponInfo.damage;
        meleeCmp.curAttackLenght = invItemCmp.itemInfo.meleeWeaponInfo.attackLenght;
        playerMeleeCmp.weaponInfo = invItemCmp.itemInfo.meleeWeaponInfo;
        curAttackCmp.weaponRotateSpeed = 10f / invItemCmp.itemInfo.itemWeight;

        playerView.movementView.weaponSpriteRenderer.sprite = playerMeleeCmp.weaponInfo.weaponSprite;
        playerView.movementView.weaponSprite.localScale = new Vector3(1, -1, 1) * playerMeleeCmp.weaponInfo.spriteScaleMultiplayer;
        playerView.movementView.weaponSprite.localEulerAngles = new Vector3(0, 0, playerMeleeCmp.weaponInfo.spriteRotation);

        playerView.shieldView._entity = shieldItemCell;

        _sceneData.Value.dropedItemsUIView.gunMagazineUI.gameObject.SetActive(false);
        _sceneData.Value.ammoInfoText.text = "";
        if (this.gameData.questsInfoForSafeData != null)
            foreach (var quest in this.gameData.questsInfoForSafeData)
            {
                ref var questNPC = ref _sceneData.Value.interactCharacters[quest.questNPCId];
                ref var questNPCCmp = ref _questNPCComponentsPool.Value.Get(_sceneData.Value.interactCharacters[quest.questNPCId]._entity);
                questNPCCmp.currentQuest = quest.currentQuest;
                if (quest.collectedItems != null)
                {
                    questNPCCmp.questIsGiven = true;
                    ref var questCmp = ref _questComponentsPool.Value.Add(questNPC._entity);
                    questCmp.curerntCollectedItems = quest.collectedItems;
                    questCmp.questCharacterId = quest.questNPCId;
                }
                else
                    questNPCCmp.questIsGiven = false;
            }

        ref var solarPanerGeneratorCmp = ref _solarPanelElectricGeneratorComponentsPool.Value.Get(playerEntity);
        solarPanerGeneratorCmp.currentElectricityEnergy = gameData.generatorElectricity;
        _sceneData.Value.dropedItemsUIView.solarBatteryenergyText.text = (int)solarPanerGeneratorCmp.currentElectricityEnergy + "mAh/ \n" + _sceneData.Value.solarEnergyGeneratorMaxCapacity + "mAh";

        if (gameData.playerHP <= 0)
            healthCmp.healthPoint = healthCmp.maxHealthPoint / 2;
        else
            healthCmp.healthPoint = gameData.playerHP;
        _sceneData.Value.playerHealthBarFilled.fillAmount = (float)healthCmp.healthPoint / healthCmp.maxHealthPoint;
        _sceneData.Value.playerHealthText.text = healthCmp.healthPoint + " / " + healthCmp.maxHealthPoint;

        int backpackCellEntity = _sceneData.Value.backpackCellView._entity;
        if (_inventoryItemComponentsPool.Value.Has(backpackCellEntity))
        {
            var backpackInfo = _inventoryItemComponentsPool.Value.Get(backpackCellEntity).itemInfo.backpackInfo;
            var invBackground = _sceneData.Value.dropedItemsUIView.inventoryBackground;
            invBackground.sprite = backpackInfo.backgroundSprite;
            invBackground.rectTransform.anchoredPosition = new Vector2(invBackground.rectTransform.anchoredPosition.x, backpackInfo.yPosition);
            invBackground.rectTransform.sizeDelta = backpackInfo.backgroundSize;
        }
        else
        {
            var invBackground = _sceneData.Value.dropedItemsUIView.inventoryBackground;
            invBackground.sprite = _sceneData.Value.dropedItemsUIView.startBackpackInfo.backgroundSprite;
            invBackground.rectTransform.anchoredPosition = new Vector2(invBackground.rectTransform.anchoredPosition.x, _sceneData.Value.dropedItemsUIView.startBackpackInfo.yPosition);
            invBackground.rectTransform.sizeDelta = _sceneData.Value.dropedItemsUIView.startBackpackInfo.backgroundSize;
        }
        if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.helmetCellView._entity))
        {
            var helmetItemInfo = _inventoryItemComponentsPool.Value.Get(_sceneData.Value.helmetCellView._entity).itemInfo.helmetInfo;
            playerView.movementView.helmetSpriteRenderer.sprite = helmetItemInfo.helmetSprite;
            playerView.movementView.helmetSpriteRenderer.transform.localPosition = helmetItemInfo.inGamePositionOnPlayer;
            playerView.movementView.hairSpriteRenderer.sprite = _sceneData.Value.johnHairsSprites[0].sprites[helmetItemInfo.hairSpriteIndex];
            playerCmp.currentAudibility = helmetItemInfo.audibilityMultiplayer;
            if (helmetItemInfo.dropTransparentMultiplayer != 0)
            {
                int curDurability = Mathf.FloorToInt(_flashLightInInventoryComponentsPool.Value.Get(_sceneData.Value.helmetCellView._entity).currentDurability / (helmetItemInfo.armorDurability / 4));
                Debug.Log(curDurability + " cur dur index of helmet");
                if (curDurability != 3 && _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.sprite != _sceneData.Value.dropedItemsUIView.crackedGlassSprites[curDurability])
                {
                    _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.sprite = _sceneData.Value.dropedItemsUIView.crackedGlassSprites[curDurability];
                    _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.gameObject.SetActive(true);
                }
            }

            ref var fOVCmp = ref _fieldOfViewComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            fOVCmp.fieldOfView = playerView.defaultFOV - helmetItemInfo.fowAngleRemove;
            fOVCmp.viewDistance = playerView.viewDistance - helmetItemInfo.fowLenghtRemove;
        }
        else
            playerCmp.currentAudibility = 1;
        if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.bodyArmorCellView._entity))
        {
            var bodyArmorItemInfo = _inventoryItemComponentsPool.Value.Get(_sceneData.Value.bodyArmorCellView._entity).itemInfo.bodyArmorInfo;
            playerView.movementView.bodyArmorSpriteRenderer.sprite = bodyArmorItemInfo.bodyArmorSprite;
            playerView.movementView.bodyArmorSpriteRenderer.transform.localPosition = bodyArmorItemInfo.inGamePositionOnPlayer;
        }
        if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.flashlightItemCellView._entity))
        {
            var itemCmp = _inventoryItemComponentsPool.Value.Get(_sceneData.Value.flashlightItemCellView._entity);
            playerCmp.view.flashLightObject.intensity = itemCmp.itemInfo.flashlightInfo.lightIntecnsity;
            playerCmp.view.flashLightObject.pointLightOuterRadius = itemCmp.itemInfo.flashlightInfo.lightRange;
            playerCmp.view.flashLightObject.color = itemCmp.itemInfo.flashlightInfo.lightColor;
            playerCmp.view.flashLightObject.pointLightInnerAngle = itemCmp.itemInfo.flashlightInfo.spotAngle;
            playerCmp.view.flashLightObject.pointLightOuterAngle = itemCmp.itemInfo.flashlightInfo.spotAngle;
        }
        _craftingTableComponentsPool.Value.Get(_sceneData.Value.playerEntity).craftingTableLevel = gameData.craftingTableLevel;
        if (gameData.craftingTableLevel != 0)
            _sceneData.Value.craftingTableInteractView.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = _sceneData.Value.craftingTablesSprites[gameData.craftingTableLevel - 1];

        ref var playerMoveCmp = ref _playerMoveComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        playerMoveCmp.maxHungerPoints = playerView.maxHungerPoints;
        playerMoveCmp.currentHungerPoints = gameData.playerHunger;
        _sceneData.Value.playerArmorBarFilled.fillAmount = playerMoveCmp.currentHungerPoints / playerMoveCmp.maxHungerPoints;
        _sceneData.Value.playerArmorText.text = playerMoveCmp.currentHungerPoints + " / " + playerMoveCmp.maxHungerPoints;

        //_inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity).moneyCount = 100;
        //globalTimeCmp.currentDayTime = 12;//

        _loadGameEventsPool.Value.Add(dataComponentEntity);
    }
    public enum SavePriority
    {
        fullSave,
        startLocationSave,
        betweenLevelSave
    }
    public void SaveGame(SavePriority savePriority)
    {
        Debug.Log("Game saved");

        int playerEntity = _sceneData.Value.playerEntity;
        var globalTimeCmp = _globalTimeComponentsPool.Value.Get(playerEntity);
        this.gameData.currentDay = globalTimeCmp.currentDay;
        this.gameData.currentDayTime = globalTimeCmp.currentDayTime;
        this.gameData.currentNightLightIntensity = globalTimeCmp.nightLightIntensity;
        this.gameData.goToLightNight = globalTimeCmp.goToLightNight;
        this.gameData.roundsToWeaterChange = globalTimeCmp.levelsToRain;
        this.gameData.changeToRain = globalTimeCmp.changedToRain;

        gameData.generatorElectricity = _solarPanelElectricGeneratorComponentsPool.Value.Get(playerEntity).currentElectricityEnergy;
        gameData.craftingTableLevel = _craftingTableComponentsPool.Value.Get(_sceneData.Value.playerEntity).craftingTableLevel;

        List<QuestInfoForSafeData> questsInfoForSafeData = new List<QuestInfoForSafeData>();
        foreach (var questNPC in _sceneData.Value.interactCharacters)
        {
            if (questNPC._characterType == InteractCharacterView.InteractNPCType.shopAndDialogeNpc || questNPC._characterType == InteractCharacterView.InteractNPCType.gunsmith)
            {
                var questNPCCmp = _questNPCComponentsPool.Value.Get(questNPC._entity);
                if (questNPCCmp.currentQuest == 0 && !questNPCCmp.questIsGiven) continue;
                else
                {
                    Debug.Log("SaveQuest");
                    if (questNPCCmp.questIsGiven)
                        questsInfoForSafeData.Add(new QuestInfoForSafeData(questNPCCmp.characterId, questNPCCmp.currentQuest, _questComponentsPool.Value.Get(questNPC._entity).curerntCollectedItems));
                    else
                        questsInfoForSafeData.Add(new QuestInfoForSafeData(questNPCCmp.characterId, questNPCCmp.currentQuest, null));
                }
            }
        }
        gameData.questsInfoForSafeData = new QuestInfoForSafeData[questsInfoForSafeData.Count];

        for (int i = 0; i < questsInfoForSafeData.Count; i++)
            gameData.questsInfoForSafeData[i] = questsInfoForSafeData[i];

        ref var playerStats = ref _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity);
        for (int i = 0; i < gameData.currentStatsExp.Length; i++)
            gameData.currentStatsExp[i] = playerStats.currentStatsExp[i];

        gameData.weaponsCurrentExpForSaveData = new NumAndIdForSafeData[playerStats.weaponsExp.Count];
        int curIndex = 0;
        foreach (var weaponExp in playerStats.weaponsExp)
        {
            gameData.weaponsCurrentExpForSaveData[curIndex] = new NumAndIdForSafeData(weaponExp.Key, (int)weaponExp.Value.weaponCurrentExp);
            curIndex++;
        }

        if (savePriority == SavePriority.fullSave)
        {
            if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.backpackCellView._entity))
                gameData.invCellsCount = _inventoryItemComponentsPool.Value.Get(_sceneData.Value.backpackCellView._entity).itemInfo.backpackInfo.cellsCount;
            else
                gameData.invCellsCount = 4;
            this.gameData.moneyInInventory = _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity).moneyCount;

            gameData.playerHunger = (int)_playerMoveComponentsPool.Value.Get(_sceneData.Value.playerEntity).currentHungerPoints;
            gameData.playerHP = _healthComponentsPool.Value.Get(playerEntity).healthPoint;
            for (int i = 0; i < _sceneData.Value.startShoppers.Length; i++)
            {
                if (_sceneData.Value.startShoppers[i] == 1) continue;
                // gameData.shoppersInfoForSafeData = new QuestInfoForSafeData[_sceneData.Value.startShoppers.Length];
                //  Debug.Log(_sceneData.Value.interactCharacters[_sceneData.Value.startShoppers[i]]._entity + " need shopper to save");
                gameData.shoppersInfoForSafeData[i].questNPCId = _sceneData.Value.startShoppers[i];
                var needShopper = _shopCharacterComponentsPool.Value.Get(_sceneData.Value.interactCharacters[gameData.shoppersInfoForSafeData[i].questNPCId]._entity);
                gameData.shoppersInfoForSafeData[i].currentQuest = needShopper.remainedMoneyToBuy;

                gameData.shoppersInfoForSafeData[i].collectedItems = new int[needShopper.remainedShopItems.Length];
                for (int j = 0; j < needShopper.remainedShopItems.Length; j++)
                    gameData.shoppersInfoForSafeData[i].collectedItems[j] = needShopper.remainedShopItems[j];
            }
        }
        else if (savePriority == SavePriority.startLocationSave)
        {
            this.gameData.moneyInInventory = 0;
            gameData.playerHunger = (int)_playerMoveComponentsPool.Value.Get(_sceneData.Value.playerEntity).maxHungerPoints;
            gameData.playerHP = _healthComponentsPool.Value.Get(playerEntity).maxHealthPoint / 2;
            gameData.invCellsCount = 4;

            for (int i = 0; i < _sceneData.Value.startShoppers.Length; i++)
            {
                if (_sceneData.Value.startShoppers[i] == 1) continue;
                gameData.shoppersInfoForSafeData[i].questNPCId = _sceneData.Value.startShoppers[i];
                var needShopper = _shopCharacterComponentsPool.Value.Get(_sceneData.Value.interactCharacters[gameData.shoppersInfoForSafeData[i].questNPCId]._entity);
                gameData.shoppersInfoForSafeData[i].currentQuest = needShopper.characterView.startMoneyToBuy;

                gameData.shoppersInfoForSafeData[i].collectedItems = new int[needShopper.remainedShopItems.Length];
                for (int j = 0; j < needShopper.remainedShopItems.Length; j++)
                    gameData.shoppersInfoForSafeData[i].collectedItems[j] = needShopper.characterView.shopItems[j].itemsCountToBuy;
            }
        }
        else
        {
            dataHandler.Save(this.gameData);
            return;
        }
        this.gameData.moneyInStorage = _inventoryComponent.Value.Get(_sceneData.Value.storageEntity).moneyCount;
        //сохранение инвентаря
        List<ItemInfoForSaveData> items = new List<ItemInfoForSaveData>();




        ref var cellsInventory = ref _cellsListComponentsPool.Value.Get(playerEntity).cells;

        List<NumAndIdForSafeData> bulletsWeaponForSaveData = new List<NumAndIdForSafeData>();
        List<NumAndIdForSafeData> durabilityItemsForSaveData = new List<NumAndIdForSafeData>();
        List<NumAndIdForSafeData> laserPointerRemainingTimeForSaveData = new List<NumAndIdForSafeData>();
        //  List<NumAndIdForSafeData> expLevelWeaponForSaveData = new List<NumAndIdForSafeData>();

        List<NumAndIdForSafeData> buttGunPartForSaveData = new List<NumAndIdForSafeData>();
        List<NumAndIdForSafeData> scopeGunPartForSaveData = new List<NumAndIdForSafeData>();
        List<NumAndIdForSafeData> downGunPartForSaveData = new List<NumAndIdForSafeData>();
        List<NumAndIdForSafeData> barrelGunPartForSaveData = new List<NumAndIdForSafeData>();

        for (int i = 0; i < cellsInventory.Count; i++)
        {
            if (_inventoryItemComponentsPool.Value.Has(cellsInventory[i]._entity))
            {
                if (savePriority == SavePriority.startLocationSave && (_inventoryCellTagsPool.Value.Has(cellsInventory[i]._entity) || _specialInventoryCellTagsPool.Value.Has(cellsInventory[i]._entity)))
                {
                    if (cellsInventory[i]._entity == _sceneData.Value.meleeWeaponCellView._entity)
                        items.Add(new ItemInfoForSaveData(25, 1, i));
                    continue;
                }
                var invItemCmp = _inventoryItemComponentsPool.Value.Get(cellsInventory[i]._entity);
                items.Add(new ItemInfoForSaveData(invItemCmp.itemInfo.itemId, invItemCmp.currentItemsCount, i));
                // Debug.Log(invItemCmp.currentItemsCount + "saved items count");
                if (invItemCmp.itemInfo.type == ItemInfo.itemType.gun)
                {
                    ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(cellsInventory[i]._entity);
                    bulletsWeaponForSaveData.Add(new NumAndIdForSafeData(i, gunInInvCmp.currentAmmo));
                    durabilityItemsForSaveData.Add(new NumAndIdForSafeData(i, gunInInvCmp.gunDurability));

                    if (gunInInvCmp.gunPartsId[0] != 0)
                        buttGunPartForSaveData.Add(new NumAndIdForSafeData(i, gunInInvCmp.gunPartsId[0]));
                    if (gunInInvCmp.gunPartsId[1] != 0)
                        scopeGunPartForSaveData.Add(new NumAndIdForSafeData(i, gunInInvCmp.gunPartsId[1]));
                    if (gunInInvCmp.gunPartsId[2] != 0)
                    {
                        downGunPartForSaveData.Add(new NumAndIdForSafeData(i, gunInInvCmp.gunPartsId[2]));
                        if (_sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[2]].gunPartInfo.laserMaxLenght != 0)//сохранение инфы о лазере если он весит на оружии
                            laserPointerRemainingTimeForSaveData.Add(new NumAndIdForSafeData(i, (int)_laserPointerForGunComponentsPool.Value.Get(cellsInventory[i]._entity).remainingLaserPointerTime));
                    }
                    if (gunInInvCmp.gunPartsId[3] != 0)
                        barrelGunPartForSaveData.Add(new NumAndIdForSafeData(i, gunInInvCmp.gunPartsId[3]));

                }
                else if (invItemCmp.itemInfo.type == ItemInfo.itemType.flashlight || invItemCmp.itemInfo.type == ItemInfo.itemType.bodyArmor || invItemCmp.itemInfo.type == ItemInfo.itemType.helmet)
                {
                    durabilityItemsForSaveData.Add(new NumAndIdForSafeData(i, (int)_flashLightInInventoryComponentsPool.Value.Get(cellsInventory[i]._entity).currentDurability));//возможно когда то поправить этот незначительный недочёт
                    if (invItemCmp.itemInfo.type == ItemInfo.itemType.helmet && invItemCmp.itemInfo.helmetInfo.addedLightIntancity != 0)
                        durabilityItemsForSaveData.Add(new NumAndIdForSafeData(i, (int)_shieldComponentsPool.Value.Get(cellsInventory[i]._entity).currentDurability));
                }
                else if (invItemCmp.itemInfo.type == ItemInfo.itemType.sheild)
                    durabilityItemsForSaveData.Add(new NumAndIdForSafeData(i, (int)_shieldComponentsPool.Value.Get(cellsInventory[i]._entity).currentDurability));
                else if (invItemCmp.itemInfo.itemId == 60)//айди дэф китов
                    _playerComponentsPool.Value.Get(playerEntity).canDeffuseMines = true;
                else if (invItemCmp.itemInfo.itemId == 62)//айди дэф китов
                    _playerComponentsPool.Value.Get(playerEntity).hasForestGuide = true;
            }
            else if (savePriority == SavePriority.startLocationSave && cellsInventory[i]._entity == _sceneData.Value.bodyArmorCellView._entity)
            {
                items.Add(new ItemInfoForSaveData(90, 1, i));
                durabilityItemsForSaveData.Add(new NumAndIdForSafeData(i, _sceneData.Value.idItemslist.items[90].bodyArmorInfo.armorDurability));
            }
        }
        this.gameData.itemsCellinfo = new ItemInfoForSaveData[items.Count];
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
        ///////////////////////// Gun parts save
        gameData.buttGunPartsForSaveData = new NumAndIdForSafeData[buttGunPartForSaveData.Count];
        for (int i = 0; i < buttGunPartForSaveData.Count; i++)
            gameData.buttGunPartsForSaveData[i] = buttGunPartForSaveData[i];

        gameData.scopeGunPartsForSaveData = new NumAndIdForSafeData[scopeGunPartForSaveData.Count];
        for (int i = 0; i < scopeGunPartForSaveData.Count; i++)
            gameData.scopeGunPartsForSaveData[i] = scopeGunPartForSaveData[i];

        gameData.downGunPartsForSaveData = new NumAndIdForSafeData[downGunPartForSaveData.Count];
        for (int i = 0; i < downGunPartForSaveData.Count; i++)
            gameData.downGunPartsForSaveData[i] = downGunPartForSaveData[i];

        gameData.barrelGunPartsForSaveData = new NumAndIdForSafeData[barrelGunPartForSaveData.Count];
        for (int i = 0; i < barrelGunPartForSaveData.Count; i++)
            gameData.barrelGunPartsForSaveData[i] = barrelGunPartForSaveData[i];

        gameData.laserPoinerRemainingTimeForSaveData = new NumAndIdForSafeData[laserPointerRemainingTimeForSaveData.Count];
        for (int i = 0; i < laserPointerRemainingTimeForSaveData.Count; i++)
        {
            gameData.laserPoinerRemainingTimeForSaveData[i] = laserPointerRemainingTimeForSaveData[i];
        }

        if (savePriority == SavePriority.startLocationSave)
        {
            dataHandler.Save(this.gameData);
            return;
        }

        this.gameData.currentUIScaleMultiplayer = _sceneData.Value.dropedItemsUIView.uiScalerSlider.value;


        dataHandler.Save(this.gameData);
    }


}
