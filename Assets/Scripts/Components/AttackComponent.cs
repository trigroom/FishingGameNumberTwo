using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackComponent
{
    public bool canAttack;
    public bool weaponIsChanged;
    public bool weaponSpriteIsChanged;
    public int damage { get; set; }

    public float currentAttackCouldown;
    public float attackCouldown;

    public float changeWeaponTime;
    public float currentChangeWeaponTime { get; set; }

    public float grenadeThrowCouldown;
    public float weaponRotateSpeed { get; set; }
}
