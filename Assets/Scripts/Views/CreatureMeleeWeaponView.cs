using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMeleeWeaponView : MonoBehaviour
{
    public float attackLenght;
    public int damage;
    public float attackCouldown;
    public bool isWideHit;
    public float attackSpeed;

    public float knockbackSpeed;
    public float stunTime;

    public BoxCollider2D meleeWeaponCollider;
    public Transform weaponTransform;
    public MeleeWeaponColliderView meleeWeaponColliderView;

    public ItemInfoForCreatureElement itemVisualInfo;
}
