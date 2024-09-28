using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HealingItemInfo", menuName = "ScriptableObjects/HealingItemInfo", order = 2)]

public class HealingItemInfo : ScriptableObject
{
    public int healingHealthPoints;
    public int healingTime;

}
