using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ActiveSpawnComponent 
{
    public Transform[] currentSpawnCreaturesPool;//всё будет браться из spawninfoview
    public float curSpawnTime;
    public float spawnTime;
    public SpawnZoneView zoneView;
    //узнать про спавены врагов

}
