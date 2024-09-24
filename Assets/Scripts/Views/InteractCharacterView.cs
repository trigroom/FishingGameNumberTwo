using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCharacterView : MonoBehaviour
{
    public enum InteractNPCType
    {
        shop,
        dialogeNpc,
        storage
    }

    public int _entity { get; private set; }
    public EcsWorld _world { get; private set; }

    [field: SerializeField] public InteractNPCType _characterType { get; protected set; }


    public void Construct(EcsWorld world, int entity)
    {
        _entity = entity;
        _world = world;
    }
}
