using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CurrentDialogeComponent 
{
    public int currentDialogeNumber;
    public bool dialogeIsStarted{ get;set; }
    public int npcId;
    public int currentPageNumber;
}
