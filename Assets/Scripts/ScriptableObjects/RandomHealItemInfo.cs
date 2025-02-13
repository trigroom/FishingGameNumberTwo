using UnityEngine;

[CreateAssetMenu(fileName = "RandomHealItemInfo", menuName = "Scriptable Objects/RandomHealItemInfo")]
public class RandomHealItemInfo : ScriptableObject
{
    public ItemInfo safeItemInfo;
    public ItemInfo poisonItemInfo;
    public float chanceToSafe;
}
