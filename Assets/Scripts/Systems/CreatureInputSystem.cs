using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class CreatureInputSystem : IEcsRunSystem
{
    private EcsPoolInject<MovementComponent> _movementComponentPool;
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;
    private EcsPoolInject<HidedObjectOutsideFOVComponent> _hidedObjectOutsideFOVComponentsPool;

    private EcsFilterInject<Inc<CreatureAIComponent>> _creatureAIComponentsFilter;

    private EcsCustomInject<SceneService> _sceneService;

    private EcsWorldInject _world;
    public void Run(IEcsSystems systems)
    {
        ref var playerMoveCmp = ref _movementComponentPool.Value.Get(_sceneService.Value.playerEntity);
        foreach (var aiCreatureEntity in _creatureAIComponentsFilter.Value)
        {
            ref var aiCmp = ref _creatureAIComponentsPool.Value.Get(aiCreatureEntity);
            ref var moveCmp = ref _movementComponentPool.Value.Get(aiCreatureEntity);

            if (moveCmp.isStunned /*&& aiCmp.reachedLastTargetâðåìåííî*/&& aiCmp.targetPositionCached != null) continue;

            if(aiCmp.currentState != CreatureAIComponent.CreatureStates.idle)
            moveCmp.pointToRotateInput = aiCmp.targetPositionCached;

            // Debug.Log(moveCmp.moveInput + "move inp and res dir " + aiCmp.resultDirection);
            if(_hidedObjectOutsideFOVComponentsPool.Value.Get(aiCreatureEntity).timeBeforeHide > 0)
            {
            if (moveCmp.moveInput != Vector2.zero)
                moveCmp.movementView.characterAnimator.SetBool("isWalking", true);
            else 
                moveCmp.movementView.characterAnimator.SetBool("isWalking", false);
            }

            moveCmp.moveInput = aiCmp.resultDirection;
        }
    }
}
