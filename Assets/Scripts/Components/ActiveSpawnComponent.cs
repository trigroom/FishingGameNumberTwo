using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ActiveSpawnComponent 
{
    public Transform[] spawnObjects;//всё будет браться из spawninfoview
    public float curSpawnTime;
    public float spawnTime;
    public SpawnZoneView zoneView;
    //узнать про спавены врагов

}
