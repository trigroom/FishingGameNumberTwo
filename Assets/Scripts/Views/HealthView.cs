using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthView : MonoBehaviour
{
    [field: SerializeField] public int maxHealth { get; private set; }
    public bool isDeath {  get; private set; }

    public int _entity { get; private set; }
    //private EcsWorld _world;
    public void Construct(int entity/*, EcsWorld world*/)
    {
        _entity = entity;
      //  _world = world;
    }
    public void Death()
    {
        isDeath = true;
        Destroy(gameObject);
    }
}
