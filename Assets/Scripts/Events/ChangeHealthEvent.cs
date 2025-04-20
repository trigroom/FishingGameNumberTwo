using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChangeHealthEvent 
{
    public int changedHealth;
    public int changedEntity;
    public bool isHeadshot;
    public float changedStunMultiplayer;
    public void SetParametrs(int health, int entity, bool headshot, float stunMultiplayer)
    {
        changedHealth = health;
        changedEntity = entity;
        isHeadshot = headshot;
        changedStunMultiplayer = stunMultiplayer;
    }
}
