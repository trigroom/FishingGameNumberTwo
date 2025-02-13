using UnityEngine;

public class HealthView : MonoBehaviour
{
    [field: SerializeField] public int maxHealth { get; private set; }
    [field: SerializeField] public Collider2D characterMainCollaider { get; private set; }
    [field: SerializeField] public Collider2D characterHeadCollaider { get; private set; }
    [field: SerializeField] public HeadColliderView headColliderView { get; private set; }
    [field: SerializeField] public InterestObjectOnLocationView interestObjectView { get; private set; }
    public bool isDeath { get; private set; }

    public int _entity { get; private set; }
    //private EcsWorld _world;
    public void Construct(int entity/*, EcsWorld world*/)
    {
        _entity = entity;
        if (headColliderView != null)
            headColliderView._entity = entity;
        //  _world = world;
    }
    public void Death()
    {
        isDeath = true;
        if (interestObjectView == null)
        {
            characterMainCollaider.enabled = false;
            if (characterHeadCollaider != null)
                characterHeadCollaider.enabled = false;
        }
        else
            Destroy(gameObject);
    }
}
