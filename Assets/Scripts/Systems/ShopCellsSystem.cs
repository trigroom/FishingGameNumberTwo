using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

public class ShopCellsSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<ShopCellComponent> _shopCellComponentsPool;
    private EcsPoolInject<ShopCharacterComponent> _shopCharacterComponentsPool;

    private EcsFilterInject<Inc<ShopOpenEvent>> _shopOpenEventsFilter;
    private EcsFilterInject<Inc<ShopCloseEvent>> _shopCloseEventsFilter;
    private EcsFilterInject<Inc<ShopCellComponent>> _shopCellComponentsFilter;

    public void Init(IEcsSystems systems)
    {
        foreach(var shopper in _sceneData.Value.shoppers)
        {
            int shopperEntity = _world.Value.NewEntity();

            ref var shopperCmp = ref _shopCharacterComponentsPool.Value.Add(shopperEntity);
            shopperCmp.items = shopper.shopItems;

            shopper.Construct(_world.Value, shopperEntity);
        }
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var shopOpenEvt in _shopOpenEventsFilter.Value)
        {
            var shopCharacterCmp = _shopCharacterComponentsPool.Value.Get(shopOpenEvt);

            foreach (var shopCell in shopCharacterCmp.items)
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

        foreach (var shopCloseEvt in _shopCloseEventsFilter.Value)
        {
            foreach (var shopCell in _shopCellComponentsFilter.Value)
            {
                ref var shopCellCmp = ref _shopCellComponentsPool.Value.Get(shopCell);

                _sceneData.Value.ReleaseShopCell( shopCellCmp.cellView);
                _shopCellComponentsPool.Value.Del(shopCell);
            }

        }
    }
}
