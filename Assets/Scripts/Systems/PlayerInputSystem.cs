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
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    //private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    private EcsPoolInject<ArmorComponent> _armorComponentsPool;
    private EcsPoolInject<CameraComponent> _cameraComponentsPool;
    private EcsPoolInject<CurrentHealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<HealFromHealItemCellEvent> _healFromHealItemCellEventsPool;
    private EcsPoolInject<InventoryCellComponent> _inventoryCellComponentsPool;
    private EcsPoolInject<FlashLightInInventoryComponent> _flashLightInInventoryComponentsPool;

    private EcsCustomInject<SceneService> _sceneService;

    private EcsWorldInject _world;

    private int _playerEntity;

    public void Init(IEcsSystems systems)
    {
        _playerEntity = _world.Value.NewEntity();

        _menuStatesComponentsPool.Value.Add(_playerEntity);

        _playerGunComponentsPool.Value.Add(_playerEntity);


        ref var playerCmp = ref _playerComponentsPool.Value.Add(_playerEntity);
        playerCmp.view = _sceneService.Value.SpawnPlayer(_world.Value, _playerEntity);
        playerCmp.money = _sceneService.Value.startMoneyForTest;
        //playerCmp.visionZoneCollider = playerCmp.view.playerInputView.visionZoneCollider;

        ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Add(_playerEntity);
        //Брать оружия из сэйва
        //weaponsInInventoryCmp.gunFirstObject = _sceneService.Value.firstWeaponTest;
        //weaponsInInventoryCmp.gunSecondObject = _sceneService.Value.secondWeaponTest;
        //weaponsInInventoryCmp.curWeapon = 0;
        ref var cameraCmp = ref _cameraComponentsPool.Value.Add(_playerEntity);
        cameraCmp.cursorPositonPart = 1;
        cameraCmp.playerPositonPart = 6;

        ref var attackCmp = ref _currentAttackComponentsPool.Value.Add(_playerEntity);

        _currentHealingItemComponentsPool.Value.Add(_playerEntity);

        attackCmp.weaponIsChanged = false;
        attackCmp.canAttack = true;

        ref var gunCmp = ref _gunComponentsPool.Value.Add(_playerEntity);

        ref var healthCmp = ref _healthComponentsPool.Value.Add(_playerEntity);
        healthCmp.healthView = playerCmp.view.healthView;
        //healthCmp.healthPoint = healthCmp.healthView.maxHealth;//для тестов
        healthCmp.healthPoint = 2;
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

        gunCmp.firePoint = movementComponent.movementView.firePoint;//временно, потом разделить точку спавна и точку стрельбы
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

        var healthCmp = _healthComponentsPool.Value.Get(_playerEntity);

        moveCmp.pointToRotateInput = _sceneService.Value.mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.H) && !_currentHealingItemComponentsPool.Value.Get(_playerEntity).isHealing && !_currentAttackComponentsPool.Value.Get(_playerEntity).weaponIsChanged && !_playerGunComponentsPool.Value.Get(_playerEntity).inScope && !_gunComponentsPool.Value.Get(_playerEntity).isReloading && healthCmp.maxHealthPoint != healthCmp.healthPoint && !_inventoryCellComponentsPool.Value.Get(_sceneService.Value.healingItemCellView._entity).isEmpty) //возможно что то ещё
        {
            _healFromHealItemCellEventsPool.Value.Add(_playerEntity);
            // _sceneService.Value.ammoInfoText.text = "восстановление здоровья...";
        }
        //Ниже использование фонарика
        else if (Input.GetKeyDown(KeyCode.L) && _playerComponentsPool.Value.Get(_sceneService.Value.playerEntity).canUseFlashlight)
        {
            ref var playerCmp = ref _playerComponentsPool.Value.Get(_sceneService.Value.playerEntity);
            playerCmp.useFlashlight = !playerCmp.useFlashlight;
            playerCmp.view.flashLightObject.gameObject.SetActive(playerCmp.useFlashlight);
        }

        if (!_inventoryCellComponentsPool.Value.Get(_sceneService.Value.flashlightItemCellView._entity).isEmpty)
        {
            ref var flashlightCmp = ref _flashLightInInventoryComponentsPool.Value.Get(_sceneService.Value.flashlightItemCellView._entity);
            ref var playerCmp = ref _playerComponentsPool.Value.Get(_sceneService.Value.playerEntity);

            if (flashlightCmp.currentChargeRemainigTime > 0 && playerCmp.useFlashlight)
                flashlightCmp.currentChargeRemainigTime -= Time.deltaTime;

            else if (playerCmp.useFlashlight && flashlightCmp.currentChargeRemainigTime <= 0)
            {
                playerCmp.useFlashlight = false;
                playerCmp.canUseFlashlight = false;
                flashlightCmp.currentChargeRemainigTime = 0;
                playerCmp.view.flashLightObject.gameObject.SetActive(false);
            }
        }
    }

}
