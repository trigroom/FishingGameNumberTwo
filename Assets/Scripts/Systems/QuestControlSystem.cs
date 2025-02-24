using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.UI;

public class QuestControlSystem : IEcsInitSystem, IEcsRunSystem
{

    private EcsPoolInject<HidedObjectOutsideFOVComponent> _hidedObjectOutsideFOVComponentsPool;
    private EcsPoolInject<QuestComponent> _questComponentsPool;
    private EcsPoolInject<QuestNPCComponent> _questNPCComponentsPool;
    private EcsPoolInject<NPCStartDialogeEvent> _npcStartDialogeEventsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<InventoryItemComponent> _inventoryItemComponentsPool;
    private EcsPoolInject<CurrentDialogeComponent> _currentDialogeComponentsPool;
    private EcsPoolInject<DeathEvent> _deathEventsPool;
    private EcsPoolInject<CreatureInventoryComponent> _creatureInventoryComponentsPool;
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<CurrentLocationComponent> _currentLocationComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<CurrentInteractedCharactersComponent> _currentInteractedCharactersComponentsPool;
    private EcsPoolInject<TrapIsNeutralizedEvent> _trapIsNeutralizedEventsPool;
    private EcsPoolInject<MenuStatesComponent> _menuStatesComponentsPool;

    private EcsFilterInject<Inc<NPCStartDialogeEvent>> _npcStartDialogeEventsFilter;
    private EcsFilterInject<Inc<TrapIsNeutralizedEvent>> _trapIsNeutralizedEventsFilter;
    private EcsFilterInject<Inc<QuestComponent>> _questComponentsFilter;
    private EcsFilterInject<Inc<OpenQuestHelperEvent>> _openQuestHelperEventsFilter;
    private EcsFilterInject<Inc<DeathEvent>, Exc<PlayerComponent>> _creatureDeathEventsFilter;

    private EcsCustomInject<SceneService> _sceneService;

