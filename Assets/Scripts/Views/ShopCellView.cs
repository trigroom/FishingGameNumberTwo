using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCellView : MonoBehaviour
{
    [field: SerializeField] public Image shopItemIcon { get; private set; }
    //[field: SerializeField] public TMP_Text shopItemButtonText { get; private set; }
    [field: SerializeField] public TMP_Text shopItemNameAndCount { get; private set; }
    [field: SerializeField] public TMP_Text shopRemainedItemCount { get; private set; }
    public ItemInfo item { get; private set; }
    [field: SerializeField] public Button shopButton { get; private set; }
    public int _entity { get; private set; }
    private EcsWorld _world;

    private void Awake()
    {
        shopButton.onClick.AddListener(BuyItem);
    }
    public void Construct(int entity, EcsWorld world)
    {
        _entity = entity;
        _world = world;
    }

    public void SetShopCellInfo(ShopItemInfo shopItemInfo, bool isUnlockedItem)
    {
        if (isUnlockedItem)
        {
            string itemCount = shopItemInfo.count != 1 ? shopItemInfo.count+" " : "";

         //   shopItemButtonText.text = "Купить";
            shopItemNameAndCount.text = itemCount + shopItemInfo.itemInfo.itemName + "\n за " + shopItemInfo.price + " $";
            shopButton.enabled = true;
            if (shopItemIcon.color != Color.white)
                shopItemIcon.color = Color.white;
        }
        else
        {
        //    shopItemButtonText.text = "";
            if (shopItemInfo.needSkillToUnlock != 999)
                shopItemNameAndCount.text = "Need " + shopItemInfo.needSkillLevelToUnlock + " level of " + (PlayerUpgradedStats.StatType)shopItemInfo.needSkillToUnlock + " to unlock";//мделать чтобы скиллы где 2 и более слов в названии раздельно прописывались 
            else
                shopItemNameAndCount.text = "Need " + shopItemInfo.needSkillLevelToUnlock + " level of reputation to unlock";
            shopButton.enabled = false;
            if (shopItemIcon.color != Color.black)
                shopItemIcon.color = Color.black;
        }
        item = shopItemInfo.itemInfo;

        shopItemIcon.sprite = item.itemSprite;
    }

    private void BuyItem()
    {
        Debug.Log("Buy item");
        _world.GetPool<BuyItemFromShopEvent>().Add(_entity);
    }

    private void FindAndCellItem()
    {
        _world.GetPool<FindAndCellItemEvent>().Add(_entity);
    }
}
