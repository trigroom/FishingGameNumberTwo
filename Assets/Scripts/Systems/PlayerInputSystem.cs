using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class PlayerInputSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentPool;
    private EcsPoolInject<PlayerInputsComponent> _playerInputsComponentsPool;
    private EcsPoolInject<MenuStatesComponent> _menuStatesComponentsPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<CurrentAttackComponent> _currentAttackComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<ArmorComponent> _armorComponentsPool;

    private EcsCustomInject<SceneService> _sceneService;

    private EcsWorldInject _world;

    private int _playerEntity;

    public void Init(IEcsSystems systems)
    {
        _playerEntity = _world.Value.NewEntity();

        _menuStatesComponentsPool.Value.Add(_playerEntity);


        ref var playerCmp = ref _playerComponentsPool.Value.Add(_playerEntity);
        playerCmp.view = _sceneService.Value.SpawnPlayer(_world.Value, _playerEntity);
        playerCmp.money = _sceneService.Value.startMoneyForTest; 
        playerCmp.visionZoneCollider = playerCmp.view.visionZoneCollider;

        ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Add(_playerEntity);
        //����� ������ �� �����
        //weaponsInInventoryCmp.gunFirstObject = _sceneService.Value.firstWeaponTest;
        //weaponsInInventoryCmp.gunSecondObject = _sceneService.Value.secondWeaponTest;
        //weaponsInInventoryCmp.curWeapon = 0;

        ref var attackCmp = ref _currentAttackComponentsPool.Value.Add(_playerEntity);

        attackCmp.weaponIsChanged = false;
        attackCmp.canAttack = true;

        ref var gunCmp = ref _gunComponentsPool.Value.Add(_playerEntity);

        ref var healthCmp = ref _healthComponentsPool.Value.Add(_playerEntity);
        healthCmp.healthView = playerCmp.view.healthView;
        healthCmp.healthPoint = healthCmp.healthView.maxHealth;
        healthCmp.maxHealthPoint = healthCmp.healthView.maxHealth;

        ref var armorCmp = ref _armorComponentsPool.Value.Add(_playerEntity);
        armorCmp.maxArmorPoint = _sceneService.Value.playerStartArmor;
        armorCmp.armorPoint = armorCmp.maxArmorPoint;
        armorCmp.armorPoint = 0;
        armorCmp.armorRecoverySpeed = _sceneService.Value.playerStartArmorRecoverySpeed;

        ref var movementComponent = ref _movementComponentPool.Value.Add(_playerEntity);
        movementComponent.movementView = playerCmp.view.movementView;
        movementComponent.moveSpeed = movementComponent.movementView.moveSpeed;
        movementComponent.entityTransform = movementComponent.movementView.objectTransform;
        movementComponent.canMove = true;

        gunCmp.firePoint = movementComponent.movementView.firePoint;//��������, ����� ��������� ����� ������ � ����� ��������
        gunCmp.weaponContainer = movementComponent.movementView.weaponContainer;

        _playerInputsComponentsPool.Value.Add(_playerEntity);

    }

    public void Run(IEcsSystems systems)
    {

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 moveDirection = new Vector3(horizontalInput, verticalInput).normalized;

        ref var moveCmp = ref _movementComponentPool.Value.Get(_playerEntity);
        moveCmp.moveInput = moveDirection;

        moveCmp.pointToRotateInput = _sceneService.Value.mainCamera.ScreenToWorldPoint(Input.mousePosition);

    }

}
