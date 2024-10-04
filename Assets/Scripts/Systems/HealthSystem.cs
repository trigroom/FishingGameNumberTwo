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
    private EcsPoolInject<CurrentHealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<CreatureDropComponent> _creatureDropComponentsPool;
    private EcsPoolInject<DroppedItemComponent> _droppedItemComponentsPool;

    private EcsFilterInject<Inc<ChangeHealthEvent>> _changeHealthEventsFilter;
    private EcsFilterInject<Inc<ArmorComponent>> _armorComponentsFilter;
    private EcsFilterInject<Inc<CurrentHealingItemComponent>> _currentHealingItemComponentsFilter;
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
                if (curHealthCmp.currentHealingTime >= curHealthCmp.healingTime)
                {
                    curHealthCmp.currentHealingTime = 0;
                    curHealthCmp.isHealing = false;
                    ChangeHealth(curHealingItem, -curHealthCmp.healingHealthPoints);//для хила - надо
                    var gunCmp = _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                    _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity; ;
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
