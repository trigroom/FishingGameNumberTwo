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
        grenade,
        flashlight,
        randomHeal,
        drop,
        backpack,
        gunPart,
        addedExpItem,
        sheild,
        helmet,
        bodyArmor,
        bullet
    }
    [Header("Item stats")]
    public Sprite itemSprite;
    public itemType type;
    public int maxCount;
    public float itemWeight;
    public int itemId;
    public string itemName;
    public string itemDescription;
    public int itemCost;
    [Header("Weapons")]
    public GunInfo gunInfo;
    public MeleeWeaponInfo meleeWeaponInfo;
    public GrenadeInfo grenadeInfo;
    [Header("Heal items")]
    public HealingItemInfo healInfo;
    [Header("Flashlights")]
    public FlashlightInfo flashlightInfo;
    [Header("BackPacks")]
    public BackpackInfo backpackInfo;
    [Header("GunParts")]
    public GunPartInfo gunPartInfo;
    [Header("AddedExpItem")]
    public ExpAddedItemInfo expAddedInfo;
    [Header("SheildItem")]
    public SheildInfo sheildInfo;
    [Header("Armor")]
    public HelmetInfo helmetInfo;
    public BodyArmorInfo bodyArmorInfo;
    [Header("Random Heal items")]
    public RandomHealItemInfo randomHealInfo;
    [Header("Bullet items")]
    public BulletInfo bulletInfo;
}
