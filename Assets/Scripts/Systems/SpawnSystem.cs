using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<ActiveSpawnComponent> _activeSpawnComponentsPool; //будут добавляться новые активные и удаляться те которые не используются
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<EntrySpawnZoneEvent> _entryCollisionWithSpawnZoneEventsPool;
    private EcsPoolInject<ExitSpawnZoneEvent> _exitCollisionWithSpawnZoneEventsPool;
    private EcsPoolInject<CreatureTag> _creatureTagsPool;
    private EcsPoolInject<SpawnZoneTag> _spawnZoneTagsPool;

    private EcsFilterInject<Inc<ActiveSpawnComponent>> _activeSpawnComponentsFilter;
    private EcsFilterInject<Inc<EntrySpawnZoneEvent>> _entrySpawnZoneEventsFilter;
    private EcsFilterInject<Inc<ExitSpawnZoneEvent>> _exitSpawnZoneEventsFilter;

    public void Run(IEcsSystems systems)
    {
        foreach(var spawnCmpEntity in _activeSpawnComponentsFilter.Value)
        {
            ref var curSpawnCmp = ref _activeSpawnComponentsPool.Value.Get(spawnCmpEntity);

            curSpawnCmp.curSpawnTime += Time.deltaTime;

            if (curSpawnCmp.curSpawnTime >= curSpawnCmp.spawnTime)
            {
                curSpawnCmp.curSpawnTime = 0;
                int creatureEntity = _world.Value.NewEntity();
                ref var creatureHealthCmp = ref _healthComponentsPool.Value.Add(creatureEntity);
                _creatureTagsPool.Value.Add(creatureEntity);

                creatureHealthCmp.healthView = _sceneData.Value.GetCreature(curSpawnCmp.spawnObjects[Random.Range(0, curSpawnCmp.spawnObjects.Length)], _sceneData.Value.GetOutOfScreenPosition());
                creatureHealthCmp.healthView.Construct(creatureEntity);
                creatureHealthCmp.maxHealthPoint = creatureHealthCmp.healthView.maxHealth;
                creatureHealthCmp.healthPoint = creatureHealthCmp.maxHealthPoint;Debug.Log(creatureHealthCmp.healthPoint + "curEnemyHealth");
            }
        }

        foreach(var entryZoneEvent in _entrySpawnZoneEventsFilter.Value)
        {
            ref var zone = ref _entryCollisionWithSpawnZoneEventsPool.Value.Get(entryZoneEvent).zoneView;
            //повесить спавн тэг како нибудь, чтобы энтити не удалялась
            int spawnCmpEntity = 0;
            if (zone._entity == 0)
            {
                spawnCmpEntity = _world.Value.NewEntity();
                zone.Construct(spawnCmpEntity);
                _spawnZoneTagsPool.Value.Add(spawnCmpEntity);
            }
            else
                spawnCmpEntity = zone._entity;

            ref var spawnCmp = ref _activeSpawnComponentsPool.Value.Add(spawnCmpEntity);
            spawnCmp.zoneView = zone;
            spawnCmp.spawnTime = zone.spawnTime;
            spawnCmp.spawnObjects = zone.spawnObjects;
        }

        foreach(var exitZoneEvent in _exitSpawnZoneEventsFilter.Value)
        {
            ref var zone = ref _exitCollisionWithSpawnZoneEventsPool.Value.Get(exitZoneEvent).zoneView;

            _activeSpawnComponentsPool.Value.Del(zone._entity);
        }
    }
}
