using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeleeWeaponComponent 
{
    public bool isHitting;
    public Vector2 endHitPoint;
    public Vector2 startHitPoint;//задавать вначале спавна сущности
    public bool moveInAttackSide;
    public float startRotation{ get; set; }
    public bool isWideAttack;
    public int attackState;
    public float curAttackLenghtMultiplayer ;
}
