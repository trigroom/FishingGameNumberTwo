using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameData
{
    public string lastGameVersion;
    [Header("Game currency")]
    public int moneyInInventory;
    public int moneyInStorage;
    public float generatorElectricity;
    public int playerHP;
    public int playerHunger;
    [Header("Inventory cells")]
    public ItemInfoForSaveData[] itemsCellinfo;
    public NumAndIdForSafeData[] durabilityItemsForSaveData;
    public NumArrayAndIdForSaveData[] bulletsWeaponForSaveData;
    public NumAndIdForSafeData[] laserPoinerRemainingTimeForSaveData;
    public NumAndIdForSafeData[] weaponsCurrentExpForSaveData;

    public NumAndIdForSafeData[] buttGunPartsForSaveData;
    public NumAndIdForSafeData[] scopeGunPartsForSaveData;
    public NumAndIdForSafeData[] downGunPartsForSaveData;
    public NumAndIdForSafeData[] barrelGunPartsForSaveData;
    public int invCellsCount;
    [Header("Play time")]
    public int currentDay;
    public int currentDayTime;
    public float currentNightLightIntensity;
    public bool goToLightNight;
    public int roundsToWeaterChange;
    public GlobalTimeComponent.WeatherType weaterType;
    public int currentMaxLocationNum;
    [Header("Settings")]
    public int maxFPS;
    public float currentUIScaleMultiplayer;
    [Header("Quests")]
    public QuestInfoForSafeData[] questsInfoForSafeData; 
    [Header("Shoppers")]
    public QuestInfoForSafeData[] shoppersInfoForSafeData{ get; set; }
    [Header("Player stats")]
    public float[] currentStatsExp;
    public int craftingTableLevel;

    public GameData()
    {
        shoppersInfoForSafeData = new QuestInfoForSafeData[0];
        currentStatsExp = new float[3];
        moneyInInventory = 100;
        maxFPS = 60;
        currentUIScaleMultiplayer = 0.7f;
    }
}
