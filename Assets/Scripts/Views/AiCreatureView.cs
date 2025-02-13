using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AiCreatureView : MonoBehaviour
{
    public float followDistance;//��������� �� ������ �������� ������������ ������ 
    public float safeDistance;//��������� �� ������ ����� ������������� �������� ��� ������ �� ������ 
    public float minSafeDistance;//��������� ��� ������� �������� ������� �� ������ //���������� ��������� ����������� ��� ������ ������� ����� ���������
    public bool isAttackWhenRetreat = true;
    public Light2D lightFromGunShot;
    //public bool isTwoWeapon;
    public bool isPeaceful;
    public int expPoints;

    //public ItemInfoForCreatureElement healItemVisualInfo;//

    [field: SerializeField] public SpriteRenderer itemSpriteRenderer { get;set; }
    public Transform itemTransform;
}
