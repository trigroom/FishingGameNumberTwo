using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropedItemsUIView : MonoBehaviour
{
    [field: SerializeField] public Transform dropItemsUI { get; set; }
    [SerializeField] public Slider dropSlider;
    [field: SerializeField] public TMP_Text currentItemsCountToDrop { get; private set; }
    [field: SerializeField] public TMP_Text currentWeaponButtonActionText { get; private set; }
    [field: SerializeField] public Button dropButton { get; private set; }

    [field: SerializeField] public Button weaponEquipButton { get; private set; }

    [field: SerializeField] public Transform itemInfoContainer { get; set; }
    public bool isEquipWeapon;

    private int curCell;

    private EcsWorld _world;

    public int curValue { get; private set; }

    private void Start()
    {
        weaponEquipButton.onClick.AddListener(EquipWeapon);
        dropButton.onClick.AddListener(DropItems);
        dropSlider.onValueChanged.AddListener(delegate { OnSliderChange(); });
        itemInfoContainer.gameObject.SetActive(false);
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
        currentItemsCountToDrop.text = curValue + "/" + dropSlider.maxValue;
    }
    private void OnSliderChange()
    {
        curValue = (int)dropSlider.value;
        currentItemsCountToDrop.text = curValue + "/" + dropSlider.maxValue;
    }

    public void ChangeActiveStateWeaponEquipButton(bool isActive)
    {
        weaponEquipButton.gameObject.SetActive(isActive);
    }

    public void EquipWeapon()
    {
        _world.GetPool<MoveWeaponToInventoryEvent>().Add(curCell);
    }
    public void DropItems()
    {
        if (curValue == 0)
            return;

        _world.GetPool<DropItemsIvent>().Add(curCell).itemsCountToDrop = curValue;


        if (curValue == dropSlider.maxValue)
        {
            itemInfoContainer.gameObject.SetActive(false);
        }
        dropSlider.value = 0;
        //закрывать описание предмета
    }
}
