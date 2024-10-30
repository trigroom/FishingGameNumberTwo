using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
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
    private EcsPoolInject<AttackComponent> _currentAttackComponentsPool;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponent;
    private EcsPoolInject<StorageCellTag> _storageCellTagsPool;
    private EcsPoolInject<NowUsedWeaponTag> _nowUsedWeaponTagsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    //private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    //private EcsPoolInject<HealingItemCellComponent> _healingItemCellComponentsPool;
    private EcsPoolInject<FlashLightInInventoryComponent> _flashlightInInventoryComponentsPool;
    private EcsPoolInject<OffInScopeStateEvent> _offInScopeStateEventsPool;

    private EcsFilterInject<Inc<SetDescriptionItemEvent>> _setDescriptionItemEventsFilter;
    private EcsFilterInject<Inc<StorageOpenEvent>> _storageOpenEventsFilter;
    private EcsFilterInject<Inc<ShopOpenEvent>> _shopOpenEventsFilter;

    public void Init(IEcsSystems systems)
    {
        _sceneData.Value.dropedItemsUIView.Construct(_world.Value);
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var storageOpen in _storageOpenEventsFilter.Value)
        {
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            ChangeStorageMenuState(ref menusStatesCmp);
            ChangeInventoryMenuState(ref menusStatesCmp);
        }
        foreach (var shopOpen in _shopOpenEventsFilter.Value)
        {
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            ChangeShopMenuState(ref menusStatesCmp);
            ChangeInventoryMenuState(ref menusStatesCmp);
        }

        foreach (var desription in _setDescriptionItemEventsFilter.Value)
        {
            ref var descriptionEvt = ref _setDescriptionItemEventsPool.Value.Get(desription);
            var item = _inventoryItemComponentPool.Value.Get(descriptionEvt.itemEntity);
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);

            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text = "<b>" + item.itemInfo.itemName + "</b>" + "\n" + "Type: " + item.itemInfo.type + "\n" + "Weight: " + item.itemInfo.itemWeight * item.currentItemsCount + "\n";

            if (!_sceneData.Value.dropedItemsUIView.dropItemsUI.gameObject.activeSelf)
                _sceneData.Value.dropedItemsUIView.dropItemsUI.gameObject.SetActive(true);

            if (menusStatesCmp.inStorageState)
            {
                if (!_sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.activeSelf)
                    _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(true);

                if (_storageCellTagsPool.Value.Has(descriptionEvt.itemEntity))
                {
                    _sceneData.Value.dropedItemsUIView.storageButtonImage.sprite = _sceneData.Value.dropedItemsUIView.transportInventoryIcon;
                }
                else
                {
                    _sceneData.Value.dropedItemsUIView.storageButtonImage.sprite = _sceneData.Value.dropedItemsUIView.transportStorageIcon;
                }
            }

            if (item.itemInfo.type == ItemInfo.itemType.gun || item.itemInfo.type == ItemInfo.itemType.meleeWeapon)//скорей всего милишка останетс€ в этой проверке
            {//помен€ть на проверку пустоты клетки
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(true);

                var invCellCmp = _inventoryCellComponentPool.Value.Get(descriptionEvt.itemEntity);
                ref var playerInvWeaponsCmp = ref _playerWeaponsInInventoryComponent.Value.Get(_sceneData.Value.playerEntity);

                if (item.itemInfo.type == ItemInfo.itemType.gun)
                {
                    var gunInInvCellCmp = _gunInventoryCellComponentsPool.Value.Get(descriptionEvt.itemEntity);
                    var playerGunCmp = _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity);

                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Gun info" + "\n" + "Damage: " + item.itemInfo.gunInfo.damage + "\n" + "Shot couldown: " + item.itemInfo.gunInfo.attackCouldown + "\n" + "Max magazine capacity: " + item.itemInfo.gunInfo.magazineCapacity + "\n" + "Reload time: " + item.itemInfo.gunInfo.reloadDuration
                        + "\n" + "Spread: " + item.itemInfo.gunInfo.minSpread + "to" + item.itemInfo.gunInfo.maxSpread + "\n" + "Shot distance: " + item.itemInfo.gunInfo.attackLenght + "\n" + "Bullet: " + _sceneData.Value.idItemslist.items[item.itemInfo.gunInfo.bulletTypeId].itemName + "\n";
                    // if (gunInInvCellCmp.isEquipedWeapon)
                    // {
                    if (_nowUsedWeaponTagsPool.Value.Has(desription))
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Durability points: " + playerGunCmp.durabilityPoints + "/" + item.itemInfo.gunInfo.maxDurabilityPoints + "\n";
                    else if (desription == _sceneData.Value.firstGunCellView._entity)
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Durability points: " + playerInvWeaponsCmp.curFirstWeaponDurability + "/" + item.itemInfo.gunInfo.maxDurabilityPoints + "\n";
                    else if (desription == _sceneData.Value.secondGunCellView._entity)
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Durability points: " + playerInvWeaponsCmp.curSecondWeaponDurability + "/" + item.itemInfo.gunInfo.maxDurabilityPoints + "\n";
                    // }
                    else
                        // {
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Durability points: " + gunInInvCellCmp.gunDurability + "/" + item.itemInfo.gunInfo.maxDurabilityPoints + "\n";
                    //  }
                    // баги с отображением дурабилити

                    if (item.itemInfo.gunInfo.isOneBulletReloaded)
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "reloads one bullet at a time" + "\n";
                    else
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "reloads immediately" + "\n";
                    if (item.itemInfo.gunInfo.isAuto)
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "is Auto" + "\n";
                    else
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "is one shoted" + "\n";


                    if (descriptionEvt.itemEntity == _sceneData.Value.firstGunCellView._entity || descriptionEvt.itemEntity == _sceneData.Value.secondGunCellView._entity)
                    {
                        if (menusStatesCmp.inStorageState)
                            _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(false);
                        _sceneData.Value.dropedItemsUIView.dropItemsUI.gameObject.SetActive(false);
                        _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "take off";
                    }
                    else if (_storageCellTagsPool.Value.Has(desription))
                        _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
                    else
                        _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "equip";
                }

                else
                {
                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Melee wapon info" + "\n" + "Damage: " + item.itemInfo.meleeWeaponInfo.damage + "\n" + "Shot couldown: " + item.itemInfo.meleeWeaponInfo.attackCouldown + "\n" + "Hit lenght: " + item.itemInfo.meleeWeaponInfo.attackLenght + "\n" + "Hit speed: " + item.itemInfo.meleeWeaponInfo.attackSpeed + "\n";

                    if (item.itemInfo.meleeWeaponInfo.isWideHit)
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Hit type: wide hit \n";
                    else
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Hit type: thrust hit \n";

                    if (item.itemInfo.meleeWeaponInfo.isAuto)
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "is Auto" + "\n";
                    else
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "is one shoted" + "\n";

                    if (descriptionEvt.itemEntity == _sceneData.Value.meleeWeaponCellView._entity)
                    {
                        if (menusStatesCmp.inStorageState)
                            _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(false);
                        _sceneData.Value.dropedItemsUIView.dropItemsUI.gameObject.SetActive(false);
                        _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
                    }
                    else
                        _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "change melee";
                    //милишку нельз€ выкинуть
                    //описание милишки
                }
                //мб выдел€ть красным цветом низкую прочность оруж€
            }

            else if (item.itemInfo.type == ItemInfo.itemType.heal)
            {
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(true);
                //var healItemCellCmp = _inventoryCellComponentPool.Value.Get(descriptionEvt.itemEntity);
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Healed health points: " + item.itemInfo.healInfo.healingHealthPoints + "\n" + "Heal time: " + item.itemInfo.healInfo.healingTime + "\n";
                //кнопка использовать
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(true);
                if (descriptionEvt.itemEntity == _sceneData.Value.healingItemCellView._entity /*&& !healItemCellCmp.isEmpty*/)
                {
                    if (menusStatesCmp.inStorageState)
                        _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.dropItemsUI.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "take off all";
                }
                else
                {
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "equip all";
                }
            }
            /*else if (item.itemInfo.type == ItemInfo.itemType.meleeWeapon)
            {
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateWeaponEquipButton(true);
                //смена на милишку
            }*/

            else if (item.itemInfo.type == ItemInfo.itemType.watch)
            {
                // var flashlightItemCellCmp = _inventoryCellComponentPool.Value.Get(descriptionEvt.itemEntity);
                ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(_sceneData.Value.playerEntity);
                float curTime = 1440 * globalTimeCmp.currentDayTime / _sceneData.Value.dayTime;

                int hours = (int)(curTime / 60) - _sceneData.Value.timeHourOffset;
                if (hours < 0)
                    hours += 24;
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Time: " + hours.ToString() + ":" + ((int)(curTime % 60)).ToString("00") + "\n";
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
            }

            else if (item.itemInfo.type == ItemInfo.itemType.flashlight)
            {
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Remaining charge time : " + ((int)_flashlightInInventoryComponentsPool.Value.Get(desription).currentChargeRemainigTime).ToString() + " / " + item.itemInfo.flashlightInfo.maxChargedTime + "\n" +
                     "Light range : " + item.itemInfo.flashlightInfo.lightRange + "\n" + "Light intensity : " + item.itemInfo.flashlightInfo.lightIntecnsity + "\n";
                if (item.itemInfo.flashlightInfo.isElectric)
                {
                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Energy to charge: " + item.itemInfo.flashlightInfo.chargeItem + "\n";
                    if (_storageCellTagsPool.Value.Has(desription))
                    {
                        _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(true);
                        _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "charge";
                    }
                }

                else
                {
                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Item to charge: " + _sceneData.Value.idItemslist.items[item.itemInfo.flashlightInfo.chargeItem].itemName + "\n";
                    _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(true);
                    _sceneData.Value.dropedItemsUIView.secondButtonActionText.text = "fill";
                }
                if (descriptionEvt.itemEntity == _sceneData.Value.flashlightItemCellView._entity)
                {
                    if (menusStatesCmp.inStorageState)
                    {
                        _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(false);
                    }
                    _sceneData.Value.dropedItemsUIView.dropItemsUI.gameObject.SetActive(false);
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
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += item.itemInfo.itemDescription;

            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(true);
            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(item.currentItemsCount, descriptionEvt.itemEntity);
        }


        if ((Input.GetKeyDown(KeyCode.I) || (Input.GetKeyDown(KeyCode.Escape)) && _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inInventoryState))
        {
            Debug.Log("open inv");
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(false);

            if (!menusStatesCmp.inInventoryState)
                _offInScopeStateEventsPool.Value.Add(_sceneData.Value.playerEntity);

            ChangeInventoryMenuState(ref menusStatesCmp);

            if (menusStatesCmp.inShopState)
            {
                ChangeShopMenuState(ref menusStatesCmp);
                _shopCloseEventsPool.Value.Add(_world.Value.NewEntity());
                _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view.playerInputView.canShoping = true;
            }

            else if (menusStatesCmp.inStorageState)
            {
                ChangeStorageMenuState(ref menusStatesCmp);

                //  _shopCloseEventsPool.Value.Add(_world.Value.NewEntity());пока никаких ивентов по закритии хранлища
                _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view.playerInputView.canUseStorage = true;
            }
        }
    }

    private void ChangeInventoryMenuState(ref MenuStatesComponent menusStatesCmp)
    {
        menusStatesCmp.inInventoryState = !menusStatesCmp.inInventoryState;
        _sceneData.Value.inventoryMenuView.ChangeMenuState(menusStatesCmp.inInventoryState);
        //
        _sceneData.Value.postProcessingCamera.enabled = menusStatesCmp.inInventoryState;
        _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).canMove = !menusStatesCmp.inInventoryState;
        _currentAttackComponentsPool.Value.Get(_sceneData.Value.playerEntity).canAttack = !menusStatesCmp.inInventoryState;
        _playerComponentsPool.Value.Get(_sceneData.Value.playerEntity).view.playerInputView.usedInventory = menusStatesCmp.inInventoryState;
        //изменение состо€ни€ стрелбы и ходьбы
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
    }
}
