using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GunComponent 
{
    public int magazineCapacity;
    public int currentMagazineCapacity;
    public float reloadDuration;
    public float currentReloadDuration;
    public float currentAttackCouldown;
    public float attackCouldown;
    public float attackLeght;
    public float currentSpread;
    public float maxSpread;
    public float minSpread;
    public float addedSpread;
    public float spreadRecoverySpeed;
    public bool isReloading;
    public bool isAuto;
    public int bulletCountToReload;
    public int bulletCount;
    public int bulletTypeId;

    public Transform weaponContainer;
    public Transform firePoint;
}
