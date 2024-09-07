using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropedItemsUIView : MonoBehaviour
{
    [SerializeField] public Slider dropSlider;
    [field: SerializeField] public TMP_Text currentItemsCountToDrop { get; private set; }
    [field: SerializeField] public Button dropButton { get; private set; }

    [field: SerializeField] public Transform itemInfoContainer { get; set; }

    private int curCell;

    private EcsWorld _world;

    public int curValue { get; private set; }

    private void Start()
    {
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
        curCell = curCellEntity;
        currentItemsCountToDrop.text = curValue + "/" + dropSlider.maxValue;
    }
    private void OnSliderChange()
    {
        curValue = (int)dropSlider.value;
        currentItemsCountToDrop.text = curValue + "/" + dropSlider.maxValue;
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
