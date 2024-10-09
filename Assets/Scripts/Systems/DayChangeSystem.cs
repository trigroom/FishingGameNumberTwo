using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class DayChangeSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<ChangeToNightEvent> _changeToNightEventsPool;//возможно не понадобится, надо просто делать проверку на isNight в других скриптах
    private EcsPoolInject<ChangeToDayEvent> _changeToDayEventsPool;
    // private EcsPoolInject<HealthComponent> _healthComponentsPool;
    //private EcsPoolInject<CurrentAttackComponent> _currentAttackComponentsPool;
    //private EcsPoolInject<GunComponent> _gunComponentsPool;
    //private EcsPoolInject<ArmorComponent> _armorComponentsPool;
    // private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;

    private EcsFilterInject<Inc<GlobalTimeComponent>> _globalTimeComponentsFilter;

    private EcsCustomInject<SceneService> _sceneService;
    private EcsWorldInject _world;

    public void Init(IEcsSystems systems)
    {
        _globalTimeComponentsPool.Value.Add(_sceneService.Value.playerEntity).changeGloabalLightTime = 45;
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entity in _globalTimeComponentsFilter.Value)
        {
            ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(entity);
            /*float curTime = 1440 * globalTimeCmp.currentDayTime / _sceneService.Value.dayTime;
            globalTimeCmp.minutesToTimerTextCount += curTime;
            if(globalTimeCmp.minutesToTimerTextCount >=1) 
            {
                globalTimeCmp.minutesToTimerTextCount--;
                int hours = (int)(curTime / 60) - _sceneService.Value.timeHourOffset;
                if (hours < 0)
                    hours += 24;
            _sceneService.Value.dropedItemsUIView.dayTimeText.text = hours.ToString() + ":" + ((int)(curTime % 60)).ToString("00"); 
            }*/

            globalTimeCmp.currentDayTime += Time.deltaTime;
            if(globalTimeCmp.currentDayTime > _sceneService.Value.dayTime*0.7f && !globalTimeCmp.isNight)
            {
                globalTimeCmp.changeGloabalLightTime = 0;
                globalTimeCmp.isNight = true;
                _changeToNightEventsPool.Value.Add(entity);
            }
            else if (globalTimeCmp.currentDayTime > _sceneService.Value.dayTime)
            {
                globalTimeCmp.changeGloabalLightTime = 0;
                globalTimeCmp.isNight = false;
                _changeToDayEventsPool.Value.Add(entity);
                globalTimeCmp.currentDay++;
                globalTimeCmp.currentDayTime = 0;
            }
            if(globalTimeCmp.changeGloabalLightTime <= 45)
            {
                globalTimeCmp.changeGloabalLightTime += Time.deltaTime;
                if (globalTimeCmp.isNight)
                {
                    Debug.Log("night");
                    _sceneService.Value.gloabalLight.intensity = Mathf.MoveTowards(_sceneService.Value.gloabalLight.intensity, 0.2f, _sceneService.Value.gloabalLight.intensity / 30 * Time.deltaTime);
                    _sceneService.Value.gloabalLight.color = _sceneService.Value.nightLightColor.Evaluate(Mathf.Lerp(0, 1, globalTimeCmp.changeGloabalLightTime / 40));
                }
                else
                {
                    Debug.Log("day");
                    _sceneService.Value.gloabalLight.intensity = Mathf.MoveTowards(_sceneService.Value.gloabalLight.intensity, 0.75f, _sceneService.Value.gloabalLight.intensity / 30 * Time.deltaTime);
                    _sceneService.Value.gloabalLight.color = _sceneService.Value.nightLightColor.Evaluate(Mathf.Lerp(1, 0, globalTimeCmp.changeGloabalLightTime / 40));
                }
            }
        }
    }
}
