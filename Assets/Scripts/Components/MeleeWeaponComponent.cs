using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeleeWeaponComponent 
{
    public bool isHitting;
    public Vector2 endHitPoint;
    public Vector2 startHitPoint;//задавать вначале спавна сущности
    public bool moveInAttackSide;
    public float startRotation;
    public bool isWideAttack;
    public float curAttackLenghtMultiplayer { get; set; }
}
