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

        ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Add(_playerEntity);
        weaponsInInventoryCmp.gunFirstObject = _sceneService.Value.firstWeaponTest;
        weaponsInInventoryCmp.gunSecondObject = _sceneService.Value.secondWeaponTest;
        weaponsInInventoryCmp.curWeapon = 0;

        ref var attackCmp = ref _currentAttackComponentsPool.Value.Add(_playerEntity);

        attackCmp.weaponIsChanged = false;
        attackCmp.damage = weaponsInInventoryCmp.gunFirstObject.damage;
        attackCmp.changeWeaponTime = playerCmp.view.changeWeaponTime;

        ref var gunCmp = ref _gunComponentsPool.Value.Add(_playerEntity);
        gunCmp.attackCouldown = weaponsInInventoryCmp.gunFirstObject.attackCouldown;
        gunCmp.currentMagazineCapacity = weaponsInInventoryCmp.gunFirstObject.magazineCapacity;
        gunCmp.magazineCapacity = gunCmp.currentMagazineCapacity;
        gunCmp.reloadDuration = weaponsInInventoryCmp.gunFirstObject.reloadDuration;
        gunCmp.attackLeght = weaponsInInventoryCmp.gunFirstObject.attackLenght;
        gunCmp.maxSpread = weaponsInInventoryCmp.gunFirstObject.maxSpread;
        gunCmp.minSpread = weaponsInInventoryCmp.gunFirstObject.minSpread;
        gunCmp.currentSpread = weaponsInInventoryCmp.gunFirstObject.minSpread;//временно так
        gunCmp.spreadRecoverySpeed = weaponsInInventoryCmp.gunFirstObject.spreadRecoverySpeed;
        gunCmp.addedSpread = weaponsInInventoryCmp.gunFirstObject.addedSpread;
        gunCmp.isAuto = weaponsInInventoryCmp.gunFirstObject.isAuto;


        ref var healtCmp = ref _healthComponentsPool.Value.Add(_playerEntity);
        healtCmp.healthView = playerCmp.view.healthView;
        healtCmp.healthPoint = healtCmp.healthView.maxHealth;
        healtCmp.maxHealthPoint = healtCmp.healthView.maxHealth;

        //задать переменные на текущие оружие

        ref var movementComponent = ref _movementComponentPool.Value.Add(_playerEntity);
        movementComponent.movementView = playerCmp.view.movementView;
        movementComponent.moveSpeed = movementComponent.movementView.moveSpeed;
        movementComponent.entityTransform = movementComponent.movementView.objectTransform;

        gunCmp.firePoint = movementComponent.movementView.firePoint;//временно, потом разделить точку спавна и точку стрельбы
        gunCmp.weaponContainer = movementComponent.movementView.weaponContainer;

        _playerInputsComponentsPool.Value.Add(_playerEntity);

    }

    public void Run(IEcsSystems systems)
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var enemy = _world.Value.NewEntity();
            ref var enemyHealthCmp = ref _healthComponentsPool.Value.Add(enemy);
            enemyHealthCmp.healthView = _sceneService.Value.GetEnemy();
            enemyHealthCmp.healthView.Construct(enemy);
            enemyHealthCmp.maxHealthPoint = enemyHealthCmp.healthView.maxHealth;
            enemyHealthCmp.healthPoint = enemyHealthCmp.maxHealthPoint;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 moveDirection = new Vector3(horizontalInput, verticalInput).normalized;

        ref var moveCmp = ref _movementComponentPool.Value.Get(_playerEntity);
        moveCmp.moveInput = moveDirection;

        moveCmp.pointToRotateInput = _sceneService.Value.mainCamera.ScreenToWorldPoint(Input.mousePosition);

    }

}
