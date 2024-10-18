using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCharacterView : InteractCharacterView
{
    public ShopItemInfo[] shopItems;
    public Sprite shopperTable;

    private void Awake()
    {
        _characterType = InteractNPCType.shop;
    }
}
