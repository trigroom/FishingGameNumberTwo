using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CreatureGunView : MonoBehaviour
{
    [Header("Gun settings")]
    public float attackLenght;
    public float reloadDuration;
    public int magazineCapacity;
    public int damage;
    public float attackCouldown;
    public int bulletInShotCount;
    public float maxSpread;
    public float minSpread;
    public float addedSpread;
    public float spreadRecoverySpeed;
    public bool bulletShellsOnShoot;
    public int bulletShellIndex;
    public Vector2 bulletShellsSpawn;
    //public GameObject gunObject;
    public bool isOneBulletReloaded;
    public Light2D lightFromGunShot;
    public float flashShotIntance;
    public ItemInfoForCreatureElement itemVisualInfo;
    [Header("Audio settings")]
    public AudioClip shotSound;
    public AudioClip reloadSound;
}
