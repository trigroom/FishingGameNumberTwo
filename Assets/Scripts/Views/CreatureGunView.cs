using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //public GameObject gunObject;
    public bool isOneBulletReloaded;
    public ItemInfoForCreatureElement itemVisualInfo;
}
