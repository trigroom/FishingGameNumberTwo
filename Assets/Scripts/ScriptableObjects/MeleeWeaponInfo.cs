using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MeleeWeaponInfo", menuName = "ScriptableObjects/MeleeWeaponInfo")]
public class MeleeWeaponInfo : ScriptableObject
{
    [Header("Weapon settings")]
    public float attackLenght;
    public int damage;
    public float attackCouldown;
    public float weaponChangeSpeed = 2;//скорость смены на это оружие, а не с него
    public bool isWideHit;
    public bool isAuto;
    public float attackSpeed;
    public Vector2 colliderSize;
    public float wideAttackSpeed;
    public float wideAttackLenght;

    public float knockbackSpeed;
    public float stunTime;
    public float staminaUsage;

    public bool isOneHandedWeapon;
    public float wideAttackDamageMultiplayer = 1;


    [Header("Visual settings")]
    public float spriteScaleMultiplayer;
    public float spriteRotation;
    public Sprite weaponSprite;

    [Header("Audio settings")]
    public AudioClip hitSound;
}
