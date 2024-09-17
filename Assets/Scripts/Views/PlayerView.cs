using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [field: SerializeField] public HealthView healthView { get; private set; }
    [field: SerializeField] public MovementView movementView { get; private set; }
    [field: SerializeField] public float checkRadius { get; private set; }
    [field: SerializeField] public LayerMask droppedItemsMask { get; private set; }
    [field: SerializeField] public TMP_Text itemInfoText;

    private EcsWorld _world;

    private int currentDroppedItem = -1;


    private void Start()
    {
        InvokeRepeating("CheckNearestDroppedItems", 0.3f, 0.3f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && currentDroppedItem != -1)
        {
            ref var itemCmp = ref _world.GetPool<AddItemEvent>().Add(currentDroppedItem);
        }
    }

    public void Construct(EcsWorld world)
    {
        _world = world;
    }

    private void CheckNearestDroppedItems()
    {
        RaycastHit2D collidedItem = Physics2D.CircleCast(gameObject.transform.position, checkRadius, Vector2.up, checkRadius,droppedItemsMask);

        if (collidedItem.collider != null)
        {
            var droppedItem = collidedItem.collider.gameObject.GetComponent<DroppedItemView>().itemEntity;
            currentDroppedItem = droppedItem;

            ref var itemCmp = ref _world.GetPool<DroppedItemComponent>().Get(droppedItem);

            SetInfoText(itemCmp.itemInfo.itemName + " " + itemCmp.currentItemsCount + " (нажми F чтобы поднять)");
        }

        else
        {
            currentDroppedItem = -1;
            SetInfoText("");
        }
    }
    //проверка ближайших вещей для подбора

    private void SetInfoText(string neededText)
    {
        itemInfoText.text = neededText;
    }

    public Vector2 GetPlayerPosition()
    {
        return gameObject.transform.position;
    }
}
