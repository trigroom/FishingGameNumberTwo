using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    public float timeFromLastShot;
    public float gunSpritePositionRecoil {  get; set; }

    public Light2D lightFromGunShot;
    public float flashShotInstance;

    public Transform weaponContainer;
    public Transform firePoint;
}
