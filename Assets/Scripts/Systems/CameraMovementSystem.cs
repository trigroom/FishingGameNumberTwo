using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementSystem : IEcsRunSystem
{
    private EcsCustomInject<SceneService> _sceneService;
    private EcsWorldInject _world;

    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    //private EcsPoolInject<PlayerComponent> _playerComponentsPool;

    private EcsFilterInject<Inc<PlayerComponent>> _playerComponentsFilter;
    public void Run(IEcsSystems systems)
    {
        foreach(var player in _playerComponentsFilter.Value)
        {
            var moveCmp = _movementComponentsPool.Value.Get(player);
            //var playerCmp = _playerComponentsPool.Value.Get(player);
           // var ray = new Ray2D(moveCmp.movementView.objectTransform.position, moveCmp.movementView.objectTransform.up);
           // _sceneService.Value.mainCamera.transform.position = new Vector3(ray.origin.x + (ray.direction.x * 1), ray.origin.y + (ray.direction.y * 1),-10);
            Vector3 targetPosition = new Vector3((moveCmp.entityTransform.position.x*6 + moveCmp.pointToRotateInput.x)/7, (moveCmp.entityTransform.position.y * 6 + moveCmp.pointToRotateInput.y) / 7, -10);
            //_sceneService.Value.mainCamera.transform.position = Vector3.MoveTowards(_sceneService.Value.mainCamera.transform.position, targetPosition, _sceneService.Value.cameraMoveSpeed * Time.deltaTime);
            _sceneService.Value.mainCamera.transform.position = targetPosition;
        }
    }
}
