using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using TMPro;
using UnityEngine;


public class UiControlSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsCustomInject<SceneService> _sceneData;
    private EcsWorldInject _world;

    private EcsPoolInject<InventoryCellComponent> _inventoryCellComponentPool;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentPool;
    private EcsPoolInject<SetDescriptionItemEvent> _setDescriptionItemEventsPool;
    private EcsPoolInject<MenuStatesComponent> _menuStatesComponentsPool;
    private EcsPoolInject<ShopCloseEvent> _shopCloseEventsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<AttackComponent> _currentAttackComponentsPool { get; set; }
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<StorageCellTag> _storageCellTagsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<OpenQuestHelperEvent> _openQuestHelperEventsPool;
    private EcsPoolInject<CurrentDialogeComponent> _currentDialogeComponentsPool;
    private EcsPoolInject<CameraComponent> _cameraComponentsPool;
    private EcsPoolInject<DurabilityInInventoryComponent> _flashlightInInventoryComponentsPool;
    private EcsPoolInject<OffInScopeStateEvent> _offInScopeStateEventsPool;
    private EcsPoolInject<RevivePlayerEvent> _revivePlayerEventsPool;
    private EcsPoolInject<LaserPointerForGunComponent> _laserPointerForGunComponentsPool;
    private EcsPoolInject<PlayerUpgradedStats> _playerUpgradedStatsPool;
    private EcsPoolInject<InventoryComponent> _inventoryComponentsPool;
    private EcsPoolInject<SpecialInventoryCellTag> _specialInventoryCellTagsPool;
    private EcsPoolInject<ShopCharacterComponent> _shopCharacterComponentsPool;
    private EcsPoolInject<TryEquipGunPartEvent> _tryEquipGunPartEventsPool;
    private EcsPoolInject<DeleteItemEvent> _deleteItemEventsPool;
    private EcsPoolInject<ChangeGunPartDescriptionEvent> _changeGunPartDescriptionEvent;
    private EcsPoolInject<EquipGunPartEvent> _equipGunPartEventsPool;
    private EcsPoolInject<ChangeWeaponFromInventoryEvent> _changeWeaponFromInventoryEventsPool;
    // private EcsPoolInject<WeaponLevelComponent> _weaponLevelComponentsPool;
    private EcsPoolInject<SecondDurabilityComponent> _shieldComponentsPool;
    private EcsPoolInject<TryCraftItemEvent> _tryCraftItemEventsPool;
    private EcsPoolInject<TransportMoneyEvent> _transportMoneyEventsPool;
    private EcsPoolInject<CraftingTableComponent> _craftingTableComponentsPool;

    private EcsPoolInject<LoadGameEvent> _loadGameEventsPool;
    private EcsPoolInject<CurrentInteractedCharactersComponent> _currentInteractedCharactersComponentsPool;

    private EcsFilterInject<Inc<SetDescriptionItemEvent>> _setDescriptionItemEventsFilter;
    private EcsFilterInject<Inc<StorageOpenEvent>> _storageOpenEventsFilter;
    private EcsFilterInject<Inc<ShopOpenEvent>> _shopOpenEventsFilter;
    private EcsFilterInject<Inc<OpenCraftingTableEvent>> _openCraftingTableEventsFilter;
    private EcsFilterInject<Inc<DeathEvent, PlayerComponent>> _playerDeathEventsFilter { get; set; }
    private EcsFilterInject<Inc<RevivePlayerEvent>> _revivePlayerEventsFilter;
    private EcsFilterInject<Inc<GunWorkshopOpenEvent>> _gunWorkshopOpenEventsFilter;
    private EcsFilterInject<Inc<TryEquipGunPartEvent>> _tryEquipGunPartEventsFilter;
    private EcsFilterInject<Inc<EquipGunPartEvent>> _equipGunPartEventsFilter;
    private EcsFilterInject<Inc<LoadGameEvent>> _loadGameEventsFilter;
    private EcsFilterInject<Inc<ChangeGunPartDescriptionEvent>> _changeGunPartDescriptionEventsFilter;
    private EcsFilterInject<Inc<TransportMoneyEvent>> _transportMoneyEventsFilter;

    public void Init(IEcsSystems systems)
    {
        Debug.Log("Check fast load");
        _sceneData.Value.dropedItemsUIView.Construct(_world.Value);
        _sceneData.Value.mainMenuView.buttons[0].onClick.AddListener(ContinueButtonAction);
        _sceneData.Value.mainMenuView.buttons[1].onClick.AddListener(OpenSettings);
        _sceneData.Value.dropedItemsUIView.showItemDescriptionButton.onClick.AddListener(ShowItemDescription);
        _sceneData.Value.dropedItemsUIView.showItemInfoButton.onClick.AddListener(ShowItemInfo);
        _sceneData.Value.dropedItemsUIView.showPlayerStatsButton.onClick.AddListener(ShowPlayerStats);
        _sceneData.Value.dropedItemsUIView.closeSettingsButton.onClick.AddListener(CloseSettings);
        _sceneData.Value.dropedItemsUIView.uiScalerSlider.onValueChanged.AddListener(delegate { ChangeUIMenusScale(); });
        _sceneData.Value.dropedItemsUIView.craftItemButton.onClick.AddListener(TryCraftItem);
        _sceneData.Value.dropedItemsUIView.firstMoneyButton.onClick.AddListener(TransportMoneyToStorage);
        _sceneData.Value.dropedItemsUIView.secondMoneyButton.onClick.AddListener(TransportMoneyToInventory);
        _sceneData.Value.dropedItemsUIView.moneySlider.onValueChanged.AddListener(delegate { ChangeMoneySliderText(); });

        foreach (var gunPartCellView in _sceneData.Value.dropedItemsUIView.gunPartCells)
        {
            gunPartCellView.Construct(_world.Value);
        }
        _craftingTableComponentsPool.Value.Add(_sceneData.Value.playerEntity);

    }

    public void ChangeMoneySliderText()
    {
        _sceneData.Value.dropedItemsUIView.moneyTransportText.text = _sceneData.Value.dropedItemsUIView.moneySlider.value + "/" + _sceneData.Value.dropedItemsUIView.moneySlider.maxValue;
    }
    public void TransportMoneyToStorage()
    {
        _transportMoneyEventsPool.Value.Add(_world.Value.NewEntity()).getMoney = false;
    }

    public void TransportMoneyToInventory()
    {
        _transportMoneyEventsPool.Value.Add(_world.Value.NewEntity()).getMoney = true;
    }

    public void ChangeGunDescription(ItemInfo item, GunInventoryCellComponent gunInInvCellCmp, GunLevelInfoElement weaponLevelCmp)
    {
      //  Debug.Log(Mathf.CeilToInt((float)item.gunInfo.damage * (1 + (weaponLevelCmp.weaponExpLevel * 0.02f))) + "+dam" + item.gunInfo.damage + "defdam");
        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Gun info" + "\n" + "Damage: " + Mathf.CeilToInt((float)item.gunInfo.damage * (1 + (weaponLevelCmp.weaponExpLevel * 0.02f))) + "\n" + "Shot couldown: " + item.gunInfo.attackCouldown + "\n" + "Max magazine capacity: " + item.gunInfo.magazineCapacity + "\n" + "Reload time: " + item.gunInfo.reloadDuration
                        + "\n" + "Spread: " + item.gunInfo.minSpread + "to" + item.gunInfo.maxSpread + "\n" + "Shot distance: " + item.gunInfo.attackLenght + "\n" + "Shot sound distance: " + item.gunInfo.shotSoundDistance + "\n" + "Bullet: " + _sceneData.Value.idItemslist.items[item.gunInfo.bulletTypeId].itemName + "\n";

        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Durability points: " + gunInInvCellCmp.gunDurability + "/" + item.gunInfo.maxDurabilityPoints + "\n";

        if (item.gunInfo.isOneBulletReloaded)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "reloads one bullet at a time" + "\n";
        else
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "reloads immediately" + "\n";
        if (item.gunInfo.isAuto)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "is Auto" + "\n";
        else
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "is one shoted" + "\n";

        if (item.gunInfo.upgradedGunId != 0)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "can be upgrade if gun level more than " + item.gunInfo.neededGunLevelToUpgrade + "\n";

        if (weaponLevelCmp.weaponExpLevel >= 2)
        {
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "gun bonuses from level: \n ui with bullets count\n";
            if (weaponLevelCmp.weaponExpLevel >= 4)
            {
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "visual indicator of recoil \n";
                if (weaponLevelCmp.weaponExpLevel >= 6)
                {
                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "can reloading in scope \n";
                    if (weaponLevelCmp.weaponExpLevel >= 8 && !item.gunInfo.isOneHandedGun)
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "can use with sheild, but use " + (item.gunInfo.attackCouldown * gunInInvCellCmp.currentGunWeight * 2f) + " stamina to shoot \n";
                }
            }
        }
    }

    public void ChangeGunPartDescription(ItemInfo item)
    {
        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Gun part tupe: " + item.gunPartInfo.gunPartType + "\n Needed gun level to equip: " + item.gunPartInfo.neededLevelToEquip + "\n";
        if (item.gunPartInfo.reloadSpeedMultiplayer > 0)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Reload speed multiplayer: " + (item.gunPartInfo.reloadSpeedMultiplayer * 100f) + "%\n";
        if (item.gunPartInfo.cameraSpreadMultiplayer > 0)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Camera add spread multiplayer: " + (item.gunPartInfo.cameraSpreadMultiplayer * 100f) + "%\n";
        if (item.gunPartInfo.spreadMultiplayer > 0)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Gun spread multiplayer: " + (item.gunPartInfo.spreadMultiplayer * 100f) + "%\n";
        if (item.gunPartInfo.attackLenght > 0)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Add shot lenght: " + item.gunPartInfo.attackLenght + "\n";
        if (item.gunPartInfo.weaponChangeSpeedMultiplayer > 0)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Gun change speed multiplayer: " + (item.gunPartInfo.weaponChangeSpeedMultiplayer * 100f) + "%\n";

        //for scope
        if (item.gunPartInfo.scopeMultiplicity > 0)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Scope multiplicity: x" + item.gunPartInfo.scopeMultiplicity + "\n";

        //for laser pointer
        if (item.gunPartInfo.energyToCharge > 0)
        {
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Laser lenght: " + item.gunPartInfo.laserMaxLenght + "\nEnergy capacity:" + item.gunPartInfo.energyToCharge;
            if (_laserPointerForGunComponentsPool.Value.Has(_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).lastMarkedCell))
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "\nremaining Work time: " + _laserPointerForGunComponentsPool.Value.Get(_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).lastMarkedCell).remainingLaserPointerTime + "/" + item.gunPartInfo.laserLightTime + "\n";
            else
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "\nwork time: " + item.gunPartInfo.laserLightTime + "\n";
        }
    }
    public void Run(IEcsSystems systems)
    {
        foreach (var transporMoneyEvt in _transportMoneyEventsFilter.Value)
        {
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            menusStatesCmp.inMoneyTransportState = !menusStatesCmp.inMoneyTransportState;
            bool isFirstState = _transportMoneyEventsPool.Value.Get(transporMoneyEvt).getMoney;
            if (!menusStatesCmp.inMoneyTransportState)
            {
                _sceneData.Value.dropedItemsUIView.firstMoneyButtonImage.sprite = _sceneData.Value.dropedItemsUIView.getMoneyIcon;
                _sceneData.Value.dropedItemsUIView.secondMoneyButtonImage.sprite = _sceneData.Value.dropedItemsUIView.takeMoneyIcon;
                if (_sceneData.Value.dropedItemsUIView.moneySlider.value != 0)
                {
                    ref var invCmp = ref _inventoryComponentsPool.Value.Get(_sceneData.Value.inventoryEntity);
                    ref var storageInvCmp = ref _inventoryComponentsPool.Value.Get(_sceneData.Value.storageEntity);
                    if (menusStatesCmp.transportMoneyToInventory)
                    {
                        invCmp.moneyCount -= (int)_sceneData.Value.dropedItemsUIView.moneySlider.value;
                        storageInvCmp.moneyCount += (int)_sceneData.Value.dropedItemsUIView.moneySlider.value;
                    }
                    else
                    {
                        invCmp.moneyCount += (int)_sceneData.Value.dropedItemsUIView.moneySlider.value;
                        storageInvCmp.moneyCount -= (int)_sceneData.Value.dropedItemsUIView.moneySlider.value;
                    }
                    _sceneData.Value.moneyText.text = invCmp.moneyCount + "$";
                    _sceneData.Value.dropedItemsUIView.storageMoneyCountText.text = storageInvCmp.moneyCount + "$";
                }
            }
            else
            {
                _sceneData.Value.dropedItemsUIView.moneySlider.value = 0;
                _sceneData.Value.dropedItemsUIView.firstMoneyButtonImage.sprite = _sceneData.Value.dropedItemsUIView.agreeIcon;
                _sceneData.Value.dropedItemsUIView.secondMoneyButtonImage.sprite = _sceneData.Value.dropedItemsUIView.exitIcon;
                menusStatesCmp.transportMoneyToInventory = isFirstState;

                if (isFirstState)
                    _sceneData.Value.dropedItemsUIView.moneySlider.maxValue = _inventoryComponentsPool.Value.Get(_sceneData.Value.inventoryEntity).moneyCount;

                else
                    _sceneData.Value.dropedItemsUIView.moneySlider.maxValue = _inventoryComponentsPool.Value.Get(_sceneData.Value.storageEntity).moneyCount;

                _sceneData.Value.dropedItemsUIView.moneyTransportText.text = "0/" + _sceneData.Value.dropedItemsUIView.moneySlider.maxValue;
            }
            _sceneData.Value.dropedItemsUIView.moneySlider.gameObject.SetActive(menusStatesCmp.inMoneyTransportState);
            _transportMoneyEventsPool.Value.Del(transporMoneyEvt);
        }

        foreach (var loadGame in _loadGameEventsFilter.Value)
        {
            int craftingTableLevel = _craftingTableComponentsPool.Value.Get(_sceneData.Value.playerEntity).craftingTableLevel;
            foreach (var craftItemCellView in _sceneData.Value.dropedItemsUIView.craftCells)
            {
                craftItemCellView.Construct(_world.Value);
                bool showCraftCell = craftItemCellView.craftRecipeInfo.craftedItemId != 999 && craftingTableLevel >= craftItemCellView.craftRecipeInfo.needCraftingTableLevel || craftItemCellView.craftRecipeInfo.craftedItemId == 999 && craftingTableLevel < craftItemCellView.craftRecipeInfo.craftedItemsCount && craftingTableLevel >= craftItemCellView.craftRecipeInfo.needCraftingTableLevel;
                craftItemCellView.gameObject.SetActive(showCraftCell);
                if (showCraftCell)
                {
                    if (craftItemCellView.craftRecipeInfo.craftedItemId != 999)
                        craftItemCellView.craftCellItemImage.sprite = _sceneData.Value.idItemslist.items[craftItemCellView.craftRecipeInfo.craftedItemId].itemSprite;
                    else
                        craftItemCellView.craftCellItemImage.sprite = _sceneData.Value.craftingTablesSprites[craftItemCellView.craftRecipeInfo.craftedItemsCount - 1];
                }
            }
            _loadGameEventsPool.Value.Del(loadGame);
        }
        foreach (var openCraftingTable in _openCraftingTableEventsFilter.Value)
        {
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (menusStatesCmp.mainMenuState != MenuStatesComponent.MainMenuState.none) return;
            _currentDialogeComponentsPool.Value.Get(_sceneData.Value.playerEntity).currentDialogeNumber = openCraftingTable;
            ChangeCraftingMenuState(ref menusStatesCmp);
            ChangeInventoryMenuState(ref menusStatesCmp);
        }

        foreach (var gunWorkshopOpen in _gunWorkshopOpenEventsFilter.Value)
        {
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (menusStatesCmp.mainMenuState != MenuStatesComponent.MainMenuState.none) return;
            _currentDialogeComponentsPool.Value.Get(_sceneData.Value.playerEntity).currentDialogeNumber = gunWorkshopOpen;
            ChangeGunWorkshopMenuState(ref menusStatesCmp);
            ChangeInventoryMenuState(ref menusStatesCmp);
        }
        foreach (var chGunPartDescriptionEntity in _changeGunPartDescriptionEventsFilter.Value)
        {
            var gunPartCellView = _changeGunPartDescriptionEvent.Value.Get(chGunPartDescriptionEntity).gunPartCellView;
            var gunPartItemInfo = _sceneData.Value.idItemslist.items[_gunInventoryCellComponentsPool.Value.Get(_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).lastMarkedCell).gunPartsId[(int)gunPartCellView.cellGunPartType]];//получаем айтем инфу обвеса
            if (gunPartCellView.isSelected)
            {
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text = "<b>" + gunPartItemInfo.itemName + "</b>" + "\n" + "Type: " + gunPartItemInfo.type + "\n";
                ChangeGunPartDescription(gunPartItemInfo);
            }
            else
            {
                int gunEntity = _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).lastMarkedCell;
                // Debug.Log("changed gun desc" + gunEntity + " gp desc " + (_gunInventoryCellComponentsPool.Value.Get(_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).lastMarkedCell).gunPartsId[(int)gunPartCellView.cellGunPartType]));
                var gunItem = _inventoryItemComponentPool.Value.Get(gunEntity).itemInfo;
                //  Debug.Log(gunItem.itemName);
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text = "<b>" + gunItem.itemName + "</b>" + "\n" + "Type: " + gunItem.type + "\n" + "Weight: " + _gunInventoryCellComponentsPool.Value.Get(_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).lastMarkedCell).currentGunWeight + "\n";
                ChangeGunDescription(gunItem, _gunInventoryCellComponentsPool.Value.Get(gunEntity), _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).weaponsExp[gunItem.itemId]);
            }
            _changeGunPartDescriptionEvent.Value.Del(chGunPartDescriptionEntity);
        }

        foreach (var tryEquipGunPart in _tryEquipGunPartEventsFilter.Value)
        {
            if (_equipGunPartEventsFilter.Value.GetEntitiesCount() > 0) return;
            var gunPartCellView = _tryEquipGunPartEventsPool.Value.Get(tryEquipGunPart).cellViewGunPart;
            _equipGunPartEventsPool.Value.Add(tryEquipGunPart).cellGunPartType = gunPartCellView.cellGunPartType;
            Debug.Log("Equip");
            _sceneData.Value.dropedItemsUIView.markInventoryCellBorder.transform.position = gunPartCellView.transform.position;
            _tryEquipGunPartEventsPool.Value.Del(tryEquipGunPart);
            //выделение этой клетки
        }

        foreach (var playerDeath in _playerDeathEventsFilter.Value)
        {
            var mainMenuView = _sceneData.Value.mainMenuView;
            mainMenuView.buttons[0].GetComponentInChildren<TMP_Text>().text = "revive";
            mainMenuView.buttons[1].gameObject.SetActive(false);

            _inventoryComponentsPool.Value.Get(_sceneData.Value.inventoryEntity).moneyCount = 0;
            _sceneData.Value.moneyText.text = "0$";

            ref var menuStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            ChangeMainMenuState(ref menuStatesCmp, false);
            if (menuStatesCmp.inInventoryState)
                ChangeInventoryMenuState(ref menuStatesCmp);
            var playerInputView = _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view.playerInputView;
            if (_currentInteractedCharactersComponentsPool.Value.Get(playerDeath).isNPCNowIsUsed)
            {
                ref var curInteractedCharCmp = ref _currentInteractedCharactersComponentsPool.Value.Get(playerDeath);
                curInteractedCharCmp.isNPCNowIsUsed = false;
                //curInteractedCharCmp.interactCharacterView = null;
                ref var currentDialogeCnp = ref _currentDialogeComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                currentDialogeCnp.currentDialogeNumber = 0;
                currentDialogeCnp.dialogeIsStarted = false;
                _sceneData.Value.dropedItemsUIView.dialogeText.text = "";
                _sceneData.Value.dropedItemsUIView.characterNameText.text = "";
            }
        }
        foreach (var playerRevive in _revivePlayerEventsFilter.Value)
        {
            var mainMenuView = _sceneData.Value.mainMenuView;
            ChangeMainMenuState(ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity), true);
          //  _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).mainMenuState = MenuStatesComponent.MainMenuState.none;
            mainMenuView.buttons[0].GetComponentInChildren<TMP_Text>().text = "continue";
            mainMenuView.buttons[1].gameObject.SetActive(true);
        }
        foreach (var storageOpen in _storageOpenEventsFilter.Value)
        {
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (menusStatesCmp.mainMenuState != MenuStatesComponent.MainMenuState.none) return;
            ChangeStorageMenuState(ref menusStatesCmp);
            ChangeInventoryMenuState(ref menusStatesCmp);
        }
        foreach (var shopOpen in _shopOpenEventsFilter.Value)
        {
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            if (menusStatesCmp.mainMenuState != MenuStatesComponent.MainMenuState.none) return;
            _sceneData.Value.dropedItemsUIView.shopperMoneyToBuy.text = "shopper money " + _shopCharacterComponentsPool.Value.Get(_currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).interactCharacterView._entity).remainedMoneyToBuy + "$";
            _sceneData.Value.dropedItemsUIView.storageButtonImage.sprite = _sceneData.Value.dropedItemsUIView.sellItemInventoryIcon;
            ChangeShopMenuState(ref menusStatesCmp);
            ChangeInventoryMenuState(ref menusStatesCmp);
        }

        foreach (var desription in _setDescriptionItemEventsFilter.Value)
        {
            ref var menuStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            var item = _inventoryItemComponentPool.Value.Get(desription);

            _sceneData.Value.dropedItemsUIView.weaponLevelExpContainer.gameObject.SetActive(item.itemInfo.type == ItemInfo.itemType.gun || item.itemInfo.type == ItemInfo.itemType.meleeWeapon);

            foreach (var tryEquipGunPartEntity in _equipGunPartEventsFilter.Value)
            {
                ref var tryEquipGunCmp = ref _equipGunPartEventsPool.Value.Get(tryEquipGunPartEntity);
                int curWeaponLevel = _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).weaponsExp[_inventoryItemComponentPool.Value.Get(menuStatesCmp.lastMarkedCell).itemInfo.itemId].weaponExpLevel;
                if (item.itemInfo.type == ItemInfo.itemType.gunPart)
                {
                    if (_inventoryItemComponentPool.Value.Has(menuStatesCmp.lastMarkedCell) && item.itemInfo.gunPartInfo.gunPartType == tryEquipGunCmp.cellGunPartType && curWeaponLevel >= item.itemInfo.gunPartInfo.neededLevelToEquip)
                    {
                        ref var gunInInvCmp = ref _gunInventoryCellComponentsPool.Value.Get(menuStatesCmp.lastMarkedCell);
                        int needIndex = (int)item.itemInfo.gunPartInfo.gunPartType;

                        gunInInvCmp.gunPartsId[needIndex] = item.itemInfo.itemId;
                        if (item.itemInfo.gunPartInfo.energyToCharge != 0)
                            _laserPointerForGunComponentsPool.Value.Add(menuStatesCmp.lastMarkedCell).remainingLaserPointerTime = item.itemInfo.gunPartInfo.laserLightTime;

                        _sceneData.Value.dropedItemsUIView.gunPartCells[needIndex].gunPartImage.sprite = item.itemInfo.itemSprite;
                        _sceneData.Value.dropedItemsUIView.gunPartCells[needIndex].isUsed = true;

                        if (menuStatesCmp.lastMarkedCell == _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity).curEquipedWeaponCellEntity)
                        {
                            if (menuStatesCmp.lastMarkedCell == _sceneData.Value.firstGunCellView._entity)//смена первого гана если он сейчас экипирован чтобы применить все новые компоненты
                                _changeWeaponFromInventoryEventsPool.Value.Add(menuStatesCmp.lastMarkedCell).SetValues(false, 0);
                            else
                                _changeWeaponFromInventoryEventsPool.Value.Add(menuStatesCmp.lastMarkedCell).SetValues(false, 1);
                        }

                        gunInInvCmp.currentGunWeight += item.itemInfo.itemWeight;
                        _inventoryComponentsPool.Value.Get(_sceneData.Value.inventoryEntity).weight += item.itemInfo.itemWeight;

                        _deleteItemEventsPool.Value.Add(desription).count = 1;//т.к. обвесы только по одному стакаютс€

                        //return;
                    }
                    else if (item.itemInfo.gunPartInfo.gunPartType != tryEquipGunCmp.cellGunPartType)
                        _sceneData.Value.ShowWarningText("now you choose " + tryEquipGunCmp.cellGunPartType + ", but not " + item.itemInfo.gunPartInfo.gunPartType);
                    else if (curWeaponLevel < item.itemInfo.gunPartInfo.neededLevelToEquip)
                        _sceneData.Value.ShowWarningText("you need " + item.itemInfo.gunPartInfo.neededLevelToEquip + " gun level to equip " + item.itemInfo.itemName);
                }
                else
                    _sceneData.Value.ShowWarningText("you need gun part to equip");
                _equipGunPartEventsPool.Value.Del(tryEquipGunPartEntity);
            }

            var invCellCmp = _inventoryCellComponentPool.Value.Get(desription);
            if (menuStatesCmp.lastMarkedCell == desription) continue;
            invCellCmp.cellView.inventoryCellAnimator.SetBool("buttonIsActive", true);
            if (menuStatesCmp.lastMarkedCell != 0)
                _inventoryCellComponentPool.Value.Get(menuStatesCmp.lastMarkedCell).cellView.inventoryCellAnimator.SetBool("buttonIsActive", false);
            _sceneData.Value.dropedItemsUIView.markInventoryCellBorder.transform.position = invCellCmp.cellView.transform.position;
            menuStatesCmp.lastMarkedCell = desription;

            if (item.itemInfo.type == ItemInfo.itemType.gun)
                _sceneData.Value.dropedItemsUIView.gunPartsCellsContainer.gameObject.SetActive(true);
            else
                _sceneData.Value.dropedItemsUIView.gunPartsCellsContainer.gameObject.SetActive(false);

            if (!_sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.activeSelf)
                _sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.SetActive(true);

            if (_specialInventoryCellTagsPool.Value.Has(desription))
            {
                _sceneData.Value.dropedItemsUIView.dropButton.gameObject.SetActive(false);
                _sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.SetActive(false);
                _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(false);
            }

            else if ((menuStatesCmp.inStorageState || menuStatesCmp.inShopState) && !_specialInventoryCellTagsPool.Value.Has(desription))
            {
                if (!_sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.activeInHierarchy)
                    _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(true);

                _sceneData.Value.dropedItemsUIView.dropButton.gameObject.SetActive(true);

                if (_storageCellTagsPool.Value.Has(desription))
                    _sceneData.Value.dropedItemsUIView.storageButtonImage.sprite = _sceneData.Value.dropedItemsUIView.transportInventoryIcon;
                else if (menuStatesCmp.inStorageState)
                    _sceneData.Value.dropedItemsUIView.storageButtonImage.sprite = _sceneData.Value.dropedItemsUIView.transportStorageIcon;
            }
            else
            {
                _sceneData.Value.dropedItemsUIView.dropButton.gameObject.SetActive(true);
                _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(false);
            }



            if (item.itemInfo.type == ItemInfo.itemType.gun || item.itemInfo.type == ItemInfo.itemType.meleeWeapon)//скорей всего милишка останетс€ в этой проверке
            {//помен€ть на проверку пустоты клетки
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(true);
                var weaponLevelCmp = _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).weaponsExp[item.itemInfo.itemId];

                _sceneData.Value.dropedItemsUIView.weaponLevelExpBar.fillAmount = weaponLevelCmp.weaponCurrentExp / _sceneData.Value.levelExpCounts[weaponLevelCmp.weaponExpLevel];
                _sceneData.Value.dropedItemsUIView.weaponLevelExpText.text = weaponLevelCmp.weaponCurrentExp + "/" + _sceneData.Value.levelExpCounts[weaponLevelCmp.weaponExpLevel] + " lvl." + weaponLevelCmp.weaponExpLevel;


                ref var playerInvWeaponsCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);

                if (item.itemInfo.type == ItemInfo.itemType.gun)
                {//где то тут что то помен€ть чтоб багов с отображением кнопок у роружи€ не было

                    var gunInInvCellCmp = _gunInventoryCellComponentsPool.Value.Get(desription);
                    _sceneData.Value.dropedItemsUIView.gunPartCells[0].gameObject.SetActive(item.itemInfo.gunInfo.gunParts.HasFlag(GunInfo.GunParts.butt));
                    _sceneData.Value.dropedItemsUIView.gunPartCells[1].gameObject.SetActive(item.itemInfo.gunInfo.gunParts.HasFlag(GunInfo.GunParts.scope));
                    _sceneData.Value.dropedItemsUIView.gunPartCells[2].gameObject.SetActive(item.itemInfo.gunInfo.gunParts.HasFlag(GunInfo.GunParts.downPart));
                    _sceneData.Value.dropedItemsUIView.gunPartCells[3].gameObject.SetActive(item.itemInfo.gunInfo.gunParts.HasFlag(GunInfo.GunParts.barrelPart));


                    for (int i = 0; i < gunInInvCellCmp.gunPartsId.Length; i++)
                    {
                        var gunPartCell = _sceneData.Value.dropedItemsUIView.gunPartCells[i];
                        gunPartCell.isSelected = false;
                        if (gunInInvCellCmp.gunPartsId[i] != 0)
                        {
                            gunPartCell.gunPartImage.sprite = _sceneData.Value.idItemslist.items[gunInInvCellCmp.gunPartsId[i]].itemSprite;
                            gunPartCell.isUsed = true;
                        }
                        else
                        {
                            gunPartCell.gunPartImage.sprite = _sceneData.Value.dropedItemsUIView.gunPartCells[i].defaultGunPartSprite;
                            gunPartCell.isUsed = false;
                        }
                    }

                    bool inWorkshop = menuStatesCmp.inGunWorkshopState;
                    if (inWorkshop)
                    {
                        _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = " recovery durability " + item.itemInfo.gunInfo.durabilityRecoveryCost + "$";
                        if (item.itemInfo.gunInfo.upgradedGunId != 0)
                        {
                            _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "upgrade " + item.itemInfo.gunInfo.upgradeCost + "$";
                            _sceneData.Value.dropedItemsUIView.currentGunImage.sprite = item.itemInfo.itemSprite;
                            var upgradedItem = _sceneData.Value.idItemslist.items[item.itemInfo.gunInfo.upgradedGunId];
                            _sceneData.Value.dropedItemsUIView.upgradedGunImage.sprite = upgradedItem.itemSprite;
                            _sceneData.Value.dropedItemsUIView.upgradedGunText.text = "Gun info" + "\n" + "Damage: " + upgradedItem.gunInfo.damage + "\n" + "Shot couldown: " + upgradedItem.gunInfo.attackCouldown + "\n" + "Max magazine capacity: " + upgradedItem.gunInfo.magazineCapacity + "\n" + "Reload time: " + upgradedItem.gunInfo.reloadDuration
                        + "\n" + "Spread: " + upgradedItem.gunInfo.minSpread + "to" + upgradedItem.gunInfo.maxSpread + "\n" + "Shot distance: " + upgradedItem.gunInfo.attackLenght + "\n" + "Bullet: " + _sceneData.Value.idItemslist.items[upgradedItem.gunInfo.bulletTypeId].itemName + "\n";
                            _sceneData.Value.dropedItemsUIView.upgradedGunText.text += "max Durability points: " + upgradedItem.gunInfo.maxDurabilityPoints + "\n";
                            if (upgradedItem.gunInfo.isOneBulletReloaded)
                                _sceneData.Value.dropedItemsUIView.upgradedGunText.text += "reloads one bullet at a time" + "\n";
                            else
                                _sceneData.Value.dropedItemsUIView.upgradedGunText.text += "reloads immediately" + "\n";
                            if (upgradedItem.gunInfo.isAuto)
                                _sceneData.Value.dropedItemsUIView.upgradedGunText.text += "is Auto" + "\n";
                            else
                                _sceneData.Value.dropedItemsUIView.upgradedGunText.text += "is one shoted" + "\n";

                        }
                    }

                    else if (desription == _sceneData.Value.firstGunCellView._entity || desription == _sceneData.Value.secondGunCellView._entity)
                    {
                        _sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.SetActive(false);
                        _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "take off";
                    }
                    else if (menuStatesCmp.inStorageState)
                    {
                        if (_laserPointerForGunComponentsPool.Value.Has(desription))
                        {
                            _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(true);
                            _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "charge laser pointer " + _sceneData.Value.idItemslist.items[gunInInvCellCmp.gunPartsId[2]].gunPartInfo.energyToCharge + "mAh";
                        }
                        if (_storageCellTagsPool.Value.Has(desription))
                            _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
                        else
                            _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "equip";
                    }

                    else
                        _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "equip";
                }

                else
                {
                    if (desription == _sceneData.Value.meleeWeaponCellView._entity)
                        _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
                    else
                        _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "change melee";
                }
                //мб выдел€ть красным цветом низкую прочность оруж€
            }
            else if (item.itemInfo.type == ItemInfo.itemType.addedExpItem)
            {
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(true);
                _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "read";
                /* _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "added " + item.itemInfo.expAddedInfo.addedExpCount + " exp points to";
                 switch (item.itemInfo.expAddedInfo.statId)
                 {
                     case 0:
                         _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += " movement speed stat\n";
                         break;
                     case 1:
                         _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += " possession of guns stat\n";
                         break;
                     case 2:
                         _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += " possession of melee weapons stat\n";
                         break;
                     case 3:
                         _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += " endurance stat\n";
                         break;
                     case 4:
                         _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += " healing speed stat\n";
                         break;
                 }*/

            }

            else if (item.itemInfo.type == ItemInfo.itemType.heal || item.itemInfo.type == ItemInfo.itemType.randomHeal || item.itemInfo.type == ItemInfo.itemType.grenade)
            {
                if (item.itemInfo.type != ItemInfo.itemType.randomHeal || _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).hasForestGuide)
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(true);
                else
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
                //var healItemCellCmp = _inventoryCellComponentPool.Value.Get(descriptionEvt.itemEntity);
                if (item.itemInfo.type == ItemInfo.itemType.heal || item.itemInfo.type == ItemInfo.itemType.randomHeal)
                {
                    //  _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Healed health points: " + item.itemInfo.healInfo.healingHealthPoints + "\n" + "Heal time: " + item.itemInfo.healInfo.healingTime + "\n";
                    //кнопка использовать
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(true);
                    if (item.itemInfo.type == ItemInfo.itemType.heal && item.itemInfo.healInfo.recoveringHungerPoints != 0 || item.itemInfo.type == ItemInfo.itemType.randomHeal)
                        _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "eating";
                    else
                        _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "use heal";
                }
                else
                {
                    //   _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Max throw distance: " + item.itemInfo.grenadeInfo.maxThrowDistance + "\n" + "Explode time: " + item.itemInfo.grenadeInfo.timeToExplode + "\n" + "Damage: " + item.itemInfo.grenadeInfo.damage + "\n" + "Explode radius: " + item.itemInfo.grenadeInfo.explodeRadius + "\n";
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
                }
                if (desription == _sceneData.Value.healingItemCellView._entity || desription == _sceneData.Value.grenadeCellView._entity /*&& !healItemCellCmp.isEmpty*/)
                {
                    // if (menusStatesCmp.inStorageState)
                    //  _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "take off all";
                }
                else
                {
                    if (item.itemInfo.type != ItemInfo.itemType.randomHeal)
                        _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "equip all";
                    else
                        _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "divide on safe and poisons";
                }
            }

            else if (item.itemInfo.type == ItemInfo.itemType.gunPart)
            {
                ChangeGunPartDescription(item.itemInfo);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
            }
            else if (item.itemInfo.itemId == 44)
            {
                // var flashlightItemCellCmp = _inventoryCellComponentPool.Value.Get(descriptionEvt.itemEntity);
                // ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                // float curTime = 1440 * globalTimeCmp.currentDayTime / _sceneData.Value.dayTime;

                // int hours = (int)(curTime / 60) + _sceneData.Value.timeHourOffset;
                //  if (hours > 23)
                //     hours -= 24;
                // _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Time: " + hours.ToString() + ":" + ((int)(curTime % 60)).ToString("00") + "\n";
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
            }//watch

            else if (item.itemInfo.type == ItemInfo.itemType.backpack)
            {
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(true);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);

                if (_storageCellTagsPool.Value.Has(desription))
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);

                else if (desription == _sceneData.Value.backpackCellView._entity)
                {
                    _sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "take off";
                }
                else
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "equip";
            }

            else if (item.itemInfo.type == ItemInfo.itemType.flashlight)
            {
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
                if (desription == _sceneData.Value.flashlightItemCellView._entity)
                {
                    _sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "take off";
                }
                else if (menuStatesCmp.inStorageState && _flashlightInInventoryComponentsPool.Value.Get(desription).currentDurability != item.itemInfo.flashlightInfo.maxChargedTime)
                {
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(true);
                    if (item.itemInfo.flashlightInfo.isElectric)
                        _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "charge";
                    else
                        _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "fill";
                }
                else
                {
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "equip";
                }

                if (_storageCellTagsPool.Value.Has(desription))
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
                else
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(true);
            }

            else if (item.itemInfo.type == ItemInfo.itemType.sheild || item.itemInfo.type == ItemInfo.itemType.helmet || item.itemInfo.type == ItemInfo.itemType.bodyArmor)
            {
                /* _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Remaining durability : " + ((int)_shieldComponentsPool.Value.Get(desription).currentDurability).ToString() + " / " + item.itemInfo.sheildInfo.sheildDurability + "\n" + " shield recovery cost " + item.itemInfo.sheildInfo.sheildRecoveryCost + "\n"
                     + " block damage " + (item.itemInfo.sheildInfo.damagePercent * 100) + "%\n" + " added recoil and reduce hit lenght/angle " + ((1 - item.itemInfo.sheildInfo.recoilPercent) * 100) + "%\n";*/

                // _sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.SetActive(true);//
                if (menuStatesCmp.inGunWorkshopState || (item.itemInfo.type == ItemInfo.itemType.helmet && item.itemInfo.helmetInfo.addedLightIntancity != 0 && menuStatesCmp.inStorageState))
                {
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(true);
                    if (menuStatesCmp.inGunWorkshopState)
                        _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "Recovery durability";
                    else
                        _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "charge nvg";
                }
                else
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
                var specialCellEntity = 0;
                if (item.itemInfo.type == ItemInfo.itemType.sheild)
                    specialCellEntity = _sceneData.Value.shieldCellView._entity;
                else if (item.itemInfo.type == ItemInfo.itemType.bodyArmor)
                    specialCellEntity = _sceneData.Value.bodyArmorCellView._entity;
                else
                    specialCellEntity = _sceneData.Value.helmetCellView._entity;
                if (desription == specialCellEntity)
                {
                    _sceneData.Value.dropedItemsUIView.divideItemsUI.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "take off";
                }
                /*  else if (_storageCellTagsPool.Value.Has(descriptionEvt.itemEntity))
                  {
                      _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "charge";
                  }*/
                else
                {
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "equip";
                }

                if (_storageCellTagsPool.Value.Has(desription))
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
                else
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(true);

            }

            else
            {
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
            }

            if (menuStatesCmp.currentItemShowedInfo == MenuStatesComponent.CurrentItemShowedInfoState.itemInfo)
                SetItemInfo(menuStatesCmp.lastMarkedCell);

            else if (menuStatesCmp.currentItemShowedInfo == MenuStatesComponent.CurrentItemShowedInfoState.itemDescription)
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text = "<b>" + item.itemInfo.itemName + "</b>" + "\n" + item.itemInfo.itemDescription;

            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(true);
            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(item.currentItemsCount, desription);
        }

        if (!_currentDialogeComponentsPool.Value.Get(_sceneData.Value.playerEntity).dialogeIsStarted)
        {
            if ((Input.GetKeyDown(KeyCode.I) || (Input.GetKeyDown(KeyCode.Escape)) && _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inInventoryState))
            {
               // Debug.Log("open inv");
                ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                if (menusStatesCmp.mainMenuState != MenuStatesComponent.MainMenuState.none) return;
                _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);

                ChangeInventoryMenuState(ref menusStatesCmp);

                if (menusStatesCmp.inShopState)
                {
                    ChangeShopMenuState(ref menusStatesCmp);
                    _shopCloseEventsPool.Value.Add(_world.Value.NewEntity());
                    _currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).isNPCNowIsUsed = false;
                }
                else if (menusStatesCmp.inGunWorkshopState)
                {
                    ChangeGunWorkshopMenuState(ref menusStatesCmp);
                    _currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).isNPCNowIsUsed = false;
                }
                else if (menusStatesCmp.inStorageState)
                {
                    ChangeStorageMenuState(ref menusStatesCmp);

                    _currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).isNPCNowIsUsed = false;
                }
                else if (menusStatesCmp.inQuestHelperState)
                    ChangeQuestHelperState(ref menusStatesCmp);
                else if (menusStatesCmp.inCraftingTableState)
                    ChangeCraftingMenuState(ref menusStatesCmp);
            }
            else if (Input.GetKeyDown(KeyCode.J) && !_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inInventoryState)
            {
                ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                if (menusStatesCmp.mainMenuState != MenuStatesComponent.MainMenuState.none) return;

                ChangeQuestHelperState(ref menusStatesCmp);
                ChangeInventoryMenuState(ref menusStatesCmp);

                _openQuestHelperEventsPool.Value.Add(_sceneData.Value.playerEntity);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && !_menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inInventoryState)
            {
                ChangeMainMenuState(ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity), false);
            }
        }
    }
    private void ShowItemDescription()
    {
        ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        if (menusStatesCmp.currentItemShowedInfo == MenuStatesComponent.CurrentItemShowedInfoState.itemDescription) return;

        if (menusStatesCmp.currentItemShowedInfo == MenuStatesComponent.CurrentItemShowedInfoState.playerStats)
        {
            _sceneData.Value.dropedItemsUIView.itemUIVerticalLayout.gameObject.SetActive(true);
            _sceneData.Value.dropedItemsUIView.playerStatsContainer.gameObject.SetActive(false);
            _sceneData.Value.dropedItemsUIView.markInventoryCellBorder.gameObject.SetActive(true);
        }

        menusStatesCmp.currentItemShowedInfo = MenuStatesComponent.CurrentItemShowedInfoState.itemDescription;
        if (_inventoryItemComponentPool.Value.Has(menusStatesCmp.lastMarkedCell))
        {
            var item = _inventoryItemComponentPool.Value.Get(menusStatesCmp.lastMarkedCell);
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text = "<b>" + item.itemInfo.itemName + "</b>" + "\n" + item.itemInfo.itemDescription;
        }
        else
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text = "Choose item to check description";
    }

    private void ShowItemInfo()
    {
        ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        if (menusStatesCmp.currentItemShowedInfo == MenuStatesComponent.CurrentItemShowedInfoState.itemInfo) return;

        if (menusStatesCmp.currentItemShowedInfo == MenuStatesComponent.CurrentItemShowedInfoState.playerStats)
        {
            _sceneData.Value.dropedItemsUIView.itemUIVerticalLayout.gameObject.SetActive(true);
            _sceneData.Value.dropedItemsUIView.playerStatsContainer.gameObject.SetActive(false);
            _sceneData.Value.dropedItemsUIView.markInventoryCellBorder.gameObject.SetActive(true);
        }

        menusStatesCmp.currentItemShowedInfo = MenuStatesComponent.CurrentItemShowedInfoState.itemInfo;
        if (_inventoryItemComponentPool.Value.Has(menusStatesCmp.lastMarkedCell))
            SetItemInfo(menusStatesCmp.lastMarkedCell);
        else
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text = "Choose item to check info";
    }

    private void SetItemInfo(int desription)
    {
        ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
        var item = _inventoryItemComponentPool.Value.Get(desription);

        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text = "<b>" + item.itemInfo.itemName + "</b>\nType: " + item.itemInfo.type + "\n";
        if (item.itemInfo.type == ItemInfo.itemType.gun)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Weight: " + _gunInventoryCellComponentsPool.Value.Get(desription).currentGunWeight.ToString("0.00") + "\n";

        else
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Weight: " + item.itemInfo.itemWeight * item.currentItemsCount + "\n";

        if (item.itemInfo.type == ItemInfo.itemType.gun || item.itemInfo.type == ItemInfo.itemType.meleeWeapon)//скорей всего милишка останетс€ в этой проверке
        {
            var weaponLevelCmp = _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity).weaponsExp[item.itemInfo.itemId];



            ref var playerInvWeaponsCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            if (item.itemInfo.type == ItemInfo.itemType.gun)
            {//где то тут что то помен€ть чтоб багов с отображением кнопок у роружи€ не было

                var gunInInvCellCmp = _gunInventoryCellComponentsPool.Value.Get(desription);

                ChangeGunDescription(item.itemInfo, gunInInvCellCmp, weaponLevelCmp);

            }

            else
            {
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Melee weapon info\nDamage: " + Mathf.CeilToInt((float)item.itemInfo.meleeWeaponInfo.damage * (1 + (weaponLevelCmp.weaponExpLevel * 0.02f))) + "\nAttack couldown: " + item.itemInfo.meleeWeaponInfo.attackCouldown /*+ "\nHit lenght: " + item.itemInfo.meleeWeaponInfo.attackLenght + "\n" + "Hit speed: " + item.itemInfo.meleeWeaponInfo.attackSpeed + "\n"*/
                    + "\nwide hit damage x" + item.itemInfo.meleeWeaponInfo.wideAttackDamageMultiplayer + "\nstamina usage " + item.itemInfo.meleeWeaponInfo.staminaUsage + "\n";
            }
        }
        else if (item.itemInfo.type == ItemInfo.itemType.addedExpItem)
        {
            _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "read";
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "added " + item.itemInfo.expAddedInfo.addedExpCount + " exp points to";
            switch (item.itemInfo.expAddedInfo.statId)
            {
                case 0:
                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += " movement speed stat\n";
                    break;
                case 1:
                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += " possession of guns stat\n";
                    break;
                case 2:
                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += " possession of melee weapons stat\n";
                    break;
                case 3:
                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += " endurance stat\n";
                    break;
                case 4:
                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += " healing speed stat\n";
                    break;
            }

        }

        else if (item.itemInfo.type == ItemInfo.itemType.heal)
        {
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Healed health points: " + item.itemInfo.healInfo.healingHealthPoints + "\n" + "Heal time: " + item.itemInfo.healInfo.healingTime + "\n";
            if (item.itemInfo.healInfo.maxBleedingRemoveLevel != 0)
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Heal can stop bleeding: " + item.itemInfo.healInfo.maxBleedingRemoveLevel + " lvl\n";
            else
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Heal can't stop bleeding\n";
            if (item.itemInfo.healInfo.effectInfo != null)
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Give " + item.itemInfo.healInfo.effectInfo.effectType + " effect " + item.itemInfo.healInfo.effectInfo.effectLevel + " level on " + item.itemInfo.healInfo.effectInfo.effectTime + " seconds\n";
        }

        else if (item.itemInfo.type == ItemInfo.itemType.grenade)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Max throw distance: " + item.itemInfo.grenadeInfo.maxThrowDistance + "\nExplode time: " + item.itemInfo.grenadeInfo.timeToExplode + "\nDamage: " + item.itemInfo.grenadeInfo.damage + "\nExplode radius: " + item.itemInfo.grenadeInfo.explodeRadius + "\n";

        else if (item.itemInfo.type == ItemInfo.itemType.gunPart)
        {
            ChangeGunPartDescription(item.itemInfo);
        }
        else if (item.itemInfo.itemId == 44)
        {
            ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            if (globalTimeCmp.goToLightNight)
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Days to next light night " + ((0.3f - globalTimeCmp.nightLightIntensity) / 0.1f) + "\n";
            else
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Days to next dark night " + (globalTimeCmp.nightLightIntensity / 0.1f) + "\n";

            if (globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none)
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Hours to next rain " + (globalTimeCmp.levelsToRain * 3) + "\n";
            else
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += globalTimeCmp.currentWeatherType + " duration: " + (globalTimeCmp.levelsToRain * 3) + " hours\n";

            float hours = globalTimeCmp.currentDayTime + _sceneData.Value.timeHourOffset;
            if (hours > 24)
                hours -= 24;


            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Time: " + hours + ":00\n";
        }
        else if (item.itemInfo.type == ItemInfo.itemType.helmet)
        {
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "head percent defence: " + item.itemInfo.helmetInfo.headDefenceMultiplayer * 100 + "\nresist from drops: " + item.itemInfo.helmetInfo.dropTransparentMultiplayer * 100 + "%\nhelmet durability: " + ((int)_flashlightInInventoryComponentsPool.Value.Get(desription).currentDurability).ToString() +
                "/" + item.itemInfo.helmetInfo.armorDurability + "\naudibility : " + item.itemInfo.helmetInfo.audibilityMultiplayer * 100 + "%\n";
            if (item.itemInfo.helmetInfo.autoBloodCleaning)
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "auto cleaning blood drops\n";
            if (item.itemInfo.helmetInfo.addedLightIntancity != 0)
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "night mode time" + (int)_shieldComponentsPool.Value.Get(desription).currentDurability + "/" + item.itemInfo.helmetInfo.nightTimeModeDuration + "\n";
            if (item.itemInfo.helmetInfo.fowAngleRemove != 0)
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "fov angle -" + item.itemInfo.helmetInfo.fowAngleRemove + " deg\n fov lenght -" + item.itemInfo.helmetInfo.fowLenghtRemove + " m\n";
        }
        else if (item.itemInfo.type == ItemInfo.itemType.bodyArmor)
        {
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "body percent defence: " + item.itemInfo.bodyArmorInfo.defenceMultiplayer * 100 + "\n" + "resist from bleeding: " + item.itemInfo.bodyArmorInfo.bleedingResistance * 100 + " %\nbody armor durability: " + ((int)_flashlightInInventoryComponentsPool.Value.Get(desription).currentDurability).ToString() +
                   "/" + item.itemInfo.bodyArmorInfo.armorDurability + "\n";
        }
        else if (item.itemInfo.type == ItemInfo.itemType.backpack)
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Cells count: " + item.itemInfo.backpackInfo.cellsCount + "\n";

        else if (item.itemInfo.type == ItemInfo.itemType.flashlight)
        {
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Remaining charge time : " + ((int)_flashlightInInventoryComponentsPool.Value.Get(desription).currentDurability).ToString() + " / " + item.itemInfo.flashlightInfo.maxChargedTime + "\n" +
                 "Light range : " + item.itemInfo.flashlightInfo.lightRange + "\n" + "Light intensity : " + item.itemInfo.flashlightInfo.lightIntecnsity + "\n";
            if (item.itemInfo.flashlightInfo.isElectric)
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Energy to charge: " + item.itemInfo.flashlightInfo.chargeItem + "\n";

            else
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Item to charge: " + _sceneData.Value.idItemslist.items[item.itemInfo.flashlightInfo.chargeItem].itemName + "\n";
        }

        else if (item.itemInfo.type == ItemInfo.itemType.sheild)
        {
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Remaining durability : " + ((int)_shieldComponentsPool.Value.Get(desription).currentDurability).ToString() + " / " + item.itemInfo.sheildInfo.sheildDurability + "\nshield recovery cost " + item.itemInfo.sheildInfo.sheildRecoveryCost + "\nblock damage "
                + (item.itemInfo.sheildInfo.damagePercent * 100) + "%\nadded recoil and reduce hit lenght/angle " + ((1 - item.itemInfo.sheildInfo.recoilPercent) * 100) + "%\n";

        }

        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "can sell for " + item.itemInfo.itemCost + "$\n";

    }


    private void ShowPlayerStats()
    {
        ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);

        if (menusStatesCmp.currentItemShowedInfo == MenuStatesComponent.CurrentItemShowedInfoState.playerStats) return;

        _sceneData.Value.dropedItemsUIView.itemUIVerticalLayout.gameObject.SetActive(false);
        _sceneData.Value.dropedItemsUIView.playerStatsContainer.gameObject.SetActive(true);
        _sceneData.Value.dropedItemsUIView.markInventoryCellBorder.gameObject.SetActive(false);

        menusStatesCmp.currentItemShowedInfo = MenuStatesComponent.CurrentItemShowedInfoState.playerStats;
        ref var playerStats = ref _playerUpgradedStatsPool.Value.Get(_sceneData.Value.playerEntity);
        for (int i = 0; i < playerStats.statLevels.Length; i++)
        {
            _sceneData.Value.dropedItemsUIView.statsFilledBarsText[i].text = (int)playerStats.currentStatsExp[i] + "/" + _sceneData.Value.levelExpCounts[playerStats.statLevels[i]] + " lvl:" + playerStats.statLevels[i];
            _sceneData.Value.dropedItemsUIView.statsFilledBarsImages[i].fillAmount = playerStats.currentStatsExp[i] / _sceneData.Value.levelExpCounts[playerStats.statLevels[i]];
        }
    }

    private void ChangeInventoryMenuState(ref MenuStatesComponent menusStatesCmp)
    {

        if (!menusStatesCmp.inInventoryState)
        {

            _offInScopeStateEventsPool.Value.Add(_sceneData.Value.playerEntity);
            var inventoryCmp = _inventoryComponentsPool.Value.Get(_sceneData.Value.inventoryEntity);
            _sceneData.Value.statsInventoryText.text = inventoryCmp.weight.ToString("0.0") + "kg/ " + inventoryCmp.currentMaxWeight + "kg \n max cells " + inventoryCmp.currentCellCount;
            _sceneData.Value.depthOfFieldMainBg.focalLength.value = 300f;
            _sceneData.Value.uiAudioSourse.clip = _sceneData.Value.openInventorySound;
            _sceneData.Value.uiAudioSourse.Play();
        }
        else
        {
            _sceneData.Value.depthOfFieldMainBg.focalLength.value = _cameraComponentsPool.Value.Get(_sceneData.Value.playerEntity).blurValue;
            if (menusStatesCmp.lastMarkedCell != 0)
            {
                _inventoryCellComponentPool.Value.Get(menusStatesCmp.lastMarkedCell).cellView.inventoryCellAnimator.SetBool("buttonIsActive", false);
                menusStatesCmp.lastMarkedCell = 0;
            }
            _sceneData.Value.uiAudioSourse.clip = _sceneData.Value.closeInventorySound;
            _sceneData.Value.uiAudioSourse.Play();
        }
        _sceneData.Value.dropedItemsUIView.charactersInteractText.gameObject.SetActive(menusStatesCmp.inInventoryState);
        menusStatesCmp.inInventoryState = !menusStatesCmp.inInventoryState;
        _sceneData.Value.inventoryMenuView.ChangeMenuState(menusStatesCmp.inInventoryState);
        //
        _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).canMove = !menusStatesCmp.inInventoryState;
        _currentAttackComponentsPool.Value.Get(_sceneData.Value.playerEntity).canAttack = !menusStatesCmp.inInventoryState;
        _currentInteractedCharactersComponentsPool.Value.Get(_sceneData.Value.playerEntity).isNPCNowIsUsed = menusStatesCmp.inInventoryState;
        Cursor.visible = menusStatesCmp.inInventoryState;
        //изменение состо€ни€ стрелбы и ходьбы
    }
    private void ChangeQuestHelperState(ref MenuStatesComponent menusStatesCmp)
    {
        menusStatesCmp.inQuestHelperState = !menusStatesCmp.inQuestHelperState;
        _sceneData.Value.questMenuView.ChangeMenuState(menusStatesCmp.inQuestHelperState);

    }
    private void ChangeShopMenuState(ref MenuStatesComponent menusStatesCmp)
    {
        menusStatesCmp.inShopState = !menusStatesCmp.inShopState;
        _sceneData.Value.shopMenuView.ChangeMenuState(menusStatesCmp.inShopState);
    }

    private void ChangeStorageMenuState(ref MenuStatesComponent menusStatesCmp)
    {
        menusStatesCmp.inStorageState = !menusStatesCmp.inStorageState;
        _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(menusStatesCmp.inStorageState);//
        _sceneData.Value.storageMenuView.ChangeMenuState(menusStatesCmp.inStorageState);
        //_sceneData.Value.dropedItemsUIView.moneyTransportContainer.gameObject.SetActive(menusStatesCmp.inStorageState);
    }

    private void ChangeCraftingMenuState(ref MenuStatesComponent menusStatesCmp)
    {
        menusStatesCmp.inCraftingTableState = !menusStatesCmp.inCraftingTableState;
        if (menusStatesCmp.inCraftingTableState)
        {
            _sceneData.Value.dropedItemsUIView.craftItemButton.gameObject.SetActive(false);//
            _sceneData.Value.dropedItemsUIView.craftedItemRecipeText.text = " Press crafting recipe to check needed items";
        }
        _sceneData.Value.craftingMenuView.ChangeMenuState(menusStatesCmp.inCraftingTableState);
    }

    private void ChangeMainMenuState(ref MenuStatesComponent menusStatesCmp, bool exitFromMenu)
    {

        var inMenu = menusStatesCmp.mainMenuState != MenuStatesComponent.MainMenuState.none;
        if (!exitFromMenu)
        {
            if (inMenu && menusStatesCmp.mainMenuState == MenuStatesComponent.MainMenuState.settings)
                CloseSettings();
            else
            {
                menusStatesCmp.mainMenuState = inMenu ? MenuStatesComponent.MainMenuState.none : MenuStatesComponent.MainMenuState.mainMenu;
                _sceneData.Value.mainMenuView.ChangeMenuState(!inMenu);
            }
        }
        else
        {
            menusStatesCmp.mainMenuState = MenuStatesComponent.MainMenuState.none;
            _sceneData.Value.mainMenuView.ChangeMenuState(false);
        }
         Debug.Log(menusStatesCmp.mainMenuState + " menustate");
        inMenu = menusStatesCmp.mainMenuState != MenuStatesComponent.MainMenuState.none;
        _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).canMove = !inMenu;
        _currentAttackComponentsPool.Value.Get(_sceneData.Value.playerEntity).canAttack = !inMenu;
        Cursor.visible = inMenu;
    }

    private void ChangeGunWorkshopMenuState(ref MenuStatesComponent menusStatesCmp)
    {
        menusStatesCmp.inGunWorkshopState = !menusStatesCmp.inGunWorkshopState;
        if (menusStatesCmp.inGunWorkshopState)
        {
            _sceneData.Value.dropedItemsUIView.upgradedGunText.text = "нажмите на пушку, которую хотите проапгрейдить";
            _sceneData.Value.dropedItemsUIView.currentGunImage.sprite = _sceneData.Value.transparentSprite;
            _sceneData.Value.dropedItemsUIView.upgradedGunImage.sprite = _sceneData.Value.transparentSprite;
        }

        _sceneData.Value.gunWorkshopMenuView.ChangeMenuState(menusStatesCmp.inGunWorkshopState);
    }

    private void OpenSettings()
    {
        _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).mainMenuState = MenuStatesComponent.MainMenuState.settings;
        _sceneData.Value.dropedItemsUIView.settingsContainer.gameObject.SetActive(true);
        _sceneData.Value.mainMenuView.transform.GetChild(1).gameObject.SetActive(false);
    }
    private void CloseSettings()
    {
        _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).mainMenuState = MenuStatesComponent.MainMenuState.mainMenu;
        _sceneData.Value.dropedItemsUIView.settingsContainer.gameObject.SetActive(false);
        _sceneData.Value.mainMenuView.transform.GetChild(1).gameObject.SetActive(true);//menu container
        float needScale = _sceneData.Value.dropedItemsUIView.uiScalerSlider.value;
        for (int i = 2; i < _sceneData.Value.mainMenuView.uiMenusToScale.Length; i++)
        {
            _sceneData.Value.mainMenuView.uiMenusToScale[i].localScale = Vector3.one * needScale;
        }
    }
    private void ChangeUIMenusScale()
    {
        _sceneData.Value.mainMenuView.uiMenusToScale[0].localScale = Vector3.one * _sceneData.Value.dropedItemsUIView.uiScalerSlider.value;
        _sceneData.Value.mainMenuView.uiMenusToScale[1].localScale = Vector3.one * _sceneData.Value.dropedItemsUIView.uiScalerSlider.value;
    }
    private void ContinueButtonAction()
    {
        if (_healthComponentsPool.Value.Get(_sceneData.Value.playerEntity).isDeath)
            _revivePlayerEventsPool.Value.Add(_sceneData.Value.playerEntity);
        ChangeMainMenuState(ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity), true);
        //_sceneData.Value.mainMenuView.ChangeMenuState(false);
    }

    public void TryCraftItem()
    {
        _tryCraftItemEventsPool.Value.Add(_world.Value.NewEntity());
    }
}
