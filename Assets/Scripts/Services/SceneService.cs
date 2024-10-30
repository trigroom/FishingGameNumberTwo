using Leopotam.EcsLite;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SceneService : MonoBehaviour
{
    [field: SerializeField] public ItemsWithIdListInfo idItemslist { get; private set; }
    [field: SerializeField] public Sprite[] scopeSpriteMasks { get; private set; }//x1,x2,x8
    [field: SerializeField] public int timeHourOffset { get; private set; }
    [field: SerializeField] public float dayTime { get; private set; }
    [field: SerializeField] public Gradient nightLightColor { get; private set; }
  //  [field: SerializeField] public Color dayLightColor { get; private set; }
    [field: SerializeField] public PostProcessVolume postProcessingCamera { get; private set; }
    [field: SerializeField] public InventoryCellView flashlightItemCellView { get; private set; }
    [field: SerializeField] public InventoryCellView healingItemCellView { get; private set; }
    [field: SerializeField] public InventoryCellView firstGunCellView { get; private set; }
    [field: SerializeField] public InventoryCellView secondGunCellView { get; private set; }
    [field: SerializeField] public InventoryCellView meleeWeaponCellView { get; private set; }
    [field: SerializeField] public TMP_Text playerStaminaText { get; private set; }
    [field: SerializeField] public Image playerStaminaBarFilled { get; private set; }
    [field: SerializeField] public TMP_Text playerArmorText { get; private set; }
    [field: SerializeField] public Image playerArmorBarFilled { get; private set; }
    [field: SerializeField] public TMP_Text playerHealthText { get; private set; }
    [field: SerializeField] public Image playerHealthBarFilled { get; private set; }
    [field: SerializeField] public UIMenuView storageMenuView { get; private set; }
    [field: SerializeField] public UIMenuView shopMenuView { get; private set; }
    [field: SerializeField] public UIMenuView inventoryMenuView { get; private set; }
    [field: SerializeField] public DropedItemsUIView dropedItemsUIView { get; private set; }
    [field: SerializeField] public Transform inventoryCellsContainer { get; private set; }
    [field: SerializeField] public Transform storageCellsContainer { get; private set; }
    [field: SerializeField] public Transform shopCellsContainer { get; private set; }
    [field: SerializeField] public GameObject inventoryCell { get; private set; }
    [field: SerializeField] public int inventoryCellsCount { get; private set; }
    [field: SerializeField] public int storageCellsCount { get; private set; }
    [field: SerializeField] public TMP_Text moneyText { get; private set; }
    [field: SerializeField] public TMP_Text statsInventoryText { get; private set; }
    [field: SerializeField] public TMP_Text statsStorageText { get; private set; }
    [field: SerializeField] public TMP_Text currentItemText { get; private set; }
    [field: SerializeField] public float maxInInventoryWeight { get; private set; }
    [field: SerializeField] public float maxInStorageWeight { get; private set; }
    [field: SerializeField] public GameObject playerPrefab { get; private set; }
    [field: SerializeField] public GameObject droppedItemPrefab { get; private set; }
    [field: SerializeField] public ShopCellView shopCellPrefab { get; private set; }
    [field: SerializeField] public float solarEnergyGeneratorSpeed { get; private set; }
    [field: SerializeField] public float solarEnergyGeneratorMaxCapacity { get; private set; }

    [field: SerializeField] public TMP_Text ammoInfoText;
    [field: SerializeField] public Light2D gloabalLight { get; set; }
    [field: SerializeField] public Camera mainCamera { get; private set; }
    [field: SerializeField] public LineRenderer bulletTracer { get; private set; }
    [field: SerializeField] public int playerEntity { get; private set; }
    //[field: SerializeField] public int cameraMoveSpeed { get; private set; }
    [field: SerializeField] public ShopCharacterView[] shoppers { get; private set; }
    private ObjectPool<LineRenderer> _bulletTracersPool;
    private ObjectPool<ShopCellView> _shopCellsPool;

    public int storageEntity;
    public int inventoryEntity;

    //всё для тестов\/
    [field: SerializeField] public ItemInfo meleeWeaponItemInfoStarted { get; private set; }
    [field: SerializeField] public ItemInfo gunItemInfoStarted { get; private set; }
    [field: SerializeField] public ShopItemInfo[] testShopItems { get; private set; }
    [field: SerializeField] public ItemInfo testItem1 { get; private set; }
    [field: SerializeField] public ItemInfo testItem2 { get; private set; }
    [field: SerializeField] public GunInfo firstWeaponTest { get; private set; }
    [field: SerializeField] public GunInfo secondWeaponTest { get; private set; }
    [field: SerializeField] public int startMoneyForTest { get; private set; }
    [field: SerializeField] public int playerStartArmor { get; private set; }
    [field: SerializeField] public float playerStartArmorRecoverySpeed { get; private set; }

    [HideInInspector]public List<Vector2> eightDirections = new List<Vector2>
    {
        new Vector2(0,1).normalized,
        new Vector2(1,1).normalized,
        new Vector2(1,0).normalized,
        new Vector2(1,-1).normalized,
        new Vector2(0,-1).normalized,
        new Vector2(-1,-1).normalized,
        new Vector2(-1,0).normalized,
        new Vector2(-1,1).normalized,
    };

    private void Awake()
    {
        _bulletTracersPool = new ObjectPool<LineRenderer>(() => Instantiate(bulletTracer));
        _shopCellsPool = new ObjectPool<ShopCellView>(() => AddShopCellToPool());
        mainCamera = Camera.main;
    }

    private ShopCellView AddShopCellToPool()
    {
        var shopCell = Instantiate(shopCellPrefab, shopCellsContainer);
        return shopCell;
    }
    public LineRenderer GetBulletTracer()
    {
        var view = _bulletTracersPool.Get();
        view.gameObject.SetActive(true);
        return view;
    }

    public void ReleaseBulletTracer(LineRenderer renderer)
    {
        renderer.gameObject.SetActive(false);
        _bulletTracersPool.Release(renderer);
    }
    public InventoryCellView GetInventoryCell(int entity, EcsWorld world, Transform cellsContainer)
    {
        var invCell = Instantiate(inventoryCell, cellsContainer).GetComponent<InventoryCellView>();
        invCell.Construct(entity, world);
        return invCell;
    }

    public ShopCellView GetShopCell(int entity, EcsWorld world)
    {
        var view = _shopCellsPool.Get();
        view.gameObject.SetActive(true);

        view.Construct(entity, world);

        return view;
    }

    public void ReleaseShopCell(ShopCellView shopCell)
    {
        shopCell.gameObject.SetActive(false);
        _shopCellsPool.Release(shopCell);
    }

    public PlayerView SpawnPlayer(EcsWorld ecsWorld, int entity)
    {
        playerEntity = entity;
        var player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<PlayerView>();
        player.playerInputView.Construct(ecsWorld, entity);
        player.playerInputView.itemInfoText = currentItemText;
        mainCamera.gameObject.transform.SetParent(player.transform);
        return player;
    }

    public CreatureView GetCreature(Transform spawnObject, Vector2 spawnPosition)
    {
        return Instantiate(spawnObject, spawnPosition, Quaternion.identity).GetComponent<CreatureView>();
    }
    public DroppedItemView SpawnDroppedItem(Vector2 spawnPoint, ItemInfo itemInfo, int entity)
    {
        var droppedItemObj = Instantiate(droppedItemPrefab, spawnPoint, Quaternion.identity).GetComponent<DroppedItemView>();
        droppedItemObj.SetParametersToItem(itemInfo.itemSprite, entity);

        return droppedItemObj;
    }

    public Vector3 GetOutOfScreenPosition()
    {
        var randomX = Random.Range(-1000, 1000);
        var randomY = Random.Range(-1000, 1000);
        var randomPosition = new Vector3(randomX, randomY);
        var randomDirection = (mainCamera.transform.position - randomPosition).normalized;
        var cameraHeight = mainCamera.orthographicSize * 2;
        var cameraWidth = cameraHeight * mainCamera.aspect;
        return new Vector3(randomDirection.x * cameraHeight, randomDirection.y * cameraWidth);
    }
}
