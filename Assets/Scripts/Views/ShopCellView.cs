using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCellView : MonoBehaviour
{
    [field: SerializeField] public Image shopItemIcon { get; private set; }
    [field: SerializeField] public TMP_Text shopItemButtonText{ get; private set; }
    [field: SerializeField] public TMP_Text shopItemNameAndCount { get; private set; }
    public ItemInfo item { get; private set; }
    [field: SerializeField] public Button shopButton { get; private set; }
    public int itemCost { get; private set; }
    public int itemCount {  get; private set; }
    private bool isBuyItem;
    private int _entity;
    private EcsWorld _world;

    private void Awake()
    {
        shopButton.onClick.AddListener(isShopButtonAction);
    }
    public void Construct(int entity, EcsWorld world)
    {
        _entity = entity;
        _world = world;
    }

    public void SetShopCellInfo(ShopItemInfo shopItemInfo)
    {
        item = shopItemInfo.itemInfo;
        itemCost = shopItemInfo.price;
        itemCount = shopItemInfo.count;

        isBuyItem = shopItemInfo.isBuyItem;

        if(isBuyItem )
        {
            shopItemButtonText.text = "Купить";
        }
        else
        {
            shopItemButtonText.text = "Продать";
        }
            shopItemNameAndCount.text = itemCount + " " + shopItemInfo.itemInfo.itemName + " за " + itemCost +" $";

        shopItemIcon.sprite = item.itemSprite;
    }
    private void isShopButtonAction()
    {
        if(isBuyItem) BuyItem();
        else FindAndCellItem();
    }

    private void BuyItem()
    {
        _world.GetPool<BuyItemFromShopEvent>().Add(_entity);
    }

    private void FindAndCellItem()
    {
        _world.GetPool<FindAndCellItemEvent>().Add(_entity);
    }
}
