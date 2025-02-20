using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class HealthSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<ChangeHealthEvent> _changeHealthEventsPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<HealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<DroppedItemComponent> _droppedItemComponentsPool;
    private EcsPoolInject<AttackComponent> _attackComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<CreatureInventoryComponent> _creatureInventoryComponentsPool;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<BloodParticleOnScreenComponent> _bloodParticleOnScreenComponentsPool;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentsPool;
    private EcsPoolInject<DeathEvent> _deathEventsPool;//пока только игрок его юзает, потом можно и живности передать 
    private EcsPoolInject<EntryInNewLocationEvent> _entryInNewLocationEventsPool;
    private EcsPoolInject<CurrentLocationComponent> _currentLocationComponentsPool;
    private EcsPoolInject<EffectComponent> _effectComponentsPool;
    private EcsPoolInject<EffectParticleLifetimeTag> _effectParticleLifetimeTagsPool;
    private EcsPoolInject<ParicleLifetimeComponent> _particleLifetimeComponentsPool;
    private EcsPoolInject<DurabilityInInventoryComponent> _durabilityInInventoryComponentsPool;
    private EcsPoolInject<CameraComponent> _cameraComponentsPool;
    private EcsPoolInject<FadedParticleOnScreenComponent> _fadedParticleOnScreenComponentsPool;
    private EcsPoolInject<OffInScopeStateEvent> _offInScopeStateEventsPool;
    private EcsPoolInject<PlayerMoveComponent> _playerMoveComponentsPool;
    private EcsPoolInject<BuildingCheckerComponent> _buildingCheckerComponentsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<PlayerUpgradedStats> _playerUpgradedStatsPool;
    private EcsPoolInject<UpgradePlayerStatEvent> _upgradePlayerStatEventsPool;
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<MenuStatesComponent> _menuStatesComponentsPool;
    private EcsPoolInject<DestroyComponentInNextFrameTag> _destroyComponentInNextFrameTagsPool;
    private EcsPoolInject<HidedObjectOutsideFOVComponent> _hidedObjectOutsideFOVComponentsPool;

    private EcsFilterInject<Inc<DestroyComponentInNextFrameTag>> _destroyComponentInNextFrameTagsFilter;
    private EcsFilterInject<Inc<ChangeHealthEvent>> _changeHealthEventsFilter;
    private EcsFilterInject<Inc<EffectComponent>> _effectComponentsFilter;
    private EcsFilterInject<Inc<HealthComponent, PlayerComponent>> _playerHealthFilter;
    private EcsFilterInject<Inc<HealingItemComponent>> _currentHealingItemComponentsFilter;
    private EcsFilterInject<Inc<ParicleLifetimeComponent>> _paricleLifetimeComponentsFilter;
    private EcsFilterInject<Inc<CreatureAIComponent>> _creatureAIComponentsFilter;
    private EcsFilterInject<Inc<RevivePlayerEvent>> _revivePlayerEventsFilter;
    private EcsFilterInject<Inc<DeathEvent>, Exc<PlayerComponent>> _enemiesDeathEventsFilter;
    private EcsFilterInject<Inc<BloodParticleOnScreenComponent>> _bloodParticleOnScreenComponentsFilter;
    public void Init(IEcsSystems systems)
    {
        ref var healthCmp = ref _healthComponentsPool.Value.Get(_sceneData.Value.playerEntity);

        ChangeHealthBarInfo(healthCmp);
    }
    public void Run(IEcsSystems systems)
    {
        foreach (var effectEntity in _effectComponentsFilter.Value)
        {
            ref var effectCmp = ref _effectComponentsPool.Value.Get(effectEntity);
            bool isPlayer = effectCmp.effectEntity == _sceneData.Value.playerEntity;

            if (effectCmp.isFirstEffectCheck)
            {
                effectCmp.isFirstEffectCheck = false;
                bool isAddEffect = true;
                bool isNewEffect = true;
                Debug.Log("try add effect entity" + effectEntity);
                //  int delEntity = 0;
                foreach (var checkedEffectEntity in _effectComponentsFilter.Value)
                {
                    if (checkedEffectEntity != effectEntity /*|| !_effectComponentsPool.Value.Has(checkedEffectEntity)*/)
                    {
                        Debug.Log("checked effect entity" + checkedEffectEntity);
                        ref var checkedEffectCmp = ref _effectComponentsPool.Value.Get(checkedEffectEntity);//
                        if (checkedEffectCmp.isFirstEffectCheck) continue;
                        if (checkedEffectCmp.effectType == effectCmp.effectType)
                        {
                            isAddEffect = false;
                            if (checkedEffectCmp.effectLevel < effectCmp.effectLevel)
                            {
                                isNewEffect = false;
                                if (isPlayer)
                                {
                                    effectCmp.effectIconView = checkedEffectCmp.effectIconView;
                                    effectCmp.effectIconView.effectIconImage.sprite = effectCmp.effectIconSprite;
                                }
                            }

                            if (!isNewEffect && checkedEffectCmp.effectDuration > effectCmp.effectDuration)
                            {
                                effectCmp.effectDuration += checkedEffectCmp.effectDuration;
                                if (isPlayer)
                                    effectCmp.effectIconView.effectTimerText.text = ((int)effectCmp.effectDuration).ToString();
                            }
                            else if (isNewEffect && checkedEffectCmp.effectDuration < effectCmp.effectDuration)
                            {
                                checkedEffectCmp.effectDuration += effectCmp.effectDuration;
                                if (isPlayer)
                                    checkedEffectCmp.effectIconView.effectTimerText.text = ((int)checkedEffectCmp.effectDuration).ToString();
                            }
                            if (isNewEffect)
                            {
                                // _world.Value.DelEntity(effectEntity);
                                _destroyComponentInNextFrameTagsPool.Value.Add(effectEntity);
                                Debug.Log("destroy effect entity" + effectEntity);

                            }
                            else
                            {
                                // delEntity = checkedEffectEntity;
                                //_world.Value.DelEntity(checkedEffectEntity);
                                _destroyComponentInNextFrameTagsPool.Value.Add(checkedEffectEntity);
                                Debug.Log("destroy effect entity" + checkedEffectEntity);
                            }
                            break;
                        }
                    }

                }
                //  if (delEntity != 0)
                //     _world.Value.DelEntity(delEntity);
                if (!isAddEffect)
                {
                    Debug.Log(isPlayer + "dont dd" + "is new "+isNewEffect);
                    if (isNewEffect)
                        effectCmp.effectDuration -= Time.deltaTime;
                    continue;
                }
                else
                {
                        Debug.Log(isPlayer + "ad effect icon");
                    if (isPlayer)
                    {
                        effectCmp.effectIconView = _sceneData.Value.GetEffectIconView();
                        effectCmp.effectIconView.effectIconImage.sprite = effectCmp.effectIconSprite;
                    }
                    if (effectCmp.effectType == EffectInfo.EffectType.mantrap)
                    {
                        var particleEntity = _world.Value.NewEntity();

                        ref var lifetimeCmp = ref _particleLifetimeComponentsPool.Value.Add(particleEntity);
                        lifetimeCmp.lifeTime = 2f;
                        lifetimeCmp.objectToHide = _sceneData.Value.GetParticlePrefab(_sceneData.Value.bloodParticleInfo);
                        lifetimeCmp.objectToHide.gameObject.transform.position = _movementComponentsPool.Value.Get(effectCmp.effectEntity).entityTransform.position;


                        _movementComponentsPool.Value.Get(effectCmp.effectEntity).isTrapped = true;
                    }
                    else if (effectCmp.effectType == EffectInfo.EffectType.bleeding)
                    {
                        ref var lifetimeCmp = ref _particleLifetimeComponentsPool.Value.Add(effectCmp.effectEntity);
                        _effectParticleLifetimeTagsPool.Value.Add(effectCmp.effectEntity);
                        lifetimeCmp.lifeTime = 1f;
                        lifetimeCmp.objectToHide = _sceneData.Value.GetParticlePrefab(_sceneData.Value.smallBloodParticleInfo);
                        var moveCmp = _movementComponentsPool.Value.Get(effectCmp.effectEntity);
                        lifetimeCmp.objectToHide.gameObject.transform.position = moveCmp.entityTransform.position;
                        lifetimeCmp.objectToHide.transform.SetParent(moveCmp.movementView.characterSpriteTransform);
                    }
                    else if (effectCmp.effectType == EffectInfo.EffectType.painkillers)
                    {
                        // ref var playerCmp = ref _playerComponentsPool.Value.Get(effectCmp.effectEntity);
                        _playerComponentsPool.Value.Get(effectCmp.effectEntity).levelOfPainkillers = effectCmp.effectLevel;
                    }
                    else if (effectCmp.effectType == EffectInfo.EffectType.cheerfulness)
                    {
                        //  _playerMoveComponentsPool.Value.Get(effectCmp.effectEntity).maxRunTime = _playerComponentsPool.Value.Get(effectCmp.effectEntity).view.runTime * (1 + _playerUpgradedStatsPool.Value.Get(effectCmp.effectEntity).statLevels[2] + (effectCmp.effectLevel * 0.3f));

                        var playerCmp = _playerComponentsPool.Value.Get(effectCmp.effectEntity);
                        ref var moveCmp = ref _movementComponentsPool.Value.Get(effectCmp.effectEntity);
                        moveCmp.maxRunTime = playerCmp.view.runTime * (1 + _playerUpgradedStatsPool.Value.Get(effectCmp.effectEntity).statLevels[2] + (effectCmp.effectLevel * 0.3f));
                        moveCmp.currentRunTimeRecoverySpeed = playerCmp.view.runTimeRecoverySpeed * (1 + (effectCmp.effectLevel * 0.3f));
                    }
                    if (isPlayer)
                        effectCmp.effectIconView.effectTimerText.text = ((int)effectCmp.effectDuration).ToString();
                    effectCmp.effectDuration -= Time.deltaTime;
                    continue;
                }
            }
            else if (effectCmp.effectDuration <= 0)
            {
                if (effectCmp.effectType == EffectInfo.EffectType.mantrap)
                {
                    _movementComponentsPool.Value.Get(effectCmp.effectEntity).isTrapped = false;
                    if (isPlayer)
                        _sceneData.Value.ammoInfoText.text = "";
                }
                else if (effectCmp.effectType == EffectInfo.EffectType.bleeding)
                {
                    //_effectParticleLifetimeTagsPool.Value.Del(effectCmp.effectEntity);
                    _effectParticleLifetimeTagsPool.Value.Del(effectCmp.effectEntity);
                    _sceneData.Value.ReleaseParticlePrefab(_particleLifetimeComponentsPool.Value.Get(effectCmp.effectEntity).objectToHide);
                    _particleLifetimeComponentsPool.Value.Del(effectCmp.effectEntity);
                }
                else if (effectCmp.effectType == EffectInfo.EffectType.painkillers)
                    _playerComponentsPool.Value.Get(effectCmp.effectEntity).levelOfPainkillers = 0;
                else if (effectCmp.effectType == EffectInfo.EffectType.cheerfulness)
                {
                    var playerCmp = _playerComponentsPool.Value.Get(effectCmp.effectEntity);
                    ref var moveCmp = ref _movementComponentsPool.Value.Get(effectCmp.effectEntity);
                    moveCmp.maxRunTime = playerCmp.view.runTime * (1 + _playerUpgradedStatsPool.Value.Get(effectCmp.effectEntity).statLevels[2]);
                    moveCmp.currentRunTimeRecoverySpeed = playerCmp.view.runTimeRecoverySpeed;
                    if (moveCmp.currentRunTime > moveCmp.maxRunTime)
                        moveCmp.currentRunTime = moveCmp.maxRunTime;
                }

                if (isPlayer)
                    _sceneData.Value.ReleaseEffecticonView(effectCmp.effectIconView);
                _world.Value.DelEntity(effectEntity);
                continue;
            }

            if (isPlayer)
                effectCmp.effectIconView.effectTimerText.text = ((int)effectCmp.effectDuration).ToString();

            if (effectCmp.effectType == EffectInfo.EffectType.bleeding)
            {
                ref var lifetimeCmp = ref _particleLifetimeComponentsPool.Value.Get(effectCmp.effectEntity);
                if (lifetimeCmp.lifeTime <= 0)//каждую секунду урон наносит
                {
                    lifetimeCmp.lifeTime++;
                    lifetimeCmp.objectToHide.Play();
                    ref var changeHealthEvt = ref _changeHealthEventsPool.Value.Add(_world.Value.NewEntity());
                    changeHealthEvt.changedEntity = effectCmp.effectEntity;
                    changeHealthEvt.changedHealth = effectCmp.effectLevel;
                }
                lifetimeCmp.lifeTime -= Time.deltaTime;
            }
            else if (effectCmp.effectType == EffectInfo.EffectType.mantrap)
            {
                if (isPlayer && !_gunComponentsPool.Value.Get(effectCmp.effectEntity).isReloading && !_currentHealingItemComponentsPool.Value.Get(effectCmp.effectEntity).isHealing)
                {
                    if (Input.GetKey(KeyCode.F))
                    {
                        effectCmp.effectDuration -= Time.deltaTime;
                        _sceneData.Value.ammoInfoText.text = "escape from a trap...";
                    }
                    else //if(_sceneData.Value.ammoInfoText.text != "")
                    {
                        _sceneData.Value.ammoInfoText.text = "hold F to free yourself";//мб где нибудь ещё текст для предупреждений сделать и туда это стаить
                    }
                }
                continue;
            }

            effectCmp.effectDuration -= Time.deltaTime;
        }
        foreach (var destroyEffect in _destroyComponentInNextFrameTagsFilter.Value)
        {
            _world.Value.DelEntity(destroyEffect);
        }
        foreach (var revivePlayer in _revivePlayerEventsFilter.Value)
        {
            ref var healthCmp = ref _healthComponentsPool.Value.Get(revivePlayer);
            ref var moveCmp = ref _movementComponentsPool.Value.Get(revivePlayer);

         //   Debug.Log(_inventoryItemComponentsPool.Value.Get(_sceneData.Value.bodyArmorCellView._entity).itemInfo);
            var bodyArmorItemInfo = _inventoryItemComponentsPool.Value.Get(_sceneData.Value.bodyArmorCellView._entity).itemInfo.bodyArmorInfo;
            moveCmp.movementView.bodyArmorSpriteRenderer.sprite = bodyArmorItemInfo.bodyArmorSprite;
            moveCmp.movementView.bodyArmorSpriteRenderer.transform.localPosition = bodyArmorItemInfo.inGamePositionOnPlayer;

            _cameraComponentsPool.Value.Get(revivePlayer).blurValue = 1;
            _sceneData.Value.depthOfFieldMainBg.focalLength.value = 1;
            _sceneData.Value.dropedItemsUIView.scopeCrossCentreImage.gameObject.SetActive(false);

            // _offInScopeStateEventsPool.Value.Add(revivePlayer);
            moveCmp.speedMultiplayer = 1;
            moveCmp.currentRunTime = 0;
            moveCmp.movementView.objectTransform.gameObject.SetActive(true);
            moveCmp.movementView.characterAnimator.SetBool("isDeath", false);
            _currentHealingItemComponentsPool.Value.Get(revivePlayer).isHealing = false;
            _gunComponentsPool.Value.Get(revivePlayer).isReloading = false;
            _attackComponentsPool.Value.Get(revivePlayer).weaponIsChanged = false;
            Time.timeScale = 1;

            foreach (var effectEntity in _effectComponentsFilter.Value)
            {
                ref var effectCmp = ref _effectComponentsPool.Value.Get(effectEntity);
                if (effectCmp.effectType == EffectInfo.EffectType.mantrap)
                {
                    _movementComponentsPool.Value.Get(effectCmp.effectEntity).isTrapped = false;
                    _sceneData.Value.ammoInfoText.text = "";
                }
                else if (effectCmp.effectType == EffectInfo.EffectType.bleeding)
                {
                    _effectParticleLifetimeTagsPool.Value.Del(effectCmp.effectEntity);
                    _sceneData.Value.ReleaseParticlePrefab(_particleLifetimeComponentsPool.Value.Get(effectCmp.effectEntity).objectToHide);
                    _particleLifetimeComponentsPool.Value.Del(effectCmp.effectEntity);
                }
                else if (effectCmp.effectType == EffectInfo.EffectType.painkillers)
                    _playerComponentsPool.Value.Get(effectCmp.effectEntity).levelOfPainkillers = 0;
                else if (effectCmp.effectType == EffectInfo.EffectType.cheerfulness)
                {
                    var playerCmp = _playerComponentsPool.Value.Get(revivePlayer);
                   // ref var moveCmp = ref _movementComponentsPool.Value.Get(effectCmp.effectEntity);
                    moveCmp.maxRunTime = playerCmp.view.runTime * (1 + _playerUpgradedStatsPool.Value.Get(effectCmp.effectEntity).statLevels[2]);
                    if (moveCmp.currentRunTime > moveCmp.maxRunTime)
                        moveCmp.currentRunTime = moveCmp.maxRunTime;
                    moveCmp.currentRunTimeRecoverySpeed = playerCmp.view.runTimeRecoverySpeed;
                }

                if (effectCmp.effectEntity == _sceneData.Value.playerEntity)
                    _sceneData.Value.ReleaseEffecticonView(effectCmp.effectIconView);
                _effectComponentsPool.Value.Del(effectEntity);

            }
            foreach (var particleEntity in _paricleLifetimeComponentsFilter.Value)
            {
                _sceneData.Value.ReleaseParticlePrefab(_particleLifetimeComponentsPool.Value.Get(particleEntity).objectToHide);

                _world.Value.DelEntity(particleEntity);
            }

            ref var curLocationCmp = ref _currentLocationComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (curLocationCmp.levelNum != 0)
            {
                curLocationCmp.levelNum = curLocationCmp.currentLocation.levels.Length;
                _entryInNewLocationEventsPool.Value.Add(_sceneData.Value.playerEntity);
            }
            else
                moveCmp.movementView.gameObject.transform.position = Vector2.zero;

            foreach (var screenParticle in _bloodParticleOnScreenComponentsFilter.Value)
            {
                _sceneData.Value.ReleaseParticleOnScreen(_bloodParticleOnScreenComponentsPool.Value.Get(screenParticle).bloodParticleImage);
                _bloodParticleOnScreenComponentsPool.Value.Del(screenParticle);
            }
            //удаление текущей арены на которой был игрок

            healthCmp.isDeath = false;
            ChangeHealthBarInfo(healthCmp);
        }
        foreach (var curHealingItem in _currentHealingItemComponentsFilter.Value)
        {
            bool isPlayer = _sceneData.Value.playerEntity == curHealingItem;
            ref var curHealthCmp = ref _currentHealingItemComponentsPool.Value.Get(curHealingItem);
            if (curHealthCmp.isHealing)
            {
                curHealthCmp.currentHealingTime += Time.deltaTime;
                if (isPlayer)
                    foreach (var screenParticle in _bloodParticleOnScreenComponentsFilter.Value)
                    {
                        ref var particleCmp = ref _bloodParticleOnScreenComponentsPool.Value.Get(screenParticle);
                        particleCmp.bloodParticleImage.color -= new Color(0, 0, 0, 0.1f * Time.deltaTime);
                        if (particleCmp.bloodParticleImage.color.a <= 0)
                        {
                            _sceneData.Value.ReleaseParticleOnScreen(particleCmp.bloodParticleImage);
                            _bloodParticleOnScreenComponentsPool.Value.Del(screenParticle);
                        }
                    }
                //менять скорость возможно
                if (curHealthCmp.currentHealingTime >= curHealthCmp.healingItemInfo.healingTime)
                {
                    curHealthCmp.currentHealingTime = 0;
                    curHealthCmp.isHealing = false;
                    if (!isPlayer || (isPlayer && (!_menuStatesComponentsPool.Value.Get(curHealingItem).inInventoryState && !_menuStatesComponentsPool.Value.Get(curHealingItem).inMainMenuState)))
                        _attackComponentsPool.Value.Get(curHealingItem).canAttack = true;
                    foreach (var effect in _effectComponentsFilter.Value)
                    {
                        ref var effectCmp = ref _effectComponentsPool.Value.Get(effect);
                        if (effectCmp.effectEntity == curHealingItem && effectCmp.effectType == EffectInfo.EffectType.bleeding && effectCmp.effectLevel <= curHealthCmp.healingItemInfo.maxBleedingRemoveLevel)//снятие эффекта кровотечения
                        {
                            effectCmp.effectDuration = 0;
                        }
                    }
                    ChangeHealth(curHealingItem, -curHealthCmp.healingItemInfo.healingHealthPoints, false);//для хила - надо
                    if (isPlayer)//если игрок
                    {
                        if (curHealthCmp.healingItemInfo.addedBlur != 0 && _cameraComponentsPool.Value.Get(curHealingItem).blurValue < 250)
                            _cameraComponentsPool.Value.Get(curHealingItem).blurValue += curHealthCmp.healingItemInfo.addedBlur;

                        if (curHealthCmp.healingItemInfo.effectInfo != null)
                        {
                            ref var effCmp = ref _effectComponentsPool.Value.Add(_world.Value.NewEntity());
                            effCmp.effectEntity = curHealingItem;
                            effCmp.effectLevel = curHealthCmp.healingItemInfo.effectInfo.effectLevel;
                            effCmp.effectType = curHealthCmp.healingItemInfo.effectInfo.effectType;
                            effCmp.isFirstEffectCheck = true;
                            effCmp.effectDuration = curHealthCmp.healingItemInfo.effectInfo.effectTime;
                            effCmp.effectIconSprite = curHealthCmp.healingItemInfo.effectInfo.effectIconSprite;
                        }

                        ref var upgradeStatsCmp = ref _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity);
                        upgradeStatsCmp.currentStatsExp[1] += curHealthCmp.healingItemInfo.healingHealthPoints * 0.5f;
                        if (upgradeStatsCmp.currentStatsExp[1] >= _sceneData.Value.levelExpCounts[upgradeStatsCmp.statLevels[1]] && !_upgradePlayerStatEventsPool.Value.Has(_sceneData.Value.playerEntity))
                            _upgradePlayerStatEventsPool.Value.Add(_sceneData.Value.playerEntity).statIndex = 1;

                        ref var playerMoveCmp = ref _playerMoveComponentsPool.Value.Get(curHealingItem);
                        if (playerMoveCmp.currentHungerPoints != playerMoveCmp.maxHungerPoints)
                        {
                            playerMoveCmp.currentHungerPoints += curHealthCmp.healingItemInfo.recoveringHungerPoints;
                            if (playerMoveCmp.currentHungerPoints > playerMoveCmp.maxHungerPoints)
                                playerMoveCmp.currentHungerPoints = playerMoveCmp.maxHungerPoints;
                            _sceneData.Value.playerArmorBarFilled.fillAmount = playerMoveCmp.currentHungerPoints / playerMoveCmp.maxHungerPoints;
                            _sceneData.Value.playerArmorText.text = (int)playerMoveCmp.currentHungerPoints + " / " + (int)playerMoveCmp.maxHungerPoints;
                        }
                        if (_playerWeaponsInInventoryComponentsPool.Value.Get(curHealingItem).curWeapon != 2)
                        {
                            var gunInfo = _inventoryItemComponentsPool.Value.Get(_playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity).curEquipedWeaponCellEntity).itemInfo.gunInfo;
                            ref var movementCmp = ref _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                            movementCmp.movementView.weaponSpriteRenderer.sprite = gunInfo.weaponSprite;
                            movementCmp.movementView.weaponSprite.localScale = new Vector3(1, -1, 1) * gunInfo.spriteScaleMultiplayer;
                            movementCmp.movementView.weaponSprite.localEulerAngles = new Vector3(0, 0, gunInfo.spriteRotation);
                        }
                        else
                        {
                            var meleeInfo = _inventoryItemComponentsPool.Value.Get(_playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity).curEquipedWeaponCellEntity).itemInfo.meleeWeaponInfo;
                            ref var movementCmp = ref _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                            movementCmp.movementView.weaponSpriteRenderer.sprite = meleeInfo.weaponSprite;
                            movementCmp.movementView.weaponSprite.localScale = new Vector3(1, -1, 1) * meleeInfo.spriteScaleMultiplayer;
                            movementCmp.movementView.weaponSprite.localEulerAngles = new Vector3(0, 0, meleeInfo.spriteRotation);
                        }
                        _sceneData.Value.ammoInfoText.text = "";

                        //сделать чтобы в руках хилка отображалась когда хилится и убиралось на нужное оружие, когда заканчивает
                    }
                    else//если враг
                    {
                        //чекать местоположение игрока, чтобы выставить корректное текущее состояние
                        ref var creatureAiCmp = ref _creatureAIComponentsPool.Value.Get(curHealingItem);
                        ref var creatureAiInvCmp = ref _creatureInventoryComponentsPool.Value.Get(curHealingItem);
                        ref var moveCmp = ref _movementComponentsPool.Value.Get(curHealingItem);

                        if (creatureAiCmp.currentTarget != null)
                        {
                            float distanceBetweenTarget = Vector2.Distance(moveCmp.entityTransform.position, creatureAiCmp.currentTarget.position);

                            if (distanceBetweenTarget > creatureAiCmp.followDistance)
                                creatureAiCmp.currentState = CreatureAIComponent.CreatureStates.idle;
                            else if (distanceBetweenTarget > creatureAiCmp.safeDistance)
                                creatureAiCmp.currentState = CreatureAIComponent.CreatureStates.follow;
                            else if (distanceBetweenTarget > creatureAiCmp.minSafeDistance)
                                creatureAiCmp.currentState = CreatureAIComponent.CreatureStates.shootingToTarget;
                            else
                                creatureAiCmp.currentState = CreatureAIComponent.CreatureStates.runAwayFromTarget;
                        }

                        //if (_currentHealingItemComponentsPool.Value.Has(curHealingItem) && _currentHealingItemComponentsPool.Value.Get(curHealingItem).isHealing) return;
                        if (_creatureInventoryComponentsPool.Value.Get(curHealingItem).isSecondWeaponUsed || creatureAiInvCmp.gunItem == null)//смена на мили
                        {
                            creatureAiCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = creatureAiInvCmp.meleeWeaponItem.weaponSprite;

                            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localScale = new Vector3(1, -1, 1) * creatureAiInvCmp.meleeWeaponItem.spriteScaleMultiplayer;
                            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, creatureAiInvCmp.meleeWeaponItem.spriteRotation);
                        }
                        else
                        {
                            creatureAiCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = creatureAiInvCmp.gunItem.weaponSprite;

                            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localScale = new Vector3(1, -1, 1) * creatureAiInvCmp.gunItem.spriteScaleMultiplayer;
                            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, creatureAiInvCmp.gunItem.spriteRotation);
                        }

                        Debug.Log("Heal After Healing" + _healthComponentsPool.Value.Get(curHealingItem).healthPoint);
                    }
                }
            }

        }
        foreach (var healthEntity in _playerHealthFilter.Value)
        {
            ref var healthCmp = ref _healthComponentsPool.Value.Get(healthEntity);
            ref var playerMoveCmp = ref _playerMoveComponentsPool.Value.Get(healthEntity);
            int maxHpToHeal;
            playerMoveCmp.currentHealingOneHealTime += Time.deltaTime;
            if (playerMoveCmp.currentHungerPoints != 0)
            {
                if (playerMoveCmp.currentHungerPoints / playerMoveCmp.maxHungerPoints >= 0.7f)
                    maxHpToHeal = Mathf.CeilToInt(healthCmp.healthView.maxHealth * 0.7f);
                else
                    maxHpToHeal = Mathf.CeilToInt(healthCmp.healthView.maxHealth * playerMoveCmp.currentHungerPoints / playerMoveCmp.maxHungerPoints);
                if (healthCmp.healthPoint < maxHpToHeal)
                {
                    if (playerMoveCmp.currentHealingOneHealTime >= 2)
                    {
                        healthCmp.healthPoint++;
                        playerMoveCmp.currentHealingOneHealTime = 0;
                        ChangeHealthBarInfo(healthCmp);
                    }
                }
            }
            else
            {
                if (playerMoveCmp.currentHealingOneHealTime >= 2)
                {
                    ChangeHealth(healthEntity, 1, false);
                    playerMoveCmp.currentHealingOneHealTime = 0;
                }
            }
        }

        foreach (var changeEvent in _changeHealthEventsFilter.Value)
        {
            // для хила в ивент надо отрицательные числа вбивать
            var changeHealthEventCmp = _changeHealthEventsPool.Value.Get(changeEvent);
            int hpEvent = changeHealthEventCmp.changedEntity;
            bool isHeadshot = changeHealthEventCmp.isHeadshot;
            int changedHealthCount = changeHealthEventCmp.changedHealth;

            if (changedHealthCount > 0)
            {
                if (_playerComponentsPool.Value.Has(hpEvent))//если игрок
                {
                    var playerCmp = _playerComponentsPool.Value.Get(hpEvent);
                    float painkillersMultiplayer = 1f - playerCmp.levelOfPainkillers * 0.4f;
                    var playerGunCmp = _playerGunComponentsPool.Value.Get(hpEvent);
                    ref var cameraCmp = ref _cameraComponentsPool.Value.Get(hpEvent);

                    if (isHeadshot && _inventoryItemComponentsPool.Value.Has(_sceneData.Value.helmetCellView._entity))
                    {
                        var helmetItem = _inventoryItemComponentsPool.Value.Get(_sceneData.Value.helmetCellView._entity);
                        ref var helmetDurabilityCmp = ref _durabilityInInventoryComponentsPool.Value.Get(_sceneData.Value.helmetCellView._entity);
                        if (helmetDurabilityCmp.currentDurability > 0)
                        {
                            helmetDurabilityCmp.currentDurability -= changedHealthCount / 2;
                            changedHealthCount = Mathf.CeilToInt(changedHealthCount * (1f - helmetItem.itemInfo.helmetInfo.headDefenceMultiplayer));

                            if (helmetItem.itemInfo.helmetInfo.dropTransparentMultiplayer != 0)
                            {
                                int curDurability = Mathf.FloorToInt(_durabilityInInventoryComponentsPool.Value.Get(_sceneData.Value.helmetCellView._entity).currentDurability / (helmetItem.itemInfo.helmetInfo.armorDurability / 4));
                                if (curDurability != 3 && _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.sprite != _sceneData.Value.dropedItemsUIView.crackedGlassSprites[curDurability])
                                {
                                    _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.sprite = _sceneData.Value.dropedItemsUIView.crackedGlassSprites[curDurability];
                                    _sceneData.Value.dropedItemsUIView.crackedGlassHelmetUI.gameObject.SetActive(true);
                                }

                            }
                        }
                        else if (helmetDurabilityCmp.currentDurability < 0)
                            helmetDurabilityCmp.currentDurability = 0;
                    }
                    else if (!isHeadshot && _inventoryItemComponentsPool.Value.Has(_sceneData.Value.bodyArmorCellView._entity))
                    {
                        var bodyArmorItem = _inventoryItemComponentsPool.Value.Get(_sceneData.Value.bodyArmorCellView._entity);
                        ref var bodyArmorDurabilityCmp = ref _durabilityInInventoryComponentsPool.Value.Get(_sceneData.Value.bodyArmorCellView._entity);
                        if (bodyArmorDurabilityCmp.currentDurability > 0)
                        {
                            bodyArmorDurabilityCmp.currentDurability -= changedHealthCount;
                            changedHealthCount = Mathf.CeilToInt(changedHealthCount * (1f - bodyArmorItem.itemInfo.bodyArmorInfo.defenceMultiplayer));//change with calculate durability
                        }
                        else if (bodyArmorDurabilityCmp.currentDurability < 0)
                            bodyArmorDurabilityCmp.currentDurability = 0;
                    }

                    if (!playerGunCmp.inScope && _sceneData.Value.mainCamera.orthographicSize > cameraCmp.currentMaxCameraSpread)
                        _sceneData.Value.mainCamera.orthographicSize -= (changedHealthCount * 0.03f) * (1 - ((_sceneData.Value.mainCamera.orthographicSize - 5f) / (cameraCmp.currentMaxCameraSpread - 5f))) * (painkillersMultiplayer);
                    else if (playerGunCmp.inScope && _sceneData.Value.mainCamera.orthographicSize > cameraCmp.currentMaxCameraSpread * playerGunCmp.currentScopeMultiplicity)
                        _sceneData.Value.mainCamera.orthographicSize -= (changedHealthCount * 0.03f) * playerGunCmp.currentScopeMultiplicity * (1 - ((_sceneData.Value.mainCamera.orthographicSize / playerGunCmp.currentScopeMultiplicity - 5f)) / (cameraCmp.currentMaxCameraSpread - 5f)) * (painkillersMultiplayer);

                    var gunCmp = _gunComponentsPool.Value.Get(hpEvent);
                    if (gunCmp.currentSpread < gunCmp.currentMaxSpread)
                        gunCmp.currentSpread += changedHealthCount * 0.01f * painkillersMultiplayer;

                    if (cameraCmp.blurValue < 250)
                        cameraCmp.blurValue += changedHealthCount * 3.5f * painkillersMultiplayer;
                    if (changedHealthCount >= 60)
                        _offInScopeStateEventsPool.Value.Add(_world.Value.NewEntity());

                    ref var moveCmp = ref _movementComponentsPool.Value.Get(hpEvent);
                    moveCmp.speedMultiplayer += (1 - moveCmp.speedMultiplayer) * changedHealthCount * 0.03f * painkillersMultiplayer;

                    float alpfaMultiplayer = 1;
                    bool isAutoCleanBlood = false;
                    if (_inventoryItemComponentsPool.Value.Has(_sceneData.Value.helmetCellView._entity))
                    {
                        var helmetItemCmpInfo = _inventoryItemComponentsPool.Value.Get(_sceneData.Value.helmetCellView._entity).itemInfo.helmetInfo;
                        alpfaMultiplayer = 1 - helmetItemCmpInfo.dropTransparentMultiplayer;
                        isAutoCleanBlood = helmetItemCmpInfo.autoBloodCleaning;
                    }
                    for (int i = 0; i < changedHealthCount / 5; i++)
                    {

                        if (!isAutoCleanBlood)
                            _bloodParticleOnScreenComponentsPool.Value.Add(_world.Value.NewEntity()).bloodParticleImage = _sceneData.Value.GetParticleOnScreen(_sceneData.Value.bloodParticleOnScreenColor, alpfaMultiplayer, false);
                        else
                            _fadedParticleOnScreenComponentsPool.Value.Add(_world.Value.NewEntity()).particleImage = _sceneData.Value.GetParticleOnScreen(_sceneData.Value.bloodParticleOnScreenColor, alpfaMultiplayer, false);
                    }
                }
                else if (_creatureAIComponentsPool.Value.Has(hpEvent))
                {
                    var creatureAiCmp = _creatureAIComponentsPool.Value.Get(hpEvent);
                    if (isHeadshot && creatureAiCmp.helmetInfo != null)
                        changedHealthCount = Mathf.CeilToInt(changedHealthCount * (1 - creatureAiCmp.helmetInfo.headDefenceMultiplayer));
                    else if (creatureAiCmp.armorInfo != null)
                        changedHealthCount = Mathf.CeilToInt(changedHealthCount * (1 - creatureAiCmp.armorInfo.defenceMultiplayer));

                    if (_gunComponentsPool.Value.Has(hpEvent))
                    {
                        var gunCmp = _gunComponentsPool.Value.Get(hpEvent);
                        if (gunCmp.currentSpread < gunCmp.currentMaxSpread)
                            gunCmp.currentSpread += changedHealthCount * 0.01f;
                    }
                    if (_movementComponentsPool.Value.Has(hpEvent))
                    {
                        ref var moveCmp = ref _movementComponentsPool.Value.Get(hpEvent);
                        moveCmp.speedMultiplayer += (1 - moveCmp.speedMultiplayer) * changedHealthCount * 0.03f;
                    }
                }
            }
            Debug.Log(changedHealthCount + " damage taken " + isHeadshot + " isHead");
            ChangeHealth(hpEvent, changedHealthCount, isHeadshot);
        }

        #region -enemy healing item using ai-
        foreach (var aiCreature in _creatureAIComponentsFilter.Value)
        {
            if (_currentHealingItemComponentsPool.Value.Has(aiCreature))
            {
                // _creatureAIComponentsPool.Value.Get();
                ref var healthCmp = ref _healthComponentsPool.Value.Get(aiCreature);
                ref var healthItemCmp = ref _currentHealingItemComponentsPool.Value.Get(aiCreature);

                if (healthCmp.healthPoint < healthCmp.maxHealthPoint * 0.3f && !healthItemCmp.isHealing)
                {
                    healthItemCmp.isHealing = true;
                    ref var creatureAiCmp = ref _creatureAIComponentsPool.Value.Get(aiCreature);
                    var creatureAiInvCmp = _creatureInventoryComponentsPool.Value.Get(aiCreature);
                    creatureAiCmp.currentState = CreatureAIComponent.CreatureStates.runAwayFromTarget;
                    _attackComponentsPool.Value.Get(aiCreature).canAttack = false;

                    creatureAiCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = creatureAiInvCmp.healingItem.inGameHealingItemSprite;

                    creatureAiCmp.creatureView.aiCreatureView.itemTransform.localScale = Vector3.one * creatureAiInvCmp.healingItem.scaleMultplayer;
                    creatureAiCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, creatureAiInvCmp.healingItem.rotationZ);
                }
            }
        }

        #endregion
    }

    private void ChangeHealth(int hpEvent, int changedHealthCount, bool isHeadshot)
    {
        ref var healthCmp = ref _healthComponentsPool.Value.Get(hpEvent);
        Debug.Log(hpEvent + " change health to " + changedHealthCount);
        if (healthCmp.isDeath) return;

        if (changedHealthCount < 0)
        {
            healthCmp.healthPoint -= changedHealthCount;
            if (healthCmp.healthPoint > healthCmp.maxHealthPoint)
                healthCmp.healthPoint = healthCmp.maxHealthPoint;
            if (hpEvent == _sceneData.Value.playerEntity)
                ChangeHealthBarInfo(healthCmp);
        }

        /* else if (_armorComponentsPool.Value.Has(hpEvent))
         {
             ref var armorCmp = ref _armorComponentsPool.Value.Get(hpEvent);
             if (armorCmp.armorPoint == 0)
             {
                 healthCmp.healthPoint -= changedHealthCount;
                 if (hpEvent == _sceneData.Value.playerEntity)
                     ChangeHealthBarInfo(healthCmp);
             }

             else if (armorCmp.armorPoint >= changedHealthCount)
             {
                 armorCmp.armorPoint -= changedHealthCount;
                 if (hpEvent == _sceneData.Value.playerEntity)
                     ChangeHungerBarInfo(armorCmp);
             }

             else
             {
                 changedHealthCount -= armorCmp.armorPoint;
                 healthCmp.healthPoint -= changedHealthCount;
                 armorCmp.armorPoint = 0;
                 if (hpEvent == _sceneData.Value.playerEntity)
                 {
                     ChangeHealthBarInfo(healthCmp);
                     ChangeHungerBarInfo(armorCmp);
                 }
             }
         }*/

        else
        {
            healthCmp.healthPoint -= changedHealthCount;
            if (hpEvent == _sceneData.Value.playerEntity)
                ChangeHealthBarInfo(healthCmp);
        }

        if (healthCmp.healthPoint <= 0)
        {
            healthCmp.isDeath = true;
            if (_creatureAIComponentsPool.Value.Has(hpEvent))
            {
                foreach (var effectEntity in _effectComponentsFilter.Value)
                {
                    ref var effectCmp = ref _effectComponentsPool.Value.Get(effectEntity);
                    if (effectCmp.effectEntity == hpEvent)
                    {
                        if (effectCmp.effectType == EffectInfo.EffectType.mantrap)
                        {
                            _movementComponentsPool.Value.Get(effectCmp.effectEntity).isTrapped = false;
                        }
                        else if (effectCmp.effectType == EffectInfo.EffectType.bleeding)
                        {
                            _effectParticleLifetimeTagsPool.Value.Del(effectCmp.effectEntity);
                            _sceneData.Value.ReleaseParticlePrefab(_particleLifetimeComponentsPool.Value.Get(effectCmp.effectEntity).objectToHide);
                            _particleLifetimeComponentsPool.Value.Del(effectCmp.effectEntity);
                        }

                        // _sceneData.Value.ReleaseEffecticonView(effectCmp.effectIconView);
                        _effectComponentsPool.Value.Del(effectEntity);
                    }

                }

                healthCmp.healthView.Death();
                _deathEventsPool.Value.Add(hpEvent).isHeadshot = isHeadshot;
                var curLocCmp = _currentLocationComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                var curLevelDroppedItems = curLocCmp.currentLocation.levels[curLocCmp.levelNum - 1].droopedItems;
                int curWaeponsCellEntity = _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity).curEquipedWeaponCellEntity;
                // ref var weaponLevelCmp = ref _weaponLevelComponentsComponentsPool.Value.Get(curWaeponsCellEntity);
                var enemyInfo = _creatureInventoryComponentsPool.Value.Get(hpEvent).enemyClassSettingInfo;
                var curWeaponItemCmp = _inventoryItemComponentsPool.Value.Get(curWaeponsCellEntity);
                var weaponLevelStats = _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).weaponsExp[curWeaponItemCmp.itemInfo.itemId];
                weaponLevelStats.weaponCurrentExp += enemyInfo.expPointsForKill;
                Debug.Log(enemyInfo.expPointsForKill + " + exp" + weaponLevelStats.weaponCurrentExp);

                if (weaponLevelStats.weaponCurrentExp >= _sceneData.Value.levelExpCounts[weaponLevelStats.weaponExpLevel])
                {
                    if (curWeaponItemCmp.itemInfo.type == ItemInfo.itemType.gun)
                        _attackComponentsPool.Value.Get(_sceneData.Value.playerEntity).damage = Mathf.CeilToInt((1 + weaponLevelStats.weaponExpLevel * 0.02f) * curWeaponItemCmp.itemInfo.gunInfo.damage);
                    else
                        _attackComponentsPool.Value.Get(_sceneData.Value.playerEntity).damage = Mathf.CeilToInt((1 + weaponLevelStats.weaponExpLevel * 0.02f) * curWeaponItemCmp.itemInfo.meleeWeaponInfo.damage);
                    weaponLevelStats.weaponExpLevel++;
                }
                int percentDrop = Random.Range(0, 101);
                int curItems = 0;
                for (int i = 0; i < curLevelDroppedItems.Length; i++)
                {
                    if (percentDrop <= curLevelDroppedItems[i].dropPercent * enemyInfo.dropPercentMultiplayer)
                    {
                        int droopedCount = Random.Range(curLevelDroppedItems[i].itemsCountMin, curLevelDroppedItems[i].itemsCountMax + 1);
                        percentDrop = Random.Range(0, 101);
                        int droppedItem = _world.Value.NewEntity();
                        curItems++;
                        ref var droppedItemComponent = ref _droppedItemComponentsPool.Value.Add(droppedItem);

                        droppedItemComponent.currentItemsCount = droopedCount;

                        Vector2 deathPos = _movementComponentsPool.Value.Get(hpEvent).entityTransform.position;
                        //если будет ган то ещё и ган инв комп добавлять с почти убитым оружием и парочкой патронов
                        droppedItemComponent.itemInfo = curLevelDroppedItems[i].droopedItem;
                        droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(new Vector2(Random.Range(deathPos.x - 1, deathPos.x + 1), Random.Range(deathPos.y - 1, deathPos.y + 1)), curLevelDroppedItems[i].droopedItem, droppedItem);
                        _hidedObjectOutsideFOVComponentsPool.Value.Add(droppedItem).hidedObjects = new Transform[] { droppedItemComponent.droppedItemView.transform.GetChild(0) };
                        if (curItems >= 3)
                            break;
                    }
                }

                // }

                healthCmp.healthPoint = 0;

            }
            else if (_playerComponentsPool.Value.Has(hpEvent))//смерть игрока
            {
                _deathEventsPool.Value.Add(hpEvent);
                Time.timeScale = 0;
                ref var playerMoveCmp = ref _playerMoveComponentsPool.Value.Get(hpEvent);
                _movementComponentsPool.Value.Get(hpEvent).currentRunTime = playerMoveCmp.playerView.runTime;
                playerMoveCmp.currentHungerPoints = playerMoveCmp.maxHungerPoints / 2;
                healthCmp.healthPoint = healthCmp.maxHealthPoint / 2;
                //сделать этот метод при спавне если будут баги
            }
            else
            {
                int curItems = 0;
                int percentDrop = Random.Range(0, 101);
                if (healthCmp.healthView.interestObjectView != null)
                {
                    var dropItemsView = healthCmp.healthView.interestObjectView;
                    for (int i = 0; i < dropItemsView.dropElements.Length; i++)
                    {
                        if (percentDrop <= dropItemsView.dropElements[i].dropPercent)
                        {
                            int droopedCount = Random.Range(dropItemsView.dropElements[i].itemsCountMin, dropItemsView.dropElements[i].itemsCountMax + 1);
                            percentDrop = Random.Range(0, 101);
                            int droppedItemEntity = _world.Value.NewEntity();
                            curItems++;
                            ref var droppedItemComponent = ref _droppedItemComponentsPool.Value.Add(droppedItemEntity);
                            droppedItemComponent.currentItemsCount = droopedCount;

                            Vector2 deathPos = healthCmp.healthView.gameObject.transform.position;
                            //если будет ган то ещё и ган инв комп добавлять с почти убитым оружием и парочкой патронов
                            droppedItemComponent.itemInfo = dropItemsView.dropElements[i].droopedItem;
                            droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(deathPos, dropItemsView.dropElements[i].droopedItem, droppedItemEntity);
                            _hidedObjectOutsideFOVComponentsPool.Value.Add(droppedItemEntity).hidedObjects = new Transform[] { droppedItemComponent.droppedItemView.transform.GetChild(0) };
                            if (curItems >= dropItemsView.maxDroppedItemsCount)
                                break;
                        }
                    }
                }
                healthCmp.healthView.Death();
                _destroyComponentInNextFrameTagsPool.Value.Add(hpEvent);
            }

            if (_movementComponentsPool.Value.Has(hpEvent))
            {
                ref var moveCmp = ref _movementComponentsPool.Value.Get(hpEvent);
                moveCmp.movementView.objectTransform.gameObject.SetActive(false);
                moveCmp.movementView.characterAnimator.SetBool("isDeath", true);
                moveCmp.movementView.characterAnimator.gameObject.SetActive(true);
            }
        }

    }
    private void ChangeHealthBarInfo(HealthComponent healthComponent)
    {
        _sceneData.Value.playerHealthBarFilled.fillAmount = (float)healthComponent.healthPoint / healthComponent.maxHealthPoint;
        _sceneData.Value.playerHealthText.text = healthComponent.healthPoint + " / " + healthComponent.maxHealthPoint;
    }


}
