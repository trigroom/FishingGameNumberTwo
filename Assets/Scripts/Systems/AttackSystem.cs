using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class AttackSystem : IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<AttackComponent> _attackComponentsPool;
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
    private EcsPoolInject<HealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;
    private EcsPoolInject<NowUsedWeaponTag> _nowUsedWeaponTagsPool;
    private EcsPoolInject<MeleeWeaponComponent> _meleeWeaponComponentsPool;
    private EcsPoolInject<PlayerMeleeWeaponComponent> _playerMeleeWeaponComponentsPool;
    private EcsPoolInject<OffInScopeStateEvent> _offInScopeStateEventsPool;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentsPool;
    private EcsPoolInject<PlayerMoveComponent> _playerMoveComponentsPool;
    private EcsPoolInject<MeleeWeaponContactEvent> _meleeWeaponContactEventsPool;
    private EcsPoolInject<CreatureInventoryComponent> _creatureInventoryComponentsPool;

    private EcsFilterInject<Inc<CreatureChangeWeaponEvent>> _creatureChangeWeaponEventsFilter;
    private EcsFilterInject<Inc<OffInScopeStateEvent>> _offInScopeStateEventsFilter;
    private EcsFilterInject<Inc<EndReloadEvent>> _endReloadEventFilter;
    private EcsFilterInject<Inc<PlayerComponent>> _playerComponentFilter;
    private EcsFilterInject<Inc<CreatureAIComponent>> _gunCreatureAiComponentsFilter;
    private EcsFilterInject<Inc<BulletTracerLifetimeComponent>> _bulletTracerLifetimeComponentFilter;
    private EcsFilterInject<Inc<ChangeWeaponFromInventoryEvent>> _changeWeaponFromInventoryEventsFilter;
    private EcsFilterInject<Inc<MeleeWeaponContactEvent>> _meleeWeaponContactEventsFilter;
    public void Run(IEcsSystems systems)
    {
        foreach (var aiCreature in _gunCreatureAiComponentsFilter.Value)
        {
            ref var creatureAi = ref _creatureAIComponentsPool.Value.Get(aiCreature);
            if (creatureAi.isPeaceful || creatureAi.colliders == null && !creatureAi.isPeaceful) continue;

            ref var attackCmp = ref _attackComponentsPool.Value.Get(aiCreature);
            var moveCmp = _movementComponentsPool.Value.Get(aiCreature);

            attackCmp.currentAttackCouldown += Time.deltaTime;

            if (moveCmp.isStunned) continue;

            if (creatureAi.creatureView.creatureMeleeView == null || (_creatureInventoryComponentsPool.Value.Has(aiCreature) && !_creatureInventoryComponentsPool.Value.Get(aiCreature).isSecondWeaponUsed))
            {
                Debug.Log((_creatureInventoryComponentsPool.Value.Get(aiCreature).isSecondWeaponUsed) + "sec used weapon");
                ref var gunCmp = ref _gunComponentsPool.Value.Get(aiCreature);

                if (creatureAi.currentState == CreatureAIComponent.CreatureStates.shootingToTarget || (creatureAi.isAttackWhenRetreat && creatureAi.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget))
                {
                    if (!gunCmp.isReloading && attackCmp.currentAttackCouldown >= attackCmp.attackCouldown && gunCmp.currentMagazineCapacity > 0 && attackCmp.canAttack)
                    {
                        attackCmp.currentAttackCouldown = 0;
                        for (int i = 0; i < gunCmp.bulletInShotCount; i++)
                            Shoot(aiCreature, LayerMask.GetMask("Player"));

                        if (gunCmp.currentSpread < gunCmp.currentMaxSpread)
                            gunCmp.currentSpread += gunCmp.currentAddedSpread;

                        attackCmp.currentAttackCouldown = 0;
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
            }
            //стрельба и презраядка врагов, и ещё стрельба если будет вклечена стребы при отступлении врага
            else
            {
                ref var meleeCmp = ref _meleeWeaponComponentsPool.Value.Get(aiCreature);

                Debug.Log((creatureAi.currentState == CreatureAIComponent.CreatureStates.shootingToTarget) + "||" + (creatureAi.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget) + "&&" + (_creatureInventoryComponentsPool.Value.Has(aiCreature) + "&&" + (!_creatureInventoryComponentsPool.Value.Get(aiCreature).isSecondWeaponUsed)));
                if (creatureAi.currentState == CreatureAIComponent.CreatureStates.shootingToTarget || (creatureAi.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget && (_creatureInventoryComponentsPool.Value.Has(aiCreature) && _creatureInventoryComponentsPool.Value.Get(aiCreature).isSecondWeaponUsed)))
                {
                    if (attackCmp.currentAttackCouldown >= attackCmp.attackCouldown && attackCmp.canAttack && !meleeCmp.isHitting)
                    {
                        ref var meleeView = ref creatureAi.creatureView.creatureMeleeView;
                        meleeView.meleeWeaponCollider.enabled = true;
                        var weaponContainerTransform = creatureAi.creatureView.movementView.weaponContainer;
                        if (meleeView.isWideHit)
                        {
                            var ray = new Ray2D(weaponContainerTransform.localPosition, weaponContainerTransform.up);
                            meleeCmp.endHitPoint = ray.origin + (ray.direction * meleeView.attackLenght);
                        }
                        else
                        {
                            meleeCmp.startRotation = creatureAi.creatureView.movementView.weaponContainer.transform.eulerAngles.z;
                        }
                        attackCmp.currentAttackCouldown = 0;
                        meleeCmp.startHitPoint = weaponContainerTransform.localPosition;
                        meleeCmp.isHitting = true;
                        meleeCmp.moveInAttackSide = true;

                    }
                }

                if (meleeCmp.isHitting)
                {
                    ref var meleeView = ref creatureAi.creatureView.creatureMeleeView;
                    var weaponContainerTransform = creatureAi.creatureView.movementView.weaponContainer;
                    if (meleeView.isWideHit)
                    {
                        if (meleeCmp.moveInAttackSide)
                        {
                            weaponContainerTransform.localPosition = Vector2.MoveTowards(weaponContainerTransform.localPosition, meleeCmp.endHitPoint, meleeView.attackSpeed * Time.deltaTime);
                            if (weaponContainerTransform.localPosition == (Vector3)meleeCmp.endHitPoint)
                            {
                                meleeCmp.moveInAttackSide = false;
                            }
                        }
                        else
                        {
                            weaponContainerTransform.localPosition = Vector2.MoveTowards(weaponContainerTransform.localPosition, meleeCmp.startHitPoint, meleeView.attackSpeed * 2 * Time.deltaTime);
                            if (weaponContainerTransform.localPosition == (Vector3)meleeCmp.startHitPoint)
                            {
                                weaponContainerTransform.localPosition = (Vector3)meleeCmp.startHitPoint;
                                meleeCmp.isHitting = false;
                                meleeView.meleeWeaponCollider.enabled = false;
                            }
                        }
                        //доделать
                    }
                    else
                    {
                        if (meleeCmp.moveInAttackSide)
                        {
                            float neededAngle = meleeCmp.startRotation + meleeView.attackLenght;
                            if (neededAngle > 360)
                                neededAngle -= 360;
                            weaponContainerTransform.transform.rotation = Quaternion.Lerp(weaponContainerTransform.transform.rotation, Quaternion.Euler(0, 0, neededAngle + 1), meleeView.attackSpeed * Time.deltaTime);//вращение на поред угол                                                                                                                                                                                                   //weaponContainerTransform.transform.Rotate(0, 0, -playerMeleeAttackCmp.weaponInfo.attackSpeed * Time.deltaTime);

                            //  Debug.Log(neededAngle + "cur angle" + weaponContainerTransform.transform.eulerAngles.z);
                            if ((int)weaponContainerTransform.transform.eulerAngles.z == (int)neededAngle)
                            {
                                meleeCmp.moveInAttackSide = false;
                            }
                        }
                        else
                        {
                            float neededAngle = meleeCmp.startRotation - meleeView.attackLenght;
                            if (neededAngle < 0)
                                neededAngle += 360;
                            weaponContainerTransform.transform.rotation = Quaternion.Lerp(weaponContainerTransform.transform.rotation, Quaternion.Euler(0, 0, neededAngle - 1), meleeView.attackSpeed * Time.deltaTime);
                            // weaponContainerTransform.transform.Rotate(0, 0, playerMeleeAttackCmp.weaponInfo.attackSpeed * Time.deltaTime);
                            // Debug.Log((weaponContainerTransform.transform.eulerAngles.z == neededAngle) +"num " + weaponContainerTransform.transform.eulerAngles.z+" != "+  neededAngle + " stat rot" + meleeAttackCmp.startRotation +" - " + -playerMeleeAttackCmp.weaponInfo.attackLenght);
                            if ((int)weaponContainerTransform.transform.eulerAngles.z == (int)neededAngle)
                            {
                                meleeCmp.isHitting = false;
                                meleeView.meleeWeaponCollider.enabled = false;
                                // weaponContainerTransform.transform.rotation = Quaternion.Euler(0, 0, 0);
                            }
                        }
                    }
                }
            }
        }

        foreach (var creatureChangeWeapon in _creatureChangeWeaponEventsFilter.Value)
        {
            ChangeCreatureWeapon(creatureChangeWeapon);
        }

        foreach (var changeWeaponFromInvEvt in _changeWeaponFromInventoryEventsFilter.Value)
        {
            ref var changeWeaponFromInvCmp = ref _changeWeaponFromInventoryEventsPool.Value.Get(changeWeaponFromInvEvt);
            ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (changeWeaponFromInvCmp.weaponCellNumberToChange < 2)
            {
                ref var invItemCmp = ref _inventoryItemComponentsPool.Value.Get(changeWeaponFromInvEvt);
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
                        weaponsInInventoryCmp.gunFirstObject = invItemCmp.itemInfo.gunInfo;
                        weaponsInInventoryCmp.curFirstWeaponAmmo = gunInInvCmp.currentAmmo;
                        weaponsInInventoryCmp.curFirstWeaponDurability = gunInInvCmp.gunDurability;
                       // Debug.Log(gunInInvCmp.gunInfo);
                    }
                    else
                    {
                        weaponsInInventoryCmp.gunSecondObject = invItemCmp.itemInfo.gunInfo;
                        weaponsInInventoryCmp.curSecondWeaponAmmo = gunInInvCmp.currentAmmo;
                        weaponsInInventoryCmp.curSecondWeaponDurability = gunInInvCmp.gunDurability;
                    }//фигня, переделать

                    ChangeWeapon(changeWeaponFromInvCmp.weaponCellNumberToChange);
                }

            }

            else
            {
                ref var plyerMeleeCmp = ref _playerMeleeWeaponComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                ref var meleeCmp = ref _meleeWeaponComponentsPool.Value.Get(_sceneData.Value.playerEntity);

                weaponsInInventoryCmp.meleeWeaponObject = _inventoryItemComponentsPool.Value.Get(changeWeaponFromInvEvt).itemInfo.meleeWeaponInfo;
                plyerMeleeCmp.weaponInfo = weaponsInInventoryCmp.meleeWeaponObject;

                ChangeWeapon(2);
                //если милишка
            }

            _changeWeaponFromInventoryEventsPool.Value.Del(changeWeaponFromInvEvt);
        }


        foreach (var playerEntity in _playerComponentFilter.Value)
        {
            var inventoryGunsCmp = _playerWeaponsInInventoryComponentsPool.Value.Get(playerEntity);

            ref var attackCmp = ref _attackComponentsPool.Value.Get(playerEntity);
            ref var curHealCmp = ref _currentHealingItemComponentsPool.Value.Get(playerEntity);

            ref var playerGunCmp = ref _playerGunComponentsPool.Value.Get(playerEntity);

            if (playerGunCmp.inScope)
                ChangeScopeMultiplicity();

            foreach (var offInScopeEvent in _offInScopeStateEventsFilter.Value)
            {
                _offInScopeStateEventsPool.Value.Del(offInScopeEvent);
            }

            attackCmp.currentAttackCouldown += Time.deltaTime;//
            ref var gunCmp = ref _gunComponentsPool.Value.Get(playerEntity);
            if (inventoryGunsCmp.curWeapon <= 1)//если стрелковое оружие
            {
                foreach (var reloadEvt in _endReloadEventFilter.Value)
                {
                    gunCmp.isReloading = true;
                    _sceneData.Value.ammoInfoText.text = "перезарядка...";
                    _endReloadEventsPool.Value.Del(reloadEvt);
                }
                if (!playerGunCmp.inScope)
                {
                    if (_sceneData.Value.mainCamera.orthographicSize > 5)//если без прицела
                        _sceneData.Value.mainCamera.orthographicSize -= Time.deltaTime * playerGunCmp.gunInfo.recoveryCameraSpread;
                    else if (_sceneData.Value.mainCamera.orthographicSize < 5)
                        _sceneData.Value.mainCamera.orthographicSize = 5;
                }
                else
                {
                    if (_sceneData.Value.mainCamera.orthographicSize > 5 * playerGunCmp.scopeMultiplicity)//если без прицела
                        _sceneData.Value.mainCamera.orthographicSize -= Time.deltaTime * playerGunCmp.gunInfo.recoveryCameraSpread * playerGunCmp.scopeMultiplicity;
                    else if (_sceneData.Value.mainCamera.orthographicSize < 5 * playerGunCmp.scopeMultiplicity)
                        _sceneData.Value.mainCamera.orthographicSize = 5 * playerGunCmp.scopeMultiplicity;
                }
                // стрельба
                if (((playerGunCmp.isAuto && Input.GetMouseButton(0)) || (!playerGunCmp.isAuto && Input.GetMouseButtonDown(0))) && !gunCmp.isReloading && gunCmp.currentMagazineCapacity > 0 && attackCmp.currentAttackCouldown >= attackCmp.attackCouldown && !attackCmp.weaponIsChanged && attackCmp.canAttack && !curHealCmp.isHealing && playerGunCmp.durabilityPoints != 0)
                {
                    int random = Random.Range(0, 101);
                    if ((playerGunCmp.misfirePercent > 0 && random > playerGunCmp.misfirePercent) || playerGunCmp.misfirePercent == 0)
                    {
                        for (int i = 0; i < gunCmp.bulletInShotCount; i++)
                            Shoot(playerEntity, LayerMask.GetMask("Enemy"));

                        if (gunCmp.currentSpread < gunCmp.currentMaxSpread)
                            gunCmp.currentSpread += gunCmp.currentAddedSpread;


                        if (!playerGunCmp.inScope && _sceneData.Value.mainCamera.orthographicSize < playerGunCmp.gunInfo.maxCameraSpread)
                            _sceneData.Value.mainCamera.orthographicSize += playerGunCmp.gunInfo.addedCameraSpread * (1 - ((_sceneData.Value.mainCamera.orthographicSize - 5f) / (playerGunCmp.gunInfo.maxCameraSpread - 5f)));
                        else if (playerGunCmp.inScope && _sceneData.Value.mainCamera.orthographicSize < playerGunCmp.gunInfo.maxCameraSpread * playerGunCmp.scopeMultiplicity)
                            _sceneData.Value.mainCamera.orthographicSize += playerGunCmp.gunInfo.addedCameraSpread * playerGunCmp.scopeMultiplicity * (1 - ((_sceneData.Value.mainCamera.orthographicSize - 5f) / (playerGunCmp.gunInfo.maxCameraSpread * playerGunCmp.scopeMultiplicity - 5f)));

                        Debug.Log("add cam spread" + (playerGunCmp.gunInfo.addedCameraSpread * playerGunCmp.scopeMultiplicity * (1 - ((_sceneData.Value.mainCamera.orthographicSize * playerGunCmp.scopeMultiplicity - 5f) / (playerGunCmp.gunInfo.maxCameraSpread * playerGunCmp.scopeMultiplicity - 5f)))));

                    }
                    Debug.Log("misshot prc" + playerGunCmp.misfirePercent);
                    attackCmp.currentAttackCouldown = 0;
                    gunCmp.currentMagazineCapacity--;
                    playerGunCmp.durabilityPoints--;

                    //изменять визуал от повреждения оружия
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



                if (gunCmp.currentSpread > gunCmp.currentMinSpread)
                    gunCmp.currentSpread -= gunCmp.spreadRecoverySpeed * Time.deltaTime;

                else if (gunCmp.currentSpread < gunCmp.currentMinSpread)
                    gunCmp.currentSpread = gunCmp.currentMinSpread;

            }

            else //милишная атака
            {
                ref var meleeAttackCmp = ref _meleeWeaponComponentsPool.Value.Get(playerEntity);
                ref var playerMeleeAttackCmp = ref _playerMeleeWeaponComponentsPool.Value.Get(playerEntity);
                ref var playerMoveCmp = ref _playerMoveComponentsPool.Value.Get(playerEntity);

                if (((playerMeleeAttackCmp.weaponInfo.isAuto && Input.GetMouseButton(0)) || (!playerMeleeAttackCmp.weaponInfo.isAuto && Input.GetMouseButtonDown(0))) && attackCmp.currentAttackCouldown >= attackCmp.attackCouldown && !attackCmp.weaponIsChanged && attackCmp.canAttack && !curHealCmp.isHealing && !meleeAttackCmp.isHitting && playerMeleeAttackCmp.weaponInfo.staminaUsage <= playerMoveCmp.currentRunTime)
                {
                    _playerComponentsPool.Value.Get(playerEntity).view.meleeWeaponCollider.enabled = true;
                    playerMoveCmp.currentRunTime -= playerMeleeAttackCmp.weaponInfo.staminaUsage;
                    var weaponContainerTransform = _playerComponentsPool.Value.Get(playerEntity).view.movementView.weaponContainer.transform;
                    if (playerMeleeAttackCmp.weaponInfo.isWideHit)
                    {
                        var ray = new Ray2D(weaponContainerTransform.localPosition, weaponContainerTransform.up);
                        meleeAttackCmp.endHitPoint = ray.origin + (ray.direction * playerMeleeAttackCmp.weaponInfo.attackLenght);
                        //Debug.Log(meleeAttackCmp.endHitPoint + "end hit pos");
                    }
                    else
                    {
                        meleeAttackCmp.startRotation = _playerComponentsPool.Value.Get(playerEntity).view.movementView.weaponContainer.transform.eulerAngles.z;
                    }
                    attackCmp.currentAttackCouldown = 0;
                    //возможно менять поворот оружия чтобы оно цепляло больше врагов
                    meleeAttackCmp.isHitting = true;
                    meleeAttackCmp.moveInAttackSide = true;
                }

                if (meleeAttackCmp.isHitting)
                {
                    var weaponContainerTransform = _playerComponentsPool.Value.Get(playerEntity).view.movementView.weaponContainer.transform;
                    if (playerMeleeAttackCmp.weaponInfo.isWideHit)
                    {
                        if (meleeAttackCmp.moveInAttackSide)
                        {
                            weaponContainerTransform.localPosition = Vector2.MoveTowards(weaponContainerTransform.localPosition, meleeAttackCmp.endHitPoint, playerMeleeAttackCmp.weaponInfo.attackSpeed * Time.deltaTime);
                            if (weaponContainerTransform.localPosition == (Vector3)meleeAttackCmp.endHitPoint)
                            {
                                meleeAttackCmp.moveInAttackSide = false;
                            }
                        }
                        else
                        {
                            weaponContainerTransform.localPosition = Vector2.MoveTowards(weaponContainerTransform.localPosition, meleeAttackCmp.startHitPoint, playerMeleeAttackCmp.weaponInfo.attackSpeed * 2 * Time.deltaTime);
                            if (weaponContainerTransform.localPosition == (Vector3)meleeAttackCmp.startHitPoint)
                            {
                                weaponContainerTransform.localPosition = (Vector3)meleeAttackCmp.startHitPoint;
                                meleeAttackCmp.isHitting = false;
                                _playerComponentsPool.Value.Get(playerEntity).view.meleeWeaponCollider.enabled = false;
                            }
                        }
                        //доделать
                    }
                    else
                    {
                        if (meleeAttackCmp.moveInAttackSide)
                        {
                            float neededAngle = meleeAttackCmp.startRotation + playerMeleeAttackCmp.weaponInfo.attackLenght;
                            if (neededAngle > 360)
                                neededAngle -= 360;
                            weaponContainerTransform.transform.rotation = Quaternion.Lerp(weaponContainerTransform.transform.rotation, Quaternion.Euler(0, 0, neededAngle + 1), playerMeleeAttackCmp.weaponInfo.attackSpeed * Time.deltaTime);//вращение на поред угол                                                                                                                                                                                                   //weaponContainerTransform.transform.Rotate(0, 0, -playerMeleeAttackCmp.weaponInfo.attackSpeed * Time.deltaTime);

                            //  Debug.Log(neededAngle + "cur angle" + weaponContainerTransform.transform.eulerAngles.z);
                            if ((int)weaponContainerTransform.transform.eulerAngles.z == (int)neededAngle)
                            {
                                meleeAttackCmp.moveInAttackSide = false;
                            }
                        }
                        else
                        {
                            float neededAngle = meleeAttackCmp.startRotation - playerMeleeAttackCmp.weaponInfo.attackLenght;
                            if (neededAngle < 0)
                                neededAngle += 360;
                            weaponContainerTransform.transform.rotation = Quaternion.Lerp(weaponContainerTransform.transform.rotation, Quaternion.Euler(0, 0, neededAngle - 1), playerMeleeAttackCmp.weaponInfo.attackSpeed * Time.deltaTime);
                            // weaponContainerTransform.transform.Rotate(0, 0, playerMeleeAttackCmp.weaponInfo.attackSpeed * Time.deltaTime);
                            // Debug.Log((weaponContainerTransform.transform.eulerAngles.z == neededAngle) +"num " + weaponContainerTransform.transform.eulerAngles.z+" != "+  neededAngle + " stat rot" + meleeAttackCmp.startRotation +" - " + -playerMeleeAttackCmp.weaponInfo.attackLenght);
                            if ((int)weaponContainerTransform.transform.eulerAngles.z == (int)neededAngle)
                            {
                                meleeAttackCmp.isHitting = false;
                                _playerComponentsPool.Value.Get(playerEntity).view.meleeWeaponCollider.enabled = false;
                                // weaponContainerTransform.transform.rotation = Quaternion.Euler(0, 0, 0);
                            }
                        }
                    }
                }
            }
            if (attackCmp.weaponIsChanged)
            {
                attackCmp.currentChangeWeaponTime += Time.deltaTime;

                if (attackCmp.currentChangeWeaponTime >= attackCmp.changeWeaponTime)
                {
                    attackCmp.currentChangeWeaponTime = 0;
                    attackCmp.weaponIsChanged = false;
                    if (inventoryGunsCmp.curWeapon != 2)
                        _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
                    else
                        _sceneData.Value.ammoInfoText.text = "";
                }
            }

            else if (!gunCmp.isReloading && !playerGunCmp.inScope && !curHealCmp.isHealing)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) && inventoryGunsCmp.curWeapon != 0 && inventoryGunsCmp.gunFirstObject != null)
                    ChangeWeapon(0);
                else if (Input.GetKeyDown(KeyCode.Alpha2) && inventoryGunsCmp.curWeapon != 1 && inventoryGunsCmp.gunSecondObject != null)
                    ChangeWeapon(1);
                else if (Input.GetKeyDown(KeyCode.Alpha3) && inventoryGunsCmp.curWeapon != 2)
                    ChangeWeapon(2);
            }
        }

        foreach (var meleeWeaponContact in _meleeWeaponContactEventsFilter.Value)
        {
            int attackedEntity = _meleeWeaponContactEventsPool.Value.Get(meleeWeaponContact).attackedEntity;
            var attackCmp = _attackComponentsPool.Value.Get(meleeWeaponContact);
            if (meleeWeaponContact == _sceneData.Value.playerEntity)
            {
                var meleeCmp = _playerMeleeWeaponComponentsPool.Value.Get(meleeWeaponContact);

                Debug.Log("dmg melee" + attackCmp.damage);
                _changeHealthEventsPool.Value.Add(_world.Value.NewEntity()).SetParametrs(attackCmp.damage, attackedEntity);

                ref var moveCmp = ref _movementComponentsPool.Value.Get(attackedEntity);

                moveCmp.isStunned = true;
                moveCmp.stunTime = meleeCmp.weaponInfo.stunTime;
                if (!moveCmp.canMove)
                    moveCmp.canMove = true;
                //Debug.Log(moveCmp.stunTime + "st time");
                moveCmp.moveSpeed = meleeCmp.weaponInfo.knockbackSpeed;
                moveCmp.moveInput = (moveCmp.entityTransform.position - _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform.position).normalized;
            }
            else
            {
                var meleeCmp = _creatureAIComponentsPool.Value.Get(meleeWeaponContact).creatureView.creatureMeleeView;

                _changeHealthEventsPool.Value.Add(_world.Value.NewEntity()).SetParametrs(attackCmp.damage, attackedEntity);

                ref var moveCmp = ref _movementComponentsPool.Value.Get(attackedEntity);

                moveCmp.isStunned = true;
                moveCmp.stunTime = meleeCmp.stunTime;
                //Debug.Log(moveCmp.stunTime + "st time");
                moveCmp.moveSpeed = meleeCmp.knockbackSpeed;
                moveCmp.moveInput = (moveCmp.entityTransform.position - _movementComponentsPool.Value.Get(meleeWeaponContact).entityTransform.position).normalized;
                Debug.Log("dmg melee" + attackCmp.damage + "mi" + moveCmp.moveInput);
            }
        }
        CheckBulletTracerLife();
        //для других сущностей отдельно
    }

    private void ChangeCreatureWeapon(int creatureEntity)
    {
        ref var attackCmp = ref _attackComponentsPool.Value.Get(creatureEntity);
        ref var creatureAiCmp = ref _creatureAIComponentsPool.Value.Get(creatureEntity);
        if (_meleeWeaponComponentsPool.Value.Get(creatureEntity).isHitting || _gunComponentsPool.Value.Get(creatureEntity).isReloading) return;

        _creatureInventoryComponentsPool.Value.Get(creatureEntity).isSecondWeaponUsed = !_creatureInventoryComponentsPool.Value.Get(creatureEntity).isSecondWeaponUsed;


        if (_creatureInventoryComponentsPool.Value.Get(creatureEntity).isSecondWeaponUsed)//милишка
        {
            var meleeWeaponView = creatureAiCmp.creatureView.creatureMeleeView;

            creatureAiCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = meleeWeaponView.itemVisualInfo.itemSprite;

            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localScale = Vector3.one * meleeWeaponView.itemVisualInfo.itemScale;
            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, meleeWeaponView.itemVisualInfo.itemRotateZ);
            //сделать поворот и скейл

            attackCmp.attackCouldown = meleeWeaponView.attackCouldown;
            attackCmp.damage = meleeWeaponView.damage;
        }
        else
        {
            var gunView = creatureAiCmp.creatureView.creatureGunView;


            creatureAiCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = gunView.itemVisualInfo.itemSprite;

            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localScale = Vector3.one * gunView.itemVisualInfo.itemScale;
            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, gunView.itemVisualInfo.itemRotateZ);

            attackCmp.attackCouldown = gunView.attackCouldown;
            attackCmp.damage = gunView.damage;
        }
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

        // Debug.Log("cur sp " + gunCmp.currentMinSpread);
    }
    private void ChangeScopeMultiplicity()
    {
        int playerEntity = _sceneData.Value.playerEntity;

        if (_playerWeaponsInInventoryComponentsPool.Value.Get(playerEntity).curWeapon == 2)
            return;

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
            if (plyerGunCmp.scopeMultiplicity == 2)
            {
                spriteMaskVision.sprite = _sceneData.Value.scopeSpriteMasks[1];
                spriteMaskVisionZone.sprite = _sceneData.Value.scopeSpriteMasks[1];
            }
            else if (plyerGunCmp.scopeMultiplicity == 8)
            {
                spriteMaskVisionZone.sprite = _sceneData.Value.scopeSpriteMasks[2];
                spriteMaskVision.sprite = _sceneData.Value.scopeSpriteMasks[2];//сделал так, потому что прицелов мало, иначе через словарь можно
            }
            _sceneData.Value.mainCamera.orthographicSize *= plyerGunCmp.scopeMultiplicity;
            moveCmp.moveSpeed /= plyerGunCmp.scopeMultiplicity;//придумать ураввнение скорости получше
                                                               // Vector2[] pointsArray = new Vector2[] { oldPointsArray[0], oldPointsArray[1], new Vector2(oldPointsArray[2].x, -0.4f - plyerGunCmp.scopeMultiplicity), new Vector2(oldPointsArray[3].x, -0.4f - plyerGunCmp.scopeMultiplicity) };
                                                               // playerCmp.visionZoneCollider.SetPath(1, pointsArray);
        }
        else
        {
            cameraCmp.cursorPositonPart = 1;
            cameraCmp.playerPositonPart = 6;
            _sceneData.Value.mainCamera.orthographicSize /= plyerGunCmp.scopeMultiplicity;
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
        //if (weaponsInInventoryCmp.curWeapon != curWeapon || weaponsInInventoryCmp.curWeapon == 2 && curWeapon == 2)//про 2 переделать
        //{
        Debug.Log("смена на " + curWeapon);
        // if ((curWeapon == 2 && weaponsInInventoryCmp.meleeWeaponObject == null) || (curWeapon == 1 && weaponsInInventoryCmp.gunSecondObject == null) || (curWeapon == 0 && weaponsInInventoryCmp.gunFirstObject == null))
        //    return;

        if (weaponsInInventoryCmp.curWeapon == 0)
        {
            _nowUsedWeaponTagsPool.Value.Del(_sceneData.Value.firstGunCellView._entity);
            weaponsInInventoryCmp.curFirstWeaponAmmo = _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity).currentMagazineCapacity;
            weaponsInInventoryCmp.curFirstWeaponDurability = _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity).durabilityPoints;
        }

        else if (weaponsInInventoryCmp.curWeapon == 1)
        {
            _nowUsedWeaponTagsPool.Value.Del(_sceneData.Value.secondGunCellView._entity);
            weaponsInInventoryCmp.curSecondWeaponAmmo = _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity).currentMagazineCapacity;
            weaponsInInventoryCmp.curSecondWeaponDurability = _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity).durabilityPoints;
        }
        else
        {
            _nowUsedWeaponTagsPool.Value.Del(_sceneData.Value.meleeWeaponCellView._entity);
        }

        ref var attackCmp = ref _attackComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        weaponsInInventoryCmp.curWeapon = curWeapon;
        if (curWeapon <= 1)
        {
            ref var gunCmp = ref _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            ref var playerGunCmp = ref _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity);


            if (curWeapon == 0)
            {
                _nowUsedWeaponTagsPool.Value.Add(_sceneData.Value.firstGunCellView._entity);
                ChangeGunStats(weaponsInInventoryCmp.gunFirstObject, weaponsInInventoryCmp.curFirstWeaponAmmo, weaponsInInventoryCmp.curFirstWeaponDurability, ref gunCmp, ref playerGunCmp, ref attackCmp);

                //менять модельку оружия
            }

            else
            {
                _nowUsedWeaponTagsPool.Value.Add(_sceneData.Value.secondGunCellView._entity);
                ChangeGunStats(weaponsInInventoryCmp.gunSecondObject, weaponsInInventoryCmp.curSecondWeaponAmmo, weaponsInInventoryCmp.curSecondWeaponDurability, ref gunCmp, ref playerGunCmp, ref attackCmp);
            }

        }

        else if (curWeapon == 2)
        {
            //смена на ближнее оружие
            _nowUsedWeaponTagsPool.Value.Add(_sceneData.Value.meleeWeaponCellView._entity);
            ChangeMeleeWeaponStats(weaponsInInventoryCmp.meleeWeaponObject, ref _playerMeleeWeaponComponentsPool.Value.Get(_sceneData.Value.playerEntity), ref attackCmp);
        }

        _sceneData.Value.ammoInfoText.text = "смена оружия";
        Debug.Log("remaining change time" + attackCmp.currentChangeWeaponTime + " > " + attackCmp.changeWeaponTime);
        attackCmp.weaponIsChanged = true;

        Debug.Log(attackCmp.weaponIsChanged);

        //}
    }

    private void ChangeMeleeWeaponStats(MeleeWeaponInfo meleeWeaponInfo, ref PlayerMeleeWeaponComponent playerMeleeCmp, ref AttackComponent curAttackCmp)
    {
        // менять статы милишки
        //менять размер коллайдера 
        curAttackCmp.damage = meleeWeaponInfo.damage;
        curAttackCmp.attackCouldown = meleeWeaponInfo.attackCouldown;
        curAttackCmp.changeWeaponTime = meleeWeaponInfo.weaponChangeSpeed;
        playerMeleeCmp.weaponInfo = meleeWeaponInfo;
        var playerView = _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view;
        playerView.weaponSpriteRenderer.sprite = meleeWeaponInfo.weaponSprite;
        playerView.weaponTransform.localScale = Vector3.one * meleeWeaponInfo.spriteScaleMultiplayer;
        playerView.weaponTransform.localEulerAngles = new Vector3(0, 0, meleeWeaponInfo.spriteRotation);
        //meleeCmp.startHitPoint = _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).movementView.weaponContainer.transform.position;//возможно менять не придётся
    }
    private void ChangeGunStats(GunInfo gunInfo, int currentAmmo, int gunDurability, ref GunComponent gunCmp, ref PlayerGunComponent playerGunCmp, ref AttackComponent curAttackCmp)
    {
        //перемещать точку выстрела
        var playerView = _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view;

        Debug.Log(gunInfo.weaponSprite);

        playerView.weaponSpriteRenderer.sprite = gunInfo.weaponSprite;
        playerView.weaponTransform.localScale = Vector3.one * gunInfo.spriteScaleMultiplayer;
        playerView.weaponTransform.localEulerAngles = new Vector3(0, 0, gunInfo.spriteRotation);

        curAttackCmp.changeWeaponTime = gunInfo.weaponChangeSpeed;
        gunCmp.reloadDuration = gunInfo.reloadDuration;
        curAttackCmp.attackCouldown = gunInfo.attackCouldown;
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

        if (playerGunCmp.durabilityPoints < playerGunCmp.gunInfo.maxDurabilityPoints * 0.6f)
        {
            playerGunCmp.durabilityGunMultiplayer = (float)System.Math.Round(1 - ((float)playerGunCmp.durabilityPoints / ((float)playerGunCmp.gunInfo.maxDurabilityPoints * 1.6f) + 0.4f), 2);
            playerGunCmp.misfirePercent = Mathf.FloorToInt((playerGunCmp.durabilityGunMultiplayer) * 60);
            Debug.Log(playerGunCmp.misfirePercent + "dur chnge ");

            CalculateRecoil(ref gunCmp, playerGunCmp, false);
        }
        else
        {
            playerGunCmp.durabilityGunMultiplayer = 0;
            playerGunCmp.misfirePercent = 0;
            CalculateRecoil(ref gunCmp, playerGunCmp, false);
        }

        curAttackCmp.damage = gunInfo.damage;
    }
    private void Shoot(int currentEntity, LayerMask targetLayer)// 6 маска игрока 7 враг
    {
        Debug.Log("Shot");
        ref var attackCmp = ref _attackComponentsPool.Value.Get(currentEntity);
        ref var gunCmp = ref _gunComponentsPool.Value.Get(currentEntity);
        gunCmp.firePoint.rotation = gunCmp.weaponContainer.rotation * Quaternion.Euler(0, 0, Random.Range(-gunCmp.currentSpread, gunCmp.currentSpread));

        var targets = Physics2D.RaycastAll(gunCmp.firePoint.position, gunCmp.firePoint.up, gunCmp.attackLeght, targetLayer);


        if (targets.Length == 0)
        {
            Debug.Log("promax");
            var tracer = CreateTracer();
            tracer.SetPosition(0, gunCmp.firePoint.position);

            var ray = new Ray2D(gunCmp.firePoint.position, gunCmp.firePoint.up);

            tracer.SetPosition(1, ray.origin + (ray.direction * gunCmp.attackLeght));
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
            tracer2.SetPosition(1, ray2.origin + (ray2.direction * gunCmp.attackLeght));
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
