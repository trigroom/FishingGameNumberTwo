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
    private EcsPoolInject<AddItemEvent> _addItemEventsComponent;
    private EcsPoolInject<DroppedItemComponent> _droppedItemComponents;

    private EcsFilterInject<Inc<InventoryCellComponent>> _inventoryCellsFilter;
    private EcsFilterInject<Inc<AddItemEvent>> _addItemEvents;

    private int inventoryEntity;
    // private EcsFilterInject<Inc<UnitCmp>> _unitCmpFilter;
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
        foreach (var addedItem in _addItemEvents.Value)
        {
           // var newItem = _addItemEventsComponent.Value.Get(addedItem);

            var dropItem = _droppedItemComponents.Value.Get(addedItem);
              dropItem.currentItemsCount = AddItemToInventory(dropItem.itemInfo, dropItem.currentItemsCount);
            if (dropItem.currentItemsCount == 0)
            {
                dropItem.droppedItemView.DestroyItemFromGround();
                _droppedItemComponents.Value.Del(addedItem);
            }
        }
        //if(Input.GetKeyDown(KeyCode.O)) { AddItemToInventory(_sceneData.Value.testItem1, 3); }
        //всё что тут будет срабатывать на ивент изменения инвентаря
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
                    itemCmp.currentItemsCount = itemCmp.itemInfo.maxCount;
                    inventoryCmp.weight += itemCmp.itemInfo.maxCount * itemCmp.itemInfo.itemWeight;
                    invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                    _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;

                    return AddItemToInventory(itemInfo, itemsCount - itemCmp.itemInfo.maxCount);
                }
                itemCmp.currentItemsCount = itemsCount;
                inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;

                invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
                return 0;
            }
            else
            {
                ref var itemCmp = ref _inventoryItemComponent.Value.Get(cell);

                if (invCellCmp.inventoryItemComponent.itemInfo.itemId == itemInfo.itemId && itemCmp.currentItemsCount != itemCmp.itemInfo.maxCount)
                {
                    if ((itemCmp.currentItemsCount + itemsCount) > itemCmp.itemInfo.maxCount)
                    {
                        int deltaCount = itemCmp.itemInfo.maxCount - itemCmp.currentItemsCount;
                        itemCmp.currentItemsCount = itemCmp.itemInfo.maxCount;
                        inventoryCmp.weight += deltaCount * itemCmp.itemInfo.itemWeight;
                        _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
                        invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                        return AddItemToInventory(itemInfo, itemCmp.currentItemsCount - deltaCount);
                    }
                    itemCmp.currentItemsCount += itemsCount;
                    inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;

                    _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
                    invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                    return 0;
                }
            }

        }
        return itemsCount;
    }
}
