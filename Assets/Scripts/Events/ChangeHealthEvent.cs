using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChangeHealthEvent 
{
    public int changedHealth;
    public int changedEntity;

    public void SetParametrs(int health, int entity)
    {
        changedHealth = health;
        changedEntity = entity;
    }
}
