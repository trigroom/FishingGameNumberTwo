using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiCreatureView : MonoBehaviour
{
    public float followDistance;//��������� �� ������ �������� ������������ ������ 
    public float safeDistance;//��������� �� ������ ����� ������������� �������� ��� ������ �� ������ 
    public float minSafeDistance;//��������� ��� ������� �������� ������� �� ������ //���������� ��������� ����������� ��� ������ ������� ����� ���������
    public bool isAttackWhenRetreat;
    public bool isTwoWeapon;
    public bool isPeaceful;

    public ItemInfoForCreatureElement healItemVisualInfo;

    public SpriteRenderer itemSpriteRenderer;
    public Transform itemTransform;
}
