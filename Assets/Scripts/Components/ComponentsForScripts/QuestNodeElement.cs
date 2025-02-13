using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class QuestNodeElement 
{
    public string[] dialogeText;
   // public bool questIsStarted;

    public string questDescription;
    public string questName;

    public TaskElement[] tasks;

    public RewardInfoElement[] rewards;
    public enum QuestType
    {
        killSomeone,
        bringSomething,
        doingSomething,
        neutralizeTrap
    }
}
