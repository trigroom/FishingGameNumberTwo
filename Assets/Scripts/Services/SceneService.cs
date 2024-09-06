using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneService : MonoBehaviour
{
    [field: SerializeField] public Transform inventoryCellsContainer { get; private set; }
    [field: SerializeField] public GameObject inventoryCell { get; private set; }
    [field: SerializeField] public int cellsCount { get; private set; }
    [field: SerializeField] public TMP_Text statsText { get; private set; }
    [field: SerializeField] public float maxWeight { get; private set; }
    [field: SerializeField] public GameObject playerPrefab { get; private set; }

    //убрать после создания магазина
    [field: SerializeField] public ItemInfo testItem1 { get; private set; }
    [field: SerializeField] public ItemInfo testItem2 { get; private set; }
    public InventoryCellView GetInventoryCell()
    {
       return Instantiate(inventoryCell, inventoryCellsContainer).GetComponent<InventoryCellView>();
    }

    public PlayerView SpawnPlayer()
    {
        return Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<PlayerView>();
    }
}
