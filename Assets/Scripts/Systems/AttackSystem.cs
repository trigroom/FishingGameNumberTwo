using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using UnityEngine;

public class AttackSystem : IEcsRunSystem, IEcsInitSystem
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
    private EcsPoolInject<ChangeHealthEvent> _changeHealthEventsPool;
    private EcsPoolInject<ChangeWeaponFromInventoryEvent> _changeWeaponFromInventoryEventsPool;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentsPool;

    private EcsFilterInject<Inc<EndReloadEvent>> _endReloadEventFilter;
    private EcsFilterInject<Inc<PlayerComponent>> _playerComponentFilter;
    private EcsFilterInject<Inc<GunComponent, PlayerComponent>> _gunComponentsFilter;
    private EcsFilterInject<Inc<BulletTracerLifetimeComponent>> _bulletTracerLifetimeComponentFilter;
    private EcsFilterInject<Inc<ChangeWeaponFromInventoryEvent>> _changeWeaponFromInventoryEventsFilter;
    public void Init(IEcsSystems systems)
    {
        var gunCmp = _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        _sceneData.Value.ammoInfoText.text = gunCmp.currentMagazineCapacity + "/" + gunCmp.magazineCapacity;
    }
    public void Run(IEcsSystems systems)
    {
        foreach (var changeWeaponFromInvEvt in _changeWeaponFromInventoryEventsFilter.Value)
        {
            ref var changeWeaponFromInvCmp = ref _changeWeaponFromInventoryEventsPool.Value.Get(changeWeaponFromInvEvt);
            if (changeWeaponFromInvCmp.weaponCellNumberToChange < 2)
            {
                ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(changeWeaponFromInvEvt);

                if (changeWeaponFromInvCmp.isDeleteWeapon)
                {
                    if (changeWeaponFromInvCmp.weaponCellNumberToChange == 0)//first gun
                    {
                        gunInInvCmp.currentAmmo = _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity).currentMagazineCapacity;//
                        weaponsInInventoryCmp.gunFirstObject = null;
                        Debug.Log("first null");
                    }

                    else //second gun
                    {
                        gunInInvCmp.currentAmmo = _gunComponentsPool.Value.Get(_sceneData.Value.playerEntity).currentMagazineCapacity;//
                        weaponsInInventoryCmp.gunSecondObject = null;
                        Debug.Log("second null");
                    }

                    if (0 != changeWeaponFromInvCmp.weaponCellNumberToChange && weaponsInInventoryCmp.gunFirstObject != null)
                        ChangeWeapon(0);
                    else if (1 != changeWeaponFromInvCmp.weaponCellNumberToChange && weaponsInInventoryCmp.gunSecondObject != null)
                        ChangeWeapon(1);
                    else
                        ChangeWeapon(2);
                }
                else
                {
                    //gunInInvCmp.gunInfo = _inventoryItemComponentsPool.Value.Get(changeWeaponFromInvEvt).itemInfo.gunInfo;//потом добавть в место где оружие будет напрямую добавляться из сохранения в быстрый слот в начале игры
                    if (changeWeaponFromInvCmp.weaponCellNumberToChange == 0)
                    {
                        weaponsInInventoryCmp.gunFirstObject = gunInInvCmp.gunInfo;
                        weaponsInInventoryCmp.curFirstWeaponAmmo = gunInInvCmp.currentAmmo;//
                        Debug.Log("first full");
                    }
                    else
                    {
                        weaponsInInventoryCmp.gunSecondObject = gunInInvCmp.gunInfo;
                        weaponsInInventoryCmp.curSecondWeaponAmmo = gunInInvCmp.currentAmmo;//
                        Debug.Log("second full");
                    }

                    ChangeWeapon(changeWeaponFromInvCmp.weaponCellNumberToChange);
                }

            }
            _changeWeaponFromInventoryEventsPool.Value.Del(changeWeaponFromInvEvt);
            //если милишка
        }


        foreach (var playerEntity in _playerComponentFilter.Value)
        {
            ref var gunCmp = ref _gunComponentsPool.Value.Get(playerEntity);
            ref var attackCmp = ref _attackComponentsPool.Value.Get(playerEntity);
            foreach (var reloadEvt in _endReloadEventFilter.Value)
            {
                gunCmp.isReloading = true;
                _sceneData.Value.ammoInfoText.text = "перезарядка...";
                _endReloadEventsPool.Value.Del(reloadEvt);
            }
            // Debug.Log((!gunCmp.isReloading) +"&&"+( gunCmp.currentMagazineCapacity > 0 )+"&&"+ (gunCmp.currentAttackCouldown >= gunCmp.attackCouldown) +"&& "+!attackCmp.weaponIsChanged +"&&"+ attackCmp.canAttack);
            gunCmp.currentAttackCouldown += Time.deltaTime;
            //  Debug.Log("cur couldown" + gunCmp.currentAttackCouldown);
            if (((gunCmp.isAuto && Input.GetMouseButton(0)) || (!gunCmp.isAuto && Input.GetMouseButtonDown(0))) && !gunCmp.isReloading && gunCmp.currentMagazineCapacity > 0 && gunCmp.currentAttackCouldown >= gunCmp.attackCouldown && !attackCmp.weaponIsChanged && attackCmp.canAttack)//потом для авто стрельбы сделать отдельную проверку isAuto && GetMouseButton || !isAuto && GetMouseButtonDown
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
                    ChangeWeaponStats(weaponsInInventoryCmp.gunFirstObject, weaponsInInventoryCmp.curFirstWeaponAmmo, ref gunCmp, ref attackCmp);

                    //менять модельку оружия
                    //переместить всё что в скобках в отдельный метод
                }

                else
                {
                    weaponsInInventoryCmp.curWeapon = curWeapon;
                    ChangeWeaponStats(weaponsInInventoryCmp.gunSecondObject, weaponsInInventoryCmp.curSecondWeaponAmmo, ref gunCmp, ref attackCmp);
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

    private void ChangeWeaponStats(GunInfo gunInfo, int curBullets, ref GunComponent gunCmp, ref CurrentAttackComponent curAttackCmp)
    {
        curAttackCmp.changeWeaponTime = gunInfo.weaponChangeSpeed;
        gunCmp.reloadDuration = gunInfo.reloadDuration;
        gunCmp.attackLeght = gunInfo.attackLenght;
        gunCmp.currentMagazineCapacity = curBullets;
        gunCmp.magazineCapacity = gunInfo.magazineCapacity;
        gunCmp.maxSpread = gunInfo.maxSpread;
        gunCmp.minSpread = gunInfo.minSpread;
        gunCmp.currentSpread = gunInfo.minSpread;
        gunCmp.spreadRecoverySpeed = gunInfo.spreadRecoverySpeed;
        gunCmp.addedSpread = gunInfo.addedSpread;
        gunCmp.isAuto = gunInfo.isAuto;
        gunCmp.bulletCount = gunInfo.bulletCount;
        gunCmp.bulletTypeId = gunInfo.bulletTypeId;

        curAttackCmp.damage = gunInfo.damage;
    }
    private void Shoot(int currentEntity, LayerMask targetLayer)// 6 маска игрока 7 враг
    {
        Debug.Log("Shot");
        ref var attackCmp = ref _attackComponentsPool.Value.Get(currentEntity);
        ref var gunCmp = ref _gunComponentsPool.Value.Get(currentEntity);
        gunCmp.firePoint.rotation = gunCmp.weaponContainer.rotation * Quaternion.Euler(0, 0, Random.Range(-gunCmp.currentSpread, gunCmp.currentSpread));

       /* var targetsTest = Physics2D.RaycastAll(gunCmp.firePoint.position, gunCmp.firePoint.up, gunCmp.attackLeght);
        foreach(var targ in targetsTest)
        {
            Debug.Log(targ.collider.gameObject.layer);
        }*/
        var targets = Physics2D.RaycastAll(gunCmp.firePoint.position, gunCmp.firePoint.up, gunCmp.attackLeght, targetLayer/**/);

       /* foreach (var targ in targetsTest)
        {
            Debug.Log(targ.collider.gameObject.layer + "enemy");
        }*/

        if (targets.Length == 0)
        {
            Debug.Log("promax");
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
                var hpEntity = target.collider.gameObject.GetComponent<HealthView>()._entity;
                ref var health = ref _healthComponentsPool.Value.Get(hpEntity);
                int startedHealth = health.healthPoint;
                //health.healthPoint -= damageReminder;//сделать ивент и запихнуть в систему здоровья
                if(!health.healthView.isDeath)
                _changeHealthEventsPool.Value.Add(_world.Value.NewEntity()).SetParametrs(damageReminder, hpEntity);
                Debug.Log(damageReminder + "dmg");
                if (health.healthPoint <= 0)
                {
                    Debug.Log(health.healthPoint + "hp and death");
                    //_world.Value.DelEntity(target.collider.gameObject.GetComponent<HealthView>()._entity);
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
