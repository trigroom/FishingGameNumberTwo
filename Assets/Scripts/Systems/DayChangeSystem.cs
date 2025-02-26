using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

public class DayChangeSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsPoolInject<GlobalTimeComponent> _globalTimeComponentsPool;
    private EcsPoolInject<ChangeToNightEvent> _changeToNightEventsPool;//возможно не понадобится, надо просто делать проверку на isNight в других скриптах
    private EcsPoolInject<ChangeToDayEvent> _changeToDayEventsPool;
    private EcsPoolInject<BuildingCheckerComponent> _buildingCheckerComponentsPool;
    private EcsPoolInject<EntryInNewLocationEvent> _entryInNewLocationEventsPool;
    private EcsPoolInject<CurrentLocationComponent> _currentLocationComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<SlowTextInstanceEvent> _slowTextInstanceEventsPool;
    private EcsPoolInject<EnemySpawnEvent> _enemySpawnEventsPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<DroppedItemComponent> _droppedItemComponentsPool;
    private EcsPoolInject<ShopCharacterComponent> _shopCharacterComponentsPool;
    private EcsPoolInject<SetupShoppersOnNewLocationEvent> _setupShoppersOnNewLocationEventsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;
    private EcsPoolInject<SaveGameEvent> _saveGameEventsPool;
    private EcsPoolInject<HidedObjectOutsideFOVComponent> _hidedObjectOutsideFOVComponentsPool;
    private EcsPoolInject<SolarPanelElectricGeneratorComponent> _solarPanelElectricGeneratorComponentsPool;

    private EcsFilterInject<Inc<DroppedItemComponent>> _droppedItemComponentsFilter;
    private EcsFilterInject<Inc<SlowTextInstanceEvent>> _slowTextInstanceEventsFilter;
    private EcsFilterInject<Inc<EntryInNewLocationEvent>> _entryInNewLocationEventsFilter;
    private EcsFilterInject<Inc<LoadGameEvent>> _loadGameEventsFilter;
    private EcsFilterInject<Inc<HealthComponent>, Exc<PlayerComponent>> _healthComponentsFilter;

    private EcsCustomInject<SceneService> _sceneService;
    private EcsWorldInject _world;

    public void Init(IEcsSystems systems)
    {
        _globalTimeComponentsPool.Value.Add(_sceneService.Value.playerEntity);
    }
    private void ChangeLevelPrefab(LocationSettingsInfo needLocation)
    {
        if (_playerComponentsPool.Value.Get(_sceneService.Value.playerEntity).nvgIsUsed)
        {
            _playerComponentsPool.Value.Get(_sceneService.Value.playerEntity).nvgIsUsed = false;
            _sceneService.Value.bloomMainBg.intensity.value = 0;
        }


        ref var curLocationCmp = ref _currentLocationComponentsPool.Value.Get(_sceneService.Value.playerEntity);
        curLocationCmp.currentEnemySpawns = null;

        //удалять все предметы с земли при переходе на некст локацию
        if (curLocationCmp.levelNum == 0)
        {
            _sceneService.Value.startLocation.gameObject.SetActive(false);
            curLocationCmp.currentLocation = needLocation;
        }

        else
        {
            foreach (var item in _droppedItemComponentsFilter.Value)
            {
                _droppedItemComponentsPool.Value.Get(item).droppedItemView.DestroyItemFromGround();
                _world.Value.DelEntity(item);
            }
            foreach (var trap in curLocationCmp.trapsPrefabs)
                _sceneService.Value.DestroyLevel(trap);//сделать пул ловушек

            foreach (var healthEntity in _healthComponentsFilter.Value)
                _world.Value.DelEntity(healthEntity);
            foreach (var iteractChar in _sceneService.Value.interactCharacters)
                iteractChar.gameObject.SetActive(false);

            //удалять всех врагов при переходе на некст локацию
            _sceneService.Value.DestroyLevel(curLocationCmp.currentLevelPrefab);
        }
        ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(_sceneService.Value.playerEntity);
        if (curLocationCmp.currentLocation == null || curLocationCmp.currentLocation.levels.Length == curLocationCmp.levelNum || needLocation == null)
        {
            foreach (var shopperIndex in _sceneService.Value.startShoppers)
            {
                _sceneService.Value.interactCharacters[shopperIndex].gameObject.SetActive(true);
                if (shopperIndex == 1) continue;
                _setupShoppersOnNewLocationEventsPool.Value.Add(_sceneService.Value.interactCharacters[shopperIndex]._entity);
            }
            _sceneService.Value.startLocation.gameObject.SetActive(true);
            if (curLocationCmp.levelNum != 0)
                _saveGameEventsPool.Value.Add(_sceneService.Value.playerEntity).type = DataPersistenceManagerSystem.SavePriority.fullSave;
            curLocationCmp.levelNum = 0;
            _movementComponentsPool.Value.Get(_sceneService.Value.playerEntity).movementView.gameObject.transform.position = Vector2.zero;

            _sceneService.Value.startLocationLightsContainer.gameObject.SetActive(globalTimeCmp.isNight);
            return;
        }
        else
        {

            curLocationCmp.currentLevelPrefab = _sceneService.Value.InstantiateLevel(needLocation.levels[curLocationCmp.levelNum].levelPrefab.transform);
            curLocationCmp.currentEnemySpawns = new List<Vector2>();
            var curLevelView = curLocationCmp.currentLevelPrefab.GetComponent<LevelSceneView>();
            globalTimeCmp.currentDayTime += 3;

            globalTimeCmp.levelsToRain--;
            if (globalTimeCmp.levelsToRain == 0)
            {
                globalTimeCmp.changedToRain = !globalTimeCmp.changedToRain;
                if (globalTimeCmp.changedToRain)
                {
                    globalTimeCmp.levelsToRain = Random.Range(10, 20);
                    globalTimeCmp.currentGlobalLightIntensity -= 0.1f;
                }
                else
                {
                    globalTimeCmp.levelsToRain = Random.Range(5, 10);
                    globalTimeCmp.currentGlobalLightIntensity += 0.1f;
                }
                _sceneService.Value.backgroundAudioSource.clip = globalTimeCmp.changedToRain ? _sceneService.Value.windEmbient : _sceneService.Value.rainEmbient;
                _sceneService.Value.backgroundAudioSource.Play();
                _sceneService.Value.rainEffectContainer.gameObject.SetActive(!globalTimeCmp.changedToRain);
            }

            if (globalTimeCmp.currentDayTime == 24)
                globalTimeCmp.currentDayTime = 0;

            if (globalTimeCmp.currentDayTime == 0 || globalTimeCmp.currentDayTime == 12)
            {
                globalTimeCmp.currentGlobalLightIntensity = 0.45f;
                _sceneService.Value.gloabalLight.color = _sceneService.Value.nightLightColor.Evaluate(0.5f);

                if (globalTimeCmp.currentDayTime == 12)
                {
                    if ((globalTimeCmp.nightLightIntensity >= 0.3f && globalTimeCmp.goToLightNight) || (globalTimeCmp.nightLightIntensity <= 0f && !globalTimeCmp.goToLightNight))
                        globalTimeCmp.goToLightNight = !globalTimeCmp.goToLightNight;

                    if (globalTimeCmp.goToLightNight)
                        globalTimeCmp.nightLightIntensity += 0.1f;
                    else
                        globalTimeCmp.nightLightIntensity -= 0.1f;
                    globalTimeCmp.isNight = true;
                }
                else
                {
                    globalTimeCmp.currentDay++;
                    globalTimeCmp.isNight = false;
                }
                if (!globalTimeCmp.changedToRain)
                    globalTimeCmp.currentGlobalLightIntensity -= 0.1f;
            }
            else if (globalTimeCmp.currentDayTime == 3)
            {
                //day
                _sceneService.Value.gloabalLight.color = _sceneService.Value.nightLightColor.Evaluate(0);
                globalTimeCmp.currentGlobalLightIntensity = 0.75f;
                if (!globalTimeCmp.changedToRain)
                    globalTimeCmp.currentGlobalLightIntensity -= 0.1f;
            }
            else if (globalTimeCmp.currentDayTime == 15)
            {
                //night
                _sceneService.Value.gloabalLight.color = _sceneService.Value.nightLightColor.Evaluate(1f);
                globalTimeCmp.currentGlobalLightIntensity = globalTimeCmp.nightLightIntensity;
                if (!globalTimeCmp.changedToRain)
                    globalTimeCmp.currentGlobalLightIntensity -= 0.1f;
            }

            Debug.Log(globalTimeCmp.currentGlobalLightIntensity + "globalLightint");
            if (globalTimeCmp.currentGlobalLightIntensity < 0.001f)
                globalTimeCmp.currentGlobalLightIntensity = 0;

            Color needColor = _sceneService.Value.nightLightColor.Evaluate(0);
            if (globalTimeCmp.currentDayTime > 12)
                needColor = _sceneService.Value.nightLightColor.Evaluate(1f);
            else if (globalTimeCmp.currentDayTime == 0 || globalTimeCmp.currentDayTime == 12)
                needColor = _sceneService.Value.nightLightColor.Evaluate(0.5f);
            foreach (var houseLight in curLevelView.lightsInHouses)
            {
                houseLight.intensity = globalTimeCmp.currentGlobalLightIntensity * 3;
                houseLight.color = needColor;
            }

            _sceneService.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity;

            _sceneService.Value.startLocationEntry.gameObject.SetActive(curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].hasStartLocationEntry);
            int spawnMainExitPositionIndex = Random.Range(0, curLevelView.exitSpawns.Length);
            if (curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].hasStartLocationEntry)
            {
                int spawnPosStartLocIndex = Random.Range(0, curLevelView.exitSpawns.Length);
                while (spawnPosStartLocIndex == spawnMainExitPositionIndex)
                    spawnPosStartLocIndex = Random.Range(0, curLevelView.exitSpawns.Length);

                _sceneService.Value.startLocationEntry.position = curLevelView.exitSpawns[spawnPosStartLocIndex].position;
            }
            curLevelView.exitTransform.position = curLevelView.exitSpawns[spawnMainExitPositionIndex].position;
            curLevelView.lightsContainer.gameObject.SetActive(globalTimeCmp.isNight);


            Debug.Log((!_globalTimeComponentsPool.Value.Get(_sceneService.Value.playerEntity).isNight) + " enectrycity generator");
            if (_solarPanelElectricGeneratorComponentsPool.Value.Get(_sceneService.Value.playerEntity).currentElectricityEnergy < _sceneService.Value.solarEnergyGeneratorMaxCapacity && !globalTimeCmp.isNight && globalTimeCmp.changedToRain)//mb change to non time and add every round
            {
                ref var electricityGeneratorCmp = ref _solarPanelElectricGeneratorComponentsPool.Value.Get(_sceneService.Value.playerEntity);
                electricityGeneratorCmp.currentElectricityEnergy += _sceneService.Value.solarEnergyGeneratorSpeed;

                if (electricityGeneratorCmp.currentElectricityEnergy > _sceneService.Value.solarEnergyGeneratorMaxCapacity)
                    electricityGeneratorCmp.currentElectricityEnergy = _sceneService.Value.solarEnergyGeneratorMaxCapacity;
                _sceneService.Value.dropedItemsUIView.solarBatteryenergyText.text = (int)electricityGeneratorCmp.currentElectricityEnergy + "mAh/ \n" + _sceneService.Value.solarEnergyGeneratorMaxCapacity + "mAh";
            }
            if (curLocationCmp.levelNum != 0)
                _saveGameEventsPool.Value.Add(_sceneService.Value.playerEntity).type = DataPersistenceManagerSystem.SavePriority.betweenLevelSave;
            else
                _saveGameEventsPool.Value.Add(_sceneService.Value.playerEntity).type = DataPersistenceManagerSystem.SavePriority.startLocationSave;


            ShopCharacterView[] shoppers;
            if (!globalTimeCmp.isNight)
                shoppers = new ShopCharacterView[curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].shoppersCount];
            else
                shoppers = new ShopCharacterView[curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].shoppersCount + 1];
            var remainngShoppers = new List<int>();
            foreach (var needShopper in curLocationCmp.currentLocation.shoppers)
                remainngShoppers.Add(needShopper);
            if (globalTimeCmp.isNight)
                foreach (var needShopper in curLocationCmp.currentLocation.nightShoppers)
                    remainngShoppers.Add(needShopper);

            foreach (var interestObjectView in curLevelView.interestsObjectsViews)
            {
                if (interestObjectView.spawnChance < Random.value)
                    interestObjectView.gameObject.SetActive(false);
                else
                {
                    int itemEntity = _world.Value.NewEntity();
                    if (interestObjectView.objectType == InterestObjectOnLocationView.InterestObjectType.collecting)
                    {


                        ref var droppedItemComponent = ref _droppedItemComponentsPool.Value.Add(itemEntity);

                        droppedItemComponent.currentItemsCount = interestObjectView.dropElements[0].itemsCountMin;

                        droppedItemComponent.itemInfo = interestObjectView.dropElements[0].droopedItem;

                        droppedItemComponent.droppedItemView = interestObjectView.dropItemView;
                        interestObjectView.dropItemView.SetParametersToItem(itemEntity);
                    }

                    else if (interestObjectView.objectType == InterestObjectOnLocationView.InterestObjectType.brocked)
                    {

                        ref var healthCmp = ref _healthComponentsPool.Value.Add(itemEntity);

                        healthCmp.healthView = interestObjectView.gameObject.GetComponent<HealthView>();
                        healthCmp.healthView.Construct(itemEntity);
                        healthCmp.maxHealthPoint = healthCmp.healthView.maxHealth;
                        healthCmp.healthPoint = healthCmp.maxHealthPoint;
                    }
                    //var hidedObjectView = ;
                    Debug.Log("hided obj view name " + interestObjectView.gameObject.name);
                    _hidedObjectOutsideFOVComponentsPool.Value.Add(itemEntity).hidedObjects = new Transform[] { interestObjectView.GetComponent<HidedOutsidePlayerFovView>().objectsToHide[0] };
                    Debug.Log("hided obj view count " + interestObjectView.GetComponent<HidedOutsidePlayerFovView>().objectsToHide.Length);
                    Debug.Log("hided obj count " + _hidedObjectOutsideFOVComponentsPool.Value.Get(itemEntity).hidedObjects.Length);
                }
            }

            var currentInterestsSpawns = new List<Vector2>();

            foreach (var needPos in curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].levelPrefab.interesPointsSpawn)
                currentInterestsSpawns.Add(needPos.position);

            for (int i = 0; i < shoppers.Length; i++)
            {
                int needShopperIndex = Random.Range(0, remainngShoppers.Count);
                int needRandomShopperPositionIndex = Random.Range(0, currentInterestsSpawns.Count);
                var curShopper = _sceneService.Value.interactCharacters[remainngShoppers[needShopperIndex]].transform.GetComponent<ShopCharacterView>();
                _setupShoppersOnNewLocationEventsPool.Value.Add(_sceneService.Value.interactCharacters[remainngShoppers[needShopperIndex]]._entity);
                shoppers[i] = curShopper;
                shoppers[i].gameObject.SetActive(true);
                shoppers[i].gameObject.transform.position = currentInterestsSpawns[needRandomShopperPositionIndex];

                remainngShoppers.RemoveAt(needShopperIndex);
                currentInterestsSpawns.RemoveAt(needRandomShopperPositionIndex);
            }
            _movementComponentsPool.Value.Get(_sceneService.Value.playerEntity).movementView.gameObject.transform.position = curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].levelPrefab.playerSpawns[Random.Range(0, curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].levelPrefab.playerSpawns.Length)].position;

            foreach (var enemySpawn in curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].levelPrefab.enemySpawns)
                curLocationCmp.currentEnemySpawns.Add(enemySpawn.position);

            for (int i = 0; i < curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].enemiesCount; i++)
                _enemySpawnEventsPool.Value.Add(_world.Value.NewEntity());

            curLocationCmp.trapsPrefabs = new Transform[curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].trapsCount];
            for (int i = 0; i < curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].trapsCount; i++)
            {
                int needPositionIndex = Random.Range(0, curLocationCmp.currentEnemySpawns.Count);
                int trapEntity = _world.Value.NewEntity();
                curLocationCmp.trapsPrefabs[i] = _sceneService.Value.InstantiateObject(curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].traps[Random.Range(0, curLocationCmp.currentLocation.levels[curLocationCmp.levelNum].traps.Length)], curLocationCmp.currentEnemySpawns[needPositionIndex]);
                curLocationCmp.trapsPrefabs[i].GetComponent<TrapView>().entity = trapEntity;
                _hidedObjectOutsideFOVComponentsPool.Value.Add(trapEntity).hidedObjects = new Transform[] { curLocationCmp.trapsPrefabs[i].GetChild(0) };
                curLocationCmp.currentEnemySpawns.RemoveAt(needPositionIndex);
            }
        }

        curLocationCmp.levelNum++;
    }
    public void Run(IEcsSystems systems)
    {
        foreach (var textEvent in _slowTextInstanceEventsFilter.Value)
        {
            ref var slowTextEvt = ref _slowTextInstanceEventsPool.Value.Get(textEvent);
            slowTextEvt.currentTime += Time.deltaTime;
            if (slowTextEvt.curChar != (int)(slowTextEvt.currentTime / 0.15f))
            {
                slowTextEvt.curChar = (int)(slowTextEvt.currentTime / 0.15f);
                if (slowTextEvt.curChar <= slowTextEvt.needText.Length)
                {
                    _sceneService.Value.dropedItemsUIView.dialogeText.text = slowTextEvt.needText.Substring(0, slowTextEvt.curChar);
                }
                else if (slowTextEvt.curChar >= slowTextEvt.needText.Length * 2)
                {
                    if (slowTextEvt.needText.Length * 3 <= slowTextEvt.curChar)
                    {
                        _slowTextInstanceEventsPool.Value.Del(textEvent);
                        _sceneService.Value.dropedItemsUIView.dialogeText.text = " ";
                    }
                    else
                        _sceneService.Value.dropedItemsUIView.dialogeText.text = slowTextEvt.needText.Substring(0, slowTextEvt.needText.Length * 3 - slowTextEvt.curChar);
                }
            }
        }
        foreach (var entry in _entryInNewLocationEventsFilter.Value)
        {
            ref var entryEvt = ref _entryInNewLocationEventsPool.Value.Get(entry);
            if (entryEvt.timeSinceEntry == 0)
            {
                Time.timeScale = 0;
                _movementComponentsPool.Value.Get(_sceneService.Value.playerEntity).canMove = false;
                _sceneService.Value.fadeScreenAnimator.SetBool("isEntry", true);
            }
            else if (entryEvt.timeSinceEntry >= 1f && !entryEvt.isFadeScreenEntry)
            {
                entryEvt.isFadeScreenEntry = true;
                ChangeLevelPrefab(entryEvt.location);
                Time.timeScale = 1;
            }
            else if (entryEvt.timeSinceEntry >= 2f && entryEvt.isFadeScreenEntry)
            {
                ref var moveCmp = ref _movementComponentsPool.Value.Get(_sceneService.Value.playerEntity);
                _sceneService.Value.fadeScreenAnimator.SetBool("isEntry", false);
                moveCmp.canMove = true;
                _entryInNewLocationEventsPool.Value.Del(entry);
                var curLocation = _currentLocationComponentsPool.Value.Get(_sceneService.Value.playerEntity);
                if (_slowTextInstanceEventsPool.Value.Has(_sceneService.Value.playerEntity))
                    _slowTextInstanceEventsPool.Value.Del(_sceneService.Value.playerEntity);//мб под гэт переделать вместо удаления
                if (curLocation.levelNum != 0)
                    _slowTextInstanceEventsPool.Value.Add(_sceneService.Value.playerEntity).needText = curLocation.currentLocation.locationName + " level " + curLocation.levelNum;
                else
                    _slowTextInstanceEventsPool.Value.Add(_sceneService.Value.playerEntity).needText = "Start location";
                return;
            }
            entryEvt.timeSinceEntry += Time.unscaledDeltaTime;

        }
        foreach (var load in _loadGameEventsFilter.Value)
        {
            ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(_sceneService.Value.playerEntity);

            _sceneService.Value.rainEffectContainer.gameObject.SetActive(!globalTimeCmp.changedToRain);

            if (globalTimeCmp.currentDayTime == 0 || globalTimeCmp.currentDayTime == 12)
            {
                globalTimeCmp.currentGlobalLightIntensity = 0.45f;
                if (globalTimeCmp.currentDayTime == 12)
                    globalTimeCmp.isNight = true;
                _sceneService.Value.gloabalLight.color = _sceneService.Value.nightLightColor.Evaluate(0.5f);
            }
            else if (globalTimeCmp.currentDayTime < 12)
            {
                //day
                _sceneService.Value.gloabalLight.color = _sceneService.Value.nightLightColor.Evaluate(0);
                globalTimeCmp.currentGlobalLightIntensity = 0.75f;
            }
            else
            {
                //night
                globalTimeCmp.isNight = true;
                _sceneService.Value.gloabalLight.color = _sceneService.Value.nightLightColor.Evaluate(1f);
                globalTimeCmp.currentGlobalLightIntensity = globalTimeCmp.nightLightIntensity;
            }
            if (!globalTimeCmp.changedToRain)
                globalTimeCmp.currentGlobalLightIntensity -= 0.1f;

            if (globalTimeCmp.currentGlobalLightIntensity < 0.001f)
                globalTimeCmp.currentGlobalLightIntensity = 0;
            _sceneService.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity;
            _sceneService.Value.backgroundAudioSource.clip = globalTimeCmp.changedToRain ? _sceneService.Value.windEmbient : _sceneService.Value.rainEmbient;
            _sceneService.Value.startLocationLightsContainer.gameObject.SetActive(globalTimeCmp.isNight);

            _sceneService.Value.backgroundAudioSource.Play();
        }
        /* foreach (var entity in _globalTimeComponentsFilter.Value)
         {
             ref var globalTimeCmp = ref _globalTimeComponentsPool.Value.Get(entity);

             if (globalTimeCmp.currentDayTime > _sceneService.Value.dayTime * 0.7f && !globalTimeCmp.isNight)
             {
                 globalTimeCmp.changeGloabalLightTime = 0;
                 globalTimeCmp.isNight = true;
                 _changeToNightEventsPool.Value.Add(_sceneService.Value.playerEntity);
                 Debug.Log("night");
             }
             else if (globalTimeCmp.currentDayTime > _sceneService.Value.dayTime && globalTimeCmp.isNight)
             {
                 globalTimeCmp.changeGloabalLightTime = 0;
                 globalTimeCmp.isNight = false;
                 _changeToDayEventsPool.Value.Add(entity);
                 globalTimeCmp.currentDay++;
                 globalTimeCmp.currentDayTime = 0;
                 Debug.Log("day");
             }
             if (globalTimeCmp.changeGloabalLightTime <= 45)
             {
                 Debug.Log("changeDay");
                 globalTimeCmp.changeGloabalLightTime += Time.deltaTime;
                 if (globalTimeCmp.isNight)
                 {
                     globalTimeCmp.currentGlobalLightIntensity = Mathf.MoveTowards(globalTimeCmp.currentGlobalLightIntensity, 0.2f, globalTimeCmp.currentGlobalLightIntensity / 30 * Time.deltaTime);
                     _sceneService.Value.gloabalLight.color = _sceneService.Value.nightLightColor.Evaluate(Mathf.Lerp(0, 1, globalTimeCmp.changeGloabalLightTime / 40));
                 }
                 else
                 {

                     globalTimeCmp.currentGlobalLightIntensity = Mathf.MoveTowards(globalTimeCmp.currentGlobalLightIntensity, 0.75f, globalTimeCmp.currentGlobalLightIntensity / 30 * Time.deltaTime);
                     _sceneService.Value.gloabalLight.color = _sceneService.Value.nightLightColor.Evaluate(Mathf.Lerp(1, 0, globalTimeCmp.changeGloabalLightTime / 40));
                 }
                 var buildingCheckCmp = _buildingCheckerComponentsPool.Value.Get(entity);
                 float intansityToRemove = 0;
                 if (buildingCheckCmp.isHideRoof)
                     intansityToRemove = 0.35f * (1 - buildingCheckCmp.timeBeforeHideRoof);
                 else
                     intansityToRemove = 0.35f * buildingCheckCmp.timeBeforeHideRoof;

                 _sceneService.Value.gloabalLight.intensity = globalTimeCmp.currentGlobalLightIntensity - intansityToRemove;
             }
         }*/
    }
}
