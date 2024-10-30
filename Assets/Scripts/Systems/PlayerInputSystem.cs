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
    private EcsPoolInject<AttackComponent> _currentAttackComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    // private EcsPoolInject<DataComponent> _dataComponentsPool;
    //private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    private EcsPoolInject<ArmorComponent> _armorComponentsPool;
    private EcsPoolInject<CameraComponent> _cameraComponentsPool;
    private EcsPoolInject<HealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<HealFromHealItemCellEvent> _healFromHealItemCellEventsPool;
    private EcsPoolInject<InventoryCellComponent> _inventoryCellComponentsPool;
    private EcsPoolInject<PlayerMoveComponent> _playerMoveComponentsPool;
    private EcsPoolInject<FlashLightInInventoryComponent> _flashLightInInventoryComponentsPool;
    private EcsPoolInject<SolarPanelElectricGeneratorComponent> _solarPanelElectricGeneratorComponentsPool;
    private EcsPoolInject<MeleeWeaponComponent> _meleeWeaponComponentsPool;
    private EcsPoolInject<SaveGameEvent> _saveGameEventsPool;//для тестов

    private EcsFilterInject<Inc<LoadGameEvent>> loadGameEventsFilter;

    private EcsCustomInject<SceneService> _sceneService;

    private EcsWorldInject _world;

    private int _playerEntity;

    public void Init(IEcsSystems systems)
    {
        _playerEntity = _world.Value.NewEntity();

        _menuStatesComponentsPool.Value.Add(_playerEntity);

        _playerGunComponentsPool.Value.Add(_playerEntity);


        _solarPanelElectricGeneratorComponentsPool.Value.Add(_playerEntity);
        ref var playerCmp = ref _playerComponentsPool.Value.Add(_playerEntity);
        playerCmp.view = _sceneService.Value.SpawnPlayer(_world.Value, _playerEntity);
        //playerCmp.money = _sceneService.Value.startMoneyForTest;
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

        playerCmp.view.meleeColliderView.Construct(_world.Value, _playerEntity);

        ref var playerMoveCmp = ref _playerMoveComponentsPool.Value.Add(_playerEntity);
        playerMoveCmp.playerView = playerCmp.view;
        playerMoveCmp.currentRunTime = playerMoveCmp.playerView.runTime;

        _sceneService.Value.playerStaminaText.text = playerMoveCmp.currentRunTime.ToString("0.0") + "/" + playerMoveCmp.playerView.runTime;

        ref var gunCmp = ref _gunComponentsPool.Value.Add(_playerEntity);

        ref var healthCmp = ref _healthComponentsPool.Value.Add(_playerEntity);
        healthCmp.healthView = playerCmp.view.healthView;
        playerCmp.view.healthView.Construct(_playerEntity);
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

        _meleeWeaponComponentsPool.Value.Add(_playerEntity).startHitPoint = playerCmp.view.movementView.weaponContainer.localPosition;

        _playerInputsComponentsPool.Value.Add(_playerEntity);

    }

    public void Run(IEcsSystems systems)
    {
        foreach (var loadGame in loadGameEventsFilter.Value)
        {
            ref var playerCmp = ref _playerComponentsPool.Value.Get(_playerEntity);
            /* playerCmp.money = _dataComponentsPool.Value.Get(loadGame).money;*/
            _sceneService.Value.moneyText.text = playerCmp.money + "$";
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            _playerComponentsPool.Value.Get(_playerEntity).money++;
            Debug.Log("now " + _playerComponentsPool.Value.Get(_playerEntity).money + " money");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            /* foreach (var save in _dataComponentsFilter.Value)
             {
                 _dataComponentsPool.Value.Get(save).money = _playerComponentsPool.Value.Get(_playerEntity).money;

                 var item = new ItemInfoForSaveData(5,5);

                 ItemInfoForSaveData[] items = new ItemInfoForSaveData[] { item , item , item };

                 _dataComponentsPool.Value.Get(save).itemsCellinfo = items;
             }*/
            _saveGameEventsPool.Value.Add(_playerEntity);
            Debug.Log("save" + _playerComponentsPool.Value.Get(_playerEntity).money + " money");
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 moveDirection = new Vector3(horizontalInput, verticalInput).normalized;

        ref var playerMoveCmp = ref _playerMoveComponentsPool.Value.Get(_sceneService.Value.playerEntity);
        ref var moveCmp = ref _movementComponentPool.Value.Get(_playerEntity);
        if (!moveCmp.isStunned)
        {
            moveCmp.moveInput = moveDirection;


            if (playerMoveCmp.isRun)
            {
                playerMoveCmp.currentRunTime -= Time.deltaTime;
                if (playerMoveCmp.currentRunTime <= 0 || moveDirection == Vector2.zero)
                {
                    //playerMoveCmp.currentRunTime = 0;
                    playerMoveCmp.isRun = false;
                    moveCmp.moveSpeed /= playerMoveCmp.playerView.runSpeedMultiplayer;
                }
                _sceneService.Value.playerStaminaBarFilled.fillAmount = playerMoveCmp.currentRunTime / playerMoveCmp.playerView.runTime;
                _sceneService.Value.playerStaminaText.text = playerMoveCmp.currentRunTime.ToString("0.0") + "/" + playerMoveCmp.playerView.runTime;
            }
            else if (playerMoveCmp.currentRunTime < playerMoveCmp.playerView.runTime)
            {
                playerMoveCmp.currentRunTime += Time.deltaTime * playerMoveCmp.playerView.runTimeRecoverySpeed;
                _sceneService.Value.playerStaminaBarFilled.fillAmount = playerMoveCmp.currentRunTime / playerMoveCmp.playerView.runTime;
                _sceneService.Value.playerStaminaText.text = playerMoveCmp.currentRunTime.ToString("0.0") + "/" + playerMoveCmp.playerView.runTime;
            }
            else if (playerMoveCmp.currentRunTime > playerMoveCmp.playerView.runTime)
                playerMoveCmp.currentRunTime = playerMoveCmp.playerView.runTime;
            // Debug.Log(playerMoveCmp.currentRunTime);
            //обновление шкалы стамины


            moveCmp.pointToRotateInput = _sceneService.Value.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        var healthCmp = _healthComponentsPool.Value.Get(_playerEntity);

        if (Input.GetKeyDown(KeyCode.H) && !_currentHealingItemComponentsPool.Value.Get(_playerEntity).isHealing && !_currentAttackComponentsPool.Value.Get(_playerEntity).weaponIsChanged && !_playerGunComponentsPool.Value.Get(_playerEntity).inScope && !_gunComponentsPool.Value.Get(_playerEntity).isReloading && healthCmp.maxHealthPoint != healthCmp.healthPoint && !_inventoryCellComponentsPool.Value.Get(_sceneService.Value.healingItemCellView._entity).isEmpty) //возможно что то ещё
        {
            _healFromHealItemCellEventsPool.Value.Add(_playerEntity);
            // _sceneService.Value.ammoInfoText.text = "восстановление здоровья...";
        }
        //Ниже использование фонарика
        /*else if  &&/* _playerComponentsPool.Value.Get(_sceneService.Value.playerEntity).canUseFlashlight !_inventoryCellComponentsPool.Value.Get(_sceneService.Value.flashlightItemCellView._entity).isEmpty)
        {
            ref var playerCmp = ref _playerComponentsPool.Value.Get(_sceneService.Value.playerEntity);
            playerCmp.useFlashlight = !playerCmp.useFlashlight;
            playerCmp.view.flashLightObject.gameObject.SetActive(playerCmp.useFlashlight);
        }*/
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {

            playerMoveCmp.isRun = !playerMoveCmp.isRun;
            if (playerMoveCmp.isRun)
                moveCmp.moveSpeed *= playerMoveCmp.playerView.runSpeedMultiplayer;
            else
                moveCmp.moveSpeed /= playerMoveCmp.playerView.runSpeedMultiplayer;
        }

        if (!_inventoryCellComponentsPool.Value.Get(_sceneService.Value.flashlightItemCellView._entity).isEmpty)
        {
            ref var flashlightCmp = ref _flashLightInInventoryComponentsPool.Value.Get(_sceneService.Value.flashlightItemCellView._entity);
            ref var playerCmp = ref _playerComponentsPool.Value.Get(_sceneService.Value.playerEntity);

            if (flashlightCmp.currentChargeRemainigTime > 0)
            {
                if (playerCmp.useFlashlight)
                    flashlightCmp.currentChargeRemainigTime -= Time.deltaTime;
                if (flashlightCmp.currentChargeRemainigTime <= 0)
                {
                    playerCmp.useFlashlight = false;
                    flashlightCmp.currentChargeRemainigTime = 0;
                    playerCmp.view.flashLightObject.gameObject.SetActive(false);
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    playerCmp.useFlashlight = !playerCmp.useFlashlight;
                    playerCmp.view.flashLightObject.gameObject.SetActive(playerCmp.useFlashlight);
                }
            }
        }
    }
}
