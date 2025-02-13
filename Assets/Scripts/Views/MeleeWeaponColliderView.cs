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
        if (!_world.GetPool<MeleeWeaponContactEvent>().Has(_entity))
        {
            if ((collision.gameObject.layer == targetLayer && collision.gameObject.tag != "Head") || collision.gameObject.layer == 17)
                _world.GetPool<MeleeWeaponContactEvent>().Add(_entity) = new MeleeWeaponContactEvent(collision.gameObject.GetComponent<HealthView>()._entity, false, false, collision.ClosestPoint(gameObject.transform.position));
            else if (/*targetLayer == 7 && collision.gameObject.layer == 16 */collision.gameObject.tag == "Head")
                _world.GetPool<MeleeWeaponContactEvent>().Add(_entity) = new MeleeWeaponContactEvent(collision.gameObject.GetComponent<HeadColliderView>()._entity, false, true, collision.ClosestPoint(gameObject.transform.position));
            else if (collision.gameObject.layer == 13 && ((collision.gameObject.tag == "Player" && targetLayer == 7) || (collision.gameObject.tag != "Player" && targetLayer == 6)))//чтобы игрок или враг не могли свой же щит удрить
            {
                _world.GetPool<MeleeWeaponContactEvent>().Add(_entity) = new MeleeWeaponContactEvent(collision.gameObject.GetComponent<ShieldView>()._entity, true, false, collision.ClosestPoint(gameObject.transform.position));
            }
        }
    }

    public void Construct(EcsWorld world, int entity)
    {
        _entity = entity;
        _world = world;
    }
}
