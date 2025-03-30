using UnityEngine;

public class InterestObjectOnLocationView : MonoBehaviour
{
    public InterestObjectType objectType;
    public float spawnChance;
    public enum InterestObjectType
    {
        brocked,
        collecting,
        explode,
        none
    }
    public DroppedItemView dropItemView;
    public NeededMasterKeyInfo neededMasterKey;
    //public int durabilityPoints;
}
