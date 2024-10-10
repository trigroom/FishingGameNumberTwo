using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static InteractCharacterView;

public class PlayerInputView : MonoBehaviour
{
    [field: SerializeField] public float checkRadius { get; private set; }
    [field: SerializeField] public LayerMask droppedItemsMask { get; private set; }
    [field: SerializeField] public LayerMask interactCharacterMask { get; private set; }
    [field: SerializeField] public LayerMask spawnZoneMask { get; private set; }
    [field: SerializeField] public TMP_Text itemInfoText;
    [field: SerializeField] public TMP_Text charactersInteractText;
   // [field: SerializeField] public PolygonCollider2D visionZoneCollider;

    private EcsWorld _world;
    private int _entity;

    private int currentDroppedItem = -1;
    private int currentActiveShopper = -1;
    public bool canShoping = true;
    public bool canUseStorage = true;
    public bool usedInventory = false;
    public bool isColliderInteract = false;

    private void Start()
    {
        InvokeRepeating("CheckNearestDroppedItems", 0.3f, 0.3f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isColliderInteract)
        {
            if (currentDroppedItem != -1)
            {
                _world.GetPool<AddItemEvent>().Add(currentDroppedItem);
            }
            else if (canShoping && !usedInventory && currentActiveShopper != -1)
            {
                canShoping = false;
                _world.GetPool<OffInScopeStateEvent>().Add(currentActiveShopper);
                _world.GetPool<ShopOpenEvent>().Add(currentActiveShopper);
            }
            else if (canUseStorage && !usedInventory)
            {
                canUseStorage = false;
                _world.GetPool<OffInScopeStateEvent>().Add(_world.NewEntity());
                _world.GetPool<StorageOpenEvent>().Add(_entity);
            }
        }
    }

    public void Construct(EcsWorld world, int entity)
    {
        _world = world;
        _entity = entity;
    }

    private void CheckNearestDroppedItems()
    {
        RaycastHit2D collidedItem = Physics2D.CircleCast(gameObject.transform.position, checkRadius, Vector2.up, checkRadius, droppedItemsMask);

        if (collidedItem.collider != null)
        {
            isColliderInteract = true;

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
                    isColliderInteract = true; 
                    currentActiveShopper = interactCharacter._entity;

                    SetInfoDroppedItemsText(" (нажми F чтобы зайти в магазин)");
                }
                else if (interactCharacter._characterType == InteractNPCType.dialogeNpc)
                {
                    isColliderInteract = true;
                    currentActiveShopper = -1;
                    SetInfoDroppedItemsText(" (нажми F чтобы поговорить)");
                }
                else if (interactCharacter._characterType == InteractNPCType.storage)
                {
                    isColliderInteract = true;
                    currentActiveShopper = -1;
                    SetInfoDroppedItemsText(" (нажми F чтобы зайти в хранилище)");
                }

            }
            else
            {
                
                isColliderInteract = false;
                SetInfoDroppedItemsText("");
            }
        }

    }
    //проверка ближайших вещей для подбора

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            Debug.Log("in zone");
            _world.GetPool<EntrySpawnZoneEvent>().Add(_entity).zoneView = collision.gameObject.GetComponent<SpawnZoneView>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            Debug.Log("no zone");
            _world.GetPool<ExitSpawnZoneEvent>().Add(_entity).zoneView = collision.gameObject.GetComponent<SpawnZoneView>();
        }
    }

    private void SetInfoDroppedItemsText(string neededText)
    {
        itemInfoText.text = neededText;
    }

    public Vector2 GetPlayerPosition()
    {
        return gameObject.transform.position;
    }
}
