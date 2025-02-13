using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct InventoryCellComponent 
{
    public InventoryCellView cellView;
    public bool isEmpty {  get; set; }
    public TMP_Text inventoryCellItemCountText;
}
