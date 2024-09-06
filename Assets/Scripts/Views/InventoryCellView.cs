using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCellView : MonoBehaviour
{
    [field: SerializeField] public Image inventoryCellItemImage;
    [field: SerializeField] public TMP_Text inventoryCellItemCountText;

    public void ChangeCellItemSprite(Sprite itemSprite)
    {
        inventoryCellItemImage.sprite = itemSprite;
    }

    public void ChangeCellItemCount(int count)
    {
        inventoryCellItemCountText.text = count.ToString();
    }

    public void ClearInventoryCell()
    {
        inventoryCellItemImage.sprite = null;//надо ставить невидимый спрайт
        inventoryCellItemCountText.text = "";
    }
}
