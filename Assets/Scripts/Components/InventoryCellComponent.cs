using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct InventoryCellComponent 
{
    public InventoryCellView cellView;
    public InventoryItemComponent inventoryItemComponent;
    public bool isEmpty;
    public int itemsCount;
    public TMP_Text inventoryCellItemCountText;
}
