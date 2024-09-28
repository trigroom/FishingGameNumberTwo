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
    private EcsPoolInject<CameraComponent> _cameraComponentsPool;

    private EcsFilterInject<Inc<CameraComponent>> _cameraComponentsFilter;
    public void Run(IEcsSystems systems)
    {
        foreach(var player in _cameraComponentsFilter.Value)
        {
            var cameraCmp = _cameraComponentsPool.Value.Get(player);
            var moveCmp = _movementComponentsPool.Value.Get(player);
            Vector3 targetPosition = new Vector3((moveCmp.entityTransform.position.x* cameraCmp.playerPositonPart + moveCmp.pointToRotateInput.x* cameraCmp.cursorPositonPart) / (cameraCmp.playerPositonPart + cameraCmp.cursorPositonPart), (moveCmp.entityTransform.position.y * cameraCmp.playerPositonPart + moveCmp.pointToRotateInput.y * cameraCmp.cursorPositonPart) / (cameraCmp.playerPositonPart+cameraCmp.cursorPositonPart), -10);
           // var ray = new Ray2D(moveCmp.movementView.objectTransform.position, moveCmp.movementView.objectTransform.up);
           // Vector3 targetPosition = new Vector3(((ray.origin.x + (ray.direction.x * cameraCmp.cursorPositonPart* cameraCmp.cursorPositonPart)) * 6 + moveCmp.pointToRotateInput.x) / 7, ((ray.origin.y + (ray.direction.y * cameraCmp.cursorPositonPart* cameraCmp.cursorPositonPart)) * 6 + moveCmp.pointToRotateInput.y) / 7, -10);
            _sceneService.Value.mainCamera.transform.position = targetPosition;


            //var playerCmp = _playerComponentsPool.Value.Get(player);
            // _sceneService.Value.mainCamera.transform.position = new Vector3(ray.origin.x + (ray.direction.x * 1), ray.origin.y + (ray.direction.y * 1),-10);
            //_sceneService.Value.mainCamera.transform.position = Vector3.MoveTowards(_sceneService.Value.mainCamera.transform.position, targetPosition, _sceneService.Value.cameraMoveSpeed * Time.deltaTime);
        }
    }
}
