using UnityEngine;

public struct MenuStatesComponent
{
    public bool inInventoryState;
    public bool inShopState;
    public bool inStorageState;
    public bool inGunWorkshopState;
    public bool inQuestHelperState;
    public MainMenuState mainMenuState;
    public bool inCraftingTableState;
    public RectTransform invCellRectTransform{ get; set; }
    public bool isGunPartDescription;

    public int lastMarkedCell;
    public int lastDraggedCell;
    public int currentGuidePage;
    public int currentMarkedGunPart ;

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

    public enum MainMenuState
    {
        none,
        mainMenu,
        settings
    }

    public bool inMoneyTransportState;
    public bool transportMoneyToInventory;

    public float timeScincePressRestartGameButton;
    public bool restartGameButtonIsPressed;
}
