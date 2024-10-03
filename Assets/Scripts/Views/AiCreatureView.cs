using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiCreatureView : MonoBehaviour
{
    public float followDistance;//дистанция на котрой начинать приследовать игрока 
    public float safeDistance;//дистанция на котрой будет останавливать существо при побеге от игрока 
    public float minSafeDistance;//дистанция при которой начинает убегать от игрока //безопасные дистанции указываются для врагов которые умеют отступать
    public bool isAttackWhenRetreat;
    public bool isPeaceful;
}
