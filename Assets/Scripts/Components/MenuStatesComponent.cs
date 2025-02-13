using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MenuStatesComponent 
{
    public bool inInventoryState;
    public bool inShopState;
    public bool inStorageState;
    public bool inGunWorkshopState;
    public bool inQuestHelperState;
    public bool inMainMenuState;
    public bool inCraftingTableState;

    public int lastMarkedCell;
    public int currentGuidePage;

    public CurrentItemShowedInfoState currentItemShowedInfo;
    public CurrentBookShowState currentBookShowState;
    public enum CurrentItemShowedInfoState
    {
        playerStats,
        itemDescription,
        itemInfo
    }

    public enum CurrentBookShowState
    {
        quests,
        guides
    }
    public bool inMoneyTransportState;
    public bool transportMoneyToInventory;

    public float timeScincePressRestartGameButton;
    public bool restartGameButtonIsPressed;
}
