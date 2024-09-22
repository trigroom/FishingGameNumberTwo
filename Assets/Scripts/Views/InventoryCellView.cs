using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCellView : MonoBehaviour
{
    [field: SerializeField] public Image inventoryCellItemImage;
    [field: SerializeField] public TMP_Text inventoryCellItemCountText;

    public int _entity {  get; private set; }
    private EcsWorld _world;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnClickOpenInfo);
    }
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
        if (count != 1)
            inventoryCellItemCountText.text = count.ToString();
        else
            inventoryCellItemCountText.text = "";
    }
    public void ClearInventoryCell()
    {
        inventoryCellItemImage.sprite = null;//надо ставить невидимый спрайт
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
}
