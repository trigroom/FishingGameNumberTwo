using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCellView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [field: SerializeField] public Image inventoryCellItemImage;
    [field: SerializeField] public TMP_Text inventoryCellItemCountText;
    [field: SerializeField] private Button inventoryCellButton;
    [field: SerializeField] public Animator inventoryCellAnimator;
    [field: SerializeField] public RectTransform invCellRectTransform;
    [field: SerializeField] public Canvas mainCanvas;
    [field: SerializeField] public CanvasGroup cellCanvasGroup;

    public int _entity;//{  get; private set; }
    private EcsWorld _world;

    private void Awake()
    {
        inventoryCellButton.onClick.AddListener(OnClickOpenInfo);
    }
    public void Construct(int entity, EcsWorld world, Canvas canvas)
    {
        mainCanvas = canvas;
        _entity = entity;
        _world = world;
        this.transform.parent.gameObject.transform.localScale = Vector3.one;
    }
    public void ChangeCellItemSprite(Sprite itemSprite)
    {
        inventoryCellButton.interactable = true;
        inventoryCellItemImage.sprite = itemSprite;
        inventoryCellItemImage.color = new Color(1, 1, 1, 1);
    }

    public void ChangeCellItemCount(int count)
    {
        Debug.Log("cell count changed to " + count);
        if (count != 1)
            inventoryCellItemCountText.text = count.ToString();
        else
            inventoryCellItemCountText.text = "";
    }
    public void ClearInventoryCell()
    {
        inventoryCellButton.interactable = false;
        inventoryCellItemImage.color = new Color(1, 1, 1, 0);//надо ставить невидимый спрайт
        inventoryCellItemCountText.text = "";
    }


    public void OnClickOpenInfo()
    {
        if (!_world.GetPool<InventoryCellComponent>().Get(_entity).isEmpty)
        {
            var pool = _world.GetPool<SetDescriptionItemEvent>();

            ref var evt = ref pool.Add(_entity);
            evt.itemEntity = _entity;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventoryCellButton.interactable)
            inventoryCellAnimator.SetBool("mouseOnUI", true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventoryCellButton.interactable)
            inventoryCellAnimator.SetBool("mouseOnUI", false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!inventoryCellButton.interactable) return;

        invCellRectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor * 1.438f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!inventoryCellButton.interactable) return;

        _world.GetPool<StartDragItemEvent>().Add(_entity);
        cellCanvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!inventoryCellButton.interactable) return;

        cellCanvasGroup.blocksRaycasts = true;

        invCellRectTransform.anchoredPosition = Vector2.zero;
        inventoryCellAnimator.SetBool("buttonIsActive", false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        _world.GetPool<EndItemDragEvent>().Add(_entity);
        Debug.Log("item drop in cell" + _entity);
    }
}
