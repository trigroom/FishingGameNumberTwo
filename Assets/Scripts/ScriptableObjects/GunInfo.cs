using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "GunInfo", menuName = "ScriptableObjects/GunInfo", order = 2)]
public class GunInfo : ScriptableObject
{
    public float attackLenght;
    public float reloadDuration;
    public int magazineCapacity;
    public int damage;
    public float attackCouldown;
    public float maxSpread;
    public float minSpread;
    public float addedSpread;
    public float spreadRecoverySpeed;
    public GameObject gunObject;
    public bool isAuto;
}
