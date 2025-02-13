using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerMoveComponent 
{
    public bool isRun;
    public float currentRunTime;
    public float currentRunTimeRecoverySpeed;
    public float maxRunTime;
    public float currentHungerPoints;
    public float currentHealingOneHealTime;
    public float maxHungerPoints;
    public bool nowIsMoving;
    public PlayerView playerView;
}
