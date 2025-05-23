using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerMoveComponent 
{
    public float currentHungerPoints { get;set; }
    public float currentHealingOneHealTime;
    public float maxHungerPoints;
    public bool nowIsMoving;
    public PlayerView playerView;
}
