using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CurrentAttackComponent 
{
    public bool canAttack;
    public bool weaponIsChanged;
    public int damage;

    public float changeWeaponTime;
    public float currentChangeWeaponTime;
}
