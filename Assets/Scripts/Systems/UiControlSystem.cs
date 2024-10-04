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
    private EcsPoolInject<CurrentAttackComponent> _currentAttackComponentsPool;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<StorageCellTag> _storageCellTagsPool;
    //private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    //private EcsPoolInject<HealingItemCellComponent> _healingItemCellComponentsPool;

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
                    _sceneData.Value.dropedItemsUIView.storageButtonText.text = "Положить в инвентарь";
                }
                else
                {
                    _sceneData.Value.dropedItemsUIView.storageButtonText.text = "Положить в хранилище";
                }
            }

            if (item.itemInfo.type == ItemInfo.itemType.gun || item.itemInfo.type == ItemInfo.itemType.meleeWeapon)//скорей всего милишка останется в этой проверке
            {//поменять на проверку пустоты клетки
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(true);
                var gunInInvCellCmp = _gunInventoryCellComponentsPool.Value.Get(descriptionEvt.itemEntity);
                if (item.itemInfo.type == ItemInfo.itemType.gun)
                {
                    var playerGunCmp = _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity);

                    _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Gun info" + "\n" + "Damage: " + item.itemInfo.gunInfo.damage + "\n" + "Shot couldown: " + item.itemInfo.gunInfo.attackCouldown + "\n" + "Max magazine capacity: " + item.itemInfo.gunInfo.magazineCapacity + "\n" + "Reload time: " + item.itemInfo.gunInfo.reloadDuration
                        + "\n" + "Durability points: " + playerGunCmp.durabilityPoints + "/" + item.itemInfo.gunInfo.maxDurabilityPoints + "\n" + "Spread: " + item.itemInfo.gunInfo.minSpread + "to" + item.itemInfo.gunInfo.maxSpread + "\n" + "Shot distance: " + item.itemInfo.gunInfo.attackLenght + "\n";
                    if (item.itemInfo.gunInfo.isOneBulletReloaded)
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "reloads one cartridge at a time" + "\n";
                    else
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "reloads immediately" + "\n";
                    if (item.itemInfo.gunInfo.isAuto)
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "is Auto" + "\n";
                    else
                        _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "is one shoted" + "\n";
                }

                else
                {
                    //описание милишки
                }
                //мб выделять красным цветом низкую прочность оружя
                if (gunInInvCellCmp.isEquipedWeapon)//поменять на проверку заполненности клетки инвентаря а не ганкмп
                {
                    if (menusStatesCmp.inStorageState)
                        _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.dropItemsUI.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "Снять";
                }
                else
                {
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "Снарядить";
                }
            }

            else if (item.itemInfo.type == ItemInfo.itemType.heal)
            {
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(true);
                var healItemCellCmp = _inventoryCellComponentPool.Value.Get(descriptionEvt.itemEntity);
                _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += "Healed health points: " + item.itemInfo.healInfo.healingHealthPoints + "\n" + "Heal time: " + item.itemInfo.healInfo.healingTime + "\n";
                //кнопка использовать
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(true);
                if (descriptionEvt.itemEntity == _sceneData.Value.healingItemCellView._entity && !healItemCellCmp.isEmpty)
                {
                    if (menusStatesCmp.inStorageState)
                        _sceneData.Value.dropedItemsUIView.storageUIContainer.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.dropItemsUI.gameObject.SetActive(false);
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "Снять все";
                }
                else
                {
                    _sceneData.Value.dropedItemsUIView.currentWeaponButtonActionText.text = "Снарядить все";
                }
            }
            /*else if (item.itemInfo.type == ItemInfo.itemType.meleeWeapon)
            {
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateWeaponEquipButton(true);
                //смена на милишку
            }*/

            else
            {
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateIsUseButton(false);
                _sceneData.Value.dropedItemsUIView.ChangeActiveStateEquipButton(false);
            }
            _sceneData.Value.dropedItemsUIView.itemDescriptionText.text += item.itemInfo.itemDescription;

            _sceneData.Value.dropedItemsUIView.itemInfoContainer.gameObject.SetActive(true);
            _sceneData.Value.dropedItemsUIView.SetSliderParametrs(item.currentItemsCount, descriptionEvt.itemEntity);
        }


        if (Input.GetKeyDown(KeyCode.I) || (Input.GetKeyDown(KeyCode.Escape) && _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity).inInventoryState))
        {
            var playerGunCmp = _playerGunComponentsPool.Value.Get(_sceneData.Value.playerEntity);
            if (playerGunCmp.inScope)
                return;
            ref var menusStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneData.Value.playerEntity);
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
        //изменение состояния стрелбы и ходьбы
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
