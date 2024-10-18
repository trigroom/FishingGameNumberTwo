using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class CreatureInputSystem : IEcsRunSystem
{
    private EcsPoolInject<MovementComponent> _movementComponentPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<AttackComponent> _currentAttackComponentsPool;
    //private EcsPoolInject<GunComponent> _gunComponentsPool;
    //private EcsPoolInject<ArmorComponent> _armorComponentsPool;
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;

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

            if (moveCmp.isStunned) continue;
            //если не идл, то точка на которую смотрит будет равна направлению движения
            //если убегает и не стреляет то тоже в сторону движения поворачивается

            if (aiCmp.currentState == CreatureAIComponent.CreatureStates.idle)
            {
                //рандомное движение
                //var heading = (playerMoveCmp.entityTransform.position- moveCmp.entityTransform.position).normalized;
                aiCmp.randomMoveTime -= Time.deltaTime;
                //Debug.Log("ran time" + aiCmp.randomMoveTime);
                if (aiCmp.randomMoveTime <= 0)
                {
                    aiCmp.randomMoveTime = Random.Range(1, 6);
                    if (moveCmp.moveInput == Vector2.zero)
                    {
                        moveCmp.moveInput = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                        moveCmp.canMove = true;
                        Debug.Log("ran mone yes");
                        Ray2D ray = new Ray2D(moveCmp.entityTransform.position, moveCmp.moveInput);
                        moveCmp.pointToRotateInput = ray.origin + (ray.direction * 20);
                    }

                    else
                    {
                        moveCmp.canMove = false;
                        moveCmp.moveInput = Vector2.zero;
                        Debug.Log("ran mone not");
                    }

                }
                //moveCmp.moveInput = heading;
            }
            else if (aiCmp.currentState == CreatureAIComponent.CreatureStates.follow)
            {
                moveCmp.pointToRotateInput = playerMoveCmp.entityTransform.position;
                if (!moveCmp.canMove)
                    moveCmp.canMove = true;
                //var heading = ( playerMoveCmp.entityTransform.position -moveCmp.entityTransform.position);
                var heading = (playerMoveCmp.entityTransform.position - moveCmp.entityTransform.position).normalized;
                // moveCmp.moveInput = heading / heading.magnitude;
                moveCmp.moveInput = heading;
            }
            else if (aiCmp.currentState == CreatureAIComponent.CreatureStates.shootingToTarget)
            {
                moveCmp.pointToRotateInput = playerMoveCmp.entityTransform.position;
                moveCmp.moveInput = Vector3.zero;
                if (moveCmp.canMove)
                    moveCmp.canMove = false;
            }
            else if (aiCmp.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget)
            {
                if (!moveCmp.canMove)
                    moveCmp.canMove = true;
                //var heading = (moveCmp.entityTransform.position - playerMoveCmp.entityTransform.position);
                var heading = (moveCmp.entityTransform.position - playerMoveCmp.entityTransform.position).normalized;
                //moveCmp.moveInput = heading / heading.magnitude;
                moveCmp.moveInput = heading;
                if (aiCmp.isAttackWhenRetreat)
                    moveCmp.pointToRotateInput = playerMoveCmp.entityTransform.position;
                else
                {
                    Ray2D ray = new Ray2D(moveCmp.entityTransform.position, moveCmp.moveInput);
                    moveCmp.pointToRotateInput = ray.origin + (ray.direction * 20);
                }
            }
            Debug.Log(aiCmp.currentState);
            //Debug.Log(moveCmp.moveInput + " move inpt ai");
        }
    }
}
