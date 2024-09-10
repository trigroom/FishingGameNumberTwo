using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ShopItemInfo", menuName = "ScriptableObjects/ShopItemInfo", order = 2)]
public class ShopItemInfo : ScriptableObject
{
    public bool isBuyItem;
    public ItemInfo itemInfo;
    public int count;
    public int price;
}
