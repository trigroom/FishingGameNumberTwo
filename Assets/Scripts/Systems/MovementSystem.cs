using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementSystem : IEcsRunSystem
{
    private EcsCustomInject<SceneService> _sceneData;
    private EcsPoolInject<MovementComponent> _movementComponentPool;
    private EcsFilterInject<Inc<MovementComponent>> _movementComponents;
    public void Run(IEcsSystems systems)
    {
        foreach(var movableObject in _movementComponents.Value)
        {
           var moveCmp = _movementComponentPool.Value.Get(movableObject);
            moveCmp.movementView.MoveUnit(moveCmp.moveSpeed* moveCmp.moveInput* Time.deltaTime);
        }
        //передвижение объекта
    }
}
