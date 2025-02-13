using UnityEngine;

[CreateAssetMenu(fileName = "ItemCraftingRecipeInfo", menuName = "Scriptable Objects/ItemCraftingRecipeInfo")]
public class ItemCraftingRecipeInfo : ScriptableObject
{
    public int[] neededCount;
    public int[] neededId;
    public int craftedItemId;
    public int craftedItemsCount;

    public int needCraftingTableLevel;
}
