using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class ShopCellsSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<ShopCellComponent> _shopCellComponentsPool;
    private EcsPoolInject<ShopCharacterComponent> _shopCharacterComponentsPool;
    private EcsPoolInject<HidedObjectOutsideFOVComponent> _hidedObjectOutsideFOVComponentsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<PlayerUpgradedStats> _playerUpgradedStatsPool;
    // private EcsPoolInject<StartLocationShopperTag> _startLocationShopperTagsPool;
    private EcsPoolInject<CurrentLocationComponent> _currentLocationComponentsPool;
    private EcsPoolInject<SetupShoppersOnNewLocationEvent> _setupShoppersOnNewLocationEventsPool;
    private EcsPoolInject<CurrentInteractedCharactersComponent> _currentInteractedCharactersComponentsPool;
    private EcsPoolInject<QuestNPCComponent> _questNPCComponentsPool;

    private EcsFilterInject<Inc<ShopOpenEvent>> _shopOpenEventsFilter;
    private EcsFilterInject<Inc<ShopCloseEvent>> _shopCloseEventsFilter;
    private EcsFilterInject<Inc<SetupShoppersOnNewLocationEvent>> _setupShoppersOnNewLocationEventsFilter;

    private int CreateShopper(ShopCharacterView shopper)
    {
        int shopperEntity = _world.Value.NewEntity();

        ref var shopperCmp = ref _shopCharacterComponentsPool.Value.Add(shopperEntity);
        shopperCmp.items = shopper.shopItems;
        shopperCmp.characterView = shopper;

        //SetShopperToDefault(ref shopperCmp);

        shopper.GetComponent<InteractCharacterView>().Construct(_world.Value, shopperEntity);

        if (shopper.gameObject.TryGetComponent<HidedOutsidePlayerFovView>(out HidedOutsidePlayerFovView hidedCmp))
        {
            ref var hidedObjCmp = ref _hidedObjectOutsideFOVComponentsPool.Value.Add(shopperEntity);
            hidedObjCmp.hidedObjects = hidedCmp.objectsToHide;
            hidedObjCmp.isSetActiveObject = true;
        }
        return shopperEntity;
    }
    private void SetShopperToDefault(ref ShopCharacterComponent shopperCmp)
    {
        shopperCmp.remainedMoneyToBuy = shopperCmp.characterView.startMoneyToBuy;
        shopperCmp.currentShopPage = 0;
        shopperCmp.remainedShopItems = new int[shopperCmp.items.Length];
        for (int i = 0; i < shopperCmp.remainedShopItems.Length; i++)
            shopperCmp.remainedShopItems[i] = shopperCmp.items[i].itemsCountToBuy;
        Debug.Log(" set shopper");
    }
    public void Init(IEcsSystems systems)
    {
        foreach (var shopper in _sceneData.Value.interactCharacters)
        {
            if (shopper._characterType != InteractCharacterView.InteractNPCType.gunsmith)
                /* _startLocationShopperTagsPool.Value.Add(*/
                CreateShopper(shopper.GetComponent<ShopCharacterView>());
        }

        foreach (var shopCell in _sceneData.Value.shopCellsList)
        {
            shopCell.Construct(_world.Value.NewEntity(), _world.Value);
            _shopCellComponentsPool.Value.Add(shopCell._entity).cellView = shopCell;
        }

        _sceneData.Value.dropedItemsUIView.shopLastPageButton.onClick.AddListener(TurnToLastShopPage);
        _sceneData.Value.dropedItemsUIView.shopNextPageButton.onClick.AddListener(TurnToNextShopPage);
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var setapShoppers in _setupShoppersOnNewLocationEventsFilter.Value)
            SetShopperToDefault(ref _shopCharacterComponentsPool.Value.Get(setapShoppers));

        foreach (var shopOpenEvt in _shopOpenEventsFilter.Value)
        {
            var shopCharacterCmp = _shopCharacterComponentsPool.Value.Get(shopOpenEvt);
            _sceneData.Value.dropedItemsUIView.shopTableImage.sprite = shopCharacterCmp.characterView.shopperTable;
            _sceneData.Value.dropedItemsUIView.shopperRepText.text = _questNPCComponentsPool.Value.Get(_currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).interactCharacterView._entity).currentQuest + " rep";

            SetShopPage(shopCharacterCmp.currentShopPage);
        }

        foreach (var shopCloseEvt in _shopCloseEventsFilter.Value)
        {
            for (int i = 0; i < _sceneData.Value.shopCellsList.Length; i++)
                _sceneData.Value.ReleaseShopCell(i);
        }
    }

    public void TurnToNextShopPage()
    {
        int shopperEntity = _currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).interactCharacterView._entity;
        SetShopPage(++_shopCharacterComponentsPool.Value.Get(shopperEntity).currentShopPage);
    }
    public void TurnToLastShopPage()
    {
        int shopperEntity = _currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).interactCharacterView._entity;
        SetShopPage(--_shopCharacterComponentsPool.Value.Get(shopperEntity).currentShopPage);
    }

    public void SetShopPage(int neededPage)
    {
        int shopCellsOnPageCount = _sceneData.Value.shopCellsList.Length;
        int shopperEntity = _currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).interactCharacterView._entity;
        ref var shopperCmp = ref _shopCharacterComponentsPool.Value.Get(shopperEntity);

        for (int i = 0; i < shopCellsOnPageCount; i++)
        {
            ref var shopCellCmp = ref _shopCellComponentsPool.Value.Get(_sceneData.Value.shopCellsList[i]._entity);
            if (shopperCmp.items.Length > neededPage * shopCellsOnPageCount + i)
            {
                ref var shopCell = ref shopperCmp.items[neededPage * shopCellsOnPageCount + i];
                shopCellCmp.cellView.gameObject.SetActive(true);
                shopCellCmp.shopperItemId = neededPage * shopCellsOnPageCount + i;

                shopCellCmp.itemInfo = shopCell.itemInfo;
                if (shopCell.needSkillLevelToUnlock == 0 || (shopCell.needSkillToUnlock != 999 && shopCell.needSkillLevelToUnlock != 0 && _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).statLevels[shopCell.needSkillToUnlock] >= shopCell.needSkillLevelToUnlock)
                    || (shopCell.needSkillToUnlock == 999 && _questNPCComponentsPool.Value.Get(_currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).interactCharacterView._entity).currentQuest >= shopCell.needSkillLevelToUnlock))
                {
                    shopCellCmp.cellView.shopRemainedItemCount.text = "remaining " + shopperCmp.remainedShopItems[shopCellCmp.shopperItemId] + " items to buy";
                    shopCellCmp.itemCost = shopCell.price;
                    shopCellCmp.itemCount = shopCell.count;
                    shopCellCmp.cellView.SetShopCellInfo(shopCell, true);
                }
                else
                {
                    shopCellCmp.cellView.SetShopCellInfo(shopCell, false);
                    shopCellCmp.cellView.shopRemainedItemCount.text = "";
                }
            }
            else
            {
                shopCellCmp.cellView.gameObject.SetActive(false);
              //  Debug.Log("Hide shop cell" + shopperCmp.items.Length);
            }
        }

        _sceneData.Value.dropedItemsUIView.shopNextPageButton.gameObject.SetActive(shopperCmp.items.Length > (neededPage + 1) * shopCellsOnPageCount);
        _sceneData.Value.dropedItemsUIView.shopLastPageButton.gameObject.SetActive(neededPage != 0);
    }
}
