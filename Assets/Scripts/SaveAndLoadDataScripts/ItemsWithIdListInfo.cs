using UnityEngine;

[CreateAssetMenu(fileName = "ItemsWithIdList", menuName = "ScriptableObjects/ItemsWithIdList", order = 2)]
public class ItemsWithIdListInfo : ScriptableObject
{
    public ItemInfo[] items;

    public InventoryCellView[] cells;
}
