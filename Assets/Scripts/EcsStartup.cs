using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Button;

public class EcsStartUp : MonoBehaviour
{
    [SerializeField] private SceneService _sceneService;

    EcsWorld _world;
    IEcsSystems _systems;

    void Start()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);
        _systems
#if UNITY_EDITOR
            .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
            .Add(new PlayerInputSystem())
            .Add(new MovementSystem())
            .Add(new InventorySystem())
            .Add(new EnemyDeathSystem())
            .Add(new UiControlSystem())
            .DelHere<AddItemEvent>()
            .DelHere<EnemyDeathEvent>()
            .DelHere<SetDescriptionItemEvent>()
            .DelHere<DropItemsIvent>()
             .Inject(_sceneService)
            .Init();
    }
    void Update()
    {
        _systems?.Run();
    }

    void OnDestroy()
    {
        if (_systems != null)
        {
            _systems.Destroy();
            _systems = null;
        }

        if (_world != null)
        {
            _world.Destroy();
            _world = null;
        }
    }
}
