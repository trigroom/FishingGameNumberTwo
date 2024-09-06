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

    private EcsFilterInject<Inc<InventoryCellComponent>> _inventoryCellsFilter;

    private int inventoryEntity;
    // private EcsFilterInject<Inc<UnitCmp>> _unitCmpFilter;
    public void Init(IEcsSystems systems)
    {
        for (int i = 0; i < _sceneData.Value.cellsCount; i++)
        {
            var cellEntity = _world.Value.NewEntity();

            ref var inventoryCellsCmp = ref _inventoryCellsComponents.Value.Add(cellEntity);
            inventoryCellsCmp.isEmpty = true;
            inventoryCellsCmp.cellView = _sceneData.Value.GetInventoryCell();

        }

        inventoryEntity = _world.Value.NewEntity();

        ref var inventoryCmp = ref _inventoryComponent.Value.Add(inventoryEntity);

        //выгрузка айтемов в инвентарь из сохранения




        _sceneData.Value.statsText.text = inventoryCmp.weight + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;

    }

    public void Run(IEcsSystems systems)
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddItemToInventory(_sceneData.Value.testItem2, 2);

        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            AddItemToInventory(_sceneData.Value.testItem1, 3);

        }
        //всё что тут будет срабатывать на ивент изменения инвентаря
    }



    private void AddItemToInventory(ItemInfo itemInfo, int itemsCount)
    {
        foreach (var cell in _inventoryCellsFilter.Value)
        {
            ref var invCellCmp = ref _inventoryCellsComponents.Value.Get(cell);
            ref var inventoryCmp = ref _inventoryComponent.Value.Get(inventoryEntity);

            //некорректно считает текущий вес и 
            // Debug.Log(_sceneData.Value.maxWeight + " < " + (inventoryCmp.weight + (itemsCount * itemInfo.itemWeight)) + "  |  " + (itemsCount * itemInfo.itemWeight));
            if (_sceneData.Value.maxWeight + 0.001f < (inventoryCmp.weight + (itemsCount * itemInfo.itemWeight)))
            {
                float weightDelta = (inventoryCmp.weight + itemsCount * itemInfo.itemWeight) - _sceneData.Value.maxWeight;

                int neededItemsToFullInventory = itemsCount - Mathf.FloorToInt(weightDelta / itemInfo.itemWeight);

                //    Debug.Log("need to full "+neededItemsToFullInventory);
                if (neededItemsToFullInventory <= 0)
                    break;


                itemsCount = neededItemsToFullInventory;
                // AddItemToInventory(itemInfo, neededItemsToFullInventory);
                //   Debug.Log("ItemsCount" + itemsCount);
                //break;
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
                    AddItemToInventory(itemInfo, itemsCount - itemCmp.itemInfo.maxCount);
                }
                else
                {
                    itemCmp.currentItemsCount = itemsCount;
                    inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;
                }

                invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
                break;
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
                        // inventoryCmp.weight += (itemsCount - itemCmp.currentItemsCount) * itemCmp.itemInfo.itemWeight;//
                        inventoryCmp.weight += deltaCount * itemCmp.itemInfo.itemWeight;
                        AddItemToInventory(itemInfo, itemCmp.currentItemsCount - deltaCount);
                    }
                    else
                    {
                        itemCmp.currentItemsCount += itemsCount;
                        inventoryCmp.weight += itemsCount * itemCmp.itemInfo.itemWeight;
                    }

                    _sceneData.Value.statsText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + _sceneData.Value.maxWeight + "kg \n max cells " + _sceneData.Value.cellsCount;
                    invCellCmp.cellView.ChangeCellItemCount(itemCmp.currentItemsCount);
                    break;
                }
            }
        }
    }
}
