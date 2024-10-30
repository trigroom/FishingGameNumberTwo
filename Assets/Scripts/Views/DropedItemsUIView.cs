using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropedItemsUIView : MonoBehaviour
{
    [Header("General Item Info UI")]
    [field: SerializeField] public Transform dropItemsUI { get; set; }
    [SerializeField] public Slider dropSlider;
    [field: SerializeField] public TMP_Text currentItemsCountToDrop { get; private set; }
    [field: SerializeField] public Transform itemInfoContainer { get; set; }
    [field: SerializeField] public Button dropButton { get; private set; }
    [field: SerializeField] public TMP_Text itemDescriptionText { get; private set; }
    [field: SerializeField] public TMP_Text secondButtonActionText { get; private set; }
    //добавить картинку пересылки из инвентаря
    [field: SerializeField] public Sprite transportStorageIcon { get; private set; }
    [field: SerializeField] public Sprite transportInventoryIcon { get; private set; }
    [Header("Heal UI")]
    [field: SerializeField] public Button healUseButton { get; private set; }
    [Header("Shop UI")]
    [field: SerializeField] public Image shopTableImage { get; private set; }
    [Header("Weapon UI")]
    [field: SerializeField] public Button weaponEquipButton { get; private set; }
    [field: SerializeField] public TMP_Text currentWeaponButtonActionText { get; private set; }

    [Header("Storage UI")]
    [field: SerializeField] public Slider storageTransportSlider { get; private set; }
    [field: SerializeField] public Button storageButton { get; private set; }
    [field: SerializeField] public Image storageButtonImage { get; private set; }
    [field: SerializeField] public TMP_Text storageTransportCountText { get; private set; }
    [field: SerializeField] public Transform storageUIContainer { get; set; }
    [field: SerializeField] public TMP_Text solarBatteryenergyText { get; private set; }

    //[field: SerializeField] public bool isEquipWeapon { get; set; }

    private int curCell;

    private EcsWorld _world;

    public int curInventorySliderValue { get; private set; }
    public int curStorageSliderValue { get; private set; }


    private void Start()
    {
        weaponEquipButton.onClick.AddListener(EquipSomething);
        healUseButton.onClick.AddListener(UseHealItem);
        dropButton.onClick.AddListener(DropItems);
        storageButton.onClick.AddListener(TransportItemsBetweenInventoryAndStorage);
        dropSlider.onValueChanged.AddListener(delegate { OnInventorySliderChange(); });
        storageTransportSlider.onValueChanged.AddListener(delegate { OnStorageSliderChange(); });
        itemInfoContainer.gameObject.SetActive(false);
        storageUIContainer.gameObject.SetActive(false);
    }
    public void Construct(EcsWorld world)
    {
        _world = world;
    }

    public void SetSliderParametrs(int maxValue, int curCellEntity)
    {
        dropSlider.maxValue = maxValue;
        if (curCell != curCellEntity)
            dropSlider.value = 0;
        curCell = curCellEntity;
        currentItemsCountToDrop.text = curInventorySliderValue + "/" + dropSlider.maxValue;

        if (storageUIContainer.gameObject.activeSelf)
        {
            storageTransportSlider.maxValue = dropSlider.maxValue;
            if (curCell != curCellEntity)
                storageTransportSlider.value = 0;

            storageTransportCountText.text = curStorageSliderValue + "/" + dropSlider.maxValue;

        }
    }

    private void OnInventorySliderChange()
    {
        curInventorySliderValue = (int)dropSlider.value;
        currentItemsCountToDrop.text = curInventorySliderValue + "/" + dropSlider.maxValue;
    }

    private void OnStorageSliderChange()
    {
        curStorageSliderValue = (int)storageTransportSlider.value;
        storageTransportCountText.text = curStorageSliderValue + "/" + storageTransportSlider.maxValue;
    }

    public void ChangeActiveStateEquipButton(bool isActive)
    {
        weaponEquipButton.gameObject.SetActive(isActive);
    }

    public void ChangeActiveStateIsUseButton(bool isActive)
    {
        healUseButton.gameObject.SetActive(isActive);
    }
    public void EquipSomething() =>_world.GetPool<MoveSpecialItemToInventoryEvent>().Add(curCell);
    public void DropItems()
    {
        if (curInventorySliderValue == 0)
            return;

        _world.GetPool<DropItemsIvent>().Add(curCell).itemsCountToDrop = curInventorySliderValue;


        if (curInventorySliderValue == dropSlider.maxValue)
        {
            itemInfoContainer.gameObject.SetActive(false);
        }
        dropSlider.value = 0;
        //закрывать описание предмета
    }

    public void UseHealItem()
    {
        _world.GetPool<HealFromInventoryEvent>().Add(curCell);
    }

    public void TransportItemsBetweenInventoryAndStorage()
    {
        if (curStorageSliderValue == 0)
            return;

        _world.GetPool<AddItemFromCellEvent>().Add(curCell).addedItemCount = curStorageSliderValue;


        if (curStorageSliderValue == storageTransportSlider.maxValue)
        {
            itemInfoContainer.gameObject.SetActive(false);
        }
        else
        {
            OnInventorySliderChange();
            OnStorageSliderChange();
        }
        dropSlider.value = 0;
        storageTransportSlider.value = 0;
    }
}
