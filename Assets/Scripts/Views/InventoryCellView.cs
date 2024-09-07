using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCellView : MonoBehaviour, IPointerEnterHandler
{
    [field: SerializeField] public Image inventoryCellItemImage;
    [field: SerializeField] public TMP_Text inventoryCellItemCountText;
    private int _entity;
    private EcsWorld _world;

    public void Construct(int entity, EcsWorld world)
    {
        _entity = entity;
        _world = world;
    }
    public void ChangeCellItemSprite(Sprite itemSprite)
    {
        inventoryCellItemImage.sprite = itemSprite;
    }

    public void ChangeCellItemCount(int count)
    {
        inventoryCellItemCountText.text = count.ToString();
    }

    public void ClearInventoryCell()
    {
        inventoryCellItemImage.sprite = null;//надо ставить невидимый спрайт
        inventoryCellItemCountText.text = "";
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_world.GetPool<InventoryCellComponent>().Get(_entity).isEmpty)
        {
            var entity = _world.NewEntity();
            var pool = _world.GetPool<SetDescriptionItemEvent>();

            ref var evt = ref pool.Add(entity);
            var itemsPool = _world.GetPool<InventoryItemComponent>();
            evt.itemInfo = itemsPool.Get(_entity).itemInfo;
        }

    }
}
