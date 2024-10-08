using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZoneView : MonoBehaviour
{
    public Transform[] daySpawnCreaturesPool;
    public Transform[] nightSpawnCreaturesPool;
    public float spawnTime;
    public int _entity { get; private set; }

    public void Construct(int entity)
    {
        _entity = entity;
    }
}
