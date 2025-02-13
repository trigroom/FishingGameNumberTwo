using UnityEngine;

public class InterestObjectOnLocationView : MonoBehaviour
{
    public InterestObjectType objectType;
    public float spawnChance;
    public enum InterestObjectType
    {
        brocked,
        collecting
    }

    public CreatureDropElement[] dropElements;
    public int maxDroppedItemsCount;
    public DroppedItemView dropItemView;
    //public int durabilityPoints;
}
