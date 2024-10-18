using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
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
    private EcsPoolInject<MeleeWeaponComponent> _meleeWeaponComponentsPool;
    private EcsPoolInject<AttackComponent> _currentAttackComponentsPool;
    private EcsPoolInject<CreatureDropComponent> _creatureDropComponentsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;

    private EcsFilterInject<Inc<ActiveSpawnComponent>> _activeSpawnComponentsFilter;
    private EcsFilterInject<Inc<EntrySpawnZoneEvent>> _entrySpawnZoneEventsFilter;
    private EcsFilterInject<Inc<ExitSpawnZoneEvent>> _exitSpawnZoneEventsFilter;
    private EcsFilterInject<Inc<ChangeToDayEvent>> _changeToDayEventsFilter;
    private EcsFilterInject<Inc<ChangeToNightEvent>> _changeToNightEventsFilter;

    public void Run(IEcsSystems systems)
    {
        foreach (var dayEvent in _changeToDayEventsFilter.Value)
        {
            foreach (var spawnCmpEntity in _activeSpawnComponentsFilter.Value)
            {
                ref var creatureAiStatesCmp = ref _creatureAIComponentsPool.Value.Get(spawnCmpEntity);
                creatureAiStatesCmp.safeDistance = creatureAiStatesCmp.creatureView.aiCreatureView.safeDistance / 1.5f;
                creatureAiStatesCmp.minSafeDistance = creatureAiStatesCmp.creatureView.aiCreatureView.minSafeDistance / 1.5f;
                creatureAiStatesCmp.followDistance = creatureAiStatesCmp.creatureView.aiCreatureView.followDistance / 1.5f;
            }
        }
        foreach (var nightEvent in _changeToNightEventsFilter.Value)
        {
            foreach (var spawnCmpEntity in _activeSpawnComponentsFilter.Value)
            {
                ref var creatureAiStatesCmp = ref _creatureAIComponentsPool.Value.Get(spawnCmpEntity);
                creatureAiStatesCmp.safeDistance = creatureAiStatesCmp.creatureView.aiCreatureView.safeDistance * 1.5f;
                creatureAiStatesCmp.minSafeDistance = creatureAiStatesCmp.creatureView.aiCreatureView.minSafeDistance * 1.5f;
                creatureAiStatesCmp.followDistance = creatureAiStatesCmp.creatureView.aiCreatureView.followDistance * 1.5f;
            }
        }
        foreach (var spawnCmpEntity in _activeSpawnComponentsFilter.Value)
        {
            ref var curSpawnCmp = ref _activeSpawnComponentsPool.Value.Get(spawnCmpEntity);

            curSpawnCmp.curSpawnTime += Time.deltaTime;
            if (curSpawnCmp.curSpawnTime >= curSpawnCmp.spawnTime)
            {
                var gloabalTimeCmp = _globalTimeComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                Debug.Log("spawn");
                curSpawnCmp.curSpawnTime = 0;

                int creatureEntity = _world.Value.NewEntity();
                ref var creatureHealthCmp = ref _healthComponentsPool.Value.Add(creatureEntity);
                _creatureTagsPool.Value.Add(creatureEntity);
                ref var creatureAiStatesCmp = ref _creatureAIComponentsPool.Value.Add(creatureEntity);
                ref var moveCmp = ref _movementComponentsPool.Value.Add(creatureEntity);
                ref var creatureDropCmp = ref _creatureDropComponentsPool.Value.Add(creatureEntity);

                creatureAiStatesCmp.creatureView = _sceneData.Value.GetCreature(curSpawnCmp.currentSpawnCreaturesPool[Random.Range(0, curSpawnCmp.currentSpawnCreaturesPool.Length)], _sceneData.Value.GetOutOfScreenPosition());
                if (!gloabalTimeCmp.isNight)//допустим то что все живности ночью будут свой слух прокачивать
                {
                    creatureAiStatesCmp.safeDistance = creatureAiStatesCmp.creatureView.aiCreatureView.safeDistance;
                    creatureAiStatesCmp.minSafeDistance = creatureAiStatesCmp.creatureView.aiCreatureView.minSafeDistance;
                    creatureAiStatesCmp.followDistance = creatureAiStatesCmp.creatureView.aiCreatureView.followDistance;
                }
                else
                {
                    creatureAiStatesCmp.safeDistance = creatureAiStatesCmp.creatureView.aiCreatureView.safeDistance * 1.5f;
                    creatureAiStatesCmp.minSafeDistance = creatureAiStatesCmp.creatureView.aiCreatureView.minSafeDistance * 1.5f;
                    creatureAiStatesCmp.followDistance = creatureAiStatesCmp.creatureView.aiCreatureView.followDistance * 1.5f;
                }
                creatureAiStatesCmp.isAttackWhenRetreat = creatureAiStatesCmp.creatureView.aiCreatureView.isAttackWhenRetreat;
                creatureAiStatesCmp.currentState = CreatureAIComponent.CreatureStates.idle;
                creatureAiStatesCmp.isPeaceful = creatureAiStatesCmp.creatureView.aiCreatureView.isPeaceful;

                creatureDropCmp.droopedItems = creatureAiStatesCmp.creatureView.dropFromCreatureView.droopedItems;

                moveCmp.movementView = creatureAiStatesCmp.creatureView.movementView;
                moveCmp.entityTransform = moveCmp.movementView.objectTransform;
                moveCmp.moveSpeed = moveCmp.movementView.moveSpeed;
                //добавить уравнение всяких оффсетов
                moveCmp.canMove = true;

                ref var attackCmp = ref _currentAttackComponentsPool.Value.Add(creatureEntity);

                if (creatureAiStatesCmp.creatureView.creatureGunView != null)
                {
                    var creatureGunInfo = creatureAiStatesCmp.creatureView.creatureGunView;
                    ref var gunCmp = ref _gunComponentsPool.Value.Add(creatureEntity);
                    gunCmp.reloadDuration = creatureGunInfo.reloadDuration;
                    gunCmp.isOneBulletReload = creatureGunInfo.isOneBulletReloaded;

                    gunCmp.currentAddedSpread = creatureGunInfo.addedSpread;
                    gunCmp.currentMaxSpread = creatureGunInfo.maxSpread;
                    gunCmp.currentMinSpread = creatureGunInfo.minSpread;
                    gunCmp.currentSpread = creatureGunInfo.minSpread;

                    attackCmp.attackCouldown = creatureGunInfo.attackCouldown;
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

                else if (creatureAiStatesCmp.creatureView.creatureMeleeView != null)
                {
                    creatureAiStatesCmp.creatureView.creatureMeleeView.meleeWeaponColliderView.Construct(_world.Value, creatureEntity);
                    /*ref var meleeCmp = ref */
                    _meleeWeaponComponentsPool.Value.Add(creatureEntity);

                    attackCmp.canAttack = true;
                    attackCmp.damage = creatureAiStatesCmp.creatureView.creatureMeleeView.damage;
                    attackCmp.attackCouldown = creatureAiStatesCmp.creatureView.creatureMeleeView.attackCouldown;
                }

                creatureHealthCmp.healthView = creatureAiStatesCmp.creatureView.healthView;
                creatureHealthCmp.healthView.Construct(creatureEntity);
                creatureHealthCmp.maxHealthPoint = creatureHealthCmp.healthView.maxHealth;
                creatureHealthCmp.healthPoint = creatureHealthCmp.maxHealthPoint; Debug.Log(creatureHealthCmp.healthPoint + "curEnemyHealth");


            }
        }

        foreach (var entryZoneEvent in _entrySpawnZoneEventsFilter.Value)
        {
            var currentTimeCmp = _globalTimeComponentsPool.Value.Get(_sceneData.Value.playerEntity);
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
            if (!currentTimeCmp.isNight)
                spawnCmp.currentSpawnCreaturesPool = zone.daySpawnCreaturesPool;
            else
                spawnCmp.currentSpawnCreaturesPool = zone.nightSpawnCreaturesPool;
        }

        foreach (var exitZoneEvent in _exitSpawnZoneEventsFilter.Value)
        {
            ref var zone = ref _exitCollisionWithSpawnZoneEventsPool.Value.Get(exitZoneEvent).zoneView;

            _activeSpawnComponentsPool.Value.Del(zone._entity);
        }
    }
}
