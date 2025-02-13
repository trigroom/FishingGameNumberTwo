using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestNodeElement;
[System.Serializable]
public class TaskElement
{
    public QuestType questType;

    public Transform spawnedObject;
    
    public int neededCount;
    public int neededId;

    public KillTaskElement killTaskInfo;
}
