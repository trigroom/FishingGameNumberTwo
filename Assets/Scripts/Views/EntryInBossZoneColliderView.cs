using UnityEngine;

public class EntryInBossZoneColliderView : MonoBehaviour
{
    public MeleeWeaponColliderView bossMeleeColliderView;
    public BossFirstView bossFirstView;
    public Collider2D traktorCollider;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        bossMeleeColliderView._world.GetPool<ChangeFirstBossPhaseEvent>().Add(bossMeleeColliderView._entity);
        bossFirstView.firstPhaseSprite.gameObject.SetActive(false);
        traktorCollider.enabled = true;
    }
}
