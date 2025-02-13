using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AiCreatureView : MonoBehaviour
{
    public float followDistance;//дистанция на котрой начинать приследовать игрока 
    public float safeDistance;//дистанция на котрой будет останавливать существо при побеге от игрока 
    public float minSafeDistance;//дистанция при которой начинает убегать от игрока //безопасные дистанции указываются для врагов которые умеют отступать
    public bool isAttackWhenRetreat = true;
    public Light2D lightFromGunShot;
    //public bool isTwoWeapon;
    public bool isPeaceful;
    public int expPoints;

    //public ItemInfoForCreatureElement healItemVisualInfo;//

    [field: SerializeField] public SpriteRenderer itemSpriteRenderer { get;set; }
    public Transform itemTransform;
}
