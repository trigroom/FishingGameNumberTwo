using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using static InteractCharacterView;

public class PlayerView : MonoBehaviour
{
    [field: SerializeField] public HealthView healthView { get; private set; }
    [field: SerializeField] public MovementView movementView { get; private set; }
    [field: SerializeField] public float checkRadius { get; private set; }
    [field: SerializeField] public LayerMask droppedItemsMask { get; private set; }
    [field: SerializeField] public LayerMask interactCharacterMask { get; private set; }
    [field: SerializeField] public TMP_Text itemInfoText;
    [field: SerializeField] public TMP_Text charactersInteractText;

    private EcsWorld _world;

    private int currentDroppedItem = -1;
    private int currentActiveShopper = -1;
    public bool canShoping = true;

    private void Start()
    {
        InvokeRepeating("CheckNearestDroppedItems", 0.3f, 0.3f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentDroppedItem != -1)
            {
                _world.GetPool<AddItemEvent>().Add(currentDroppedItem);
            }
            else if (currentActiveShopper != -1 && canShoping)
            {
                canShoping = false;
                _world.GetPool<ShopOpenEvent>().Add(currentActiveShopper);
            }
        }
    }

    public void Construct(EcsWorld world)
    {
        _world = world;
    }

    private void CheckNearestDroppedItems()
    {
        RaycastHit2D collidedItem = Physics2D.CircleCast(gameObject.transform.position, checkRadius, Vector2.up, checkRadius, droppedItemsMask);

        if (collidedItem.collider != null)
        {
            var droppedItem = collidedItem.collider.gameObject.GetComponent<DroppedItemView>().itemEntity;
            currentDroppedItem = droppedItem;

            ref var itemCmp = ref _world.GetPool<DroppedItemComponent>().Get(droppedItem);

            SetInfoDroppedItemsText(itemCmp.itemInfo.itemName + " " + itemCmp.currentItemsCount + " (нажми F чтобы поднять)");
        }
        else
        {
            currentDroppedItem = -1;
            RaycastHit2D collidedCharacter = Physics2D.CircleCast(gameObject.transform.position, checkRadius, Vector2.up, checkRadius, interactCharacterMask);
            if (collidedCharacter.collider != null)
            {
                var interactCharacter = collidedCharacter.collider.gameObject.GetComponent<InteractCharacterView>();

                if (interactCharacter._characterType == InteractNPCType.shop)
                {
                    currentActiveShopper = interactCharacter._entity;
                    
                    SetInfoDroppedItemsText(" (нажми F чтобы зайти в магазин)");
                }
                else if (interactCharacter._characterType == InteractNPCType.dialogeNpc)
                {
                    SetInfoDroppedItemsText(" (нажми F чтобы поговорить)");
                }

            }
            else
            {
                currentActiveShopper = -1;
                SetInfoDroppedItemsText("");
            }
        }

    }
    //проверка ближайших вещей для подбора


    private void SetInfoDroppedItemsText(string neededText)
    {
        itemInfoText.text = neededText;
    }

    public Vector2 GetPlayerPosition()
    {
        return gameObject.transform.position;
    }
}
