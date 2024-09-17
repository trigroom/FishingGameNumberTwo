using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class AttackSystem : IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<CurrentAttackComponent> _attackComponentsPool;
    //private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    //private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<BulletTracerLifetimeComponent> _bulletTracerLifetimeComponentsPool;
    private EcsPoolInject<EndReloadEvent> _endReloadEventsPool;
    private EcsPoolInject<ReloadEvent> _reloadEventsPool;

    private EcsFilterInject<Inc<EndReloadEvent>> _endReloadEventFilter;
    private EcsFilterInject<Inc<PlayerComponent>> _playerComponentFilter;
    private EcsFilterInject<Inc<BulletTracerLifetimeComponent>> _bulletTracerLifetimeComponentFilter;
    public void Run(IEcsSystems systems)
    {
        foreach (var playerEntity in _playerComponentFilter.Value)
        {
            ref var gunCmp = ref _gunComponentsPool.Value.Get(playerEntity);
            ref var attackCmp = ref _attackComponentsPool.Value.Get(playerEntity);

            foreach (var reloadEvt in _endReloadEventFilter.Value)
            {
                Debug.Log("Reload gun");
                gunCmp.isReloading = true;
                _sceneData.Value.ammoInfoText.text = "перезарядка...";
                _endReloadEventsPool.Value.Del(reloadEvt);
            }

            gunCmp.currentAttackCouldown += Time.deltaTime;
            if (((gunCmp.isAuto && Input.GetMouseButton(0)) || (!gunCmp.isAuto && Input.GetMouseButtonDown(0))) && !gunCmp.isReloading && gunCmp.currentMagazineCapacity > 0 && gunCmp.currentAttackCouldown >= gunCmp.attackCouldown && !attackCmp.weaponIsChanged)//потом для авто стрельбы сделать отдельную проверку isAuto && GetMouseButton || !isAuto && GetMouseButtonDown
            {
                for (int i = 0; i < gunCmp.bulletCount; i++)
                    Shoot(playerEntity, LayerMask.GetMask("Enemy"));

                if (gunCmp.currentSpread < gunCmp.maxSpread)
                    gunCmp.currentSpread += gunCmp.addedSpread;

                gunCmp.currentAttackCouldown = 0;
                gunCmp.currentMagazineCapacity--;
                _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
                if (gunCmp.currentMagazineCapacity == 0)
                {
                    _reloadEventsPool.Value.Add(playerEntity);
                }
            }

            else if (gunCmp.isReloading)
            {
                gunCmp.currentReloadDuration += Time.deltaTime;

                if (gunCmp.currentReloadDuration >= gunCmp.reloadDuration)
                {
                    gunCmp.currentReloadDuration = 0;
                    gunCmp.currentMagazineCapacity += gunCmp.bulletCountToReload;
                    _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
                    gunCmp.isReloading = false;
                }
            }

            else if (Input.GetKeyDown(KeyCode.R) && gunCmp.currentMagazineCapacity != gunCmp.magazineCapacity)
            {
                Debug.Log("try reload");
                _reloadEventsPool.Value.Add(playerEntity);
            }

            else if (!gunCmp.isReloading)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    ChangeWeapon(0);
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    ChangeWeapon(1);
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    ChangeWeapon(2);
            }

            if (attackCmp.weaponIsChanged)
            {
                attackCmp.currentChangeWeaponTime += Time.deltaTime;
                if (attackCmp.currentChangeWeaponTime >= attackCmp.changeWeaponTime)
                {
                    attackCmp.currentChangeWeaponTime = 0;
                    attackCmp.weaponIsChanged = false;
                    _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
                }
            }

            if (gunCmp.currentSpread > gunCmp.minSpread)
                gunCmp.currentSpread -= gunCmp.spreadRecoverySpeed * Time.deltaTime;

            else if (gunCmp.currentSpread < gunCmp.minSpread)
                gunCmp.currentSpread = gunCmp.minSpread;

        }


        CheckBulletTracerLife();
        //для других сущностей отдельно
    }



    private void ChangeWeapon(int curWeapon)
    {
        ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        if (weaponsInInventoryCmp.curWeapon != curWeapon)
        {
            if ((curWeapon == 2 && weaponsInInventoryCmp.meleeWeaponObject == null) || (curWeapon == 1 && weaponsInInventoryCmp.gunSecondObject == null) || (curWeapon == 0 && weaponsInInventoryCmp.gunFirstObject == null))
                return;

            ref var attackCmp = ref _attackComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (curWeapon <= 1)
            {
                ref var gunCmp = ref _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);

                if (weaponsInInventoryCmp.curWeapon == 0)
                    weaponsInInventoryCmp.curFirstWeaponAmmo = gunCmp.currentMagazineCapacity;

                if (weaponsInInventoryCmp.curWeapon == 1)
                    weaponsInInventoryCmp.curSecondWeaponAmmo = gunCmp.currentMagazineCapacity;

                if (curWeapon == 0)
                {
                    weaponsInInventoryCmp.curWeapon = curWeapon;
                    attackCmp.changeWeaponTime = weaponsInInventoryCmp.gunFirstObject.weaponChangeSpeed;
                    gunCmp.reloadDuration = weaponsInInventoryCmp.gunFirstObject.reloadDuration;
                    gunCmp.currentMagazineCapacity = weaponsInInventoryCmp.curFirstWeaponAmmo;
                    gunCmp.magazineCapacity = weaponsInInventoryCmp.gunFirstObject.magazineCapacity;
                    gunCmp.maxSpread = weaponsInInventoryCmp.gunFirstObject.maxSpread;
                    gunCmp.minSpread = weaponsInInventoryCmp.gunFirstObject.minSpread;
                    gunCmp.currentSpread = weaponsInInventoryCmp.gunFirstObject.minSpread;//временно так
                    gunCmp.spreadRecoverySpeed = weaponsInInventoryCmp.gunFirstObject.spreadRecoverySpeed;
                    gunCmp.addedSpread = weaponsInInventoryCmp.gunFirstObject.addedSpread;
                    gunCmp.isAuto = weaponsInInventoryCmp.gunFirstObject.isAuto;
                    gunCmp.bulletCount = weaponsInInventoryCmp.gunFirstObject.bulletCount;
                    gunCmp.bulletTypeId = weaponsInInventoryCmp.gunFirstObject.bulletTypeId;

                    attackCmp.damage = weaponsInInventoryCmp.gunFirstObject.damage;

                    //менять модельку оружия
                    //переместить всё что в скобках в отдельный метод
                }

                else
                {
                    weaponsInInventoryCmp.curWeapon = curWeapon;
                    attackCmp.changeWeaponTime = weaponsInInventoryCmp.gunSecondObject.weaponChangeSpeed;
                    gunCmp.reloadDuration = weaponsInInventoryCmp.gunSecondObject.reloadDuration;
                    gunCmp.currentMagazineCapacity = weaponsInInventoryCmp.curSecondWeaponAmmo;
                    gunCmp.magazineCapacity = weaponsInInventoryCmp.gunSecondObject.magazineCapacity;
                    gunCmp.maxSpread = weaponsInInventoryCmp.gunSecondObject.maxSpread;
                    gunCmp.minSpread = weaponsInInventoryCmp.gunSecondObject.minSpread;
                    gunCmp.currentSpread = weaponsInInventoryCmp.gunSecondObject.minSpread;//временно так
                    gunCmp.spreadRecoverySpeed = weaponsInInventoryCmp.gunSecondObject.spreadRecoverySpeed;
                    gunCmp.addedSpread = weaponsInInventoryCmp.gunSecondObject.addedSpread;
                    gunCmp.isAuto = weaponsInInventoryCmp.gunSecondObject.isAuto;
                    gunCmp.bulletCount = weaponsInInventoryCmp.gunSecondObject.bulletCount;
                    gunCmp.bulletTypeId = weaponsInInventoryCmp.gunSecondObject.bulletTypeId;

                    attackCmp.damage = weaponsInInventoryCmp.gunSecondObject.damage;
                }

            }

            if (curWeapon == 2)
            {
                //смена на ближнее оружие
            }

            _sceneData.Value.ammoInfoText.text = "смена оружия";
            attackCmp.weaponIsChanged = true;

        }
    }
    private void Shoot(int currentEntity, LayerMask targetLayer)// 6 маска игрока 7 враг
    {
        ref var attackCmp = ref _attackComponentsPool.Value.Get(currentEntity);
        ref var gunCmp = ref _gunComponentsPool.Value.Get(currentEntity);
        gunCmp.firePoint.rotation = gunCmp.weaponContainer.rotation * Quaternion.Euler(0, 0, Random.Range(-gunCmp.currentSpread, gunCmp.currentSpread));//некорректный расчёт разброса, переделать

        Debug.Log(gunCmp.currentSpread + " curSpread");
        var targets = Physics2D.RaycastAll(gunCmp.firePoint.position, gunCmp.firePoint.up, gunCmp.attackLeght, targetLayer);
        //сделать чтобы игрок поворачивал оружие
        if (targets.Length == 0)
        {
            var tracer = CreateTracer();
            tracer.SetPosition(0, gunCmp.firePoint.position);

            var ray = new Ray2D(gunCmp.firePoint.position, gunCmp.firePoint.up);

            tracer.SetPosition(1, ray.origin + (ray.direction * 20));
            return;
        }

        else
        {
            int damageReminder = attackCmp.damage;


            foreach (var target in targets)
            {
                ref var health = ref _healthComponentsPool.Value.Get(target.collider.gameObject.GetComponent<HealthView>()._entity);
                int startedHealth = health.healthPoint;
                health.healthPoint -= damageReminder;
                if (health.healthPoint <= 0)
                {
                    Debug.Log(health.healthPoint + "hp and death");
                    health.healthView.Death();
                    _world.Value.DelEntity(target.collider.gameObject.GetComponent<HealthView>()._entity);
                    damageReminder -= startedHealth;
                    continue;
                }

                else
                {
                    Debug.Log(health.healthPoint + "hp");
                    var tracer = CreateTracer();
                    tracer.SetPosition(0, gunCmp.firePoint.position);
                    tracer.SetPosition(1, target.point);
                    return;
                }
            }


            var tracer2 = CreateTracer();
            var ray2 = new Ray2D(gunCmp.firePoint.position, gunCmp.firePoint.up);

            tracer2.SetPosition(0, gunCmp.firePoint.position);
            tracer2.SetPosition(1, ray2.origin + (ray2.direction * 20));
        }

    }

    private LineRenderer CreateTracer()
    {
        var tracerEntity = _world.Value.NewEntity();

        ref var lifetimeCmp = ref _bulletTracerLifetimeComponentsPool.Value.Add(tracerEntity);
        lifetimeCmp.lifetime = 2f;
        lifetimeCmp.lineRenderer = _sceneData.Value.GetBulletTracer();

        return lifetimeCmp.lineRenderer;
    }

    private void CheckBulletTracerLife()
    {
        foreach (var bulletTracerEntity in _bulletTracerLifetimeComponentFilter.Value)
        {
            ref var lifetimeCmp = ref _bulletTracerLifetimeComponentsPool.Value.Get(bulletTracerEntity);
            Color tracerColor = new Color(255, 255, 255, lifetimeCmp.lifetime / 2);
            lifetimeCmp.lineRenderer.startColor = tracerColor;
            lifetimeCmp.lineRenderer.endColor = tracerColor;
            lifetimeCmp.lifetime -= Time.deltaTime;
            if (lifetimeCmp.lifetime > 0)
                continue;
            _sceneData.Value.ReleaseBulletTracer(lifetimeCmp.lineRenderer);

            _world.Value.DelEntity(bulletTracerEntity);
        }
    }
}
