using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CreatureAIComponent 
{
    public enum CreatureStates
    {
        idle,
        follow,
        shootingToTarget,
        runAwayFromTarget
    }

    public CreatureStates currentState;
    public float safeDistance;
    public float minSafeDistance;
    public float followDistance;
    public bool isAttackWhenRetreat;
    public CreatureView creatureView;
    public bool isPeaceful;
    public float timeToUpdate;

    public Transform targetTransform;
    //для рандомного движение в идле
    public Vector2 randomDirection;
    public float randomMoveTime;
}
