using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class SpawnSystem : IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<ActiveSpawnComponent> _activeSpawnComponentsPool; //����� ����������� ����� �������� � ��������� �� ������� �� ������������
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<EntrySpawnZoneEvent> _entrySpawnZoneEventsPool;
    private EcsPoolInject<ExitSpawnZoneEvent> _exitCollisionWithSpawnZoneEventsPool;
    private EcsPoolInject<CreatureTag> _creatureTagsPool;
    private EcsPoolInject<SpawnZoneTag> _spawnZoneTagsPool;
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<MeleeWeaponComponent> _meleeWeaponComponentsPool;
    private EcsPoolInject<AttackComponent> _currentAttackComponentsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<HealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<CreatureInventoryComponent> _creatureInventoryComponentsPool;
    private EcsPoolInject<CurrentLocationComponent> _currentLocationComponentsPool;
    private EcsPoolInject<HidedObjectOutsideFOVComponent> _hidedObjectOutsideFOVComponentsPool;

    private EcsFilterInject<Inc<ActiveSpawnComponent>> _activeSpawnComponentsFilter;
    private EcsFilterInject<Inc<EntrySpawnZoneEvent>> _entrySpawnZoneEventsFilter;
    private EcsFilterInject<Inc<ExitSpawnZoneEvent>> _exitSpawnZoneEventsFilter;
    private EcsFilterInject<Inc<EnemySpawnEvent>> _enemySpawnEventsFilter;
    private EcsFilterInject<Inc<ChangeToDayEvent>> _changeToDayEventsFilter;
    private EcsFilterInject<Inc<ChangeToNightEvent>> _changeToNightEventsFilter;
    private EcsFilterInject<Inc<CreatureAIComponent>> _creatureAIComponentsFilter;


    public void Run(IEcsSystems systems)
    {
        foreach (var enemySpawn in _enemySpawnEventsFilter.Value)
        {
            ref var curLocationCmp = ref _currentLocationComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            int creatureEntity = _world.Value.NewEntity();
            ref var creatureAiStatesCmp = ref _creatureAIComponentsPool.Value.Add(creatureEntity);
            int needEnemySpawnPositionIndex = Random.Range(0, curLocationCmp.currentEnemySpawns.Count);
            creatureAiStatesCmp.creatureView = _sceneData.Value.GetCreature(_sceneData.Value.defaultEnemy, curLocationCmp.currentEnemySpawns[needEnemySpawnPositionIndex]);
            creatureAiStatesCmp.creatureView.gameObject.transform.SetParent(curLocationCmp.currentLevelPrefab);
            curLocationCmp.currentEnemySpawns.RemoveAt(needEnemySpawnPositionIndex);
            CreatureSpawn(creatureEntity, ref creatureAiStatesCmp);
        }

    /*    foreach (var entryZoneEvent in _entrySpawnZoneEventsFilter.Value)
        {
            var currentTimeCmp = _globalTimeComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            ref var zone = ref _entrySpawnZoneEventsPool.Value.Get(entryZoneEvent).zoneView;
            //�������� ����� ��� ���� ������, ����� ������ �� ���������
            int spawnCmpEntity = -1;
            if (zone._entity == -1)
            {
                spawnCmpEntity = _world.Value.NewEntity();
                zone.Construct(spawnCmpEntity);
                _spawnZoneTagsPool.Value.Add(spawnCmpEntity);
            }
            else
                spawnCmpEntity = zone._entity;

            if (!_activeSpawnComponentsPool.Value.Has(spawnCmpEntity))
            {
                ref var spawnCmp = ref _activeSpawnComponentsPool.Value.Add(spawnCmpEntity);
                spawnCmp.zoneView = zone;
                spawnCmp.spawnTime = zone.spawnTime;
                if (!currentTimeCmp.isNight)
                    spawnCmp.currentSpawnCreaturesPool = zone.daySpawnCreaturesPool;
                else
                    spawnCmp.currentSpawnCreaturesPool = zone.nightSpawnCreaturesPool;
            }
        }

        foreach (var exitZoneEvent in _exitSpawnZoneEventsFilter.Value)
        {
            ref var zone = ref _exitCollisionWithSpawnZoneEventsPool.Value.Get(exitZoneEvent).zoneView;

            _activeSpawnComponentsPool.Value.Del(zone._entity);
        }*/
    }
    private void CreatureSpawn(int creatureEntity, ref CreatureAIComponent creatureAiStatesCmp)
    {
        ref var curLocationCmp = ref _currentLocationComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        var curLevel = curLocationCmp.currentLocation.levels[curLocationCmp.levelNum - 1];

        ref var moveCmp = ref _movementComponentsPool.Value.Add(creatureEntity);
        ref var creatureInventoryCmp = ref _creatureInventoryComponentsPool.Value.Add(creatureEntity);
        ref var attackCmp = ref _currentAttackComponentsPool.Value.Add(creatureEntity);

        float chanceToEnemyClass = Random.value;
        int curClassIndex = 0;
        foreach (var chance in curLevel.chancesToEnemyClass)
        {
            if (chance > chanceToEnemyClass)
            {
                creatureInventoryCmp.enemyClassSettingInfo = curLevel.enemyClassSettingsInfo[curClassIndex];
                break;
            }
            curClassIndex++;
        }

        if (curLevel.chanceToOneWeapon > Random.value)
        {
            if (curLevel.chanceToMeleeWeapon > Random.value)
                creatureInventoryCmp.meleeWeaponItem = curLevel.meleeWeaponsInfo[Random.Range(0, curLevel.meleeWeaponsInfo.Length)];
            else
                creatureInventoryCmp.gunItem = curLevel.gunsInfo[Random.Range(0, curLevel.gunsInfo.Length)];
        }
        else
        {
            creatureAiStatesCmp.isTwoWeapon = true;
            creatureInventoryCmp.meleeWeaponItem = curLevel.meleeWeaponsInfo[Random.Range(0, curLevel.meleeWeaponsInfo.Length)];
            creatureInventoryCmp.gunItem = curLevel.gunsInfo[Random.Range(0, curLevel.gunsInfo.Length)];
        }
        if (curLevel.chanceToHealingItem > Random.value)
            creatureInventoryCmp.healingItem = curLevel.healingItemsInfo[Random.Range(0, curLevel.healingItemsInfo.Length)];
        if (curLevel.chanceToHelmetItem > Random.value)
            creatureInventoryCmp.helmetItem = curLevel.helmetsInfo[Random.Range(0, curLevel.helmetsInfo.Length)];
        if (curLevel.chanceToBodyArmorItem > Random.value)
            creatureInventoryCmp.bodyArmorItem = curLevel.bodyArmorsInfo[Random.Range(0, curLevel.bodyArmorsInfo.Length)];
        if (curLevel.chanceToShieldItem > Random.value)
            creatureInventoryCmp.shieldItem = curLevel.shieldItemsInfo[Random.Range(0, curLevel.shieldItemsInfo.Length)];

        var globalTimeCmp = _globalTimeComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        ref var creatureHealthCmp = ref _healthComponentsPool.Value.Add(creatureEntity);
        _creatureTagsPool.Value.Add(creatureEntity);
        creatureAiStatesCmp.reachedLastTarget = true;
        creatureAiStatesCmp.creatureView.entity = creatureEntity;
        float visionZoneMultiplayer = 1;
        if (creatureInventoryCmp.helmetItem != null && creatureInventoryCmp.helmetItem.helmetInfo.addedLightIntancity != 0)
            visionZoneMultiplayer = 0.85f;
        else if (globalTimeCmp.isNight)
            visionZoneMultiplayer = 0.4f + globalTimeCmp.currentGlobalLightIntensity;

        visionZoneMultiplayer *= creatureInventoryCmp.enemyClassSettingInfo.visionLenghtMultiplayer;
        _hidedObjectOutsideFOVComponentsPool.Value.Add(creatureEntity).hidedObjects = creatureAiStatesCmp.creatureView.gameObject.GetComponent<HidedOutsidePlayerFovView>().objectsToHide/*�������� ���� ��� �� ��� ����������� ���� ������ ������*/  ;

        if (creatureInventoryCmp.gunItem != null)
        {
            creatureAiStatesCmp.safeDistance = (4+creatureInventoryCmp.gunItem.gunInfo.attackLenght / 8) * visionZoneMultiplayer;
            if (creatureInventoryCmp.meleeWeaponItem != null)
                creatureAiStatesCmp.minSafeDistance = 1.8f;
            else
                creatureAiStatesCmp.minSafeDistance = creatureAiStatesCmp.safeDistance * 0.5f;
            creatureAiStatesCmp.followDistance = creatureAiStatesCmp.safeDistance * 1.5f;
        }
        else
        {
            creatureAiStatesCmp.safeDistance = 5 * visionZoneMultiplayer;
            creatureAiStatesCmp.minSafeDistance = 1.8f;
            creatureAiStatesCmp.followDistance = 12 * visionZoneMultiplayer;
        }

        Debug.Log("follow " + creatureAiStatesCmp.followDistance + " safe " + creatureAiStatesCmp.safeDistance + " min " + creatureAiStatesCmp.minSafeDistance);

        creatureAiStatesCmp.needSightOnTargetTime = creatureInventoryCmp.enemyClassSettingInfo.needSightOnTargetTime;
        creatureAiStatesCmp.isAttackWhenRetreat = creatureAiStatesCmp.creatureView.aiCreatureView.isAttackWhenRetreat;
        //creatureAiStatesCmp.currentState = CreatureAIComponent.CreatureStates.idle;//

        creatureAiStatesCmp.currentTarget = _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).entityTransform;
        moveCmp.movementView = creatureAiStatesCmp.creatureView.movementView;
        moveCmp.entityTransform = moveCmp.movementView.objectTransform;

        float distanceBetweenTarget = Vector2.Distance(moveCmp.entityTransform.position, creatureAiStatesCmp.currentTarget.position);

        if (distanceBetweenTarget > creatureAiStatesCmp.followDistance)
            creatureAiStatesCmp.currentState = CreatureAIComponent.CreatureStates.idle;
        else if (distanceBetweenTarget > creatureAiStatesCmp.safeDistance)
            creatureAiStatesCmp.currentState = CreatureAIComponent.CreatureStates.follow;
        else if (distanceBetweenTarget > creatureAiStatesCmp.minSafeDistance)
            creatureAiStatesCmp.currentState = CreatureAIComponent.CreatureStates.shootingToTarget;
        else
            creatureAiStatesCmp.currentState = CreatureAIComponent.CreatureStates.runAwayFromTarget;

        creatureAiStatesCmp.isPeaceful = creatureAiStatesCmp.creatureView.aiCreatureView.isPeaceful;

        moveCmp.moveSpeed = creatureInventoryCmp.enemyClassSettingInfo.movementSpeed;
        moveCmp.maxRunTime = creatureInventoryCmp.enemyClassSettingInfo.staminaCount;
        moveCmp.currentRunTime = moveCmp.maxRunTime;
        moveCmp.currentRunTimeRecoverySpeed = 1;
        //�������� ��������� ������ ��������
        moveCmp.canMove = true;

        if(creatureInventoryCmp.shieldItem != null)
        {
            moveCmp.movementView.shieldView.shieldSpriteRenderer.sprite = creatureInventoryCmp.shieldItem.sheildInfo.sheildSprite;
            moveCmp.movementView.shieldView.shieldCollider.size = creatureInventoryCmp.shieldItem.sheildInfo.sheildColliderScale;
            moveCmp.movementView.shieldView.shieldCollider.enabled = true;
        }

        if (creatureInventoryCmp.healingItem != null)
        {
            ref var healItemCmp = ref _currentHealingItemComponentsPool.Value.Add(creatureEntity);
            healItemCmp.healingItemInfo = creatureInventoryCmp.healingItem.healInfo;
        }

        if (creatureInventoryCmp.gunItem != null)
        {
            var creatureGunInfo = creatureInventoryCmp.gunItem.gunInfo;
            ref var gunCmp = ref _gunComponentsPool.Value.Add(creatureEntity);
            gunCmp.reloadDuration = creatureGunInfo.reloadDuration;
            gunCmp.isOneBulletReload = creatureGunInfo.isOneBulletReloaded;

            gunCmp.currentAddedSpread = creatureGunInfo.addedSpread;
            gunCmp.currentMaxSpread = creatureGunInfo.maxSpread;
            gunCmp.currentMinSpread = creatureGunInfo.minSpread;
            gunCmp.currentSpread = creatureGunInfo.minSpread;

            gunCmp.attackLeght = creatureGunInfo.attackLenght;
            gunCmp.bulletInShotCount = creatureGunInfo.bulletCount;
            gunCmp.magazineCapacity = creatureGunInfo.magazineCapacity;
            gunCmp.currentMagazineCapacity = creatureGunInfo.magazineCapacity;
            gunCmp.spreadRecoverySpeed = 2 * creatureInventoryCmp.enemyClassSettingInfo.recoverySpreadMultiplayer;
            gunCmp.firePoint = moveCmp.movementView.firePoint;
            gunCmp.weaponContainer = moveCmp.movementView.weaponContainer;
            gunCmp.lightFromGunShot = creatureAiStatesCmp.creatureView.aiCreatureView.lightFromGunShot;
            gunCmp.flashShotInstance = creatureInventoryCmp.gunItem.gunInfo.shotFlashIntance;

            attackCmp.attackCouldown = creatureGunInfo.attackCouldown;
            attackCmp.canAttack = true;
            attackCmp.damage = creatureGunInfo.damage;

            creatureAiStatesCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = creatureGunInfo.weaponSprite;
            creatureAiStatesCmp.creatureView.aiCreatureView.itemTransform.localScale = new Vector3(1, -1, 1) * creatureGunInfo.spriteScaleMultiplayer;
            creatureAiStatesCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, creatureGunInfo.spriteRotation);

            attackCmp.weaponRotateSpeed = (30f / (attackCmp.damage * creatureInventoryCmp.gunItem.gunInfo.bulletCount) + 1.5f) * creatureInventoryCmp.enemyClassSettingInfo.weaponRotationSpeedMultiplayer;

           moveCmp.movementView.bulletShellSpawnPoint.localPosition = creatureGunInfo.bulletShellPointPosition;
        }

        if (creatureInventoryCmp.meleeWeaponItem != null)
        {
            creatureAiStatesCmp.creatureView.meleeWeaponColliderView.Construct(_world.Value, creatureEntity);
            /*ref var meleeCmp = ref */
            _meleeWeaponComponentsPool.Value.Add(creatureEntity).isWideAttack = Random.value > 0.5f;

            if (creatureInventoryCmp.gunItem == null)
            {
                var meleeWeapon = creatureInventoryCmp.meleeWeaponItem.meleeWeaponInfo;
                attackCmp.canAttack = true;
                attackCmp.damage = meleeWeapon.damage;
                attackCmp.attackCouldown = meleeWeapon.attackCouldown;

                creatureAiStatesCmp.creatureView.aiCreatureView.itemSpriteRenderer.sprite = meleeWeapon.weaponSprite;

                creatureAiStatesCmp.creatureView.aiCreatureView.itemTransform.localScale = new Vector3(1, -1, 1) * meleeWeapon.spriteScaleMultiplayer;
                creatureAiStatesCmp.creatureView.aiCreatureView.itemTransform.localEulerAngles = new Vector3(0, 0, meleeWeapon.spriteRotation);

                attackCmp.weaponRotateSpeed = (50f / attackCmp.damage+1) * creatureInventoryCmp.enemyClassSettingInfo.weaponRotationSpeedMultiplayer;
            }
            else if (creatureAiStatesCmp.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget)
                creatureInventoryCmp.isSecondWeaponUsed = true;

        }

        var randomHair = _sceneData.Value.johnHairsSprites[Random.Range(0, _sceneData.Value.johnHairsSprites.Length)];

        if (creatureInventoryCmp.bodyArmorItem != null)
        {
            moveCmp.movementView.bodyArmorSpriteRenderer.transform.localPosition = creatureInventoryCmp.bodyArmorItem.bodyArmorInfo.inGamePositionOnPlayer;
            moveCmp.movementView.bodyArmorSpriteRenderer.sprite = creatureInventoryCmp.bodyArmorItem.bodyArmorInfo.bodyArmorSprite;
        }
        if (creatureInventoryCmp.helmetItem != null)
        {
            var helmetItem = creatureInventoryCmp.helmetItem.helmetInfo;
            moveCmp.movementView.helmetSpriteRenderer.transform.localPosition = helmetItem.inGamePositionOnPlayer;
            moveCmp.movementView.helmetSpriteRenderer.sprite = helmetItem.helmetSprite;
            moveCmp.movementView.hairSpriteRenderer.sprite = randomHair.sprites[helmetItem.hairSpriteIndex];
        }
        else
            moveCmp.movementView.hairSpriteRenderer.sprite = randomHair.sprites[1];
        moveCmp.movementView.hairSpriteRenderer.color = _sceneData.Value.hairsColors[Random.Range(0, _sceneData.Value.hairsColors.Length)];

        creatureHealthCmp.healthView = creatureAiStatesCmp.creatureView.healthView;
        creatureHealthCmp.healthView.Construct(creatureEntity);
        creatureHealthCmp.maxHealthPoint = creatureInventoryCmp.enemyClassSettingInfo.healthPoint;
        creatureHealthCmp.healthPoint = creatureHealthCmp.maxHealthPoint; Debug.Log(creatureHealthCmp.healthPoint + "curEnemyHealth");
    }
}
