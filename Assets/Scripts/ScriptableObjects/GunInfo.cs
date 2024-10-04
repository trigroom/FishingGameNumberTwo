using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "GunInfo", menuName = "ScriptableObjects/GunInfo", order = 2)]
public class GunInfo : ScriptableObject
{
    [Header("Gun settings")]
    public float attackLenght;
    public float reloadDuration;
    public int magazineCapacity;
    public int damage;
    public float attackCouldown;
    public float maxSpread;
    public float minSpread;
    public float addedSpread;
    public float weaponChangeSpeed = 2;//скорость с мены на это оружие, а не с него
    public float spreadRecoverySpeed;
    public GameObject gunObject;
    public bool isAuto;
    public int bulletTypeId;
    public int bulletCount = 1;
    public bool isOneBulletReloaded;
    public int maxDurabilityPoints;

    [Header("Scope settings")]
    public int scopeMultiplicity;
}
