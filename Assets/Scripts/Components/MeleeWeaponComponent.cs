using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeleeWeaponComponent 
{
    public bool isHitting { get; set; }
    public Vector2 endHitPoint;
    public Vector2 startHitPoint;//�������� ������� ������ ��������
    public bool moveInAttackSide;
    public float startRotation;
}
