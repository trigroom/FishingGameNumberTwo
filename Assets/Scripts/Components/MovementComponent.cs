using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MovementComponent 
{
    public bool canMove;
    public float moveSpeed;
    public bool isStunned;
    public float stunTime;
    public MovementView movementView;
    public Transform entityTransform;
    public Vector2 moveInput;

    public Vector2 pointToRotateInput;
}
