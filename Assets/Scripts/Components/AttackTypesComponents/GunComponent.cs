using UnityEngine;

public struct GunComponent
{
    public int magazineCapacity;
    public int currentMagazineCapacity;
    public float reloadDuration;
    public float currentReloadDuration;
    public float attackLeght;
    public float currentSpread;
    public float currentMaxSpread;
    public float currentMinSpread;
    public float currentAddedSpread;
    public float spreadRecoverySpeed;
    public bool isReloading;
    public int bulletInShotCount;
    public bool isOneBulletReload;


    public Transform weaponContainer;
    public Transform firePoint;
}
