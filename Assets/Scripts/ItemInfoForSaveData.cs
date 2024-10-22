using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct ItemInfoForSaveData 
{
    public int itemId;
    public int itemCount;
    public int itemCellId;

    public ItemInfoForSaveData(int id, int count, int cellId)
    {
        itemId = id;
        itemCount = count;
        itemCellId = cellId;
    }
}
