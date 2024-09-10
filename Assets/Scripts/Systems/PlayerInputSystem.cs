using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class PlayerInputSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentPool;
    private EcsPoolInject<PlayerInputsComponent> _playerInputsComponentsPool;

    private EcsCustomInject<SceneService> _sceneService;

    private EcsWorldInject _world;

    private int _playerEntity;

    public void Init(IEcsSystems systems)
    {
        _playerEntity = _world.Value.NewEntity();

        ref var playerCmp = ref _playerComponentsPool.Value.Add(_playerEntity);
        playerCmp.view = _sceneService.Value.SpawnPlayer(_world.Value, _playerEntity);
        playerCmp.money = _sceneService.Value.startMoneyForTest;

        ref var movementComponent = ref _movementComponentPool.Value.Add(_playerEntity);
        movementComponent.movementView = playerCmp.view.movementView;
        movementComponent.moveSpeed = movementComponent.movementView.moveSpeed;

        _playerInputsComponentsPool.Value.Add(_playerEntity);

    }

    public void Run(IEcsSystems systems)
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 moveDirection = new Vector3(horizontalInput, verticalInput).normalized;

        _movementComponentPool.Value.Get(_playerEntity).moveInput = moveDirection;
    }

}
