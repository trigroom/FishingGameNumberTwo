using UnityEngine;

public class InterestObjectOnLocationView : MonoBehaviour
{
    public InterestObjectType objectType;
    public enum InterestObjectType
    {
        brocked,
        collecting,
        explode,
        none
    }
    public DroppedItemView dropItemView;
    //public int durabilityPoints;
}