    private EcsWorldInject _world;
    public void Init(IEcsSystems systems)
    {
        int curIndex = 0;
        foreach (var questChar in _sceneService.Value.interactCharacters)
        {
            if (questChar._characterType != InteractCharacterView.InteractNPCType.shop)
            {
                int charEntity = 0;
                if (questChar._characterType == InteractCharacterView.InteractNPCType.gunsmith)
                {
                    charEntity = _world.Value.NewEntity();
                    questChar.Construct(_world.Value, charEntity);
                    _hidedObjectOutsideFOVComponentsPool.Value.Add(charEntity).hidedObjects = new Transform[] { questChar.gameObject.transform.GetChild(0)/*поменять если где то ещё понадобится этот спрайт рэндэр*/ };
                }
                else
                {
                    charEntity = questChar._entity;
                }
                _questNPCComponentsPool.Value.Add(charEntity).characterId = curIndex;
                questChar.GetComponent<QuestCharacterView>().characterId = curIndex;
            }
            curIndex++;
        }
        _sceneService.Value.dropedItemsUIView.questDescriptionLastButton.onClick.AddListener(LastQuestPage);
        _sceneService.Value.dropedItemsUIView.questDescriptionNextButton.onClick.AddListener(NextQuestPage);

        _sceneService.Value.dropedItemsUIView.bookmarkViews[0].GetComponent<Button>().onClick.AddListener(ChangeBookmarkToQuest);
        _sceneService.Value.dropedItemsUIView.bookmarkViews[1].GetComponent<Button>().onClick.AddListener(ChangeBookmarkToGuide);

       // _sceneService.Value.questMenuView.transform.GetChild(0).gameObject.SetActive(true);
        _sceneService.Value.dropedItemsUIView.bookmarkViews[0].animator.SetBool("BookmarkIsActive", true);
       // _sceneService.Value.questMenuView.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Run(IEcsSystems systems)
    {
        ref var curDialogPlayerCmp = ref _currentDialogeComponentsPool.Value.Get(_sceneService.Value.playerEntity);
        foreach (var npc in _npcStartDialogeEventsFilter.Value)
        {
            int npcId = _npcStartDialogeEventsPool.Value.Get(npc).questNPCId;
            curDialogPlayerCmp.dialogeIsStarted = true;
            curDialogPlayerCmp.npcId = npcId;
            var curNPC = _sceneService.Value.interactCharacters[npcId].GetComponent<QuestCharacterView>();
            var questNPCCmp = _questNPCComponentsPool.Value.Get(_currentInteractedCharactersComponentsPool.Value.Get(_sceneService.Value.playerEntity).interactCharacterView._entity);
            _sceneService.Value.dropedItemsUIView.dialogeText.text = curNPC.questNode[questNPCCmp.currentQuest].dialogeText[0];
            _sceneService.Value.dropedItemsUIView.characterNameText.text = curNPC.characterName;


            Debug.Log("npcId " + npcId);
        }
        foreach (var trap in _trapIsNeutralizedEventsFilter.Value)
        {
            var trapType = (int)_trapIsNeutralizedEventsPool.Value.Get(trap).trapType;

            foreach (var quest in _questComponentsFilter.Value)
            {
                ref var questCmp = ref _questComponentsPool.Value.Get(quest);
                // if (questCmp.questComplited) continue;
                var questNPCCmp = _questNPCComponentsPool.Value.Get(_sceneService.Value.interactCharacters[questCmp.questCharacterId]._entity);
                var questNPC = _sceneService.Value.interactCharacters[questCmp.questCharacterId].GetComponent<QuestCharacterView>();
                Debug.Log(questNPC.characterName + " cur npc quest checked");
                for (int i = 0; i < questNPC.questNode[questNPCCmp.currentQuest].tasks.Length; i++)
                    if (questNPC.questNode[questNPCCmp.currentQuest].tasks[i].questType == QuestNodeElement.QuestType.neutralizeTrap && questNPC.questNode[questNPCCmp.currentQuest].tasks[i].neededId == trapType)
                        questCmp.curerntCollectedItems[i]++;
            }
        }
        foreach (var enemyDeath in _creatureDeathEventsFilter.Value)
        {
            Debug.Log("EnemyIsKilled");
            bool isHeadshot = _deathEventsPool.Value.Get(enemyDeath).isHeadshot;
            string curLocation = _currentLocationComponentsPool.Value.Get(_sceneService.Value.playerEntity).currentLocation.locationName;
            var creatureInventory = _creatureInventoryComponentsPool.Value.Get(enemyDeath);
            var curTime = _globalTimeComponentsPool.Value.Get(_sceneService.Value.playerEntity).currentDayTime;
            float distanceToPlayer = Vector2.Distance(_movementComponentsPool.Value.Get(enemyDeath).movementView.gameObject.transform.position, _movementComponentsPool.Value.Get(_sceneService.Value.playerEntity).movementView.gameObject.transform.position);
            var playerWeapons = _playerWeaponsInInventoryComponentsPool.Value.Get(_sceneService.Value.playerEntity);
            int playerGunType = 0;
            if (playerWeapons.curWeapon != 2)
                playerGunType = (int)_inventoryItemComponentsPool.Value.Get(playerWeapons.curEquipedWeaponCellEntity).itemInfo.gunInfo.gunType;

            foreach (var quest in _questComponentsFilter.Value)
            {
                ref var questCmp = ref _questComponentsPool.Value.Get(quest);
                // if (questCmp.questComplited) continue;
                var questNPCCmp = _questNPCComponentsPool.Value.Get(_sceneService.Value.interactCharacters[questCmp.questCharacterId]._entity);
                var questNPC = _sceneService.Value.interactCharacters[questCmp.questCharacterId].GetComponent<QuestCharacterView>();
                Debug.Log(questNPC.characterName + " cur npc quest checked");
                for (int i = 0; i < questNPC.questNode[questNPCCmp.currentQuest].tasks.Length; i++)
                {
                    if (questNPC.questNode[questNPCCmp.currentQuest].tasks[i].questType == QuestNodeElement.QuestType.killSomeone)
                    {
                        // bool addKill = true;
                        var currentKillTask = questNPC.questNode[questNPCCmp.currentQuest].tasks[i].killTaskInfo;

                        if ((currentKillTask.playerGunType == 6 && playerWeapons.curWeapon != 2) || (currentKillTask.playerGunType < 6 && (playerWeapons.curWeapon == 2) || (playerWeapons.curWeapon != 2 && playerGunType != currentKillTask.playerGunType)))
                            continue;
                        if ((currentKillTask.enemyGunType == 6 && (creatureInventory.meleeWeaponItem == null || creatureInventory.gunItem != null && !creatureInventory.isSecondWeaponUsed)) || (currentKillTask.enemyGunType < 6 && (creatureInventory.gunItem == null || (creatureInventory.gunItem != null && (creatureInventory.isSecondWeaponUsed ||
                            !creatureInventory.isSecondWeaponUsed && (int)creatureInventory.gunItem.gunType != currentKillTask.enemyGunType)))))
                            continue;
                        if ((currentKillTask.minDistanceToKill != 0 && currentKillTask.minDistanceToKill > distanceToPlayer) || (currentKillTask.maxDistanceToKill != 0 && currentKillTask.minDistanceToKill < distanceToPlayer))
                            continue;
                        if (currentKillTask.isHeadshotKill && !isHeadshot)
                            continue;
                        if (currentKillTask.minTimeToKill != 0 && currentKillTask.maxTimeToKill != 0 && currentKillTask.minTimeToKill > curTime && currentKillTask.maxTimeToKill < curTime)
                            continue;
                        if (currentKillTask.locationToKill != "" && currentKillTask.locationToKill != curLocation)
                            continue;

                        questCmp.curerntCollectedItems[i]++;
                        //   Debug.Log(questCmp.curerntCollectedItems[i] + " killed enemies for quest");
                    }
                }
            }
            _world.Value.DelEntity(enemyDeath);
        }
        foreach (var openQuestHelper in _openQuestHelperEventsFilter.Value)
        {
            var menuStatesCmp = _menuStatesComponentsPool.Value.Get(_sceneService.Value.playerEntity);
            if (menuStatesCmp.currentBookShowState == MenuStatesComponent.CurrentBookShowState.quests)
            {
                
                //  curDialogPlayerCmp.currentPageNumber = 0;
                if (_questComponentsFilter.Value.GetEntitiesCount() <= 3)
                    _sceneService.Value.dropedItemsUIView.questDescriptionNextButton.gameObject.SetActive(false);
                else
                    _sceneService.Value.dropedItemsUIView.questDescriptionNextButton.gameObject.SetActive(true);
                if (curDialogPlayerCmp.currentPageNumber == 0)
                    _sceneService.Value.dropedItemsUIView.questDescriptionLastButton.gameObject.SetActive(false);
                else
                    _sceneService.Value.dropedItemsUIView.questDescriptionLastButton.gameObject.SetActive(true);

                UpdateQuestHelperPage(curDialogPlayerCmp.currentPageNumber);
            }
            else
            {
            }
        //    Debug.Log(_sceneService.Value.dropedItemsUIView.bookmarkViews[(int)menuStatesCmp.currentBookShowState].gameObject.activeInHierarchy + " BookmarkIsActive");
            //    _sceneService.Value.dropedItemsUIView.bookmarkViews[(int)menuStatesCmp.currentBookShowState].animator.SetBool("BookmarkIsActive", true);
        }
        //прогонять через фильтр смертей все квесты

        if (curDialogPlayerCmp.dialogeIsStarted && Input.GetKeyDown(KeyCode.Space))
        {
            curDialogPlayerCmp.currentDialogeNumber++;
            var curNPC = _sceneService.Value.interactCharacters[curDialogPlayerCmp.npcId].GetComponent<QuestCharacterView>();
            ref var questNPCCmp = ref _questNPCComponentsPool.Value.Get(_currentInteractedCharactersComponentsPool.Value.Get(_sceneService.Value.playerEntity).interactCharacterView._entity);
            if (curDialogPlayerCmp.currentDialogeNumber == curNPC.questNode[questNPCCmp.currentQuest].dialogeText.Length)
            {
                //добавление квеста в список
                Debug.Log("quest added");
                questNPCCmp.questIsGiven = true;
                ref var questCmp = ref _questComponentsPool.Value.Add(curNPC.GetComponent<InteractCharacterView>()._entity);
                questCmp.curerntCollectedItems = new int[curNPC.questNode[questNPCCmp.currentQuest].tasks.Length];
                questCmp.questCharacterId = curNPC.characterId;
                questCmp.quest = curNPC.questNode[questNPCCmp.currentQuest];//под вопросом

                curDialogPlayerCmp.dialogeIsStarted = false;
                curDialogPlayerCmp.currentDialogeNumber = 0;
                _sceneService.Value.dropedItemsUIView.dialogeText.text = "";
                _sceneService.Value.dropedItemsUIView.characterNameText.text = "";
                _currentInteractedCharactersComponentsPool.Value.Get(_sceneService.Value.playerEntity).isNPCNowIsUsed = false;
                return;
            }

            _sceneService.Value.dropedItemsUIView.dialogeText.text = curNPC.questNode[questNPCCmp.currentQuest].dialogeText[curDialogPlayerCmp.currentDialogeNumber];
        }
    }

    public void ChangeBookmarkToQuest()
    {
        Debug.Log("close guide");

        ref var menuStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneService.Value.playerEntity);

        _sceneService.Value.dropedItemsUIView.bookmarkViews[(int)menuStatesCmp.currentBookShowState].animator.SetBool("BookmarkIsActive", false);
        menuStatesCmp.currentBookShowState = MenuStatesComponent.CurrentBookShowState.quests;
        _sceneService.Value.dropedItemsUIView.bookmarkViews[(int)menuStatesCmp.currentBookShowState].animator.SetBool("BookmarkIsActive", true);

        int pageNumber = _currentDialogeComponentsPool.Value.Get(_sceneService.Value.playerEntity).currentPageNumber;
        if (_questComponentsFilter.Value.GetEntitiesCount() <= 3)
            _sceneService.Value.dropedItemsUIView.questDescriptionNextButton.gameObject.SetActive(false);
        else
            _sceneService.Value.dropedItemsUIView.questDescriptionNextButton.gameObject.SetActive(true);
        if (pageNumber == 0)
            _sceneService.Value.dropedItemsUIView.questDescriptionLastButton.gameObject.SetActive(false);
        else
            _sceneService.Value.dropedItemsUIView.questDescriptionLastButton.gameObject.SetActive(true);

        _sceneService.Value.questDescription[1].gameObject.SetActive(true);
        _sceneService.Value.questDescription[2].gameObject.SetActive(true);
        _sceneService.Value.dropedItemsUIView.guideImage.gameObject.SetActive(false);

        UpdateQuestHelperPage(pageNumber);
    }

