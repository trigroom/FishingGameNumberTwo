using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class InventorySystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<InventoryComponent> _inventoryComponent;
    private EcsPoolInject<InventoryCellComponent> _inventoryCellsComponents;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponent;
    private EcsPoolInject<DroppedItemComponent> _droppedItemComponents;
    private EcsPoolInject<DropItemsIvent> _dropItemsEventsComponent;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<PlayerComponent> _playerTagsPool;
    private EcsPoolInject<ShopCellComponent> _shopCellComponentsPool;
    private EcsPoolInject<EndReloadEvent> _endReloadEventsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;

    private EcsFilterInject<Inc<ReloadEvent>> _reloadEventsFilter;
    private EcsFilterInject<Inc<InventoryItemComponent>> _inventoryItemsFilter;
    private EcsFilterInject<Inc<InventoryCellComponent>> _inventoryCellsFilter;
    private EcsFilterInject<Inc<AddItemEvent>> _addItemEventsFilter;
    private EcsFilterInject<Inc<DropItemsIvent>> _dropItemEventsFilter;
    private EcsFilterInject<Inc<FindAndCellItemEvent>> _findAndCellItemEventsFilter;
    private EcsFilterInject<Inc<BuyItemFromShopEvent>> _buyItemFromShopEventFilter;

    private int inventoryEntity;
    public void Init(IEcsSystems systems)
    {
        for (int i = 0; i < _sceneData.Value.cellsCount; i++)
        {
            var cellEntity = _world.Value.NewEntity();

            ref var inventoryCellsCmp = ref _inventoryCellsComponents.Value.Add(cellEntity);
            inventoryCellsCmp.isEmpty = true;
            inventoryCellsCmp.cellView = _sceneData.Value.GetInventoryCell(cellEntity, _world.Value);

        }

        inventoryEntity = _world.Value.NewEntity();

        ref var inventoryCmp = ref _inventoryComponent.Value.Add(inventoryEntity);

        //выгрузка айтемов в инвентарь из сохранения

        _sceneData.Value.statsText.text = inventoryCmp.weight + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var addedItem in _addItemEventsFilter.Value)
        {
            ref var dropItem = ref _droppedItemComponents.Value.Get(addedItem);
            dropItem.currentItemsCount = AddItemToInventory(dropItem.itemInfo, dropItem.currentItemsCount);
            if (dropItem.currentItemsCount == 0)
            {
                dropItem.droppedItemView.DestroyItemFromGround();
                _droppedItemComponents.Value.Del(addedItem);
            }

            SetMoveSpeedFromWeight();
        }

        foreach (var reloadEvent in _reloadEventsFilter.Value)
        {
            ref var gunCmp = ref _gunComponentsPool.Value.Get(reloadEvent);
            int possibleBulletsToReload = FindItemCountInInventory(gunCmp.bulletTypeId);
            Debug.Log(possibleBulletsToReload);

            if (possibleBulletsToReload == 0)
                return;

            else if (gunCmp.magazineCapacity - gunCmp.currentMagazineCapacity < possibleBulletsToReload)
                possibleBulletsToReload = gunCmp.magazineCapacity - gunCmp.currentMagazineCapacity;

            FindItem(possibleBulletsToReload, gunCmp.bulletTypeId, true);
            gunCmp.bulletCountToReload = possibleBulletsToReload;
            Debug.Log(gunCmp.bulletCountToReload);
            _endReloadEventsPool.Value.Add(reloadEvent);
        }

        foreach (var droppedItem in _dropItemEventsFilter.Value)
        {
            DropItemsFromInventory(droppedItem);

            SetMoveSpeedFromWeight();
        }

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

        foreach (var buyItem in _buyItemFromShopEventFilter.Value)
        {
            ref var shopCellCmp = ref _shopCellComponentsPool.Value.Get(buyItem);

            ref var playerCmp = ref _playerTagsPool.Value.Get(_sceneData.Value.playerEntity);

            if (playerCmp.money < shopCellCmp.itemCost || !CanAddItems(shopCellCmp.itemInfo, shopCellCmp.itemCount)) break;

            AddItemToInventory(shopCellCmp.itemInfo, shopCellCmp.itemCount);

            playerCmp.money -= shopCellCmp.itemCost;
            _sceneData.Value.moneyText.text = playerCmp.money + "$";
            SetMoveSpeedFromWeight();
        }
    }

    private bool CanAddItems(ItemInfo addedItemInfo, int addedItemCount)
    {
        ref var inventoryCmp = ref _inventoryComponent.Value.Get(inventoryEntity);
        if (_sceneData.Value.maxWeight - inventoryCmp.weight < addedItemCount * addedItemInfo.itemWeight) return false;

        int possipleAddedItem = 0;
        foreach (var invItem in _inventoryItemsFilter.Value)
        {
            var invItemCmp = _inventoryItemComponent.Value.Get(invItem);

            if (invItemCmp.itemInfo.itemId == addedItemInfo.itemId)
                possipleAddedItem += invItemCmp.itemInfo.maxCount - invItemCmp.currentItemsCount;
        }
        possipleAddedItem += (_sceneData.Value.cellsCount - _inventoryItemsFilter.Value.GetEntitiesCount()) * addedItemInfo.maxCount;

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
            _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;

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
            _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
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
        if (inventoryCmp.weight / _sceneData.Value.maxWeight > 0.7f)
        {
            var playerMoveCmp = _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            playerMoveCmp.moveSpeed = playerMoveCmp.movementView.moveSpeed - (playerMoveCmp.movementView.moveSpeed * ((inventoryCmp.weight / _sceneData.Value.maxWeight) - 0.7f) * 2);
            Debug.Log(playerMoveCmp.moveSpeed);
        }

    }

    private void DropItemsFromInventory(int itemInventoryCell)
    {
        ref var dropEvt = ref _dropItemsEventsComponent.Value.Get(itemInventoryCell);
        ref var invItemCmp = ref _inventoryItemComponent.Value.Get(itemInventoryCell);
        ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(itemInventoryCell);
        ref var inventoryCmp = ref _inventoryComponent.Value.Get(inventoryEntity);

        inventoryCmp.weight -= dropEvt.itemsCountToDrop * invItemCmp.itemInfo.itemWeight;
        _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;

        var droppedItem = _world.Value.NewEntity();

        ref var droppedItemComponent = ref _droppedItemComponents.Value.Add(droppedItem);

        droppedItemComponent.currentItemsCount = dropEvt.itemsCountToDrop;

        droppedItemComponent.itemInfo = invItemCmp.itemInfo;

        var playerView = _playerTagsPool.Value.Get(_sceneData.Value.playerEntity).view;

        droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(playerView.GetPlayerPosition(), _sceneData.Value.testItem1, droppedItem);


        if (dropEvt.itemsCountToDrop == invItemCmp.currentItemsCount)
        {
            _inventoryItemComponent.Value.Del(itemInventoryCell);
            invCellCmp.cellView.ClearInventoryCell();
            invCellCmp.isEmpty = true;
            _inventoryItemComponent.Value.Del(itemInventoryCell);
        }
        else
        {
            invItemCmp.currentItemsCount -= dropEvt.itemsCountToDrop;
            invCellCmp.cellView.ChangeCellItemCount(invItemCmp.currentItemsCount);
            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(invItemCmp.currentItemsCount, itemInventoryCell);
        }
    }

    private int AddItemToInventory(ItemInfo itemInfo, int itemsCount)
    {

        foreach (var cell in _inventoryCellsFilter.Value)
        {
            ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(cell);
            ref var inventoryCmp = ref _inventoryComponent.Value.Get(inventoryEntity);

            if (_sceneData.Value.maxWeight + 0.001f < (inventoryCmp.weight + (itemsCount * itemInfo.itemWeight)))
            {
                float weightDelta = (inventoryCmp.weight + itemsCount * itemInfo.itemWeight) - _sceneData.Value.maxWeight;

                int neededItemsToFullInventory = itemsCount - Mathf.FloorToInt(weightDelta / itemInfo.itemWeight);

                if (neededItemsToFullInventory <= 0)
                {
                    return itemsCount;
                }

                AddItemToInventory(itemInfo, neededItemsToFullInventory);
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
                    _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
                    _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                    return AddItemToInventory(itemInfo, itemsCount - delta);
                }
                itemCmp.currentItemsCount = itemsCount;
                inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;
                invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
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
                        _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
                        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                        _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                        return AddItemToInventory(itemInfo, itemsCount - deltaCount);
                    }
                    itemCmp.currentItemsCount += itemsCount;
                    inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;

                    _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
                    invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                    _sceneData.Value.dropedItemsUIView.SetSliderParametrs(itemCmp.currentItemsCount, cell);
                    return 0;
                }
            }

        }
        return itemsCount;
    }
}
