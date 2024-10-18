using Leopotam.EcsLite;
using UnityEngine;

public class MeleeWeaponColliderView : MonoBehaviour
{
    private EcsWorld _world;
    [SerializeField] private int targetLayer;
    private int _entity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //   Debug.Log("meleeContact" + collision.gameObject.layer);
        if (collision.gameObject.layer == targetLayer)
            _world.GetPool<MeleeWeaponContactEvent>().Add(_entity).attackedEntity 
                = collision.gameObject.GetComponent<HealthView>()._entity;
    }

    public void Construct(EcsWorld world, int entity)
    {
        _entity = entity;
        _world = world;
    }
}
