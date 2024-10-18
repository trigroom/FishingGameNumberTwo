using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class CreatureStatesControlSystem : IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<AttackComponent> _currentAttackComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;

    private EcsFilterInject<Inc<CreatureAIComponent>> _creatureAIComponentsFilter;
    public void Run(IEcsSystems systems)
    {
        foreach (var aiEntity in _creatureAIComponentsFilter.Value)
        {
            ref var aiEntityCmp = ref _creatureAIComponentsPool.Value.Get(aiEntity);
            aiEntityCmp.timeToUpdate += Time.deltaTime;
            if (aiEntityCmp.timeToUpdate >= 0.2f)//ии обновл€етс€ 5 раз в секунду’
            {
                aiEntityCmp.timeToUpdate = 0;
                CheckPlayerDistance(aiEntity, ref aiEntityCmp);
            }
        }
    }

    private void CheckPlayerDistance(int aiEntity, ref CreatureAIComponent aiEntityCmp)
    {
        ref var moveCmp = ref _movementComponentsPool.Value.Get(aiEntity);
        RaycastHit2D ray = Physics2D.CircleCast(moveCmp.entityTransform.position, aiEntityCmp.followDistance, moveCmp.entityTransform.up, aiEntityCmp.followDistance, LayerMask.GetMask("Player"));
        if (ray.collider == null)
        {
            Debug.Log("creature is nor collide");
            if (aiEntityCmp.currentState != CreatureAIComponent.CreatureStates.idle)
                aiEntityCmp.currentState = CreatureAIComponent.CreatureStates.idle;
            return;

        }
        var distanceBetweenPlayer = Vector2.Distance(moveCmp.entityTransform.position, ray.collider.transform.position);
        switch (aiEntityCmp.currentState)
        {
            case CreatureAIComponent.CreatureStates.idle:
                if (aiEntityCmp.isPeaceful && distanceBetweenPlayer < aiEntityCmp.minSafeDistance)
                    aiEntityCmp.currentState = CreatureAIComponent.CreatureStates.runAwayFromTarget;
                if (distanceBetweenPlayer > aiEntityCmp.safeDistance && !aiEntityCmp.isPeaceful)
                    aiEntityCmp.currentState = CreatureAIComponent.CreatureStates.follow;
                break;

            case CreatureAIComponent.CreatureStates.follow:
                if (distanceBetweenPlayer < aiEntityCmp.safeDistance && distanceBetweenPlayer > aiEntityCmp.minSafeDistance)
                    aiEntityCmp.currentState = CreatureAIComponent.CreatureStates.shootingToTarget;
                break;

            case CreatureAIComponent.CreatureStates.shootingToTarget:
                ref var curAtkCmp = ref _currentAttackComponentsPool.Value.Get(aiEntity);
                if (!curAtkCmp.canAttack)
                    curAtkCmp.canAttack = true;
                // Debug.Log(distanceBetweenPlayer + "<=" + aiEntityCmp.minSafeDistance);
                if (distanceBetweenPlayer <= aiEntityCmp.minSafeDistance)
                    aiEntityCmp.currentState = CreatureAIComponent.CreatureStates.runAwayFromTarget;
                else if (distanceBetweenPlayer >= aiEntityCmp.safeDistance*1.15f)
                    aiEntityCmp.currentState = CreatureAIComponent.CreatureStates.follow;
                break;

            case CreatureAIComponent.CreatureStates.runAwayFromTarget:
                if (distanceBetweenPlayer >= aiEntityCmp.safeDistance)
                {
                    if (!aiEntityCmp.isPeaceful)
                    {
                        _currentAttackComponentsPool.Value.Get(aiEntity).canAttack = false;
                        aiEntityCmp.currentState = CreatureAIComponent.CreatureStates.shootingToTarget;
                    }
                    else
                        aiEntityCmp.currentState = CreatureAIComponent.CreatureStates.idle;
                }
                break;
        }

    }
}
