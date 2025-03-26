using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class MovementSystem : IEcsRunSystem
{
    //private EcsCustomInject<SceneService> _sceneData;
    private EcsPoolInject<MovementComponent> _movementComponentPool;
    private EcsPoolInject<MeleeWeaponComponent> _meleeWeaponComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<InventoryComponent> _inventoryComponentsPool;
    private EcsPoolInject<PlayerMoveComponent> _playerMoveComponentsPool;
    private EcsPoolInject<PlayerUpgradedStats> _playerUpgradedStatsPool;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentsPool;
    private EcsPoolInject<FadedParticleOnScreenComponent> _fadedParticleOnScreenComponentsPool;
    private EcsPoolInject<BuildingCheckerComponent> _buildingCheckerComponentsPool;
    private EcsPoolInject<AttackComponent> _attackComponentsPool;

    private EcsFilterInject<Inc<MovementComponent>> _movementComponentFilter;
    private EcsFilterInject<Inc<FadedParticleOnScreenComponent>> _fadedParticleOnScreenComponentsFilter;

    private EcsCustomInject<SceneService> _sceneService;
    private EcsWorldInject _world;

    public void Run(IEcsSystems systems)
    {
        ref var playerCmp = ref _playerComponentsPool.Value.Get(_sceneService.Value.playerEntity);
        foreach (var screenParticle in _fadedParticleOnScreenComponentsFilter.Value)
        {
            ref var screenParticleCmp = ref _fadedParticleOnScreenComponentsPool.Value.Get(screenParticle);
            screenParticleCmp.particleImage.color -= new Color(0, 0, 0, 0.1f * Time.deltaTime);
            if (screenParticleCmp.particleImage.color.a <= 0)
            {
                _sceneService.Value.ReleaseParticleOnScreen(screenParticleCmp.particleImage);
                _fadedParticleOnScreenComponentsPool.Value.Del(screenParticle);
            }
        }
        foreach (var movableObject in _movementComponentFilter.Value)
        {
            bool isPlayer = _playerComponentsPool.Value.Has(movableObject);
            //if (!_movementComponentPool.Value.Has(movableObject)) continue;
            ref var moveCmp = ref _movementComponentPool.Value.Get(movableObject);

            if (moveCmp.speedMultiplayer > 1)
                moveCmp.speedMultiplayer = 1;
            else if (moveCmp.speedMultiplayer > 0)
                moveCmp.speedMultiplayer -= Time.deltaTime * 0.1f;
            else if (moveCmp.speedMultiplayer < 0)
                moveCmp.speedMultiplayer = 0;

            if (moveCmp.isStunned)
            {
                moveCmp.stunTime -= Time.deltaTime;
                if (moveCmp.stunTime <= 0)
                {
                    moveCmp.stunTime = 0;
                    moveCmp.isStunned = false;
                    if (!isPlayer)
                        moveCmp.moveSpeed = moveCmp.movementView.moveSpeed;//временно и для игрока другое будет уравнение
                    else
                    {
                        ref var inventoryCmp = ref _inventoryComponentsPool.Value.Get(_sceneService.Value.inventoryEntity);
                        moveCmp.moveSpeed = moveCmp.movementView.moveSpeed + moveCmp.movementView.moveSpeed / 50 * _playerUpgradedStatsPool.Value.Get(_sceneService.Value.playerEntity).statLevels[0];
                        if (inventoryCmp.weight / inventoryCmp.currentMaxWeight > 0.6f)
                            moveCmp.moveSpeed -= (moveCmp.movementView.moveSpeed * ((inventoryCmp.weight / inventoryCmp.currentMaxWeight) - 0.6f) * 2);
                        if (moveCmp.isRun)
                            moveCmp.isRun = false;
                    }
                    moveCmp.moveInput = Vector2.zero;
                }
            }

            if (moveCmp.canMove && !moveCmp.isTrapped)
            {
                if (moveCmp.moveInput != Vector2.zero)
                {
                    if (isPlayer)
                    {
                        ref var invCmp = ref _inventoryComponentsPool.Value.Get(_sceneService.Value.inventoryEntity);
                        if (invCmp.currentMaxWeight / 2f < invCmp.weight)
                        {
                            ref var playerStats = ref _playerUpgradedStatsPool.Value.Get(_sceneService.Value.playerEntity);
                            playerStats.currentStatsExp[0] += Time.fixedDeltaTime * 0.5f;
                            if (playerStats.currentStatsExp[0] >= _sceneService.Value.levelExpCounts[playerStats.statLevels[0]])//0 потому что стат веса
                            {
                                playerStats.statLevels[0]++;
                                invCmp.currentMaxWeight = _sceneService.Value.maxInInventoryWeight + playerStats.statLevels[0] * _sceneService.Value.maxInInventoryWeight / 50f;
                                Debug.Log(invCmp.currentMaxWeight + " up max weight");
                            }
                        }
                    }
                    if (moveCmp.isRun)
                    {
                        moveCmp.movementView.MoveUnit(moveCmp.moveSpeed * moveCmp.moveInput * Time.deltaTime * moveCmp.movementView.runSpeedMultiplayer * (1 - moveCmp.speedMultiplayer));
                        if (!isPlayer)
                            moveCmp.currentRunTime -= Time.deltaTime;
                    }
                    else
                        moveCmp.movementView.MoveUnit(moveCmp.moveSpeed * moveCmp.moveInput * Time.deltaTime * (1 - moveCmp.speedMultiplayer));
                    moveCmp.timeFromLastStep -= Time.deltaTime;
                    if (moveCmp.timeFromLastStep <= 0)
                    {
                        if (isPlayer && moveCmp.isRun && !_buildingCheckerComponentsPool.Value.Get(_sceneService.Value.playerEntity).isHideRoof)
                        {
                            moveCmp.timeFromLastStep = 0.4f;

                            float alpfaMultiplayer = 1;

                            if (_inventoryItemComponentsPool.Value.Has(_sceneService.Value.helmetCellView._entity))
                                alpfaMultiplayer = 1 - _inventoryItemComponentsPool.Value.Get(_sceneService.Value.helmetCellView._entity).itemInfo.helmetInfo.dropTransparentMultiplayer;

                            _fadedParticleOnScreenComponentsPool.Value.Add(_world.Value.NewEntity()).particleImage = _sceneService.Value.GetParticleOnScreen(_sceneService.Value.grassParticleOnScreenColor, alpfaMultiplayer, false);
                        }
                        else if(!isPlayer && moveCmp.isRun)
                            moveCmp.timeFromLastStep = 0.4f;
                        else
                            moveCmp.timeFromLastStep = 0.8f;
                        if (isPlayer)
                            moveCmp.movementView.weaponAudioSource.PlayOneShot(_sceneService.Value.stepsOnGrassSounds[Random.Range(0, _sceneService.Value.stepsOnGrassSounds.Length)]);
                        else
                        {
                            var playerPosition = _movementComponentPool.Value.Get(_sceneService.Value.playerEntity).entityTransform.position;
                            float distanceToPlayer = Vector2.Distance(moveCmp.entityTransform.position, (Vector2)playerPosition);
                            if (distanceToPlayer <= playerCmp.currentAudibility * 10)
                            {
                                Vector2 directionToPlayer = (_movementComponentPool.Value.Get(_sceneService.Value.playerEntity).entityTransform.position - moveCmp.entityTransform.position).normalized;
                                RaycastHit2D hit = Physics2D.Raycast(moveCmp.entityTransform.position, directionToPlayer, 20f, LayerMask.GetMask("Obstacle", "Player"));

                                if (hit.collider != null && (LayerMask.GetMask("Player") & (1 << hit.collider.gameObject.layer)) != 0)
                                    moveCmp.movementView.weaponAudioSource.volume = playerCmp.currentAudibility * 10f / distanceToPlayer;
                                else
                                    moveCmp.movementView.weaponAudioSource.volume = playerCmp.currentAudibility * 10f / distanceToPlayer * 0.7f;

                                moveCmp.movementView.weaponAudioSource.panStereo = (moveCmp.entityTransform.position.x - playerPosition.x) / 10f;
                                moveCmp.movementView.weaponAudioSource.PlayOneShot(_sceneService.Value.stepsOnGrassSounds[Random.Range(0, _sceneService.Value.stepsOnGrassSounds.Length)]);
                            }
                        }

                    }
                }
                if (!isPlayer && !moveCmp.isRun && moveCmp.currentRunTime < moveCmp.maxRunTime)
                  moveCmp.currentRunTime += Time.deltaTime * moveCmp.currentRunTimeRecoverySpeed;
            }

            if (_meleeWeaponComponentsPool.Value.Has(movableObject) && _meleeWeaponComponentsPool.Value.Get(movableObject).isHitting) continue; //возможно что то поправить


            Vector2 direction = (moveCmp.pointToRotateInput - (Vector2)moveCmp.entityTransform.position).normalized;
            float rotateZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion lookRotation = Quaternion.Euler(0f, 0f, rotateZ + moveCmp.movementView.offsetToWeapon);
            Quaternion needRotation = Quaternion.Slerp(moveCmp.movementView.weaponContainer.rotation, lookRotation, Time.deltaTime * _attackComponentsPool.Value.Get(movableObject).weaponRotateSpeed * (1 - moveCmp.speedMultiplayer)/*moveCmp.movementView.weaponRotateSpeed*/);

            moveCmp.movementView.RotateWeaponCentre(needRotation);

            float rotY = moveCmp.movementView.characterSpriteTransform.rotation.y;


           if (moveCmp.movementView.weaponSpriteRenderer.transform.localRotation.y == 0 && rotateZ > -85 && rotateZ < 85)
                  moveCmp.movementView.weaponSpriteRenderer.transform.localRotation = Quaternion.Euler(0, -180, moveCmp.movementView.weaponSpriteRenderer.transform.localEulerAngles.z);
              else if (moveCmp.movementView.weaponSpriteRenderer.transform.localRotation.y != 0 && (rotateZ < -95 || rotateZ > 95))
                  moveCmp.movementView.weaponSpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, moveCmp.movementView.weaponSpriteRenderer.transform.localEulerAngles.z);
           // var weaponTransform = moveCmp.movementView.weaponSpriteRenderer.transform;
           // if ((weaponTransform.localScale.x < 0 && rotateZ > -85 && rotateZ < 85)||(weaponTransform.localScale.x > 0 && (rotateZ < -95 || rotateZ > 95)))
           //     weaponTransform.localScale = new Vector2 (-weaponTransform.localScale.x, weaponTransform.localScale.y);

            if (_meleeWeaponComponentsPool.Value.Has(movableObject) && !_meleeWeaponComponentsPool.Value.Get(movableObject).isHitting && moveCmp.movementView.nonWeaponContainer != null)
            {
                if (moveCmp.movementView.nonWeaponContainer.localScale.x > 0 && rotateZ > -85 && rotateZ < 85)
                    moveCmp.movementView.nonWeaponContainer.localScale = new Vector2(-1, 1);
                else if (moveCmp.movementView.nonWeaponContainer.localScale.x < 0 && (rotateZ < -95 || rotateZ > 95))
                    moveCmp.movementView.nonWeaponContainer.localScale = new Vector2(1, 1);

                moveCmp.movementView.RotateNonWeaponCentre(needRotation);
            }
             if (direction.x < 0 && rotateZ < -95 || rotateZ > 95)
                 moveCmp.movementView.characterSpriteTransform.localRotation = Quaternion.Euler(0, -180, 0);
             else if (direction.x > 0 && rotateZ > -85 && rotateZ < 85)
                 moveCmp.movementView.characterSpriteTransform.localRotation = Quaternion.Euler(0, 0, 0);

          //  if ((direction.x < 0 && moveCmp.movementView.characterSpriteTransform.localScale.x > 0) ||(direction.x > 0 && moveCmp.movementView.characterSpriteTransform.localScale.x < 0) )
           //     moveCmp.movementView.characterSpriteTransform.localScale = new Vector2(-moveCmp.movementView.characterSpriteTransform.localScale.x, moveCmp.movementView.characterSpriteTransform.localScale.y);
        }
    }

}

