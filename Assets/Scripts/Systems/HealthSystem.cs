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
    private EcsPoolInject<ArmorComponent> _armorComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<HealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<CreatureDropComponent> _creatureDropComponentsPool;
    private EcsPoolInject<DroppedItemComponent> _droppedItemComponentsPool;
    private EcsPoolInject<AttackComponent> _attackComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<CreatureInventoryComponent> _creatureInventoryComponentsPool;

    private EcsFilterInject<Inc<ChangeHealthEvent>> _changeHealthEventsFilter;
    private EcsFilterInject<Inc<ArmorComponent>> _armorComponentsFilter;
    private EcsFilterInject<Inc<HealingItemComponent>> _currentHealingItemComponentsFilter;
    private EcsFilterInject<Inc<CreatureAIComponent>> _creatureAIComponentsFilter;
    public void Init(IEcsSystems systems)
    {
        ref var healthCmp = ref _healthComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var armorCmp = ref _armorComponentsPool.Value.Get(_sceneData.Value.playerEntity);

        ChangeHealthBarInfo(healthCmp);
        ChangeArmorBarInfo(armorCmp);
    }
    public void Run(IEcsSystems systems)
    {
        foreach (var curHealingItem in _currentHealingItemComponentsFilter.Value)
        {
            ref var curHealthCmp = ref _currentHealingItemComponentsPool.Value.Get(curHealingItem);
            if (curHealthCmp.isHealing)
            {
                curHealthCmp.currentHealingTime += Time.deltaTime;
                //менять скорость возможно
                if (curHealthCmp.currentHealingTime >= curHealthCmp.healingItemInfo.healingTime)
                {
                    curHealthCmp.currentHealingTime = 0;
                    curHealthCmp.isHealing = false;
                    _attackComponentsPool.Value.Get(curHealingItem).canAttack = true;
                    ChangeHealth(curHealingItem, -curHealthCmp.healingItemInfo.healingHealthPoints);//для хила - надо
                    if (_sceneData.Value.playerEntity == curHealingItem)//если игрок
                    {
                        if (_playerWeaponsInInventoryComponentsPool.Value.Get(curHealingItem).curWeapon != 2)
                        {
                            var gunCmp = _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                            _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
                        }
                        else
                            _sceneData.Value.ammoInfoText.text = "";

                        //сделать чтобы в руках хилка отображалась когда хилится и убиралось на нужное оружие, когда заканчивает
                    }
                    else//если враг
                    {
                        //чекать местоположение игрока, чтобы выставить корректное текущее состояние
                        ref var creatureAiCmp = ref _creatureAIComponentsPool.Value.Get(curHealingItem);
                        ref var moveCmp = ref _movementComponentsPool.Value.Get(curHealingItem);
                        float distanceBetweenTarget = Vector2.Distance(moveCmp.entityTransform.position, creatureAiCmp.targetTransform.position);



                        if (distanceBetweenTarget > creatureAiCmp.followDistance)
                            creatureAiCmp.currentState = CreatureAIComponent.CreatureStates.idle;
                        else if (distanceBetweenTarget > creatureAiCmp.safeDistance)
                            creatureAiCmp.currentState = CreatureAIComponent.CreatureStates.follow;
                        else if (distanceBetweenTarget > creatureAiCmp.minSafeDistance)
                            creatureAiCmp.currentState = CreatureAIComponent.CreatureStates.shootingToTarget;
                        else
                            creatureAiCmp.currentState = CreatureAIComponent.CreatureStates.runAwayFromTarget;

                        if (_creatureInventoryComponentsPool.Value.Has(curHealingItem) && _creatureInventoryComponentsPool.Value.Get(curHealingItem).isSecondWeaponUsed || !_creatureInventoryComponentsPool.Value.Has(curHealingItem) && creatureAiCmp.creatureView.creatureMeleeView != null)//смена на мили
                        {
                            creatureAiCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = creatureAiCmp.creatureView.creatureMeleeView.itemVisualInfo.itemSprite;

                            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localScale = Vector3.one * creatureAiCmp.creatureView.creatureMeleeView.itemVisualInfo.itemScale;
                            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, creatureAiCmp.creatureView.creatureMeleeView.itemVisualInfo.itemRotateZ);
                        }
                        else
                        {
                            creatureAiCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = creatureAiCmp.creatureView.creatureGunView.itemVisualInfo.itemSprite;

                            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localScale = Vector3.one * creatureAiCmp.creatureView.creatureGunView.itemVisualInfo.itemScale;
                            creatureAiCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, creatureAiCmp.creatureView.creatureGunView.itemVisualInfo.itemRotateZ);
                        }

                        Debug.Log("Heal After Healing" + _healthComponentsPool.Value.Get(curHealingItem).healthPoint);
                    }
                }
            }

        }
        foreach (var armorEntity in _armorComponentsFilter.Value)
        {
            ref var armorCmp = ref _armorComponentsPool.Value.Get(armorEntity);

            if (armorCmp.armorPoint != armorCmp.maxArmorPoint)
            {
                armorCmp.armorPointsToAdd += armorCmp.armorRecoverySpeed * Time.deltaTime;

                if (armorCmp.armorPointsToAdd >= 1)
                {
                    armorCmp.armorPoint += (int)armorCmp.armorPointsToAdd;
                    if (armorCmp.armorPoint > armorCmp.maxArmorPoint)
                        armorCmp.armorPoint = armorCmp.maxArmorPoint;

                    armorCmp.armorPointsToAdd -= (int)armorCmp.armorPointsToAdd;
                    ChangeArmorBarInfo(armorCmp);
                }
            }
        }

        foreach (var changeEvent in _changeHealthEventsFilter.Value)
        {
            // для хила в ивент надо отрицательные числа вбивать
            int changedHealthCount = _changeHealthEventsPool.Value.Get(changeEvent).changedHealth;
            int hpEvent = _changeHealthEventsPool.Value.Get(changeEvent).changedEntity;

            ChangeHealth(hpEvent, changedHealthCount);
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
                    creatureAiCmp.currentState = CreatureAIComponent.CreatureStates.runAwayFromTarget;
                    _attackComponentsPool.Value.Get(aiCreature).canAttack = false;

                    creatureAiCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = creatureAiCmp.creatureView.aiCreatureView.healItemVisualInfo.itemSprite;

                    creatureAiCmp.creatureView.aiCreatureView.itemTransform.localScale = Vector3.one * creatureAiCmp.creatureView.aiCreatureView.healItemVisualInfo.itemScale;
                    creatureAiCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, creatureAiCmp.creatureView.aiCreatureView.healItemVisualInfo.itemRotateZ);
                }
            }
        }

        #endregion
    }

    private void ChangeHealth(int hpEvent, int changedHealthCount)
    {
        if (!_healthComponentsPool.Value.Has(hpEvent))
            return;
        ref var healthCmp = ref _healthComponentsPool.Value.Get(hpEvent);

        if (changedHealthCount < 0)
        {
            healthCmp.healthPoint -= changedHealthCount;
            if (healthCmp.healthPoint > healthCmp.maxHealthPoint)
                healthCmp.healthPoint = healthCmp.maxHealthPoint;
            if (hpEvent == _sceneData.Value.playerEntity)
                ChangeHealthBarInfo(healthCmp);
        }

        else if (_armorComponentsPool.Value.Has(hpEvent))
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
                    ChangeArmorBarInfo(armorCmp);
            }

            else
            {
                changedHealthCount -= armorCmp.armorPoint;
                healthCmp.healthPoint -= changedHealthCount;
                armorCmp.armorPoint = 0;
                if (hpEvent == _sceneData.Value.playerEntity)
                {
                    ChangeHealthBarInfo(healthCmp);
                    ChangeArmorBarInfo(armorCmp);
                }
            }
        }

        else
        {
            healthCmp.healthPoint -= changedHealthCount;
            if (hpEvent == _sceneData.Value.playerEntity)
                ChangeHealthBarInfo(healthCmp);
        }

        if (healthCmp.healthPoint <= 0)
        {
            healthCmp.healthView.Death();
            if (hpEvent == _sceneData.Value.playerEntity)
            {
                ChangeHealthBarInfo(healthCmp);
                //кудато как то возродить игрока и выкинуть ему какую то менюшку
            }

            else
            {
                if (_creatureDropComponentsPool.Value.Has(hpEvent))
                {
                    var dropItems = _creatureDropComponentsPool.Value.Get(hpEvent);

                    for (int i = 0; i < dropItems.droopedItems.Length; i++)
                    {
                        if (Random.Range(0, 101) <= dropItems.droopedItems[i].dropPercent)
                        {
                            int droopedCount = Random.Range(dropItems.droopedItems[i].itemsCountMin, dropItems.droopedItems[i].itemsCountMax + 1);

                            var droppedItem = _world.Value.NewEntity();

                            ref var droppedItemComponent = ref _droppedItemComponentsPool.Value.Add(droppedItem);

                            droppedItemComponent.currentItemsCount = droopedCount;

                            Vector2 deathPos = _movementComponentsPool.Value.Get(hpEvent).entityTransform.position;
                            //если будет ган то ещё и ган инв комп добавлять с почти убитым оружием и парочкой патронов
                            droppedItemComponent.itemInfo = dropItems.droopedItems[i].droopedItem;
                            droppedItemComponent.droppedItemView = _sceneData.Value.SpawnDroppedItem(new Vector2(Random.Range(deathPos.x - 1, deathPos.x + 1), Random.Range(deathPos.y - 1, deathPos.y + 1)), dropItems.droopedItems[i].droopedItem, droppedItem);
                        }
                    }
                    _creatureDropComponentsPool.Value.Del(hpEvent);
                }

                healthCmp.healthPoint = 0;
                _healthComponentsPool.Value.Del(hpEvent);
                _creatureAIComponentsPool.Value.Del(hpEvent);
                _movementComponentsPool.Value.Del(hpEvent);
                if (_armorComponentsPool.Value.Has(hpEvent))
                    _armorComponentsPool.Value.Del(hpEvent);

            }
            //акие то доп действия при смерти(ивент смерти можн)
            //анимация смерти и в конце неё полностью энтити удалять
        }

    }
    private void ChangeHealthBarInfo(HealthComponent healthComponent)
    {
        _sceneData.Value.playerHealthBarFilled.fillAmount = (float)healthComponent.healthPoint / healthComponent.maxHealthPoint;
        _sceneData.Value.playerHealthText.text = healthComponent.healthPoint + " / " + healthComponent.maxHealthPoint;
    }

    private void ChangeArmorBarInfo(ArmorComponent healthComponent)
    {
        _sceneData.Value.playerArmorBarFilled.fillAmount = (float)healthComponent.armorPoint / healthComponent.maxArmorPoint;
        _sceneData.Value.playerArmorText.text = healthComponent.armorPoint + " / " + healthComponent.maxArmorPoint;
    }

}
