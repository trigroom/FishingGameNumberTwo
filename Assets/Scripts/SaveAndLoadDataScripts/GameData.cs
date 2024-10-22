using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameData
{
    [Header("Game currency")]
    public int money;
    [Header("Inventory cells")]
    public ItemInfoForSaveData[] itemsCellinfo;
    public NumAndIdForSafeData[] durabilityItemsForSaveData;
    public NumAndIdForSafeData[] bulletsWeaponForSaveData;
    [Header("Play time")]
    public int currentDay;
    public float currentDayTime;
    [Header("Settings")]
    public int maxFPS;

    public GameData()
    {
        money = 100;
        maxFPS = 60;
    }
}
