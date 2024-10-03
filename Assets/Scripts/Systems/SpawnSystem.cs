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
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<CurrentAttackComponent> _currentAttackComponentsPool;

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
                Debug.Log("spawn");
                curSpawnCmp.curSpawnTime = 0;
                int creatureEntity = _world.Value.NewEntity();
                ref var creatureHealthCmp = ref _healthComponentsPool.Value.Add(creatureEntity);
                _creatureTagsPool.Value.Add(creatureEntity);
                ref var creatureAiStatesCmp = ref _creatureAIComponentsPool.Value.Add(creatureEntity);
                ref var moveCmp = ref _movementComponentsPool.Value.Add(creatureEntity);

                creatureAiStatesCmp.creatureView = _sceneData.Value.GetCreature(curSpawnCmp.spawnObjects[Random.Range(0, curSpawnCmp.spawnObjects.Length)], _sceneData.Value.GetOutOfScreenPosition());
                creatureAiStatesCmp.safeDistance = creatureAiStatesCmp.creatureView.aiCreatureView.safeDistance;
                creatureAiStatesCmp.minSafeDistance = creatureAiStatesCmp.creatureView.aiCreatureView.minSafeDistance;
                creatureAiStatesCmp.followDistance = creatureAiStatesCmp.creatureView.aiCreatureView.followDistance;
                creatureAiStatesCmp.isAttackWhenRetreat = creatureAiStatesCmp.creatureView.aiCreatureView.isAttackWhenRetreat;
                creatureAiStatesCmp.currentState = CreatureAIComponent.CreatureStates.idle;
                creatureAiStatesCmp.isPeaceful = creatureAiStatesCmp.creatureView.aiCreatureView.isPeaceful;


                moveCmp.movementView = creatureAiStatesCmp.creatureView.movementView;
                moveCmp.entityTransform = moveCmp.movementView.objectTransform;
                moveCmp.moveSpeed = moveCmp.movementView.moveSpeed;
                //добавить уравнение всяких оффсетов
                moveCmp.canMove = true;

                if (creatureAiStatesCmp.creatureView.creatureGunView != null)
                {
                   var creatureGunInfo = creatureAiStatesCmp.creatureView.creatureGunView;
                   ref var gunCmp = ref _gunComponentsPool.Value.Add(creatureEntity);
                    ref var attackCmp = ref _currentAttackComponentsPool.Value.Add(creatureEntity);
                    gunCmp.reloadDuration = creatureGunInfo.reloadDuration;
                    gunCmp.isOneBulletReload = creatureGunInfo.isOneBulletReloaded;

                    gunCmp.currentAddedSpread = creatureGunInfo.addedSpread;
                    gunCmp.currentMaxSpread = creatureGunInfo.maxSpread;
                    gunCmp.currentMinSpread = creatureGunInfo.minSpread;
                    gunCmp.currentSpread = creatureGunInfo.minSpread;

                    gunCmp.attackCouldown = creatureGunInfo.attackCouldown;
                    gunCmp.attackLeght = creatureGunInfo.attackLenght;
                    gunCmp.bulletInShotCount = creatureGunInfo.bulletInShotCount;
                    gunCmp.magazineCapacity = creatureGunInfo.magazineCapacity;
                    gunCmp.currentMagazineCapacity = gunCmp.magazineCapacity;
                    gunCmp.spreadRecoverySpeed = creatureGunInfo.spreadRecoverySpeed;
                    gunCmp.firePoint = moveCmp.movementView.firePoint;
                    gunCmp.weaponContainer = moveCmp.movementView.weaponContainer;

                    attackCmp.canAttack = true;
                    attackCmp.damage = creatureGunInfo.damage;
                }

                creatureHealthCmp.healthView = creatureAiStatesCmp.creatureView.healthView;
                creatureHealthCmp.healthView.Construct(creatureEntity);
                creatureHealthCmp.maxHealthPoint = creatureHealthCmp.healthView.maxHealth;
               // creatureHealthCmp.healthPoint = creatureHealthCmp.maxHealthPoint;Debug.Log(creatureHealthCmp.healthPoint + "curEnemyHealth");


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
