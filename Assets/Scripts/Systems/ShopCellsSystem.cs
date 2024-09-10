using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCellsSystem : IEcsInitSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<ShopCellComponent> _shopCellComponentsPool;

    public void Init(IEcsSystems systems)
    {
        foreach (var shopCell in _sceneData.Value.testShopItems) 
        {
            var shopCellEntity = _world.Value.NewEntity();

            ref var shopCellCmp = ref _shopCellComponentsPool.Value.Add(shopCellEntity);

            shopCellCmp.cellView = _sceneData.Value.GetShopCell(shopCellEntity, _world.Value);
            shopCellCmp.cellView.SetShopCellInfo(shopCell);
            shopCellCmp.itemInfo = shopCell.itemInfo;
            shopCellCmp.itemCost = shopCell.price;
            shopCellCmp.itemCount = shopCell.count;
        }
    }
}
