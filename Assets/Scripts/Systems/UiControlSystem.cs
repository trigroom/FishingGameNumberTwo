using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class UiControlSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsCustomInject<SceneService> _sceneData;
    private EcsWorldInject _world;

    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentPool;
    private EcsPoolInject<SetDescriptionItemEvent> _setDescriptionItemEventsPool;
    private EcsPoolInject<MenuStatesComponent> _menuStatesComponentsPool;
    private EcsPoolInject<ShopCloseEvent> _shopCloseEventsPool;
    private EcsPoolInject<PlayerComponent> _playerComponent;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<CurrentAttackComponent> _currentAttackComponentsPool;

    private EcsFilterInject<Inc<SetDescriptionItemEvent>> _setDescriptionItemEvents;
    private EcsFilterInject<Inc<ShopOpenEvent>> _shopOpenEvents;

    public void Init(IEcsSystems systems)
    {
        _sceneData.Value.dropedItemsUIView.Construct(_world.Value);
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var shopOpen in _shopOpenEvents.Value)
        {
            Debug.Log("shop open");
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            ChangeInventoryMenuState(ref menusStatesCmp);
            ChangeShopMenuState(ref menusStatesCmp);
        }

        foreach (var desription in _setDescriptionItemEvents.Value)
        {
            var descriptionEvt = _setDescriptionItemEventsPool.Value.Get(desription);
            var item = _inventoryItemComponentPool.Value.Get(descriptionEvt.itemEntity);

            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(true);
            _sceneData.Value.hoverDescriptionText.text = item.itemInfo.itemName + "\n" + "вес " + item.itemInfo.itemWeight;
            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(item.currentItemsCount, descriptionEvt.itemEntity);
        }


        if (Input.GetKeyDown(KeyCode.I) || (Input.GetKeyDown(KeyCode.Escape) && _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inInventoryState))
        {
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            ChangeInventoryMenuState(ref menusStatesCmp);

            if (menusStatesCmp.inShopState)
            {
                ChangeShopMenuState(ref menusStatesCmp);
                _shopCloseEventsPool.Value.Add(_world.Value.NewEntity());
                _playerComponent.Value.Get(_sceneData.Value.playerEntity).view.canShoping = true;
            }
        }
    }

    private void ChangeInventoryMenuState(ref MenuStatesComponent menusStatesCmp)
    {
        menusStatesCmp.inInventoryState = !menusStatesCmp.inInventoryState;
        _sceneData.Value.inventoryMenuView.ChangeMenuState(menusStatesCmp.inInventoryState);

        _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).canMove = !menusStatesCmp.inInventoryState;
        _currentAttackComponentsPool.Value.Get(_sceneData.Value.playerEntity).canAttack = !menusStatesCmp.inInventoryState;
        //изменение состояния стрелбы и ходьбы
    }

    private void ChangeShopMenuState(ref MenuStatesComponent menusStatesCmp)
    {
        menusStatesCmp.inShopState = !menusStatesCmp.inShopState;
        _sceneData.Value.shopMenuView.ChangeMenuState(menusStatesCmp.inShopState);
    }
}
