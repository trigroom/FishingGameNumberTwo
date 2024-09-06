using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemInfo", menuName = "ScriptableObjects/ItemInfo", order = 1)]
public class ItemInfo : ScriptableObject
{
    public enum itemType
    {
        weapon,
        heal,
        bullet
    }
    public Sprite itemSprite;
    public itemType type;
    public int maxCount;
    public float itemWeight;
    public int itemId;
}
