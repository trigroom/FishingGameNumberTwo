using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct QuestInfoForSafeData 
{
    public int questNPCId;
    public int currentQuest;
    public int[] collectedItems;

    public QuestInfoForSafeData(int _questNPCId, int _currentQuest, int[] _collectedItems)
    {
        questNPCId = _questNPCId;
        currentQuest = _currentQuest;
        collectedItems = _collectedItems;
    }
}
