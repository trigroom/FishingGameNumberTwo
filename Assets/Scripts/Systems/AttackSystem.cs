using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackSystem : IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<AttackComponent> _attackComponentsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<BulletShellComponent> _bulletShellComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<BulletTracerLifetimeComponent> _bulletTracerLifetimeComponentsPool;
    private EcsPoolInject<ParicleLifetimeComponent> _paricleLifetimeComponentsPool;
    private EcsPoolInject<EndReloadEvent> _endReloadEventsPool;
    private EcsPoolInject<ReloadEvent> _reloadEventsPool;
    private EcsPoolInject<ChangeHealthEvent> _changeHealthEventsPool;
    private EcsPoolInject<ChangeWeaponFromInventoryEvent> _changeWeaponFromInventoryEventsPool;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<CameraComponent> _cameraComponentsPool;
    private EcsPoolInject<HealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;
    private EcsPoolInject<ChangeFirstBossPhaseEvent> _changeFirstBossPhaseEventsPool;
    private EcsPoolInject<NowUsedWeaponTag> _nowUsedWeaponTagsPool { get; set; }
    private EcsPoolInject<MeleeWeaponComponent> _meleeWeaponComponentsPool;
    private EcsPoolInject<PlayerMeleeWeaponComponent> _playerMeleeWeaponComponentsPool;
    private EcsPoolInject<OffInScopeStateEvent> _offInScopeStateEventsPool;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentsPool;
    private EcsPoolInject<PlayerMoveComponent> _playerMoveComponentsPool;
    private EcsPoolInject<MeleeWeaponContactEvent> _meleeWeaponContactEventsPool;
    private EcsPoolInject<CreatureInventoryComponent> _creatureInventoryComponentsPool;
    private EcsPoolInject<FieldOfViewComponent> _fieldOfViewComponentsPool;
    private EcsPoolInject<InventoryCellComponent> _inventoryCellComponentsPool;
    private EcsPoolInject<HidedObjectOutsideFOVComponent> _hidedObjectOutsideFOVComponentsPool;
    private EcsPoolInject<ExplodeGrenadeEvent> _explodeGrenadeEventsPool;
    private EcsPoolInject<GrenadeComponent> _grenadeComponentsPool;
    private EcsPoolInject<OneShotSoundComponent> _oneShotSoundComponentsPool;
    private EcsPoolInject<GrenadeExplodeComponent> _grenadeExplodeComponentsPool;
    private EcsPoolInject<LaserPointerForGunComponent> _laserPointerForGunComponentsPool;
    private EcsPoolInject<PlayerUpgradedStats> _playerUpgradedStatsPool;
    private EcsPoolInject<InventoryComponent> _inventoryComponentsPool;
    private EcsPoolInject<BuildingCheckerComponent> _buildingCheckerComponentsPool;
    private EcsPoolInject<MineExplodeEvent> _mineExplodeEventsPool;
    private EcsPoolInject<SecondDurabilityComponent> _shieldComponentsPool;
    private EcsPoolInject<DurabilityInInventoryComponent> _durabilityInInventoryComponentsPool;
    private EcsPoolInject<MenuStatesComponent> _menuStatesComponentsPool;
    private EcsPoolInject<EffectComponent> _effectComponentsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<UpgradePlayerStatEvent> _upgradePlayerStatEventsPool;

    private EcsFilterInject<Inc<CreatureChangeWeaponEvent>> _creatureChangeWeaponEventsFilter;
    private EcsFilterInject<Inc<OffInScopeStateEvent>> _offInScopeStateEventsFilter;
    private EcsFilterInject<Inc<EndReloadEvent>> _endReloadEventFilter;
    private EcsFilterInject<Inc<BulletShellComponent>> _bulletShellComponentsFilter;
    private EcsFilterInject<Inc<PlayerComponent>> _playerComponentFilter;
    private EcsFilterInject<Inc<CreatureAIComponent>> _gunCreatureAiComponentsFilter;
    private EcsFilterInject<Inc<BulletTracerLifetimeComponent>> _bulletTracerLifetimeComponentFilter;
    private EcsFilterInject<Inc<ParicleLifetimeComponent>, Exc<EffectParticleLifetimeTag>> _paricleLifetimeComponentsFilter;
    private EcsFilterInject<Inc<ChangeWeaponFromInventoryEvent>> _changeWeaponFromInventoryEventsFilter;
    private EcsFilterInject<Inc<MeleeWeaponContactEvent>> _meleeWeaponContactEventsFilter;
    private EcsFilterInject<Inc<GunComponent>> _gunComponentsFilter;
    private EcsFilterInject<Inc<ExplodeGrenadeEvent>> _explodeGrenadeEventsFilter;
    private EcsFilterInject<Inc<OneShotSoundComponent>> _oneShotSoundComponentsFilter;
    private EcsFilterInject<Inc<GrenadeExplodeComponent>> _grenadeExplodeComponentsFilter;
    private EcsFilterInject<Inc<CalculateRecoilEvent>> _calculateRecoilEventsFilter;
    private EcsFilterInject<Inc<MineExplodeEvent>> _mineExplodeEventsFilter;
    private EcsFilterInject<Inc<ChangeFirstBossPhaseEvent>> _changeFirstBossPhaseEventsFilter;
    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
            n += 360;
        return n;
    }
    public void Run(IEcsSystems systems)
    {
        int playerEntity = _sceneData.Value.playerEntity;
        ref var playerGunCmp = ref _playerGunComponentsPool.Value.Get(playerEntity);
        foreach (var bulletShell in _bulletShellComponentsFilter.Value)
        {
            ref var bulletShellCmp = ref _bulletShellComponentsPool.Value.Get(bulletShell);
            if (!bulletShellCmp.isLeft)
                bulletShellCmp.bulletShellPrefab.transform.position = new Vector2(bulletShellCmp.bulletShellPrefab.transform.position.x + 6f * Time.deltaTime, bulletShellCmp.bulletShellPrefab.transform.position.y + bulletShellCmp.currentBulletShellSpeed * Time.deltaTime);
            else
                bulletShellCmp.bulletShellPrefab.transform.position = new Vector2(bulletShellCmp.bulletShellPrefab.transform.position.x - 6f * Time.deltaTime, bulletShellCmp.bulletShellPrefab.transform.position.y + bulletShellCmp.currentBulletShellSpeed * Time.deltaTime);
            bulletShellCmp.currentBulletShellSpeed -= Time.deltaTime * bulletShellCmp.horizontalSpeed;
            if (bulletShellCmp.currentBulletShellSpeed < -8f)
            {
                _sceneData.Value.ReleaseBulletShell(bulletShellCmp.bulletShellPrefab);
                _bulletShellComponentsPool.Value.Del(bulletShell);
            }
        }
        foreach (var mineExplodeEntity in _mineExplodeEventsFilter.Value)
        {
            var mineExplodeEvt = _mineExplodeEventsPool.Value.Get(mineExplodeEntity);
            var grenadeInfo = mineExplodeEvt.grenadeInfo;

            var grenade = _sceneData.Value.GetGrenadeObject();

            grenade.gameObject.transform.position = mineExplodeEvt.explodeSpawnPosition;

            grenade.grenadeSprite.sprite = grenadeInfo.grenadeSprite;

            ref var grenadeCmp = ref _grenadeComponentsPool.Value.Add(_world.Value.NewEntity());
            grenadeCmp.grenadeView = grenade;
            grenadeCmp.grenadeView.grenadeCollider.enabled = false;
            grenadeCmp.grenadeInfo = grenadeInfo;
            grenadeCmp.currentTimeToExplode = 0.5f;

            _mineExplodeEventsPool.Value.Del(mineExplodeEntity);
        }

        foreach (var grenadeExplodeEntity in _grenadeExplodeComponentsFilter.Value)
        {
            ref var grenadeExplodeCmp = ref _grenadeExplodeComponentsPool.Value.Get(grenadeExplodeEntity);
            float tdt = Time.deltaTime;
            grenadeExplodeCmp.currentTimeAfterExplode -= tdt;
            if (grenadeExplodeCmp.grenadeView.flashLight.intensity > 0)
                grenadeExplodeCmp.grenadeView.flashLight.intensity -= tdt * 10;

            if (grenadeExplodeCmp.currentTimeAfterExplode <= 0)
            {
                grenadeExplodeCmp.grenadeView.flashLight.gameObject.SetActive(false);
                _sceneData.Value.ReleaseGrenadeObject(grenadeExplodeCmp.grenadeView);
                _grenadeExplodeComponentsPool.Value.Del(grenadeExplodeEntity);
            }
        }
        foreach (var sound in _oneShotSoundComponentsFilter.Value)
        {
            ref var oneShotSoundCmp = ref _oneShotSoundComponentsPool.Value.Get(sound);

            oneShotSoundCmp.time -= Time.deltaTime;

            if (oneShotSoundCmp.time <= 0)
            {
                _sceneData.Value.ReleaseSoundObject(oneShotSoundCmp.audioSource);
                _oneShotSoundComponentsPool.Value.Del(sound);
            }
        }
        foreach (var explodedGrenadeEntity in _explodeGrenadeEventsFilter.Value)
        {
            ref var grenadeCmp = ref _grenadeComponentsPool.Value.Get(explodedGrenadeEntity);
            int rayCount = 16;
            float addedAngle = 360 / rayCount;
            float curAngle = 0;
            Vector2 origin = grenadeCmp.grenadeView.rigidbodyGrenade.position;

            List<Collider2D> damagedColliders = new List<Collider2D>();
            for (int i = 0; i < rayCount; i++)
            {
                RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(curAngle), grenadeCmp.grenadeInfo.explodeRadius, LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("Player") | LayerMask.GetMask("EnemyHead") | LayerMask.GetMask("Shield"));
                Debug.DrawRay(origin, GetVectorFromAngle(curAngle) * grenadeCmp.grenadeInfo.explodeRadius, Color.red);

                if (raycastHit2D.collider != null && !damagedColliders.Contains(raycastHit2D.collider) && raycastHit2D.collider.gameObject.layer != 10)
                {
                    damagedColliders.Add(raycastHit2D.collider);
                    var particles = CreateParticles(0);
                    particles.gameObject.transform.position = raycastHit2D.point;
                }
                else
                {
                    RaycastHit2D raycastHitCheckExplode2D = Physics2D.Raycast(origin, GetVectorFromAngle(curAngle), grenadeCmp.grenadeInfo.explodeRadius * 4f, LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Enemy"));
                    if (raycastHitCheckExplode2D.collider != null && raycastHitCheckExplode2D.collider.gameObject.layer != 10)
                    {
                        ref var aiCmp = ref _creatureAIComponentsPool.Value.Get(raycastHitCheckExplode2D.collider.gameObject.GetComponent<HealthView>()._entity);
                        if (aiCmp.currentState == CreatureAIComponent.CreatureStates.idle)
                        {
                            aiCmp.colliders = new List<Transform>() { _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform.transform };
                            
                            aiCmp.targetPositionCached = origin;
                            aiCmp.reachedLastTarget = false;
                            aiCmp.timeFromLastTargetSeen = 0f;
                            aiCmp.currentState = CreatureAIComponent.CreatureStates.follow;
                        }
                    }
                }

                curAngle += addedAngle;
            }

            //какойто визуальный эффект взрыва
            var audioSource = _sceneData.Value.PlaySoundFXClip(grenadeCmp.grenadeInfo.explodeSound, origin, 1);
            int explodeGrenadeEntity = _world.Value.NewEntity();
            ref var oneshotSoundCmp = ref _oneShotSoundComponentsPool.Value.Add(explodeGrenadeEntity);
            ref var grenadeExplodeCmp = ref _grenadeExplodeComponentsPool.Value.Add(explodeGrenadeEntity);

            oneshotSoundCmp.audioSource = audioSource;
            oneshotSoundCmp.time = grenadeCmp.grenadeInfo.explodeSound.length;

            grenadeExplodeCmp.currentTimeAfterExplode = 1f;
            grenadeExplodeCmp.grenadeView = grenadeCmp.grenadeView;
            grenadeCmp.grenadeView.SetVisualParametrs(grenadeCmp.grenadeInfo.explodeRadius);
            //звук взрыва
            foreach (var damgableCollider in damagedColliders)
            {
                int damagedEntity = 0;
                int damage = Mathf.CeilToInt(grenadeCmp.grenadeInfo.damage * ((grenadeCmp.grenadeInfo.explodeRadius - Vector2.Distance(origin, damgableCollider.transform.position) + 0.3f) / grenadeCmp.grenadeInfo.explodeRadius));
                if (damgableCollider.gameObject.layer == 13)//если щит
                {
                    var hpEntity = damgableCollider.gameObject.GetComponent<ShieldView>()._entity;
                    if (hpEntity == _sceneData.Value.shieldCellView._entity)
                    {
                        int shieldEntity = _sceneData.Value.shieldCellView._entity;
                        ref var shieldCmp = ref _shieldComponentsPool.Value.Get(shieldEntity);
                        ref var shieldItemCmp = ref _inventoryItemComponentsPool.Value.Get(shieldEntity);
                        int curDamageToShield = Mathf.CeilToInt(damage * shieldItemCmp.itemInfo.sheildInfo.damagePercent);
                        shieldCmp.currentDurability -= curDamageToShield;
                        if (shieldCmp.currentDurability < 0)
                            shieldCmp.currentDurability = 0;

                        var particles = CreateParticles(1);//сделать партиклы искр от металла
                        particles.gameObject.transform.position = damgableCollider.gameObject.transform.position;

                        if (shieldCmp.currentDurability <= 0)
                            _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view.movementView.shieldView.shieldCollider.enabled = false;

                        continue;
                    }
                }
                else
                {
                    bool isHeadShot = false;
                    if (damgableCollider.tag == "Head")
                    {
                        damagedEntity = damgableCollider.gameObject.GetComponent<HeadColliderView>()._entity;
                        isHeadShot = true;
                    }
                    else
                        damagedEntity = damgableCollider.gameObject.GetComponent<HealthView>()._entity;




                    if ((float)damage > Random.Range(0, 100))
                    {
                        float effectTime = damage / 3;
                        if (_creatureAIComponentsPool.Value.Has(damagedEntity) && _creatureAIComponentsPool.Value.Get(damagedEntity).armorInfo != null)
                            effectTime *= 1 - _creatureAIComponentsPool.Value.Get(damagedEntity).armorInfo.bleedingResistance;
                        else if (_playerComponentsPool.Value.Has(damagedEntity) && _inventoryItemComponentsPool.Value.Has(_sceneData.Value.bodyArmorCellView._entity) && _inventoryItemComponentsPool.Value.Get(_sceneData.Value.bodyArmorCellView._entity).itemInfo.bodyArmorInfo.bleedingResistance != 0)
                            effectTime *= 1 - _inventoryItemComponentsPool.Value.Get(_sceneData.Value.bodyArmorCellView._entity).itemInfo.bodyArmorInfo.bleedingResistance;
                        ref var effCmp = ref _effectComponentsPool.Value.Add(_world.Value.NewEntity());
                        effCmp.effectEntity = damagedEntity;

                        int bleedingLevel = 1;
                        if (damage > 20)
                            bleedingLevel = 2;
                        else if (damage > 50)
                            bleedingLevel = 3;

                        effCmp.effectLevel = bleedingLevel;
                        effCmp.effectType = EffectInfo.EffectType.bleeding;
                        effCmp.isFirstEffectCheck = true;
                        effCmp.effectIconSprite = _sceneData.Value.bloodEffectsSprites[bleedingLevel - 1];
                        effCmp.effectDuration = effectTime;

                    }

                    _changeHealthEventsPool.Value.Add(_world.Value.NewEntity()).SetParametrs(damage, damagedEntity, isHeadShot, 0.6f);

                }

            }
            grenadeCmp.grenadeView.grenadeSprite.sprite = _sceneData.Value.transparentSprite;
            _explodeGrenadeEventsPool.Value.Del(explodedGrenadeEntity);
            _grenadeComponentsPool.Value.Del(explodedGrenadeEntity);
        }
        foreach (var gunCmpEntity in _gunComponentsFilter.Value)
        {
            ref var gunCmp = ref _gunComponentsPool.Value.Get(gunCmpEntity);

            if (gunCmp.timeFromLastShot > 0)
            {
                gunCmp.lightFromGunShot.intensity = gunCmp.timeFromLastShot * gunCmp.flashShotInstance;
                gunCmp.timeFromLastShot -= Time.deltaTime;
            }
            else if (gunCmp.timeFromLastShot < 0)
            {
                gunCmp.timeFromLastShot = 0;
                gunCmp.lightFromGunShot.intensity = 0;
            }

            bool playerInScope = gunCmpEntity == playerEntity && playerGunCmp.inScope;
            float needMinSpriteRecoil = playerInScope ? 0.5f : 0.8f;
            if (!_attackComponentsPool.Value.Get(gunCmpEntity).weaponIsChanged)
            {
                if (gunCmp.gunSpritePositionRecoil < needMinSpriteRecoil)
                    gunCmp.gunSpritePositionRecoil += gunCmp.spreadRecoverySpeed * 0.003f;
                if (gunCmp.gunSpritePositionRecoil > needMinSpriteRecoil)
                {
                    if (playerGunCmp.changedInScopeState && playerInScope)
                        gunCmp.gunSpritePositionRecoil -= needMinSpriteRecoil * 0.005f;
                    else
                        gunCmp.gunSpritePositionRecoil = needMinSpriteRecoil;
                }
            }

            _movementComponentsPool.Value.Get(gunCmpEntity).movementView.weaponSprite.transform.localPosition = new Vector2(0, gunCmp.gunSpritePositionRecoil);

        }
        var playerPosition = _movementComponentsPool.Value.Get(playerEntity).entityTransform.position;
        ref var playerCmp = ref _playerComponentsPool.Value.Get(playerEntity);
        foreach (var aiCreature in _gunCreatureAiComponentsFilter.Value)
        {
            ref var creatureAi = ref _creatureAIComponentsPool.Value.Get(aiCreature);
            if (creatureAi.isPeaceful) continue;
            ref var creatureAiInventory = ref _creatureInventoryComponentsPool.Value.Get(aiCreature);

            ref var attackCmp = ref _attackComponentsPool.Value.Get(aiCreature);
            var moveCmp = _movementComponentsPool.Value.Get(aiCreature);

            if (creatureAi.currentState != CreatureAIComponent.CreatureStates.idle)
                attackCmp.currentAttackCouldown += Time.deltaTime;

            if (moveCmp.isStunned) continue;

            if (creatureAiInventory.meleeWeaponItem == null || creatureAiInventory.gunItem != null && !_creatureInventoryComponentsPool.Value.Get(aiCreature).isSecondWeaponUsed)
            {
                ref var gunCmp = ref _gunComponentsPool.Value.Get(aiCreature);
                var gunItem = creatureAiInventory.gunItem.gunInfo;
                if (creatureAi.currentState != CreatureAIComponent.CreatureStates.idle)
                {
                    if (!gunCmp.isReloading && attackCmp.currentAttackCouldown >= attackCmp.attackCouldown && gunCmp.currentMagazineCapacity > 0 && attackCmp.canAttack && creatureAi.sightOnTarget && (creatureAi.needSightOnTargetTime + (Vector2.Distance(creatureAi.currentTarget.position, creatureAi.creatureView.transform.position) * 0.1f)) < creatureAi.sightOnTargetTime)
                    {
                        float distanceToPlayer = Vector2.Distance(moveCmp.entityTransform.position, playerPosition);

                        attackCmp.currentAttackCouldown = 0;
                        if (!_buildingCheckerComponentsPool.Value.Get(_sceneData.Value.playerEntity).isHideRoof)
                            creatureAi.creatureView.movementView.weaponAudioSource.volume = _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).currentAudibility * gunItem.shotSoundDistance / distanceToPlayer * playerCmp.currentAudibility;
                        else
                            creatureAi.creatureView.movementView.weaponAudioSource.volume = _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).currentAudibility * gunItem.shotSoundDistance / distanceToPlayer * 1.35f * playerCmp.currentAudibility;//если стреляет в помещении то в 1,35 раза громче
                        creatureAi.creatureView.movementView.weaponAudioSource.clip = gunItem.shotSound;
                        creatureAi.creatureView.movementView.weaponAudioSource.panStereo = (moveCmp.entityTransform.position.x - playerPosition.x) / 6f;
                        creatureAi.creatureView.movementView.weaponAudioSource.Play();
                        for (int i = 0; i < gunCmp.currentBulletInfo.bulletCount; i++)
                            Shoot(aiCreature, LayerMask.GetMask("Player"));

                        if (gunCmp.currentSpread < gunCmp.currentMaxSpread)
                            gunCmp.currentSpread += gunCmp.currentAddedSpread * gunCmp.currentBulletInfo.addedSpreadMultiplayer;

                        attackCmp.currentAttackCouldown = 0;
                        gunCmp.currentMagazineCapacity--;

                        if (gunItem.bulletShellSpawnOnShot && _hidedObjectOutsideFOVComponentsPool.Value.Get(aiCreature).timeBeforeHide > 0)
                            BulletShellSpawn(gunCmp.currentBulletInfo.bulletCase, moveCmp.movementView.bulletShellSpawnPoint.position, moveCmp.movementView.weaponContainer.localEulerAngles.z, moveCmp.movementView.characterSpriteTransform.localRotation.y == 0);
                        else if (!gunItem.bulletShellSpawnOnShot)
                            creatureAi.bulletShellsToReload++;

                        if (gunCmp.currentMagazineCapacity == 0)
                            gunCmp.isReloading = true;
                    }
                }
                else if ((creatureAi.currentState == CreatureAIComponent.CreatureStates.follow && gunCmp.currentMagazineCapacity < gunCmp.magazineCapacity / 2) || (creatureAi.currentState == CreatureAIComponent.CreatureStates.idle && gunCmp.currentMagazineCapacity < gunCmp.magazineCapacity))//будет перезаряжать свои патроны в состоянии идла только если у его их меньше половины
                {
                    gunCmp.isReloading = true;
                    float distanceToPlayer = Vector2.Distance(moveCmp.entityTransform.position, playerPosition);
                    if (6f * playerCmp.currentAudibility >= distanceToPlayer)
                    {
                        if(creatureAiInventory.gunItem.gunInfo.startReloadSound != null)
                        creatureAi.creatureView.movementView.weaponAudioSource.clip = creatureAiInventory.gunItem.gunInfo.startReloadSound;
                        creatureAi.creatureView.movementView.weaponAudioSource.panStereo = (moveCmp.entityTransform.position.x - playerPosition.x) / 6f;
                        creatureAi.creatureView.movementView.weaponAudioSource.volume = 6f * playerCmp.currentAudibility / distanceToPlayer;
                        creatureAi.creatureView.movementView.weaponAudioSource.Play();
                    }
                }

                if (gunCmp.isReloading)
                {
                    gunCmp.currentReloadDuration += Time.deltaTime;
                    if (gunCmp.currentReloadDuration >= gunCmp.reloadDuration)
                    {
                        if (!gunItem.bulletShellSpawnOnShot && _hidedObjectOutsideFOVComponentsPool.Value.Get(aiCreature).timeBeforeHide > 0)
                        {
                            Vector2 spawnPosition = moveCmp.movementView.bulletShellSpawnPoint.position;
                            for (int i = 0; i < creatureAi.bulletShellsToReload; i++)
                            {
                                BulletShellSpawn(gunCmp.currentBulletInfo.bulletCase, spawnPosition, moveCmp.movementView.weaponContainer.localEulerAngles.z, moveCmp.movementView.characterSpriteTransform.localRotation.y == 0);
                                if (i % 2 == 0)
                                    spawnPosition = new Vector2(spawnPosition.x, spawnPosition.y + 0.1f);
                            }
                            creatureAi.bulletShellsToReload = 0;
                        }
                        gunCmp.currentReloadDuration = 0;
                        gunCmp.isReloading = false;
                        if (creatureAiInventory.gunItem.gunInfo.endReloadSound)
                        creatureAi.creatureView.movementView.weaponAudioSource.clip = creatureAiInventory.gunItem.gunInfo.endReloadSound;
                        if (gunCmp.isOneBulletReload)
                            gunCmp.currentMagazineCapacity++;
                        else
                            gunCmp.currentMagazineCapacity = gunCmp.magazineCapacity;
                    }
                }

                if (gunCmp.currentSpread > gunCmp.currentMinSpread)
                    gunCmp.currentSpread -= gunCmp.spreadRecoverySpeed * Time.deltaTime;

                else if (gunCmp.currentSpread < gunCmp.currentMinSpread)
                    gunCmp.currentSpread = gunCmp.currentMinSpread;
            }
            
            else
            {
                ref var meleeCmp = ref _meleeWeaponComponentsPool.Value.Get(aiCreature);

                if (creatureAi.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget && (_creatureInventoryComponentsPool.Value.Get(aiCreature).isSecondWeaponUsed || creatureAiInventory.gunItem == null))
                {
                    if (attackCmp.currentAttackCouldown >= attackCmp.attackCouldown && attackCmp.canAttack && !meleeCmp.isHitting && creatureAi.sightOnTarget)
                    {
                        ref var meleeItem = ref creatureAiInventory.meleeWeaponItem.meleeWeaponInfo;
                        creatureAi.creatureView.movementView.weaponCollider.enabled = true;
                        var weaponContainerTransform = creatureAi.creatureView.movementView.weaponContainer;
                        if (!meleeCmp.isWideAttack)
                        {
                            var ray = new Ray2D(weaponContainerTransform.localPosition, weaponContainerTransform.up);
                            meleeCmp.endHitPoint = ray.origin + (ray.direction * meleeItem.attackLenght);
                        }
                        else
                            meleeCmp.startRotation = creatureAi.creatureView.movementView.weaponContainer.transform.eulerAngles.z;

                       // creatureAi.creatureView.movementView.weaponAudioSource.clip = meleeItem.hitSound;
                        creatureAi.creatureView.movementView.weaponAudioSource.panStereo = (moveCmp.entityTransform.position.x - playerPosition.x) / 5f;
                        creatureAi.creatureView.movementView.weaponAudioSource.volume = 5f / Vector2.Distance(moveCmp.entityTransform.position, playerPosition);
                        creatureAi.creatureView.movementView.weaponAudioSource.Play();

                        attackCmp.currentAttackCouldown = 0;
                        meleeCmp.startHitPoint = weaponContainerTransform.localPosition;
                        meleeCmp.isHitting = true;
                        meleeCmp.moveInAttackSide = true;

                    }
                }

                if (meleeCmp.isHitting)
                {
                    ref var meleeItem = ref creatureAiInventory.meleeWeaponItem.meleeWeaponInfo;
                    var weaponContainerTransform = creatureAi.creatureView.movementView.weaponContainer;
                    if (!meleeCmp.isWideAttack)
                    {
                        if (meleeCmp.moveInAttackSide)
                        {
                            weaponContainerTransform.localPosition = Vector2.MoveTowards(weaponContainerTransform.localPosition, meleeCmp.endHitPoint, meleeItem.attackSpeed * Time.deltaTime);
                            if (weaponContainerTransform.localPosition == (Vector3)meleeCmp.endHitPoint)
                                meleeCmp.moveInAttackSide = false;
                        }
                        else
                        {
                            weaponContainerTransform.localPosition = Vector2.MoveTowards(weaponContainerTransform.localPosition, meleeCmp.startHitPoint, meleeItem.attackSpeed * 2 * Time.deltaTime);
                            if (weaponContainerTransform.localPosition == (Vector3)meleeCmp.startHitPoint)
                            {
                                weaponContainerTransform.localPosition = (Vector3)meleeCmp.startHitPoint;
                                meleeCmp.isHitting = false;
                                creatureAi.creatureView.movementView.weaponCollider.enabled = false;
                            }
                        }
                        //доделать
                    }
                    else
                    {
                        if (meleeCmp.moveInAttackSide)
                        {
                            float neededAngle = meleeCmp.startRotation + meleeItem.wideAttackLenght;
                            if (neededAngle > 360)
                                neededAngle -= 360;
                            weaponContainerTransform.transform.rotation = Quaternion.Lerp(weaponContainerTransform.transform.rotation, Quaternion.Euler(0, 0, neededAngle + 1), meleeItem.wideAttackSpeed * Time.deltaTime);//вращение на поред угол                                                                                                                                                                                                   //weaponContainerTransform.transform.Rotate(0, 0, -playerMeleeAttackCmp.weaponInfo.attackSpeed * Time.deltaTime);

                            if ((int)weaponContainerTransform.transform.eulerAngles.z == (int)neededAngle)
                                meleeCmp.moveInAttackSide = false;
                        }
                        else
                        {
                            float neededAngle = meleeCmp.startRotation - meleeItem.wideAttackLenght;
                            if (neededAngle < 0)
                                neededAngle += 360;
                            weaponContainerTransform.transform.rotation = Quaternion.Lerp(weaponContainerTransform.transform.rotation, Quaternion.Euler(0, 0, neededAngle - 1), meleeItem.wideAttackSpeed * Time.deltaTime);

                            if ((int)weaponContainerTransform.transform.eulerAngles.z == (int)neededAngle)
                            {
                                meleeCmp.isHitting = false;
                                creatureAi.creatureView.movementView.weaponCollider.enabled = false;
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
                ref var plyerGunCmp = ref _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                ref var gunCmp = ref _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                if (plyerGunCmp.inScope)
                {
                    ChangeScopeMultiplicity();
                }
                if (changeWeaponFromInvCmp.isDeleteWeapon)
                {
                    if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.firstGunCellView._entity))
                        ChangeWeapon(0);
                    else if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.secondGunCellView._entity))
                        ChangeWeapon(1);
                    else
                        ChangeWeapon(2);
                }
                else
                    ChangeWeapon(changeWeaponFromInvCmp.weaponCellNumberToChange);

            }

            else
            {
                ref var plyerMeleeCmp = ref _playerMeleeWeaponComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                ref var meleeCmp = ref _meleeWeaponComponentsPool.Value.Get(_sceneData.Value.playerEntity);

                if (changeWeaponFromInvCmp.isDeleteWeapon)
                {
                    if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.firstGunCellView._entity))
                        ChangeWeapon(0);
                    else if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.secondGunCellView._entity))
                        ChangeWeapon(1);
                    else
                        ChangeWeapon(2);
                }
                else
                    ChangeWeapon(2);
                //если милишка
            }

            _changeWeaponFromInventoryEventsPool.Value.Del(changeWeaponFromInvEvt);
        }

        foreach(var boss in _changeFirstBossPhaseEventsFilter.Value)
        {
            ref var creatureAI = ref _creatureInventoryComponentsPool.Value.Get(boss);
            ref var moveCmp = ref _movementComponentsPool.Value.Get(boss);
            ref var creatureCmp = ref _creatureAIComponentsPool.Value.Get(boss);

            moveCmp.moveSpeed = creatureAI.enemyClassSettingInfo.movementSpeed;
            creatureCmp.creatureView.healthView.characterMainCollaider.enabled = true;
            creatureCmp.creatureView.healthView.headColliderView.enabled = true;
            creatureCmp.timeFromLastTargetSeen = -1000f;

            _changeFirstBossPhaseEventsPool.Value.Del(boss);
        }
        foreach (var entity in _playerComponentFilter.Value)
        {
            var inventoryGunsCmp = _playerWeaponsInInventoryComponentsPool.Value.Get(playerEntity);

            ref var playerAttackCmp = ref _attackComponentsPool.Value.Get(playerEntity);

            ref var curHealCmp = ref _currentHealingItemComponentsPool.Value.Get(playerEntity);

            ref var cameraCmp = ref _cameraComponentsPool.Value.Get(playerEntity);

            if (playerAttackCmp.grenadeThrowCouldown > 0)
                playerAttackCmp.grenadeThrowCouldown -= Time.deltaTime;
            else if (playerAttackCmp.grenadeThrowCouldown < 0)
                playerAttackCmp.grenadeThrowCouldown = 0;

            if (!playerGunCmp.changedInScopeState)//отдача камеры 
            {
                if (!playerGunCmp.inScope)
                {
                    if (_sceneData.Value.mainCamera.orthographicSize < 5)//если без прицела
                        _sceneData.Value.mainCamera.orthographicSize += Time.deltaTime * cameraCmp.currentRecoveryCameraSpread;
                    else if (_sceneData.Value.mainCamera.orthographicSize > 5)
                        _sceneData.Value.mainCamera.orthographicSize = 5;
                    else if (_sceneData.Value.mainCamera.orthographicSize < 0.5)
                        _sceneData.Value.mainCamera.orthographicSize = 0.5f;
                }
                else
                {
                    if (_sceneData.Value.mainCamera.orthographicSize < 5 * playerGunCmp.currentScopeMultiplicity)//если без прицела
                        _sceneData.Value.mainCamera.orthographicSize += Time.deltaTime * cameraCmp.currentRecoveryCameraSpread * playerGunCmp.currentScopeMultiplicity;
                    else if (_sceneData.Value.mainCamera.orthographicSize > 5 * playerGunCmp.currentScopeMultiplicity)
                        _sceneData.Value.mainCamera.orthographicSize = 5 * playerGunCmp.currentScopeMultiplicity;
                   
                }
            }

            else
            {
                playerGunCmp.timeAfterChangedInScopeState -= Time.deltaTime;

                if (playerGunCmp.inScope)
                {
                    _sceneData.Value.mainCamera.orthographicSize += (playerGunCmp.currentScopeMultiplicity - 1) * 5 * Time.deltaTime * playerGunCmp.cameraOrtograficalSizeDifference;

                    if (playerGunCmp.currentScopeMultiplicity <= 3)
                        cameraCmp.playerPositonPart = 6 - (6 - 6 / playerGunCmp.currentScopeMultiplicity) * (1 - playerGunCmp.timeAfterChangedInScopeState);
                    else
                        cameraCmp.playerPositonPart = 6 - 4 * (1 - playerGunCmp.timeAfterChangedInScopeState);
                }
                else
                {
                    _sceneData.Value.mainCamera.orthographicSize -= (playerGunCmp.currentScopeMultiplicity - 1) * 5 * Time.deltaTime * playerGunCmp.cameraOrtograficalSizeDifference;
                    if (playerGunCmp.currentScopeMultiplicity <= 3)
                        cameraCmp.playerPositonPart = 6 / playerGunCmp.currentScopeMultiplicity + (6 - 6 / playerGunCmp.currentScopeMultiplicity) * (1 - playerGunCmp.timeAfterChangedInScopeState);
                    else
                        cameraCmp.playerPositonPart = 2 + 4 * (1 - playerGunCmp.timeAfterChangedInScopeState);
                }


                if (playerGunCmp.timeAfterChangedInScopeState <= 0)
                {
                    playerGunCmp.changedInScopeState = false;
                    if (!playerGunCmp.inScope)
                        cameraCmp.playerPositonPart = 6;
                    else
                    {
                        if (playerGunCmp.currentScopeMultiplicity <= 3)
                            cameraCmp.playerPositonPart = 6 / playerGunCmp.currentScopeMultiplicity;
                        else
                            cameraCmp.playerPositonPart = 2;
                    }
                }
            }

            foreach (var offInScopeEvent in _offInScopeStateEventsFilter.Value)
            {
                if (playerGunCmp.inScope)
                    ChangeScopeMultiplicity();
                _offInScopeStateEventsPool.Value.Del(offInScopeEvent);
            }
            ref var playerMoveCmp = ref _playerMoveComponentsPool.Value.Get(playerEntity);
            ref var moveCmp = ref _movementComponentsPool.Value.Get(playerEntity);
            playerAttackCmp.currentAttackCouldown += Time.deltaTime;//
            ref var gunCmp = ref _gunComponentsPool.Value.Get(playerEntity);

            foreach (var item in _calculateRecoilEventsFilter.Value)
                CalculateRecoil(ref gunCmp, playerGunCmp, false);

            if (inventoryGunsCmp.curWeapon <= 1)//если стрелковое оружие
            {
                ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(_playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity).curEquipedWeaponCellEntity);
                var playerView = playerCmp.view;

                playerView.leftRecoilTracker.localRotation = Quaternion.Euler(0, 0, gunCmp.currentSpread);
                playerView.rightRecoilTracker.localRotation = Quaternion.Euler(0, 0, -gunCmp.currentSpread);

                if (_laserPointerForGunComponentsPool.Value.Has(inventoryGunsCmp.curEquipedWeaponCellEntity) && !playerAttackCmp.weaponIsChanged)
                {
                    ref var laserPointerCmp = ref _laserPointerForGunComponentsPool.Value.Get(inventoryGunsCmp.curEquipedWeaponCellEntity);
                    ref var laserInfo = ref _sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[2]].gunPartInfo;
                    if (laserPointerCmp.laserIsOn)
                    {
                        if (Input.GetKeyDown(KeyCode.Y))
                        {
                            laserPointerCmp.laserIsOn = false;

                            playerView.laserPointerLineRenderer.gameObject.SetActive(laserPointerCmp.laserIsOn);
                        }

                        else if (laserPointerCmp.remainingLaserPointerTime > 0)
                        {
                            laserPointerCmp.remainingLaserPointerTime -= Time.deltaTime;
                            var laserPointerContainer = playerView.laserPointerTransform;
                            var ray = Physics2D.Raycast(laserPointerContainer.position, laserPointerContainer.right, laserInfo.laserMaxLenght, LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("InteractedCharacter"));
                            if (ray.collider != null)
                            {
                                playerView.laserPointerLineRenderer.SetPosition(1, ray.point);
                                if (ray.collider.gameObject.layer == 7)//enemy layer
                                {
                                    int enemyEntity = ray.transform.gameObject.GetComponent<HealthView>()._entity;
                                    ref var aiCmpEnemy = ref _creatureAIComponentsPool.Value.Get(enemyEntity);
                                    if ((aiCmpEnemy.creatureView.movementView.characterSpriteTransform.localRotation.y == 0 && playerView.movementView.transform.position.x > aiCmpEnemy.creatureView.movementView.characterSpriteTransform.position.x) || (aiCmpEnemy.creatureView.movementView.characterSpriteTransform.localRotation.y != 0 && playerView.movementView.transform.position.x < aiCmpEnemy.creatureView.movementView.characterSpriteTransform.position.x))
                                    {
                                        aiCmpEnemy.targetPositionCached = playerView.transform.position;
                                        aiCmpEnemy.reachedLastTarget = false;
                                        if (aiCmpEnemy.currentState == CreatureAIComponent.CreatureStates.idle)
                                        {
                                            aiCmpEnemy.timeFromLastTargetSeen = 0f;
                                            aiCmpEnemy.currentState = CreatureAIComponent.CreatureStates.follow;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var raySecond = new Ray2D(laserPointerContainer.position, laserPointerContainer.right);
                                playerView.laserPointerLineRenderer.SetPosition(1, raySecond.origin + (raySecond.direction * laserInfo.laserMaxLenght));
                            }
                            playerView.laserPointerLineRenderer.SetPosition(0, laserPointerContainer.position);
                        }
                        else if (laserPointerCmp.remainingLaserPointerTime < 0)
                        {
                            laserPointerCmp.remainingLaserPointerTime = 0;
                            laserPointerCmp.laserIsOn = false;
                            playerView.laserPointerLineRenderer.gameObject.SetActive(laserPointerCmp.laserIsOn);
                        }

                    }
                    else if (Input.GetKeyDown(KeyCode.Y) && laserPointerCmp.remainingLaserPointerTime > 0)
                    {
                        laserPointerCmp.laserIsOn = true;

                        var laserPointerContainer = playerView.laserPointerTransform;
                        var ray = Physics2D.Raycast(laserPointerContainer.position, laserPointerContainer.up, laserInfo.laserMaxLenght, LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("InteractedCharacter"));
                        if (ray.collider != null)
                            playerView.laserPointerLineRenderer.SetPosition(1, ray.point);
                        else
                        {
                            var raySecond = new Ray2D(laserPointerContainer.position, laserPointerContainer.up);
                            playerView.laserPointerLineRenderer.SetPosition(1, raySecond.origin + (raySecond.direction * laserInfo.laserMaxLenght));
                        }
                        playerView.laserPointerLineRenderer.SetPosition(0, laserPointerContainer.position);


                        playerView.laserPointerLineRenderer.gameObject.SetActive(laserPointerCmp.laserIsOn);
                    }
                }


                foreach (var reloadEvt in _endReloadEventFilter.Value)
                {
                    gunCmp.isReloading = true;
                    _sceneData.Value.ammoInfoText.text = "перезарядка...";
                    if(playerGunCmp.gunInfo.startReloadSound != null)
                    playerCmp.view.movementView.weaponAudioSource.clip = playerGunCmp.gunInfo.startReloadSound;
                    playerCmp.view.movementView.weaponAudioSource.Play();
                    _endReloadEventsPool.Value.Del(reloadEvt);
                }

                if (playerGunCmp.inScope)
                {
                    Vector3 mousePos = Vector2.zero;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(_sceneData.Value.bluredUICanvas, Input.mousePosition, Camera.main, out mousePos);

                    _sceneData.Value.dropedItemsUIView.scopeCrossCentreImage.rectTransform.transform.position = mousePos;

                    if (!playerGunCmp.changedInScopeState)
                    {
                        float staminaChanged = Time.deltaTime * ((2f + gunInInvCmp.currentGunWeight) * 0.1f);
                        moveCmp.currentRunTime -= staminaChanged;
                        _sceneData.Value.playerStaminaBarFilled.fillAmount = moveCmp.currentRunTime / playerMoveCmp.playerView.runTime;
                        _sceneData.Value.playerStaminaText.text = moveCmp.currentRunTime.ToString("0.0") + "/" + playerMoveCmp.playerView.runTime;
                        _sceneData.Value.staminaAnimator.SetFloat("StaminaAnimSpeed", 1 + (1 - _sceneData.Value.playerStaminaBarFilled.fillAmount) * 2);
                        if (moveCmp.currentRunTime <= 0)
                        {
                            ChangeScopeMultiplicity();
                            moveCmp.currentRunTime -= 1;
                        }

                        ref var upgradeStatsCmp = ref _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity);
                        upgradeStatsCmp.currentStatsExp[2] += staminaChanged;
                        if (upgradeStatsCmp.currentStatsExp[2] >= _sceneData.Value.levelExpCounts[upgradeStatsCmp.statLevels[2]] && !_upgradePlayerStatEventsPool.Value.Has(_sceneData.Value.playerEntity))
                            _upgradePlayerStatEventsPool.Value.Add(_sceneData.Value.playerEntity).statIndex = 2;
                    }
                }
                // стрельба
                ref var itemInfo = ref _inventoryItemComponentsPool.Value.Get(_playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity).curEquipedWeaponCellEntity);
                if (((playerGunCmp.isAuto && Input.GetMouseButton(0)) || (!playerGunCmp.isAuto && Input.GetMouseButtonDown(0))) && !gunCmp.isReloading && gunInInvCmp.currentAmmo.Count > 0 && playerAttackCmp.currentAttackCouldown >= playerAttackCmp.attackCouldown && !playerAttackCmp.weaponIsChanged
                    && playerAttackCmp.canAttack && !curHealCmp.isHealing && gunInInvCmp.gunDurability != 0 && (playerGunCmp.gunInfo.isOneHandedGun || (!playerGunCmp.gunInfo.isOneHandedGun && (playerView.movementView.shieldView.shieldObject.localPosition == Vector3.zero
                    || playerView.movementView.shieldView.shieldObject.localPosition != Vector3.zero && _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).weaponsExp[itemInfo.itemInfo.itemId].weaponExpLevel <= 8//change to 8
                     && moveCmp.currentRunTime >= playerGunCmp.gunInfo.attackCouldown * gunInInvCmp.currentGunWeight * 2f))))
                {
                    if (!playerGunCmp.gunInfo.isOneHandedGun && playerView.movementView.shieldView.shieldObject.localPosition != Vector3.zero)
                        moveCmp.currentRunTime -= playerGunCmp.gunInfo.attackCouldown * gunInInvCmp.currentGunWeight * 2f;
                    
                    Vector2 direction = (moveCmp.pointToRotateInput - (Vector2)moveCmp.entityTransform.position).normalized;
                    if (Physics2D.Raycast(playerCmp.view.playerTransform.position, direction, 1f, LayerMask.GetMask("Obstacle"))) continue;

                    if (playerGunCmp.gunInfo.bulletShellSpawnOnShot)
                        BulletShellSpawn(gunCmp.currentBulletInfo.bulletCase, moveCmp.movementView.bulletShellSpawnPoint.position, moveCmp.movementView.weaponContainer.localEulerAngles.z, moveCmp.movementView.characterSpriteTransform.localRotation.y == 0);
                    if (gunInInvCmp.gunDurability < playerGunCmp.gunInfo.maxDurabilityPoints * 0.6f)
                    {
                        playerGunCmp.durabilityGunMultiplayer = (float)System.Math.Round(1 - ((float)gunInInvCmp.gunDurability / ((float)playerGunCmp.gunInfo.maxDurabilityPoints * 1.6f) + 0.4f), 2);
                        playerGunCmp.misfirePercent = Mathf.FloorToInt((playerGunCmp.durabilityGunMultiplayer) * 60);

                        CalculateRecoil(ref gunCmp, playerGunCmp, false);
                    }
                    else if (playerGunCmp.misfirePercent != 0)
                    {
                        playerGunCmp.misfirePercent = 0;
                        playerGunCmp.durabilityGunMultiplayer = 0;
                        CalculateRecoil(ref gunCmp, playerGunCmp, false);
                    }

                    if ((playerGunCmp.misfirePercent > 0 && Random.Range(0, 101) > playerGunCmp.misfirePercent) || playerGunCmp.misfirePercent == 0)
                    {
                        float needVolume = _buildingCheckerComponentsPool.Value.Get(_sceneData.Value.playerEntity).isHideRoof ? 1f : 0.7f;
                        needVolume *= playerGunCmp.inScope ? 1f : 0.7f;
                        ref var oneShotSoundCmp = ref _oneShotSoundComponentsPool.Value.Add(_world.Value.NewEntity());
                        oneShotSoundCmp.audioSource = _sceneData.Value.PlaySoundFXClip(playerGunCmp.gunInfo.shotSound, moveCmp.entityTransform.position, needVolume);
                        oneShotSoundCmp.time = playerGunCmp.gunInfo.shotSound.length;

                        if (playerGunCmp.inScope)//mb not only in scope
                        {
                            float staminaChanged = (2f + gunInInvCmp.currentGunWeight) * 0.05f + playerGunCmp.gunInfo.damage * 0.02f;
                            moveCmp.currentRunTime -= staminaChanged;
                        }
                        for (int i = 0; i < gunCmp.currentBulletInfo.bulletCount; i++)
                            Shoot(playerEntity, LayerMask.GetMask("Enemy"));


                        if (gunCmp.currentSpread < gunCmp.currentMaxSpread)
                            gunCmp.currentSpread += gunCmp.currentAddedSpread * gunCmp.currentBulletInfo.addedSpreadMultiplayer;

                        var triggeredEnemies = Physics2D.CircleCastAll(moveCmp.entityTransform.position, playerGunCmp.currentShotSoundLenght, moveCmp.entityTransform.up, playerGunCmp.currentShotSoundLenght, LayerMask.GetMask("Enemy"));
                        foreach (var enemy in triggeredEnemies)
                        {
                            var directionToEnemy = enemy.transform.position - moveCmp.entityTransform.position;
                            RaycastHit2D raycastHit2D = Physics2D.Raycast(moveCmp.entityTransform.position, directionToEnemy.normalized, playerGunCmp.currentShotSoundLenght, LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Enemy"));

                            if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject.layer != 10)
                            {
                                int enemyEntity = enemy.transform.gameObject.GetComponent<HealthView>()._entity;
                                ref var aiCmpEnemy = ref _creatureAIComponentsPool.Value.Get(enemyEntity);
                                aiCmpEnemy.targetPositionCached = moveCmp.entityTransform.position;
                                aiCmpEnemy.reachedLastTarget = false;
                                if (aiCmpEnemy.currentState == CreatureAIComponent.CreatureStates.idle)
                                {
                                    aiCmpEnemy.timeFromLastTargetSeen = -5f;
                                    aiCmpEnemy.currentState = CreatureAIComponent.CreatureStates.follow;
                                }
                            }
                        }

                        if (!playerGunCmp.inScope && _sceneData.Value.mainCamera.orthographicSize > cameraCmp.currentMaxCameraSpread)
                        {
                            float addCamSpread = playerGunCmp.gunInfo.damage * gunCmp.currentBulletInfo.bulletCount * gunInInvCmp.currentGunWeight * 0.003f;
                            _sceneData.Value.mainCamera.orthographicSize -= (addCamSpread + addCamSpread * playerGunCmp.sumAddedCameraSpreadMultiplayer) * (1 - ((_sceneData.Value.mainCamera.orthographicSize - 5f) / (cameraCmp.currentMaxCameraSpread - 5f)));
                        }
                        else if (playerGunCmp.inScope && _sceneData.Value.mainCamera.orthographicSize > cameraCmp.currentMaxCameraSpread * playerGunCmp.currentScopeMultiplicity)
                        {
                            float addCamSpread = playerGunCmp.gunInfo.damage * gunCmp.currentBulletInfo.bulletCount * gunInInvCmp.currentGunWeight * 0.003f;
                            _sceneData.Value.mainCamera.orthographicSize -= (addCamSpread + addCamSpread * playerGunCmp.sumAddedCameraSpreadMultiplayer) * playerGunCmp.currentScopeMultiplicity * (1 - ((_sceneData.Value.mainCamera.orthographicSize / playerGunCmp.currentScopeMultiplicity - 5f)) / (cameraCmp.currentMaxCameraSpread - 5f));
                        }
                    }
                    playerAttackCmp.currentAttackCouldown = 0;

                    ref var playerStats = ref _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity);
                    playerStats.currentStatsExp[1] += playerAttackCmp.damage * gunCmp.currentBulletInfo.bulletCount * Mathf.Pow(0.95f, gunCmp.currentBulletInfo.bulletCount) * 0.05f;
                    if (playerStats.currentStatsExp[0] >= _sceneData.Value.levelExpCounts[playerStats.statLevels[0]])//0 потому что стат sili
                    {
                        playerStats.statLevels[0]++;
                        CalculateRecoil(ref gunCmp, playerGunCmp, false);
                    }

                    gunInInvCmp.gunDurability-= _globalTimeComponentsPool.Value.Get(playerEntity).currentWeatherType != GlobalTimeComponent.WeatherType.none && !_buildingCheckerComponentsPool.Value.Get(playerEntity).isHideRoof ?
                        gunCmp.currentBulletInfo.removedGunDurability * 1.5f : gunCmp.currentBulletInfo.removedGunDurability;

                    if (gunInInvCmp.gunDurability < 0)
                        gunInInvCmp.gunDurability = 0;
                    if (!playerGunCmp.gunInfo.bulletShellSpawnOnShot)
                        gunInInvCmp.bulletShellsToReload.Add(gunInInvCmp.currentAmmo[0]);

                    ref var invCmp = ref _inventoryComponentsPool.Value.Get(_sceneData.Value.inventoryEntity);
                    Debug.Log(gunInInvCmp.currentAmmo[0]);
                    float bulletWeight = _sceneData.Value.idItemslist.items[gunInInvCmp.currentAmmo[0]].itemWeight;
                    gunInInvCmp.currentAmmo.RemoveAt(0);//delete bullet
                    invCmp.weight -= bulletWeight;
                    gunInInvCmp.currentGunWeight -= bulletWeight;
                    _sceneData.Value.statsInventoryText.text = invCmp.weight + "kg/ " + invCmp.currentMaxWeight + "kg \n max cells " + invCmp.currentCellCount;
                    if (_sceneData.Value.dropedItemsUIView.gunMagazineUI.gameObject.activeInHierarchy)
                        playerGunCmp.bulletUIObjects[gunInInvCmp.currentAmmo.Count].gameObject.SetActive(false);

                    for (int i = 0; i < gunInInvCmp.currentAmmo.Count; i++)
                        playerGunCmp.bulletUIObjects[i].color = _sceneData.Value.idItemslist.items[gunInInvCmp.currentAmmo[i]].bulletInfo.uiBulletColor;

                    if (gunInInvCmp.currentAmmo.Count == 0 && !playerGunCmp.inScope && !gunCmp.isReloading && playerStats.weaponsExp[itemInfo.itemInfo.itemId].weaponExpLevel >= 7)
                    {
                        playerGunCmp.isContinueReload = true;
                        _reloadEventsPool.Value.Add(playerEntity);
                        if (!playerGunCmp.gunInfo.bulletShellSpawnOnShot && gunInInvCmp.bulletShellsToReload.Count != 0)
                        {
                            Vector2 spawnPosition = moveCmp.movementView.bulletShellSpawnPoint.position;
                            for (int i = 0; i < gunInInvCmp.bulletShellsToReload.Count; i++)
                            {
                                BulletShellSpawn(_sceneData.Value.idItemslist.items[gunInInvCmp.bulletShellsToReload[i]].bulletInfo.bulletCase, spawnPosition, playerView.movementView.weaponContainer.localEulerAngles.z, playerView.movementView.characterSpriteTransform.localRotation.y == 0);
                                if (i % 2 == 0)
                                    spawnPosition = new Vector2(spawnPosition.x, spawnPosition.y + 0.1f);
                            }
                            gunInInvCmp.bulletShellsToReload.Clear();
                        }
                    }
                    else if(gunInInvCmp.currentAmmo.Count != 0)
                    gunCmp.currentBulletInfo = _sceneData.Value.idItemslist.items[gunInInvCmp.currentAmmo[0]].bulletInfo;

                }
                else if (Input.GetMouseButtonDown(1) && playerGunCmp.currentScopeMultiplicity != 1 && !gunCmp.isReloading && !playerAttackCmp.weaponIsChanged && playerAttackCmp.canAttack && !curHealCmp.isHealing && !playerGunCmp.changedInScopeState)
                    ChangeScopeMultiplicity();

                else if (gunCmp.isReloading)
                {
                    gunCmp.currentReloadDuration += Time.deltaTime;

                    if (gunCmp.currentReloadDuration >= gunCmp.reloadDuration)
                    {
                        gunCmp.currentReloadDuration = 0;
                        gunInInvCmp.currentAmmo.AddRange(Enumerable.Repeat(playerGunCmp.bulletIdToreload,playerGunCmp.bulletCountToReload));
                        if (_sceneData.Value.dropedItemsUIView.gunMagazineUI.gameObject.activeInHierarchy)
                        {
                            if (gunCmp.isOneBulletReload)
                                playerGunCmp.bulletUIObjects[gunInInvCmp.currentAmmo.Count - 1].gameObject.SetActive(true);
                            else
                                for (int i = 0; i < gunInInvCmp.currentAmmo.Count; i++)
                                    playerGunCmp.bulletUIObjects[i].gameObject.SetActive(true);

                            for (int i = 0; i < gunInInvCmp.currentAmmo.Count; i++)
                                playerGunCmp.bulletUIObjects[i].color = _sceneData.Value.idItemslist.items[gunInInvCmp.currentAmmo[i]].bulletInfo.uiBulletColor;
                        }
                        ref var upgradeStatsCmp = ref _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity);
                        gunCmp.currentBulletInfo = _sceneData.Value.idItemslist.items[gunInInvCmp.currentAmmo[0]].bulletInfo;
                        upgradeStatsCmp.currentStatsExp[1] += playerGunCmp.bulletCountToReload * gunCmp.currentBulletInfo.bulletCount * playerAttackCmp.damage * 0.1f;
                        if (upgradeStatsCmp.currentStatsExp[1] >= _sceneData.Value.levelExpCounts[upgradeStatsCmp.statLevels[1]] && !_upgradePlayerStatEventsPool.Value.Has(_sceneData.Value.playerEntity))
                            _upgradePlayerStatEventsPool.Value.Add(_sceneData.Value.playerEntity).statIndex = 1;

                        if(playerGunCmp.gunInfo.endReloadSound != null)
                        playerCmp.view.movementView.weaponAudioSource.clip = playerGunCmp.gunInfo.endReloadSound;
                        if (gunCmp.isOneBulletReload && gunInInvCmp.currentAmmo.Count != gunCmp.magazineCapacity && playerGunCmp.isContinueReload && upgradeStatsCmp.weaponsExp[itemInfo.itemInfo.itemId].weaponExpLevel >= 7)
                        {
                            _reloadEventsPool.Value.Add(playerEntity);
                            return;
                        }
                        gunCmp.isReloading = false;
                        _sceneData.Value.ammoInfoText.text = "";
                    }
                }

                if (Input.GetKeyDown(KeyCode.R) && gunInInvCmp.currentAmmo.Count != gunCmp.magazineCapacity && (!playerGunCmp.inScope || (playerGunCmp.inScope && _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).weaponsExp[itemInfo.itemInfo.itemId].weaponExpLevel >= 6))
                    && !curHealCmp.isHealing && playerView.movementView.shieldView.shieldObject.localPosition == Vector3.zero && !playerAttackCmp.weaponIsChanged)
                {
                    if (!gunCmp.isReloading)
                    {
                        playerGunCmp.isContinueReload = true;
                        _reloadEventsPool.Value.Add(playerEntity);
                        if (!playerGunCmp.gunInfo.bulletShellSpawnOnShot && gunInInvCmp.bulletShellsToReload.Count != 0)
                        {
                            Vector2 spawnPosition = moveCmp.movementView.bulletShellSpawnPoint.position;
                            for (int i = 0; i < gunInInvCmp.bulletShellsToReload.Count; i++)
                            {
                                BulletShellSpawn(_sceneData.Value.idItemslist.items[gunInInvCmp.bulletShellsToReload[i]].bulletInfo.bulletCase, spawnPosition, playerView.movementView.weaponContainer.localEulerAngles.z, playerView.movementView.characterSpriteTransform.localRotation.y == 0);
                                if (i % 2 == 0)
                                    spawnPosition = new Vector2(spawnPosition.x, spawnPosition.y + 0.1f);
                            }
                            gunInInvCmp.bulletShellsToReload.Clear();
                        }
                    }
                    else
                        playerGunCmp.isContinueReload = !playerGunCmp.isContinueReload;
                }


                if (!playerGunCmp.changedInScopeState)
                {
                    if (gunCmp.currentSpread > gunCmp.currentMinSpread)
                        gunCmp.currentSpread -= gunCmp.spreadRecoverySpeed * Time.deltaTime;

                    else if (gunCmp.currentSpread < gunCmp.currentMinSpread)
                        gunCmp.currentSpread = gunCmp.currentMinSpread;
                }
            }

            else //милишная атака
            {
                ref var meleeAttackCmp = ref _meleeWeaponComponentsPool.Value.Get(playerEntity);
                ref var playerMeleeAttackCmp = ref _playerMeleeWeaponComponentsPool.Value.Get(playerEntity);
                    ref var playerStats = ref _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity);

                if ((playerMeleeAttackCmp.isAuto && (Input.GetMouseButton(0) || Input.GetMouseButton(1)) || (!playerMeleeAttackCmp.isAuto && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))))
                    && playerAttackCmp.currentAttackCouldown >= playerAttackCmp.attackCouldown && !playerAttackCmp.weaponIsChanged && playerAttackCmp.canAttack
                    && !curHealCmp.isHealing && !meleeAttackCmp.isHitting && moveCmp.currentRunTime > 0)
                {
                    Vector2 direction = (moveCmp.pointToRotateInput - (Vector2)moveCmp.entityTransform.position).normalized;
                    if (Physics2D.Raycast(playerCmp.view.playerTransform.position, direction, 1f, LayerMask.GetMask("Obstacle"))) continue;

                    playerStats.currentStatsExp[0] += playerAttackCmp.damage * 0.3f;
                    if (playerStats.currentStatsExp[0] >= _sceneData.Value.levelExpCounts[playerStats.statLevels[0]])
                        playerStats.statLevels[0]++;

                   // playerCmp.view.movementView.weaponAudioSource.clip = playerMeleeAttackCmp.weaponInfo.hitSound;
                    playerCmp.view.movementView.weaponAudioSource.Play();

                    _playerComponentsPool.Value.Get(playerEntity).view.movementView.weaponCollider.enabled = true;

                    if (playerMoveCmp.currentHungerPoints > 0)
                    {
                        playerMoveCmp.currentHungerPoints -= playerMeleeAttackCmp.weaponInfo.staminaUsage;

                        if (playerMoveCmp.currentHungerPoints < 0)
                            playerMoveCmp.currentHungerPoints = 0;

                        _sceneData.Value.playerArmorBarFilled.fillAmount = playerMoveCmp.currentHungerPoints / playerMoveCmp.maxHungerPoints;
                        _sceneData.Value.playerArmorText.text = (int)playerMoveCmp.currentHungerPoints + " / " + (int)playerMoveCmp.maxHungerPoints;
                    }
                    var weaponContainerTransform = _playerComponentsPool.Value.Get(playerEntity).view.movementView.weaponContainer.transform;
                    if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
                        meleeAttackCmp.isWideAttack = false;
                    else
                        meleeAttackCmp.isWideAttack = true;
                    
                    float needStaminaUsage = playerMeleeAttackCmp.weaponInfo.staminaUsage;
                    if (meleeAttackCmp.isWideAttack)
                        needStaminaUsage *= playerMeleeAttackCmp.weaponInfo.wideAttackDamageMultiplayer;
                    moveCmp.currentRunTime -= needStaminaUsage - (playerStats.statLevels[2] * needStaminaUsage * 0.02f);//1 lvl - 2% stamia use
                    if (meleeAttackCmp.isWideAttack)
                    {
                        var ray = new Ray2D(weaponContainerTransform.localPosition, weaponContainerTransform.up);
                        meleeAttackCmp.endHitPoint = ray.origin + (ray.direction * playerMeleeAttackCmp.weaponInfo.attackLenght * meleeAttackCmp.curAttackLenghtMultiplayer);
                    }
                    else
                        meleeAttackCmp.startRotation = _playerComponentsPool.Value.Get(playerEntity).view.movementView.weaponContainer.transform.eulerAngles.z;
                    Debug.Log(meleeAttackCmp.startRotation + " needAngle");
                    playerAttackCmp.currentAttackCouldown = 0;
                    meleeAttackCmp.isHitting = true;
                    meleeAttackCmp.moveInAttackSide = true;
                    meleeAttackCmp.attackState = 1;
                }

                if (meleeAttackCmp.isHitting)
                {
                    var weaponContainerTransform = _playerComponentsPool.Value.Get(playerEntity).view.movementView.weaponContainer.transform;
                    if (!meleeAttackCmp.isWideAttack)
                    {
                        if (meleeAttackCmp.attackState == 2)
                        {
                            float neededAngle = meleeAttackCmp.startRotation + playerMeleeAttackCmp.weaponInfo.wideAttackLenght * meleeAttackCmp.curAttackLenghtMultiplayer;

                            if (neededAngle > 360)
                                neededAngle -= 360;
                            weaponContainerTransform.transform.rotation = Quaternion.Slerp(weaponContainerTransform.transform.rotation, Quaternion.Euler(0, 0, neededAngle), playerMeleeAttackCmp.weaponInfo.wideAttackSpeed * Time.deltaTime);//вращение на поред угол                                                                                                                                                                                                   //weaponContainerTransform.transform.Rotate(0, 0, -playerMeleeAttackCmp.weaponInfo.attackSpeed * Time.deltaTime);

                            if ((int)weaponContainerTransform.transform.eulerAngles.z == Mathf.CeilToInt( neededAngle) || (int)weaponContainerTransform.transform.eulerAngles.z == Mathf.FloorToInt(neededAngle))
                                meleeAttackCmp.attackState = 3;

                            Debug.Log((int)neededAngle + " needAngle" + Mathf.CeilToInt(neededAngle) + "max" + Mathf.FloorToInt(neededAngle));
                        }
                        else if(meleeAttackCmp.attackState == 1)
                        {
                            float neededAngle = meleeAttackCmp.startRotation - playerMeleeAttackCmp.weaponInfo.wideAttackLenght * meleeAttackCmp.curAttackLenghtMultiplayer;
                            if (neededAngle < 0)
                                neededAngle += 360;
                            weaponContainerTransform.transform.rotation = Quaternion.Slerp(weaponContainerTransform.transform.rotation, Quaternion.Euler(0, 0, neededAngle), playerMeleeAttackCmp.weaponInfo.wideAttackSpeed * Time.deltaTime);
                            if ((int)weaponContainerTransform.transform.eulerAngles.z == meleeAttackCmp.startRotation)
                            {
                                meleeAttackCmp.isHitting = false;
                                _playerComponentsPool.Value.Get(playerEntity).view.movementView.weaponCollider.enabled = false;
                            }
                            if ((int)weaponContainerTransform.transform.eulerAngles.z == Mathf.CeilToInt(neededAngle) || (int)weaponContainerTransform.transform.eulerAngles.z == Mathf.FloorToInt(neededAngle))
                                meleeAttackCmp.attackState = 2;
                            Debug.Log((int)neededAngle + " needAngle" + Mathf.CeilToInt(neededAngle) + "max" + Mathf.FloorToInt(neededAngle));
                        }
                        else if(meleeAttackCmp.attackState == 3)
                        {
                            float neededAngle = meleeAttackCmp.startRotation - playerMeleeAttackCmp.weaponInfo.wideAttackLenght * meleeAttackCmp.curAttackLenghtMultiplayer;
                            if (neededAngle < 0)
                                neededAngle += 360;
                            weaponContainerTransform.transform.rotation = Quaternion.Slerp(weaponContainerTransform.transform.rotation, Quaternion.Euler(0, 0, meleeAttackCmp.startRotation), playerMeleeAttackCmp.weaponInfo.wideAttackSpeed * Time.deltaTime);
                            if ((int)weaponContainerTransform.transform.eulerAngles.z == Mathf.CeilToInt(meleeAttackCmp.startRotation) || (int)weaponContainerTransform.transform.eulerAngles.z == Mathf.FloorToInt(meleeAttackCmp.startRotation))
                            {
                                meleeAttackCmp.isHitting = false;
                                _playerComponentsPool.Value.Get(playerEntity).view.movementView.weaponCollider.enabled = false;
                            }
                            Debug.Log((int)neededAngle + " needAngle" + Mathf.CeilToInt(meleeAttackCmp.startRotation) + "max" + Mathf.FloorToInt(meleeAttackCmp.startRotation));
                        }
                        //доделать
                    }
                    else
                    {
                        if (meleeAttackCmp.moveInAttackSide)
                        {
                            weaponContainerTransform.localPosition = Vector2.MoveTowards(weaponContainerTransform.localPosition, meleeAttackCmp.endHitPoint, playerMeleeAttackCmp.weaponInfo.attackSpeed * Time.deltaTime);
                            if (weaponContainerTransform.localPosition == (Vector3)meleeAttackCmp.endHitPoint)
                                meleeAttackCmp.moveInAttackSide = false;
                        }
                        else
                        {
                            weaponContainerTransform.localPosition = Vector2.MoveTowards(weaponContainerTransform.localPosition, meleeAttackCmp.startHitPoint, playerMeleeAttackCmp.weaponInfo.attackSpeed * 2 * Time.deltaTime);
                            if (weaponContainerTransform.localPosition == (Vector3)meleeAttackCmp.startHitPoint)
                            {
                                weaponContainerTransform.localPosition = (Vector3)meleeAttackCmp.startHitPoint;
                                meleeAttackCmp.isHitting = false;
                                _playerComponentsPool.Value.Get(playerEntity).view.movementView.weaponCollider.enabled = false;
                            }
                        }

                    }
                }
            }
            if (playerAttackCmp.weaponIsChanged)
            {
                playerAttackCmp.currentChangeWeaponTime += Time.deltaTime;
                if (playerAttackCmp.currentChangeWeaponTime <= playerAttackCmp.changeWeaponTime / 2)
                {
                    if (gunCmp.gunSpritePositionRecoil > 0)
                        gunCmp.gunSpritePositionRecoil -= Time.deltaTime * (0.8f / playerAttackCmp.changeWeaponTime);
                }
                else
                {
                    gunCmp.gunSpritePositionRecoil += Time.deltaTime * (0.8f / playerAttackCmp.changeWeaponTime);
                    if (!playerAttackCmp.weaponSpriteIsChanged)
                    {
                        playerAttackCmp.weaponSpriteIsChanged = true;
                        var weaponInfo = _inventoryItemComponentsPool.Value.Get(_playerWeaponsInInventoryComponentsPool.Value.Get(playerEntity).curEquipedWeaponCellEntity).itemInfo;
                        if (weaponInfo.type == ItemInfo.itemType.gun)
                        {
                            moveCmp.movementView.weaponSpriteRenderer.sprite = weaponInfo.gunInfo.weaponSprite;
                            moveCmp.movementView.weaponSprite.localScale = new Vector3(1, -1, 1) * weaponInfo.gunInfo.spriteScaleMultiplayer;
                            moveCmp.movementView.weaponSprite.localEulerAngles = new Vector3(0, 0, weaponInfo.gunInfo.spriteRotation);
                        }
                        else
                        {
                            moveCmp.movementView.weaponSpriteRenderer.sprite = weaponInfo.meleeWeaponInfo.weaponSprite;
                            moveCmp.movementView.weaponSprite.localScale = new Vector3(1, -1, 1) * weaponInfo.meleeWeaponInfo.spriteScaleMultiplayer;
                            moveCmp.movementView.weaponSprite.localEulerAngles = new Vector3(0, 0, weaponInfo.meleeWeaponInfo.spriteRotation);
                        }
                    }
                }
                if (playerAttackCmp.currentChangeWeaponTime >= playerAttackCmp.changeWeaponTime)
                {
                    playerAttackCmp.weaponSpriteIsChanged = false;
                    playerAttackCmp.currentChangeWeaponTime = 0;
                    playerAttackCmp.weaponIsChanged = false;
                    _sceneData.Value.ammoInfoText.text = "";
                }
            }

            else if (!gunCmp.isReloading && !playerGunCmp.inScope && !curHealCmp.isHealing && !_meleeWeaponComponentsPool.Value.Get(playerEntity).isHitting)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) && inventoryGunsCmp.curWeapon != 0 && !_inventoryCellComponentsPool.Value.Get(_sceneData.Value.firstGunCellView._entity).isEmpty)
                    ChangeWeapon(0);
                else if (Input.GetKeyDown(KeyCode.Alpha2) && inventoryGunsCmp.curWeapon != 1 && !_inventoryCellComponentsPool.Value.Get(_sceneData.Value.secondGunCellView._entity).isEmpty)
                    ChangeWeapon(1);
                else if (Input.GetKeyDown(KeyCode.Alpha3) && inventoryGunsCmp.curWeapon != 2)
                    ChangeWeapon(2);
            }

            if (cameraCmp.blurValue > 1)
            {
                if (playerMoveCmp.currentHungerPoints > playerMoveCmp.maxHungerPoints * 0.2f)
                    cameraCmp.blurValue -= Time.deltaTime * 7;
                else
                    cameraCmp.blurValue -= Time.deltaTime * 2;
                if (!_menuStatesComponentsPool.Value.Get(playerEntity).inInventoryState)
                    _sceneData.Value.depthOfFieldMainBg.focalLength.value = cameraCmp.blurValue;
            }


        }

        foreach (var meleeWeaponContact in _meleeWeaponContactEventsFilter.Value)
        {
            var meleeContactEventCmp = _meleeWeaponContactEventsPool.Value.Get(meleeWeaponContact);
            int attackedEntity = meleeContactEventCmp.attackedEntity;
            bool isShield = meleeContactEventCmp.isShield;
            bool isHeadshot = meleeContactEventCmp.isHeadshot;
            Vector2 contactPosition = meleeContactEventCmp.contactPosition;
            var attackCmp = _attackComponentsPool.Value.Get(meleeWeaponContact);

            int entityOfCreature = attackedEntity;
            if (attackedEntity == _sceneData.Value.shieldCellView._entity)
                entityOfCreature = _sceneData.Value.playerEntity;

            var meleeWeaponCmp = _meleeWeaponComponentsPool.Value.Get(meleeWeaponContact);
            int needDamage = attackCmp.damage;

            if (isHeadshot)
                needDamage *= 2;

            int needPaticleNum = 0;
            if (isShield || !_movementComponentsPool.Value.Has(attackedEntity))
                needPaticleNum = 1;
            var particles = CreateParticles(needPaticleNum);
            particles.gameObject.transform.position = contactPosition;
            if (meleeWeaponContact == _sceneData.Value.playerEntity)//нанесение урона врагу
            {
                var meleeCmp = _playerMeleeWeaponComponentsPool.Value.Get(meleeWeaponContact);
                if (meleeWeaponCmp.isWideAttack)
                    needDamage = Mathf.CeilToInt(needDamage * meleeCmp.weaponInfo.wideAttackDamageMultiplayer);
                if (!isShield)
                    _changeHealthEventsPool.Value.Add(_world.Value.NewEntity()).SetParametrs(needDamage, attackedEntity, isHeadshot, meleeWeaponCmp.isWideAttack?0.8f:0.6f);
                else
                {
                    ref var shieldCmp = ref _shieldComponentsPool.Value.Get(attackedEntity);
                    shieldCmp.currentDurability -= needDamage;//пока неизвестно где у врага будут лежать параметры щита
                    _playerComponentsPool.Value.Get(meleeWeaponContact).view.movementView.weaponCollider.enabled = false;
                }
                if (_movementComponentsPool.Value.Has(attackedEntity))
                {
                    ref var attackedCreatureMoveCmp = ref _movementComponentsPool.Value.Get(attackedEntity);
                    attackedCreatureMoveCmp.isStunned = true;
                    attackedCreatureMoveCmp.stunTime = meleeCmp.weaponInfo.stunTime;
                    if (!attackedCreatureMoveCmp.canMove)
                        attackedCreatureMoveCmp.canMove = true;
                    //Debug.Log(moveCmp.stunTime + "st time");
                    attackedCreatureMoveCmp.moveSpeed = meleeCmp.weaponInfo.knockbackSpeed;
                    attackedCreatureMoveCmp.moveInput = (attackedCreatureMoveCmp.entityTransform.position - _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform.position).normalized;
                }
                if (_creatureAIComponentsPool.Value.Has(attackedEntity) && ((meleeWeaponCmp.isWideAttack && (float)needDamage / 3 > Random.Range(0, 100)) || (!meleeWeaponCmp.isWideAttack && (float)needDamage / 2 > Random.Range(0, 100))))
                {
                    float effectTime = needDamage / 3;
                    if (_creatureAIComponentsPool.Value.Get(attackedEntity).armorInfo != null)
                        effectTime *= 1 - _creatureAIComponentsPool.Value.Get(attackedEntity).armorInfo.bleedingResistance;
                    ref var effCmp = ref _effectComponentsPool.Value.Add(_world.Value.NewEntity());
                    effCmp.effectEntity = attackedEntity;

                    int bleedingLevel = 1;
                    if (!meleeWeaponCmp.isWideAttack && needDamage > 40)
                        bleedingLevel = 2;

                    effCmp.effectLevel = bleedingLevel;
                    effCmp.effectType = EffectInfo.EffectType.bleeding;
                    effCmp.isFirstEffectCheck = true;
                    effCmp.effectDuration = effectTime;

                }
            }
            else
            {
                ref var creatureAiCmp = ref _creatureAIComponentsPool.Value.Get(meleeWeaponContact);
                var meleeCmp = _creatureInventoryComponentsPool.Value.Get(meleeWeaponContact).meleeWeaponItem.meleeWeaponInfo;
                if (meleeWeaponCmp.isWideAttack)
                    needDamage = Mathf.CeilToInt(needDamage * meleeCmp.wideAttackDamageMultiplayer);
                if (!isShield)
                    _changeHealthEventsPool.Value.Add(_world.Value.NewEntity()).SetParametrs(needDamage, attackedEntity, isHeadshot, meleeWeaponCmp.isWideAttack ? 0.8f : 0.6f);
                else
                {
                    ref var shieldCmp = ref _shieldComponentsPool.Value.Get(attackedEntity);
                    shieldCmp.currentDurability -= Mathf.CeilToInt(needDamage * _inventoryItemComponentsPool.Value.Get(attackedEntity).itemInfo.sheildInfo.damagePercent);
                    creatureAiCmp.creatureView.movementView.weaponCollider.enabled = false;
                    //после удара об щит коллайдер пропадает
                }
                if (_movementComponentsPool.Value.Has(entityOfCreature))
                {
                    ref var attackedCreatureMoveCmp = ref _movementComponentsPool.Value.Get(entityOfCreature);
                    attackedCreatureMoveCmp.isStunned = true;

                    attackedCreatureMoveCmp.stunTime = meleeCmp.stunTime;
                    attackedCreatureMoveCmp.moveSpeed = meleeCmp.knockbackSpeed;

                    attackedCreatureMoveCmp.moveInput = (attackedCreatureMoveCmp.entityTransform.position - _movementComponentsPool.Value.Get(meleeWeaponContact).entityTransform.position).normalized;
                    
                }

                if (_playerComponentsPool.Value.Has(attackedEntity) && ((meleeWeaponCmp.isWideAttack && (float)needDamage / 3 > Random.Range(0, 100)) || (!meleeWeaponCmp.isWideAttack && (float)needDamage / 2 > Random.Range(0, 100))))
                {
                    float effectTime = needDamage / 3;
                    if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.bodyArmorCellView._entity))
                        effectTime *= 1 - _inventoryItemComponentsPool.Value.Get(_sceneData.Value.bodyArmorCellView._entity).itemInfo.bodyArmorInfo.bleedingResistance;
                    ref var effCmp = ref _effectComponentsPool.Value.Add(_world.Value.NewEntity());
                    effCmp.effectEntity = attackedEntity;

                    int bleedingLevel = 1;
                    if (!meleeWeaponCmp.isWideAttack && needDamage > 40)
                        bleedingLevel = 2;
                    effCmp.effectLevel = bleedingLevel;
                    effCmp.effectType = EffectInfo.EffectType.bleeding;
                    effCmp.isFirstEffectCheck = true;
                    effCmp.effectIconSprite = _sceneData.Value.bloodEffectsSprites[bleedingLevel - 1];
                    effCmp.effectDuration = effectTime;
                }
            }
        }
        CheckLifetimeObjectsTime();
    }

    private void ChangeCreatureWeapon(int creatureEntity)
    {
        ref var attackCmp = ref _attackComponentsPool.Value.Get(creatureEntity);
        ref var creatureAiCmp = ref _creatureAIComponentsPool.Value.Get(creatureEntity);
        if (_meleeWeaponComponentsPool.Value.Get(creatureEntity).isHitting || _gunComponentsPool.Value.Get(creatureEntity).isReloading) return;

        _creatureInventoryComponentsPool.Value.Get(creatureEntity).isSecondWeaponUsed = !_creatureInventoryComponentsPool.Value.Get(creatureEntity).isSecondWeaponUsed;
        ref var creatureAiInvCmp = ref _creatureInventoryComponentsPool.Value.Get(creatureEntity);

        if (_creatureInventoryComponentsPool.Value.Get(creatureEntity).isSecondWeaponUsed)//милишка
        {
            var meleeWeaponView = creatureAiInvCmp.meleeWeaponItem.meleeWeaponInfo;
            if (creatureAiInvCmp.helmetItem == null || (creatureAiInvCmp.helmetItem != null && !_currentHealingItemComponentsPool.Value.Get(creatureEntity).isHealing))
            {
                creatureAiCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = meleeWeaponView.weaponSprite;

                creatureAiCmp.creatureView.aiCreatureView.itemTransform.localScale = new Vector3(1, -1, 1) * meleeWeaponView.spriteScaleMultiplayer;
                creatureAiCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, meleeWeaponView.spriteRotation);

            }
            //сделать поворот и скейл

            attackCmp.attackCouldown = meleeWeaponView.attackCouldown;
            attackCmp.damage = meleeWeaponView.damage;
            attackCmp.weaponRotateSpeed = (50f / (creatureAiInvCmp.meleeWeaponItem.itemWeight) + 1) * creatureAiInvCmp.enemyClassSettingInfo.weaponRotationSpeedMultiplayer;
        }
        else
        {
            var gunItem = creatureAiInvCmp.gunItem.gunInfo;
            if (creatureAiInvCmp.helmetItem == null || (creatureAiInvCmp.helmetItem != null && !_currentHealingItemComponentsPool.Value.Get(creatureEntity).isHealing))
            {
                creatureAiCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = gunItem.weaponSprite;

                creatureAiCmp.creatureView.aiCreatureView.itemTransform.localScale = new Vector3(1, -1, 1) * gunItem.spriteScaleMultiplayer;
                creatureAiCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, gunItem.spriteRotation);
            }
            attackCmp.attackCouldown = gunItem.attackCouldown;
            attackCmp.damage = gunItem.damage;
            attackCmp.weaponRotateSpeed = (5f / (creatureAiInvCmp.gunItem.itemWeight) + 1.5f) * creatureAiInvCmp.enemyClassSettingInfo.weaponRotationSpeedMultiplayer;
        }
    }
    private void CalculateRecoil(ref GunComponent gunCmp, PlayerGunComponent playerGunComponent, bool isScopeCalculate)
    {
        gunCmp.currentAddedSpread = playerGunComponent.addedSpread + playerGunComponent.addedSpread * playerGunComponent.durabilityGunMultiplayer;
        gunCmp.currentMaxSpread = playerGunComponent.maxSpread + playerGunComponent.maxSpread * playerGunComponent.durabilityGunMultiplayer;
        gunCmp.currentMinSpread = playerGunComponent.minSpread + playerGunComponent.minSpread * playerGunComponent.durabilityGunMultiplayer;
        if (isScopeCalculate)
        {
            float scopeRecoilMultiplayer = Mathf.Pow(0.85f, playerGunComponent.currentScopeMultiplicity);
            if (playerGunComponent.inScope)
            {
                gunCmp.currentAddedSpread *= scopeRecoilMultiplayer;//переделать на степени
                gunCmp.currentMaxSpread *= scopeRecoilMultiplayer;
                gunCmp.currentMinSpread *= scopeRecoilMultiplayer;
            }
            /* if(!playerGunComponent.inScope)
             {
                 gunCmp.currentAddedSpread += playerGunComponent.addedSpread / scopeRecoilMultiplayer;
                 gunCmp.currentMaxSpread += playerGunComponent.maxSpread / scopeRecoilMultiplayer;
                 gunCmp.currentMinSpread += playerGunComponent.minSpread / scopeRecoilMultiplayer;
             }*/
        }
        float statRecoil = _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).statLevels[0] / 50f;
        gunCmp.currentAddedSpread -= playerGunComponent.addedSpread * statRecoil;
        gunCmp.currentMaxSpread -= playerGunComponent.maxSpread * statRecoil;
        gunCmp.currentMinSpread -= playerGunComponent.minSpread * statRecoil;

        if (_movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).isRun)
        {
            gunCmp.currentAddedSpread /= 0.6f;
            gunCmp.currentMaxSpread /= 0.6f;
            gunCmp.currentMinSpread /= 0.6f;
        }
        else if (_playerMoveComponentsPool.Value.Get(_sceneData.Value.playerEntity).nowIsMoving)
        {
            gunCmp.currentAddedSpread /= 0.8f;
            gunCmp.currentMaxSpread /= 0.8f;
            gunCmp.currentMinSpread /= 0.8f;
        }
        if (_playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view.movementView.shieldView.shieldObject.localPosition != Vector3.zero)
            gunCmp.currentMinSpread /= _inventoryItemComponentsPool.Value.Get(_sceneData.Value.shieldCellView._entity).itemInfo.sheildInfo.recoilPercent;

        //   Debug.Log("min recoil " + gunCmp.currentMinSpread + " max recoil " + gunCmp.currentMaxSpread + " dur mult" + playerGunComponent.durabilityGunMultiplayer);

    }
    private void ChangeScopeMultiplicity()
    {
        int playerEntity = _sceneData.Value.playerEntity;

        if (_playerWeaponsInInventoryComponentsPool.Value.Get(playerEntity).curWeapon == 2)
            return;

        ref var playerCmp = ref _playerComponentsPool.Value.Get(playerEntity);
        if (playerCmp.view.movementView.shieldView.shieldObject.localPosition != Vector3.zero) return;
        ref var plyerGunCmp = ref _playerGunComponentsPool.Value.Get(playerEntity);
        ref var gunCmp = ref _gunComponentsPool.Value.Get(playerEntity);
        ref var cameraCmp = ref _cameraComponentsPool.Value.Get(playerEntity);
        ref var moveCmp = ref _movementComponentsPool.Value.Get(playerEntity);
        ref var fovCmp = ref _fieldOfViewComponentsPool.Value.Get(playerEntity);

        if (!plyerGunCmp.inScope)
        {
            moveCmp.isRun = false;
            _sceneData.Value.dropedItemsUIView.scopeCrossCentreImage.gameObject.SetActive(true);
            fovCmp.fieldOfView /= plyerGunCmp.currentScopeMultiplicity;
            fovCmp.viewDistance *= plyerGunCmp.currentScopeMultiplicity;

            moveCmp.moveSpeed /= plyerGunCmp.currentScopeMultiplicity;//придумать ураввнение скорости получше
            plyerGunCmp.cameraOrtograficalSizeDifference = _sceneData.Value.mainCamera.orthographicSize / 5;
        }
        else
        {
            _sceneData.Value.dropedItemsUIView.scopeCrossCentreImage.gameObject.SetActive(false);
            moveCmp.moveSpeed *= plyerGunCmp.currentScopeMultiplicity;

            fovCmp.fieldOfView *= plyerGunCmp.currentScopeMultiplicity;
            fovCmp.viewDistance /= plyerGunCmp.currentScopeMultiplicity;
            plyerGunCmp.cameraOrtograficalSizeDifference = _sceneData.Value.mainCamera.orthographicSize / (5 * plyerGunCmp.currentScopeMultiplicity);
        }
        plyerGunCmp.inScope = !plyerGunCmp.inScope;

        if (playerCmp.nvgIsUsed && plyerGunCmp.inScope && plyerGunCmp.currentScopeMultiplicity > 2.1f)
        {
            var globalTimeCmp = _globalTimeComponentsPool.Value.Get(playerEntity);
            playerCmp.nvgIsUsed = false;
            if (_buildingCheckerComponentsPool.Value.Get(playerEntity).isHideRoof)
                _sceneData.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity - 0.35f;
            else
                _sceneData.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity;
            if (globalTimeCmp.currentDayTime >= 15)
                _sceneData.Value.gloabalLight.color = _sceneData.Value.globalLightColors[2];
            else if (globalTimeCmp.currentDayTime == 12 || globalTimeCmp.currentDayTime == 0)
                _sceneData.Value.gloabalLight.color = _sceneData.Value.gloabalLight.color = globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none ? _sceneData.Value.globalLightColors[1] : _sceneData.Value.globalLightColors[4];
            else
                _sceneData.Value.gloabalLight.color = _sceneData.Value.gloabalLight.color = globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none ? _sceneData.Value.globalLightColors[0] : _sceneData.Value.globalLightColors[3];
            _sceneData.Value.bloomMainBg.intensity.value = 0;
            if (_sceneData.Value.gloabalLight.intensity < 0)
                _sceneData.Value.gloabalLight.intensity = 0;
        }

        CalculateRecoil(ref gunCmp, plyerGunCmp, true);
        plyerGunCmp.timeAfterChangedInScopeState = 1f;
        plyerGunCmp.changedInScopeState = true;
    }
    private void ChangeWeapon(int curWeapon)
    {
        ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);

        if (weaponsInInventoryCmp.curWeapon == 0)
        {

            if (_laserPointerForGunComponentsPool.Value.Has(weaponsInInventoryCmp.curEquipedWeaponCellEntity) && _laserPointerForGunComponentsPool.Value.Get(weaponsInInventoryCmp.curEquipedWeaponCellEntity).laserIsOn)
            {
                ref var laserPointerCmp = ref _laserPointerForGunComponentsPool.Value.Get(weaponsInInventoryCmp.curEquipedWeaponCellEntity);
                laserPointerCmp.laserIsOn = false;
                _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view.laserPointerLineRenderer.gameObject.SetActive(laserPointerCmp.laserIsOn);
            }
        }

        else if (weaponsInInventoryCmp.curWeapon == 1)
        {

            if (_laserPointerForGunComponentsPool.Value.Has(weaponsInInventoryCmp.curEquipedWeaponCellEntity) && _laserPointerForGunComponentsPool.Value.Get(weaponsInInventoryCmp.curEquipedWeaponCellEntity).laserIsOn)
            {
                ref var laserPointerCmp = ref _laserPointerForGunComponentsPool.Value.Get(weaponsInInventoryCmp.curEquipedWeaponCellEntity);
                laserPointerCmp.laserIsOn = false;
                _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view.laserPointerLineRenderer.gameObject.SetActive(laserPointerCmp.laserIsOn);
            }
        }
        
        ref var attackCmp = ref _attackComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        weaponsInInventoryCmp.curWeapon = curWeapon;



        if (curWeapon <= 1)
            ChangeGunStats(curWeapon);

        else if (curWeapon == 2)
        {
            //смена на ближнее оружие
            ChangeMeleeWeaponStats();
        }

        _sceneData.Value.ammoInfoText.text = "смена оружия";
        attackCmp.weaponIsChanged = true;

    }

    private void ChangeMeleeWeaponStats()
    {
        // менять статы милишки
        //менять размер коллайдера 
        _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity).curEquipedWeaponCellEntity = _sceneData.Value.meleeWeaponCellView._entity;
        var itemInfo = _inventoryItemComponentsPool.Value.Get(_sceneData.Value.meleeWeaponCellView._entity).itemInfo;
        var meleeWeaponInfo = itemInfo.meleeWeaponInfo;
        ref var curAttackCmp = ref _attackComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var playerMeleeWeaponCmp = ref _playerMeleeWeaponComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var meleeWeaponCmp = ref _meleeWeaponComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        curAttackCmp.attackCouldown = meleeWeaponInfo.attackCouldown;
        curAttackCmp.changeWeaponTime = meleeWeaponInfo.weaponChangeSpeed;

        playerMeleeWeaponCmp.isAuto = _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).weaponsExp[_inventoryItemComponentsPool.Value.Get(_sceneData.Value.meleeWeaponCellView._entity).itemInfo.itemId].weaponExpLevel >= 5;

        playerMeleeWeaponCmp.weaponInfo = meleeWeaponInfo;
        var playerView = _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view;
        if (playerView.laserPointerLineRenderer.gameObject.activeInHierarchy)
            playerView.laserPointerLineRenderer.gameObject.SetActive(false);

        foreach(var gunPartSprite in playerView.gunPartsSprites)
            gunPartSprite.sprite = _sceneData.Value.transparentSprite;

        playerView.leftRecoilTracker.gameObject.SetActive(false);
        playerView.rightRecoilTracker.gameObject.SetActive(false);
        if (playerView.movementView.shieldView.shieldObject.localPosition == Vector3.zero)
            meleeWeaponCmp.curAttackLenghtMultiplayer = meleeWeaponInfo.attackLenght;
        else
            meleeWeaponCmp.curAttackLenghtMultiplayer = meleeWeaponInfo.attackLenght * _inventoryItemComponentsPool.Value.Get(_sceneData.Value.shieldCellView._entity).itemInfo.sheildInfo.recoilPercent;
        //curAttackCmp.damage = Mathf.CeilToInt(meleeWeaponInfo.damage + (meleeWeaponInfo.damage * _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).statLevels[2] * 0.05f));
        //   Debug.Log("def m d" + meleeWeaponInfo.damage + " cur m d " + curAttackCmp.damage);
        curAttackCmp.damage = meleeWeaponInfo.damage;//от левела оружия зависит
        ref var playerGunCmp = ref _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        _sceneData.Value.dropedItemsUIView.gunMagazineUI.gameObject.SetActive(false);
        for (int i = 0; i < playerGunCmp.bulletUIObjects.Count; i++)
        {
            var curBulletUI = playerGunCmp.bulletUIObjects[i];
            if (!playerGunCmp.bulletUIObjects[i].gameObject.activeInHierarchy) break;
            curBulletUI.gameObject.SetActive(false);
        }
        if (!meleeWeaponInfo.isOneHandedWeapon && !_inventoryCellComponentsPool.Value.Get(_sceneData.Value.shieldCellView._entity).isEmpty && playerView.movementView.shieldView.shieldObject.localPosition != Vector3.zero)
        {
            playerView.movementView.shieldView.shieldObject.SetParent(playerView.movementView.shieldView.shieldContainer);
            playerView.movementView.shieldView.shieldObject.localPosition = Vector3.zero;
            playerView.movementView.shieldView.shieldObject.localRotation = Quaternion.Euler(0, 180, 0);
            playerView.movementView.shieldView.shieldSpriteRenderer.sortingOrder = 2;
        }

        playerView.movementView.weaponCollider.size = meleeWeaponInfo.colliderSize;

        curAttackCmp.weaponRotateSpeed = 10f / itemInfo.itemWeight;
    }
    private void ChangeGunStats(int curGun)
    {
        ref var playerStatsCmp = ref _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity);
        //перемещать точку выстрела
        var playerView = _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view;
        ref var curAttackCmp = ref _attackComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var playerGunCmp = ref _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var weaponsInInvCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var gunCmp = ref _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);


        if (curGun == 1)
            weaponsInInvCmp.curEquipedWeaponCellEntity = _sceneData.Value.secondGunCellView._entity;
        else
            weaponsInInvCmp.curEquipedWeaponCellEntity = _sceneData.Value.firstGunCellView._entity;
        ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(weaponsInInvCmp.curEquipedWeaponCellEntity);
        var itemInfo = _inventoryItemComponentsPool.Value.Get(weaponsInInvCmp.curEquipedWeaponCellEntity).itemInfo;
        var gunInfo = itemInfo.gunInfo;
        float durabilityPoints = gunInInvCmp.gunDurability;
        int weaponLevel = playerStatsCmp.weaponsExp[itemInfo.itemId].weaponExpLevel;

        if (gunInfo.scopeMultiplicity != 1)
        {
            playerGunCmp.currentScopeMultiplicity = gunInfo.scopeMultiplicity;
            _sceneData.Value.dropedItemsUIView.scopeCrossCentreImage.sprite = gunInfo.centreCrossSprite;
            _sceneData.Value.dropedItemsUIView.scopeCrossCentreImage.rectTransform.sizeDelta = new Vector2(gunInfo.centreCrossSprite.rect.width * 3, gunInfo.centreCrossSprite.rect.height * 3);
            if (gunInfo.blackBGSize != 0)
            {
                _sceneData.Value.dropedItemsUIView.scopeBlackBGImage.gameObject.SetActive(true);
                _sceneData.Value.dropedItemsUIView.scopeBlackBGImage.rectTransform.sizeDelta = Vector2.one * gunInfo.blackBGSize;
            }
            else
                _sceneData.Value.dropedItemsUIView.scopeBlackBGImage.gameObject.SetActive(false);
        }
        else if (gunInInvCmp.gunPartsId[1] != 0)
        {
            var gunPartInfo = _sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[1]].gunPartInfo;
            playerGunCmp.currentScopeMultiplicity = gunPartInfo.scopeMultiplicity;

            _sceneData.Value.dropedItemsUIView.scopeCrossCentreImage.sprite = gunPartInfo.centreScopeSprite;
            _sceneData.Value.dropedItemsUIView.scopeCrossCentreImage.rectTransform.sizeDelta = new Vector2(gunPartInfo.centreScopeSprite.rect.width * 3, gunPartInfo.centreScopeSprite.rect.height * 3);
            if (gunPartInfo.blackBGSize != 0)
            {
                _sceneData.Value.dropedItemsUIView.scopeBlackBGImage.gameObject.SetActive(true);
                _sceneData.Value.dropedItemsUIView.scopeBlackBGImage.rectTransform.sizeDelta = Vector2.one * gunPartInfo.blackBGSize;
            }
            else
                _sceneData.Value.dropedItemsUIView.scopeBlackBGImage.gameObject.SetActive(false);

        }
        else
        {
            playerGunCmp.currentScopeMultiplicity = 1;
            _sceneData.Value.dropedItemsUIView.scopeCrossCentreImage.gameObject.SetActive(false);
        }

        playerGunCmp.sumAddedAttackLenghtMultiplayer = 0;
        playerGunCmp.sumAddedCameraSpreadMultiplayer = 0;
        playerGunCmp.sumAddedReloadSpeedMultiplayer = 0;
        playerGunCmp.sumAddedSpreadMultiplayer = 0;
        playerGunCmp.sumAddedWeaponChangeSpeedMultiplayer = 0;
        playerGunCmp.currentShotSoundLenght = gunInfo.shotSoundDistance;

        for (int i = 0; i < gunInInvCmp.gunPartsId.Length; i++)
        {
            if (gunInInvCmp.gunPartsId[i] != 0)
            {
                var gunPartInfo = _sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[i]].gunPartInfo;

                if (gunPartInfo.shotSoundLenghtMultiplayer != 0)
                    playerGunCmp.currentShotSoundLenght *= gunPartInfo.shotSoundLenghtMultiplayer;
                playerGunCmp.sumAddedAttackLenghtMultiplayer += gunPartInfo.attackLenght;
                playerGunCmp.sumAddedCameraSpreadMultiplayer += gunPartInfo.cameraSpreadMultiplayer;
                playerGunCmp.sumAddedReloadSpeedMultiplayer += gunPartInfo.reloadSpeedMultiplayer;
                playerGunCmp.sumAddedSpreadMultiplayer += gunPartInfo.spreadMultiplayer;
                playerGunCmp.sumAddedWeaponChangeSpeedMultiplayer += gunPartInfo.weaponChangeSpeedMultiplayer;
                //change gp sprites\/
                playerView.gunPartsSprites[i].sprite = gunPartInfo.spriteForAddOnGunModel;
                playerView.gunPartsSprites[i].transform.localPosition = gunPartInfo.inGameOffset + gunInfo.gunPartsInGamePositions[i];
            }
            else
                playerView.gunPartsSprites[i].sprite = _sceneData.Value.transparentSprite;
        }

        gunCmp.attackLeght = gunInfo.attackLenght * (1 + playerGunCmp.sumAddedAttackLenghtMultiplayer);
        curAttackCmp.changeWeaponTime = gunInfo.weaponChangeSpeed + gunInfo.weaponChangeSpeed * playerGunCmp.sumAddedWeaponChangeSpeedMultiplayer - playerStatsCmp.statLevels[1] * gunInfo.reloadDuration * 0.02f;//по 2% за уровень ловкости
        gunCmp.reloadDuration = gunInfo.reloadDuration + gunInfo.reloadDuration * playerGunCmp.sumAddedReloadSpeedMultiplayer - playerStatsCmp.statLevels[1] * gunInfo.reloadDuration * 0.015f - weaponLevel * 0.015f;//по 1.5% за уровень ловкости
        curAttackCmp.attackCouldown = gunInfo.attackCouldown;

        playerGunCmp.gunInfo = gunInfo;
        gunCmp.magazineCapacity = gunInfo.magazineCapacity;
        gunCmp.currentAddedSpread = gunInfo.addedSpread;
        playerGunCmp.isAuto = gunInfo.isAuto;

       
        gunCmp.isOneBulletReload = gunInfo.isOneBulletReloaded;
        playerView.lightFromGunShot.gameObject.transform.localPosition = new Vector2(playerGunCmp.gunInfo.firepointPosition.x, playerGunCmp.gunInfo.firepointPosition.y);

        gunCmp.flashShotInstance = gunInfo.shotFlashIntance;
        playerView.lightFromGunShot.pointLightInnerAngle = gunInfo.shotFlashAngle;
        playerView.lightFromGunShot.pointLightOuterAngle = gunInfo.shotFlashAngle;
        playerView.lightFromGunShot.pointLightOuterRadius = gunInfo.shotFlashDistance;

        gunCmp.currentMaxSpread = gunInfo.maxSpread;
        gunCmp.currentMinSpread = gunInfo.minSpread;
        gunCmp.currentSpread = gunInfo.minSpread;

        playerGunCmp.maxSpread = gunInfo.maxSpread;
        playerGunCmp.minSpread = gunInfo.minSpread;
        playerGunCmp.addedSpread = gunInfo.addedSpread + playerGunCmp.sumAddedSpreadMultiplayer * gunInfo.addedSpread;

        if (gunInInvCmp.gunPartsId[2] != 0 && _sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[2]].gunPartInfo.laserMaxLenght != 0)//
        {
            var gunPartinfo = _sceneData.Value.idItemslist.items[gunInInvCmp.gunPartsId[2]].gunPartInfo;
            playerView.laserPointerLineRenderer.colorGradient = gunPartinfo.laserColor;
            playerView.laserPointerTransform.localPosition = gunPartinfo.laserPosition;
        }
        else if (playerView.laserPointerLineRenderer.gameObject.activeInHierarchy)
            playerView.laserPointerLineRenderer.gameObject.SetActive(false);

        if (durabilityPoints < playerGunCmp.gunInfo.maxDurabilityPoints * 0.6f)
        {
            playerGunCmp.durabilityGunMultiplayer = (float)System.Math.Round(1 - (durabilityPoints / ((float)playerGunCmp.gunInfo.maxDurabilityPoints * 1.6f) + 0.4f), 2);
            playerGunCmp.misfirePercent = Mathf.FloorToInt((playerGunCmp.durabilityGunMultiplayer) * 60);
            // Debug.Log(playerGunCmp.misfirePercent + "dur chnge ");

            CalculateRecoil(ref gunCmp, playerGunCmp, false);
        }
        else
        {
            playerGunCmp.durabilityGunMultiplayer = 0;
            playerGunCmp.misfirePercent = 0;
            CalculateRecoil(ref gunCmp, playerGunCmp, false);
        }

        _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).movementView.bulletShellSpawnPoint.localPosition = gunInfo.bulletShellPointPosition;

        if (weaponLevel >= 4)
        {
            playerView.leftRecoilTracker.gameObject.SetActive(true);
            playerView.rightRecoilTracker.gameObject.SetActive(true);
        }
        else
        {
            playerView.leftRecoilTracker.gameObject.SetActive(false);
            playerView.rightRecoilTracker.gameObject.SetActive(false);
        }

        if (weaponLevel >= 0)// need 2, 0 for test
        {
            var magUI = _sceneData.Value.dropedItemsUIView.gunMagazineUI;
            // Debug.Log("setactive mag");
            magUI.gameObject.SetActive(true);
            magUI.sprite = gunInfo.magUISprite;
            magUI.rectTransform.sizeDelta = gunInfo.magUISize;
            magUI.rectTransform.anchoredPosition = gunInfo.magUIPosition;
            if (playerGunCmp.bulletUIObjects.Count < gunInfo.magazineCapacity)
            {
                for (int i = 0; i < playerGunCmp.bulletUIObjects.Count; i++)
                {
                    var curBulletUI = playerGunCmp.bulletUIObjects[i];
                    curBulletUI.gameObject.SetActive(true);
                    curBulletUI.rectTransform.anchoredPosition = gunInfo.bulletsUIPositions[i];
                }
                for (int i = playerGunCmp.bulletUIObjects.Count; i < gunInfo.magazineCapacity; i++)
                {
                    var curBulletUI = _sceneData.Value.GetBulletForMagUI();
                    curBulletUI.rectTransform.anchoredPosition = gunInfo.bulletsUIPositions[i];
                    playerGunCmp.bulletUIObjects.Add(curBulletUI);
                }
                for (int i = gunInInvCmp.currentAmmo.Count; i < gunInfo.magazineCapacity; i++)
                {
                    var curBulletUI = playerGunCmp.bulletUIObjects[i];
                    curBulletUI.gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < gunInfo.magazineCapacity; i++)
                {
                    var curBulletUI = playerGunCmp.bulletUIObjects[i];
                    if (gunInInvCmp.currentAmmo.Count > i)
                        curBulletUI.gameObject.SetActive(true);
                    else
                        curBulletUI.gameObject.SetActive(false);
                    curBulletUI.rectTransform.anchoredPosition = gunInfo.bulletsUIPositions[i];
                }

                for (int i = gunInfo.magazineCapacity; i < playerGunCmp.bulletUIObjects.Count; i++)
                {
                    var curBulletUI = playerGunCmp.bulletUIObjects[i];
                    if (!curBulletUI.gameObject.activeInHierarchy) break;
                    curBulletUI.gameObject.SetActive(false);
                }
            }
            //if (playerGunCmp.bulletUIObjects[gunInfo.magazineCapacity-1].sprite != gunInfo.bulletUISprite)
                for (int i = 0; i < gunInfo.magazineCapacity; i++)
                    playerGunCmp.bulletUIObjects[i].sprite = gunInfo.bulletUISprite;

            //if (playerGunCmp.bulletUIObjects[gunInfo.magazineCapacity-1].rectTransform.sizeDelta != gunInfo.bulletUISize)
                for (int i = 0; i < gunInfo.magazineCapacity; i++)
                    playerGunCmp.bulletUIObjects[i].rectTransform.sizeDelta = gunInfo.bulletUISize;

            for (int i = 0; i < gunInInvCmp.currentAmmo.Count; i++)
                playerGunCmp.bulletUIObjects[i].color = _sceneData.Value.idItemslist.items[gunInInvCmp.currentAmmo[i]].bulletInfo.uiBulletColor;
        }
        else
        {
            _sceneData.Value.dropedItemsUIView.gunMagazineUI.gameObject.SetActive(false);
            for (int i = 0; i < playerGunCmp.bulletUIObjects.Count; i++)
            {
                var curBulletUI = playerGunCmp.bulletUIObjects[i];
                if (!playerGunCmp.bulletUIObjects[i].gameObject.activeInHierarchy) break;
                curBulletUI.gameObject.SetActive(false);
            }
        }
        curAttackCmp.damage = gunInfo.damage + Mathf.CeilToInt(_playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).weaponsExp[itemInfo.itemId].weaponExpLevel * 0.02f * gunInfo.damage);
        if (!gunInfo.isOneHandedGun && weaponLevel < 8 && !_inventoryCellComponentsPool.Value.Get(_sceneData.Value.shieldCellView._entity).isEmpty && playerView.movementView.shieldView.shieldObject.localPosition != Vector3.zero)//level 0 for test
        {
            playerView.movementView.shieldView.shieldObject.SetParent(playerView.movementView.shieldView.shieldContainer);
            playerView.movementView.shieldView.shieldObject.localPosition = Vector3.zero;
            playerView.movementView.shieldView.shieldObject.localRotation = Quaternion.Euler(0, 180, 0);
            playerView.movementView.shieldView.shieldSpriteRenderer.sortingOrder = 2;
        }

        if(gunInInvCmp.currentAmmo.Count != 0)
        gunCmp.currentBulletInfo = _sceneData.Value.idItemslist.items[gunInInvCmp.currentAmmo[0]].bulletInfo;

        curAttackCmp.weaponRotateSpeed = 10f / gunInInvCmp.currentGunWeight;
        //Debug.Log(curAttackCmp.damage);
    }

    private void BulletShellSpawn(Sprite needBulletShell, Vector2 spawnBulletShellPosition, float zRotation, bool isLeft)
    {
        ref var bulletShellCmp = ref _bulletShellComponentsPool.Value.Add(_world.Value.NewEntity());
        bulletShellCmp.bulletShellPrefab = _sceneData.Value.GetBulletShell();
        bulletShellCmp.bulletShellPrefab.transform.localRotation = Quaternion.Euler(0, 0, zRotation);
        bulletShellCmp.bulletShellPrefab.transform.position = spawnBulletShellPosition;
        bulletShellCmp.bulletShellPrefab.sprite = needBulletShell;
        bulletShellCmp.isLeft = isLeft;
        bulletShellCmp.horizontalSpeed = Random.Range(5f, 8f);
        bulletShellCmp.currentBulletShellSpeed = Random.Range(0.3f, 1.8f);
        //  Debug.Log("spawn bulletShell");
    }
    private void Shoot(int currentEntity, LayerMask targetLayer)// 6 маска игрока 7 враг
    {
        // Debug.Log("Shot");
        ref var attackCmp = ref _attackComponentsPool.Value.Get(currentEntity);
        ref var gunCmp = ref _gunComponentsPool.Value.Get(currentEntity);
        float gunSpritePositionRecoil = (gunCmp.currentAddedSpread / gunCmp.currentBulletInfo.bulletCount) * (0.1f - _movementComponentsPool.Value.Get(currentEntity).movementView.weaponSprite.transform.localPosition.y) * -0.08f;
        //   Debug.Log("gunSpritePositionRecoil" + gunSpritePositionRecoil);
        gunCmp.gunSpritePositionRecoil -= gunSpritePositionRecoil;
        bool spawnVisualEffects = !_hidedObjectOutsideFOVComponentsPool.Value.Has(currentEntity) || (_hidedObjectOutsideFOVComponentsPool.Value.Has(currentEntity) && _hidedObjectOutsideFOVComponentsPool.Value.Get(currentEntity).timeBeforeHide > 0);

        if (gunCmp.timeFromLastShot <= 0 && spawnVisualEffects)
            gunCmp.timeFromLastShot = 0.2f;

        gunCmp.firePoint.rotation = gunCmp.weaponContainer.rotation * Quaternion.Euler(0, 0, Random.Range(-gunCmp.currentSpread, gunCmp.currentSpread));

        LayerMask needMask = targetLayer | LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Shield") | LayerMask.GetMask("BrokedObject");
        if (targetLayer == LayerMask.GetMask("Enemy"))
            needMask = needMask | LayerMask.GetMask("EnemyHead");
        var targets = Physics2D.RaycastAll(gunCmp.firePoint.position, gunCmp.firePoint.up, gunCmp.attackLeght * gunCmp.currentBulletInfo.addedLenghtMultiplayer, needMask);


        if (targets.Length == 0)
        {
            //  Debug.Log("promax");
            if (spawnVisualEffects)
            {
                var tracer = CreateTracer();
                tracer.SetPosition(0, gunCmp.firePoint.position);

                var ray = new Ray2D(gunCmp.firePoint.position, gunCmp.firePoint.up);

                tracer.SetPosition(1, ray.origin + (ray.direction * gunCmp.attackLeght * gunCmp.currentBulletInfo.addedLenghtMultiplayer));
            }
            return;
        }

        else
        {
            int damageReminder = Mathf.RoundToInt(attackCmp.damage * gunCmp.currentBulletInfo.addedDamageMultiplayer);


            foreach (var target in targets)
            {
                if (target.collider.gameObject.layer == 13)//если щит
                {
                    var hpEntity = target.collider.gameObject.GetComponent<ShieldView>()._entity;
                    if (hpEntity == _sceneData.Value.shieldCellView._entity)
                    {
                        int shieldEntity = _sceneData.Value.shieldCellView._entity;
                        ref var shieldCmp = ref _shieldComponentsPool.Value.Get(shieldEntity);
                        ref var shieldItemCmp = ref _inventoryItemComponentsPool.Value.Get(shieldEntity);
                        int curDamageToShield = Mathf.CeilToInt(damageReminder * shieldItemCmp.itemInfo.sheildInfo.damagePercent);
                        if (shieldCmp.currentDurability - curDamageToShield > 0)
                        {
                            shieldCmp.currentDurability -= curDamageToShield;
                            damageReminder -= curDamageToShield;
                            Debug.Log("SheildHP" + shieldCmp.currentDurability);
                        }
                        else
                        {
                            damageReminder -= (int)shieldCmp.currentDurability;
                            shieldCmp.currentDurability = 0;
                            Debug.Log("SheildIsBroken");
                        }
                        var particles = CreateParticles(1);//сделать партиклы искр от металла
                        particles.gameObject.transform.position = target.point;

                        if (shieldCmp.currentDurability <= 0)
                        {
                            _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view.movementView.shieldView.shieldCollider.enabled = false;
                        }
                        continue;
                    }
                }
                else if (target.collider.gameObject.layer != 10)//если игрок
                {
                    var hpEntity = 0;
                    bool isHeadshot = target.collider.tag == "Head";
                    if (isHeadshot)
                    {
                        hpEntity = target.collider.gameObject.GetComponent<HeadColliderView>()._entity;
                        damageReminder *= 2;
                    }
                    else
                        hpEntity = target.collider.gameObject.GetComponent<HealthView>()._entity;
                    ref var health = ref _healthComponentsPool.Value.Get(hpEntity);
                    int startedHealth = health.healthPoint;

                    if (target.collider.gameObject.layer == 7)//если врага пуля касается
                    {
                        ref var aiCmp = ref _creatureAIComponentsPool.Value.Get(hpEntity);
                        if (aiCmp.targets == null)
                        {
                            var playerCollider = _movementComponentsPool.Value.Get(currentEntity).movementView.gameObject;
                            ref var moveCmp = ref _movementComponentsPool.Value.Get(hpEntity);
                            Vector2 direction = (playerCollider.transform.position - moveCmp.entityTransform.position).normalized;
                            RaycastHit2D hitSightOnTarget = Physics2D.Raycast(moveCmp.entityTransform.position, aiCmp.creatureView.movementView.weaponContainer.up, aiCmp.followDistance, LayerMask.GetMask("Obstacle", "Player"/*, "Enemy"*/));
                            aiCmp.sightOnTarget = hitSightOnTarget.collider != null && hitSightOnTarget.collider.gameObject.layer == 6;
                            float distanceToTarget = Vector2.Distance(playerCollider.transform.position, moveCmp.entityTransform.position);
                            if (aiCmp.colliders == null)
                            {
                                //  Debug.Log("Сообщить о игроке");
                                Collider2D[] closestEnemies = Physics2D.OverlapCircleAll(moveCmp.entityTransform.position, /*расстояние нахождения препятствий*/10f, LayerMask.GetMask("Enemy"));
                                foreach (var enemy in closestEnemies)
                                {
                                    int enemyEntity = enemy.gameObject.GetComponent<HealthView>()._entity;
                                    if (enemyEntity != hpEntity)
                                    {
                                        ref var teammateAiCmp = ref _creatureAIComponentsPool.Value.Get(enemyEntity);
                                        if (teammateAiCmp.targets == null)
                                        {
                                            var dir = (playerCollider.transform.position - enemy.transform.position).normalized;
                                            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                                            if (n < 0)
                                                n += 360;

                                            float angleRad = n * (Mathf.PI / 180f);

                                            RaycastHit2D hitOnTarget = Physics2D.Raycast(enemy.transform.position, new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)), teammateAiCmp.followDistance * 8, LayerMask.GetMask("Obstacle", "Player"/*, "Enemy"*/));

                                            if (hitOnTarget.collider != null && hitOnTarget.collider.gameObject.layer == 6)
                                            {
                                                teammateAiCmp.targets = new List<Transform>() { playerCollider.transform };
                                                teammateAiCmp.targetPositionCached = playerCollider.transform.position;
                                                teammateAiCmp.reachedLastTarget = false;
                                                if (teammateAiCmp.currentState == CreatureAIComponent.CreatureStates.idle)
                                                {
                                                    teammateAiCmp.timeFromLastTargetSeen = 0f;
                                                    teammateAiCmp.currentState = CreatureAIComponent.CreatureStates.follow;
                                                }
                                                //  Debug.Log("where they shoot?");
                                            }
                                        }
                                    }
                                }
                                //
                            }

                            aiCmp.colliders = new List<Transform>() { playerCollider.transform };

                            aiCmp.targetPositionCached = playerCollider.transform.position;
                            aiCmp.reachedLastTarget = false;
                            if (aiCmp.currentState == CreatureAIComponent.CreatureStates.idle)
                            {
                                aiCmp.timeFromLastTargetSeen = 0f;
                                aiCmp.currentState = CreatureAIComponent.CreatureStates.follow;
                            }
                        }
                    }
                    //  else // если игрок
                    // {
                    
                    // }
                    if (target.collider.gameObject.layer != 17 && ((float)damageReminder * gunCmp.currentBulletInfo.addedBleedingMultiplayer) / 4 > Random.Range(0, 100))
                    {
                        float effectTime = damageReminder / 3 * gunCmp.currentBulletInfo.addedBleedingMultiplayer;
                        if (target.collider.gameObject.layer != 7 && _inventoryItemComponentsPool.Value.Has(_sceneData.Value.bodyArmorCellView._entity))
                            effectTime *= 1 - _inventoryItemComponentsPool.Value.Get(_sceneData.Value.bodyArmorCellView._entity).itemInfo.bodyArmorInfo.bleedingResistance;
                        else if (target.collider.gameObject.layer == 7 && _creatureAIComponentsPool.Value.Get(hpEntity).armorInfo != null)
                            effectTime *= 1 - _creatureAIComponentsPool.Value.Get(hpEntity).armorInfo.bleedingResistance;
                        ref var effCmp = ref _effectComponentsPool.Value.Add(_world.Value.NewEntity());
                        effCmp.effectEntity = hpEntity;
                        effCmp.effectLevel = 1;
                        effCmp.effectType = EffectInfo.EffectType.bleeding;
                        effCmp.isFirstEffectCheck = true;
                        if (target.collider.gameObject.layer != 7)
                            effCmp.effectIconSprite = _sceneData.Value.bloodEffectsSprites[0];
                        effCmp.effectDuration = effectTime;

                    }

                    if (!health.healthView.isDeath)
                        _changeHealthEventsPool.Value.Add(_world.Value.NewEntity()).SetParametrs(damageReminder, hpEntity, isHeadshot, gunCmp.currentBulletInfo.addedStunMultiplayer);
                    //  Debug.Log(damageReminder + "dmg");
                    if (health.healthPoint <= 0)
                    {
                        damageReminder -= startedHealth;
                        return;
                    }

                    else
                    {
                        var tracer = CreateTracer();
                        tracer.SetPosition(0, gunCmp.firePoint.position);
                        tracer.SetPosition(1, target.point);

                        var particles = target.collider.gameObject.layer != 17 ? CreateParticles(0) : CreateParticles(1);
                        particles.gameObject.transform.position = target.point;
                        return;
                    }
                }
                else
                {
                    if (spawnVisualEffects)
                    {
                        var tracer = CreateTracer();
                        tracer.SetPosition(0, gunCmp.firePoint.position);
                        tracer.SetPosition(1, target.point);
                    }
                    return;
                }
            }

            if (spawnVisualEffects)
            {
                var tracer2 = CreateTracer();
                var ray2 = new Ray2D(gunCmp.firePoint.position, gunCmp.firePoint.up);

                tracer2.SetPosition(0, gunCmp.firePoint.position);
                tracer2.SetPosition(1, ray2.origin + (ray2.direction * gunCmp.attackLeght));
            }
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

    private ParticleSystem CreateParticles(int needType)//0 blood, 1 sparkle
    {
        var particleEntity = _world.Value.NewEntity();

        ref var lifetimeCmp = ref _paricleLifetimeComponentsPool.Value.Add(particleEntity);
        lifetimeCmp.lifeTime = 2f;
        if (needType == 0)
            lifetimeCmp.objectToHide = _sceneData.Value.GetParticlePrefab(_sceneData.Value.bloodParticleInfo);
        else
            lifetimeCmp.objectToHide = _sceneData.Value.GetParticlePrefab(_sceneData.Value.sparcleParticleInfo);

        return lifetimeCmp.objectToHide;
    }

    private void CheckLifetimeObjectsTime()
    {
        foreach (var bulletTracerEntity in _bulletTracerLifetimeComponentFilter.Value)
        {
            ref var lifetimeCmp = ref _bulletTracerLifetimeComponentsPool.Value.Get(bulletTracerEntity);
            Color tracerColor = new Color(255, 255, 255, lifetimeCmp.lifetime / 6);
            lifetimeCmp.lineRenderer.startColor = tracerColor;
            lifetimeCmp.lineRenderer.endColor = tracerColor;
            lifetimeCmp.lifetime -= Time.deltaTime;
            if (lifetimeCmp.lifetime > 0)
                continue;
            _sceneData.Value.ReleaseBulletTracer(lifetimeCmp.lineRenderer);

            _world.Value.DelEntity(bulletTracerEntity);
        }

        foreach (var particleEntity in _paricleLifetimeComponentsFilter.Value)
        {
            ref var lifetimeCmp = ref _paricleLifetimeComponentsPool.Value.Get(particleEntity);

            lifetimeCmp.lifeTime -= Time.deltaTime;
            if (lifetimeCmp.lifeTime > 0)
                continue;
            _sceneData.Value.ReleaseParticlePrefab(lifetimeCmp.objectToHide);

            _world.Value.DelEntity(particleEntity);
        }
    }

}
