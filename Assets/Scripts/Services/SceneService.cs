using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class SceneService : MonoBehaviour
{
    [field: SerializeField] public UIMenuView inventoryMenuView { get; private set; }
    [field: SerializeField] public DropedItemsUIView dropedItemsUIView { get; private set; }
    [field: SerializeField] public Transform inventoryCellsContainer { get; private set; }
    [field: SerializeField] public Transform shopCellsContainer { get; private set; }
    [field: SerializeField] public GameObject inventoryCell { get; private set; }
    [field: SerializeField] public int cellsCount { get; private set; }
    [field: SerializeField] public TMP_Text moneyText { get; private set; }
    [field: SerializeField] public TMP_Text statsText { get; private set; }
    [field: SerializeField] public TMP_Text currentItemText { get; private set; }
    [field: SerializeField] public float maxWeight { get; private set; }
    [field: SerializeField] public GameObject playerPrefab { get; private set; }
    [field: SerializeField] public GameObject droppedItemPrefab { get; private set; }
    [field: SerializeField] public GameObject shopCellPrefab { get; private set; }

    [field: SerializeField] public TMP_Text hoverDescriptionText;
    [field: SerializeField] public TMP_Text ammoInfoText;
    [field: SerializeField] public Camera mainCamera{ get; private set; }
    [field: SerializeField] public LineRenderer bulletTracer { get; private set; }
    [field: SerializeField] public int playerEntity { get; private set; }
    private ObjectPool<LineRenderer> _bulletTracersPool;

    //всё для тестов\/
    [field: SerializeField] public ShopItemInfo[] testShopItems { get; private set; }
    [field: SerializeField] public ItemInfo testItem1 { get; private set; }
    [field: SerializeField] public ItemInfo testItem2 { get; private set; }
    [field: SerializeField] public GunInfo firstWeaponTest { get; private set; }
    [field: SerializeField] public GunInfo secondWeaponTest { get; private set; }
    [field: SerializeField] public int startMoneyForTest { get; private set; }
    [field: SerializeField] public GameObject testEnemy { get; private set; }

    private void Awake()
    {
        _bulletTracersPool = new ObjectPool<LineRenderer>(() => Instantiate(bulletTracer));
        mainCamera = Camera.main;
    }

    public LineRenderer GetBulletTracer()
    {
        Debug.Log("Spawntracer");
        var view = _bulletTracersPool.Get();
        view.gameObject.SetActive(true);
        return view;
    }

    public void ReleaseBulletTracer(LineRenderer renderer)
    {
        renderer.gameObject.SetActive(false);
        _bulletTracersPool.Release(renderer);
    }
    public InventoryCellView GetInventoryCell(int entity, EcsWorld world)
    {
        var invCell = Instantiate(inventoryCell, inventoryCellsContainer).GetComponent<InventoryCellView>();
        invCell.Construct(entity, world);
        return invCell;
    }

    public ShopCellView GetShopCell(int entity, EcsWorld world)
    {
        var shopCell = Instantiate(shopCellPrefab, shopCellsContainer).GetComponent<ShopCellView>();
        shopCell.Construct(entity, world);
        return shopCell;
    }
    public PlayerView SpawnPlayer(EcsWorld ecsWorld, int entity)
    {
        var player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<PlayerView>();
        player.Construct(ecsWorld);
        player.itemInfoText = currentItemText;
        playerEntity = entity;
        return player;
    }

    public HealthView GetEnemy()
    {
        return Instantiate(testEnemy, Vector2.zero, Quaternion.identity).GetComponent<HealthView>();
    }
    public DroppedItemView SpawnDroppedItem(Vector2 spawnPoint, ItemInfo itemInfo, int entity)
    {
        var droppedItemObj = Instantiate(droppedItemPrefab, spawnPoint, Quaternion.identity).GetComponent<DroppedItemView>();
        droppedItemObj.SetParametersToItem(itemInfo.itemSprite, entity);

        return droppedItemObj;
    }
}
