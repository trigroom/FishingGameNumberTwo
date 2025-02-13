using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ExpAddedItemInfo", menuName = "ScriptableObjects/ExpAddedItemInfo", order = 2)]
public class ExpAddedItemInfo : ScriptableObject
{
    public int statId;
    public int addedExpCount;
}
