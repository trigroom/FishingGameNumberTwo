using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ShopCharacterComponent 
{
    public ShopItemInfo[] items;
    public int[] remainedShopItems;
    public ShopCharacterView characterView;
    public int remainedMoneyToBuy {  get; set; }
    public int currentShopPage;
}
