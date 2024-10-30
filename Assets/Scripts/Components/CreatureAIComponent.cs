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
    public float timeFromLastTargetSeen;

    public Transform currentTarget;
    public List<Transform> targets;
    public Collider2D[] obstacles;
    public List<Transform> colliders;
    public float[] dangersResultTemp;
    public float[] interstsTemp;
    public bool isLeftMoveCircle;
    public int randomMoveDirectionIndex;
    public float timeFromLastChangeLeftMoveCircle;
    public bool isStoppedMoveInIdleState;
    public Vector2 targetPositionCached;
    public Vector2 resultDirection;

    public bool reachedLastTarget;

    //для рандомного движение в идле
    public Vector2 randomDirection;
    public float randomMoveTime;


}
