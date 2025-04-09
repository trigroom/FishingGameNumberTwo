using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GunInfo", menuName = "ScriptableObjects/GunInfo", order = 2)]
public class GunInfo : ScriptableObject
{
    [Header("Gun settings")]
    public GunType gunType;
    public float attackLenght;
    public float shotSoundDistance;
    public float reloadDuration;
    public int magazineCapacity;
    public int damage;
    public float attackCouldown;
    public float maxSpread;
    public float minSpread;
    public float addedSpread;
    public float weaponChangeSpeed = 2;//скорость с мены на это оружие, а не с него
    public bool isAuto;
    public int bulletTypeId;
    public int bulletCount = 1;
    public bool isOneBulletReloaded;
    public int maxDurabilityPoints;
    public int bulletShellIndex;

   // public float maxCameraSpread;
   // public float addedCameraSpread;
   // public float recoveryCameraSpread;

    public Vector2 firepointPosition;
    public Vector2 bulletShellPointPosition;

    public bool bulletShellSpawnOnShot;

    public int durabilityRecoveryCost;
    public bool isOneHandedGun;

    public LaserPointerForGunElement laserPointer ;
    public Vector2 laserPointerPosition;
    [Header("Visual settings")]
    public Vector2[] gunPartsInGamePositions = new Vector2[4];
    public float spriteScaleMultiplayer;
    public float spriteRotation;
    public Sprite weaponSprite;
    public float shotFlashDistance = 3;
    public float shotFlashIntance = 10;
    public float shotFlashAngle = 50;
    public Vector2[] bulletsUIPositions;
    public Sprite bulletUISprite;
    public Sprite magUISprite;
    public Vector2 magUISize;
    public Vector2 magUIPosition;
    public Vector2 bulletUISize;

    [Header("Scope settings")]
    public float scopeMultiplicity;
    public float blackBGSize;
    public Sprite centreCrossSprite;

    [Header("Audio settings")]
    public AudioClip shotSound;
    public AudioClip startReloadSound;
    public AudioClip endReloadSound;

    [Header("Upgrade info")]
    public int upgradeCost;
    public int upgradedGunId;
    public int neededGunLevelToUpgrade ;
    public GunParts gunParts;

    [Flags]
    public enum GunParts
    {
        butt = 1,
        scope = 2,
        downPart = 4,
        barrelPart = 8,
    }

    public enum GunType
    {
        pistol,
        shotgun,
        sniper,
        revolver,
        smg,
        ar
    }
}