    public void ChangeBookmarkToGuide()
    {
        ref var menuStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneService.Value.playerEntity);
        _sceneService.Value.questDescription[1].gameObject.SetActive(false);
        _sceneService.Value.questDescription[2].gameObject.SetActive(false);
        _sceneService.Value.dropedItemsUIView.bookmarkViews[(int)menuStatesCmp.currentBookShowState].animator.SetBool("BookmarkIsActive", false);
        menuStatesCmp.currentBookShowState = MenuStatesComponent.CurrentBookShowState.guides;
        _sceneService.Value.dropedItemsUIView.bookmarkViews[(int)menuStatesCmp.currentBookShowState].animator.SetBool("BookmarkIsActive", true);

        _sceneService.Value.dropedItemsUIView.questDescriptionNextButton.gameObject.SetActive(true);
        _sceneService.Value.dropedItemsUIView.questDescriptionLastButton.gameObject.SetActive(true);

        _sceneService.Value.dropedItemsUIView.guideImage.gameObject.SetActive(true);
        UpdateGuidePage(menuStatesCmp.currentGuidePage);
        Debug.Log("open guide");
    }
    public void NextQuestPage()
    {
        ref var menuStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneService.Value.playerEntity);

        if (menuStatesCmp.currentBookShowState == MenuStatesComponent.CurrentBookShowState.quests)
        {
            ref var curDialogPlayerCmp = ref _currentDialogeComponentsPool.Value.Get(_sceneService.Value.playerEntity);
            curDialogPlayerCmp.currentPageNumber++;
            if (_questComponentsFilter.Value.GetEntitiesCount() <= (curDialogPlayerCmp.currentPageNumber + 1) * 3)
                _sceneService.Value.dropedItemsUIView.questDescriptionNextButton.gameObject.SetActive(false);
            if (!_sceneService.Value.dropedItemsUIView.questDescriptionLastButton.IsActive())
                _sceneService.Value.dropedItemsUIView.questDescriptionLastButton.gameObject.SetActive(true);
            UpdateQuestHelperPage(curDialogPlayerCmp.currentPageNumber);
        }
        else
        {
            menuStatesCmp.currentGuidePage++;
            if (_sceneService.Value.guidePagesInfo.Length <= menuStatesCmp.currentGuidePage)
                menuStatesCmp.currentGuidePage = 0;
            UpdateGuidePage(menuStatesCmp.currentGuidePage);
        }
    }
    public void LastQuestPage()
    {
        ref var menuStatesCmp = ref _menuStatesComponentsPool.Value.Get(_sceneService.Value.playerEntity);

        if (menuStatesCmp.currentBookShowState == MenuStatesComponent.CurrentBookShowState.quests)
        {
            ref var curDialogPlayerCmp = ref _currentDialogeComponentsPool.Value.Get(_sceneService.Value.playerEntity);
            curDialogPlayerCmp.currentPageNumber--;
            if (curDialogPlayerCmp.currentPageNumber == 0)
                _sceneService.Value.dropedItemsUIView.questDescriptionLastButton.gameObject.SetActive(false);
            else
                _sceneService.Value.dropedItemsUIView.questDescriptionLastButton.gameObject.SetActive(true);
            if (!_sceneService.Value.dropedItemsUIView.questDescriptionNextButton.IsActive())
                _sceneService.Value.dropedItemsUIView.questDescriptionNextButton.gameObject.SetActive(true);
            UpdateQuestHelperPage(curDialogPlayerCmp.currentPageNumber);
        }
        else
        {
            menuStatesCmp.currentGuidePage--;
            if (menuStatesCmp.currentGuidePage <= -1)
                menuStatesCmp.currentGuidePage = _sceneService.Value.guidePagesInfo.Length - 1;
            UpdateGuidePage(menuStatesCmp.currentGuidePage);
        }
    }
    private void UpdateGuidePage(int guidePage)
    {
        var curGuidePage = _sceneService.Value.guidePagesInfo[guidePage];
        _sceneService.Value.questDescription[0].text = curGuidePage.guideText;
        _sceneService.Value.dropedItemsUIView.guideImage.sprite = curGuidePage.quidePicture;
        _sceneService.Value.dropedItemsUIView.guideImage.SetNativeSize();
    }
    private void UpdateQuestHelperPage(int pageNumber)
    {
        int curDescription = -3 * pageNumber;
        foreach (var quest in _questComponentsFilter.Value)
        {
            if (curDescription >= 3) return;
            else if (curDescription >= 0)
            {
                ref var questCmp = ref _questComponentsPool.Value.Get(quest);
                ref var questNPCCmp = ref _questNPCComponentsPool.Value.Get(_sceneService.Value.interactCharacters[questCmp.questCharacterId]._entity);
                var questNPC = _sceneService.Value.interactCharacters[questCmp.questCharacterId].GetComponent<QuestCharacterView>();
                Debug.Log("quest desc num" + curDescription);
                ref var questDescription = ref _sceneService.Value.questDescription[curDescription];
                questDescription.text = "<b>" + questNPC.questNode[questNPCCmp.currentQuest].questName + "</b>" + "\n";
                questDescription.text += questNPC.questNode[questNPCCmp.currentQuest].questDescription + "\n";
                for (int i = 0; i < questNPC.questNode[questNPCCmp.currentQuest].tasks.Length; i++)
                {
                    var task = questNPC.questNode[questNPCCmp.currentQuest].tasks[i];
                    questDescription.text += (i + 1) + ")";
                    if (task.questType == QuestNodeElement.QuestType.killSomeone)
                    {
                        questDescription.text += questCmp.curerntCollectedItems[i] + "/" + task.neededCount + "kills";
                        if (task.killTaskInfo.playerGunType == 6)
                            questDescription.text += " from melee weapon";
                        else if (task.killTaskInfo.playerGunType < 6)
                            questDescription.text += " from " + (GunInfo.GunType)task.killTaskInfo.playerGunType;
                        if (task.killTaskInfo.maxDistanceToKill != 0)
                            questDescription.text += " at a distance closer then " + (GunInfo.GunType)task.killTaskInfo.playerGunType + " meters";
                        else if (task.killTaskInfo.minDistanceToKill != 0)
                            questDescription.text += " at a distance more then " + (GunInfo.GunType)task.killTaskInfo.playerGunType + " meters";

                        if (task.killTaskInfo.isHeadshotKill)
                            questDescription.text += " in the head";

                        if (task.killTaskInfo.maxTimeToKill != 0 && task.killTaskInfo.minTimeToKill != 0)
                        {
                            var curTime = _globalTimeComponentsPool.Value.Get(_sceneService.Value.playerEntity).currentDayTime;
                            int maxTime = curTime + _sceneService.Value.timeHourOffset + task.killTaskInfo.maxTimeToKill;
                            if (maxTime > 24)
                                maxTime -= 24;

                            int minTime = curTime + _sceneService.Value.timeHourOffset + task.killTaskInfo.minTimeToKill;
                            if (minTime > 24)
                                minTime -= 24;

                            questDescription.text += " from " + minTime + ":00 to " + maxTime + ":00";
                        }

                        if (task.killTaskInfo.locationToKill != "")
                            questDescription.text += " on location '" + task.killTaskInfo.locationToKill + "'";

                        if (task.killTaskInfo.enemyGunType == 6)
                            questDescription.text += " when enemy was with melee weapon";
                        else if (task.killTaskInfo.enemyGunType < 6)
                            questDescription.text += " when enemy was with " + (GunInfo.GunType)task.killTaskInfo.enemyGunType;

                    }
                    else if (task.questType == QuestNodeElement.QuestType.bringSomething)
                        questDescription.text += "bring " + task.neededCount + " " + _sceneService.Value.idItemslist.items[task.neededId].itemName;
                    else if (task.questType == QuestNodeElement.QuestType.neutralizeTrap)
                        questDescription.text += "neutalized " + (TrapView.TrapType)task.neededId + " " + questCmp.curerntCollectedItems[i] + "/" + task.neededCount;
                    questDescription.text += "\n";
                }
                //kill an enemy with a rifle at a distance of 30 meters in the head with a knife at night in the last location
                questDescription.text += "reward: " + "\n";

                for (int i = 0; i < questNPC.questNode[questNPCCmp.currentQuest].rewards.Length; i++)
                {
                    var reward = questNPC.questNode[questNPCCmp.currentQuest].rewards[i];

                    if (reward.rewardItemId != 999)
                        questDescription.text += reward.rewardItemsCount + " " + _sceneService.Value.idItemslist.items[reward.rewardItemId].itemName + "\n";
                    else
                        questDescription.text += reward.rewardItemsCount + "$" + "\n";
                }
            }
            curDescription++;
        }

        while (curDescription < 3)
        {
            _sceneService.Value.questDescription[curDescription].text = "";
            curDescription++;
        }
    }
}
