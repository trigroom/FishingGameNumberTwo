using Leopotam.EcsLite;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SceneService : MonoBehaviour
{
    [field: SerializeField] public AudioClip[] randomEmbientSounds { get; private set; }
    [field: SerializeField] public AudioClip offOnDeviceSound { get; private set; }
    [field: SerializeField] public AudioSource backgroundAudioSource { get; private set; }
    [field: SerializeField] public AudioClip rainEmbient { get; private set; }
    [field: SerializeField] public AudioClip windEmbient { get; private set; }
    [field: SerializeField] public Animator staminaAnimator { get; private set; }
    [field: SerializeField] public Animator healthAnimator { get; private set; }
    [field: SerializeField] public Transform startLocationLightsContainer { get; private set; }
    [field: SerializeField] public RectTransform mainCanvas { get; private set; }
    [field: SerializeField] public Animator warningInfoTextAnimator { get; private set; }
    [field: SerializeField] public TMP_Text warningInfoText { get; private set; }
    [field: SerializeField] public AudioSource uiAudioSourse { get; private set; }
    [field: SerializeField] public AudioClip equipItemSound { get; private set; }
    [field: SerializeField] public AudioClip closeInventorySound { get; private set; }
    [field: SerializeField] public AudioClip openInventorySound { get; private set; }
    [field: SerializeField] public SpritesArrayElement[] johnHairsSprites { get; private set; }
    [field: SerializeField] public Color[] hairsColors { get; private set; }
    [field: SerializeField] public GuidePageInfo[] guidePagesInfo { get; private set; }
    [field: SerializeField] public TMP_Text gameVersionText { get; private set; }
    [field: SerializeField] public ItemsWithIdListInfo idItemslist { get; private set; }
    [field: SerializeField] public Transform startLocationEntry { get; private set; }
    [field: SerializeField] public RectTransform bluredUICanvas { get; private set; }
    [field: SerializeField] public int[] startShoppers { get; private set; }
    [field: SerializeField] public int[] levelExpCounts { get; private set; }
    [field: SerializeField] public InteractCharacterView[] interactCharacters { get; private set; }
    [field: SerializeField] public Sprite[] craftingTablesSprites { get; private set; }
    [field: SerializeField] public InteractCharacterView craftingTableInteractView { get; private set; }
    [field: SerializeField] public Transform defaultEnemy { get; private set; }

    [field: SerializeField] public Animator fadeScreenAnimator { get; private set; }
    [field: SerializeField] public Transform locationsContainer { get; private set; }
    [field: SerializeField] public SpriteRenderer bulletShellPrefab { get; private set; }
    [field: SerializeField] public Sprite[] bulletShellSprites { get; private set; }
    [field: SerializeField] public Transform startLocation { get; private set; }
    [field: SerializeField] public Transform rainEffectContainer { get; private set; }
  //  [field: SerializeField] public Transform playerSpriteContainer { get; private set; }
    [field: SerializeField] public ParticleSettingsInfo bloodParticleInfo { get; private set; }
    [field: SerializeField] public ParticleSettingsInfo sparcleParticleInfo { get; private set; }
    [field: SerializeField] public ParticleSettingsInfo smallBloodParticleInfo { get; private set; }
    [field: SerializeField] public int timeHourOffset { get; private set; }
    [field: SerializeField] public float dayTime { get; private set; }
    [field: SerializeField] public Gradient nightLightColor { get; private set; }
    [field: SerializeField] public InteractCharacterView storageInteractView { get; private set; }
    //  [field: SerializeField] public Color dayLightColor { get; private set; }
    [field: SerializeField] public InventoryCellView flashlightItemCellView { get; private set; }
    [field: SerializeField] public InventoryCellView healingItemCellView { get; private set; }
    [field: SerializeField] public InventoryCellView firstGunCellView { get; private set; }
    [field: SerializeField] public InventoryCellView secondGunCellView { get; private set; }
    [field: SerializeField] public InventoryCellView meleeWeaponCellView { get; private set; }
    [field: SerializeField] public InventoryCellView grenadeCellView { get; private set; }
    [field: SerializeField] public InventoryCellView backpackCellView { get; private set; }
    [field: SerializeField] public InventoryCellView shieldCellView { get; private set; }
    [field: SerializeField] public InventoryCellView bodyArmorCellView { get; private set; }
    [field: SerializeField] public InventoryCellView helmetCellView { get; private set; }
    [field: SerializeField] public TMP_Text playerStaminaText { get; private set; }
    [field: SerializeField] public Image playerStaminaBarFilled { get; private set; }
    [field: SerializeField] public Image bulletForMagUI { get; private set; }
    [field: SerializeField] public TMP_Text playerArmorText { get; private set; }
    [field: SerializeField] public Image playerArmorBarFilled { get; private set; }
    [field: SerializeField] public TMP_Text playerHealthText { get; private set; }
    [field: SerializeField] public Image playerHealthBarFilled { get; private set; }
    [field: SerializeField] public MainMenuView mainMenuView { get; private set; }
    [field: SerializeField] public UIMenuView storageMenuView { get; private set; }
    [field: SerializeField] public UIMenuView questMenuView { get; private set; }
    [field: SerializeField] public UIMenuView gunWorkshopMenuView { get; private set; }
    [field: SerializeField] public UIMenuView shopMenuView { get; private set; }
    [field: SerializeField] public UIMenuView inventoryMenuView { get; private set; }
    [field: SerializeField] public UIMenuView craftingMenuView { get; private set; }
    [field: SerializeField] public UIElementsView dropedItemsUIView { get; private set; }
    [field: SerializeField] public Transform inventoryCellsContainer { get; private set; }
    [field: SerializeField] public Transform questsDescriptionsContainer { get; private set; }
    [field: SerializeField] public Transform storageCellsContainer { get; private set; }
    [field: SerializeField] public Transform effectsIconsContainer { get; private set; }
    [field: SerializeField] public Transform shopCellsContainer { get; private set; }
    [field: SerializeField] public InventoryCellView inventoryCell { get; private set; }
    [field: SerializeField] public int storageCellsCount { get; private set; }
    [field: SerializeField] public TMP_Text moneyText { get; private set; }
    [field: SerializeField] public TMP_Text statsInventoryText { get; private set; }
    [field: SerializeField] public TMP_Text statsStorageText { get; private set; }
    [field: SerializeField] public TMP_Text currentItemText { get; private set; }
    [field: SerializeField] public float maxInInventoryWeight { get; private set; }
    [field: SerializeField] public float maxInStorageWeight { get; private set; }
    [field: SerializeField] public GameObject playerPrefab { get; private set; }
    [field: SerializeField] public GrenadeView grenadePrefab { get; private set; }
    [field: SerializeField] public GameObject droppedItemPrefab { get; private set; }
    [field: SerializeField] public ShopCellView shopCellPrefab { get; private set; }
    [field: SerializeField] public ShopCellView[] shopCellsList { get; private set; }
    [field: SerializeField] public float solarEnergyGeneratorSpeed { get; private set; }
    [field: SerializeField] public float solarEnergyGeneratorMaxCapacity { get; private set; }
    [field: SerializeField] public TMP_Text ammoInfoText;
    [field: SerializeField] public Light2D gloabalLight { get; set; }
    [field: SerializeField] public Camera mainCamera { get; private set; }
    [field: SerializeField] public LineRenderer bulletTracer { get; private set; }
    [field: SerializeField] public ParticleSystem particlePrefab { get; private set; }
    [field: SerializeField] public int playerEntity;//{ get; private set; }
    [field: SerializeField] public TMP_Text[] questDescription { get; private set; }
    [field: SerializeField] public AudioSource soundFXObject { get; private set; }
    //[field: SerializeField] public int cameraMoveSpeed { get; private set; }
   // [field: SerializeField] public ShopCharacterView[] shoppers { get; private set; }
    [field: SerializeField] public EffectIconView effectIconViewPrefab { get; private set; }
    [field: SerializeField] public Sprite transparentSprite { get; private set; }
    private ObjectPool<LineRenderer> _bulletTracersPool;
    private ObjectPool<ParticleSystem> _particlesPool;
    private ObjectPool<GrenadeView> _grenadesPool;
    private ObjectPool<AudioSource> _oneShotSoundsPool;
    private ObjectPool<Image> _bulletsForMagUIImagesPool;
    private ObjectPool<EffectIconView> _effectIconViewsPool;
    private ObjectPool<SpriteRenderer> _bulletShellsPool;
    private ObjectPool<Image> _particlesOnScreenImagesPool;
    private ObjectPool<InventoryCellView> _inventoryCellViewsPool;//
    public InventoryCellView[] _inventoryCellsViewsPool;//задавать номер в массиве в тэгах клетки инвентаря

    public int storageEntity { get; set; }
    public int inventoryEntity;
    [Header("Steps sounds")]
    [field: SerializeField] public AudioClip[] stepsOnGrassSounds { get; private set; }
    [field: SerializeField] public AudioClip[] stepsOnWoodSounds { get; private set; }
    [field: SerializeField] public AudioClip[] stepsOnStoneSounds { get; private set; }
    [Header("For tests")]
    //всё для тестов\/
    [field: SerializeField] public int playerStartArmor { get; private set; }
    [field: SerializeField] public float playerStartArmorRecoverySpeed { get; private set; }
    [field: SerializeField] public Image bloodParticleOnScreen { get; private set; }
    [field: SerializeField] public Sprite[] bloodParticlesSprites { get; private set; }
    [field: SerializeField] public Transform particlesOnScreenContainer { get; private set; }
    [field: SerializeField] public Color bloodParticleOnScreenColor { get; private set; }
    [field: SerializeField] public Color grassParticleOnScreenColor { get; private set; }
    [field: SerializeField] public Color rainDropOnScreenColor { get; private set; }
    [field: SerializeField] public UnityEngine.Rendering.Volume volumeMainBg { get; set; }
    [field: SerializeField] public UnityEngine.Rendering.VolumeProfile volumeProfileMainBg { get; set; }
    [field: SerializeField] public DepthOfField depthOfFieldMainBg;
    [field: SerializeField] public Bloom bloomMainBg;
    [field: SerializeField] public Sprite[] bloodEffectsSprites { get; private set; }
    [field: SerializeField] public Sprite[] rainDropsEffectsSprites { get; private set; }

    [HideInInspector]
    public List<Vector2> eightDirections = new List<Vector2>
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
        gameVersionText.text = Application.version.ToString();
       // Instantiate(playerSpriteContainer);
        volumeMainBg.profile.TryGet(out depthOfFieldMainBg);
        volumeMainBg.profile.TryGet(out bloomMainBg);

        _effectIconViewsPool = new ObjectPool<EffectIconView>(() => Instantiate(effectIconViewPrefab, effectsIconsContainer));
        _bulletTracersPool = new ObjectPool<LineRenderer>(() => Instantiate(bulletTracer));
        _bulletTracersPool = new ObjectPool<LineRenderer>(() => Instantiate(bulletTracer));
        _particlesPool = new ObjectPool<ParticleSystem>(() => Instantiate(particlePrefab));
        _grenadesPool = new ObjectPool<GrenadeView>(() => Instantiate(grenadePrefab));
        _bulletShellsPool = new ObjectPool<SpriteRenderer>(() => Instantiate(bulletShellPrefab));
        _oneShotSoundsPool = new ObjectPool<AudioSource>(() => Instantiate(soundFXObject));
        _particlesOnScreenImagesPool = new ObjectPool<Image>(() => Instantiate(bloodParticleOnScreen, particlesOnScreenContainer));
        _bulletsForMagUIImagesPool = new ObjectPool<Image>(() => Instantiate(bulletForMagUI, dropedItemsUIView.gunMagazineUI.transform));
        _inventoryCellViewsPool = new ObjectPool<InventoryCellView>(() => Instantiate(inventoryCell));//пока так
        mainCamera = Camera.main;
    }
    public Transform InstantiateLevel(Transform level)
    {
        return Instantiate(level, locationsContainer);
    }

    public void DestroyLevel(Transform level)
    {
        Destroy(level.gameObject);
    }
    public EffectIconView GetEffectIconView()
    {
        var view = _effectIconViewsPool.Get();
        view.gameObject.SetActive(true);
        return view;
    }
    public void ReleaseEffecticonView(EffectIconView view)
    {
        view.gameObject.SetActive(false);
        _effectIconViewsPool.Release(view);
    }
    public LineRenderer GetBulletTracer()
    {
        var view = _bulletTracersPool.Get();
        view.gameObject.SetActive(true);
        return view;
    }
    public SpriteRenderer GetBulletShell()
    {
        var bulletShell = _bulletShellsPool.Get();
        bulletShell.gameObject.SetActive(true);
        return bulletShell;
    }
    public void ReleaseBulletShell(SpriteRenderer bulletShell)
    {
        bulletShell.gameObject.SetActive(false);
        _bulletShellsPool.Release(bulletShell);
    }
    public Image GetParticleOnScreen(Color particleColor, float alpfaMultiplayer, bool isRainDrop)
    {
        var screenParticle = _particlesOnScreenImagesPool.Get();
        screenParticle.gameObject.SetActive(true);
        if (!isRainDrop)
        {
            screenParticle.sprite = bloodParticlesSprites[Random.Range(0, bloodParticlesSprites.Length)];

            if (Random.value > 0.5f)
                screenParticle.rectTransform.localScale = new Vector2(screenParticle.rectTransform.localScale.x, -1);
            else
                screenParticle.rectTransform.localScale = new Vector2(screenParticle.rectTransform.localScale.x, 1);

            if (Random.value > 0.5f)
                screenParticle.rectTransform.localScale = new Vector2(-1, screenParticle.rectTransform.localScale.y);
            else
                screenParticle.rectTransform.localScale = new Vector2(1, screenParticle.rectTransform.localScale.y);

            screenParticle.rectTransform.localScale *= Random.Range(0.6f, 1.2f);
        }
        else
        {
            int needRainDropIndex = 0;
            if(Random.value < 0.1f)
                needRainDropIndex = 2;
            else if(Random.value < 0.35f)
                needRainDropIndex = 1;
            screenParticle.sprite = rainDropsEffectsSprites[needRainDropIndex];
            screenParticle.rectTransform.localScale = Vector2.one * 0.2f * (needRainDropIndex+1);
        }

        if (Random.value > 0.5f)
            screenParticle.rectTransform.anchoredPosition = new Vector2(screenParticle.rectTransform.anchoredPosition.x, Random.Range(0.2f, 0.5f));
        else
            screenParticle.rectTransform.anchoredPosition = new Vector2(screenParticle.rectTransform.anchoredPosition.x, Random.Range(-0.5f, -0.2f));

        if (Random.value > 0.5f)
            screenParticle.rectTransform.anchoredPosition = new Vector2(Random.Range(0.3f, 1f), screenParticle.rectTransform.anchoredPosition.y);
        else
            screenParticle.rectTransform.anchoredPosition = new Vector2(Random.Range(-1f, -0.3f), screenParticle.rectTransform.anchoredPosition.y);

        screenParticle.color = new Color(particleColor.r, particleColor.g, particleColor.b, Random.Range(particleColor.a - 0.1f, particleColor.a + 0.1f) * alpfaMultiplayer);
        return screenParticle;
    }
    public void ReleaseParticleOnScreen(Image screenParticle)
    {
        screenParticle.gameObject.SetActive(false);
        _particlesOnScreenImagesPool.Release(screenParticle);
    }
    public Image GetBulletForMagUI()
    {
        var bulletImage = _bulletsForMagUIImagesPool.Get();
        bulletImage.gameObject.SetActive(true);
        return bulletImage;
    }

    public GrenadeView GetGrenadeObject()
    {
        var view = _grenadesPool.Get();
        view.gameObject.SetActive(true);
        view.grenadeCollider.enabled = true;
        return view;
    }
    public void ReleaseGrenadeObject(GrenadeView view)
    {
        view.gameObject.SetActive(false);
        _grenadesPool.Release(view);
    }
    public void ReleaseSoundObject(AudioSource audio)
    {
        audio.gameObject.SetActive(false);
        _oneShotSoundsPool.Release(audio);
    }
    public void ReleaseBulletTracer(LineRenderer renderer)
    {
        renderer.gameObject.SetActive(false);
        _bulletTracersPool.Release(renderer);
    }
    public void ReleaseBulletFromUIMag(Image bulletImage)
    {
        bulletImage.gameObject.SetActive(false);
        _bulletsForMagUIImagesPool.Release(bulletImage);
    }

    public ParticleSystem GetParticlePrefab(ParticleSettingsInfo particleInfo)
    {
        var view = _particlesPool.Get();
        if (view.textureSheetAnimation.GetSprite(0) != particleInfo.particleSprite)
        {
            var needParticle = view;
            var needParticleMain = needParticle.main;

            var needParticleMainStartLifetime = needParticleMain.startLifetime;
            needParticleMainStartLifetime.constantMin = particleInfo.startLifetimeMin;
            needParticleMainStartLifetime.constantMax = particleInfo.startLifetimeMax;
            needParticleMain.startLifetime = needParticleMainStartLifetime;

            var needParticleMainStartSize = needParticleMain.startSize;
            needParticleMainStartSize.constantMin = particleInfo.startSizeMin;
            needParticleMainStartSize.constantMax = particleInfo.startSizeMax;
            needParticleMain.startSize = needParticleMainStartSize;

            var needParticleMainStartSpeed = needParticleMain.startSpeed;
            needParticleMainStartSpeed.constantMin = particleInfo.startSpeedMin;
            needParticleMainStartSpeed.constantMax = particleInfo.startSpeedMax;
            needParticleMain.startSpeed = needParticleMainStartSpeed;

            needParticle.textureSheetAnimation.SetSprite(0, particleInfo.particleSprite);
            needParticleMain.startColor = particleInfo.startColor;

            view = needParticle;
        }
        view.gameObject.SetActive(true);
        return view;
    }

    public void ReleaseParticlePrefab(ParticleSystem particles)
    {
        particles.gameObject.transform.SetParent(null);
        particles.gameObject.SetActive(false);

        _particlesPool.Release(particles);
    }

    public InventoryCellView GetItemCell(Transform cellsContainer)
    {
        var invCell = _inventoryCellViewsPool.Get();
        invCell.gameObject.SetActive(true);
        invCell.gameObject.transform.SetParent(cellsContainer);
        return invCell;
    }

    public InventoryCellView GetInventoryCell(int cellId)
    {
        var invCell = _inventoryCellsViewsPool[cellId];
        invCell.gameObject.SetActive(true);
        return invCell;
    }

    public ShopCellView GetShopCell(int cellIndex)
    {
        var view = shopCellsList[cellIndex];
        view.gameObject.SetActive(true);

        return view;
    }

    public void ReleaseShopCell(int cellIndex)
    {
        var view = shopCellsList[cellIndex];
        view.gameObject.SetActive(false);
    }

    public PlayerView SpawnPlayer(EcsWorld ecsWorld, int entity)
    {
        playerEntity = entity;
        var player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).transform.GetChild(1).GetComponent<PlayerView>();
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
        spawnPoint = new Vector2(Random.Range(spawnPoint.x - 0.7f, spawnPoint.x + 0.7f), Random.Range(spawnPoint.y - 0.7f, spawnPoint.y + 0.7f));
        var droppedItemObj = Instantiate(droppedItemPrefab, spawnPoint, Quaternion.identity).GetComponent<DroppedItemView>();
        droppedItemObj.SetParametersToItem(itemInfo.itemSprite, entity);

        return droppedItemObj;
    }

    public DroppedItemView SpawnDroppedItemWithoutSpriteChange(Vector2 spawnPoint, int entity)
    {
        var droppedItemObj = Instantiate(droppedItemPrefab, spawnPoint, Quaternion.identity).GetComponent<DroppedItemView>();
        droppedItemObj.SetParametersToItem(entity);

        return droppedItemObj;
    }

    public void ShowWarningText(string needText)
    {
        var screenPoint =(Input.mousePosition-mainCanvas.position)/mainCanvas.localScale.x;
        warningInfoText.rectTransform.anchoredPosition = screenPoint;
        warningInfoTextAnimator.SetTrigger("SetText");
        warningInfoText.text = needText;
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

    public AudioSource PlaySoundFXClip(AudioClip clip, Vector2 spawnPosition, float volume)
    {
        var audioSource = _oneShotSoundsPool.Get();
        audioSource.gameObject.SetActive(true);
        audioSource.transform.position = spawnPosition;

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        return audioSource;
    }

    public Transform InstantiateObject(Transform spawnObject, Vector2 spawnPosition)
    {
        return Instantiate(spawnObject, spawnPosition, Quaternion.identity).transform;
    }
}
