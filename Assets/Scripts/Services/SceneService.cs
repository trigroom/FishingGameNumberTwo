using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class SceneService : MonoBehaviour
{
    [field: SerializeField] public DropedItemsUIView dropedItemsUIView { get; private set; }
    [field: SerializeField] public Transform inventoryCellsContainer { get; private set; }
    [field: SerializeField] public GameObject inventoryCell { get; private set; }
    [field: SerializeField] public int cellsCount { get; private set; }
    [field: SerializeField] public TMP_Text statsText { get; private set; }
    [field: SerializeField] public TMP_Text currentItemText { get; private set; }
    [field: SerializeField] public float maxWeight { get; private set; }
    [field: SerializeField] public GameObject playerPrefab { get; private set; }
    [field: SerializeField] public GameObject droppedItemPrefab { get; private set; }

    [field: SerializeField] public TMP_Text hoverDescriptionText;

    //убрать после создания магазина
    [field: SerializeField] public ItemInfo testItem1 { get; private set; }
    [field: SerializeField] public ItemInfo testItem2 { get; private set; }

    [field: SerializeField] public int playerEntity { get; private set; }

    public InventoryCellView GetInventoryCell(int entity, EcsWorld world)
    {
        var invCell = Instantiate(inventoryCell, inventoryCellsContainer).GetComponent<InventoryCellView>();
        invCell.Construct(entity, world);
        return invCell;
    }

    public PlayerView SpawnPlayer(EcsWorld ecsWorld, int entity)
    {
        var player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<PlayerView>();
        player.Construct(ecsWorld);
        player.itemInfoText = currentItemText;
        playerEntity = entity;
        return player;
    }

    public DroppedItemView SpawnDroppedItem(Vector2 spawnPoint, ItemInfo itemInfo, int entity)
    {
        var droppedItemObj = Instantiate(droppedItemPrefab, spawnPoint, Quaternion.identity).GetComponent<DroppedItemView>();
        droppedItemObj.SetParametersToItem(itemInfo.itemSprite, itemInfo.itemName, entity);

        return droppedItemObj;
    }
}
