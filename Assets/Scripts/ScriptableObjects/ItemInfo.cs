using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemInfo", menuName = "ScriptableObjects/ItemInfo", order = 1)]
public class ItemInfo : ScriptableObject
{
    public enum itemType
    {
        gun,
        meleeWeapon,
        heal,
        bullet,
        drop
    }
    [Header("Item stats")]
    public Sprite itemSprite;
    public itemType type;
    public int maxCount;
    public float itemWeight;
    public int itemId;
    public string itemName;
    public string itemDescription;
    [Header("Weapons")]
    public GunInfo gunInfo;
    [Header("Heal items")]
    public HealingItemInfo healInfo;

}
