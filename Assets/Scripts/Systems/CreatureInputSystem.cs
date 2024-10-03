using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.GraphicsBuffer;

public class CreatureInputSystem : IEcsRunSystem
{
    private EcsPoolInject<MovementComponent> _movementComponentPool;
    private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<CurrentAttackComponent> _currentAttackComponentsPool;
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

            moveCmp.pointToRotateInput = playerMoveCmp.entityTransform.position;
            //���� �� ���, �� ����� �� ������� ������� ����� ����� ����������� ��������
            //���� ������� � �� �������� �� ���� � ������� �������� ��������������

            if (aiCmp.currentState == CreatureAIComponent.CreatureStates.idle)
            {
                //��������� ��������
                //var heading = (playerMoveCmp.entityTransform.position- moveCmp.entityTransform.position).normalized;
                moveCmp.moveInput = Vector3.zero;
                if (moveCmp.canMove)
                    moveCmp.canMove = false;
                //moveCmp.moveInput = heading;
            }
            else if (aiCmp.currentState == CreatureAIComponent.CreatureStates.follow)
            {
                if(!moveCmp.canMove)
                moveCmp.canMove = true;
                //var heading = ( playerMoveCmp.entityTransform.position -moveCmp.entityTransform.position);
                var heading = (playerMoveCmp.entityTransform.position- moveCmp.entityTransform.position).normalized;
               // moveCmp.moveInput = heading / heading.magnitude;
              moveCmp.moveInput = heading;
            }
            else if(aiCmp.currentState == CreatureAIComponent.CreatureStates.shootingToTarget)
            {
                moveCmp.moveInput = Vector3.zero;
                if(moveCmp.canMove)
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
            }
            Debug.Log(aiCmp.currentState);
            //Debug.Log(moveCmp.moveInput + " move inpt ai");
        }
    }
}
