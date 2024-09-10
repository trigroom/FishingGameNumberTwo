using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiControlSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsCustomInject<SceneService> _sceneData;
    private EcsWorldInject _world;

    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentPool;
    private EcsPoolInject<SetDescriptionItemEvent> _setDescriptionItemEventsPool;
    private EcsPoolInject<MenuStatesComponent> _menuStatesComponentsPool;

    private EcsFilterInject<Inc<SetDescriptionItemEvent>> _setDescriptionItemEvents;

    public void Init(IEcsSystems systems)
    {
        _sceneData.Value.dropedItemsUIView.Construct(_world.Value);
    }

    public void Run(IEcsSystems systems)
    {
        foreach(var desription in _setDescriptionItemEvents.Value)
        {
            var descriptionEvt =_setDescriptionItemEventsPool.Value.Get(desription);
            var item = _inventoryItemComponentPool.Value.Get(descriptionEvt.itemEntity);
                
            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(true);
            _sceneData.Value.hoverDescriptionText.text = item.itemInfo.itemName + "\n" +"вес "+ item.itemInfo.itemWeight;
            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(item.currentItemsCount, descriptionEvt.itemEntity);
        }

        if(Input.GetKeyDown(KeyCode.I) || (Input.GetKeyDown(KeyCode.Escape) && _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inInventoryState))
        {
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            menusStatesCmp.inInventoryState = !menusStatesCmp.inInventoryState;
            _sceneData.Value.inventoryMenuView.ChangeMenuState( menusStatesCmp.inInventoryState);
        }
    }
}
