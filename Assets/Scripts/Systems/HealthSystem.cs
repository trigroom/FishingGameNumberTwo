using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class HealthSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<ChangeHealthEvent> _changeHealthEventsPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<ArmorComponent> _armorComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<CurrentHealingItemComponent> _currentHealingItemComponentsPool;

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
        foreach(var curHealingItem in _currentHealingItemComponentsFilter.Value)
        {
            ref var curHealthCmp = ref _currentHealingItemComponentsPool.Value.Get(curHealingItem);
            if (curHealthCmp.isHealing)
            {
                curHealthCmp.currentHealingTime += Time.deltaTime;
                //������ �������� ��������
                if (curHealthCmp.currentHealingTime >= curHealthCmp.healingTime)
                {
                    curHealthCmp.currentHealingTime = 0;
                    curHealthCmp.isHealing = false;
                    ChangeHealth(curHealingItem, -curHealthCmp.healingHealthPoints);//��� ���� - ����
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
            // ��� ���� � ����� ���� ������������� ����� �������
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
                //���� �� ��� �������� ��� ������(����� ������ ����)
                healthCmp.healthPoint = 0;
                _healthComponentsPool.Value.Del(hpEvent);
                if (_armorComponentsPool.Value.Has(hpEvent))
                    _armorComponentsPool.Value.Del(hpEvent);
                if (hpEvent == _sceneData.Value.playerEntity)
                    ChangeHealthBarInfo(healthCmp);
                //�������� ������ � � ����� �� ��������� ������ �������
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
