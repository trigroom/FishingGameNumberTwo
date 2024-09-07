using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiControlSystem : IEcsRunSystem
{
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<SetDescriptionItemEvent> _setDescriptionItemEventsPool;

    private EcsFilterInject<Inc<SetDescriptionItemEvent>> _setDescriptionItemEvents;
    public void Run(IEcsSystems systems)
    {
        foreach(var desription in _setDescriptionItemEvents.Value)
        {
            var descriptionEvt =_setDescriptionItemEventsPool.Value.Get(desription);
            var item = descriptionEvt.itemInfo;
                
            _sceneData.Value.hoverDescriptionText.text = item.itemName + "\n" +"вес "+ item.itemWeight;
        }
    }
}
