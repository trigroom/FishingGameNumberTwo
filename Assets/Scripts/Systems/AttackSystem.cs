using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class AttackSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<CurrentAttackComponent> _attackComponentsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<BulletTracerLifetimeComponent> _bulletTracerLifetimeComponentsPool;
    private EcsPoolInject<EndReloadEvent> _endReloadEventsPool;
    private EcsPoolInject<ReloadEvent> _reloadEventsPool;
    private EcsPoolInject<ChangeHealthEvent> _changeHealthEventsPool;
    private EcsPoolInject<ChangeWeaponFromInventoryEvent> _changeWeaponFromInventoryEventsPool;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<CameraComponent> _cameraComponentsPool;
    private EcsPoolInject<CurrentHealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;
    private EcsPoolInject<NowUsedWeaponTag> _nowUsedWeaponTagsPool;

    private EcsFilterInject<Inc<EndReloadEvent>> _endReloadEventFilter;
    private EcsFilterInject<Inc<PlayerComponent>> _playerComponentFilter;
    private EcsFilterInject<Inc<GunComponent, CreatureAIComponent>> _gunCreatureAiComponentsFilter;
    private EcsFilterInject<Inc<BulletTracerLifetimeComponent>> _bulletTracerLifetimeComponentFilter;
    private EcsFilterInject<Inc<ChangeWeaponFromInventoryEvent>> _changeWeaponFromInventoryEventsFilter;
    public void Init(IEcsSystems systems)
    {
        var gunCmp = _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
    }
    public void Run(IEcsSystems systems)
    {
        foreach (var aiCreature in _gunCreatureAiComponentsFilter.Value)
        {
            ref var creatureAi = ref _creatureAIComponentsPool.Value.Get(aiCreature);
            ref var gunCmp = ref _gunComponentsPool.Value.Get(aiCreature);
            ref var attackCmp = ref _attackComponentsPool.Value.Get(aiCreature);
            Debug.Log(gunCmp.currentSpread + "curSpread " + gunCmp.currentMinSpread + "minSpread ");

            gunCmp.currentAttackCouldown += Time.deltaTime;

            if (creatureAi.currentState == CreatureAIComponent.CreatureStates.shootingToTarget || (creatureAi.isAttackWhenRetreat && creatureAi.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget))
            {
                if (!gunCmp.isReloading && gunCmp.currentAttackCouldown >= gunCmp.attackCouldown && gunCmp.currentMagazineCapacity > 0 && attackCmp.canAttack)
                {
                    gunCmp.currentAttackCouldown = 0;
                    for (int i = 0; i < gunCmp.bulletInShotCount; i++)
                        Shoot(aiCreature, LayerMask.GetMask("Player"));

                    if (gunCmp.currentSpread < gunCmp.currentMaxSpread)
                        gunCmp.currentSpread += gunCmp.currentAddedSpread;

                    gunCmp.currentAttackCouldown = 0;
                    gunCmp.currentMagazineCapacity--;


                    if (gunCmp.currentMagazineCapacity == 0)
                        gunCmp.isReloading = true;
                }
            }
            else if ((creatureAi.currentState == CreatureAIComponent.CreatureStates.follow && gunCmp.currentMagazineCapacity < gunCmp.magazineCapacity / 2) || (creatureAi.currentState == CreatureAIComponent.CreatureStates.idle && gunCmp.currentMagazineCapacity < gunCmp.magazineCapacity))//будет перезаряжать свои патроны в состоянии идла только если у его их меньше половины
            {
                gunCmp.isReloading = true;
            }

            if (gunCmp.isReloading)
            {
                gunCmp.currentReloadDuration += Time.deltaTime;
                if (gunCmp.currentReloadDuration >= gunCmp.reloadDuration)
                {
                    gunCmp.currentReloadDuration = 0;
                    gunCmp.isReloading = false;
                    gunCmp.currentMagazineCapacity = gunCmp.magazineCapacity;
                }
            }

            if (gunCmp.currentSpread > gunCmp.currentMinSpread)
                gunCmp.currentSpread -= gunCmp.spreadRecoverySpeed * Time.deltaTime;

            else if (gunCmp.currentSpread < gunCmp.currentMinSpread)
                gunCmp.currentSpread = gunCmp.currentMinSpread;
            //стрельба и презраядка врагов, и ещё стрельба если будет вклечена стребы при отступлении врага
        }


        foreach (var changeWeaponFromInvEvt in _changeWeaponFromInventoryEventsFilter.Value)
        {
            ref var changeWeaponFromInvCmp = ref _changeWeaponFromInventoryEventsPool.Value.Get(changeWeaponFromInvEvt);
            if (changeWeaponFromInvCmp.weaponCellNumberToChange < 2)
            {
                ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(changeWeaponFromInvEvt);
                ref var plyerGunCmp = ref _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                ref var gunCmp = ref _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                if (plyerGunCmp.inScope)
                {
                    ChangeScopeMultiplicity();
                }
                if (changeWeaponFromInvCmp.isDeleteWeapon)
                {
                    if (changeWeaponFromInvCmp.weaponCellNumberToChange == 0)//first gun
                    {
                        if (_nowUsedWeaponTagsPool.Value.Has(_sceneData.Value.firstGunCellView._entity))
                        {
                        gunInInvCmp.currentAmmo = gunCmp.currentMagazineCapacity;//
                        gunInInvCmp.gunDurability = plyerGunCmp.durabilityPoints;
                        Debug.Log(gunInInvCmp.gunDurability + "gun db");
                        }
                        else
                        {
                            gunInInvCmp.currentAmmo = weaponsInInventoryCmp.curFirstWeaponAmmo;
                            gunInInvCmp.gunDurability = weaponsInInventoryCmp.curFirstWeaponDurability;
                        }
                        weaponsInInventoryCmp.gunFirstObject = null;
                    }

                    else //second gun
                    {
                        if (_nowUsedWeaponTagsPool.Value.Has(_sceneData.Value.secondGunCellView._entity))
                        {
                            gunInInvCmp.currentAmmo = gunCmp.currentMagazineCapacity;//
                            gunInInvCmp.gunDurability = plyerGunCmp.durabilityPoints;
                            Debug.Log(gunInInvCmp.gunDurability + "gun db");
                        }
                        else
                        {
                            gunInInvCmp.currentAmmo = weaponsInInventoryCmp.curSecondWeaponAmmo;
                            gunInInvCmp.gunDurability = weaponsInInventoryCmp.curSecondWeaponDurability;
                        }
                        weaponsInInventoryCmp.gunSecondObject = null;
                        Debug.Log(gunInInvCmp.gunDurability + "gun db");
                    }

                    if (0 != changeWeaponFromInvCmp.weaponCellNumberToChange && weaponsInInventoryCmp.gunFirstObject != null)
                        ChangeWeapon(0);
                    else if (1 != changeWeaponFromInvCmp.weaponCellNumberToChange && weaponsInInventoryCmp.gunSecondObject != null)
                        ChangeWeapon(1);
                    else
                        ChangeWeapon(2);
                }
                else
                {
                    //gunInInvCmp.gunInfo = _inventoryItemComponentsPool.Value.Get(changeWeaponFromInvEvt).itemInfo.gunInfo;//потом добавть в место где оружие будет напрямую добавляться из сохранения в быстрый слот в начале игры
                    if (changeWeaponFromInvCmp.weaponCellNumberToChange == 0)
                    {
                        weaponsInInventoryCmp.gunFirstObject = gunInInvCmp.gunInfo;
                        weaponsInInventoryCmp.curFirstWeaponAmmo = gunInInvCmp.currentAmmo;
                        weaponsInInventoryCmp.curFirstWeaponDurability = gunInInvCmp.gunDurability;
                    }
                    else
                    {
                        weaponsInInventoryCmp.gunSecondObject = gunInInvCmp.gunInfo;
                        weaponsInInventoryCmp.curSecondWeaponAmmo = gunInInvCmp.currentAmmo;
                        weaponsInInventoryCmp.curSecondWeaponDurability = gunInInvCmp.gunDurability;
                    }

                    ChangeWeapon(changeWeaponFromInvCmp.weaponCellNumberToChange);
                }

            }
            _changeWeaponFromInventoryEventsPool.Value.Del(changeWeaponFromInvEvt);
            //если милишка
        }


        foreach (var playerEntity in _playerComponentFilter.Value)
        {
            ref var gunCmp = ref _gunComponentsPool.Value.Get(playerEntity);
            ref var attackCmp = ref _attackComponentsPool.Value.Get(playerEntity);
            ref var curHealCmp = ref _currentHealingItemComponentsPool.Value.Get(playerEntity);
            ref var playerGunCmp = ref _playerGunComponentsPool.Value.Get(playerEntity);

            foreach (var reloadEvt in _endReloadEventFilter.Value)
            {
                gunCmp.isReloading = true;
                _sceneData.Value.ammoInfoText.text = "перезарядка...";
                _endReloadEventsPool.Value.Del(reloadEvt);
            }
            gunCmp.currentAttackCouldown += Time.deltaTime;
            // стрельба
            if (((playerGunCmp.isAuto && Input.GetMouseButton(0)) || (!playerGunCmp.isAuto && Input.GetMouseButtonDown(0))) && !gunCmp.isReloading && gunCmp.currentMagazineCapacity > 0 && gunCmp.currentAttackCouldown >= gunCmp.attackCouldown && !attackCmp.weaponIsChanged && attackCmp.canAttack && !curHealCmp.isHealing && playerGunCmp.durabilityPoints != 0)
            {
                int random = Random.Range(0, 101);
                if ((playerGunCmp.misfirePercent > 0 && random > playerGunCmp.misfirePercent) || playerGunCmp.misfirePercent == 0)
                {
                    for (int i = 0; i < gunCmp.bulletInShotCount; i++)
                        Shoot(playerEntity, LayerMask.GetMask("Enemy"));


                    if (gunCmp.currentSpread < gunCmp.currentMaxSpread)
                        gunCmp.currentSpread += gunCmp.currentAddedSpread;
                }

                gunCmp.currentAttackCouldown = 0;
                gunCmp.currentMagazineCapacity--;
                playerGunCmp.durabilityPoints--;

                //изменять харки и визуал от повреждения оружия
                if (playerGunCmp.durabilityPoints < playerGunCmp.gunInfo.maxDurabilityPoints * 0.6f)
                {
                    playerGunCmp.durabilityGunMultiplayer = (float)System.Math.Round(1 - ((float)playerGunCmp.durabilityPoints / ((float)playerGunCmp.gunInfo.maxDurabilityPoints * 1.6f) + 0.4f), 2);
                    playerGunCmp.misfirePercent = Mathf.FloorToInt((playerGunCmp.durabilityGunMultiplayer) * 60);

                    CalculateRecoil(ref gunCmp, playerGunCmp, false);
                }


                _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
                if (gunCmp.currentMagazineCapacity == 0 && !playerGunCmp.inScope)
                {
                    if (!gunCmp.isReloading)
                    {
                        playerGunCmp.isContinueReload = true;
                        _reloadEventsPool.Value.Add(playerEntity);
                    }
                    else
                        playerGunCmp.isContinueReload = !playerGunCmp.isContinueReload;
                }
            }
            else if (Input.GetMouseButtonDown(1) && playerGunCmp.scopeMultiplicity != 1 && !gunCmp.isReloading && !attackCmp.weaponIsChanged && attackCmp.canAttack && !curHealCmp.isHealing)
                ChangeScopeMultiplicity();

            else if (gunCmp.isReloading)
            {
                gunCmp.currentReloadDuration += Time.deltaTime;

                if (gunCmp.currentReloadDuration >= gunCmp.reloadDuration)
                {
                    gunCmp.currentReloadDuration = 0;
                    gunCmp.currentMagazineCapacity += playerGunCmp.bulletCountToReload;
                    if (gunCmp.isOneBulletReload && gunCmp.currentMagazineCapacity != gunCmp.magazineCapacity && playerGunCmp.isContinueReload)
                    {
                        _reloadEventsPool.Value.Add(playerEntity);
                        return;
                    }
                    gunCmp.isReloading = false;
                    _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
                }
            }

            else if (Input.GetKeyDown(KeyCode.R) && gunCmp.currentMagazineCapacity != gunCmp.magazineCapacity && !playerGunCmp.inScope && !curHealCmp.isHealing)
            {
                Debug.Log("try reload");
                if (!gunCmp.isReloading)
                {
                    playerGunCmp.isContinueReload = true;
                    _reloadEventsPool.Value.Add(playerEntity);
                }
                else
                    playerGunCmp.isContinueReload = !playerGunCmp.isContinueReload;
            }

            else if (!gunCmp.isReloading && !playerGunCmp.inScope && !curHealCmp.isHealing)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    ChangeWeapon(0);
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    ChangeWeapon(1);
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    ChangeWeapon(2);
            }

            if (attackCmp.weaponIsChanged)
            {
                attackCmp.currentChangeWeaponTime += Time.deltaTime;
                if (attackCmp.currentChangeWeaponTime >= attackCmp.changeWeaponTime)
                {
                    attackCmp.currentChangeWeaponTime = 0;
                    attackCmp.weaponIsChanged = false;
                    _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
                }
            }

            if (gunCmp.currentSpread > gunCmp.currentMinSpread)
                gunCmp.currentSpread -= gunCmp.spreadRecoverySpeed * Time.deltaTime;

            else if (gunCmp.currentSpread < gunCmp.currentMinSpread)
                gunCmp.currentSpread = gunCmp.currentMinSpread;

        }


        CheckBulletTracerLife();
        //для других сущностей отдельно
    }

    private void CalculateRecoil(ref GunComponent gunCmp, PlayerGunComponent playerGunComponent, bool isScopeCalculate)
    {
        gunCmp.currentAddedSpread = playerGunComponent.addedSpread + playerGunComponent.addedSpread * playerGunComponent.durabilityGunMultiplayer;
        gunCmp.currentMaxSpread = playerGunComponent.maxSpread + playerGunComponent.maxSpread * playerGunComponent.durabilityGunMultiplayer;
        gunCmp.currentMinSpread = playerGunComponent.minSpread + playerGunComponent.minSpread * playerGunComponent.durabilityGunMultiplayer;
        if (isScopeCalculate)
        {
            if (playerGunComponent.inScope)
            {
                gunCmp.currentAddedSpread -= playerGunComponent.addedSpread / playerGunComponent.scopeMultiplicity * 0.25f;
                gunCmp.currentMaxSpread -= playerGunComponent.maxSpread / playerGunComponent.scopeMultiplicity * 0.25f;
                gunCmp.currentMinSpread -= playerGunComponent.minSpread / playerGunComponent.scopeMultiplicity * 0.25f;
            }
            else
            {
                gunCmp.currentAddedSpread += playerGunComponent.addedSpread / playerGunComponent.scopeMultiplicity * 0.25f;
                gunCmp.currentMaxSpread += playerGunComponent.maxSpread / playerGunComponent.scopeMultiplicity * 0.25f;
                gunCmp.currentMinSpread += playerGunComponent.minSpread / playerGunComponent.scopeMultiplicity * 0.25f;
            }
        }

        Debug.Log("cur sp " + gunCmp.currentMinSpread);
    }
    private void ChangeScopeMultiplicity()
    {
        int playerEntity = _sceneData.Value.playerEntity;

        ref var plyerGunCmp = ref _playerGunComponentsPool.Value.Get(playerEntity);
        ref var gunCmp = ref _gunComponentsPool.Value.Get(playerEntity);
        ref var cameraCmp = ref _cameraComponentsPool.Value.Get(playerEntity);
        ref var moveCmp = ref _movementComponentsPool.Value.Get(playerEntity);
        ref var playerCmp = ref _playerComponentsPool.Value.Get(playerEntity);
        //Vector2[] oldPointsArray = playerCmp.visionZoneCollider.GetPath(1);
         var spriteMaskVision = playerCmp.view.visionZoneSprite;
        var spriteMaskVisionZone = playerCmp.view.visionZoneSpriteMask;
        if (!plyerGunCmp.inScope)
        {
            cameraCmp.cursorPositonPart = 1;
            if (plyerGunCmp.scopeMultiplicity <= 3)
                cameraCmp.playerPositonPart = 6 / plyerGunCmp.scopeMultiplicity;
            else
                cameraCmp.playerPositonPart = 2;
            if(plyerGunCmp.scopeMultiplicity == 2)
            {
                spriteMaskVision.sprite = _sceneData.Value.scopeSpriteMasks[1];
                spriteMaskVisionZone.sprite = _sceneData.Value.scopeSpriteMasks[1];
            }
            else if (plyerGunCmp.scopeMultiplicity == 8)
            {
                spriteMaskVisionZone.sprite = _sceneData.Value.scopeSpriteMasks[2];
                spriteMaskVision.sprite = _sceneData.Value.scopeSpriteMasks[2];//сделал так, потому что прицелов мало, иначе через словарь можно
            }
            _sceneData.Value.mainCamera.orthographicSize = plyerGunCmp.scopeMultiplicity * 5;
            moveCmp.moveSpeed /= plyerGunCmp.scopeMultiplicity;//придумать ураввнение скорости получше
           // Vector2[] pointsArray = new Vector2[] { oldPointsArray[0], oldPointsArray[1], new Vector2(oldPointsArray[2].x, -0.4f - plyerGunCmp.scopeMultiplicity), new Vector2(oldPointsArray[3].x, -0.4f - plyerGunCmp.scopeMultiplicity) };
           // playerCmp.visionZoneCollider.SetPath(1, pointsArray);
        }
        else
        {
            cameraCmp.cursorPositonPart = 1;
            cameraCmp.playerPositonPart = 6;
            _sceneData.Value.mainCamera.orthographicSize = 5;
            moveCmp.moveSpeed *= plyerGunCmp.scopeMultiplicity;
            spriteMaskVisionZone.sprite = _sceneData.Value.scopeSpriteMasks[0];
            spriteMaskVision.sprite = _sceneData.Value.scopeSpriteMasks[0];
            //Vector2[] pointsArray = new Vector2[] { oldPointsArray[0], oldPointsArray[1], new Vector2(oldPointsArray[2].x, -0.4f), new Vector2(oldPointsArray[3].x, -0.4f) };
            // playerCmp.visionZoneCollider.SetPath(1, pointsArray);
        }
        CalculateRecoil(ref gunCmp, plyerGunCmp, true);
        plyerGunCmp.inScope = !plyerGunCmp.inScope;

    }
    private void ChangeWeapon(int curWeapon)
    {
        ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        if (weaponsInInventoryCmp.curWeapon != curWeapon)
        {
            if ((curWeapon == 2 && weaponsInInventoryCmp.meleeWeaponObject == null) || (curWeapon == 1 && weaponsInInventoryCmp.gunSecondObject == null) || (curWeapon == 0 && weaponsInInventoryCmp.gunFirstObject == null))
                return;

            ref var attackCmp = ref _attackComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (curWeapon <= 1)
            {
                ref var gunCmp = ref _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                ref var playerGunCmp = ref _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity);

                if (weaponsInInventoryCmp.curWeapon == 0)
                {
                    _nowUsedWeaponTagsPool.Value.Del(_sceneData.Value.firstGunCellView._entity);
                    weaponsInInventoryCmp.curFirstWeaponAmmo = gunCmp.currentMagazineCapacity;
                    weaponsInInventoryCmp.curFirstWeaponDurability = playerGunCmp.durabilityPoints;
                }

                else /*if (weaponsInInventoryCmp.curWeapon == 1)*/
                {
                    _nowUsedWeaponTagsPool.Value.Del(_sceneData.Value.secondGunCellView._entity);
                    weaponsInInventoryCmp.curSecondWeaponAmmo = gunCmp.currentMagazineCapacity;
                    weaponsInInventoryCmp.curSecondWeaponDurability = playerGunCmp.durabilityPoints;
                }

                if (curWeapon == 0)
                {
                    weaponsInInventoryCmp.curWeapon = curWeapon;
                    _nowUsedWeaponTagsPool.Value.Add(_sceneData.Value.firstGunCellView._entity);
                    ChangeWeaponStats(weaponsInInventoryCmp.gunFirstObject, weaponsInInventoryCmp.curFirstWeaponAmmo, weaponsInInventoryCmp.curFirstWeaponDurability, ref gunCmp, ref playerGunCmp, ref attackCmp);

                    //менять модельку оружия
                }

                else
                {
                    weaponsInInventoryCmp.curWeapon = curWeapon;
                    _nowUsedWeaponTagsPool.Value.Add(_sceneData.Value.secondGunCellView._entity);
                    ChangeWeaponStats(weaponsInInventoryCmp.gunSecondObject, weaponsInInventoryCmp.curSecondWeaponAmmo, weaponsInInventoryCmp.curSecondWeaponDurability, ref gunCmp, ref playerGunCmp, ref attackCmp);
                }

            }

            if (curWeapon == 2)
            {
                //смена на ближнее оружие
            }

            _sceneData.Value.ammoInfoText.text = "смена оружия";
            attackCmp.weaponIsChanged = true;

        }
    }

    private void ChangeWeaponStats(GunInfo gunInfo, int currentAmmo, int gunDurability, ref GunComponent gunCmp, ref PlayerGunComponent playerGunCmp, ref CurrentAttackComponent curAttackCmp)
    {
        curAttackCmp.changeWeaponTime = gunInfo.weaponChangeSpeed;
        gunCmp.reloadDuration = gunInfo.reloadDuration;
        gunCmp.attackLeght = gunInfo.attackLenght;
        playerGunCmp.scopeMultiplicity = gunInfo.scopeMultiplicity;
        gunCmp.currentMagazineCapacity = currentAmmo;
        playerGunCmp.durabilityPoints = gunDurability;
        playerGunCmp.gunInfo = gunInfo;
        gunCmp.magazineCapacity = gunInfo.magazineCapacity;
        gunCmp.spreadRecoverySpeed = gunInfo.spreadRecoverySpeed;
        gunCmp.currentAddedSpread = gunInfo.addedSpread;
        playerGunCmp.isAuto = gunInfo.isAuto;
        gunCmp.bulletInShotCount = gunInfo.bulletCount;
        playerGunCmp.bulletTypeId = gunInfo.bulletTypeId;
        gunCmp.isOneBulletReload = gunInfo.isOneBulletReloaded;

        gunCmp.currentMaxSpread = gunInfo.maxSpread;
        gunCmp.currentMinSpread = gunInfo.minSpread;
        gunCmp.currentSpread = gunInfo.minSpread;

        playerGunCmp.maxSpread = gunInfo.maxSpread;
        playerGunCmp.minSpread = gunInfo.minSpread;
        playerGunCmp.addedSpread = gunInfo.addedSpread;

        curAttackCmp.damage = gunInfo.damage;
    }
    private void Shoot(int currentEntity, LayerMask targetLayer)// 6 маска игрока 7 враг
    {
        Debug.Log("Shot");
        ref var attackCmp = ref _attackComponentsPool.Value.Get(currentEntity);
        ref var gunCmp = ref _gunComponentsPool.Value.Get(currentEntity);
        gunCmp.firePoint.rotation = gunCmp.weaponContainer.rotation * Quaternion.Euler(0, 0, Random.Range(-gunCmp.currentSpread, gunCmp.currentSpread));

        var targets = Physics2D.RaycastAll(gunCmp.firePoint.position, gunCmp.firePoint.up, gunCmp.attackLeght, targetLayer/**/);


        if (targets.Length == 0)
        {
            Debug.Log("promax");
            var tracer = CreateTracer();
            tracer.SetPosition(0, gunCmp.firePoint.position);

            var ray = new Ray2D(gunCmp.firePoint.position, gunCmp.firePoint.up);

            tracer.SetPosition(1, ray.origin + (ray.direction * 20));
            return;
        }

        else
        {
            int damageReminder = attackCmp.damage;


            foreach (var target in targets)
            {
                var hpEntity = target.collider.gameObject.GetComponent<HealthView>()._entity;
                ref var health = ref _healthComponentsPool.Value.Get(hpEntity);
                int startedHealth = health.healthPoint;
                //health.healthPoint -= damageReminder;//сделать ивент и запихнуть в систему здоровья
                if (!health.healthView.isDeath)
                    _changeHealthEventsPool.Value.Add(_world.Value.NewEntity()).SetParametrs(damageReminder, hpEntity);
                Debug.Log(damageReminder + "dmg");
                if (health.healthPoint <= 0)
                {
                    Debug.Log(health.healthPoint + "hp and death");
                    //_world.Value.DelEntity(target.collider.gameObject.GetComponent<HealthView>()._entity);
                    damageReminder -= startedHealth;
                    continue;
                }

                else
                {
                    Debug.Log(health.healthPoint + "hp");
                    var tracer = CreateTracer();
                    tracer.SetPosition(0, gunCmp.firePoint.position);
                    tracer.SetPosition(1, target.point);
                    return;
                }
            }


            var tracer2 = CreateTracer();
            var ray2 = new Ray2D(gunCmp.firePoint.position, gunCmp.firePoint.up);

            tracer2.SetPosition(0, gunCmp.firePoint.position);
            tracer2.SetPosition(1, ray2.origin + (ray2.direction * 20));
        }

    }

    private LineRenderer CreateTracer()
    {
        var tracerEntity = _world.Value.NewEntity();

        ref var lifetimeCmp = ref _bulletTracerLifetimeComponentsPool.Value.Add(tracerEntity);
        lifetimeCmp.lifetime = 2f;
        lifetimeCmp.lineRenderer = _sceneData.Value.GetBulletTracer();

        return lifetimeCmp.lineRenderer;
    }

    private void CheckBulletTracerLife()
    {
        foreach (var bulletTracerEntity in _bulletTracerLifetimeComponentFilter.Value)
        {
            ref var lifetimeCmp = ref _bulletTracerLifetimeComponentsPool.Value.Get(bulletTracerEntity);
            Color tracerColor = new Color(255, 255, 255, lifetimeCmp.lifetime / 2);
            lifetimeCmp.lineRenderer.startColor = tracerColor;
            lifetimeCmp.lineRenderer.endColor = tracerColor;
            lifetimeCmp.lifetime -= Time.deltaTime;
            if (lifetimeCmp.lifetime > 0)
                continue;
            _sceneData.Value.ReleaseBulletTracer(lifetimeCmp.lineRenderer);

            _world.Value.DelEntity(bulletTracerEntity);
        }
    }

}
