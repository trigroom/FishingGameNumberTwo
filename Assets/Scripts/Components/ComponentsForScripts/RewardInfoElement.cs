using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class RewardInfoElement 
{
    public int rewardItemsCount;
    public int rewardItemId;//0 для монет, <0 для номера айтема который надо заспавнить
    public Transform spawnedObject;
}
