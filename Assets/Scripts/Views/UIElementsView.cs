using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementsView : MonoBehaviour
{
    [Header("General Item Info UI")]
    [field: SerializeField] public TMP_Text shopperRepText { get; private set; }
    [field: SerializeField] public Animator masterkeyAnimator { get; set; }
    [field: SerializeField] public Transform masterkeyMinigameContainer { get; set; }
    [field: SerializeField] public Transform lockerCellsContainer { get; set; }
    [field: SerializeField] public LockCellView[] lockerCellViews { get; set; }
    [field: SerializeField] public Image guideImage { get; private set; }
    [field: SerializeField] public BookmarkView[] bookmarkViews { get; set; }
    [field: SerializeField] public Transform divideItemsUI { get; set; }
    [field: SerializeField] public Slider generalSlider { get; set; }
    [field: SerializeField] public TMP_Text currentItemsCountToDrop { get; private set; }
    [field: SerializeField] public Transform itemInfoContainer { get; set; }
    [field: SerializeField] public Button dropButton { get; private set; }
    [field: SerializeField] public Button divideButton { get; private set; }
    [field: SerializeField] public TMP_Text itemDescriptionText { get; private set; }
    [field: SerializeField] public TMP_Text secondButtonActionText { get; private set; }
    [field: SerializeField] public Image markInventoryCellBorder { get; private set; }
    [field: SerializeField] public Sprite transportStorageIcon { get; private set; }
    [field: SerializeField] public Sprite transportInventoryIcon { get; private set; }
    [field: SerializeField] public Transform itemUIVerticalLayout { get; set; }
    [field: SerializeField] public Transform playerStatsContainer { get; set; }
    [field: SerializeField] public Button showPlayerStatsButton { get; private set; }
    [field: SerializeField] public Button showItemDescriptionButton { get; private set; }
    [field: SerializeField] public Button showItemInfoButton { get; private set; }
    [field: SerializeField] public Image inventoryBackground { get; private set; }
    [field: SerializeField] public BackpackInfo startBackpackInfo { get; private set; }
    [field: SerializeField] public TMP_Text itemInfoText;
    [field: SerializeField] public TMP_Text charactersInteractText;

    [field: SerializeField] public Image firstMoneyButtonImage { get; private set; }
    [field: SerializeField] public Image secondMoneyButtonImage { get; private set; }
    [field: SerializeField] public Button firstMoneyButton { get; private set; }
    [field: SerializeField] public Button secondMoneyButton { get; private set; }
    [field: SerializeField] public Sprite getMoneyIcon { get; private set; }
    [field: SerializeField] public Sprite takeMoneyIcon { get; private set; }
    [field: SerializeField] public Sprite agreeIcon { get; private set; }
    [field: SerializeField] public Sprite exitIcon { get; private set; }
    [field: SerializeField] public Slider moneySlider { get; set; }
    [field: SerializeField] public Transform moneyTransportContainer { get; set; }
    [field: SerializeField] public TMP_Text moneyTransportText;

    [Header("Heal UI")]
    [field: SerializeField] public Button healUseButton { get; private set; }
    [Header("Shop UI")]
    [field: SerializeField] public Image shopTableImage { get; private set; }
    [field: SerializeField] public Sprite sellItemInventoryIcon { get; private set; }
    [field: SerializeField] public TMP_Text shopperMoneyToBuy { get; private set; }
    [field: SerializeField] public Button shopNextPageButton { get; private set; }
    [field: SerializeField] public Button shopLastPageButton { get; private set; }
    [Header("Weapon UI")]
    [field: SerializeField] public Image scopeCrossCentreImage { get; private set; }
    [field: SerializeField] public Image scopeBlackBGImage { get; private set; }
    [field: SerializeField] public Button weaponEquipButton { get; private set; }
    [field: SerializeField] public TMP_Text currentWeaponButtonActionText { get; private set; }
    [field: SerializeField] public Image gunMagazineUI { get; private set; }
    [field: SerializeField] public Transform gunPartsCellsContainer { get; private set; }
    [field: SerializeField] public GunPartCellView[] gunPartCells { get; private set; }
    [field: SerializeField] public Image weaponLevelExpBar { get; private set; }
    [field: SerializeField] public TMP_Text weaponLevelExpText { get; private set; }
    [field: SerializeField] public Transform weaponLevelExpContainer { get; private set; }


    [Header("Storage UI")]
    //  [field: SerializeField] public Slider storageTransportSlider { get; private set; }
    [field: SerializeField] public Button storageButton { get; private set; }
    [field: SerializeField] public Image storageButtonImage { get; private set; }
    // [field: SerializeField] public TMP_Text storageTransportCountText { get; private set; }
    [field: SerializeField] public Transform storageUIContainer { get; set; }
    [field: SerializeField] public TMP_Text solarBatteryenergyText { get; private set; }
    [field: SerializeField] public TMP_Text storageMoneyCountText { get; private set; }

    [Header("Dialoge UI")]
    [field: SerializeField] public TMP_Text characterNameText { get; private set; }
    [field: SerializeField] public TMP_Text dialogeText { get; private set; }
    [field: SerializeField] public Button questDescriptionNextButton { get; private set; }
    [field: SerializeField] public Button questDescriptionLastButton { get; private set; }

    [Header("Gunsmith UI")]
    [field: SerializeField] public TMP_Text upgradedGunText { get; private set; }
    [field: SerializeField] public Image currentGunImage { get; private set; }
    [field: SerializeField] public Image upgradedGunImage { get; private set; }
    [Header("Crafting table UI")]
    [field: SerializeField] public TMP_Text craftedItemRecipeText { get; private set; }
    [field: SerializeField] public Button craftItemButton { get; private set; }
    [field: SerializeField] public CraftCellView[] craftCells { get; private set; }

    [Header("Player stats UI")]
    [field: SerializeField] public Image[] statsFilledBarsImages { get; private set; }
    [field: SerializeField] public TMP_Text[] statsFilledBarsText { get; private set; }
    [field: SerializeField] public TMP_Text statsDescriptionText { get; private set; }
    [field: SerializeField] public Image crackedGlassHelmetUI { get; private set; }
    [field: SerializeField] public Sprite[] crackedGlassSprites { get; private set; }
    [Header("Main Menu UI")]
    [field: SerializeField] public TMP_Text restartGameButtonText { get; private set; }
    [field: SerializeField] public TMP_Text exitGameButtonText { get; private set; }
    [field: SerializeField] public Button restartGameButton { get; private set; }
    [field: SerializeField] public Button thirdActionButton { get; private set; }
    [field: SerializeField] public Transform settingsContainer { get; set; }
    [field: SerializeField] public Button closeSettingsButton { get; set; }
    [field: SerializeField] public Slider uiScalerSlider { get; set; }

    public int curCell { get; private set; }

    private EcsWorld _world;

    public int curInventorySliderValue { get; private set; }

    private void Start()
    {
        curInventorySliderValue = 1;
        generalSlider.minValue = 1;
        weaponEquipButton.onClick.AddListener(EquipSomething);

        healUseButton.onClick.AddListener(UseHealItem);
        dropButton.onClick.AddListener(DropItems);
        thirdActionButton.onClick.AddListener(ExitFromGame);
        divideButton.onClick.AddListener(DivideItems);
        storageButton.onClick.AddListener(TransportItemsBetweenInventoryAndStorage);
        generalSlider.onValueChanged.AddListener(delegate { OnInventorySliderChange(); });
        itemInfoContainer.gameObject.SetActive(false);
        storageUIContainer.gameObject.SetActive(false);
    }
    public void Construct(EcsWorld world)
    {
        _world = world;
    }
    private void ExitFromGame()
    {
        Application.Quit();
    }

    public void SetSliderParametrs(int maxValue, int curCellEntity)
    {
        if (curCell != curCellEntity)
            generalSlider.value = 1;
        curCell = curCellEntity;
        if (maxValue <= 1)
        {
            generalSlider.gameObject.SetActive(false);
            currentItemsCountToDrop.gameObject.SetActive(false);
            return;
        }
        else
        {
            generalSlider.gameObject.SetActive(true);
            currentItemsCountToDrop.gameObject.SetActive(true);
        }
        generalSlider.maxValue = maxValue;
        currentItemsCountToDrop.text = curInventorySliderValue + "/" + generalSlider.maxValue;
    }

    private void OnInventorySliderChange()
    {
        curInventorySliderValue = (int)generalSlider.value;
        currentItemsCountToDrop.text = curInventorySliderValue + "/" + generalSlider.maxValue;
    }

    public void ChangeActiveStateEquipButton(bool isActive)
    {
        weaponEquipButton.gameObject.SetActive(isActive);
    }

    public void ChangeActiveStateIsUseButton(bool isActive)
    {
        healUseButton.gameObject.SetActive(isActive);
    }
    public void EquipSomething() => _world.GetPool<MoveSpecialItemToInventoryEvent>().Add(curCell);
    public void DropItems()
    {
        _world.GetPool<DropItemsIvent>().Add(curCell);
    }

    public void DivideItems()
    {
        if (curInventorySliderValue == 0) return;
        _world.GetPool<DivideItemEvent>().Add(curCell);
        Debug.Log("divideItems");
    }

    public void UseHealItem()
    {
        _world.GetPool<HealFromInventoryEvent>().Add(curCell);
    }

    public void TransportItemsBetweenInventoryAndStorage()
    {
        _world.GetPool<AddItemFromCellEvent>().Add(curCell);
    }
}
