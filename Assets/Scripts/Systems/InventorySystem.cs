using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class InventorySystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<InventoryComponent> _inventoryComponent;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<InventoryCellComponent> _inventoryCellsComponents;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponent;
    private EcsPoolInject<DroppedItemComponent> _droppedItemComponents;
    private EcsPoolInject<DropItemsIvent> _dropItemsEventsComponent;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<PlayerComponent> _playerTagsPool;
    private EcsPoolInject<ShopCellComponent> _shopCellComponentsPool;
    private EcsPoolInject<EndReloadEvent> _endReloadEventsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<ChangeWeaponFromInventoryEvent> _changeWeaponFromInventoryEventsPool;
    private EcsPoolInject<CurrentAttackComponent> _currentAttackComponentsPool;
    private EcsPoolInject<SpecialInventoryCellTag> _weaponInventoryCellTagsPool;
    private EcsPoolInject<StorageCellTag> _storageCellTagsPool;
    private EcsPoolInject<AddItemFromCellEvent> _addItemFromCellEventsPool;
    private EcsPoolInject<CurrentHealingItemComponent> _currentHealingItemComponentsPool;
    //private EcsPoolInject<HealingItemCellTag> _healingItemCellTagsPool;

    private EcsFilterInject<Inc<ReloadEvent>> _reloadEventsFilter;
    private EcsFilterInject<Inc<InventoryItemComponent>, Exc<StorageCellTag, SpecialInventoryCellTag>> _inventoryItemsFilter;
    private EcsFilterInject<Inc<InventoryItemComponent, StorageCellTag>> _storageItemsFilter;
    private EcsFilterInject<Inc<InventoryCellComponent>, Exc<SpecialInventoryCellTag, StorageCellTag/*, HealingItemCellTag*/>> _inventoryCellsFilter;
    private EcsFilterInject<Inc<InventoryCellComponent, StorageCellTag>, Exc<SpecialInventoryCellTag>> _storageCellsFilter;
    private EcsFilterInject<Inc<AddItemEvent>> _addItemEventsFilter;
    private EcsFilterInject<Inc<AddItemFromCellEvent>, Exc<StorageCellTag>> _addItemToStorageEventsFilter;
    private EcsFilterInject<Inc<AddItemFromCellEvent, StorageCellTag>> _addItemFromStorageEventsFilter;
    private EcsFilterInject<Inc<DropItemsIvent>> _dropItemEventsFilter;
    private EcsFilterInject<Inc<FindAndCellItemEvent>> _findAndCellItemEventsFilter;
    private EcsFilterInject<Inc<BuyItemFromShopEvent>> _buyItemFromShopEventFilter;
    private EcsFilterInject<Inc<MoveSpecialItemToInventoryEvent>> _moveWeaponToInventoryEventsFilter;
    private EcsFilterInject<Inc<HealFromInventoryEvent>> _healFromInventoryEventsFilter;
    private EcsFilterInject<Inc<HealFromHealItemCellEvent>> _healFromHealItemCellEventsFilter;
    //private EcsFilterInject<Inc<MoveWeaponToInventoryEvent>> _moveWeaponToInventoryEventsFilter; удалить этот ивент

    private int inventoryEntity;
    private int storageEntity;
    public void Init(IEcsSystems systems)
    {
        for (int i = 0; i < _sceneData.Value.inventoryCellsCount; i++)
        {
            var cellEntity = _world.Value.NewEntity();

            ref var inventoryCellsCmp = ref _inventoryCellsComponents.Value.Add(cellEntity);
            inventoryCellsCmp.isEmpty = true;
            inventoryCellsCmp.cellView = _sceneData.Value.GetInventoryCell(cellEntity, _world.Value, _sceneData.Value.inventoryCellsContainer);

        }

        for (int i = 0; i < _sceneData.Value.storageCellsCount; i++)
        {
            var cellEntity = _world.Value.NewEntity();

            ref var inventoryCellsCmp = ref _inventoryCellsComponents.Value.Add(cellEntity);
            _storageCellTagsPool.Value.Add(cellEntity);
            inventoryCellsCmp.isEmpty = true;
            inventoryCellsCmp.cellView = _sceneData.Value.GetInventoryCell(cellEntity, _world.Value, _sceneData.Value.storageCellsContainer);

        }

        #region -special cells setup-
        int firstCell = _world.Value.NewEntity();
        _sceneData.Value.firstGunCellView.Construct(firstCell, _world.Value);
        ref var invCellCmpFirstWeapon = ref _inventoryCellsComponents.Value.Add(firstCell);
        invCellCmpFirstWeapon.isEmpty = true;
        invCellCmpFirstWeapon.cellView = _sceneData.Value.firstGunCellView;
        _weaponInventoryCellTagsPool.Value.Add(firstCell);

        int secondCell = _world.Value.NewEntity();
        _sceneData.Value.secondGunCellView.Construct(secondCell, _world.Value);
        ref var invCellCmpSecondWeapon = ref _inventoryCellsComponents.Value.Add(secondCell);
        invCellCmpSecondWeapon.isEmpty = true;
        invCellCmpSecondWeapon.cellView = _sceneData.Value.secondGunCellView;
        _weaponInventoryCellTagsPool.Value.Add(secondCell);

        int meleeCell = _world.Value.NewEntity();
        _sceneData.Value.meleeWeaponCellView.Construct(meleeCell, _world.Value);
        ref var invCellCmpMeleeWeapon = ref _inventoryCellsComponents.Value.Add(meleeCell);
        invCellCmpMeleeWeapon.isEmpty = true;
        invCellCmpMeleeWeapon.cellView = _sceneData.Value.meleeWeaponCellView;
        _weaponInventoryCellTagsPool.Value.Add(meleeCell);

        int healingItemCell = _world.Value.NewEntity();
        _sceneData.Value.healingItemCellView.Construct(healingItemCell, _world.Value);
        ref var invCellCmpHealingItem = ref _inventoryCellsComponents.Value.Add(healingItemCell);
        invCellCmpHealingItem.isEmpty = true;
        invCellCmpHealingItem.cellView = _sceneData.Value.healingItemCellView;
        _weaponInventoryCellTagsPool.Value.Add(healingItemCell);
        #endregion


        inventoryEntity = _world.Value.NewEntity();
        ref var inventoryCmp = ref _inventoryComponent.Value.Add(inventoryEntity);

        storageEntity = _world.Value.NewEntity();
        ref var storageCmp = ref _inventoryComponent.Value.Add(storageEntity);

        //for test

        _sceneData.Value.firstGunCellView.ChangeCellItemSprite(_sceneData.Value.gunItemInfoStarted.itemSprite);

        //_inventoryItemComponent.Value.Get(_sceneData.Value.firstGunCellView._entity);
        ref var invItemCmp = ref _inventoryItemComponent.Value.Add(_sceneData.Value.firstGunCellView._entity);
        ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Add(_sceneData.Value.firstGunCellView._entity);
        ref var gunCmp = ref _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var curAttackCmp = ref _currentAttackComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var playerWeaponsInInvCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(_sceneData.Value.firstGunCellView._entity);

        invCellCmp.isEmpty = false;

        invItemCmp.currentItemsCount = 1;
        _sceneData.Value.firstGunCellView.ChangeCellItemCount(invItemCmp.currentItemsCount);

        invItemCmp.itemInfo = _sceneData.Value.gunItemInfoStarted;
        gunInInvCmp.gunInfo = invItemCmp.itemInfo.gunInfo;
        gunInInvCmp.isEquipedWeapon = true;

        playerWeaponsInInvCmp.gunFirstObject = gunInInvCmp.gunInfo;
        playerWeaponsInInvCmp.curEquipedWeaponsCount++;

        curAttackCmp.changeWeaponTime = invItemCmp.itemInfo.gunInfo.weaponChangeSpeed;
        gunCmp.reloadDuration = invItemCmp.itemInfo.gunInfo.reloadDuration;
        gunCmp.attackLeght = invItemCmp.itemInfo.gunInfo.attackLenght;
        gunCmp.currentMagazineCapacity = invItemCmp.itemInfo.gunInfo.magazineCapacity;//менять на сохранённые
        gunCmp.magazineCapacity = invItemCmp.itemInfo.gunInfo.magazineCapacity;
        gunCmp.currentMaxSpread = invItemCmp.itemInfo.gunInfo.maxSpread;
        gunCmp.scopeMultiplicity = invItemCmp.itemInfo.gunInfo.scopeMultiplicity;
        gunCmp.currentMinSpread = invItemCmp.itemInfo.gunInfo.minSpread;
        gunCmp.currentSpread = invItemCmp.itemInfo.gunInfo.minSpread;
        gunCmp.spreadRecoverySpeed = invItemCmp.itemInfo.gunInfo.spreadRecoverySpeed;
        gunCmp.currentAddedSpread = invItemCmp.itemInfo.gunInfo.addedSpread;
        gunCmp.isAuto = invItemCmp.itemInfo.gunInfo.isAuto;
        gunCmp.bulletCount = invItemCmp.itemInfo.gunInfo.bulletCount;
        gunCmp.bulletTypeId = invItemCmp.itemInfo.gunInfo.bulletTypeId;
        gunCmp.attackCouldown = invItemCmp.itemInfo.gunInfo.attackCouldown;
        curAttackCmp.damage = invItemCmp.itemInfo.gunInfo.damage;
        gunCmp.currentMagazineCapacity = invItemCmp.itemInfo.gunInfo.magazineCapacity;
        gunCmp.isOneBulletReload = invItemCmp.itemInfo.gunInfo.isOneBulletReloaded;

        gunCmp.maxSpread = invItemCmp.itemInfo.gunInfo.maxSpread;
        gunCmp.minSpread = invItemCmp.itemInfo.gunInfo.minSpread;
        gunCmp.addedSpread = invItemCmp.itemInfo.gunInfo.addedSpread;

        //выгрузка айтемов в инвентарь из сохранения

        _sceneData.Value.statsInventoryText.text = inventoryCmp.weight + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
    }
    private void UseHealItem(ref HealthComponent playerHealthComponent, ref CurrentHealingItemComponent healingItemPlayer, int changedCell)
    {
        if (playerHealthComponent.healthPoint != playerHealthComponent.maxHealthPoint && !healingItemPlayer.isHealing)
        {
            ref var invItmCmp = ref _inventoryItemComponent.Value.Get(changedCell);

            healingItemPlayer.isHealing = true;
            healingItemPlayer.healingHealthPoints = invItmCmp.itemInfo.healInfo.healingHealthPoints;
            healingItemPlayer.healingTime = invItmCmp.itemInfo.healInfo.healingTime;

            invItmCmp.currentItemsCount--;
            ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(changedCell);
            if (invItmCmp.currentItemsCount == 0)
            {
                _inventoryItemComponent.Value.Del(changedCell);

                invCellCmp.isEmpty = true;
                invCellCmp.cellView.ClearInventoryCell();
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
            }
            else
            {
                invCellCmp.cellView.ChangeCellItemCount(invItmCmp.currentItemsCount);
                _sceneData.Value.dropedItemsUIView.SetSliderParametrs(invItmCmp.currentItemsCount, changedCell);
            }
        }
    }
    public void Run(IEcsSystems systems)
    {
        foreach (var healFromInvItemEntity in _healFromInventoryEventsFilter.Value)
        {
            var playerHealthComponent = _healthComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            ref var healingItemPlayer = ref _currentHealingItemComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            UseHealItem(ref playerHealthComponent, ref healingItemPlayer, healFromInvItemEntity);
        }
        foreach (var healItemEntity in _healFromHealItemCellEventsFilter.Value)//через H и быстрый слот 
        {
            int cellEntity = _sceneData.Value.healingItemCellView._entity;
            var playerHealthComponent = _healthComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            ref var healingItemPlayer = ref _currentHealingItemComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            UseHealItem(ref playerHealthComponent, ref healingItemPlayer, cellEntity);
            /*if (playerHealthComponent.healthPoint != playerHealthComponent.maxHealthPoint)//_inventoryItemComponent.Value.Has(cellEntity) && 
            {
                ref var invItmCmp = ref _inventoryItemComponent.Value.Get(cellEntity);

                healingItemPlayer.isHealing = true;
                healingItemPlayer.healingHealthPoints = invItmCmp.itemInfo.healInfo.healingHealthPoints;
                healingItemPlayer.healingTime = invItmCmp.itemInfo.healInfo.healingTime;

                invItmCmp.currentItemsCount--;
                ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(cellEntity);
                if (invItmCmp.currentItemsCount == 0)
                {
                    _inventoryItemComponent.Value.Del(cellEntity);

                    invCellCmp.isEmpty = true;
                    invCellCmp.cellView.ClearInventoryCell();
                }
                else
                    invCellCmp.cellView.ChangeCellItemCount(invItmCmp.currentItemsCount);
            }*/
        }
        foreach (var movedToFastCellWeapon in _moveWeaponToInventoryEventsFilter.Value)
        {
            if (_currentHealingItemComponentsPool.Value.Get(_sceneData.Value.playerEntity).isHealing)
                return;
            ref var oldInvCellCmp = ref _inventoryCellsComponents.Value.Get(movedToFastCellWeapon);
            ref var oldInvItemCmp = ref _inventoryItemComponent.Value.Get(movedToFastCellWeapon);
            if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.gun)
            {
                ref var playerWeaponsInInvCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                ref var gunInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(movedToFastCellWeapon);

                if (gunInvCmp.isEquipedWeapon && _sceneData.Value.inventoryCellsCount > _inventoryItemsFilter.Value.GetEntitiesCount() && playerWeaponsInInvCmp.curEquipedWeaponsCount != 1)
                {
                    playerWeaponsInInvCmp.curEquipedWeaponsCount--;
                    Debug.Log(playerWeaponsInInvCmp.curEquipedWeaponsCount + "cur weapons");
                    // кастомное  добавление одного айтема без пибавления веса
                    foreach (var cell in _inventoryCellsFilter.Value)
                    {
                        if (_inventoryCellsComponents.Value.Get(cell).isEmpty)
                        {
                            Debug.Log(_inventoryCellsFilter.Value.GetEntitiesCount() + "take weapon to inv");
                            ref var curInvCellCmp = ref _inventoryCellsComponents.Value.Get(cell);

                            _inventoryItemComponent.Value.Copy(movedToFastCellWeapon, cell);
                            _gunInventoryCellComponentsPool.Value.Copy(movedToFastCellWeapon, cell);

                            ref var itemCmp = ref _inventoryItemComponent.Value.Get(cell);
                            ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(cell);

                            gunInInvCmp.isEquipedWeapon = false;

                            if (_sceneData.Value.firstGunCellView._entity == movedToFastCellWeapon)
                                _changeWeaponFromInventoryEventsPool.Value.Add(cell).SetValues(true, 0);

                            else
                                _changeWeaponFromInventoryEventsPool.Value.Add(cell).SetValues(true, 1);

                            oldInvCellCmp.isEmpty = true;
                            curInvCellCmp.isEmpty = false;
                            curInvCellCmp.inventoryItemComponent = itemCmp;
                            curInvCellCmp.cellView.ChangeCellItemSprite(itemCmp.itemInfo.itemSprite);
                            curInvCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                            //доделать что то
                            Debug.Log("del weapon from fast cells");
                            break;
                        }
                    }
                }
                else if (!gunInvCmp.isEquipedWeapon && (playerWeaponsInInvCmp.gunSecondObject == null || playerWeaponsInInvCmp.gunFirstObject == null))
                {
                    playerWeaponsInInvCmp.curEquipedWeaponsCount++;
                    Debug.Log(playerWeaponsInInvCmp.curEquipedWeaponsCount + "cur weapons");
                    if (playerWeaponsInInvCmp.gunFirstObject == null)
                    {
                        Debug.Log("add 1st weapon to fast cell");
                        int firstGunCellEntity = _sceneData.Value.firstGunCellView._entity;

                        _inventoryItemComponent.Value.Copy(movedToFastCellWeapon, firstGunCellEntity);
                        _gunInventoryCellComponentsPool.Value.Copy(movedToFastCellWeapon, firstGunCellEntity);

                        oldInvCellCmp.isEmpty = true;
                        _inventoryCellsComponents.Value.Get(firstGunCellEntity).isEmpty = false;

                        ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(firstGunCellEntity);

                        gunInInvCmp.isEquipedWeapon = true;

                        ref var curInvCell = ref _inventoryItemComponent.Value.Get(firstGunCellEntity);
                        var curCellView = _sceneData.Value.firstGunCellView;

                        curCellView.ChangeCellItemSprite(curInvCell.itemInfo.itemSprite);
                        curCellView.ChangeCellItemCount(curInvCell.currentItemsCount);
                        _changeWeaponFromInventoryEventsPool.Value.Add(firstGunCellEntity).SetValues(false, 0);
                    }
                    else if (playerWeaponsInInvCmp.gunSecondObject == null)
                    {
                        Debug.Log("add 2nd weapon to fast cell");
                        int secondGunCellEntity = _sceneData.Value.secondGunCellView._entity;

                        _inventoryItemComponent.Value.Copy(movedToFastCellWeapon, secondGunCellEntity);
                        _gunInventoryCellComponentsPool.Value.Copy(movedToFastCellWeapon, secondGunCellEntity);

                        oldInvCellCmp.isEmpty = true;
                        _inventoryCellsComponents.Value.Get(secondGunCellEntity).isEmpty = false;

                        ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(secondGunCellEntity);

                        gunInInvCmp.isEquipedWeapon = true;

                        ref var curInvCell = ref _inventoryItemComponent.Value.Get(secondGunCellEntity);
                        var curCellView = _sceneData.Value.secondGunCellView;

                        curCellView.ChangeCellItemSprite(curInvCell.itemInfo.itemSprite);
                        curCellView.ChangeCellItemCount(curInvCell.currentItemsCount);
                        _changeWeaponFromInventoryEventsPool.Value.Add(secondGunCellEntity).SetValues(false, 1);
                    }
                    _gunInventoryCellComponentsPool.Value.Del(movedToFastCellWeapon);
                }
                else
                {
                    Debug.Log("return null weapon");
                    return;
                }

                _inventoryItemComponent.Value.Del(movedToFastCellWeapon);
                oldInvCellCmp.cellView.ClearInventoryCell();
                oldInvCellCmp.isEmpty = true;
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);

            }
            else if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.meleeWeapon)
            {
                //
            }
            else if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.heal)
            {
                var healItemCellEntity = _sceneData.Value.healingItemCellView._entity;

                ref var invCellForHealItemsCmp = ref _inventoryCellsComponents.Value.Get(healItemCellEntity);
                if (movedToFastCellWeapon == healItemCellEntity)//если убираем из клетки с хилом
                {
                    Debug.Log(oldInvItemCmp.currentItemsCount + " items to add in inv from heal cell");
                    int neededItems = oldInvItemCmp.currentItemsCount - AddItemToInventory(oldInvItemCmp.itemInfo, oldInvItemCmp.currentItemsCount, false);
                    ref var invCmp = ref _inventoryComponent.Value.Get(inventoryEntity);
                    invCmp.weight -= neededItems * oldInvItemCmp.itemInfo.itemWeight;
                    _sceneData.Value.statsInventoryText.text = invCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
                    if (neededItems == oldInvItemCmp.currentItemsCount)
                    {
                        _inventoryItemComponent.Value.Del(movedToFastCellWeapon);
                        oldInvCellCmp.cellView.ClearInventoryCell();
                        oldInvCellCmp.isEmpty = true;
                    }

                    else
                    {
                        oldInvItemCmp.currentItemsCount -= neededItems;
                        oldInvCellCmp.cellView.ChangeCellItemCount(oldInvItemCmp.currentItemsCount);
                    }

                }
                else
                {//перемещение из инвентаря в хил клетку
                    if (_inventoryItemComponent.Value.Has(healItemCellEntity) && _inventoryItemComponent.Value.Get(healItemCellEntity).itemInfo.itemId == oldInvItemCmp.itemInfo.itemId)//если какой то хил айтем уже лежит в клетке
                    {
                        ref var healInvItemCell = ref _inventoryItemComponent.Value.Get(healItemCellEntity);
                        if (healInvItemCell.currentItemsCount == healInvItemCell.itemInfo.maxCount)
                            return;

                        if (oldInvItemCmp.currentItemsCount + healInvItemCell.currentItemsCount > healInvItemCell.itemInfo.maxCount)
                        {
                            oldInvItemCmp.currentItemsCount -= healInvItemCell.itemInfo.maxCount - healInvItemCell.currentItemsCount;
                            healInvItemCell.currentItemsCount = healInvItemCell.itemInfo.maxCount;
                            oldInvCellCmp.cellView.ChangeCellItemCount(oldInvItemCmp.currentItemsCount);
                            invCellForHealItemsCmp.cellView.ChangeCellItemCount(healInvItemCell.currentItemsCount);
                        }
                        else
                        {
                            healInvItemCell.currentItemsCount += oldInvItemCmp.currentItemsCount;
                            oldInvItemCmp.currentItemsCount = 0;
                            invCellForHealItemsCmp.cellView.ChangeCellItemCount(healInvItemCell.currentItemsCount);
                            oldInvCellCmp.cellView.ClearInventoryCell();
                            oldInvCellCmp.isEmpty = true;
                            _inventoryItemComponent.Value.Del(movedToFastCellWeapon);
                        }
                    }
                    else
                    {
                        _inventoryItemComponent.Value.Copy(movedToFastCellWeapon, healItemCellEntity);

                        ref var healInvItemCell = ref _inventoryItemComponent.Value.Get(healItemCellEntity);
                        ref var healInvCellCmp = ref _inventoryCellsComponents.Value.Get(healItemCellEntity);

                        healInvCellCmp.cellView
                            .ChangeCellItemSprite
                            (healInvItemCell.itemInfo.itemSprite);
                        healInvCellCmp.cellView.ChangeCellItemCount(healInvItemCell.currentItemsCount);
                        oldInvCellCmp.cellView.ClearInventoryCell();

                        healInvCellCmp.isEmpty = false;
                        oldInvCellCmp.isEmpty = true;

                        _inventoryItemComponent.Value.Del(movedToFastCellWeapon);
                    }
                    _inventoryItemComponent.Value.Copy(movedToFastCellWeapon, healItemCellEntity);
                }
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
            }
            // if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.gun)
            //     _gunInventoryCellComponentsPool.Value.Del(movedToFastCellWeapon);
            //иначе удалять ближнее
        }

        /* foreach (var movedToInvWeapon in _moveWeaponToInventoryEventsFilter.Value)
         {

         }*/
        #region -default add item-
        foreach (var addedItem in _addItemEventsFilter.Value)//
        {
            ref var dropItem = ref _droppedItemComponents.Value.Get(addedItem);

            if (dropItem.itemInfo.type == ItemInfo.itemType.gun)
            {
                int addedInvCell = -1;
                dropItem.currentItemsCount = AddItemToInventory(dropItem.itemInfo, dropItem.currentItemsCount, ref addedInvCell, false);

                if (addedInvCell != -1)
                {
                    _gunInventoryCellComponentsPool.Value.Copy(addedItem, addedInvCell);
                    _gunInventoryCellComponentsPool.Value.Del(addedItem);
                    Debug.Log(_gunInventoryCellComponentsPool.Value.Get(addedInvCell).currentAmmo + "патроны после взятия");
                }
            }

            else
                dropItem.currentItemsCount = AddItemToInventory(dropItem.itemInfo, dropItem.currentItemsCount, false);
            if (dropItem.currentItemsCount == 0)
            {
                dropItem.droppedItemView.DestroyItemFromGround();
                _droppedItemComponents.Value.Del(addedItem);
            }

            SetMoveSpeedFromWeight();
        }
        #endregion

        #region -add items to storage-
        foreach (var addedItem in _addItemToStorageEventsFilter.Value)//
        {
            ref int curAddedItemsCount = ref _addItemFromCellEventsPool.Value.Get(addedItem).addedItemCount;
            Debug.Log("add item to storage " + curAddedItemsCount);

            ref var deletedInvItemCmp = ref _inventoryItemComponent.Value.Get(addedItem);

            int addedInvCell = -1;

            int startAddedItems = curAddedItemsCount;
            curAddedItemsCount = AddItemToInventory(deletedInvItemCmp.itemInfo, curAddedItemsCount, ref addedInvCell, true);//поменять т,к, вес прибавляется к игроку
            Debug.Log("add inv cell num " + addedInvCell);
            if (addedInvCell != -1)
            {
                ref var inventoryCmp = ref _inventoryComponent.Value.Get(inventoryEntity);

                inventoryCmp.weight -= (startAddedItems - curAddedItemsCount) * deletedInvItemCmp.itemInfo.itemWeight;
                deletedInvItemCmp.currentItemsCount -= startAddedItems - curAddedItemsCount;


                if (deletedInvItemCmp.itemInfo.type == ItemInfo.itemType.gun)
                {
                    _gunInventoryCellComponentsPool.Value.Copy(addedItem, addedInvCell);
                    _gunInventoryCellComponentsPool.Value.Del(addedItem);
                    Debug.Log(_gunInventoryCellComponentsPool.Value.Get(addedInvCell).currentAmmo + "патроны после взятия");
                }


                //   else
                //       dropItem.currentItemsCount = AddItemToInventory(dropItem.itemInfo, dropItem.currentItemsCount);
                if (deletedInvItemCmp.currentItemsCount == 0)
                {
                    _inventoryItemComponent.Value.Del(addedItem);
                    ref var delInvCellCmp = ref _inventoryCellsComponents.Value.Get(addedItem);
                    delInvCellCmp.isEmpty = true;
                    delInvCellCmp.cellView.ClearInventoryCell();
                    _droppedItemComponents.Value.Del(addedItem);
                }
                else
                {
                    _inventoryCellsComponents.Value.Get(addedItem).cellView.ChangeCellItemCount(deletedInvItemCmp.currentItemsCount);
                }

                SetMoveSpeedFromWeight();
            }
        }
        #endregion

        #region -add items from storage-
        foreach (var addedItem in _addItemFromStorageEventsFilter.Value)
        {
            int curAddedItemsCount = _addItemFromCellEventsPool.Value.Get(addedItem).addedItemCount;

            ref var deletedItemFromStorage = ref _inventoryItemComponent.Value.Get(addedItem);

            int addedInvCell = -1;
            Debug.Log("remove item to storage " + curAddedItemsCount);
            int startAddedItems = curAddedItemsCount;
            curAddedItemsCount = AddItemToInventory(deletedItemFromStorage.itemInfo, curAddedItemsCount, ref addedInvCell, false);
            if (addedInvCell != -1)
            {
                ref var inventoryCmp = ref _inventoryComponent.Value.Get(storageEntity);

                inventoryCmp.weight -= (startAddedItems - curAddedItemsCount) * deletedItemFromStorage.itemInfo.itemWeight;
                deletedItemFromStorage.currentItemsCount -= startAddedItems - curAddedItemsCount;

                if (deletedItemFromStorage.itemInfo.type == ItemInfo.itemType.gun)
                {
                    _gunInventoryCellComponentsPool.Value.Copy(addedItem, addedInvCell);
                    _gunInventoryCellComponentsPool.Value.Del(addedItem);
                    Debug.Log(_gunInventoryCellComponentsPool.Value.Get(addedInvCell).currentAmmo + "патроны после взятия");
                }


                //   else
                //       dropItem.currentItemsCount = AddItemToInventory(dropItem.itemInfo, dropItem.currentItemsCount);
                if (deletedItemFromStorage.currentItemsCount == 0)
                {
                    _inventoryItemComponent.Value.Del(addedItem);
                    ref var delInvCellCmp = ref _inventoryCellsComponents.Value.Get(addedItem);
                    delInvCellCmp.isEmpty = true;
                    delInvCellCmp.cellView.ClearInventoryCell();
                    _droppedItemComponents.Value.Del(addedItem);
                }
                else
                {
                    _inventoryCellsComponents.Value.Get(addedItem).cellView.ChangeCellItemCount(deletedItemFromStorage.currentItemsCount);
                }

                SetMoveSpeedFromWeight();
            }
        }
        #endregion
        //поменять внутри верхнего всё

        #region -reload event-
        foreach (var reloadEvent in _reloadEventsFilter.Value)
        {
            ref var gunCmp = ref _gunComponentsPool.Value.Get(reloadEvent);
            int possibleBulletsToReload = FindItemCountInInventory(gunCmp.bulletTypeId);
            if (possibleBulletsToReload == 0)
            {
                gunCmp.currentReloadDuration = 0;
                gunCmp.isReloading = false;
                gunCmp.isContinueReload = false;
                _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
                return;
            }

            else if (gunCmp.isOneBulletReload)
            {
                FindItem(1, gunCmp.bulletTypeId, true);
                gunCmp.bulletCountToReload = 1;
                _endReloadEventsPool.Value.Add(reloadEvent);
                return;
            }

            else if (gunCmp.magazineCapacity - gunCmp.currentMagazineCapacity < possibleBulletsToReload)
                possibleBulletsToReload = gunCmp.magazineCapacity - gunCmp.currentMagazineCapacity;

            FindItem(possibleBulletsToReload, gunCmp.bulletTypeId, true);
            gunCmp.bulletCountToReload = possibleBulletsToReload;
            _endReloadEventsPool.Value.Add(reloadEvent);
        }
        #endregion

        foreach (var droppedItem in _dropItemEventsFilter.Value)
        {
            DropItemsFromInventory(droppedItem);

            SetMoveSpeedFromWeight();
        }

        #region -find and cell items from inventory-
        foreach (var findItem in _findAndCellItemEventsFilter.Value)
        {
            ref var shopCellCmp = ref _shopCellComponentsPool.Value.Get(findItem);

            if (FindItem(shopCellCmp.itemCount, shopCellCmp.itemInfo.itemId, true))
            {
                ref var moneyCmp = ref _playerTagsPool.Value.Get(_sceneData.Value.playerEntity);
                moneyCmp.money += shopCellCmp.itemCost;
                _sceneData.Value.moneyText.text = moneyCmp.money + "$";
                SetMoveSpeedFromWeight();
            }
            //сделать функцию удаления айтемв для очистки кода
        }
        #endregion

        #region -buy and add items to inventory-
        foreach (var buyItem in _buyItemFromShopEventFilter.Value)
        {
            ref var shopCellCmp = ref _shopCellComponentsPool.Value.Get(buyItem);

            ref var playerCmp = ref _playerTagsPool.Value.Get(_sceneData.Value.playerEntity);

            if (playerCmp.money < shopCellCmp.itemCost || !CanAddItems(shopCellCmp.itemInfo, shopCellCmp.itemCount, false)) break;

            AddItemToInventory(shopCellCmp.itemInfo, shopCellCmp.itemCount, false);

            playerCmp.money -= shopCellCmp.itemCost;
            _sceneData.Value.moneyText.text = playerCmp.money + "$";
            SetMoveSpeedFromWeight();
        }
        #endregion
    }


    private bool CanAddItems(ItemInfo addedItemInfo, int addedItemCount, bool toStorage)
    {
        ref var inventoryCmp = ref toStorage ? ref _inventoryComponent.Value.Get(storageEntity) : ref _inventoryComponent.Value.Get(inventoryEntity);//под присмотром, возможны проблемы
        if (_sceneData.Value.maxInInventoryWeight - inventoryCmp.weight < addedItemCount * addedItemInfo.itemWeight) return false;

        int possipleAddedItem = 0;

        if (!toStorage)
        {
            foreach (var invItem in _inventoryItemsFilter.Value)
            {
                var invItemCmp = _inventoryItemComponent.Value.Get(invItem);

                if (invItemCmp.itemInfo.itemId == addedItemInfo.itemId)
                    possipleAddedItem += invItemCmp.itemInfo.maxCount - invItemCmp.currentItemsCount;
            }

            possipleAddedItem += (_sceneData.Value.inventoryCellsCount - _inventoryItemsFilter.Value.GetEntitiesCount()) * addedItemInfo.maxCount;
        }
        else
        {
            foreach (var invItem in _storageItemsFilter.Value)
            {
                var invItemCmp = _inventoryItemComponent.Value.Get(invItem);

                if (invItemCmp.itemInfo.itemId == addedItemInfo.itemId)
                    possipleAddedItem += invItemCmp.itemInfo.maxCount - invItemCmp.currentItemsCount;
            }

            possipleAddedItem += (_sceneData.Value.storageCellsCount - _storageItemsFilter.Value.GetEntitiesCount()) * addedItemInfo.maxCount;
        }

        if (possipleAddedItem < addedItemCount) return false;

        return true;
    }

    private bool FindItem(int neededItemsCount, int neededItemId, bool isDeleteFindedItems)
    {
        int curFindedItems = FindItemCountInInventory(neededItemId);


        if (curFindedItems >= neededItemsCount)
        {
            if (!isDeleteFindedItems) return true;
            ref var inventoryCmp = ref _inventoryComponent.Value.Get(inventoryEntity);

            float itemWeight = 0;
            int startNeededItemsCount = neededItemsCount;
            _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;

            foreach (var invItem in _inventoryItemsFilter.Value)
            {
                ref var invItemCmp = ref _inventoryItemComponent.Value.Get(invItem);
                if (invItemCmp.itemInfo.itemId == neededItemId)
                {
                    if (itemWeight == 0) itemWeight = invItemCmp.itemInfo.itemWeight;

                    ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(invItem);

                    if (invItemCmp.currentItemsCount <= neededItemsCount)
                    {
                        neededItemsCount -= invItemCmp.currentItemsCount;

                        _inventoryItemComponent.Value.Del(invItem);
                        invCellCmp.cellView.ClearInventoryCell();
                        invCellCmp.isEmpty = true;
                        _inventoryItemComponent.Value.Del(invItem);
                        _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                        if (neededItemsCount == 0) break;
                    }

                    else
                    {
                        invItemCmp.currentItemsCount -= neededItemsCount;

                        invCellCmp.cellView.ChangeCellItemCount(invItemCmp.currentItemsCount);
                        _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                        break;
                    }
                }
            }

            inventoryCmp.weight -= startNeededItemsCount * itemWeight;
            _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
            return true;
        }
        return false;
    }

    private int FindItemCountInInventory(int itemId)
    {
        int findedItemsCount = 0;

        foreach (var invItem in _inventoryItemsFilter.Value)
        {
            var invItemCmp = _inventoryItemComponent.Value.Get(invItem);
            if (invItemCmp.itemInfo.itemId == itemId)
                findedItemsCount += invItemCmp.currentItemsCount;
        }

        return findedItemsCount;
    }

    private void SetMoveSpeedFromWeight()
    {
        ref var inventoryCmp = ref _inventoryComponent.Value.Get(inventoryEntity);
        if (inventoryCmp.weight / _sceneData.Value.maxInInventoryWeight > 0.7f)
        {
            var playerMoveCmp = _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            playerMoveCmp.moveSpeed = playerMoveCmp.movementView.moveSpeed - (playerMoveCmp.movementView.moveSpeed * ((inventoryCmp.weight / _sceneData.Value.maxInInventoryWeight) - 0.7f) * 2);
            Debug.Log(playerMoveCmp.moveSpeed);
        }

    }

    private void DropItemsFromInventory(int itemInventoryCell)
    {
        ref var dropEvt = ref _dropItemsEventsComponent.Value.Get(itemInventoryCell);
        ref var invItemCmp = ref _inventoryItemComponent.Value.Get(itemInventoryCell);
        ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(itemInventoryCell);

        bool isStorageCell = _storageCellTagsPool.Value.Has(itemInventoryCell);

        if (!isStorageCell)
        {
            ref var inventoryCmp = ref _inventoryComponent.Value.Get(inventoryEntity);
            inventoryCmp.weight -= dropEvt.itemsCountToDrop * invItemCmp.itemInfo.itemWeight;
            _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
        }
        else
        {
            ref var inventoryCmp = ref _inventoryComponent.Value.Get(storageEntity);
            inventoryCmp.weight -= dropEvt.itemsCountToDrop * invItemCmp.itemInfo.itemWeight;
            _sceneData.Value.statsStorageText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInStorageWeight + "kg \n max cells " + _sceneData.Value.storageCellsCount;
        }

        var droppedItem = _world.Value.NewEntity();

        ref var droppedItemComponent = ref _droppedItemComponents.Value.Add(droppedItem);

        droppedItemComponent.currentItemsCount = dropEvt.itemsCountToDrop;

        droppedItemComponent.itemInfo = invItemCmp.itemInfo;

        var playerView = _playerTagsPool.Value.Get(_sceneData.Value.playerEntity).view;

        droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(playerView.GetPlayerPosition(), _sceneData.Value.testItem1, droppedItem);


        if (dropEvt.itemsCountToDrop == invItemCmp.currentItemsCount)
        {
            if (invItemCmp.itemInfo.type == ItemInfo.itemType.gun)
            {
                _gunInventoryCellComponentsPool.Value.Copy(itemInventoryCell, droppedItem);
                _gunInventoryCellComponentsPool.Value.Del(itemInventoryCell);
                Debug.Log(_gunInventoryCellComponentsPool.Value.Get(droppedItem).currentAmmo + "патроны до дропа");
            }
            _inventoryItemComponent.Value.Del(itemInventoryCell);
            invCellCmp.cellView.ClearInventoryCell();
            invCellCmp.isEmpty = true;
        }
        else
        {
            invItemCmp.currentItemsCount -= dropEvt.itemsCountToDrop;
            invCellCmp.cellView.ChangeCellItemCount(invItemCmp.currentItemsCount);
            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(invItemCmp.currentItemsCount, itemInventoryCell);
        }
    }

    private int AddItemToInventory(ItemInfo itemInfo, int itemsCount, bool toStorage)
    {
        if (!toStorage)
            foreach (var cell in _inventoryCellsFilter.Value)
            {
                ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(cell);
                ref var inventoryCmp = ref _inventoryComponent.Value.Get(inventoryEntity);

                if (_sceneData.Value.maxInInventoryWeight + 0.001f < (inventoryCmp.weight + (itemsCount * itemInfo.itemWeight)))
                {
                    float weightDelta = (inventoryCmp.weight + itemsCount * itemInfo.itemWeight) - _sceneData.Value.maxInInventoryWeight;

                    int neededItemsToFullInventory = itemsCount - Mathf.FloorToInt(weightDelta / itemInfo.itemWeight);

                    if (neededItemsToFullInventory <= 0)
                    {
                        return itemsCount;
                    }

                    AddItemToInventory(itemInfo, neededItemsToFullInventory, toStorage);
                    return itemsCount - neededItemsToFullInventory;
                }

                else if (invCellCmp.isEmpty)
                {
                    ref var itemCmp = ref _inventoryItemComponent.Value.Add(cell);

                    itemCmp.itemInfo = itemInfo;

                    if (itemCmp.itemInfo.type == ItemInfo.itemType.gun)
                    {
                        ref var gunInvCmp = ref _gunInventoryCellComponentsPool.Value.Add(cell);

                        gunInvCmp.isEquipedWeapon = false;
                        gunInvCmp.gunInfo = itemCmp.itemInfo.gunInfo;
                    }



                    invCellCmp.isEmpty = false;
                    invCellCmp.inventoryItemComponent = itemCmp;
                    invCellCmp.cellView.ChangeCellItemSprite(itemCmp.itemInfo.itemSprite);

                    if (itemsCount > itemCmp.itemInfo.maxCount)
                    {
                        int delta = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                        itemCmp.currentItemsCount = itemCmp.itemInfo.maxCount;
                        inventoryCmp.weight += itemCmp.itemInfo.maxCount * itemCmp.itemInfo.itemWeight;
                        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                        _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        return AddItemToInventory(itemInfo, itemsCount - delta, toStorage);
                    }
                    itemCmp.currentItemsCount = itemsCount;
                    inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;
                    invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                    _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
                    //_sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                    return 0;
                }
                else
                {
                    ref var itemCmp = ref _inventoryItemComponent.Value.Get(cell);

                    if (invCellCmp.inventoryItemComponent.itemInfo.itemId == itemInfo.itemId && itemCmp.currentItemsCount != itemCmp.itemInfo.maxCount)
                    {
                        if (itemsCount > itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount)
                        {
                            int deltaCount = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                            itemCmp.currentItemsCount = itemCmp.itemInfo.maxCount;
                            inventoryCmp.weight += deltaCount * itemCmp.itemInfo.itemWeight;
                            _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
                            invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                            return AddItemToInventory(itemInfo, itemsCount - deltaCount, toStorage);
                        }
                        itemCmp.currentItemsCount += itemsCount;
                        inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;

                        _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
                        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        return 0;
                    }
                }

            }
        else
            foreach (var cell in _storageCellsFilter.Value)
            {
                ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(cell);
                ref var inventoryCmp = ref _inventoryComponent.Value.Get(storageEntity);

                if (_sceneData.Value.maxInStorageWeight + 0.001f < (inventoryCmp.weight + (itemsCount * itemInfo.itemWeight)))
                {
                    float weightDelta = (inventoryCmp.weight + itemsCount * itemInfo.itemWeight) - _sceneData.Value.maxInStorageWeight;

                    int neededItemsToFullInventory = itemsCount - Mathf.FloorToInt(weightDelta / itemInfo.itemWeight);

                    if (neededItemsToFullInventory <= 0)
                    {
                        return itemsCount;
                    }

                    AddItemToInventory(itemInfo, neededItemsToFullInventory, toStorage);
                    return itemsCount - neededItemsToFullInventory;
                }

                else if (invCellCmp.isEmpty)
                {
                    ref var itemCmp = ref _inventoryItemComponent.Value.Add(cell);

                    itemCmp.itemInfo = itemInfo;

                    if (itemCmp.itemInfo.type == ItemInfo.itemType.gun)
                    {
                        ref var gunInvCmp = ref _gunInventoryCellComponentsPool.Value.Add(cell);

                        gunInvCmp.isEquipedWeapon = false;
                        gunInvCmp.gunInfo = itemCmp.itemInfo.gunInfo;
                    }



                    invCellCmp.isEmpty = false;
                    invCellCmp.inventoryItemComponent = itemCmp;
                    invCellCmp.cellView.ChangeCellItemSprite(itemCmp.itemInfo.itemSprite);

                    if (itemsCount > itemCmp.itemInfo.maxCount)
                    {
                        int delta = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                        itemCmp.currentItemsCount = itemCmp.itemInfo.maxCount;
                        inventoryCmp.weight += itemCmp.itemInfo.maxCount * itemCmp.itemInfo.itemWeight;
                        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                        _sceneData.Value.statsStorageText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInStorageWeight + "kg \n max cells " + _sceneData.Value.storageCellsCount;
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        return AddItemToInventory(itemInfo, itemsCount - delta, toStorage);
                    }
                    itemCmp.currentItemsCount = itemsCount;
                    inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;
                    invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                    _sceneData.Value.statsStorageText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInStorageWeight + "kg \n max cells " + _sceneData.Value.storageCellsCount;
                    //_sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                    return 0;
                }
                else
                {
                    ref var itemCmp = ref _inventoryItemComponent.Value.Get(cell);

                    if (invCellCmp.inventoryItemComponent.itemInfo.itemId == itemInfo.itemId && itemCmp.currentItemsCount != itemCmp.itemInfo.maxCount)
                    {
                        if (itemsCount > itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount)
                        {
                            int deltaCount = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                            itemCmp.currentItemsCount = itemCmp.itemInfo.maxCount;
                            inventoryCmp.weight += deltaCount * itemCmp.itemInfo.itemWeight;
                            _sceneData.Value.statsStorageText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInStorageWeight + "kg \n max cells " + _sceneData.Value.storageCellsCount;
                            invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                            return AddItemToInventory(itemInfo, itemsCount - deltaCount, toStorage);
                        }
                        itemCmp.currentItemsCount += itemsCount;
                        inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;

                        _sceneData.Value.statsStorageText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInStorageWeight + "kg \n max cells " + _sceneData.Value.storageCellsCount;
                        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        return 0;
                    }
                }

            }
        return itemsCount;
    }

    private int AddItemToInventory(ItemInfo itemInfo, int itemsCount, ref int invCellToAdd, bool toStorage)//работает только для еденичных вещей(оружия напрмер)
    {
        if (!toStorage)
            foreach (var cell in _inventoryCellsFilter.Value)
            {
                invCellToAdd = cell;

                ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(cell);
                ref var inventoryCmp = ref _inventoryComponent.Value.Get(inventoryEntity);

                if (_sceneData.Value.maxInInventoryWeight + 0.001f < (inventoryCmp.weight + (itemsCount * itemInfo.itemWeight)))
                {
                    float weightDelta = (inventoryCmp.weight + itemsCount * itemInfo.itemWeight) - _sceneData.Value.maxInInventoryWeight;

                    int neededItemsToFullInventory = itemsCount - Mathf.FloorToInt(weightDelta / itemInfo.itemWeight);

                    if (neededItemsToFullInventory <= 0)
                    {
                        invCellToAdd = -1;
                        return itemsCount;
                    }
                    //возможна ошибка будет тут и неравильная клетка если будут айтемы с колвом бльше 1
                    AddItemToInventory(itemInfo, neededItemsToFullInventory, ref invCellToAdd, toStorage);
                    return itemsCount - neededItemsToFullInventory;
                }

                else if (invCellCmp.isEmpty)
                {
                    ref var itemCmp = ref _inventoryItemComponent.Value.Add(cell);

                    itemCmp.itemInfo = itemInfo;

                    invCellCmp.isEmpty = false;
                    invCellCmp.inventoryItemComponent = itemCmp;
                    invCellCmp.cellView.ChangeCellItemSprite(itemCmp.itemInfo.itemSprite);

                    if (itemsCount > itemCmp.itemInfo.maxCount)
                    {
                        int delta = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                        itemCmp.currentItemsCount = itemCmp.itemInfo.maxCount;
                        inventoryCmp.weight += itemCmp.itemInfo.maxCount * itemCmp.itemInfo.itemWeight;
                        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                        _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        invCellToAdd = -1;
                        return AddItemToInventory(itemInfo, itemsCount - delta, ref invCellToAdd, toStorage);
                    }
                    itemCmp.currentItemsCount = itemsCount;
                    inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;
                    invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                    _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
                    //_sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                    return 0;
                }
                else
                {
                    ref var itemCmp = ref _inventoryItemComponent.Value.Get(cell);

                    if (invCellCmp.inventoryItemComponent.itemInfo.itemId == itemInfo.itemId && itemCmp.currentItemsCount != itemCmp.itemInfo.maxCount)
                    {
                        if (itemsCount > itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount)
                        {
                            int deltaCount = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                            itemCmp.currentItemsCount = itemCmp.itemInfo.maxCount;
                            inventoryCmp.weight += deltaCount * itemCmp.itemInfo.itemWeight;
                            _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
                            invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                            invCellToAdd = -1;
                            return AddItemToInventory(itemInfo, itemsCount - deltaCount, ref invCellToAdd, toStorage);
                        }
                        itemCmp.currentItemsCount += itemsCount;
                        inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;

                        _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInInventoryWeight + "kg \n max cells " + _sceneData.Value.inventoryCellsCount;
                        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        return 0;
                    }
                }
            }
        else
            foreach (var cell in _storageCellsFilter.Value)
            {
                invCellToAdd = cell;

                ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(cell);
                ref var inventoryCmp = ref _inventoryComponent.Value.Get(storageEntity);

                if (_sceneData.Value.maxInStorageWeight + 0.001f < (inventoryCmp.weight + (itemsCount * itemInfo.itemWeight)))
                {
                    float weightDelta = (inventoryCmp.weight + itemsCount * itemInfo.itemWeight) - _sceneData.Value.maxInStorageWeight;

                    int neededItemsToFullInventory = itemsCount - Mathf.FloorToInt(weightDelta / itemInfo.itemWeight);

                    if (neededItemsToFullInventory <= 0)
                    {
                        invCellToAdd = -1;
                        return itemsCount;
                    }

                    AddItemToInventory(itemInfo, neededItemsToFullInventory, ref invCellToAdd, toStorage);
                    return itemsCount - neededItemsToFullInventory;
                }

                else if (invCellCmp.isEmpty)
                {
                    ref var itemCmp = ref _inventoryItemComponent.Value.Add(cell);

                    itemCmp.itemInfo = itemInfo;

                    if (itemCmp.itemInfo.type == ItemInfo.itemType.gun)
                    {
                        ref var gunInvCmp = ref _gunInventoryCellComponentsPool.Value.Add(cell);

                        gunInvCmp.isEquipedWeapon = false;
                        gunInvCmp.gunInfo = itemCmp.itemInfo.gunInfo;
                    }



                    invCellCmp.isEmpty = false;
                    invCellCmp.inventoryItemComponent = itemCmp;
                    invCellCmp.cellView.ChangeCellItemSprite(itemCmp.itemInfo.itemSprite);

                    if (itemsCount > itemCmp.itemInfo.maxCount)
                    {
                        int delta = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                        itemCmp.currentItemsCount = itemCmp.itemInfo.maxCount;
                        inventoryCmp.weight += itemCmp.itemInfo.maxCount * itemCmp.itemInfo.itemWeight;
                        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                        _sceneData.Value.statsStorageText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInStorageWeight + "kg \n max cells " + _sceneData.Value.storageCellsCount;
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        return AddItemToInventory(itemInfo, itemsCount - delta, ref invCellToAdd, toStorage);
                    }
                    itemCmp.currentItemsCount = itemsCount;
                    inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;
                    invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                    _sceneData.Value.statsStorageText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInStorageWeight + "kg \n max cells " + _sceneData.Value.storageCellsCount;
                    //_sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                    return 0;
                }
                else
                {
                    ref var itemCmp = ref _inventoryItemComponent.Value.Get(cell);

                    if (invCellCmp.inventoryItemComponent.itemInfo.itemId == itemInfo.itemId && itemCmp.currentItemsCount != itemCmp.itemInfo.maxCount)
                    {
                        if (itemsCount > itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount)
                        {
                            int deltaCount = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                            itemCmp.currentItemsCount = itemCmp.itemInfo.maxCount;
                            inventoryCmp.weight += deltaCount * itemCmp.itemInfo.itemWeight;
                            _sceneData.Value.statsStorageText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInStorageWeight + "kg \n max cells " + _sceneData.Value.storageCellsCount;
                            invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                            invCellToAdd = -1;
                            return AddItemToInventory(itemInfo, itemsCount - deltaCount, ref invCellToAdd, toStorage);
                        }
                        itemCmp.currentItemsCount += itemsCount;
                        inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;

                        _sceneData.Value.statsStorageText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxInStorageWeight + "kg \n max cells " + _sceneData.Value.storageCellsCount;
                        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        return 0;
                    }
                }

            }
        return itemsCount;
    }
}
