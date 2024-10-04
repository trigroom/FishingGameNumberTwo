using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathSystem : IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<DroppedItemComponent> _droppedItemComponent;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<CreatureDropComponent> _creatureDropComponentsPool;
   // private EcsPoolInject<EnemyDeathEvent> _enemyDeathEventComponents;


    private EcsFilterInject<Inc<EnemyDeathEvent>> _enemyDeathEventFilter;

    public void Run(IEcsSystems systems)
    {
        foreach (var item in _enemyDeathEventFilter.Value) 
        {
            DropItem(item, 2);
        }

        if(Input.GetKeyDown(KeyCode.K)) 
        {
            DropItem(1, 2);
        }
    }

    private void DropItem(int enemy, int itemsCount)
    {
        var droppedItem = _world.Value.NewEntity();

        ref var droppedItemComponent =  ref _droppedItemComponent.Value.Add(droppedItem);

        droppedItemComponent.currentItemsCount = itemsCount;
        //правильный
        /*  var enemyCmp = _enemyComponents.Value.Get(enemy);
         *  droppedItemComponent.itemInfo = enemyCmp.itemInfo
         droppedItemComponent.droppedItemView =  _sceneData.Value.SpawnDroppedItem(_enemyDeathEventComponents.Value.Get(enemy).deathPosition, enemyCmp.enemyView.droopedItem,droppedItem);*/

        //для теста
        droppedItemComponent.itemInfo = _sceneData.Value.testItem1;
        droppedItemComponent.droppedItemView =_sceneData.Value.SpawnDroppedItem( Vector2.zero,_sceneData.Value.testItem1, droppedItem);
    }
}
