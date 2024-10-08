using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using UnityEngine;

public class EcsStartUp : MonoBehaviour
{
    [SerializeField] private SceneService _sceneService;

    EcsWorld _world;
    IEcsSystems _systemsFixedUpdate;
    IEcsSystems _systemsUpdate;

    void Start()
    {
        _world = new EcsWorld();
        _systemsFixedUpdate = new EcsSystems(_world);
        _systemsUpdate = new EcsSystems(_world);


        _systemsUpdate
#if UNITY_EDITOR
            .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
            .Add(new CreatureInputSystem())
            .Add(new PlayerInputSystem())
           .Add(new DayChangeSystem())
            .Add(new CameraMovementSystem())
            .Add(new CreatureStatesControlSystem())
            .Add(new AttackSystem())
            .Add(new InventorySystem())
            .Add(new HealthSystem())
            .Add(new UiControlSystem())
            .Add(new ShopCellsSystem())
            .Add(new SpawnSystem())

        .DelHere<ReloadEvent>()
        .DelHere<AddItemEvent>()
        .DelHere<EnemyDeathEvent>()
        .DelHere<SetDescriptionItemEvent>()
        .DelHere<DropItemsIvent>()
        .DelHere<FindAndCellItemEvent>()
        .DelHere<BuyItemFromShopEvent>()
        .DelHere<ShopCloseEvent>()
        .DelHere<ShopOpenEvent>()
        .DelHere<ChangeHealthEvent>()
        .DelHere<StorageOpenEvent>()
        .DelHere<MoveSpecialItemToInventoryEvent>()
        .DelHere<AddItemFromCellEvent>()
        .DelHere<EntrySpawnZoneEvent>()
        .DelHere<ExitSpawnZoneEvent>()
        .DelHere<HealFromHealItemCellEvent>()
        .DelHere<HealFromInventoryEvent>()
        .DelHere<ChangeToNightEvent>()
        .DelHere<ChangeToDayEvent>()
         //  .DelHere<ChangeWeaponFromInventoryEvent>()

         .Inject(_sceneService)
         .Init();
        _systemsFixedUpdate
         .Add(new MovementSystem())
          .Inject(_sceneService)

         .Init();

    }
    void Update()
    {
        _systemsUpdate?.Run();
    }

    void FixedUpdate()
    {
        _systemsFixedUpdate?.Run();
    }

    void OnDestroy()
    {
        if (_systemsUpdate != null)
        {
            _systemsUpdate.Destroy();
            _systemsUpdate = null;
        }

        if (_systemsFixedUpdate != null)
        {
            _systemsFixedUpdate.Destroy();
            _systemsFixedUpdate = null;
        }

        if (_world != null)
        {
            _world.Destroy();
            _world = null;
        }
    }
}
