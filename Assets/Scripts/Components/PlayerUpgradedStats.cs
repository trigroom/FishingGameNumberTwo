using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*0.Movement
1.Accuracy
2.Melee damage
3.Max weight*/
public struct PlayerUpgradedStats 
{
    public int[] statLevels { get; set; }
    public float[] currentStatsExp;
    public enum StatType
    {
        stong,
        accuracy,
        stamina
    }

    public Dictionary <int ,GunLevelInfoElement> weaponsExp{  get; set; }
}
