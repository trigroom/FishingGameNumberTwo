using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static InteractCharacterView;
using static PlayerInputView;

public class PlayerInputSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentPool;
    private EcsPoolInject<PlayerInputsComponent> _playerInputsComponentsPool;
    private EcsPoolInject<MenuStatesComponent> _menuStatesComponentsPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<AttackComponent> _currentAttackComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<GunComponent> _gunComponentsPool;
    private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    private EcsPoolInject<InventoryComponent> _inventoryComponentsPool;
    // private EcsPoolInject<DataComponent> _dataComponentsPool;
    //private EcsPoolInject<PlayerGunComponent> _playerGunComponentsPool;
    private EcsPoolInject<FieldOfViewComponent> _fieldOfViewComponentsPool;
    private EcsPoolInject<TrapIsNeutralizedEvent> _trapIsNeutralizedEventsPool;
    private EcsPoolInject<CameraComponent> _cameraComponentsPool;
    private EcsPoolInject<HidedObjectOutsideFOVComponent> _hidedObjectOutsideFOVComponentsPool;
    private EcsPoolInject<HealingItemComponent> _currentHealingItemComponentsPool;
    private EcsPoolInject<HealFromHealItemCellEvent> _healFromHealItemCellEventsPool;
    private EcsPoolInject<InventoryCellComponent> _inventoryCellComponentsPool;
    private EcsPoolInject<CurrentDialogeComponent> _currentDialogeComponentsPool;
    private EcsPoolInject<PlayerMoveComponent> _playerMoveComponentsPool;
    private EcsPoolInject<DurabilityInInventoryComponent> _flashLightInInventoryComponentsPool;
    private EcsPoolInject<SolarPanelElectricGeneratorComponent> _solarPanelElectricGeneratorComponentsPool;
    private EcsPoolInject<MeleeWeaponComponent> _meleeWeaponComponentsPool;
    private EcsPoolInject<ChangeInBuildingStateEvent> _changeInBuildingStateEventsPool;
    private EcsPoolInject<BuildingCheckerComponent> _buildingCheckerComponentsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<ThrowGrenadeEvent> _throwGrenadeEventsPool;
    private EcsPoolInject<DropItemsIvent> _dropItemsIventsPool;
    private EcsPoolInject<AddItemFromCellEvent> _addItemFromCellEventsPool;
    private EcsPoolInject<DivideItemEvent> _divideItemEventsPool;
    private EcsPoolInject<PlayerUpgradedStats> _playerUpgradedStatsPool;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentsPool;
    private EcsPoolInject<CalculateRecoilEvent> _calculateRecoilEventsPool;
    private EcsPoolInject<SecondDurabilityComponent> _shieldComponentsPool;
    private EcsPoolInject<NeutralizeTrapEvent> _neutralizeTrapEventsPool;
    private EcsPoolInject<CurrentLocationComponent> _currentLocationComponentsPool;
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;
    private EcsPoolInject<OffInScopeStateEvent> _offInScopeStateEventsPool;
    private EcsPoolInject<FadedParticleOnScreenComponent> _fadedParticleOnScreenComponentsPool;
    private EcsPoolInject<UpgradePlayerStatEvent> _upgradePlayerStatEventsPool;
    private EcsPoolInject<CurrentInteractedCharactersComponent> _currentInteractedCharactersComponentsPool;
    private EcsPoolInject<CheckInteractedObjectsEvent> _checkInteractedObjectsEventsPool;
    private EcsPoolInject<QuestNPCComponent> _questNPCComponentsPool;
    private EcsPoolInject<DroppedItemComponent> _droppedItemComponentsPool;
    private EcsPoolInject<CheckComplitedQuestEvent> _checkComplitedQuestEventsPool;
    private EcsPoolInject<NPCStartDialogeEvent> _npcStartDialogeEventsPool;
    private EcsPoolInject<GunWorkshopOpenEvent> _gunWorkshopOpenEventsPool;
    private EcsPoolInject<ShopOpenEvent> _shopOpenEventsPool;
    private EcsPoolInject<StorageOpenEvent> _storageOpenEventsPool;
    private EcsPoolInject<OpenCraftingTableEvent> _openCraftingTableEventsPool;
    private EcsPoolInject<BreakNeutralizeTrapEvent> _breakNeutralizeTrapEventsPool;
    private EcsPoolInject<AddItemEvent> _addItemEventsPool;
    private EcsPoolInject<EmbientHelperComponent> _embientHelperComponentsPool;
    private EcsPoolInject<OneShotSoundComponent> _oneShotSoundComponentsPool;
    private EcsPoolInject<DeleteItemEvent> _deleteItemEventsPool;
    private EcsPoolInject<DurabilityInInventoryComponent> _durabilityInInventoryComponentsPool;
    private EcsPoolInject<GunInventoryCellComponent> _gunInventoryCellComponentsPool;
    private EcsPoolInject<LockerMinigameCompnent> _lockerMinigameCompnentsPool;
    private EcsPoolInject<ChangeUnderNightLightPlayerStateEvent> _changeUnderNightLightPlayerStateEventsPool;

    private EcsFilterInject<Inc<LoadGameEvent>> loadGameEventsFilter;
    private EcsFilterInject<Inc<CheckInteractedObjectsEvent>> _checkInteractedObjectsEventsFilter;
    private EcsFilterInject<Inc<NeutralizeTrapEvent>> _neutralizeTrapEventsFilter;
    private EcsFilterInject<Inc<BreakNeutralizeTrapEvent>> _breakNeutralizeTrapEventsFilter;
    private EcsFilterInject<Inc<ChangeInBuildingStateEvent>> _changeInBuildingStateEventsFilter;
    private EcsFilterInject<Inc<HidedObjectOutsideFOVComponent>> _hidedObjectOutsideFOVComponentsFilter;
    private EcsFilterInject<Inc<UpgradePlayerStatEvent>> _upgradePlayerStatEventsFilter;
    private EcsFilterInject<Inc<ChangeUnderNightLightPlayerStateEvent>> _changeUnderNightLightPlayerStateEventsFilter;
    private EcsFilterInject<Inc<InventoryItemComponent>, Exc<StorageCellTag, SpecialInventoryCellTag>> _inventoryItemsFilter;

    private EcsCustomInject<SceneService> _sceneService;

    private EcsWorldInject _world;

    private int _playerEntity;

    public void Init(IEcsSystems systems)
    {
        _playerEntity = _world.Value.NewEntity();

        _menuStatesComponentsPool.Value.Add(_playerEntity).currentItemShowedInfo = MenuStatesComponent.CurrentItemShowedInfoState.itemInfo;

        _playerGunComponentsPool.Value.Add(_playerEntity).bulletUIObjects = new List<Image>();

        _lockerMinigameCompnentsPool.Value.Add(_playerEntity);
        _solarPanelElectricGeneratorComponentsPool.Value.Add(_playerEntity);
        ref var playerCmp = ref _playerComponentsPool.Value.Add(_playerEntity);
        playerCmp.view = _sceneService.Value.SpawnPlayer(_world.Value, _playerEntity);
        _currentInteractedCharactersComponentsPool.Value.Add(_playerEntity);
        ref var weaponsInInventoryCmp = ref _playerWeaponsInInventoryComponentsPool.Value.Add(_playerEntity);
        ref var cameraCmp = ref _cameraComponentsPool.Value.Add(_playerEntity);
        cameraCmp.cursorPositonPart = 1;
        cameraCmp.playerPositonPart = 6;
        cameraCmp.currentMaxCameraSpread = playerCmp.view.maxCameraSpread;
        cameraCmp.currentRecoveryCameraSpread = playerCmp.view.recoveryCameraSpread;
        _healthComponentsPool.Value.Add(_playerEntity);
        ref var attackCmp = ref _currentAttackComponentsPool.Value.Add(_playerEntity);
        _currentHealingItemComponentsPool.Value.Add(_playerEntity);
        _currentLocationComponentsPool.Value.Add(_playerEntity);
        attackCmp.weaponIsChanged = false;
        attackCmp.canAttack = true;

        playerCmp.view.meleeColliderView.Construct(_world.Value, _playerEntity);
        playerCmp.view.movementView.shieldView._entity = _playerEntity;
        ref var fieldOfViewCmp = ref _fieldOfViewComponentsPool.Value.Add(_playerEntity);
        fieldOfViewCmp.fieldOfView = playerCmp.view.defaultFOV;
        fieldOfViewCmp.viewDistance = playerCmp.view.viewDistance;

        ref var playerMoveCmp = ref _playerMoveComponentsPool.Value.Add(_playerEntity);
        playerMoveCmp.playerView = playerCmp.view;

        _embientHelperComponentsPool.Value.Add(_playerEntity).timeToNextShot = Random.Range(15, 40);

        ref var gunCmp = ref _gunComponentsPool.Value.Add(_playerEntity);
        gunCmp.gunSpritePositionRecoil = 0.7f;
        gunCmp.lightFromGunShot = playerCmp.view.lightFromGunShot;


        ref var movementComponent = ref _movementComponentPool.Value.Add(_playerEntity);
        _sceneService.Value.playerStaminaText.text = movementComponent.currentRunTime.ToString("0.0") + "/" + playerCmp.view.runTime;
        movementComponent.movementView = playerCmp.view.movementView;
        movementComponent.entityTransform = movementComponent.movementView.objectTransform;
        movementComponent.canMove = true;
        movementComponent.currentRunTime = playerMoveCmp.playerView.runTime;
        _currentDialogeComponentsPool.Value.Add(_playerEntity);
        gunCmp.firePoint = movementComponent.movementView.firePoint;
        gunCmp.weaponContainer = movementComponent.movementView.weaponContainer;

        _meleeWeaponComponentsPool.Value.Add(_playerEntity).startHitPoint = playerCmp.view.movementView.weaponContainer.localPosition;

        _playerInputsComponentsPool.Value.Add(_playerEntity);
        _buildingCheckerComponentsPool.Value.Add(_playerEntity);
    }

    public void Run(IEcsSystems systems)
    {
        if (Input.GetKeyDown(KeyCode.Z))
            _inventoryComponentsPool.Value.Get(_sceneService.Value.inventoryEntity).moneyCount += 50;


        //  if (Input.GetKeyDown(KeyCode.X))
        //   StartLockMinigame(2);
        //del in full game




        ref var curInteactedObjectsCmp = ref _currentInteractedCharactersComponentsPool.Value.Get(_playerEntity);
        if (curInteactedObjectsCmp.interactionType != InteractionType.none)
        {
            if (Input.GetKeyDown(KeyCode.F) && !_healthComponentsPool.Value.Get(_playerEntity).isDeath)
            {
                Debug.Log("Press F");
                if (curInteactedObjectsCmp.isNPCNowIsUsed) return;

                else if (curInteactedObjectsCmp.interactionType == InteractionType.droppedItem && _droppedItemComponentsPool.Value.Has(curInteactedObjectsCmp.dropItemView.itemEntity))
                {
                    _addItemEventsPool.Value.Add(curInteactedObjectsCmp.dropItemView.itemEntity);
                    Debug.Log(" F is Pressed");
                }

                else if (curInteactedObjectsCmp.interactionType == InteractionType.interactedCharacter)
                {
                    _offInScopeStateEventsPool.Value.Add(curInteactedObjectsCmp.interactCharacterView._entity);
                    if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.shopAndDialogeNpc || curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.shop)
                    {
                        curInteactedObjectsCmp.isNPCNowIsUsed = true;
                        _shopOpenEventsPool.Value.Add(curInteactedObjectsCmp.interactCharacterView._entity);
                    }
                    else if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.gunsmith)
                    {
                        curInteactedObjectsCmp.isNPCNowIsUsed = true;
                        _gunWorkshopOpenEventsPool.Value.Add(curInteactedObjectsCmp.interactCharacterView._entity);
                    }
                    else if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.storage)
                    {
                        curInteactedObjectsCmp.isNPCNowIsUsed = true;
                        _storageOpenEventsPool.Value.Add(curInteactedObjectsCmp.interactCharacterView._entity);
                    }
                    else if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.craftingTable)
                    {
                        _openCraftingTableEventsPool.Value.Add(curInteactedObjectsCmp.interactCharacterView._entity);
                        curInteactedObjectsCmp.isNPCNowIsUsed = true;
                    }
                    else if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.openedDoor)
                    {
                        if (_lockerMinigameCompnentsPool.Value.Get(_playerEntity).inGame) return;
                        var openedDoorView = curInteactedObjectsCmp.interactCharacterView.gameObject.GetComponent<OpenedDoorView>();
                        int needItemIdToOpen = openedDoorView.needItemIdToOpen;

                        NeededMasterKeyInfo masterKeyInfo;
                        if (FindItemCountInInventory(needItemIdToOpen, false) != 0)
                        {
                            openedDoorView.doorCollider.enabled = false;
                            openedDoorView.gameObject.GetComponent<SpriteRenderer>().sprite = openedDoorView.openDoorSprite;
                            return;
                        }
                        else if (curInteactedObjectsCmp.interactCharacterView.gameObject.TryGetComponent(out masterKeyInfo) && FindItemCountInInventory(134, true) != 0)
                        {
                            _lockerMinigameCompnentsPool.Value.Get(_playerEntity).interactCharacter = curInteactedObjectsCmp.interactCharacterView;
                            StartLockMinigame(masterKeyInfo.lockerCellsCount, masterKeyInfo.masterkeySpeed);
                            if (FindItemCountInInventory(134, false) == 1)
                                _sceneService.Value.dropedItemsUIView.charactersInteractText.text = "";
                        }
                    }
                    else if(curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.openedContainer)
                    {
                        if (_lockerMinigameCompnentsPool.Value.Get(_playerEntity).inGame) return;
                        NeededMasterKeyInfo masterKeyInfo;
                        if (curInteactedObjectsCmp.interactCharacterView.gameObject.TryGetComponent(out masterKeyInfo) && FindItemCountInInventory(134, true) != 0)
                        {
                            _lockerMinigameCompnentsPool.Value.Get(_playerEntity).interactCharacter = curInteactedObjectsCmp.interactCharacterView;
                            StartLockMinigame(masterKeyInfo.lockerCellsCount, masterKeyInfo.masterkeySpeed);
                            if (FindItemCountInInventory(134, false) == 1)
                                _sceneService.Value.dropedItemsUIView.charactersInteractText.text = "";
                        }
                    }
                }
                else if (curInteactedObjectsCmp.interactionType == InteractionType.trap)
                {
                    if (curInteactedObjectsCmp.trapView.type != TrapView.TrapType.mine || curInteactedObjectsCmp.trapView.type == TrapView.TrapType.mine && _playerComponentsPool.Value.Get(_playerEntity).canDeffuseMines)
                    {
                        if (_neutralizeTrapEventsPool.Value.Has(_playerEntity))
                            _breakNeutralizeTrapEventsPool.Value.Add(_playerEntity);
                        else
                            _neutralizeTrapEventsPool.Value.Add(_playerEntity).currentNeutralizeTime = curInteactedObjectsCmp.trapView.neutralizeTime;
                    }

                }
            }
            else if (Input.GetKeyDown(KeyCode.T) && curInteactedObjectsCmp.interactionType == InteractionType.interactedCharacter && (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.shopAndDialogeNpc || curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.gunsmith) && !curInteactedObjectsCmp.isNPCNowIsUsed)
            {
                var currentQuestCharacter = curInteactedObjectsCmp.interactCharacterView.gameObject.GetComponent<QuestCharacterView>();
                var questNPCCmp = _questNPCComponentsPool.Value.Get(curInteactedObjectsCmp.interactCharacterView._entity);
                if (questNPCCmp.currentQuest < currentQuestCharacter.questNode.Length)
                {
                    if (questNPCCmp.questIsGiven)
                        _checkComplitedQuestEventsPool.Value.Add(currentQuestCharacter.GetComponent<InteractCharacterView>()._entity).characterId = currentQuestCharacter.characterId;
                    else
                    {
                        _npcStartDialogeEventsPool.Value.Add(currentQuestCharacter.GetComponent<InteractCharacterView>()._entity).questNPCId = currentQuestCharacter.characterId;
                        curInteactedObjectsCmp.isNPCNowIsUsed = true;
                    }
                }
            }
        }
        ref var moveCmp = ref _movementComponentPool.Value.Get(_playerEntity);
        foreach (var interactCheck in _checkInteractedObjectsEventsFilter.Value)
        {
            var checkInteractCmp = _checkInteractedObjectsEventsPool.Value.Get(interactCheck);

            ref var interactText = ref _sceneService.Value.dropedItemsUIView.charactersInteractText;

            curInteactedObjectsCmp.interactionType = checkInteractCmp.interactionType;

            if (checkInteractCmp.interactionType == InteractionType.none)
                interactText.text = "";
            else
            {
                if (checkInteractCmp.interactionType == InteractionType.droppedItem)
                {
                    curInteactedObjectsCmp.dropItemView = checkInteractCmp.currentDropItem;
                    var itemInfo = _droppedItemComponentsPool.Value.Get(curInteactedObjectsCmp.dropItemView.itemEntity);
                    interactText.text = "Press F to take " + itemInfo.currentItemsCount + " " + itemInfo.itemInfo.itemName;
                }
                else if (checkInteractCmp.interactionType == InteractionType.interactedCharacter)
                {
                    curInteactedObjectsCmp.interactCharacterView = checkInteractCmp.currentInteractCharacter;
                    if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.shop || curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.shopAndDialogeNpc || curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.gunsmith)
                    {
                        if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.shop)
                            interactText.text = " (нажми F чтобы зайти в магазин)";
                        else
                        {
                            var currentQuestCharacter = curInteactedObjectsCmp.interactCharacterView.gameObject.GetComponent<QuestCharacterView>();
                            var questNPCCmp = _questNPCComponentsPool.Value.Get(curInteactedObjectsCmp.interactCharacterView._entity);
                            if (questNPCCmp.currentQuest >= currentQuestCharacter.questNode.Length)
                            {
                                if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.shopAndDialogeNpc)
                                    interactText.text = " (нажми F чтобы зайти в магазин\nAll " + currentQuestCharacter.characterName + " quests is complete)";
                                else
                                    interactText.text = " (нажми F чтобы зайти в мастескую\nAll " + currentQuestCharacter.characterName + " quests is complete)";

                            }
                            else if (!questNPCCmp.questIsGiven)
                            {
                                if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.shopAndDialogeNpc)
                                    interactText.text = " (нажми F чтобы зайти в магазин\nPress T to speak with " + currentQuestCharacter.characterName + ")";
                                else
                                    interactText.text = " (нажми F чтобы зайти в мастескую\nPress T to speak with " + currentQuestCharacter.characterName + ")";

                            }
                            else
                            {
                                if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.shopAndDialogeNpc)
                                    interactText.text = " (нажми F чтобы зайти в магазин\nPress T to give quest " + currentQuestCharacter.characterName + ")";
                                else
                                    interactText.text = " (нажми F чтобы зайти в мастескую\nPress T to give quest " + currentQuestCharacter.characterName + ")";
                            }
                        }
                    }
                    else if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.storage)
                    {
                        interactText.text = " (нажми F чтобы открыть хранилище)";
                    }
                    else if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.craftingTable)
                    {
                        interactText.text = " (нажми F чтобы войти в верстак)";
                    }
                    else if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.openedDoor)
                    {
                        var openedDoorView = curInteactedObjectsCmp.interactCharacterView.gameObject.GetComponent<OpenedDoorView>();
                        int needItemIdToOpen = openedDoorView.needItemIdToOpen;
                        NeededMasterKeyInfo masterKeyInfo;

                        if (FindItemCountInInventory(needItemIdToOpen, false) != 0)
                        {
                            interactText.text = "(нажми F для того чтобы открыть дверь)";
                            _checkInteractedObjectsEventsPool.Value.Del(interactCheck);
                            return;
                        }

                        else if (curInteactedObjectsCmp.interactCharacterView.gameObject.TryGetComponent(out masterKeyInfo) && FindItemCountInInventory(134, false) != 0)
                            interactText.text = " (нажми F чтобы открыть дверь с помощью отмычки или найди " + _sceneService.Value.idItemslist.items[needItemIdToOpen].itemName + " для открытия)";

                        else if (masterKeyInfo == null)
                        {
                            interactText.text = " (нужен " + _sceneService.Value.idItemslist.items[needItemIdToOpen].itemName + " для открытия двери)";

                        }
                        else
                            interactText.text = " (нужен " + _sceneService.Value.idItemslist.items[needItemIdToOpen].itemName + " или отмычка для открытия двери)";
                    }
                    else if (curInteactedObjectsCmp.interactCharacterView._characterType == InteractNPCType.openedContainer)
                    {
                        if (FindItemCountInInventory(134, true) != 0)
                            interactText.text = "Нажми F для использования отмычки и открытия контейнера";
                        else
                            interactText.text = "Нужны отмычки для открытия контейнера";
                    }
                }
                else if (checkInteractCmp.interactionType == InteractionType.trap)
                {

                    curInteactedObjectsCmp.trapView = checkInteractCmp.currentTrap;
                    if (curInteactedObjectsCmp.trapView.type != TrapView.TrapType.mine)
                        interactText.text = "(нажми F чтобы обезвредить капкан)";
                    else if (_playerComponentsPool.Value.Get(_playerEntity).canDeffuseMines)
                        interactText.text = "(нажми F чтобы обезвредить мину)";
                    else
                        interactText.text = "(нужен набор сапёра чтобы обезвредить мину)";
                }
            }

            _checkInteractedObjectsEventsPool.Value.Del(interactCheck);
        }
        foreach (var upgradeStat in _upgradePlayerStatEventsFilter.Value)
        {
            int upgradedStatIndex = _upgradePlayerStatEventsPool.Value.Get(upgradeStat).statIndex;
            ref var playerStatsCmp = ref _playerUpgradedStatsPool.Value.Get(_playerEntity);
            switch (upgradedStatIndex)
            {
                case 0://str
                    playerStatsCmp.statLevels[0]++;
                    break;
                case 1://acc
                    playerStatsCmp.statLevels[1]++;
                    break;
                case 2://stamina
                    playerStatsCmp.statLevels[2]++;
                    ref var inventoryCmp = ref _inventoryComponentsPool.Value.Get(_sceneService.Value.inventoryEntity);
                    moveCmp.moveSpeed = moveCmp.movementView.moveSpeed + moveCmp.movementView.moveSpeed / 50 * playerStatsCmp.statLevels[0];
                    if (inventoryCmp.weight / inventoryCmp.currentMaxWeight > 0.7f)
                        moveCmp.moveSpeed -= moveCmp.movementView.moveSpeed * ((inventoryCmp.weight / inventoryCmp.currentMaxWeight) - 0.7f) * 2;
                    var playerGunCmp = _playerGunComponentsPool.Value.Get(_playerEntity);
                    if (playerGunCmp.inScope)
                        moveCmp.moveSpeed /= playerGunCmp.currentScopeMultiplicity;

                    moveCmp.maxRunTime = _playerComponentsPool.Value.Get(_playerEntity).view.runTime * (1 + playerStatsCmp.statLevels[2] * 0.02f);

                    break;
            }
            _upgradePlayerStatEventsPool.Value.Del(upgradeStat);
        }
        foreach (var neutralizeTrap in _neutralizeTrapEventsFilter.Value)
        {
            foreach (var breakNeutralizeTrap in _breakNeutralizeTrapEventsFilter.Value)
            {
                _movementComponentPool.Value.Get(neutralizeTrap).canMove = true;
                _neutralizeTrapEventsPool.Value.Del(neutralizeTrap);
                _sceneService.Value.ammoInfoText.text = "";
                return;
            }
            ref var trapEvent = ref _neutralizeTrapEventsPool.Value.Get(neutralizeTrap);
            if (!trapEvent.isFirstCheck)
            {
                _movementComponentPool.Value.Get(neutralizeTrap).canMove = false;
                _sceneService.Value.ammoInfoText.text = "Neutralize Trap";
            }
            trapEvent.currentNeutralizeTime -= Time.deltaTime;
            //Debug.Log(trapEvent.currentNeutralizeTime);
            if (trapEvent.currentNeutralizeTime <= 0)
            {
                _trapIsNeutralizedEventsPool.Value.Add(neutralizeTrap).trapType = curInteactedObjectsCmp.trapView.type;
                _sceneService.Value.ammoInfoText.text = "";
                curInteactedObjectsCmp.trapView.spriteRenderer.sprite = curInteactedObjectsCmp.trapView.safetyTrapSprite;
                curInteactedObjectsCmp.trapView.trapCollider.enabled = false;
                curInteactedObjectsCmp.trapView = null;
                _movementComponentPool.Value.Get(neutralizeTrap).canMove = true;

                _neutralizeTrapEventsPool.Value.Del(neutralizeTrap);
            }
        }
        ref var buildChecker = ref _buildingCheckerComponentsPool.Value.Get(_playerEntity);
        ref var gloabalTimeCmp = ref _globalTimeComponentsPool.Value.Get(_playerEntity);
        ref var playerCmp = ref _playerComponentsPool.Value.Get(_playerEntity);

        float fov = _fieldOfViewComponentsPool.Value.Get(_playerEntity).fieldOfView / 2 - 180;
        if (playerCmp.view.leftFOVTracker.localEulerAngles.z != fov)
        {
            // Debug.Log("change fov to" + fov);
            var playerView = playerCmp.view;
            playerView.leftFOVTracker.localRotation = Quaternion.Euler(0, 0, fov);
            playerView.rightFOVTracker.localRotation = Quaternion.Euler(0, 0, -fov);
        }
        if (gloabalTimeCmp.currentWeatherType != GlobalTimeComponent.WeatherType.none)
        {
            if (!buildChecker.isHideRoof)
            {
                if (gloabalTimeCmp.lastRainDropTime <= 0)
                {
                    gloabalTimeCmp.lastRainDropTime = gloabalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.rain ? Random.Range(0.3f, 0.8f) : Random.Range(0.1f, 0.25f);
                    if (_inventoryItemComponentsPool.Value.Has(_sceneService.Value.helmetCellView._entity))
                        _fadedParticleOnScreenComponentsPool.Value.Add(_world.Value.NewEntity()).particleImage = _sceneService.Value.GetParticleOnScreen(_sceneService.Value.rainDropOnScreenColor, 1f - _inventoryItemComponentsPool.Value.Get(_sceneService.Value.helmetCellView._entity).itemInfo.helmetInfo.dropTransparentMultiplayer, true);
                    else
                        _fadedParticleOnScreenComponentsPool.Value.Add(_world.Value.NewEntity()).particleImage = _sceneService.Value.GetParticleOnScreen(_sceneService.Value.rainDropOnScreenColor, 1f, true);
                }

                gloabalTimeCmp.lastRainDropTime -= Time.deltaTime;

            }
            if (gloabalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.thunderstorm)
            {
                gloabalTimeCmp.lastThunderTime -= Time.deltaTime;
                if (gloabalTimeCmp.lastThunderTime <= 0)
                {
                    if (!buildChecker.isHideRoof)
                        gloabalTimeCmp.thuderIsLighting = true;
                    gloabalTimeCmp.lastThunderTime = Random.Range(10, 20);
                    var needSound = _sceneService.Value.thunderSounds[Random.Range(0, _sceneService.Value.thunderSounds.Length)];
                    _oneShotSoundComponentsPool.Value.Add(_world.Value.NewEntity()).Construct(_sceneService.Value.PlaySoundFXClip(needSound, moveCmp.movementView.transform.position, buildChecker.isHideRoof ? Random.Range(0.1f, 0.4f) : Random.Range(0.5f, 1f)), needSound.length);
                }
                if (gloabalTimeCmp.thuderIsLighting)
                {
                    gloabalTimeCmp.currentThunderLight += Time.deltaTime * 3;
                    if (gloabalTimeCmp.currentThunderLight > 2f)
                        gloabalTimeCmp.thuderIsLighting = false;
                }
                else if (gloabalTimeCmp.currentThunderLight > 0 && !buildChecker.isHideRoof)
                {
                    gloabalTimeCmp.currentThunderLight -= Time.deltaTime;

                    var defaultLightIntancity = gloabalTimeCmp.currentGlobalLightIntensity;
                    if (playerCmp.nvgIsUsed)
                        defaultLightIntancity += _inventoryItemComponentsPool.Value.Get(_sceneService.Value.helmetCellView._entity).itemInfo.helmetInfo.addedLightIntancity;

                    if (defaultLightIntancity < gloabalTimeCmp.currentThunderLight)
                        _sceneService.Value.gloabalLight.intensity = gloabalTimeCmp.currentThunderLight;
                    else
                        _sceneService.Value.gloabalLight.intensity = defaultLightIntancity;
                }
                else if (gloabalTimeCmp.currentThunderLight < 0)
                    gloabalTimeCmp.currentThunderLight = 0;
            }
        }
        var healthCmp = _healthComponentsPool.Value.Get(_playerEntity);
        foreach (var changeRoofState in _changeInBuildingStateEventsFilter.Value)
        {
            ref var changeStateCmp = ref _changeInBuildingStateEventsPool.Value.Get(changeRoofState);
            buildChecker.isHideRoof = changeStateCmp.isHideRoof;
            buildChecker.roofSpriteRenderer = changeStateCmp.roofTilemaps;
            if (buildChecker.timeBeforeHideRoof > 0)
                buildChecker.timeBeforeHideRoof = 1f - buildChecker.timeBeforeHideRoof;
            else
                buildChecker.timeBeforeHideRoof = 1f;

            _sceneService.Value.backgroundAudioSource.volume = buildChecker.isHideRoof ? 0.3f : 1f;
            if (gloabalTimeCmp.currentWeatherType != GlobalTimeComponent.WeatherType.none)
                _sceneService.Value.rainEffectContainer.gameObject.SetActive(!buildChecker.isHideRoof);
            //change rain sound to roof rain sound

            _changeInBuildingStateEventsPool.Value.Del(changeRoofState);
        }

        if (playerCmp.nvgIsUsed)
        {
            ref var nvgChargeCmp = ref _shieldComponentsPool.Value.Get(_sceneService.Value.helmetCellView._entity);
            nvgChargeCmp.currentDurability -= Time.deltaTime;
            if (nvgChargeCmp.currentDurability < 0)
            {
                nvgChargeCmp.currentDurability = 0;
                playerCmp.nvgIsUsed = false;
                ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(_playerEntity);
                if (buildChecker.isHideRoof)
                    _sceneService.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity - 0.35f;
                else
                    _sceneService.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity;
                if (globalTimeCmp.currentDayTime >= 15)
                    _sceneService.Value.gloabalLight.color = _sceneService.Value.globalLightColors[2];
                else if (globalTimeCmp.currentDayTime == 12 || globalTimeCmp.currentDayTime == 0)
                    _sceneService.Value.gloabalLight.color = globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none ? _sceneService.Value.globalLightColors[1] : _sceneService.Value.globalLightColors[4];
                else
                    _sceneService.Value.gloabalLight.color = globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none ? _sceneService.Value.globalLightColors[0] : _sceneService.Value.globalLightColors[3];
                _sceneService.Value.bloomMainBg.intensity.value = 0;

                if (_sceneService.Value.gloabalLight.intensity < 0)
                    _sceneService.Value.gloabalLight.intensity = 0f;
            }
        }
        if (buildChecker.timeBeforeHideRoof > 0f)
        {
            if (buildChecker.roofSpriteRenderer == null)
            {
                buildChecker.timeBeforeHideRoof = 0;
                buildChecker.isHideRoof = false;
            }
            else
            {
                buildChecker.timeBeforeHideRoof -= Time.deltaTime;
                if (buildChecker.isHideRoof)
                {
                    if (playerCmp.nvgIsUsed)
                    {
                        var helmetInfo = _inventoryItemComponentsPool.Value.Get(_sceneService.Value.helmetCellView._entity).itemInfo.helmetInfo;
                        var addHelmetIntansity = helmetInfo.addedLightIntancity;
                        if (_sceneService.Value.gloabalLight.intensity > addHelmetIntansity)
                        {
                            if (gloabalTimeCmp.isNight)
                                _sceneService.Value.gloabalLight.intensity = gloabalTimeCmp.currentGlobalLightIntensity - (1 - buildChecker.timeBeforeHideRoof) * 0.35f + addHelmetIntansity;
                            else
                            {
                                _sceneService.Value.gloabalLight.intensity = gloabalTimeCmp.currentGlobalLightIntensity - (1 - buildChecker.timeBeforeHideRoof) * 0.35f + (addHelmetIntansity * 5 - (4 * addHelmetIntansity * (1 - buildChecker.timeBeforeHideRoof)));
                                _sceneService.Value.bloomMainBg.intensity.value = helmetInfo.addedBloom + helmetInfo.addedBloom * 4 * buildChecker.timeBeforeHideRoof;
                            }
                        }
                        else if (_sceneService.Value.gloabalLight.intensity < addHelmetIntansity)
                        {
                            _sceneService.Value.gloabalLight.intensity = addHelmetIntansity;
                            _sceneService.Value.bloomMainBg.intensity.value = helmetInfo.addedBloom;
                        }

                    }
                    else
                    {
                        if (_sceneService.Value.gloabalLight.intensity > 0f)
                            _sceneService.Value.gloabalLight.intensity = gloabalTimeCmp.currentGlobalLightIntensity - (1 - buildChecker.timeBeforeHideRoof) * 0.35f;
                        else if (_sceneService.Value.gloabalLight.intensity < 0)
                            _sceneService.Value.gloabalLight.intensity = 0;
                    }
                    ChangeAlfaAllSpriteInGroup(buildChecker.timeBeforeHideRoof, buildChecker.roofSpriteRenderer.tilemaps);
                }
                else
                {
                    float sumIntansity = 0;
                    ChangeAlfaAllSpriteInGroup(1 - buildChecker.timeBeforeHideRoof, buildChecker.roofSpriteRenderer.tilemaps);
                    if (gloabalTimeCmp.currentGlobalLightIntensity > 0f)
                        sumIntansity = gloabalTimeCmp.currentGlobalLightIntensity - buildChecker.timeBeforeHideRoof * 0.35f;
                    if (playerCmp.nvgIsUsed)
                    {
                        var helmetInfo = _inventoryItemComponentsPool.Value.Get(_sceneService.Value.helmetCellView._entity).itemInfo.helmetInfo;
                        var addedHelmetInt = helmetInfo.addedLightIntancity;
                        if (gloabalTimeCmp.isNight)
                            sumIntansity += addedHelmetInt;
                        else
                        {
                            sumIntansity += addedHelmetInt + (addedHelmetInt * 4 * (1 - buildChecker.timeBeforeHideRoof));
                            _sceneService.Value.bloomMainBg.intensity.value = helmetInfo.addedBloom + helmetInfo.addedBloom * 4 * (1 - buildChecker.timeBeforeHideRoof);
                        }
                    }
                    _sceneService.Value.gloabalLight.intensity = sumIntansity;
                }

                if (buildChecker.timeBeforeHideRoof < 0f)
                {
                    buildChecker.timeBeforeHideRoof = 0f;
                    float sumIntansity = 0;
                    if (buildChecker.isHideRoof)
                    {
                        ChangeAlfaAllSpriteInGroup(buildChecker.timeBeforeHideRoof, buildChecker.roofSpriteRenderer.tilemaps);
                        if (gloabalTimeCmp.currentGlobalLightIntensity - 0.35f > 0f)
                            sumIntansity = gloabalTimeCmp.currentGlobalLightIntensity - 0.35f;
                        else
                            sumIntansity = 0;
                    }
                    else
                    {
                        ChangeAlfaAllSpriteInGroup(1 - buildChecker.timeBeforeHideRoof, buildChecker.roofSpriteRenderer.tilemaps);
                        if (gloabalTimeCmp.currentGlobalLightIntensity > 0f)
                            sumIntansity = gloabalTimeCmp.currentGlobalLightIntensity;
                        else
                            sumIntansity = 0;
                    }
                    if (playerCmp.nvgIsUsed)
                    {
                        var helmetInfo = _inventoryItemComponentsPool.Value.Get(_sceneService.Value.helmetCellView._entity).itemInfo.helmetInfo;
                        if (gloabalTimeCmp.isNight || buildChecker.isHideRoof)
                        {
                            sumIntansity += helmetInfo.addedLightIntancity;
                            _sceneService.Value.bloomMainBg.intensity.value = helmetInfo.addedBloom;
                        }
                        else
                        {
                            sumIntansity += helmetInfo.addedLightIntancity * 5;
                            _sceneService.Value.bloomMainBg.intensity.value = helmetInfo.addedBloom * 5;
                        }
                    }
                    _sceneService.Value.gloabalLight.intensity = sumIntansity;
                }
            }
        }
        ref var playerMoveCmp = ref _playerMoveComponentsPool.Value.Get(_playerEntity);

        foreach (var loadGame in loadGameEventsFilter.Value)
        {
            ref var inventoryCmp = ref _inventoryComponentsPool.Value.Get(_sceneService.Value.inventoryEntity);
            _sceneService.Value.moneyText.text = inventoryCmp.moneyCount + "$";
            var upgradedStatsCmp = _playerUpgradedStatsPool.Value.Get(_playerEntity);
            moveCmp.maxRunTime = playerCmp.view.runTime * (1 + upgradedStatsCmp.statLevels[2] * 0.02f);
            moveCmp.currentRunTimeRecoverySpeed = playerCmp.view.runTimeRecoverySpeed;
            inventoryCmp.currentMaxWeight = _sceneService.Value.maxInInventoryWeight + upgradedStatsCmp.statLevels[0] * _sceneService.Value.maxInInventoryWeight / 50f;
            _gunComponentsPool.Value.Get(_playerEntity).spreadRecoverySpeed = playerCmp.view.gunRecoverySpreadSpeed * 1 + (upgradedStatsCmp.statLevels[0] * 0.02f);
            moveCmp.moveSpeed = moveCmp.movementView.moveSpeed + moveCmp.movementView.moveSpeed / 50 * upgradedStatsCmp.statLevels[0];
            if (inventoryCmp.weight / inventoryCmp.currentMaxWeight > 0.6f)
                moveCmp.moveSpeed -= (moveCmp.movementView.moveSpeed * ((inventoryCmp.weight / inventoryCmp.currentMaxWeight) - 0.6f) * 2);
        }

        bool inInventory = _menuStatesComponentsPool.Value.Get(_playerEntity).inInventoryState;

        if (inInventory && _sceneService.Value.dropedItemsUIView.divideItemsUI.gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.D) && _sceneService.Value.dropedItemsUIView.dropButton.gameObject.activeInHierarchy)
                _dropItemsIventsPool.Value.Add(_sceneService.Value.dropedItemsUIView.curCell);
            else if (Input.GetKeyDown(KeyCode.A) && _inventoryItemComponentsPool.Value.Get(_sceneService.Value.dropedItemsUIView.curCell).currentItemsCount > 1)
                _sceneService.Value.dropedItemsUIView.generalSlider.value = _inventoryItemComponentsPool.Value.Get(_sceneService.Value.dropedItemsUIView.curCell).currentItemsCount;
            else if (Input.GetKeyDown(KeyCode.S) && _sceneService.Value.dropedItemsUIView.storageButton.gameObject.activeInHierarchy)
                _addItemFromCellEventsPool.Value.Add(_sceneService.Value.dropedItemsUIView.curCell);
            else if (Input.GetKeyDown(KeyCode.W) && _sceneService.Value.dropedItemsUIView.divideButton.gameObject.activeInHierarchy)
                _divideItemEventsPool.Value.Add(_sceneService.Value.dropedItemsUIView.curCell);
            else if (Input.GetKeyDown(KeyCode.E) && _sceneService.Value.dropedItemsUIView.weaponEquipButton.gameObject.activeInHierarchy)
            {
                //экипировать чтолибо
            }
        }

        if (healthCmp.isDeath) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput);

        moveDirection = moveDirection.normalized;

        if (playerMoveCmp.nowIsMoving && moveDirection == Vector2.zero)
        {
            playerMoveCmp.nowIsMoving = false;
            moveCmp.movementView.characterAnimator.SetBool("isWalking", playerMoveCmp.nowIsMoving);
            _calculateRecoilEventsPool.Value.Add(_world.Value.NewEntity());
        }
        else if (!playerMoveCmp.nowIsMoving && moveDirection != Vector2.zero)
        {
            playerMoveCmp.nowIsMoving = true;
            moveCmp.movementView.characterAnimator.SetBool("isWalking", playerMoveCmp.nowIsMoving);
            _calculateRecoilEventsPool.Value.Add(_world.Value.NewEntity());
        }
        if (playerMoveCmp.nowIsMoving && playerMoveCmp.currentHungerPoints != 0)
        {
            if (moveCmp.isRun)
                playerMoveCmp.currentHungerPoints -= Time.deltaTime * 2;
            else
                playerMoveCmp.currentHungerPoints -= Time.deltaTime;

            if (playerMoveCmp.currentHungerPoints < 0)
                playerMoveCmp.currentHungerPoints = 0;
            _sceneService.Value.playerArmorBarFilled.fillAmount = playerMoveCmp.currentHungerPoints / playerMoveCmp.maxHungerPoints;
            _sceneService.Value.playerArmorText.text = (int)playerMoveCmp.currentHungerPoints + " / " + (int)playerMoveCmp.maxHungerPoints;
        }
        if (!moveCmp.isStunned)
        {
            if (!_currentDialogeComponentsPool.Value.Get(_playerEntity).dialogeIsStarted)
            {
                moveCmp.moveInput = moveDirection;

                float hungerMultiplayer = playerMoveCmp.currentHungerPoints / playerMoveCmp.maxHungerPoints;
                if (moveCmp.isRun)
                {
                    ref var playerStats = ref _playerUpgradedStatsPool.Value.Get(_playerEntity);


                    playerStats.currentStatsExp[2] += Time.fixedDeltaTime * 0.5f;
                    if (playerStats.currentStatsExp[2] >= _sceneService.Value.levelExpCounts[playerStats.statLevels[2]] && !_upgradePlayerStatEventsPool.Value.Has(_sceneService.Value.playerEntity))
                        _upgradePlayerStatEventsPool.Value.Add(_sceneService.Value.playerEntity).statIndex = 2;

                    moveCmp.currentRunTime -= Time.deltaTime;
                    if (moveCmp.currentRunTime <= 0 || moveDirection == Vector2.zero || !moveCmp.canMove || inInventory || healthCmp.isDeath || moveCmp.isTrapped)
                    {
                        if (moveCmp.currentRunTime <= 0)
                            moveCmp.currentRunTime--;
                        moveCmp.isRun = false;
                        _calculateRecoilEventsPool.Value.Add(_world.Value.NewEntity());
                    }
                    _sceneService.Value.playerStaminaBarFilled.fillAmount = moveCmp.currentRunTime / moveCmp.maxRunTime;
                    _sceneService.Value.playerStaminaText.text = moveCmp.currentRunTime.ToString("0.0") + "/" + moveCmp.maxRunTime;
                    _sceneService.Value.staminaAnimator.SetFloat("StaminaAnimSpeed", 1 + (1 - _sceneService.Value.playerStaminaBarFilled.fillAmount) * 2);
                }
                else if ((moveCmp.currentRunTime < moveCmp.maxRunTime && hungerMultiplayer > 0.5f || hungerMultiplayer <= 0.5f && moveCmp.currentRunTime < moveCmp.maxRunTime * (hungerMultiplayer + 0.5f)) && !_playerGunComponentsPool.Value.Get(_sceneService.Value.playerEntity).inScope)
                {
                    moveCmp.currentRunTime += Time.deltaTime * moveCmp.currentRunTimeRecoverySpeed;
                    _sceneService.Value.playerStaminaBarFilled.fillAmount = moveCmp.currentRunTime / moveCmp.maxRunTime;
                    _sceneService.Value.playerStaminaText.text = moveCmp.currentRunTime.ToString("0.0") + "/" + moveCmp.maxRunTime;
                    _sceneService.Value.staminaAnimator.SetFloat("StaminaAnimSpeed", 1 + (1 - _sceneService.Value.playerStaminaBarFilled.fillAmount) * 2);
                }
                else if (moveCmp.currentRunTime > moveCmp.maxRunTime)
                    moveCmp.currentRunTime = moveCmp.maxRunTime;
            }
            else
                moveCmp.moveInput = Vector2.zero;
            moveCmp.pointToRotateInput = _sceneService.Value.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (!_currentHealingItemComponentsPool.Value.Get(_playerEntity).isHealing)
        {
            if (Input.GetKeyDown(KeyCode.N) && _inventoryItemComponentsPool.Value.Has(_sceneService.Value.helmetCellView._entity) && _inventoryItemComponentsPool.Value.Get(_sceneService.Value.helmetCellView._entity).itemInfo.helmetInfo.addedLightIntancity != 0 && _shieldComponentsPool.Value.Get(_sceneService.Value.helmetCellView._entity).currentDurability > 0) //and check nvg charge
            {
                ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(_playerEntity);
                var helmetInfo = _inventoryItemComponentsPool.Value.Get(_sceneService.Value.helmetCellView._entity).itemInfo.helmetInfo;

                playerCmp.nvgIsUsed = !playerCmp.nvgIsUsed;
                _sceneService.Value.uiAudioSourse.clip = _sceneService.Value.offOnDeviceSound;
                _sceneService.Value.uiAudioSourse.Play();
                var playerGunCmp = _playerGunComponentsPool.Value.Get(_playerEntity);
                bool isHideRoof = _buildingCheckerComponentsPool.Value.Get(_playerEntity).isHideRoof;
                if (playerCmp.nvgIsUsed)
                {

                    if (playerGunCmp.inScope && playerGunCmp.currentScopeMultiplicity > 2.1f)
                        _offInScopeStateEventsPool.Value.Add(_playerEntity);
                    if (isHideRoof)
                    {
                        if (globalTimeCmp.currentGlobalLightIntensity - 0.35f <= 0f)
                            _sceneService.Value.gloabalLight.intensity = helmetInfo.addedLightIntancity;
                        else
                            _sceneService.Value.gloabalLight.intensity = helmetInfo.addedLightIntancity + globalTimeCmp.currentGlobalLightIntensity - 0.35f;
                        _sceneService.Value.bloomMainBg.intensity.value = helmetInfo.addedBloom;
                    }
                    else if (!globalTimeCmp.isNight)
                    {
                        _sceneService.Value.gloabalLight.intensity = helmetInfo.addedLightIntancity * 5 + globalTimeCmp.currentGlobalLightIntensity;
                        _sceneService.Value.bloomMainBg.intensity.value = helmetInfo.addedBloom * 5;
                    }
                    else
                    {
                        _sceneService.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity + helmetInfo.addedLightIntancity;
                        _sceneService.Value.bloomMainBg.intensity.value = helmetInfo.addedBloom;
                    }
                    _sceneService.Value.gloabalLight.color = helmetInfo.visionColor;
                }
                else
                {
                    if (isHideRoof)
                        _sceneService.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity - 0.35f;
                    else
                        _sceneService.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity;
                    if (globalTimeCmp.currentDayTime >= 15)
                        _sceneService.Value.gloabalLight.color = _sceneService.Value.globalLightColors[2];
                    else if (globalTimeCmp.currentDayTime == 12 || globalTimeCmp.currentDayTime == 0)
                        _sceneService.Value.gloabalLight.color = globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none ? _sceneService.Value.globalLightColors[1] : _sceneService.Value.globalLightColors[4];
                    else
                        _sceneService.Value.gloabalLight.color = globalTimeCmp.currentWeatherType == GlobalTimeComponent.WeatherType.none ? _sceneService.Value.globalLightColors[0] : _sceneService.Value.globalLightColors[3];
                    _sceneService.Value.bloomMainBg.intensity.value = 0;

                }
                if (_sceneService.Value.gloabalLight.intensity < 0)
                    _sceneService.Value.gloabalLight.intensity = 0f;
            }

            else if (!_currentAttackComponentsPool.Value.Get(_playerEntity).weaponIsChanged && !_playerGunComponentsPool.Value.Get(_playerEntity).inScope)
            {
                if (Input.GetKeyDown(KeyCode.H) && healthCmp.maxHealthPoint != healthCmp.healthPoint && !_inventoryCellComponentsPool.Value.Get(_sceneService.Value.healingItemCellView._entity).isEmpty && !_gunComponentsPool.Value.Get(_playerEntity).isReloading)
                    _healFromHealItemCellEventsPool.Value.Add(_playerEntity);

                else if (Input.GetKeyDown(KeyCode.G) && !_inventoryCellComponentsPool.Value.Get(_sceneService.Value.grenadeCellView._entity).isEmpty) //возможно что то ещё
                {
                    ref var playerAttackCmp = ref _currentAttackComponentsPool.Value.Get(_playerEntity);
                    if (playerAttackCmp.grenadeThrowCouldown <= 0)
                    {
                        _throwGrenadeEventsPool.Value.Add(_playerEntity);
                        playerAttackCmp.grenadeThrowCouldown = 0.5f;
                    }
                }

                else if (Input.GetKeyDown(KeyCode.C) && !_inventoryCellComponentsPool.Value.Get(_sceneService.Value.shieldCellView._entity).isEmpty && !_gunComponentsPool.Value.Get(_playerEntity).isReloading) //возможно что то ещё
                {
                    var playerWeaponsInInvCmp = _playerWeaponsInInventoryComponentsPool.Value.Get(_playerEntity);
                    var itemCmp = _inventoryItemComponentsPool.Value.Get(playerWeaponsInInvCmp.curEquipedWeaponCellEntity);
                    if (playerWeaponsInInvCmp.curWeapon == 2 && itemCmp.itemInfo.meleeWeaponInfo.isOneHandedWeapon || playerWeaponsInInvCmp.curWeapon < 2 && (itemCmp.itemInfo.gunInfo.isOneHandedGun || (!itemCmp.itemInfo.gunInfo.isOneHandedGun && _playerUpgradedStatsPool.Value.Get(_playerEntity).weaponsExp[itemCmp.itemInfo.itemId].weaponExpLevel >= 8)))//поменять уровень на 6
                    {
                        ref var shieldCmp = ref _shieldComponentsPool.Value.Get(_sceneService.Value.shieldCellView._entity);
                        //playerCmp.view.movementView.weaponSpriteRenderer.transform.localRotation
                        // playerCmp.view.movementView.shieldView.shieldObject.localRotation = new Vector3(playerCmp.view.movementView.shieldView.shieldObject.localScale.x * -1, playerCmp.view.movementView.shieldView.shieldObject.localScale.y, playerCmp.view.movementView.shieldView.shieldObject.localScale.z);


                        if (playerCmp.view.movementView.shieldView.shieldObject.localPosition != Vector3.zero)
                        {
                            playerCmp.view.movementView.shieldView.shieldObject.localScale = Vector3.one;
                            playerCmp.view.movementView.shieldView.shieldObject.SetParent(playerCmp.view.movementView.shieldView.shieldContainer, false);
                            playerCmp.view.movementView.shieldView.shieldObject.localPosition = Vector3.zero;
                            playerCmp.view.movementView.shieldView.shieldObject.localRotation = Quaternion.Euler(0, 180, 0);
                            playerCmp.view.movementView.shieldView.shieldSpriteRenderer.sortingOrder = 2;
                            if (_playerWeaponsInInventoryComponentsPool.Value.Get(_sceneService.Value.playerEntity).curWeapon == 2)
                            {
                                int meleeWeaponEntity = _sceneService.Value.meleeWeaponCellView._entity;
                                _meleeWeaponComponentsPool.Value.Get(_sceneService.Value.playerEntity).curAttackLenghtMultiplayer = _inventoryItemComponentsPool.Value.Get(meleeWeaponEntity).itemInfo.meleeWeaponInfo.attackLenght;
                            }
                        }
                        else
                        {
                            playerCmp.view.movementView.shieldView.shieldObject.localScale = Vector3.one * 6;
                            //playerCmp.view.movementView.shieldView.shieldObject.localScale = new Vector3(playerCmp.view.movementView.shieldView.shieldObject.localScale.x * -1, playerCmp.view.movementView.shieldView.shieldObject.localScale.y, playerCmp.view.movementView.shieldView.shieldObject.localScale.z);
                            playerCmp.view.movementView.shieldView.shieldObject.SetParent(playerCmp.view.movementView.nonWeaponContainer, false);
                            playerCmp.view.movementView.shieldView.shieldObject.localRotation = Quaternion.Euler(0, 180, 90);
                            playerCmp.view.movementView.shieldView.shieldObject.localPosition = _inventoryItemComponentsPool.Value.Get(_sceneService.Value.shieldCellView._entity).itemInfo.sheildInfo.sheildInHandsPosition;
                            playerCmp.view.movementView.shieldView.shieldSpriteRenderer.sortingOrder = 6;
                            if (_playerWeaponsInInventoryComponentsPool.Value.Get(_sceneService.Value.playerEntity).curWeapon == 2)
                            {
                                int meleeWeaponEntity = _sceneService.Value.meleeWeaponCellView._entity;
                                _meleeWeaponComponentsPool.Value.Get(_sceneService.Value.playerEntity).curAttackLenghtMultiplayer = _inventoryItemComponentsPool.Value.Get(meleeWeaponEntity).itemInfo.meleeWeaponInfo.attackLenght * _inventoryItemComponentsPool.Value.Get(_sceneService.Value.shieldCellView._entity).itemInfo.sheildInfo.recoilPercent;
                            }
                        }
                    }
                    _calculateRecoilEventsPool.Value.Add(_world.Value.NewEntity());

                }

                else if (Input.GetKeyDown(KeyCode.LeftShift) && moveDirection != Vector2.zero && moveCmp.canMove && !inInventory && !healthCmp.isDeath && !moveCmp.isStunned && moveCmp.currentRunTime > 0)
                {
                    moveCmp.isRun = !moveCmp.isRun;
                    _calculateRecoilEventsPool.Value.Add(_world.Value.NewEntity());
                }
            }
        }
        if (!_inventoryCellComponentsPool.Value.Get(_sceneService.Value.flashlightItemCellView._entity).isEmpty)
        {
            ref var flashlightCmp = ref _flashLightInInventoryComponentsPool.Value.Get(_sceneService.Value.flashlightItemCellView._entity);

            if (flashlightCmp.currentDurability > 0)
            {
                if (playerCmp.useFlashlight)
                {
                    flashlightCmp.currentDurability -= Time.deltaTime;

                    if (flashlightCmp.currentDurability <= 0)
                    {
                        playerCmp.useFlashlight = false;
                        flashlightCmp.currentDurability = 0;
                        playerCmp.view.flashLightObject.gameObject.SetActive(false);
                    }
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    playerCmp.useFlashlight = !playerCmp.useFlashlight;
                    _sceneService.Value.uiAudioSourse.clip = _sceneService.Value.offOnDeviceSound;
                    _sceneService.Value.uiAudioSourse.Play();
                    playerCmp.view.flashLightObject.gameObject.SetActive(playerCmp.useFlashlight);
                }
            }
        }
        foreach (var objEntity in _hidedObjectOutsideFOVComponentsFilter.Value)
        {
            ref var hidedObjCmp = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(objEntity);

            if (hidedObjCmp.timeBeforeHide < 0)
            {
                hidedObjCmp.timeBeforeHide = 0;
                foreach (var spriteRenderer in hidedObjCmp.hidedObjects)
                    if (spriteRenderer != null)
                        spriteRenderer.gameObject.SetActive(false);
            }
            else if (hidedObjCmp.timeBeforeHide > 0)
                hidedObjCmp.timeBeforeHide -= Time.deltaTime;
        }

        ref var embientHelperCmp = ref _embientHelperComponentsPool.Value.Get(_playerEntity);
        embientHelperCmp.timeToNextShot -= Time.deltaTime;
        if (embientHelperCmp.timeToNextShot <= 0)
        {
            embientHelperCmp.timeToNextShot -= Time.deltaTime;
            if (embientHelperCmp.audioSource == null)
            {
                embientHelperCmp.audioSource = _sceneService.Value.PlaySoundFXClip(_sceneService.Value.randomEmbientSounds[Random.Range(0, _sceneService.Value.randomEmbientSounds.Length)], playerMoveCmp.playerView.transform.position, Random.Range(0.01f, 0.15f));
                embientHelperCmp.audioSource.panStereo = Random.Range(-1f, 1f);
            }
            else
            {
                embientHelperCmp.audioSource.volume = Random.Range(0.01f, 0.15f);
                embientHelperCmp.audioSource.clip = _sceneService.Value.randomEmbientSounds[Random.Range(0, _sceneService.Value.randomEmbientSounds.Length)];
                embientHelperCmp.audioSource.panStereo = Random.Range(-1f, 1f);
                embientHelperCmp.audioSource.transform.position = playerMoveCmp.playerView.transform.position;
                embientHelperCmp.audioSource.Play();
            }
            embientHelperCmp.timeToNextShot = Random.Range(15, 40);
        }
        foreach (var changeUnderNightLightPlayerStateEventEntity in _changeUnderNightLightPlayerStateEventsFilter.Value)
        {
            playerCmp.underNightLightRadius = _changeUnderNightLightPlayerStateEventsPool.Value.Get(changeUnderNightLightPlayerStateEventEntity).playerCheckColliderRadius;
        }

        FieldOfViewCheck();
        if (gloabalTimeCmp.isNight && (playerCmp.useFlashlight || playerCmp.underNightLightRadius != 0))
        {
            playerCmp.underNightLightTime += Time.deltaTime;
            if (playerCmp.underNightLightTime > 1)
            {
                var needCheckRadius = playerCmp.underNightLightRadius != 0 ? playerCmp.underNightLightRadius : _inventoryItemComponentsPool.Value.Get(_sceneService.Value.flashlightItemCellView._entity).itemInfo.flashlightInfo.lightRange;
                playerCmp.underNightLightTime = 0;

                var triggeredEnemies = Physics2D.CircleCastAll(moveCmp.entityTransform.position, needCheckRadius, moveCmp.entityTransform.up, needCheckRadius, LayerMask.GetMask("Enemy"));
                foreach (var enemy in triggeredEnemies)
                {
                    var directionToEnemy = enemy.transform.position - moveCmp.entityTransform.position;
                    //  var angleToEnemy = new Quaternion(0, 0, GetAngleFromVectorFloat(directionToEnemy), 0);
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(moveCmp.entityTransform.position, directionToEnemy.normalized, needCheckRadius, LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Enemy"));

                    if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject.layer != 10)
                    {
                        int enemyEntity = enemy.transform.gameObject.GetComponent<HealthView>()._entity;
                        ref var aiCmpEnemy = ref _creatureAIComponentsPool.Value.Get(enemyEntity);
                        aiCmpEnemy.targetPositionCached = moveCmp.entityTransform.position;
                        aiCmpEnemy.reachedLastTarget = false;
                        if (aiCmpEnemy.currentState == CreatureAIComponent.CreatureStates.idle)
                        {
                            aiCmpEnemy.timeFromLastTargetSeen = -5f;
                            aiCmpEnemy.currentState = CreatureAIComponent.CreatureStates.follow;
                        }
                    }
                }

            }
        }

        ref var lockerMinigameCmp = ref _lockerMinigameCompnentsPool.Value.Get(_playerEntity);

        if (lockerMinigameCmp.inGame)
        {
            lockerMinigameCmp.curSpeed = 50;
            var masterKeyContainerTransform = _sceneService.Value.dropedItemsUIView.lockerCellsContainer.transform;
            if (lockerMinigameCmp.stopedTime > 0)
                lockerMinigameCmp.stopedTime -= Time.deltaTime;
            else
            {
                // Debug.Log("WinGame" + masterKeyContainerTransform.transform.localPosition.x);
                if (!lockerMinigameCmp.inLeft)
                    masterKeyContainerTransform.transform.localPosition = Vector2.MoveTowards(masterKeyContainerTransform.transform.localPosition, new Vector2(55, masterKeyContainerTransform.transform.localPosition.y), lockerMinigameCmp.curSpeed * Time.deltaTime);
                else
                    masterKeyContainerTransform.transform.localPosition = Vector2.MoveTowards(masterKeyContainerTransform.transform.localPosition, new Vector2(-51, masterKeyContainerTransform.transform.localPosition.y), lockerMinigameCmp.curSpeed * Time.deltaTime);

                if (masterKeyContainerTransform.transform.localPosition.x > 54)
                    lockerMinigameCmp.inLeft = true;
                else if (masterKeyContainerTransform.transform.localPosition.x < -50)
                    lockerMinigameCmp.inLeft = false;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    lockerMinigameCmp.stopedTime = 1f;
                    _sceneService.Value.dropedItemsUIView.masterkeyAnimator.SetTrigger("UseMasterkey");
                    bool isLose = true;
                    var masterKeyContainerRectTransform = masterKeyContainerTransform.GetComponent<RectTransform>();
                    for (int i = 0; i < lockerMinigameCmp.lockerCells.Count; i++)
                    {
                        var curCell = lockerMinigameCmp.lockerCells[i];
                        var curCellRect = curCell.GetComponent<RectTransform>();
                        if (masterKeyContainerRectTransform.localPosition.x < curCellRect.localPosition.x + 3.2f && masterKeyContainerRectTransform.localPosition.x > curCellRect.localPosition.x - 3.2f)
                        {
                            lockerMinigameCmp.needCount--;
                            curCell.LockActivate(true);
                            lockerMinigameCmp.lockerCells.RemoveAt(i);
                            isLose = false;
                            break;
                        }
                    }
                    if (isLose || lockerMinigameCmp.needCount == 0)
                        EndLockMinigame();
                }

            }
        }
    }

    private void ChangeAlfaAllSpriteInGroup(float needAlpfa, Tilemap[] spriteRenderers)
    {
        foreach (Tilemap spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = needAlpfa;
            spriteRenderer.color = color;
        }
    }

    private void StartLockMinigame(int needCount, float masterkeySpeed)
    {
        ref var lockerMinigameCmp = ref _lockerMinigameCompnentsPool.Value.Get(_playerEntity);
        lockerMinigameCmp.curSpeed = masterkeySpeed;
        lockerMinigameCmp.stopedTime = 1f;
        _sceneService.Value.dropedItemsUIView.masterkeyMinigameContainer.gameObject.SetActive(true);
        _sceneService.Value.dropedItemsUIView.lockerCellsContainer.transform.localPosition = new Vector2(-47, _sceneService.Value.dropedItemsUIView.lockerCellsContainer.transform.localPosition.y);
        lockerMinigameCmp.needCount = needCount;
        lockerMinigameCmp.lockerCells = new List<LockCellView>();
        for (int i = 0; i < needCount; i++)
        {
            lockerMinigameCmp.lockerCells.Add(_sceneService.Value.dropedItemsUIView.lockerCellViews[i]);
            lockerMinigameCmp.lockerCells[i].gameObject.transform.localPosition = new Vector2(Random.Range(96 / needCount * i + 2, 96 / needCount * (i + 1) - 2) - 46, lockerMinigameCmp.lockerCells[i].gameObject.transform.localPosition.y);
            lockerMinigameCmp.lockerCells[i].gameObject.SetActive(true);
            lockerMinigameCmp.lockerCells[i].LockActivate(false);
        }
        lockerMinigameCmp.inGame = true;
    }

    private void EndLockMinigame()
    {
        ref var lockerMinigameCmp = ref _lockerMinigameCompnentsPool.Value.Get(_playerEntity);
        if (lockerMinigameCmp.needCount == 0)
        {
            if (lockerMinigameCmp.interactCharacter._characterType == InteractNPCType.openedDoor)
            {
                var openedDoorView = _currentInteractedCharactersComponentsPool.Value.Get(_playerEntity).interactCharacterView.gameObject.GetComponent<OpenedDoorView>();
                openedDoorView.doorCollider.enabled = false;
                openedDoorView.gameObject.GetComponent<SpriteRenderer>().sprite = openedDoorView.openDoorSprite;
            }
            else if(lockerMinigameCmp.interactCharacter._characterType == InteractNPCType.openedContainer)
            {
                var dropView = _currentInteractedCharactersComponentsPool.Value.Get(_playerEntity).interactCharacterView.gameObject.GetComponent<DroppedItemsListView>();
                //dropView.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                dropView.gameObject.GetComponent<Collider2D>().enabled = false;
                //
                int curItems = 0;
                int percentDrop = Random.Range(0, 101);

                for (int i = 0; i < dropView.dropElements.Length; i++)
                {
                    if (percentDrop <= dropView.dropElements[i].dropPercent)
                    {
                        int droopedCount = Random.Range(dropView.dropElements[i].itemsCountMin, dropView.dropElements[i].itemsCountMax + 1);
                        percentDrop = Random.Range(0, 101);
                        int droppedItemEntity = _world.Value.NewEntity();
                        curItems++;
                        ref var droppedItemComponent = ref _droppedItemComponentsPool.Value.Add(droppedItemEntity);
                        droppedItemComponent.currentItemsCount = droopedCount;

                        Vector2 deathPos = dropView.gameObject.transform.position;
                        droppedItemComponent.itemInfo = dropView.dropElements[i].droopedItem;
                        droppedItemComponent.droppedItemView = _sceneService.Value.SpawnDroppedItem(deathPos, dropView.dropElements[i].droopedItem, droppedItemEntity);
                        _hidedObjectOutsideFOVComponentsPool.Value.Add(droppedItemEntity).hidedObjects = new Transform[] { droppedItemComponent.droppedItemView.transform.GetChild(0) };

                        if (droppedItemComponent.itemInfo.type == ItemInfo.itemType.gun)
                        {
                            ref var gunInvCmp = ref _gunInventoryCellComponentsPool.Value.Add(droppedItemEntity);
                            gunInvCmp.currentGunWeight = droppedItemComponent.itemInfo.itemWeight;
                            gunInvCmp.gunDurability = (int)Random.Range(droppedItemComponent.itemInfo.gunInfo.maxDurabilityPoints * 0.3f, droppedItemComponent.itemInfo.gunInfo.maxDurabilityPoints);
                            gunInvCmp.gunPartsId = new int[4];
                            gunInvCmp.isEquipedWeapon = false;
                        }
                        else if (droppedItemComponent.itemInfo.type == ItemInfo.itemType.flashlight || droppedItemComponent.itemInfo.type == ItemInfo.itemType.bodyArmor || droppedItemComponent.itemInfo.type == ItemInfo.itemType.helmet)
                        {
                            if (droppedItemComponent.itemInfo.type == ItemInfo.itemType.flashlight)
                                _durabilityInInventoryComponentsPool.Value.Add(droppedItemEntity).currentDurability = (int)Random.Range(droppedItemComponent.itemInfo.flashlightInfo.maxChargedTime * 0.3f, droppedItemComponent.itemInfo.flashlightInfo.maxChargedTime);
                            else if (droppedItemComponent.itemInfo.type == ItemInfo.itemType.bodyArmor)
                                _durabilityInInventoryComponentsPool.Value.Add(droppedItemEntity).currentDurability = (int)Random.Range(droppedItemComponent.itemInfo.bodyArmorInfo.armorDurability * 0.3f, droppedItemComponent.itemInfo.bodyArmorInfo.armorDurability);
                            else if (droppedItemComponent.itemInfo.type == ItemInfo.itemType.helmet)
                                _durabilityInInventoryComponentsPool.Value.Add(droppedItemEntity).currentDurability = (int)Random.Range(droppedItemComponent.itemInfo.helmetInfo.armorDurability * 0.3f, droppedItemComponent.itemInfo.helmetInfo.armorDurability);
                            if (droppedItemComponent.itemInfo.type == ItemInfo.itemType.helmet && droppedItemComponent.itemInfo.helmetInfo.addedLightIntancity != 0)
                                _shieldComponentsPool.Value.Add(droppedItemEntity).currentDurability = (int)Random.Range(droppedItemComponent.itemInfo.helmetInfo.nightTimeModeDuration * 0.3f, droppedItemComponent.itemInfo.helmetInfo.nightTimeModeDuration);
                        }
                        else if (droppedItemComponent.itemInfo.type == ItemInfo.itemType.sheild)
                            _shieldComponentsPool.Value.Add(droppedItemEntity).currentDurability = (int)Random.Range(droppedItemComponent.itemInfo.sheildInfo.sheildDurability * 0.3f, droppedItemComponent.itemInfo.sheildInfo.sheildDurability);

                        if (curItems >= dropView.maxDroppedItemsCount)
                            break;
                    }
                }




            }
            Debug.Log("WinGame");
        }
        else
            Debug.Log("LoseGame");
        for (int i = 0; i < _sceneService.Value.dropedItemsUIView.lockerCellViews.Length; i++)
        {
            _sceneService.Value.dropedItemsUIView.lockerCellViews[i].gameObject.SetActive(false);
        }
        lockerMinigameCmp.inGame = false;
        _sceneService.Value.dropedItemsUIView.masterkeyMinigameContainer.gameObject.SetActive(false);
    }
    private void FieldOfViewCheck()
    {
        ref var moveCmp = ref _movementComponentPool.Value.Get(_playerEntity);

        ref var playerCmp = ref _playerComponentsPool.Value.Get(_playerEntity);
        ref var fOVCmp = ref _fieldOfViewComponentsPool.Value.Get(_playerEntity);
        float fov = fOVCmp.fieldOfView;
        float viewDistance = fOVCmp.viewDistance;
        Vector3 origin = new Vector2(playerCmp.view.movementView.transform.position.x, playerCmp.view.movementView.transform.position.y - 0.2f);
        int rayCount = Mathf.CeilToInt(fov / 3);
        float angle = GetAngleFromVectorFloat(moveCmp.pointToRotateInput - (Vector2)origin) - fov / 2;
        float angleIncrease = fov / rayCount;

        List<Collider2D> checkedColliders = new List<Collider2D>();

        fOVCmp.timeBeforeDetect += Time.deltaTime;
        if (fOVCmp.timeBeforeDetect >= 0.12f)
        {
            fOVCmp.timeBeforeDetect = 0;
            LayerMask needLayers = LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("InteractedCharacter") | LayerMask.GetMask("DroopedItem") | LayerMask.GetMask("BrokedObject") | LayerMask.GetMask("Trap");
            for (int i = 0; i < rayCount; i++)
            {
                RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, needLayers);

                Debug.DrawRay(origin, GetVectorFromAngle(angle) * viewDistance, Color.yellow);
                if (raycastHit2D.collider != null)
                {
                    bool isHasThisCollider = false;
                    foreach (var col in checkedColliders)
                        if (col == raycastHit2D.collider)
                            isHasThisCollider = true;
                    if (!isHasThisCollider)
                    {
                        checkedColliders.Add(raycastHit2D.collider);
                        int objectLayer = raycastHit2D.collider.gameObject.layer;
                        switch (objectLayer)
                        {
                            case 7:
                                ref var hidedObjCmp = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(raycastHit2D.collider.gameObject.GetComponent<CreatureView>().entity);//может быть как то оптимизировать
                                hidedObjCmp.timeBeforeHide = 0.5f;
                                foreach (var spriteRenderer in hidedObjCmp.hidedObjects)
                                    spriteRenderer.gameObject.SetActive(true);
                                break;

                            case 8:
                                if (_hidedObjectOutsideFOVComponentsPool.Value.Has(raycastHit2D.collider.gameObject.GetComponent<InteractCharacterView>()._entity))
                                {
                                    ref var hidedObjCmp1 = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(raycastHit2D.collider.gameObject.GetComponent<InteractCharacterView>()._entity);

                                    hidedObjCmp1.timeBeforeHide = 0.5f;
                                    foreach (var spriteRenderer in hidedObjCmp1.hidedObjects)
                                        spriteRenderer.gameObject.SetActive(true);
                                }
                                break;

                            case 3:
                                ref var hidedObjCmp2 = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(raycastHit2D.collider.gameObject.GetComponent<DroppedItemView>().itemEntity);
                                hidedObjCmp2.timeBeforeHide = 0.5f;
                                Debug.Log(hidedObjCmp2.hidedObjects.Length + "Hide dropped item");
                                foreach (var spriteRenderer in hidedObjCmp2.hidedObjects)
                                    spriteRenderer.gameObject.SetActive(true);
                                break;

                            case 17:
                                HealthView healthView;
                                if(raycastHit2D.collider.gameObject.TryGetComponent(out healthView))
                                {
                                ref var hidedObjCmp3 = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(healthView._entity);
                                hidedObjCmp3.timeBeforeHide = 0.5f;
                                Debug.Log(hidedObjCmp3.hidedObjects.Length + "Hide dropped item");
                                foreach (var spriteRenderer in hidedObjCmp3.hidedObjects)
                                    spriteRenderer.gameObject.SetActive(true);
                                }
                                break;
                            case 15:
                                ref var hidedObjCmp4 = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(raycastHit2D.collider.gameObject.GetComponent<TrapView>().entity);
                                hidedObjCmp4.timeBeforeHide = 0.5f;
                                foreach (var spriteRenderer in hidedObjCmp4.hidedObjects)
                                    spriteRenderer.gameObject.SetActive(true);
                                break;
                        }
                    }
                }
                angle += angleIncrease;
            }

            angleIncrease = (360 - fov) / rayCount;
            origin = new Vector2(playerCmp.view.movementView.transform.position.x, playerCmp.view.movementView.transform.position.y - 0.9f);

            for (int i = 0; i < rayCount; i++)//check around
            {
                RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), 2f, needLayers);
                Debug.DrawRay(origin, GetVectorFromAngle(angle) * 2, Color.yellow);
                if (raycastHit2D.collider != null)
                {
                    bool isHasThisCollider = false;
                    foreach (var col in checkedColliders)
                        if (col == raycastHit2D.collider)
                            isHasThisCollider = true;
                    if (!isHasThisCollider)
                    {
                        checkedColliders.Add(raycastHit2D.collider);
                        int objectLayer = raycastHit2D.collider.gameObject.layer;
                        switch (objectLayer)
                        {
                            case 7:
                                ref var hidedObjCmp = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(raycastHit2D.collider.gameObject.GetComponent<CreatureView>().entity);//может быть как то оптимизировать

                                hidedObjCmp.timeBeforeHide = 0.5f;
                                foreach (var spriteRenderer in hidedObjCmp.hidedObjects)
                                    spriteRenderer.gameObject.SetActive(true);
                                break;

                            case 8:
                                if (_hidedObjectOutsideFOVComponentsPool.Value.Has(raycastHit2D.collider.gameObject.GetComponent<InteractCharacterView>()._entity))
                                {
                                    ref var hidedObjCmp1 = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(raycastHit2D.collider.gameObject.GetComponent<InteractCharacterView>()._entity);

                                    hidedObjCmp1.timeBeforeHide = 0.5f;
                                    foreach (var spriteRenderer in hidedObjCmp1.hidedObjects)
                                        spriteRenderer.gameObject.SetActive(true);
                                }
                                break;

                            case 3:
                                ref var hidedObjCmp2 = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(raycastHit2D.collider.gameObject.GetComponent<DroppedItemView>().itemEntity);
                                hidedObjCmp2.timeBeforeHide = 0.5f;
                                Debug.Log(hidedObjCmp2.hidedObjects.Length + "Hide dropped item");
                                foreach (var spriteRenderer in hidedObjCmp2.hidedObjects)
                                    spriteRenderer.gameObject.SetActive(true);
                                break;

                            case 17:
                                ref var hidedObjCmp3 = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(raycastHit2D.collider.gameObject.GetComponent<HealthView>()._entity);
                                hidedObjCmp3.timeBeforeHide = 0.5f;
                                Debug.Log(hidedObjCmp3.hidedObjects.Length + "Hide dropped item");
                                foreach (var spriteRenderer in hidedObjCmp3.hidedObjects)
                                    spriteRenderer.gameObject.SetActive(true);
                                break;
                            case 15:
                                ref var hidedObjCmp4 = ref _hidedObjectOutsideFOVComponentsPool.Value.Get(raycastHit2D.collider.gameObject.GetComponent<TrapView>().entity);
                                hidedObjCmp4.timeBeforeHide = 0.5f;
                                foreach (var spriteRenderer in hidedObjCmp4.hidedObjects)
                                    spriteRenderer.gameObject.SetActive(true);
                                break;
                        }
                    }
                }
                angle += angleIncrease;
            }

        }
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }


    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
            n += 360;
        return n;
    }

    private int FindItemCountInInventory(int itemId, bool isDelete)
    {
        int findedItemsCount = 0;

        foreach (var invItem in _inventoryItemsFilter.Value)
        {
            var invItemCmp = _inventoryItemComponentsPool.Value.Get(invItem);
            if (invItemCmp.itemInfo.itemId == itemId)
                findedItemsCount += invItemCmp.currentItemsCount;

            if (isDelete)
            {
                isDelete = false;
                _deleteItemEventsPool.Value.Add(invItem).count = 1;
            }
        }

        return findedItemsCount;
    }
}
