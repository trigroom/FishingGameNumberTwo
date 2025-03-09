using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<InventoryComponent> _inventoryComponent;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<InventoryCellComponent> _inventoryCellsComponents;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentsPool;
    private EcsPoolInject<DroppedItemComponent> _droppedItemComponents;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<ShopCellComponent> _shopCellComponentsPool;
    private EcsPoolInject<EndReloadEvent> _endReloadEventsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<ChangeWeaponFromInventoryEvent> _changeWeaponFromInventoryEventsPool;
    private EcsPoolInject<StorageCellTag> _storageCellTagsPool;
    private EcsPoolInject<AddItemFromCellEvent> _addItemFromCellEventsPool;
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    private EcsPoolInject<HealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<DurabilityInInventoryComponent> _durabilityInInventoryComponentsPool;
    private EcsPoolInject<SolarPanelElectricGeneratorComponent> _solarPanelElectricGeneratorComponentsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<MenuStatesComponent> _menuStatesComponentsPool;
    private EcsPoolInject<QuestComponent> _questComponentsPool;
    private EcsPoolInject<AttackComponent> _attackComponentsPool;
    private EcsPoolInject<ExplodeGrenadeEvent> _explodeGrenadeEventsPool;
    private EcsPoolInject<GrenadeComponent> _grenadeComponentsPool;
    private EcsPoolInject<DeleteItemEvent> _deleteItemEventsPool;
    private EcsPoolInject<LaserPointerForGunComponent> _laserPointerForGunComponentsPool;
    private EcsPoolInject<PlayerUpgradedStats> _playerUpgradedStatsPool;
    private EcsPoolInject<ShopCharacterComponent> _shopCharacterComponentsPool;
    private EcsPoolInject<CellsListComponent> _cellsListComponentsPool;
    private EcsPoolInject<InventoryCellTag> _inventoryCellTagsPool;
    //private EcsPoolInject<WeaponLevelComponent> _weaponLevelComponentsPool;
    private EcsPoolInject<SecondDurabilityComponent> _shieldComponentsPool;
    private EcsPoolInject<ShowCraftItemRecipeEvent> _showCraftItemRecipeEventsPool;
    private EcsPoolInject<CraftingTableComponent> _craftingTableComponentsPool;
    private EcsPoolInject<BuildingCheckerComponent> _buildingCheckerComponentsPool;
    private EcsPoolInject<FieldOfViewComponent> _fieldOfViewComponentsPool;
    private EcsPoolInject<CurrentInteractedCharactersComponent> _currentInteractedCharactersComponentsPool;
    private EcsPoolInject<QuestNPCComponent> _questNPCComponentsPool;
    private EcsPoolInject<SetInventoryCellsToNewValueEvent> _setInventoryCellsToNewValueEventsPool;
    private EcsPoolInject<HidedObjectOutsideFOVComponent> _hidedObjectOutsideFOVComponentsPool;
    private EcsPoolInject<CameraComponent> _cameraComponentsPool;

    private EcsFilterInject<Inc<ReloadEvent>> _reloadEventsFilter;
    private EcsFilterInject<Inc<InventoryItemComponent>, Exc<StorageCellTag, SpecialInventoryCellTag>> _inventoryItemsFilter;
    private EcsFilterInject<Inc<InventoryItemComponent, StorageCellTag>> _storageItemsFilter;
    private EcsFilterInject<Inc<InventoryCellComponent>, Exc<SpecialInventoryCellTag, StorageCellTag/*, HealingItemCellTag*/>> _inventoryCellsFilter;
    private EcsFilterInject<Inc<InventoryCellComponent, StorageCellTag>, Exc<SpecialInventoryCellTag>> _storageCellsFilter;
    private EcsFilterInject<Inc<AddItemEvent>> _addItemEventsFilter;
    private EcsFilterInject<Inc<AddItemFromCellEvent>, Exc<StorageCellTag>> _addItemToStorageEventsFilter;
    private EcsFilterInject<Inc<AddItemFromCellEvent, StorageCellTag>> _addItemFromStorageEventsFilter;
    private EcsFilterInject<Inc<DeathEvent, PlayerComponent>> _playerDeathEventsFilter { get; set; }
    private EcsFilterInject<Inc<DropItemsIvent>> _dropItemEventsFilter;
    private EcsFilterInject<Inc<FindAndCellItemEvent>> _findAndCellItemEventsFilter;
    private EcsFilterInject<Inc<BuyItemFromShopEvent>> _buyItemFromShopEventFilter;
    private EcsFilterInject<Inc<MoveSpecialItemToInventoryEvent>> _moveWeaponToInventoryEventsFilter;
    private EcsFilterInject<Inc<HealFromInventoryEvent>> _healFromInventoryEventsFilter;
    private EcsFilterInject<Inc<HealFromHealItemCellEvent>> _healFromHealItemCellEventsFilter;
    private EcsFilterInject<Inc<CheckComplitedQuestEvent>> _checkComplitedQuestEventsFilter;
    private EcsFilterInject<Inc<ThrowGrenadeEvent>> _throwGrenadeEventsFilter;
    private EcsFilterInject<Inc<GrenadeComponent>> _grenadeComponentsFilter;
    private EcsFilterInject<Inc<DivideItemEvent>> _divideItemEventsFilter;
    private EcsFilterInject<Inc<LoadGameEvent>> _loadGameEventsFilter;
    private EcsFilterInject<Inc<DeleteItemEvent>> _deleteItemEventsFilter;
    private EcsFilterInject<Inc<ShowCraftItemRecipeEvent>> _showCraftItemRecipeEventsFilter;
    private EcsFilterInject<Inc<TryCraftItemEvent>> _tryCraftItemEventsFilter;
    private EcsFilterInject<Inc<SetInventoryCellsToNewValueEvent>> _setInventoryCellsToNewValueEventsFilter;
    private EcsFilterInject<Inc<SpecialInventoryCellTag, InventoryItemComponent>> _specialItemsFilter;
    private void CopySpecialComponents(int srcEntity, int needEntity, ItemInfo itemInfo)
    {
        if (itemInfo.type == ItemInfo.itemType.gun)
        {
            _gunInventoryCellComponentsPool.Value.Copy(srcEntity, needEntity);
            if (_laserPointerForGunComponentsPool.Value.Has(srcEntity))
                _laserPointerForGunComponentsPool.Value.Copy(srcEntity, needEntity);
        }
        else if (itemInfo.type == ItemInfo.itemType.flashlight || itemInfo.type == ItemInfo.itemType.helmet || itemInfo.type == ItemInfo.itemType.bodyArmor)
        {
            _durabilityInInventoryComponentsPool.Value.Copy(srcEntity, needEntity);
            if (itemInfo.type == ItemInfo.itemType.helmet && itemInfo.helmetInfo.addedLightIntancity != 0)
                _shieldComponentsPool.Value.Copy(srcEntity, needEntity);
        }
        else if (itemInfo.type == ItemInfo.itemType.sheild)
            _shieldComponentsPool.Value.Copy(srcEntity, needEntity);
    }
    private void DelSpecialComponents(int delEntity, ItemInfo itemInfo)
    {
        if (itemInfo.type == ItemInfo.itemType.gun)
        {
            _gunInventoryCellComponentsPool.Value.Del(delEntity);
            if (_laserPointerForGunComponentsPool.Value.Has(delEntity))
                _laserPointerForGunComponentsPool.Value.Del(delEntity);
        }
        else if (itemInfo.type == ItemInfo.itemType.flashlight || itemInfo.type == ItemInfo.itemType.helmet || itemInfo.type == ItemInfo.itemType.bodyArmor)
        {
            _durabilityInInventoryComponentsPool.Value.Del(delEntity);
            if (itemInfo.type == ItemInfo.itemType.helmet && itemInfo.helmetInfo.addedLightIntancity != 0)
                _shieldComponentsPool.Value.Del(delEntity);
        }
        else if (itemInfo.type == ItemInfo.itemType.sheild)
            _shieldComponentsPool.Value.Del(delEntity);
    }
    private void UseHealItem(ref HealthComponent playerHealthComponent, ref HealingItemComponent healingItemPlayer, int changedCell)
    {
        if (/*playerHealthComponent.healthPoint != playerHealthComponent.maxHealthPoint &&*/ !healingItemPlayer.isHealing)
        {
            ref var invItmCmp = ref _inventoryItemComponentsPool.Value.Get(changedCell);

            healingItemPlayer.isHealing = true;
            if (invItmCmp.itemInfo.type == ItemInfo.itemType.heal)
                healingItemPlayer.healingItemInfo = invItmCmp.itemInfo.healInfo;
            else
            {
                if (Random.value < invItmCmp.itemInfo.randomHealInfo.chanceToSafe)
                    healingItemPlayer.healingItemInfo = invItmCmp.itemInfo.randomHealInfo.safeItemInfo.healInfo;
                else
                    healingItemPlayer.healingItemInfo = invItmCmp.itemInfo.randomHealInfo.poisonItemInfo.healInfo;
            }

            if (healingItemPlayer.healingItemInfo.recoveringHungerPoints == 0)
                _sceneData.Value.ammoInfoText.text = "Healing...";
            else
                _sceneData.Value.ammoInfoText.text = "Eating...";

            ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(changedCell);

            if (invItmCmp.currentItemsCount == 1)
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
            else
                _sceneData.Value.dropedItemsUIView.SetSliderParametrs(invItmCmp.currentItemsCount - 1, changedCell);

            ref var movementCmp = ref _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            movementCmp.movementView.weaponSpriteRenderer.sprite = healingItemPlayer.healingItemInfo.inGameHealingItemSprite;
            movementCmp.movementView.weaponSprite.localScale = Vector3.one * healingItemPlayer.healingItemInfo.scaleMultplayer;
            movementCmp.movementView.weaponSprite.localEulerAngles = new Vector3(0, 0, healingItemPlayer.healingItemInfo.rotationZ);

            DeleteItem(ref invItmCmp, ref invCellCmp, 1, changedCell);
        }
    }
    public void Run(IEcsSystems systems)
    {
        foreach (var tryCraftItem in _tryCraftItemEventsFilter.Value)
        {
            ref var craftingTableCmp = ref _craftingTableComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            var idListItems = _sceneData.Value.idItemslist.items;

            for (int i = 0; i < craftingTableCmp.currentCraftingInfo.neededCount.Length; i++)
                FindItem(craftingTableCmp.currentCraftingInfo.neededCount[i], craftingTableCmp.currentCraftingInfo.neededId[i], true, false);
            var inventoryCmp = _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);

            if (craftingTableCmp.currentCraftingInfo.craftedItemId == 999)
            {
                craftingTableCmp.craftingTableLevel = craftingTableCmp.currentCraftingInfo.craftedItemsCount;
                _sceneData.Value.craftingTableInteractView.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = _sceneData.Value.craftingTablesSprites[craftingTableCmp.craftingTableLevel - 1];

                foreach (var craftItemCellView in _sceneData.Value.dropedItemsUIView.craftCells)
                {
                    bool showCraftCell = craftItemCellView.craftRecipeInfo.craftedItemId != 999 && craftingTableCmp.craftingTableLevel >= craftItemCellView.craftRecipeInfo.needCraftingTableLevel || craftItemCellView.craftRecipeInfo.craftedItemId == 999 && craftingTableCmp.craftingTableLevel < craftItemCellView.craftRecipeInfo.craftedItemsCount && craftingTableCmp.craftingTableLevel >= craftItemCellView.craftRecipeInfo.needCraftingTableLevel;
                    craftItemCellView.gameObject.SetActive(showCraftCell);
                    if (showCraftCell)
                    {
                        if (craftItemCellView.craftRecipeInfo.craftedItemId != 999)
                            craftItemCellView.craftCellItemImage.sprite = _sceneData.Value.idItemslist.items[craftItemCellView.craftRecipeInfo.craftedItemId].itemSprite;
                        else
                            craftItemCellView.craftCellItemImage.sprite = _sceneData.Value.craftingTablesSprites[craftItemCellView.craftRecipeInfo.craftedItemsCount - 1];
                    }
                }
            }
            else
            {
                if (_inventoryItemsFilter.Value.GetEntitiesCount() < inventoryCmp.currentCellCount && idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].itemWeight * craftingTableCmp.currentCraftingInfo.craftedItemsCount + inventoryCmp.weight < inventoryCmp.currentMaxWeight)
                {
                    var itemType = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].type;
                    if (itemType == ItemInfo.itemType.gun || itemType == ItemInfo.itemType.flashlight || itemType == ItemInfo.itemType.sheild || itemType == ItemInfo.itemType.helmet || itemType == ItemInfo.itemType.bodyArmor)
                    {
                        var itemsCells = new List<int>();
                        int addedItemsCount;
                        (addedItemsCount, itemsCells) = AddItemToInventoryWithCellNumbers(idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId], craftingTableCmp.currentCraftingInfo.craftedItemsCount, itemsCells, false);
                        if (itemType == ItemInfo.itemType.gun)
                            _gunInventoryCellComponentsPool.Value.Get(itemsCells[0]).gunDurability = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].gunInfo.maxDurabilityPoints;
                        else if (itemType == ItemInfo.itemType.flashlight)
                            _durabilityInInventoryComponentsPool.Value.Get(itemsCells[0]).currentDurability = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].flashlightInfo.maxChargedTime;//mb delete
                        else if (itemType == ItemInfo.itemType.sheild)
                            _shieldComponentsPool.Value.Get(itemsCells[0]).currentDurability = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].sheildInfo.sheildDurability;
                        else if (itemType == ItemInfo.itemType.bodyArmor)
                            _durabilityInInventoryComponentsPool.Value.Get(itemsCells[0]).currentDurability = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].bodyArmorInfo.armorDurability;
                        else if (itemType == ItemInfo.itemType.helmet)
                            _durabilityInInventoryComponentsPool.Value.Get(itemsCells[0]).currentDurability = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].helmetInfo.armorDurability;
                    }
                    else
                    {
                        AddItemToInventory(idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId], craftingTableCmp.currentCraftingInfo.craftedItemsCount, false);
                    }
                }

                else
                {
                    var droppedItemEntity = _world.Value.NewEntity();

                    var itemType = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].type;
                    if (itemType == ItemInfo.itemType.gun || itemType == ItemInfo.itemType.flashlight || itemType == ItemInfo.itemType.sheild || itemType == ItemInfo.itemType.helmet || itemType == ItemInfo.itemType.bodyArmor)
                    {
                        if (itemType == ItemInfo.itemType.gun)
                            _gunInventoryCellComponentsPool.Value.Add(droppedItemEntity).gunDurability = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].gunInfo.maxDurabilityPoints;
                        else if (itemType == ItemInfo.itemType.flashlight)
                            _durabilityInInventoryComponentsPool.Value.Add(droppedItemEntity).currentDurability = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].flashlightInfo.maxChargedTime;
                        else if (itemType == ItemInfo.itemType.sheild)
                            _shieldComponentsPool.Value.Add(droppedItemEntity).currentDurability = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].sheildInfo.sheildDurability;
                        else if (itemType == ItemInfo.itemType.bodyArmor)
                            _durabilityInInventoryComponentsPool.Value.Add(droppedItemEntity).currentDurability = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].bodyArmorInfo.armorDurability;
                        else if (itemType == ItemInfo.itemType.helmet)
                            _durabilityInInventoryComponentsPool.Value.Add(droppedItemEntity).currentDurability = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].helmetInfo.armorDurability;
                    }

                    ref var droppedItemComponent = ref _droppedItemComponents.Value.Add(droppedItemEntity);

                    droppedItemComponent.currentItemsCount = craftingTableCmp.currentCraftingInfo.craftedItemsCount;

                    droppedItemComponent.itemInfo = idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId];

                    droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(_movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform.position, idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId], droppedItemEntity);
                }
            }

            SetMoveSpeedFromWeight();

            for (int i = 0; i < craftingTableCmp.currentCraftingInfo.neededCount.Length; i++)
            {
                if (!FindItem(craftingTableCmp.currentCraftingInfo.neededCount[i], craftingTableCmp.currentCraftingInfo.neededId[i], false, false))
                {
                    _sceneData.Value.dropedItemsUIView.craftItemButton.gameObject.SetActive(false);
                    break;
                }
            }
        }
        foreach (var showCraftRecipe in _showCraftItemRecipeEventsFilter.Value)
        {
            ref var craftingTableCmp = ref _craftingTableComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            craftingTableCmp.currentCraftingInfo = _showCraftItemRecipeEventsPool.Value.Get(showCraftRecipe).craftInfo;
            var idListItems = _sceneData.Value.idItemslist.items;
            if (craftingTableCmp.currentCraftingInfo.craftedItemId != 999)
            {
                _sceneData.Value.dropedItemsUIView.craftedItemRecipeText.text = "<b>" + craftingTableCmp.currentCraftingInfo.craftedItemsCount + " " + idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].itemName + "</b>\n Recipe: \n ";
                craftingTableCmp.currentWeightDelta = craftingTableCmp.currentCraftingInfo.craftedItemsCount * idListItems[craftingTableCmp.currentCraftingInfo.craftedItemId].itemWeight;
            }
            else
            {
                _sceneData.Value.dropedItemsUIView.craftedItemRecipeText.text = "<b> Upgrade crafting table to " + craftingTableCmp.currentCraftingInfo.craftedItemsCount + " Level </b>\n Recipe: \n ";
                craftingTableCmp.currentWeightDelta = 0;
            }

            bool showCraftButton = true;
            for (int i = 0; i < craftingTableCmp.currentCraftingInfo.neededCount.Length; i++)
            {
                if (showCraftButton && !FindItem(craftingTableCmp.currentCraftingInfo.neededCount[i], craftingTableCmp.currentCraftingInfo.neededId[i], false, false))
                    showCraftButton = false;
                craftingTableCmp.currentWeightDelta -= idListItems[craftingTableCmp.currentCraftingInfo.neededId[i]].itemWeight * craftingTableCmp.currentCraftingInfo.neededCount[i];
                _sceneData.Value.dropedItemsUIView.craftedItemRecipeText.text += craftingTableCmp.currentCraftingInfo.neededCount[i] + " " + idListItems[craftingTableCmp.currentCraftingInfo.neededId[i]].itemName + "\n";
            }

            _sceneData.Value.dropedItemsUIView.craftItemButton.gameObject.SetActive(showCraftButton);
        }
        foreach (var loadEvent in _loadGameEventsFilter.Value)
        {
            SetMoveSpeedFromWeight();

        }
        foreach (var divideEventCell in _divideItemEventsFilter.Value)
        {
            ref var currentItem = ref _inventoryItemComponentsPool.Value.Get(divideEventCell);
            //   if (!_storageCellTagsPool.Value.Has(divideEventCell))
            //   {
            bool isDevide = false;
            var needCellFilter = !_storageCellTagsPool.Value.Has(divideEventCell) ? _inventoryCellsFilter.Value : _storageCellsFilter.Value;
            foreach (var cellEntity in needCellFilter)
            {
                ref var cellCmp = ref _inventoryCellsComponents.Value.Get(cellEntity);
                if (cellCmp.isEmpty)
                {
                    ref var itemCmp = ref _inventoryItemComponentsPool.Value.Add(cellEntity);
                    ref var oldItemCmp = ref _inventoryItemComponentsPool.Value.Get(divideEventCell);
                    itemCmp.itemInfo = oldItemCmp.itemInfo;
                    ref var oldCellCmp = ref _inventoryCellsComponents.Value.Get(cellEntity);
                    int neededItemCount = (int)_sceneData.Value.dropedItemsUIView.generalSlider.value;
                    //Debug.Log(neededItemCount + "needed items count");

                    CopySpecialComponents(divideEventCell, cellEntity, oldItemCmp.itemInfo);

                    AddItem(ref cellCmp, ref itemCmp, neededItemCount, cellEntity);
                    _deleteItemEventsPool.Value.Add(divideEventCell).count = neededItemCount;
                    isDevide = true;
                    break;
                }
            }
            if (!isDevide)
                _sceneData.Value.ShowWarningText("there are no free cells to move the " + currentItem.itemInfo.itemName);

            //  }
            /*   else
               {
                   foreach (var cellEntity in _storageCellsFilter.Value)
                   {
                       ref var cellCmp = ref _inventoryCellsComponents.Value.Get(cellEntity);
                       if (cellCmp.isEmpty)
                       {
                           ref var itemCmp = ref _inventoryItemComponentsPool.Value.Add(cellEntity);
                           ref var oldItemCmp = ref _inventoryItemComponentsPool.Value.Get(divideEventCell);
                           itemCmp.itemInfo = oldItemCmp.itemInfo;
                           ref var oldCellCmp = ref _inventoryCellsComponents.Value.Get(cellEntity);
                           int neededItemCount = (int)_sceneData.Value.dropedItemsUIView.generalSlider.value;
                           //  Debug.Log(neededItemCount + "needed items count");

                           CopySpecialComponents(divideEventCell, cellEntity, oldItemCmp.itemInfo);

                           AddItem(ref cellCmp, ref itemCmp, neededItemCount, cellEntity);
                           _deleteItemEventsPool.Value.Add(divideEventCell).count = neededItemCount;
                           break;
                       }
                   }
               }*/
        }
        foreach (var deleteItem in _deleteItemEventsFilter.Value)
        {
            ref var oldItemCmp = ref _inventoryItemComponentsPool.Value.Get(deleteItem);
            ref var oldCellCmp = ref _inventoryCellsComponents.Value.Get(deleteItem);
            DeleteItem(ref oldItemCmp, ref oldCellCmp, _deleteItemEventsPool.Value.Get(deleteItem).count, deleteItem);
            if (!oldCellCmp.isEmpty)
            {
                _sceneData.Value.dropedItemsUIView.SetSliderParametrs(oldItemCmp.currentItemsCount, deleteItem);
                _sceneData.Value.dropedItemsUIView.generalSlider.value = 0;
            }
            else
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
            _deleteItemEventsPool.Value.Del(deleteItem);
        }
        ref var electricityGeneratorCmp = ref _solarPanelElectricGeneratorComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        foreach (var throwGrenade in _throwGrenadeEventsFilter.Value)
        {
            int changedCell = _sceneData.Value.grenadeCellView._entity;
            ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(changedCell);
            ref var invItmCmp = ref _inventoryItemComponentsPool.Value.Get(changedCell);
            var playerMoveCmp = _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            //Debug.Log(playerMoveCmp.movementView.weaponContainer.position + " " + playerMoveCmp.movementView.objectTransform.position);

            var grenade = _sceneData.Value.GetGrenadeObject();

            grenade.gameObject.transform.position = playerMoveCmp.movementView.weaponSpriteRenderer.transform.position;
            grenade.gameObject.transform.rotation = playerMoveCmp.movementView.weaponContainer.rotation;

            grenade.grenadeSprite.sprite = invItmCmp.itemInfo.grenadeInfo.grenadeSprite;

            float distanceToCursor = Vector2.Distance(playerMoveCmp.entityTransform.position, playerMoveCmp.pointToRotateInput);
            if (distanceToCursor > invItmCmp.itemInfo.grenadeInfo.maxThrowDistance)
                distanceToCursor = invItmCmp.itemInfo.grenadeInfo.maxThrowDistance;

            ref var grenadeCmp = ref _grenadeComponentsPool.Value.Add(_world.Value.NewEntity());
            grenadeCmp.grenadeView = grenade;
            grenadeCmp.grenadeView.rigidbodyGrenade = grenade.gameObject.GetComponent<Rigidbody2D>();
            grenadeCmp.grenadeInfo = invItmCmp.itemInfo.grenadeInfo;
            grenadeCmp.currentTimeToExplode = invItmCmp.itemInfo.grenadeInfo.timeToExplode;

            grenadeCmp.grenadeView.rigidbodyGrenade.AddForce(grenade.transform.up * distanceToCursor, ForceMode2D.Impulse);


            DeleteItem(ref invItmCmp, ref invCellCmp, 1, changedCell);
        }
        foreach (var grenadeCmpEntity in _grenadeComponentsFilter.Value)
        {
            //Debug.Log("explode grenade");
            ref var grenadeCmp = ref _grenadeComponentsPool.Value.Get(grenadeCmpEntity);

            if (grenadeCmp.grenadeView.rigidbodyGrenade.linearVelocity.magnitude > 0.01)
                grenadeCmp.grenadeView.rigidbodyGrenade.linearVelocity -= grenadeCmp.grenadeView.rigidbodyGrenade.linearVelocity * Time.deltaTime;
            else
                grenadeCmp.grenadeView.rigidbodyGrenade.linearVelocity = Vector2.zero;
            if (grenadeCmp.currentTimeToExplode > 0)
                grenadeCmp.currentTimeToExplode -= Time.deltaTime;
            else if (grenadeCmp.currentTimeToExplode < 0)
            {
                _explodeGrenadeEventsPool.Value.Add(grenadeCmpEntity);
                grenadeCmp.currentTimeToExplode = 0;
            }

        }
        foreach (var checkQuest in _checkComplitedQuestEventsFilter.Value)
        {
            ref var questCmp = ref _questComponentsPool.Value.Get(checkQuest);
            var questNPC = _sceneData.Value.interactCharacters[questCmp.questCharacterId].GetComponent<QuestCharacterView>();
            ref var questNPCCmp = ref _questNPCComponentsPool.Value.Get(_sceneData.Value.interactCharacters[questCmp.questCharacterId]._entity);
            ref var playerCmp = ref _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            for (int i = 0; i < questNPC.questNode[questNPCCmp.currentQuest].tasks.Length; i++)
            {
                var task = questNPC.questNode[questNPCCmp.currentQuest].tasks[i];
                if (task.questType == QuestNodeElement.QuestType.killSomeone && task.neededCount > questCmp.curerntCollectedItems[i])
                    return;
                else if (task.questType == QuestNodeElement.QuestType.bringSomething && FindItemCountInInventory(task.neededId, false) < task.neededCount)
                    return;
            }

            for (int i = 0; i < questNPC.questNode[questNPCCmp.currentQuest].tasks.Length; i++)
            {
                var task = questNPC.questNode[questNPCCmp.currentQuest].tasks[i];
                if (task.questType == QuestNodeElement.QuestType.bringSomething)
                    FindItem(task.neededCount, task.neededId, true, false);
            }

            for (int i = 0; i < questNPC.questNode[questNPCCmp.currentQuest].rewards.Length; i++)
            {
                var reward = questNPC.questNode[questNPCCmp.currentQuest].rewards[i];
                if (reward.rewardItemId == 0)
                {
                    ref var invCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                    invCmp.moneyCount += reward.rewardItemsCount;
                    _sceneData.Value.moneyText.text = invCmp.moneyCount + " $";
                }
                else
                {
                    var itemInfo = _sceneData.Value.idItemslist.items[reward.rewardItemId];
                    if (CanAddItems(itemInfo, reward.rewardItemsCount, false))
                        AddItemToInventory(itemInfo, reward.rewardItemsCount, false);
                    else
                    {
                        var droppedItem = _world.Value.NewEntity();

                        ref var droppedItemComponent = ref _droppedItemComponents.Value.Add(droppedItem);

                        droppedItemComponent.currentItemsCount = reward.rewardItemsCount;

                        droppedItemComponent.itemInfo = itemInfo;

                        droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(_movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform.position, itemInfo, droppedItem);
                        _hidedObjectOutsideFOVComponentsPool.Value.Add(droppedItem).hidedObjects = new Transform[] { droppedItemComponent.droppedItemView.gameObject.transform.GetChild(0) };


                        TryAddSpecialItemComponent(itemInfo, droppedItem);
                    }
                }
            }

            if (questNPCCmp.currentQuest < questNPC.questNode.Length - 1)
                questNPCCmp.questIsGiven = false;
            _questComponentsPool.Value.Del(checkQuest);
            questNPCCmp.currentQuest++;
        }

        foreach (var playerDeath in _playerDeathEventsFilter.Value)
        {
            var playerPosition = _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform.position;

            if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.backpackCellView._entity))
            {
                ChangeInventoryCellCount(_sceneData.Value.dropedItemsUIView.startBackpackInfo.cellsCount - _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity).currentCellCount);

                var invBackground = _sceneData.Value.dropedItemsUIView.inventoryBackground;
                invBackground.sprite = _sceneData.Value.dropedItemsUIView.startBackpackInfo.backgroundSprite;
                invBackground.rectTransform.anchoredPosition = new Vector2(invBackground.rectTransform.anchoredPosition.x, _sceneData.Value.dropedItemsUIView.startBackpackInfo.yPosition);
                invBackground.rectTransform.sizeDelta = _sceneData.Value.dropedItemsUIView.startBackpackInfo.backgroundSize;
            }

            foreach (var itemInventoryCell in _inventoryItemsFilter.Value)
            {
                ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Get(itemInventoryCell);
                ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(itemInventoryCell);

                Vector2 randomItemPosition = new Vector2(Random.Range(playerPosition.x - 2, playerPosition.x + 2), Random.Range(playerPosition.y - 2, playerPosition.y + 2));

                var droppedItem = _world.Value.NewEntity();

                ref var droppedItemComponent = ref _droppedItemComponents.Value.Add(droppedItem);

                droppedItemComponent.currentItemsCount = invItemCmp.currentItemsCount;

                droppedItemComponent.itemInfo = invItemCmp.itemInfo;

                droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(randomItemPosition, invItemCmp.itemInfo, droppedItem);
                _hidedObjectOutsideFOVComponentsPool.Value.Add(droppedItem).hidedObjects = new Transform[] { droppedItemComponent.droppedItemView.transform.GetChild(0) };
                CopySpecialComponents(itemInventoryCell, droppedItem, invItemCmp.itemInfo);

                DeleteItem(ref invItemCmp, ref invCellCmp, invItemCmp.currentItemsCount, itemInventoryCell);
            }
            ref var playerCmp = ref _playerComponentsPool.Value.Get(playerDeath);
            if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.shieldCellView._entity))
            {
                playerCmp.view.movementView.shieldView.shieldObject.SetParent(playerCmp.view.movementView.shieldView.shieldContainer);
                playerCmp.view.movementView.shieldView.shieldObject.localPosition = Vector3.zero;
                playerCmp.view.movementView.shieldView.shieldObject.localRotation = Quaternion.Euler(0, 0, 0);
                playerCmp.view.movementView.shieldView.shieldObject.gameObject.SetActive(false);
            }
            if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.bodyArmorCellView._entity))
                playerCmp.view.movementView.bodyArmorSpriteRenderer.sprite = _sceneData.Value.transparentSprite;
            if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.helmetCellView._entity))
            {
                playerCmp.view.movementView.hairSpriteRenderer.sprite = _sceneData.Value.johnHairsSprites[0].sprites[1];
                playerCmp.view.movementView.helmetSpriteRenderer.sprite = _sceneData.Value.transparentSprite;
                _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.gameObject.SetActive(false);
                if (playerCmp.nvgIsUsed)
                {
                    playerCmp.nvgIsUsed = false;
                    ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                    if (_buildingCheckerComponentsPool.Value.Get(_sceneData.Value.playerEntity).isHideRoof)
                        _sceneData.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity - 0.35f;
                    else
                        _sceneData.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity;
                    if (globalTimeCmp.currentDayTime >= 15)
                        _sceneData.Value.gloabalLight.color = _sceneData.Value.globalLightColors[2];
                    else if (globalTimeCmp.currentDayTime == 12 || globalTimeCmp.currentDayTime == 0)
                        _sceneData.Value.gloabalLight.color = globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none ? _sceneData.Value.globalLightColors[1] : _sceneData.Value.globalLightColors[4];
                    else
                        _sceneData.Value.gloabalLight.color = globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none ? _sceneData.Value.globalLightColors[0] : _sceneData.Value.globalLightColors[3];
                    _sceneData.Value.bloomMainBg.intensity.value = 0;

                    if (_sceneData.Value.gloabalLight.intensity < 0)
                        _sceneData.Value.gloabalLight.intensity = 0f;
                }
            }
            if (playerCmp.useFlashlight)
            {
                playerCmp.useFlashlight = false;
                playerCmp.view.flashLightObject.gameObject.SetActive(false);
            }


            foreach (var specialItemInventoryCell in _specialItemsFilter.Value)
            {
                if (specialItemInventoryCell == _sceneData.Value.meleeWeaponCellView._entity)
                {
                    ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Get(specialItemInventoryCell);
                    invItemCmp.itemInfo = _sceneData.Value.idItemslist.items[25];
                    _inventoryCellsComponents.Value.Get(specialItemInventoryCell).cellView.ChangeCellItemSprite(invItemCmp.itemInfo.itemSprite);
                }
                else if (specialItemInventoryCell == _sceneData.Value.bodyArmorCellView._entity)
                {
                }
                else
                {
                    ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Get(specialItemInventoryCell);
                    ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(specialItemInventoryCell);

                    Vector2 randomItemPosition = new Vector2(Random.Range(playerPosition.x - 2, playerPosition.x + 2), Random.Range(playerPosition.y - 2, playerPosition.y + 2));

                    var droppedItem = _world.Value.NewEntity();

                    ref var droppedItemComponent = ref _droppedItemComponents.Value.Add(droppedItem);

                    droppedItemComponent.currentItemsCount = invItemCmp.currentItemsCount;

                    droppedItemComponent.itemInfo = invItemCmp.itemInfo;

                    droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(randomItemPosition, invItemCmp.itemInfo, droppedItem);
                    _hidedObjectOutsideFOVComponentsPool.Value.Add(droppedItem).hidedObjects = new Transform[] { droppedItemComponent.droppedItemView.transform.GetChild(0) };
                    if (invItemCmp.itemInfo.type == ItemInfo.itemType.gun)
                        _gunInventoryCellComponentsPool.Value.Get(specialItemInventoryCell).isEquipedWeapon = false;
                    CopySpecialComponents(specialItemInventoryCell, droppedItem, invItemCmp.itemInfo);
                    DeleteItem(ref invItemCmp, ref invCellCmp, invItemCmp.currentItemsCount, specialItemInventoryCell);
                }
            }
            int bodyArmorCellEntity = _sceneData.Value.bodyArmorCellView._entity;
            ref var invArmorCellCmp = ref _inventoryCellsComponents.Value.Get(bodyArmorCellEntity);
            if (_inventoryItemComponentsPool.Value.Has(bodyArmorCellEntity))
            {
                ref var invArmorItemCmp = ref _inventoryItemComponentsPool.Value.Get(bodyArmorCellEntity);
                invArmorItemCmp.itemInfo = _sceneData.Value.idItemslist.items[90];
                _inventoryCellsComponents.Value.Get(bodyArmorCellEntity).cellView.ChangeCellItemSprite(invArmorItemCmp.itemInfo.itemSprite);
                ref var durabilityItemCmp = ref _durabilityInInventoryComponentsPool.Value.Get(bodyArmorCellEntity);

                durabilityItemCmp.currentDurability = _sceneData.Value.idItemslist.items[90].bodyArmorInfo.armorDurability;
            }
            else
            {
                invArmorCellCmp.isEmpty = false;
                ref var invArmorItemCmp = ref _inventoryItemComponentsPool.Value.Add(bodyArmorCellEntity);
                invArmorItemCmp.currentItemsCount = 1;
                invArmorItemCmp.itemInfo = _sceneData.Value.idItemslist.items[90];
                _inventoryCellsComponents.Value.Get(bodyArmorCellEntity).cellView.ChangeCellItemSprite(invArmorItemCmp.itemInfo.itemSprite);
                ref var durabilityItemCmp = ref _durabilityInInventoryComponentsPool.Value.Add(bodyArmorCellEntity);

                durabilityItemCmp.currentDurability = _sceneData.Value.idItemslist.items[90].bodyArmorInfo.armorDurability;
            }

            ref var invCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
            invCmp.weight = _inventoryItemComponentsPool.Value.Get(bodyArmorCellEntity).itemInfo.itemWeight + _inventoryItemComponentsPool.Value.Get(_sceneData.Value.meleeWeaponCellView._entity).itemInfo.itemWeight;
            _sceneData.Value.statsInventoryText.text = invCmp.weight.ToString("0.0") + "kg/ " + invCmp.currentMaxWeight + "kg \n max cells " + invCmp.currentCellCount;

            _changeWeaponFromInventoryEventsPool.Value.Add(playerDeath).SetValues(true, 2);
            _playerWeaponsInInventoryComponentsPool.Value.Get(playerDeath).curEquipedWeaponsCount = 1;

            var weaponInfo = _inventoryItemComponentsPool.Value.Get(_sceneData.Value.meleeWeaponCellView._entity).itemInfo.meleeWeaponInfo;
            ref var moveCmp = ref _movementComponentsPool.Value.Get(playerDeath);
            moveCmp.movementView.weaponSpriteRenderer.sprite = weaponInfo.weaponSprite;
            moveCmp.movementView.weaponSprite.localScale = new Vector3(1, -1, 1) * weaponInfo.spriteScaleMultiplayer;
            moveCmp.movementView.weaponSprite.localEulerAngles = new Vector3(0, 0, weaponInfo.spriteRotation);
            ref var playerGunCmp = ref _playerGunComponentsPool.Value.Get(playerDeath);
            if (playerGunCmp.inScope)
            {

                ref var fieldOfViewCmp = ref _fieldOfViewComponentsPool.Value.Get(playerDeath);
                fieldOfViewCmp.fieldOfView = playerCmp.view.defaultFOV;
                fieldOfViewCmp.viewDistance = playerCmp.view.viewDistance;


                playerGunCmp.inScope = false;

                playerGunCmp.changedInScopeState = false;
                _sceneData.Value.mainCamera.orthographicSize = 5;
                _cameraComponentsPool.Value.Get(playerDeath).playerPositonPart = 2;
                moveCmp.moveSpeed *= playerGunCmp.currentScopeMultiplicity;

                _sceneData.Value.dropedItemsUIView.scopeCrossCentreImage.gameObject.SetActive(false);
            }
        }

        foreach (var usedItemEntity in _healFromInventoryEventsFilter.Value)
        {
            var itemCmp = _inventoryItemComponentsPool.Value.Get(usedItemEntity);

            var playerAttackCmp = _attackComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (_currentHealingItemComponentsPool.Value.Get(_sceneData.Value.playerEntity).isHealing || playerAttackCmp.weaponIsChanged)
                return;

            if (itemCmp.itemInfo.type == ItemInfo.itemType.heal || itemCmp.itemInfo.type == ItemInfo.itemType.randomHeal)
            {
                var playerHealthComponent = _healthComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                ref var healingItemPlayer = ref _currentHealingItemComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                UseHealItem(ref playerHealthComponent, ref healingItemPlayer, usedItemEntity);
            }
            else if (itemCmp.itemInfo.type == ItemInfo.itemType.addedExpItem)
            {
                ref var playerStatsCmp = ref _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity);
                int statId = itemCmp.itemInfo.expAddedInfo.statId;
                playerStatsCmp.currentStatsExp[statId] += itemCmp.itemInfo.expAddedInfo.addedExpCount;
                ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(usedItemEntity);
                if (itemCmp.currentItemsCount == 1)
                    _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                else
                    _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, usedItemEntity);
                while (playerStatsCmp.currentStatsExp[statId] >= _sceneData.Value.levelExpCounts[playerStatsCmp.statLevels[statId]])
                    playerStatsCmp.statLevels[statId]++;

                _sceneData.Value.dropedItemsUIView.statsFilledBarsText[statId].text = (int)playerStatsCmp.currentStatsExp[statId] + "/" + _sceneData.Value.levelExpCounts[playerStatsCmp.statLevels[statId]] + " lvl:" + playerStatsCmp.statLevels[statId];
                _sceneData.Value.dropedItemsUIView.statsFilledBarsImages[statId].fillAmount = playerStatsCmp.currentStatsExp[statId] / _sceneData.Value.levelExpCounts[playerStatsCmp.statLevels[statId]];
                _deleteItemEventsPool.Value.Add(usedItemEntity).count = 1;
                if (statId == 0)
                    _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity).currentMaxWeight = _sceneData.Value.maxInInventoryWeight + playerStatsCmp.statLevels[0] * _sceneData.Value.maxInInventoryWeight / 50f;


                //  Debug.Log(itemCmp.currentItemsCount);
                return;
            }
            else if (itemCmp.itemInfo.type == ItemInfo.itemType.flashlight)
            {
                ref var flashlightCmp = ref _durabilityInInventoryComponentsPool.Value.Get(usedItemEntity);
                if (flashlightCmp.currentDurability != itemCmp.itemInfo.flashlightInfo.maxChargedTime)
                {
                    if (itemCmp.itemInfo.flashlightInfo.isElectric)
                    {
                        float needEnergy = electricityGeneratorCmp.currentElectricityEnergy - (float)itemCmp.itemInfo.flashlightInfo.chargeItem * (1 - flashlightCmp.currentDurability / itemCmp.itemInfo.flashlightInfo.maxChargedTime);
                        if (needEnergy > 0)
                        {
                            flashlightCmp.currentDurability = itemCmp.itemInfo.flashlightInfo.maxChargedTime;
                            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                            electricityGeneratorCmp.currentElectricityEnergy = needEnergy;
                            _sceneData.Value.dropedItemsUIView.solarBatteryenergyText.text = (int)electricityGeneratorCmp.currentElectricityEnergy + "mAh/ \n" + _sceneData.Value.solarEnergyGeneratorMaxCapacity + "mAh";
                        }
                        else
                            _sceneData.Value.ShowWarningText("need " + (needEnergy * -1) + "more mA to charge item");
                    }
                    else if (!itemCmp.itemInfo.flashlightInfo.isElectric)
                    {
                        if ((_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inStorageState && FindItem(1, itemCmp.itemInfo.flashlightInfo.chargeItem, true, true)) || FindItem(1, itemCmp.itemInfo.flashlightInfo.chargeItem, true, false))
                        {
                            flashlightCmp.currentDurability = itemCmp.itemInfo.flashlightInfo.maxChargedTime;
                            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                        }
                        else
                            _sceneData.Value.ShowWarningText("need " + _sceneData.Value.idItemslist.items[itemCmp.itemInfo.flashlightInfo.chargeItem] + " to charge");
                    }

                }
            }
            else if (itemCmp.itemInfo.type == ItemInfo.itemType.bodyArmor)
            {
                ref var durabilityCmp = ref _durabilityInInventoryComponentsPool.Value.Get(usedItemEntity);
                ref var inventoryCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                if (inventoryCmp.moneyCount < itemCmp.itemInfo.bodyArmorInfo.recoveryCost)
                {
                    _sceneData.Value.ShowWarningText("need another " + (itemCmp.itemInfo.bodyArmorInfo.recoveryCost - inventoryCmp.moneyCount) + "$ to recovery armor");
                    return;
                }
                if (durabilityCmp.currentDurability == itemCmp.itemInfo.bodyArmorInfo.armorDurability)
                {
                    _sceneData.Value.ShowWarningText("armor durability is full");
                    return;
                }

                inventoryCmp.moneyCount -= itemCmp.itemInfo.bodyArmorInfo.recoveryCost;
                durabilityCmp.currentDurability = itemCmp.itemInfo.bodyArmorInfo.armorDurability;
                _sceneData.Value.moneyText.text = inventoryCmp.moneyCount + "$";
            }
            else if (itemCmp.itemInfo.type == ItemInfo.itemType.helmet)
            {
                ref var inventoryCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                if (_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inGunWorkshopState)
                {
                    ref var durabilityCmp = ref _durabilityInInventoryComponentsPool.Value.Get(usedItemEntity);
                    // if ((inventoryCmp.moneyCount - itemCmp.itemInfo.helmetInfo.recoveryCost) < 0 && durabilityCmp.currentDurability == itemCmp.itemInfo.helmetInfo.armorDurability) return;

                    if (inventoryCmp.moneyCount < itemCmp.itemInfo.helmetInfo.recoveryCost)
                    {
                        _sceneData.Value.ShowWarningText("need another " + (itemCmp.itemInfo.helmetInfo.recoveryCost - inventoryCmp.moneyCount) + "$ to recovery helmet");
                        return;
                    }
                    if (durabilityCmp.currentDurability == itemCmp.itemInfo.helmetInfo.armorDurability)
                    {
                        _sceneData.Value.ShowWarningText("helmet durability is full");
                        return;
                    }

                    inventoryCmp.moneyCount -= itemCmp.itemInfo.helmetInfo.recoveryCost;
                    durabilityCmp.currentDurability = itemCmp.itemInfo.helmetInfo.armorDurability;
                    _sceneData.Value.moneyText.text = inventoryCmp.moneyCount + "$";

                    if (usedItemEntity == _sceneData.Value.helmetCellView._entity && itemCmp.itemInfo.helmetInfo.dropTransparentMultiplayer != 0)
                        _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.gameObject.SetActive(false);
                }
                else
                {
                    ref var energyCmp = ref _shieldComponentsPool.Value.Get(usedItemEntity);
                    float needEnergy = electricityGeneratorCmp.currentElectricityEnergy - itemCmp.itemInfo.helmetInfo.nightTimeModeDuration * (1 - energyCmp.currentDurability / itemCmp.itemInfo.helmetInfo.nightTimeModeDuration);
                    if (needEnergy > 0)
                    {
                        energyCmp.currentDurability = itemCmp.itemInfo.helmetInfo.nightTimeModeDuration;
                        electricityGeneratorCmp.currentElectricityEnergy = needEnergy;
                        _sceneData.Value.dropedItemsUIView.solarBatteryenergyText.text = (int)electricityGeneratorCmp.currentElectricityEnergy + "mAh/ \n" + _sceneData.Value.solarEnergyGeneratorMaxCapacity + "mAh";
                    }
                    else
                        _sceneData.Value.ShowWarningText("need " + (needEnergy * -1) + "more mA to charge nvg");
                }
            }
            else if (itemCmp.itemInfo.type == ItemInfo.itemType.sheild)
            {
                if (_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inGunWorkshopState)
                {
                    ref var inventoryCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                    ref var sheildCmp = ref _shieldComponentsPool.Value.Get(usedItemEntity);
                    if ((inventoryCmp.moneyCount - itemCmp.itemInfo.sheildInfo.sheildRecoveryCost) < 0 && sheildCmp.currentDurability == itemCmp.itemInfo.sheildInfo.sheildDurability) return;

                    if (inventoryCmp.moneyCount < itemCmp.itemInfo.sheildInfo.sheildRecoveryCost)
                    {
                        _sceneData.Value.ShowWarningText("need another " + (itemCmp.itemInfo.sheildInfo.sheildRecoveryCost - inventoryCmp.moneyCount) + "$ to recovery shield");
                        return;
                    }
                    if (sheildCmp.currentDurability == itemCmp.itemInfo.sheildInfo.sheildDurability)
                    {
                        _sceneData.Value.ShowWarningText("shield durability is full");
                        return;
                    }

                    if (usedItemEntity == _sceneData.Value.shieldCellView._entity && sheildCmp.currentDurability == 0)
                        _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view.movementView.shieldView.shieldCollider.enabled = true;

                    inventoryCmp.moneyCount -= itemCmp.itemInfo.sheildInfo.sheildRecoveryCost;
                    sheildCmp.currentDurability = itemCmp.itemInfo.sheildInfo.sheildDurability;
                    _sceneData.Value.moneyText.text = inventoryCmp.moneyCount + "$";

                }
            }
            else if (itemCmp.itemInfo.type == ItemInfo.itemType.gun)
            {
                ref var gunInInvCellCmp = ref _gunInventoryCellComponentsPool.Value.Get(usedItemEntity);
                var menuStatesCmp = _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                if (menuStatesCmp.inStorageState)
                {
                    ref var laserPointerCmp = ref _laserPointerForGunComponentsPool.Value.Get(usedItemEntity);
                    var laserGunPartInfo = _sceneData.Value.idItemslist.items[gunInInvCellCmp.gunPartsId[2]].gunPartInfo;
                    if (laserPointerCmp.remainingLaserPointerTime == laserGunPartInfo.laserLightTime)
                    {
                        _sceneData.Value.ShowWarningText("laser pointer charge is full");
                        break;
                    }

                    float needEnergy = electricityGeneratorCmp.currentElectricityEnergy - laserGunPartInfo.energyToCharge * (1 - laserPointerCmp.remainingLaserPointerTime / laserGunPartInfo.laserLightTime);

                    // Debug.Log(needEnergy + " needenergy");
                    if (needEnergy > 0)
                    {
                        laserPointerCmp.remainingLaserPointerTime = laserGunPartInfo.laserLightTime;
                        electricityGeneratorCmp.currentElectricityEnergy = needEnergy;
                        _sceneData.Value.dropedItemsUIView.solarBatteryenergyText.text = (int)electricityGeneratorCmp.currentElectricityEnergy + "mAh/ \n" + _sceneData.Value.solarEnergyGeneratorMaxCapacity + "mAh";
                    }
                    else
                        _sceneData.Value.ShowWarningText("need " + (needEnergy * -1) + "more mA to charge nvg");
                }
                else if (menuStatesCmp.inGunWorkshopState)
                {
                    var upgradedGun = _sceneData.Value.idItemslist.items[itemCmp.itemInfo.gunInfo.upgradedGunId];
                    ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(usedItemEntity);
                    ref var playerCmp = ref _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                    var invCmp = _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                    // Debug.Log((upgradedGun.itemWeight - itemCmp.itemInfo.itemWeight + _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity).weight > _sceneData.Value.maxInInventoryWeight || (playerCmp.money - itemCmp.itemInfo.gunInfo.upgradeCost) < 0) + "Upg gun");
                    if ((invCmp.moneyCount - itemCmp.itemInfo.gunInfo.upgradeCost) < 0)
                    {
                        _sceneData.Value.ShowWarningText("need another " + (itemCmp.itemInfo.gunInfo.upgradeCost - invCmp.moneyCount) + "$ to upgrade Gun");
                        return;
                    }
                    if (upgradedGun.itemWeight - _gunInventoryCellComponentsPool.Value.Get(usedItemEntity).currentGunWeight + invCmp.weight > invCmp.currentMaxWeight)
                    {
                        _sceneData.Value.ShowWarningText("the new gun is so heavy");
                        return;
                    }
                    invCmp.moneyCount -= itemCmp.itemInfo.gunInfo.upgradeCost;
                    DeleteItem(ref itemCmp, ref invCellCmp, 1, usedItemEntity);
                    ref var upgradedWeaponItemCmp = ref _inventoryItemComponentsPool.Value.Add(usedItemEntity);
                    upgradedWeaponItemCmp.itemInfo = upgradedGun;
                    invCellCmp.isEmpty = false;
                    //  invCellCmp.cellView.inventoryCellButton.enabled = true;
                    AddItem(ref invCellCmp, ref upgradedWeaponItemCmp, 1, usedItemEntity);
                    TryAddSpecialItemComponent(itemCmp.itemInfo, usedItemEntity);
                    gunInInvCellCmp.isEquipedWeapon = true;
                    gunInInvCellCmp.gunDurability = upgradedGun.gunInfo.maxDurabilityPoints;
                    //invCellCmp.inventoryItemComponent = upgradedWeaponItemCmp;
                    ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                    if (_sceneData.Value.firstGunCellView._entity == usedItemEntity)
                        _changeWeaponFromInventoryEventsPool.Value.Add(usedItemEntity).SetValues(false, 0);

                    else
                        _changeWeaponFromInventoryEventsPool.Value.Add(usedItemEntity).SetValues(false, 1);
                    _sceneData.Value.moneyText.text = invCmp.moneyCount + "$";

                    _sceneData.Value.dropedItemsUIView.currentGunImage.sprite = _sceneData.Value.transparentSprite;
                    _sceneData.Value.dropedItemsUIView.upgradedGunImage.sprite = _sceneData.Value.transparentSprite;
                    _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                    return;
                }
            }
        }
        foreach (var healItemEntity in _healFromHealItemCellEventsFilter.Value)
        {
            int cellEntity = _sceneData.Value.healingItemCellView._entity;
            var playerHealthComponent = _healthComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            ref var healingItemPlayer = ref _currentHealingItemComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            UseHealItem(ref playerHealthComponent, ref healingItemPlayer, cellEntity);
        }
        foreach (var movedToFastCellWeapon in _moveWeaponToInventoryEventsFilter.Value)
        {
            // Debug.Log(movedToFastCellWeapon + " movedToFastCellWeapon");
            ref var oldInvCellCmp = ref _inventoryCellsComponents.Value.Get(movedToFastCellWeapon);
            ref var oldInvItemCmp = ref _inventoryItemComponentsPool.Value.Get(movedToFastCellWeapon);
            oldInvCellCmp.cellView.inventoryCellAnimator.SetBool("buttonIsActive", false);
            _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).lastMarkedCell = 0;
            var playerAttackCmp = _attackComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (_currentHealingItemComponentsPool.Value.Get(_sceneData.Value.playerEntity).isHealing || playerAttackCmp.weaponIsChanged)
                break;
            if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.gun)
            {
                ref var playerWeaponsInInvCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                ref var gunInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(movedToFastCellWeapon);
                if (_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inGunWorkshopState)
                {
                    ref var invCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                    if (gunInvCmp.gunDurability == oldInvItemCmp.itemInfo.gunInfo.maxDurabilityPoints)
                    {
                        _sceneData.Value.ShowWarningText("gun durability is full");
                        break;
                    }
                    if (invCmp.moneyCount < oldInvItemCmp.itemInfo.gunInfo.durabilityRecoveryCost)
                    {
                        _sceneData.Value.ShowWarningText("need another " + (oldInvItemCmp.itemInfo.gunInfo.durabilityRecoveryCost - invCmp.moneyCount) + "$ to recovery helmet");
                        break;
                    }
                    invCmp.moneyCount -= oldInvItemCmp.itemInfo.gunInfo.durabilityRecoveryCost;
                    gunInvCmp.gunDurability = oldInvItemCmp.itemInfo.gunInfo.maxDurabilityPoints;
                    _sceneData.Value.moneyText.text = invCmp.moneyCount + "$";
                    _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                    break;
                }

                else if (gunInvCmp.isEquipedWeapon && _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity).currentCellCount > _inventoryItemsFilter.Value.GetEntitiesCount() && playerWeaponsInInvCmp.curEquipedWeaponsCount != 1)
                {
                    playerWeaponsInInvCmp.curEquipedWeaponsCount--;
                    foreach (var cell in _inventoryCellsFilter.Value)
                    {
                        if (_inventoryCellsComponents.Value.Get(cell).isEmpty)
                        {
                            ref var curInvCellCmp = ref _inventoryCellsComponents.Value.Get(cell);

                            _inventoryItemComponentsPool.Value.Copy(movedToFastCellWeapon, cell);
                            _gunInventoryCellComponentsPool.Value.Copy(movedToFastCellWeapon, cell);
                            if (_laserPointerForGunComponentsPool.Value.Has(movedToFastCellWeapon))
                                _laserPointerForGunComponentsPool.Value.Copy(movedToFastCellWeapon, cell);

                            ref var itemCmp = ref _inventoryItemComponentsPool.Value.Get(cell);
                            ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(cell);

                            gunInInvCmp.isEquipedWeapon = false;

                            if (_sceneData.Value.firstGunCellView._entity == movedToFastCellWeapon)
                                _changeWeaponFromInventoryEventsPool.Value.Add(cell).SetValues(true, 0);

                            else
                                _changeWeaponFromInventoryEventsPool.Value.Add(cell).SetValues(true, 1);

                            curInvCellCmp.isEmpty = false;
                            curInvCellCmp.cellView.ChangeCellItemSprite(itemCmp.itemInfo.itemSprite);
                            curInvCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                            _gunInventoryCellComponentsPool.Value.Del(movedToFastCellWeapon);
                            break;
                        }
                    }

                    _inventoryItemComponentsPool.Value.Del(movedToFastCellWeapon);
                    oldInvCellCmp.cellView.ClearInventoryCell();
                    oldInvCellCmp.isEmpty = true;
                    _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                }
                else if (!gunInvCmp.isEquipedWeapon && playerWeaponsInInvCmp.curEquipedWeaponsCount < 3)
                {
                    playerWeaponsInInvCmp.curEquipedWeaponsCount++;

                    /*  if (_inventoryCellsComponents.Value.Get(_sceneData.Value.firstGunCellView._entity).isEmpty)
                      {
                          int firstGunCellEntity = _sceneData.Value.firstGunCellView._entity;

                          _inventoryItemComponentsPool.Value.Copy(movedToFastCellWeapon, firstGunCellEntity);
                          _gunInventoryCellComponentsPool.Value.Copy(movedToFastCellWeapon, firstGunCellEntity);
                          if (_laserPointerForGunComponentsPool.Value.Has(movedToFastCellWeapon))
                              _laserPointerForGunComponentsPool.Value.Copy(movedToFastCellWeapon, firstGunCellEntity);

                          ref var gunCellCmp = ref _inventoryCellsComponents.Value.Get(firstGunCellEntity);
                          gunCellCmp.isEmpty = false;

                          ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(firstGunCellEntity);

                          gunInInvCmp.isEquipedWeapon = true;

                          ref var curInvCell = ref _inventoryItemComponentsPool.Value.Get(firstGunCellEntity);
                          var curCellView = _sceneData.Value.firstGunCellView;

                          curCellView.ChangeCellItemSprite(curInvCell.itemInfo.itemSprite);
                          curCellView.ChangeCellItemCount(curInvCell.currentItemsCount);
                          _changeWeaponFromInventoryEventsPool.Value.Add(firstGunCellEntity).SetValues(false, 0);
                      }
                      else if (_inventoryCellsComponents.Value.Get(_sceneData.Value.secondGunCellView._entity).isEmpty)
                      {
                          int secondGunCellEntity = _sceneData.Value.secondGunCellView._entity;

                          _inventoryItemComponentsPool.Value.Copy(movedToFastCellWeapon, secondGunCellEntity);
                          _gunInventoryCellComponentsPool.Value.Copy(movedToFastCellWeapon, secondGunCellEntity);
                          if (_laserPointerForGunComponentsPool.Value.Has(movedToFastCellWeapon))
                              _laserPointerForGunComponentsPool.Value.Copy(movedToFastCellWeapon, secondGunCellEntity);

                          ref var gunCellCmp = ref _inventoryCellsComponents.Value.Get(secondGunCellEntity);
                          gunCellCmp.isEmpty = false;

                          ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(secondGunCellEntity);

                          gunInInvCmp.isEquipedWeapon = true;

                          ref var curInvCell = ref _inventoryItemComponentsPool.Value.Get(secondGunCellEntity);
                          var curCellView = _sceneData.Value.secondGunCellView;

                          curCellView.ChangeCellItemSprite(curInvCell.itemInfo.itemSprite);
                          curCellView.ChangeCellItemCount(curInvCell.currentItemsCount);
                          _changeWeaponFromInventoryEventsPool.Value.Add(secondGunCellEntity).SetValues(false, 1);
                      }*/

                    int needCellNum = _inventoryCellsComponents.Value.Get(_sceneData.Value.firstGunCellView._entity).isEmpty ? 0 : 1;
                    var needCellView = needCellNum == 0 ? _sceneData.Value.firstGunCellView : _sceneData.Value.secondGunCellView;
                    int gunCellEntity = needCellView._entity;

                    _inventoryItemComponentsPool.Value.Copy(movedToFastCellWeapon, gunCellEntity);
                    _gunInventoryCellComponentsPool.Value.Copy(movedToFastCellWeapon, gunCellEntity);
                    if (_laserPointerForGunComponentsPool.Value.Has(movedToFastCellWeapon))
                        _laserPointerForGunComponentsPool.Value.Copy(movedToFastCellWeapon, gunCellEntity);

                    ref var gunCellCmp = ref _inventoryCellsComponents.Value.Get(gunCellEntity);
                    gunCellCmp.isEmpty = false;

                    ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(gunCellEntity);

                    gunInInvCmp.isEquipedWeapon = true;

                    ref var curInvCell = ref _inventoryItemComponentsPool.Value.Get(gunCellEntity);

                    needCellView.ChangeCellItemSprite(curInvCell.itemInfo.itemSprite);
                    needCellView.ChangeCellItemCount(curInvCell.currentItemsCount);
                    _changeWeaponFromInventoryEventsPool.Value.Add(gunCellEntity).SetValues(false, needCellNum);


                    _gunInventoryCellComponentsPool.Value.Del(movedToFastCellWeapon);
                    _laserPointerForGunComponentsPool.Value.Del(movedToFastCellWeapon);
                    _inventoryItemComponentsPool.Value.Del(movedToFastCellWeapon);
                    oldInvCellCmp.cellView.ClearInventoryCell();
                    oldInvCellCmp.isEmpty = true;
                    _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                }
                else
                    break;

            }
            else if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.meleeWeapon)
            {
                if (_currentHealingItemComponentsPool.Value.Get(_sceneData.Value.playerEntity).isHealing)
                    break;
                int meleeCellEntity = _sceneData.Value.meleeWeaponCellView._entity;

                SwapCellItems(meleeCellEntity, movedToFastCellWeapon);


                _changeWeaponFromInventoryEventsPool.Value.Add(meleeCellEntity).SetValues(false, 2);

                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
            }
            else if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.randomHeal)
            {
                if (oldInvItemCmp.currentItemsCount == 1)
                {
                    if (Random.value < oldInvItemCmp.itemInfo.randomHealInfo.chanceToSafe)
                        oldInvItemCmp.itemInfo = oldInvItemCmp.itemInfo.randomHealInfo.safeItemInfo;
                    else
                        oldInvItemCmp.itemInfo = oldInvItemCmp.itemInfo.randomHealInfo.poisonItemInfo;
                }
                else
                {
                    int safeBerriesCount = Mathf.CeilToInt(oldInvItemCmp.itemInfo.randomHealInfo.chanceToSafe * (oldInvItemCmp.currentItemsCount - 1));
                    int poisonBerriesCount = oldInvItemCmp.currentItemsCount - 1 - safeBerriesCount;
                    if (Random.value < oldInvItemCmp.itemInfo.randomHealInfo.chanceToSafe)
                        safeBerriesCount++;
                    else
                        poisonBerriesCount++;
                    var safeItemInfo = oldInvItemCmp.itemInfo.randomHealInfo.safeItemInfo;
                    var poisonItemInfo = oldInvItemCmp.itemInfo.randomHealInfo.poisonItemInfo;
                    DeleteItem(ref oldInvItemCmp, ref oldInvCellCmp, oldInvItemCmp.currentItemsCount, movedToFastCellWeapon);
                    if (_storageCellTagsPool.Value.Has(movedToFastCellWeapon))
                    {
                        AddItemToInventory(safeItemInfo, safeBerriesCount, true);
                        if (CanAddItems(poisonItemInfo, poisonBerriesCount, true))
                            AddItemToInventory(poisonItemInfo, poisonBerriesCount, true);
                        else if (CanAddItems(poisonItemInfo, poisonBerriesCount, false))
                            AddItemToInventory(poisonItemInfo, poisonBerriesCount, false);
                        else
                        {
                            var droppedItemEntity = _world.Value.NewEntity();

                            ref var droppedItemComponent = ref _droppedItemComponents.Value.Add(droppedItemEntity);

                            droppedItemComponent.currentItemsCount = poisonBerriesCount;

                            droppedItemComponent.itemInfo = poisonItemInfo;

                            droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(_movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform.position, poisonItemInfo, droppedItemEntity);
                        }
                    }
                    else
                    {
                        AddItemToInventory(safeItemInfo, safeBerriesCount, false);
                        if (CanAddItems(poisonItemInfo, poisonBerriesCount, false))
                            AddItemToInventory(poisonItemInfo, poisonBerriesCount, false);
                        else
                        {
                            var droppedItemEntity = _world.Value.NewEntity();

                            ref var droppedItemComponent = ref _droppedItemComponents.Value.Add(droppedItemEntity);

                            droppedItemComponent.currentItemsCount = poisonBerriesCount;

                            droppedItemComponent.itemInfo = poisonItemInfo;

                            droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(_movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform.position, poisonItemInfo, droppedItemEntity);
                        }
                    }

                }
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
            }
            else if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.flashlight || oldInvItemCmp.itemInfo.type == ItemInfo.itemType.backpack || oldInvItemCmp.itemInfo.type == ItemInfo.itemType.sheild || oldInvItemCmp.itemInfo.type == ItemInfo.itemType.bodyArmor || oldInvItemCmp.itemInfo.type == ItemInfo.itemType.helmet)
            {
                bool isFlashlight = oldInvItemCmp.itemInfo.type == ItemInfo.itemType.flashlight;

                var specialItemCellEntity = _sceneData.Value.backpackCellView._entity;
                ref var playerCmp = ref _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                var invCmp = _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                if (isFlashlight)
                {
                    specialItemCellEntity = _sceneData.Value.flashlightItemCellView._entity;

                    if (playerCmp.useFlashlight)
                    {
                        playerCmp.useFlashlight = false;
                        playerCmp.view.flashLightObject.gameObject.SetActive(false);
                    }
                }
                else if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.sheild)
                {
                    specialItemCellEntity = _sceneData.Value.shieldCellView._entity;
                    playerCmp.view.movementView.shieldView.shieldObject.SetParent(playerCmp.view.movementView.shieldView.shieldContainer);
                    playerCmp.view.movementView.shieldView.shieldObject.localPosition = Vector3.zero;
                    playerCmp.view.movementView.shieldView.shieldObject.localRotation = Quaternion.Euler(0, 0, 0);
                    playerCmp.view.movementView.shieldView.shieldObject.gameObject.SetActive(false);
                }
                else if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.bodyArmor)
                {
                    specialItemCellEntity = _sceneData.Value.bodyArmorCellView._entity;
                    playerCmp.view.movementView.bodyArmorSpriteRenderer.sprite = _sceneData.Value.transparentSprite;
                }
                else if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.helmet)
                {
                    specialItemCellEntity = _sceneData.Value.helmetCellView._entity;
                    playerCmp.view.movementView.hairSpriteRenderer.sprite = _sceneData.Value.johnHairsSprites[0].sprites[1];
                    playerCmp.view.movementView.helmetSpriteRenderer.sprite = _sceneData.Value.transparentSprite;
                    _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.gameObject.SetActive(false);
                    playerCmp.nvgIsUsed = false;
                    ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                    if (_buildingCheckerComponentsPool.Value.Get(_sceneData.Value.playerEntity).isHideRoof)
                        _sceneData.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity - 0.35f;
                    else
                        _sceneData.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity;
                    if (globalTimeCmp.currentDayTime >= 15)
                        _sceneData.Value.gloabalLight.color = _sceneData.Value.globalLightColors[2];
                    else if (globalTimeCmp.currentDayTime == 12 || globalTimeCmp.currentDayTime == 0)
                        _sceneData.Value.gloabalLight.color = globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none ? _sceneData.Value.globalLightColors[1] : _sceneData.Value.globalLightColors[4];
                    else
                        _sceneData.Value.gloabalLight.color = globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none ? _sceneData.Value.globalLightColors[0] : _sceneData.Value.globalLightColors[3];
                    _sceneData.Value.bloomMainBg.intensity.value = 0;

                    if (_sceneData.Value.gloabalLight.intensity < 0)
                        _sceneData.Value.gloabalLight.intensity = 0f;
                }
                if (movedToFastCellWeapon == specialItemCellEntity && _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity).currentCellCount > _inventoryItemsFilter.Value.GetEntitiesCount())
                {
                    foreach (var cell in _inventoryCellsFilter.Value)
                        if (_inventoryCellsComponents.Value.Get(cell).isEmpty)
                        {
                            ref var curInvCellCmp = ref _inventoryCellsComponents.Value.Get(cell);

                            _inventoryItemComponentsPool.Value.Copy(movedToFastCellWeapon, cell);
                            ref var itemCmp = ref _inventoryItemComponentsPool.Value.Get(cell);

                            CopySpecialComponents(movedToFastCellWeapon, cell, oldInvItemCmp.itemInfo);


                            curInvCellCmp.isEmpty = false;
                            curInvCellCmp.cellView.ChangeCellItemSprite(itemCmp.itemInfo.itemSprite);
                            curInvCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);

                            DelSpecialComponents(movedToFastCellWeapon, oldInvItemCmp.itemInfo);

                            _inventoryItemComponentsPool.Value.Del(movedToFastCellWeapon);
                            oldInvCellCmp.cellView.ClearInventoryCell();
                            oldInvCellCmp.isEmpty = true;

                            if (itemCmp.itemInfo.type == ItemInfo.itemType.backpack)
                                _setInventoryCellsToNewValueEventsPool.Value.Add(_sceneData.Value.playerEntity).changedCount = _sceneData.Value.dropedItemsUIView.startBackpackInfo.cellsCount - invCmp.currentCellCount;

                            else if (itemCmp.itemInfo.type == ItemInfo.itemType.helmet)
                            {
                                ref var fOVCmp = ref _fieldOfViewComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                                fOVCmp.fieldOfView = playerCmp.view.defaultFOV;
                                fOVCmp.viewDistance = playerCmp.view.viewDistance;
                            }
                            break;
                        }

                }
                else if (movedToFastCellWeapon != specialItemCellEntity)
                {
                    ref var curInvCellCmp = ref _inventoryCellsComponents.Value.Get(specialItemCellEntity);

                    if (_inventoryCellsComponents.Value.Get(specialItemCellEntity).isEmpty)
                    {
                        _inventoryItemComponentsPool.Value.Copy(movedToFastCellWeapon, specialItemCellEntity);
                        CopySpecialComponents(movedToFastCellWeapon, specialItemCellEntity, oldInvItemCmp.itemInfo);

                        var flashlightItemCmp = _inventoryItemComponentsPool.Value.Get(specialItemCellEntity);


                        curInvCellCmp.isEmpty = false;
                        curInvCellCmp.cellView.ChangeCellItemSprite(flashlightItemCmp.itemInfo.itemSprite);
                        curInvCellCmp.cellView.ChangeCellItemCount(flashlightItemCmp.currentItemsCount);
                        if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.sheild)
                            _shieldComponentsPool.Value.Del(movedToFastCellWeapon);
                        else if (oldInvItemCmp.itemInfo.type != ItemInfo.itemType.backpack)
                            _durabilityInInventoryComponentsPool.Value.Del(movedToFastCellWeapon);

                        _inventoryItemComponentsPool.Value.Del(movedToFastCellWeapon);
                        oldInvCellCmp.cellView.ClearInventoryCell();
                        oldInvCellCmp.isEmpty = true;

                    }
                    else
                    {
                        SwapCellItems(movedToFastCellWeapon, specialItemCellEntity);
                    }
                    ref var itemCmp = ref _inventoryItemComponentsPool.Value.Get(specialItemCellEntity);
                    if (isFlashlight)
                    {
                        playerCmp.view.flashLightObject.intensity = itemCmp.itemInfo.flashlightInfo.lightIntecnsity;
                        playerCmp.view.flashLightObject.pointLightOuterRadius = itemCmp.itemInfo.flashlightInfo.lightRange;
                        playerCmp.view.flashLightObject.color = itemCmp.itemInfo.flashlightInfo.lightColor;
                        playerCmp.view.flashLightObject.pointLightInnerAngle = itemCmp.itemInfo.flashlightInfo.spotAngle;
                        playerCmp.view.flashLightObject.pointLightOuterAngle = itemCmp.itemInfo.flashlightInfo.spotAngle;
                    }
                    else if (itemCmp.itemInfo.type == ItemInfo.itemType.sheild)
                    {
                        playerCmp.view.movementView.shieldView.shieldObject.gameObject.SetActive(true);
                        playerCmp.view.movementView.shieldView.shieldSpriteRenderer.sprite = itemCmp.itemInfo.sheildInfo.sheildSprite;
                        playerCmp.view.movementView.shieldView.shieldCollider.size = itemCmp.itemInfo.sheildInfo.sheildColliderScale;
                        playerCmp.view.movementView.shieldView.shieldCollider.offset = itemCmp.itemInfo.sheildInfo.sheildColliderPositionOffset;
                        playerCmp.view.movementView.shieldView.shieldCollider.gameObject.transform.SetParent(playerCmp.view.movementView.shieldView.shieldContainer);
                        if (playerCmp.view.movementView.shieldView.shieldObject.localScale.x > 0)
                            playerCmp.view.movementView.shieldView.shieldObject.localScale = new Vector3(playerCmp.view.movementView.shieldView.shieldObject.localScale.x * -1, playerCmp.view.movementView.shieldView.shieldObject.localScale.y, playerCmp.view.movementView.shieldView.shieldObject.localScale.z);
                        playerCmp.view.movementView.shieldView.shieldObject.localPosition = Vector2.zero;
                        if (_shieldComponentsPool.Value.Get(specialItemCellEntity).currentDurability > 0)
                            playerCmp.view.movementView.shieldView.shieldCollider.enabled = true;
                    }
                    else if (itemCmp.itemInfo.type == ItemInfo.itemType.bodyArmor)
                    {
                        playerCmp.view.movementView.bodyArmorSpriteRenderer.sprite = itemCmp.itemInfo.bodyArmorInfo.bodyArmorSprite;
                        playerCmp.view.movementView.bodyArmorSpriteRenderer.transform.localPosition = itemCmp.itemInfo.bodyArmorInfo.inGamePositionOnPlayer;
                    }
                    else if (itemCmp.itemInfo.type == ItemInfo.itemType.helmet)
                    {
                        playerCmp.view.movementView.helmetSpriteRenderer.sprite = itemCmp.itemInfo.helmetInfo.helmetSprite;
                        playerCmp.view.movementView.helmetSpriteRenderer.transform.localPosition = itemCmp.itemInfo.helmetInfo.inGamePositionOnPlayer;
                        playerCmp.view.movementView.hairSpriteRenderer.sprite = _sceneData.Value.johnHairsSprites[0].sprites[itemCmp.itemInfo.helmetInfo.hairSpriteIndex];

                        if (itemCmp.itemInfo.helmetInfo.dropTransparentMultiplayer != 0)
                        {
                            int curDurability = Mathf.FloorToInt(_durabilityInInventoryComponentsPool.Value.Get(_sceneData.Value.helmetCellView._entity).currentDurability / (itemCmp.itemInfo.helmetInfo.armorDurability / 4));
                            if (curDurability != 4 && _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.sprite != _sceneData.Value.dropedItemsUIView.crackedGlassSprites[curDurability])
                            {
                                _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.sprite = _sceneData.Value.dropedItemsUIView.crackedGlassSprites[curDurability];
                                _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.gameObject.SetActive(true);
                            }

                        }
                        if (itemCmp.itemInfo.helmetInfo.fowAngleRemove != 0)
                        {
                            ref var fOVCmp = ref _fieldOfViewComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                            fOVCmp.fieldOfView = playerCmp.view.defaultFOV - itemCmp.itemInfo.helmetInfo.fowAngleRemove;
                            fOVCmp.viewDistance = playerCmp.view.viewDistance - itemCmp.itemInfo.helmetInfo.fowLenghtRemove;
                        }

                    }
                    else
                        _setInventoryCellsToNewValueEventsPool.Value.Add(_sceneData.Value.playerEntity).changedCount = itemCmp.itemInfo.backpackInfo.cellsCount - invCmp.currentCellCount;


                }
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
            }
            else if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.heal || oldInvItemCmp.itemInfo.type == ItemInfo.itemType.grenade)
            {
                var healItemCellEntity = 0;
                if (oldInvItemCmp.itemInfo.type == ItemInfo.itemType.heal)
                    healItemCellEntity = _sceneData.Value.healingItemCellView._entity;
                else
                    healItemCellEntity = _sceneData.Value.grenadeCellView._entity;

                ref var invCellForHealItemsCmp = ref _inventoryCellsComponents.Value.Get(healItemCellEntity);
                if (movedToFastCellWeapon == healItemCellEntity)
                {
                    int neededItems = oldInvItemCmp.currentItemsCount - AddItemToInventory(oldInvItemCmp.itemInfo, oldInvItemCmp.currentItemsCount, false);
                    ref var invCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                    invCmp.weight -= neededItems * oldInvItemCmp.itemInfo.itemWeight;
                    _sceneData.Value.statsInventoryText.text = invCmp.weight.ToString("0.0") + "kg/ " + invCmp.currentMaxWeight + "kg \n max cells " + invCmp.currentCellCount;
                    if (neededItems == oldInvItemCmp.currentItemsCount)
                    {
                        _inventoryItemComponentsPool.Value.Del(movedToFastCellWeapon);
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
                {
                    if (_inventoryItemComponentsPool.Value.Has(healItemCellEntity) && _inventoryItemComponentsPool.Value.Get(healItemCellEntity).itemInfo.itemId == oldInvItemCmp.itemInfo.itemId)
                    {
                        ref var healInvItemCell = ref _inventoryItemComponentsPool.Value.Get(healItemCellEntity);
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
                            _inventoryItemComponentsPool.Value.Del(movedToFastCellWeapon);
                        }
                    }
                    else
                    {
                        _inventoryItemComponentsPool.Value.Copy(movedToFastCellWeapon, healItemCellEntity);

                        ref var healInvItemCell = ref _inventoryItemComponentsPool.Value.Get(healItemCellEntity);
                        ref var healInvCellCmp = ref _inventoryCellsComponents.Value.Get(healItemCellEntity);

                        healInvCellCmp.cellView.ChangeCellItemSprite(healInvItemCell.itemInfo.itemSprite);
                        healInvCellCmp.cellView.ChangeCellItemCount(healInvItemCell.currentItemsCount);
                        oldInvCellCmp.cellView.ClearInventoryCell();

                        healInvCellCmp.isEmpty = false;
                        oldInvCellCmp.isEmpty = true;

                        _inventoryItemComponentsPool.Value.Del(movedToFastCellWeapon);
                    }
                    _inventoryItemComponentsPool.Value.Copy(movedToFastCellWeapon, healItemCellEntity);
                }
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
            }
            _sceneData.Value.uiAudioSourse.clip = _sceneData.Value.equipItemSound;
            _sceneData.Value.uiAudioSourse.Play();
        }

        #region -take dropped item-
        foreach (var addedItem in _addItemEventsFilter.Value)
        {
            ref var dropItem = ref _droppedItemComponents.Value.Get(addedItem);

            if (dropItem.itemInfo.type == ItemInfo.itemType.gun)
            {
                var inventoryCmp = _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                if (inventoryCmp.weight + _gunInventoryCellComponentsPool.Value.Get(addedItem).currentGunWeight > inventoryCmp.currentMaxWeight) return;
                foreach (var invCell in _inventoryCellsFilter.Value)
                {
                    ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(invCell);
                    if (invCellCmp.isEmpty)
                    {
                        ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Add(invCell);
                        invItemCmp.itemInfo = dropItem.itemInfo;
                        _gunInventoryCellComponentsPool.Value.Copy(addedItem, invCell);
                        AddItem(ref invCellCmp, ref invItemCmp, 1, invCell);
                        if (_laserPointerForGunComponentsPool.Value.Has(addedItem))
                            _laserPointerForGunComponentsPool.Value.Copy(addedItem, invCell);
                        _gunInventoryCellComponentsPool.Value.Del(addedItem);
                        _laserPointerForGunComponentsPool.Value.Del(addedItem);

                        dropItem.currentItemsCount = 0;
                        break;
                    }
                }


            }

            else if (dropItem.itemInfo.type == ItemInfo.itemType.flashlight || dropItem.itemInfo.type == ItemInfo.itemType.meleeWeapon || dropItem.itemInfo.type == ItemInfo.itemType.sheild || dropItem.itemInfo.type == ItemInfo.itemType.bodyArmor || dropItem.itemInfo.type == ItemInfo.itemType.helmet)
            {
                List<int> addedInvCell = new List<int>();
                (dropItem.currentItemsCount, addedInvCell) = AddItemToInventoryWithCellNumbers(dropItem.itemInfo, dropItem.currentItemsCount, addedInvCell, false);
                if (addedInvCell.Count != 0)
                {
                    CopySpecialComponents(addedItem, addedInvCell[0], dropItem.itemInfo);
                    DelSpecialComponents(addedItem, dropItem.itemInfo);
                }
            }


            else
                dropItem.currentItemsCount = AddItemToInventory(dropItem.itemInfo, dropItem.currentItemsCount, false);
            if (dropItem.currentItemsCount == 0)
            {
                dropItem.droppedItemView.DestroyItemFromGround();
                _droppedItemComponents.Value.Del(addedItem);
                _hidedObjectOutsideFOVComponentsPool.Value.Del(addedItem);
                ref var curIntractCharCmp = ref _currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                curIntractCharCmp.dropItemView = null;
                curIntractCharCmp.interactionType = PlayerInputView.InteractionType.none;
            }
            else
                _sceneData.Value.dropedItemsUIView.charactersInteractText.text = "Press F to take " + dropItem.currentItemsCount + " " + dropItem.itemInfo.itemName;

            SetMoveSpeedFromWeight();
        }
        #endregion

        #region -add items to storage-
        foreach (var addedItem in _addItemToStorageEventsFilter.Value)
        {
            int droppedItemsCount = (int)_sceneData.Value.dropedItemsUIView.generalSlider.value;
            ref var deletedInvItemCmp = ref _inventoryItemComponentsPool.Value.Get(addedItem);
            ref var inventoryCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.storageEntity);
            if (_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inShopState)
            {
                ref var shopperCmp = ref _shopCharacterComponentsPool.Value.Get(_currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).interactCharacterView._entity);
                int itemsCost = deletedInvItemCmp.itemInfo.itemCost * droppedItemsCount;
                if (itemsCost <= shopperCmp.remainedMoneyToBuy)
                {
                    shopperCmp.remainedMoneyToBuy -= itemsCost;
                    ref var invCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                    invCmp.moneyCount += itemsCost;
                    _sceneData.Value.moneyText.text = invCmp.moneyCount + "$";
                    _sceneData.Value.dropedItemsUIView.shopperMoneyToBuy.text = "shopper money " + shopperCmp.remainedMoneyToBuy + "$";
                    DeleteItem(ref deletedInvItemCmp, ref _inventoryCellsComponents.Value.Get(addedItem), droppedItemsCount, addedItem);
                    // SetMoveSpeedFromWeight();
                }
                else
                    _sceneData.Value.ShowWarningText("the shopper ran out of money");
            }


            else if (deletedInvItemCmp.itemInfo.type == ItemInfo.itemType.gun)
            {
                if (inventoryCmp.weight + _gunInventoryCellComponentsPool.Value.Get(addedItem).currentGunWeight > inventoryCmp.currentMaxWeight) return;
                foreach (var invCell in _storageCellsFilter.Value)
                {
                    ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(invCell);
                    if (invCellCmp.isEmpty)
                    {
                        ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Add(invCell);
                        invItemCmp.itemInfo = deletedInvItemCmp.itemInfo;
                        _gunInventoryCellComponentsPool.Value.Copy(addedItem, invCell);
                        AddItem(ref invCellCmp, ref invItemCmp, 1, invCell);
                        if (_laserPointerForGunComponentsPool.Value.Has(addedItem))
                        {
                            _laserPointerForGunComponentsPool.Value.Copy(addedItem, invCell);
                            _laserPointerForGunComponentsPool.Value.Del(addedItem);
                        }
                        _deleteItemEventsPool.Value.Add(addedItem).count = 1;
                        break;
                    }
                }
            }
            else
            {
                int curAddedItemsCount = droppedItemsCount;
                List<int> addedInvCell = new List<int>();
                (curAddedItemsCount, addedInvCell) = AddItemToInventoryWithCellNumbers(deletedInvItemCmp.itemInfo, curAddedItemsCount, addedInvCell, true);
                if (addedInvCell.Count != 0)
                {
                    CopySpecialComponents(addedItem, addedInvCell[0], deletedInvItemCmp.itemInfo);
                    //    Debug.Log("remove item to storage start" + (droppedItemsCount - curAddedItemsCount));

                    ref var delInvCellCmp = ref _inventoryCellsComponents.Value.Get(addedItem);
                    DeleteItem(ref deletedInvItemCmp, ref delInvCellCmp, droppedItemsCount - curAddedItemsCount, addedItem);

                    if (delInvCellCmp.isEmpty)
                        _sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.SetActive(false);
                    else
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(deletedInvItemCmp.currentItemsCount, addedItem);

                }
            }
            SetMoveSpeedFromWeight();
            if (_inventoryItemComponentsPool.Value.Has(addedItem))
                _sceneData.Value.dropedItemsUIView.SetSliderParametrs(deletedInvItemCmp.currentItemsCount, addedItem);
            else
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
        }
        #endregion

        #region -add items from storage-
        foreach (var addedItem in _addItemFromStorageEventsFilter.Value)
        {
            ref var deletedItemFromStorage = ref _inventoryItemComponentsPool.Value.Get(addedItem);
            int droppedItemsCount = (int)_sceneData.Value.dropedItemsUIView.generalSlider.value;
            int curAddedItemsCount = droppedItemsCount;


            ref var inventoryCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);

            if (deletedItemFromStorage.itemInfo.type == ItemInfo.itemType.gun)
            {
                if (inventoryCmp.weight + _gunInventoryCellComponentsPool.Value.Get(addedItem).currentGunWeight > inventoryCmp.currentMaxWeight) return;
                foreach (var invCell in _inventoryCellsFilter.Value)
                {
                    ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(invCell);
                    if (invCellCmp.isEmpty)
                    {
                        ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Add(invCell);
                        invItemCmp.itemInfo = deletedItemFromStorage.itemInfo;
                        _gunInventoryCellComponentsPool.Value.Copy(addedItem, invCell);
                        AddItem(ref invCellCmp, ref invItemCmp, 1, invCell);
                        if (_laserPointerForGunComponentsPool.Value.Has(addedItem))
                        {
                            _laserPointerForGunComponentsPool.Value.Copy(addedItem, invCell);
                            _laserPointerForGunComponentsPool.Value.Del(addedItem);
                        }
                        _deleteItemEventsPool.Value.Add(addedItem).count = 1;
                        break;
                    }
                }
            }
            else
            {
                List<int> addedInvCell = new List<int>();
                (curAddedItemsCount, addedInvCell) = AddItemToInventoryWithCellNumbers(deletedItemFromStorage.itemInfo, curAddedItemsCount, addedInvCell, false);
                if (addedInvCell.Count != 0)
                {
                    CopySpecialComponents(addedItem, addedInvCell[0], deletedItemFromStorage.itemInfo);
                    ref var cellCmp = ref _inventoryCellsComponents.Value.Get(addedItem);
                    DeleteItem(ref deletedItemFromStorage, ref cellCmp, droppedItemsCount - curAddedItemsCount, addedItem);

                    if (cellCmp.isEmpty)
                        _sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.SetActive(false);
                    else
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(deletedItemFromStorage.currentItemsCount, addedItem);

                }

            }
            SetMoveSpeedFromWeight();
            if (_inventoryItemComponentsPool.Value.Has(addedItem))
                _sceneData.Value.dropedItemsUIView.SetSliderParametrs(deletedItemFromStorage.currentItemsCount, addedItem);
            else
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
        }
        #endregion

        #region -reload event-
        foreach (var reloadEvent in _reloadEventsFilter.Value)
        {
            ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(_playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity).curEquipedWeaponCellEntity);
            ref var playerGunCmp = ref _playerGunComponentsPool.Value.Get(reloadEvent);
            ref var gunCmp = ref _gunComponentsPool.Value.Get(reloadEvent);
            bool seekBulletsinStorage = false;

            if (_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inStorageState)
                seekBulletsinStorage = true;
            int possibleBulletsToReload = FindItemCountInInventory(playerGunCmp.bulletTypeId, seekBulletsinStorage);
            ref var invCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
            if (possibleBulletsToReload == 0)
            {
                gunCmp.currentReloadDuration = 0;
                gunCmp.isReloading = false;
                playerGunCmp.isContinueReload = false;
                _sceneData.Value.ammoInfoText.text = "";
                return;
            }

            else if (gunCmp.isOneBulletReload)
            {
                var oneBulletWeight = _sceneData.Value.idItemslist.items[playerGunCmp.bulletTypeId].itemWeight;
                invCmp.weight += oneBulletWeight;
                gunInInvCmp.currentGunWeight += oneBulletWeight;

                FindItem(1, playerGunCmp.bulletTypeId, true, seekBulletsinStorage);
                playerGunCmp.bulletCountToReload = 1;
                _endReloadEventsPool.Value.Add(reloadEvent);
                return;
            }

            else if (gunCmp.magazineCapacity - gunInInvCmp.currentAmmo < possibleBulletsToReload)
                possibleBulletsToReload = gunCmp.magazineCapacity - gunInInvCmp.currentAmmo;

            var bulletWeight = _sceneData.Value.idItemslist.items[playerGunCmp.bulletTypeId].itemWeight * possibleBulletsToReload;
            invCmp.weight += bulletWeight;
            gunInInvCmp.currentGunWeight += bulletWeight;

            FindItem(possibleBulletsToReload, playerGunCmp.bulletTypeId, true, seekBulletsinStorage);
            playerGunCmp.bulletCountToReload = possibleBulletsToReload;
            _endReloadEventsPool.Value.Add(reloadEvent);
        }
        #endregion

        foreach (var droppedItem in _dropItemEventsFilter.Value)
            DropItemsFromInventory(droppedItem);

        #region -find and cell items from inventory-
        foreach (var findItem in _findAndCellItemEventsFilter.Value)
        {
            ref var shopCellCmp = ref _shopCellComponentsPool.Value.Get(findItem);

            if (FindItem(shopCellCmp.itemCount, shopCellCmp.itemInfo.itemId, true, false))
            {
                ref var invCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
                invCmp.moneyCount += shopCellCmp.itemCost;
                _sceneData.Value.moneyText.text = invCmp.moneyCount + "$";
                SetMoveSpeedFromWeight();
            }
        }
        #endregion

        #region -buy and add items to inventory-
        foreach (var buyItem in _buyItemFromShopEventFilter.Value)
        {
            ref var shopCellCmp = ref _shopCellComponentsPool.Value.Get(buyItem);

            ref var invCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);

            ref var shopperCmp = ref _shopCharacterComponentsPool.Value.Get(_currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).interactCharacterView._entity);

            if (invCmp.moneyCount < shopCellCmp.itemCost)
            {
                _sceneData.Value.ShowWarningText("need another " + (shopCellCmp.itemCost - invCmp.moneyCount) + "$ to buy");
                break;
            }
            if (shopperCmp.remainedShopItems[shopCellCmp.shopperItemId] <= 0)
            {
                _sceneData.Value.ShowWarningText("the " + shopCellCmp.itemInfo.itemName + " is out of stock");
                break;
            }
            if (!CanAddItems(shopCellCmp.itemInfo, shopCellCmp.itemCount, false))
                break;

            shopperCmp.remainedMoneyToBuy += shopCellCmp.itemCost;
            shopperCmp.remainedShopItems[shopCellCmp.shopperItemId]--;
            shopCellCmp.cellView.shopRemainedItemCount.text = "remaining " + shopperCmp.remainedShopItems[shopCellCmp.shopperItemId] + " items to buy";
            _sceneData.Value.dropedItemsUIView.shopperMoneyToBuy.text = "shopper money " + shopperCmp.remainedMoneyToBuy + "$";

            List<int> weaponCellCount = new List<int>();
            int addedItems = 0;
            (addedItems, weaponCellCount) = AddItemToInventoryWithCellNumbers(shopCellCmp.itemInfo, shopCellCmp.itemCount, weaponCellCount, false);

            if (shopCellCmp.itemInfo.type == ItemInfo.itemType.gun)
            {
                ref var gunCmp = ref _gunInventoryCellComponentsPool.Value.Get(weaponCellCount[0]);
                gunCmp.gunDurability = shopCellCmp.itemInfo.gunInfo.maxDurabilityPoints;
            }
            else if (shopCellCmp.itemInfo.type == ItemInfo.itemType.flashlight || shopCellCmp.itemInfo.type == ItemInfo.itemType.bodyArmor || shopCellCmp.itemInfo.type == ItemInfo.itemType.helmet)
            {
                ref var flashlightCmp = ref _durabilityInInventoryComponentsPool.Value.Get(weaponCellCount[0]);
                if (shopCellCmp.itemInfo.type == ItemInfo.itemType.flashlight)
                    flashlightCmp.currentDurability = shopCellCmp.itemInfo.flashlightInfo.maxChargedTime;
                else if (shopCellCmp.itemInfo.type == ItemInfo.itemType.helmet)
                    flashlightCmp.currentDurability = shopCellCmp.itemInfo.helmetInfo.armorDurability;
                else
                    flashlightCmp.currentDurability = shopCellCmp.itemInfo.bodyArmorInfo.armorDurability;
                if (shopCellCmp.itemInfo.type == ItemInfo.itemType.helmet && shopCellCmp.itemInfo.helmetInfo.addedLightIntancity != 0)
                {
                    ref var shieldCmp = ref _shieldComponentsPool.Value.Get(weaponCellCount[0]);
                    shieldCmp.currentDurability = shopCellCmp.itemInfo.helmetInfo.nightTimeModeDuration;
                }
            }
            else if (shopCellCmp.itemInfo.type == ItemInfo.itemType.sheild)
            {
                ref var shieldCmp = ref _shieldComponentsPool.Value.Get(weaponCellCount[0]);
                shieldCmp.currentDurability = shopCellCmp.itemInfo.sheildInfo.sheildDurability;

            }

            invCmp.moneyCount -= shopCellCmp.itemCost;
            _sceneData.Value.moneyText.text = invCmp.moneyCount + "$";
            ref var menuStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (menuStatesCmp.lastMarkedCell != 0)
            {
                _inventoryCellsComponents.Value.Get(menuStatesCmp.lastMarkedCell).cellView.inventoryCellAnimator.SetBool("buttonIsActive", false);
                menuStatesCmp.lastMarkedCell = 0;
            }
            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
            SetMoveSpeedFromWeight();
        }
        #endregion
        foreach (var setCellsCount in _setInventoryCellsToNewValueEventsFilter.Value)
        {
            ChangeInventoryCellCount(_setInventoryCellsToNewValueEventsPool.Value.Get(setCellsCount).changedCount);
            _setInventoryCellsToNewValueEventsPool.Value.Del(setCellsCount);
        }
    }


    private bool CanAddItems(ItemInfo addedItemInfo, int addedItemCount, bool toStorage)
    {
        ref var inventoryCmp = ref toStorage ? ref _inventoryComponent.Value.Get(_sceneData.Value.storageEntity) : ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
        if (inventoryCmp.currentMaxWeight - inventoryCmp.weight < addedItemCount * addedItemInfo.itemWeight)
        {
            if (toStorage)
                _sceneData.Value.ShowWarningText("too heavy to put in storage");
            else
                _sceneData.Value.ShowWarningText("too heavy to put in inventory");
            return false;
        }
        int possipleAddedItem = 0;

        if (!toStorage)
        {
            foreach (var invItem in _inventoryItemsFilter.Value)
            {
                var invItemCmp = _inventoryItemComponentsPool.Value.Get(invItem);

                if (invItemCmp.itemInfo.itemId == addedItemInfo.itemId)
                    possipleAddedItem += invItemCmp.itemInfo.maxCount - invItemCmp.currentItemsCount;
            }

            possipleAddedItem += (inventoryCmp.currentCellCount - _inventoryItemsFilter.Value.GetEntitiesCount()) * addedItemInfo.maxCount;
        }
        else
        {
            foreach (var invItem in _storageItemsFilter.Value)
            {
                var invItemCmp = _inventoryItemComponentsPool.Value.Get(invItem);

                if (invItemCmp.itemInfo.itemId == addedItemInfo.itemId)
                    possipleAddedItem += invItemCmp.itemInfo.maxCount - invItemCmp.currentItemsCount;
            }

            possipleAddedItem += (inventoryCmp.currentCellCount - _storageItemsFilter.Value.GetEntitiesCount()) * addedItemInfo.maxCount;
        }

        if (possipleAddedItem < addedItemCount)
        {
            if (toStorage)
                _sceneData.Value.ShowWarningText("need more space in storage");
            else
                _sceneData.Value.ShowWarningText("need more space in inventory");
            return false;
        }

        return true;
    }

    private void SwapCellItems(int swappedCellFirst, int swappedCellSecond)
    {
        var changedWeaponInvCellCmp = _inventoryItemComponentsPool.Value.Get(swappedCellFirst);
        _inventoryItemComponentsPool.Value.Copy(swappedCellSecond, swappedCellFirst);//


        ref var fastCellItemCmp = ref _inventoryItemComponentsPool.Value.Get(swappedCellFirst);
        if (fastCellItemCmp.itemInfo.type == ItemInfo.itemType.flashlight || fastCellItemCmp.itemInfo.type == ItemInfo.itemType.bodyArmor || fastCellItemCmp.itemInfo.type == ItemInfo.itemType.helmet)
        {
            var changedFlashlightCmp = _durabilityInInventoryComponentsPool.Value.Get(swappedCellFirst);
            _durabilityInInventoryComponentsPool.Value.Copy(swappedCellSecond, swappedCellFirst);
            _durabilityInInventoryComponentsPool.Value.Get(swappedCellSecond) = changedFlashlightCmp;
            if (_shieldComponentsPool.Value.Has(swappedCellSecond) && _shieldComponentsPool.Value.Has(swappedCellFirst))
            {
                var changedShieldCmp = _shieldComponentsPool.Value.Get(swappedCellFirst);
                _shieldComponentsPool.Value.Copy(swappedCellSecond, swappedCellFirst);
                _shieldComponentsPool.Value.Get(swappedCellSecond) = changedShieldCmp;
            }
            else if (_shieldComponentsPool.Value.Has(swappedCellSecond))
            {
                _shieldComponentsPool.Value.Copy(swappedCellSecond, swappedCellFirst);
                _shieldComponentsPool.Value.Del(swappedCellSecond);
            }
            else if (_shieldComponentsPool.Value.Has(swappedCellFirst))
            {
                _shieldComponentsPool.Value.Copy(swappedCellFirst, swappedCellSecond);
                _shieldComponentsPool.Value.Del(swappedCellFirst);
            }
        }
        else if (fastCellItemCmp.itemInfo.type == ItemInfo.itemType.sheild)
        {
            var changedShieldCmp = _shieldComponentsPool.Value.Get(swappedCellFirst);
            _shieldComponentsPool.Value.Copy(swappedCellSecond, swappedCellFirst);
            _shieldComponentsPool.Value.Get(swappedCellSecond) = changedShieldCmp;
        }

        ref var inventoryItemCmp = ref _inventoryItemComponentsPool.Value.Get(swappedCellSecond);
        inventoryItemCmp = changedWeaponInvCellCmp;

        ref var curInvCellCmp = ref _inventoryCellsComponents.Value.Get(swappedCellSecond);
        curInvCellCmp.cellView.ChangeCellItemSprite(inventoryItemCmp.itemInfo.itemSprite);

        _inventoryCellsComponents.Value.Get(swappedCellFirst).cellView.ChangeCellItemSprite(fastCellItemCmp.itemInfo.itemSprite);
    }
    private void AddItem(ref InventoryCellComponent invCellCmp, ref InventoryItemComponent itemCmp, int addedItemsCount, int cellEntity)//��� ��� ������������ ������
    {
        if (itemCmp.currentItemsCount == 0)
        {
            invCellCmp.cellView.ChangeCellItemSprite(itemCmp.itemInfo.itemSprite);
            invCellCmp.isEmpty = false;

        }
        itemCmp.currentItemsCount += addedItemsCount;

        if (_storageCellTagsPool.Value.Has(cellEntity))
        {
            ref var storageCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.storageEntity);
            if (itemCmp.itemInfo.type == ItemInfo.itemType.gun && _gunInventoryCellComponentsPool.Value.Has(cellEntity))
            {
                storageCmp.weight += _gunInventoryCellComponentsPool.Value.Get(cellEntity).currentGunWeight;
            }
            else
                storageCmp.weight += addedItemsCount * itemCmp.itemInfo.itemWeight;
            _sceneData.Value.statsStorageText.text = storageCmp.weight.ToString("0.0") + "kg/ " + storageCmp.currentMaxWeight + "kg \n max cells " + storageCmp.currentCellCount;
        }
        else
        {
            if (itemCmp.itemInfo.itemId == 60)
                _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).canDeffuseMines = true;
            if (itemCmp.itemInfo.itemId == 62)
                _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).hasForestGuide = true;
            ref var inventoryCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
            if (itemCmp.itemInfo.type == ItemInfo.itemType.gun && _gunInventoryCellComponentsPool.Value.Has(cellEntity))
                inventoryCmp.weight += _gunInventoryCellComponentsPool.Value.Get(cellEntity).currentGunWeight;
            else
                inventoryCmp.weight += addedItemsCount * itemCmp.itemInfo.itemWeight;
            _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + inventoryCmp.currentMaxWeight + "kg \n max cells " + inventoryCmp.currentCellCount;
        }
        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
    }

    private void ChangeInventoryCellCount(int changedCellCount)
    {
        ref var cellsListCmp = ref _cellsListComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var invCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);

        if (changedCellCount < 0)
        {
            for (int i = 0; i < changedCellCount * -1; i++)
            {
                int cellEntity = cellsListCmp.cells[^1]._entity;
                ref var cellCmp = ref _inventoryCellsComponents.Value.Get(cellEntity);
                if (_inventoryItemComponentsPool.Value.Has(cellEntity))
                {
                    ref var itemCmp = ref _inventoryItemComponentsPool.Value.Get(cellEntity);

                    var droppedItem = _world.Value.NewEntity();
                    ref var droppedItemComponent = ref _droppedItemComponents.Value.Add(droppedItem);
                    int droppedItemsCount = itemCmp.currentItemsCount;

                    droppedItemComponent.currentItemsCount = droppedItemsCount;

                    droppedItemComponent.itemInfo = itemCmp.itemInfo;

                    droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(_movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform.position, itemCmp.itemInfo, droppedItem);
                    _hidedObjectOutsideFOVComponentsPool.Value.Add(droppedItem).hidedObjects = new Transform[] { droppedItemComponent.droppedItemView.gameObject.transform.GetChild(0) };

                    CopySpecialComponents(cellEntity, droppedItem, itemCmp.itemInfo);

                    DeleteItem(ref itemCmp, ref cellCmp, droppedItemsCount, cellEntity);

                    cellCmp.cellView.ClearInventoryCell();
                }
                cellCmp.cellView.gameObject.SetActive(false);
                _inventoryCellsComponents.Value.Del(cellEntity);
                cellsListCmp.cells.RemoveAt(cellsListCmp.cells.Count - 1);
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                SetMoveSpeedFromWeight();
            }
        }
        else
        {
            int startAddCell = cellsListCmp.cells.Count;
            for (int i = 0; i < changedCellCount; i++)
            {
                var cellView = _sceneData.Value.GetInventoryCell(_inventoryCellsFilter.Value.GetEntitiesCount());
                if (cellView._entity == 0)
                {
                    int cellEntity = _world.Value.NewEntity();
                    cellView.Construct(cellEntity, _world.Value);
                    _inventoryCellTagsPool.Value.Add(cellEntity);
                }
                if (_inventoryCellsComponents.Value.Has(cellView._entity))
                {
                    ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(cellView._entity);
                    invCellCmp.isEmpty = true;
                    invCellCmp.cellView = cellView;
                }
                else
                {
                    ref var invCellCmp = ref _inventoryCellsComponents.Value.Add(cellView._entity);
                    invCellCmp.isEmpty = true;
                    invCellCmp.cellView = cellView;
                }

                cellsListCmp.cells.Add(cellView);
            }
        }
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
        invCmp.currentCellCount += changedCellCount;
        _sceneData.Value.statsInventoryText.text = invCmp.weight.ToString("0.0") + "kg/ " + invCmp.currentMaxWeight + "kg \n max cells " + invCmp.currentCellCount;

    }
    private void DeleteItem(ref InventoryItemComponent itemCmpToDel, ref InventoryCellComponent invCellCmpToDel, int deletedItemsCount, int cellEntity)
    {
        int delItemId = itemCmpToDel.itemInfo.itemId;
        if (_storageCellTagsPool.Value.Has(cellEntity))
        {
            ref var storageCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.storageEntity);
            if (itemCmpToDel.itemInfo.type != ItemInfo.itemType.gun)
                storageCmp.weight -= deletedItemsCount * itemCmpToDel.itemInfo.itemWeight;
            else
                storageCmp.weight -= _gunInventoryCellComponentsPool.Value.Get(cellEntity).currentGunWeight;
            _sceneData.Value.statsStorageText.text = storageCmp.weight.ToString("0.0") + "kg/ " + storageCmp.currentMaxWeight + "kg \n max cells " + storageCmp.currentCellCount;
        }
        else
        {
            ref var inventoryCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);
            if (itemCmpToDel.itemInfo.type != ItemInfo.itemType.gun)
                inventoryCmp.weight -= deletedItemsCount * itemCmpToDel.itemInfo.itemWeight;
            else
                inventoryCmp.weight -= _gunInventoryCellComponentsPool.Value.Get(cellEntity).currentGunWeight;
            _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + inventoryCmp.currentMaxWeight + "kg \n max cells " + inventoryCmp.currentCellCount;
        }
        if (itemCmpToDel.currentItemsCount == deletedItemsCount)
        {
            DelSpecialComponents(cellEntity, itemCmpToDel.itemInfo);
            invCellCmpToDel.cellView.inventoryCellAnimator.SetBool("buttonIsActive", false);
            _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).lastMarkedCell = 0;
            invCellCmpToDel.cellView.ClearInventoryCell();
            _inventoryItemComponentsPool.Value.Del(cellEntity);
            invCellCmpToDel.isEmpty = true;
        }
        else
        {
            itemCmpToDel.currentItemsCount -= deletedItemsCount;
            invCellCmpToDel.cellView.ChangeCellItemCount(itemCmpToDel.currentItemsCount);
        }
        if (!_healthComponentsPool.Value.Get(_sceneData.Value.playerEntity).isDeath)
        {
            if (delItemId == 60 && !FindItem(1, 60, false, false))
                _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).canDeffuseMines = false;
            else if (delItemId == 62 && !FindItem(1, 62, false, false))
                _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).hasForestGuide = false;
        }
    }

    private bool FindItem(int neededItemsCount, int neededItemId, bool isDeleteFindedItems, bool seekInStorage)
    {
        int curFindedItems = FindItemCountInInventory(neededItemId, seekInStorage);

        if (curFindedItems >= neededItemsCount)
        {
            if (!isDeleteFindedItems) return true;

            foreach (var invItem in _inventoryItemsFilter.Value)
            {
                ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Get(invItem);
                if (invItemCmp.itemInfo.itemId == neededItemId)
                {

                    ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(invItem);


                    if (invItemCmp.currentItemsCount <= neededItemsCount)
                    {
                        neededItemsCount -= invItemCmp.currentItemsCount;
                        DeleteItem(ref invItemCmp, ref invCellCmp, invItemCmp.currentItemsCount, invItem);
                        _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                        if (neededItemsCount == 0) break;
                    }

                    else
                    {
                        DeleteItem(ref invItemCmp, ref invCellCmp, neededItemsCount, invItem);
                        _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                        break;
                    }
                }
            }
            if (seekInStorage)
            {
                foreach (var storageItem in _storageItemsFilter.Value)
                {
                    ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Get(storageItem);
                    if (invItemCmp.itemInfo.itemId == neededItemId)
                    {

                        ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(storageItem);

                        if (invItemCmp.currentItemsCount <= neededItemsCount)
                        {
                            neededItemsCount -= invItemCmp.currentItemsCount;

                            DeleteItem(ref invItemCmp, ref invCellCmp, invItemCmp.currentItemsCount, storageItem);
                            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                            if (neededItemsCount == 0) break;
                        }

                        else
                        {
                            DeleteItem(ref invItemCmp, ref invCellCmp, neededItemsCount, storageItem);
                            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
                            break;
                        }
                    }
                }
            }

            return true;
        }
        return false;
    }

    private int FindItemCountInInventory(int itemId, bool seekInStorage)
    {
        int findedItemsCount = 0;

        foreach (var invItem in _inventoryItemsFilter.Value)
        {
            var invItemCmp = _inventoryItemComponentsPool.Value.Get(invItem);
            if (invItemCmp.itemInfo.itemId == itemId)
                findedItemsCount += invItemCmp.currentItemsCount;
        }

        if (seekInStorage)
            foreach (var invItem in _storageItemsFilter.Value)
            {
                var invItemCmp = _inventoryItemComponentsPool.Value.Get(invItem);
                if (invItemCmp.itemInfo.itemId == itemId)
                    findedItemsCount += invItemCmp.currentItemsCount;
            }

        return findedItemsCount;
    }

    private void SetMoveSpeedFromWeight()
    {
        ref var inventoryCmp = ref _inventoryComponent.Value.Get(_sceneData.Value.inventoryEntity);

        ref var playerMoveCmp = ref _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        playerMoveCmp.moveSpeed = playerMoveCmp.movementView.moveSpeed + playerMoveCmp.movementView.moveSpeed / 50 * _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).statLevels[0];
        if (inventoryCmp.weight / inventoryCmp.currentMaxWeight > 0.6f)
            playerMoveCmp.moveSpeed -= (playerMoveCmp.movementView.moveSpeed * ((inventoryCmp.weight / inventoryCmp.currentMaxWeight) - 0.6f) * 2);
    }

    private void DropItemsFromInventory(int itemInventoryCell)
    {
        ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Get(itemInventoryCell);
        ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(itemInventoryCell);

        var droppedItem = _world.Value.NewEntity();

        ref var droppedItemComponent = ref _droppedItemComponents.Value.Add(droppedItem);
        int droppedItemsCount = (int)_sceneData.Value.dropedItemsUIView.generalSlider.value;

        droppedItemComponent.currentItemsCount = droppedItemsCount;

        bool isDeleteCellItem = droppedItemComponent.currentItemsCount == droppedItemsCount ? true : false;

        droppedItemComponent.itemInfo = invItemCmp.itemInfo;

        Debug.Log(droppedItemComponent.itemInfo);
        droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(_movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform.position, droppedItemComponent.itemInfo, droppedItem);
        _hidedObjectOutsideFOVComponentsPool.Value.Add(droppedItem).hidedObjects = new Transform[] { droppedItemComponent.droppedItemView.gameObject.transform.GetChild(0) };
        CopySpecialComponents(itemInventoryCell, droppedItem, droppedItemComponent.itemInfo);
        DeleteItem(ref invItemCmp, ref invCellCmp, droppedItemsCount, itemInventoryCell);

        if (isDeleteCellItem)
            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);
        else
            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(invItemCmp.currentItemsCount, itemInventoryCell);

        SetMoveSpeedFromWeight();


    }

    private int AddItemToInventory(ItemInfo itemInfo, int itemsCount, bool toStorage)
    {
        if (itemsCount == 0) return 0;

        int needInvEntity = toStorage ? _sceneData.Value.storageEntity : _sceneData.Value.inventoryEntity;
        ref var inventoryCmp = ref _inventoryComponent.Value.Get(needInvEntity);
        var cellsFilter = toStorage ? _storageCellsFilter.Value : _inventoryCellsFilter.Value;
        foreach (var cell in cellsFilter)
        {
            ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(cell);
            if (!invCellCmp.isEmpty)
            {
                var invItemCmp = _inventoryItemComponentsPool.Value.Get(cell);
                if (invItemCmp.itemInfo.itemId != itemInfo.itemId || (invItemCmp.currentItemsCount == itemInfo.maxCount && (invItemCmp.itemInfo.itemId == itemInfo.itemId))) continue;
            }

            if (inventoryCmp.currentMaxWeight + 0.001f < (inventoryCmp.weight + (itemsCount * itemInfo.itemWeight)))
            {
                float weightDelta = (inventoryCmp.weight + itemsCount * itemInfo.itemWeight) - inventoryCmp.currentMaxWeight;

                int neededItemsToFullInventory = itemsCount - Mathf.CeilToInt(weightDelta / itemInfo.itemWeight);

                if (neededItemsToFullInventory <= 0)
                {
                    return itemsCount;
                }

                if (!invCellCmp.isEmpty)
                {
                    ref var itemCmp = ref _inventoryItemComponentsPool.Value.Get(cell);
                    if (itemCmp.currentItemsCount + neededItemsToFullInventory < itemCmp.itemInfo.maxCount)
                    {
                        AddItem(ref invCellCmp, ref itemCmp, neededItemsToFullInventory, cell);
                        return AddItemToInventory(itemInfo, itemsCount - neededItemsToFullInventory, toStorage);
                    }
                    else
                    {
                        int delta = itemInfo.maxCount - itemCmp.currentItemsCount;
                        AddItem(ref invCellCmp, ref itemCmp, delta, cell);
                        return AddItemToInventory(itemInfo, itemsCount - delta, toStorage);
                    }
                }
                else
                {
                    ref var itemCmp = ref _inventoryItemComponentsPool.Value.Add(cell);
                    itemCmp.itemInfo = itemInfo;
                    if (neededItemsToFullInventory >= itemInfo.maxCount)
                    {
                        AddItem(ref invCellCmp, ref itemCmp, itemCmp.itemInfo.maxCount, cell);
                        return AddItemToInventory(itemInfo, itemsCount - itemInfo.maxCount, toStorage);
                    }
                    else
                    {
                        AddItem(ref invCellCmp, ref itemCmp, neededItemsToFullInventory, cell);
                        return AddItemToInventory(itemInfo, itemsCount - neededItemsToFullInventory, toStorage);
                    }
                }

            }

            else if (invCellCmp.isEmpty)
            {
                ref var itemCmp = ref _inventoryItemComponentsPool.Value.Add(cell);

                itemCmp.itemInfo = itemInfo;

                TryAddSpecialItemComponent(itemInfo, cell);

                if (itemsCount > itemCmp.itemInfo.maxCount)
                {
                    int delta = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                    AddItem(ref invCellCmp, ref itemCmp, itemCmp.itemInfo.maxCount, cell);
                    return AddItemToInventory(itemInfo, itemsCount - delta, toStorage);
                }
                AddItem(ref invCellCmp, ref itemCmp, itemsCount, cell);
                return 0;
            }
            else
            {
                ref var itemCmp = ref _inventoryItemComponentsPool.Value.Get(cell);

                if (itemCmp.itemInfo.itemId == itemInfo.itemId && itemCmp.currentItemsCount != itemCmp.itemInfo.maxCount)
                {
                    if (itemsCount > itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount)
                    {
                        int deltaCount = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                        AddItem(ref invCellCmp, ref itemCmp, deltaCount, cell);
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        return AddItemToInventory(itemInfo, itemsCount - deltaCount, toStorage);
                    }
                    AddItem(ref invCellCmp, ref itemCmp, itemsCount, cell);
                    _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                    return 0;
                }
            }
        }

        return itemsCount;
    }

    private (int addedItemsCount, List<int> cellsWithItem) AddItemToInventoryWithCellNumbers(ItemInfo itemInfo, int itemsCount, List<int> cellsWithItem, bool toStorage)
    {
        if (itemsCount == 0) return (0, cellsWithItem);
        int needInvEntity = toStorage ? _sceneData.Value.storageEntity : _sceneData.Value.inventoryEntity;
        ref var inventoryCmp = ref _inventoryComponent.Value.Get(needInvEntity);
        var cellsFilter = toStorage ? _storageCellsFilter.Value : _inventoryCellsFilter.Value;
        //
        foreach (var cell in cellsFilter)
        {

            ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(cell);
            if (!invCellCmp.isEmpty && (_inventoryItemComponentsPool.Value.Get(cell).itemInfo.itemId != itemInfo.itemId || (_inventoryItemComponentsPool.Value.Get(cell).currentItemsCount == itemInfo.maxCount && (_inventoryItemComponentsPool.Value.Get(cell).itemInfo.itemId == itemInfo.itemId)))) continue;

            if (inventoryCmp.currentMaxWeight + 0.001f < (inventoryCmp.weight + (itemsCount * itemInfo.itemWeight)))
            {
                float weightDelta = (inventoryCmp.weight + itemsCount * itemInfo.itemWeight) - inventoryCmp.currentMaxWeight;

                int neededItemsToFullInventory = itemsCount - Mathf.CeilToInt(weightDelta / itemInfo.itemWeight);

                if (neededItemsToFullInventory <= 0)
                    return (itemsCount, cellsWithItem);
                else
                {
                    cellsWithItem.Add(cell);
                    if (!invCellCmp.isEmpty)
                    {
                        ref var itemCmp = ref _inventoryItemComponentsPool.Value.Get(cell);
                        if (itemCmp.currentItemsCount + neededItemsToFullInventory <= itemCmp.itemInfo.maxCount)
                        {
                            AddItem(ref invCellCmp, ref itemCmp, neededItemsToFullInventory, cell);
                            return AddItemToInventoryWithCellNumbers(itemInfo, itemsCount - neededItemsToFullInventory, cellsWithItem, toStorage);
                        }
                        else
                        {
                            int delta = itemInfo.maxCount - itemCmp.currentItemsCount;
                            AddItem(ref invCellCmp, ref itemCmp, delta, cell);
                            return AddItemToInventoryWithCellNumbers(itemInfo, itemsCount - delta, cellsWithItem, toStorage);
                        }
                    }
                    else
                    {
                        ref var itemCmp = ref _inventoryItemComponentsPool.Value.Add(cell);
                        itemCmp.itemInfo = itemInfo;
                        if (neededItemsToFullInventory > itemInfo.maxCount)
                        {
                            AddItem(ref invCellCmp, ref itemCmp, itemCmp.itemInfo.maxCount, cell);
                            return AddItemToInventoryWithCellNumbers(itemInfo, itemsCount - itemInfo.maxCount, cellsWithItem, toStorage);
                        }
                        else
                        {
                            AddItem(ref invCellCmp, ref itemCmp, neededItemsToFullInventory, cell);
                            return AddItemToInventoryWithCellNumbers(itemInfo, itemsCount - neededItemsToFullInventory, cellsWithItem, toStorage);

                        }
                    }
                }
            }

            else if (invCellCmp.isEmpty)
            {
                ref var itemCmp = ref _inventoryItemComponentsPool.Value.Add(cell);

                itemCmp.itemInfo = itemInfo;


                TryAddSpecialItemComponent(itemInfo, cell);
                cellsWithItem.Add(cell);

                if (itemsCount > itemCmp.itemInfo.maxCount)
                {
                    int delta = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                    AddItem(ref invCellCmp, ref itemCmp, delta, cell);
                    _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                    return AddItemToInventoryWithCellNumbers(itemInfo, itemsCount - delta, cellsWithItem, toStorage);
                }
                AddItem(ref invCellCmp, ref itemCmp, itemsCount, cell);
                _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                return (0, cellsWithItem);
            }
            else
            {
                ref var itemCmp = ref _inventoryItemComponentsPool.Value.Get(cell);
                cellsWithItem.Add(cell);
                if (itemCmp.itemInfo.itemId == itemInfo.itemId && itemCmp.currentItemsCount != itemCmp.itemInfo.maxCount)
                {
                    if (itemsCount > itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount)
                    {
                        int deltaCount = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                        AddItem(ref invCellCmp, ref itemCmp, deltaCount, cell);
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        return AddItemToInventoryWithCellNumbers(itemInfo, itemsCount - deltaCount, cellsWithItem, toStorage);
                    }
                    AddItem(ref invCellCmp, ref itemCmp, itemsCount, cell);
                    _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                    return (0, cellsWithItem);
                }
            }

        }
        return (itemsCount, cellsWithItem);
    }
    private void TryAddSpecialItemComponent(ItemInfo itemInfo, int cell)
    {
        if (itemInfo.type == ItemInfo.itemType.gun)
        {
            ref var gunInvCmp = ref _gunInventoryCellComponentsPool.Value.Add(cell);
            gunInvCmp.currentGunWeight = itemInfo.itemWeight;
            gunInvCmp.gunPartsId = new int[4];
            gunInvCmp.isEquipedWeapon = false;
        }
        else if (itemInfo.type == ItemInfo.itemType.flashlight || itemInfo.type == ItemInfo.itemType.bodyArmor || itemInfo.type == ItemInfo.itemType.helmet)
        {
            _durabilityInInventoryComponentsPool.Value.Add(cell);
            if (itemInfo.type == ItemInfo.itemType.helmet && itemInfo.helmetInfo.addedLightIntancity != 0)
                _shieldComponentsPool.Value.Add(cell);
        }
        else if (itemInfo.type == ItemInfo.itemType.sheild)
            _shieldComponentsPool.Value.Add(cell);
    }
}
