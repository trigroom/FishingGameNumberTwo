using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MovementComponent 
{
    public bool canMove;
    public float moveSpeed{ get; set; }
    public bool isStunned;
    public float stunTime;
    public MovementView movementView;
    public Transform entityTransform;
    public Vector2 moveInput;
    public float timeFromLastStep;
    public bool isTrapped;
    public float speedMultiplayer ;
    public Vector2 pointToRotateInput { get; set; }

    public bool isRun;
    public float currentRunTime;
    public float currentRunTimeRecoverySpeed;
    public float maxRunTime{ get; set; }
}
