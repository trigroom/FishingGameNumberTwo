using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class CameraMovementSystem : IEcsRunSystem
{
    private EcsCustomInject<SceneService> _sceneService;
    private EcsWorldInject _world;

    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<PlayerMoveComponent> _playerMovementComponentsPool;
    private EcsPoolInject<CameraComponent> _cameraComponentsPool;

    private EcsFilterInject<Inc<CameraComponent>> _cameraComponentsFilter;
    public void Run(IEcsSystems systems)
    {
        foreach (var player in _cameraComponentsFilter.Value)
        {
            ref var cameraCmp = ref _cameraComponentsPool.Value.Get(player);
            var moveCmp = _movementComponentsPool.Value.Get(player);
            var playerMoveCmp = _playerMovementComponentsPool.Value.Get(player);
            Vector3 neededCameraPosition;
            Vector3 targetPosition = new Vector3((moveCmp.entityTransform.position.x * cameraCmp.playerPositonPart + moveCmp.pointToRotateInput.x * cameraCmp.cursorPositonPart) / (cameraCmp.playerPositonPart + cameraCmp.cursorPositonPart), (moveCmp.entityTransform.position.y * cameraCmp.playerPositonPart + moveCmp.pointToRotateInput.y * cameraCmp.cursorPositonPart) / (cameraCmp.playerPositonPart + cameraCmp.cursorPositonPart), -10);

            if (playerMoveCmp.isRun)
            {
                cameraCmp.needRunCameraOffset = moveCmp.moveInput * cameraCmp.runCameraOffsetLenght * 4f;
                cameraCmp.currentRunCameraOffset = Vector3.MoveTowards(cameraCmp.currentRunCameraOffset, cameraCmp.needRunCameraOffset, 0.005f);
                neededCameraPosition = new Vector3(targetPosition.x + cameraCmp.currentRunCameraOffset.x, targetPosition.y + cameraCmp.currentRunCameraOffset.y, -10);

                if (moveCmp.moveInput != Vector2.zero)
                    cameraCmp.lastPlayerInput = moveCmp.moveInput;
                if (cameraCmp.runCameraOffsetLenght < 0.4f)
                    cameraCmp.runCameraOffsetLenght += Time.deltaTime * 0.1f;
            }
            else if (cameraCmp.runCameraOffsetLenght > 0)
            {
                cameraCmp.needRunCameraOffset = moveCmp.moveInput * cameraCmp.runCameraOffsetLenght * 4f;
                cameraCmp.currentRunCameraOffset = Vector3.MoveTowards(cameraCmp.currentRunCameraOffset, cameraCmp.needRunCameraOffset, 0.02f);
                neededCameraPosition = new Vector3(targetPosition.x + cameraCmp.currentRunCameraOffset.x, targetPosition.y + cameraCmp.currentRunCameraOffset.y, -10);

                cameraCmp.runCameraOffsetLenght -= Time.deltaTime * 0.15f;
            }
            else
            {
                if (cameraCmp.runCameraOffsetLenght > 0)
                    cameraCmp.runCameraOffsetLenght -= Time.deltaTime * 0.2f;
                neededCameraPosition = targetPosition;
            }
            _sceneService.Value.mainCamera.transform.position = Vector3.MoveTowards(_sceneService.Value.mainCamera.transform.position, neededCameraPosition, 0.1f);

        }
    }
}
