using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CreatureStatesControlSystem : IEcsRunSystem
{
    private EcsWorldInject _world;
    private EcsCustomInject<SceneService> _sceneData;

    private EcsPoolInject<AttackComponent> _currentAttackComponentsPool;
    private EcsPoolInject<MovementComponent> _movementComponentsPool;
    private EcsPoolInject<CreatureAIComponent> _creatureAIComponentsPool;
    private EcsPoolInject<HealingItemComponent> _healingItemComponentsPool;
    private EcsPoolInject<CreatureInventoryComponent> _creatureInventoryComponentsPool;
    private EcsPoolInject<CreatureChangeWeaponEvent> _creatureChangeWeaponEventsPool;

    private EcsFilterInject<Inc<CreatureAIComponent>> _creatureAIComponentsFilter;
    public void Run(IEcsSystems systems)
    {
        foreach (var aiEntity in _creatureAIComponentsFilter.Value)
        {
            ref var aiEntityCmp = ref _creatureAIComponentsPool.Value.Get(aiEntity);
            aiEntityCmp.timeToUpdate += Time.deltaTime;
            if (aiEntityCmp.timeToUpdate >= 0.1f)//ии обновляется 5 раз в секундуХ
            {
                aiEntityCmp.timeToUpdate = 0;
                Detect(aiEntity);

                GetDirectionToMove(aiEntity);
                // CheckPlayerDistance(aiEntity, ref aiEntityCmp);
            }
            if (aiEntityCmp.sightOnTarget && aiEntityCmp.currentTarget != null && aiEntityCmp.needSightOnTargetTime + (Vector2.Distance(aiEntityCmp.currentTarget.position, aiEntityCmp.creatureView.transform.position) * 0.1f) + 1.5f > aiEntityCmp.sightOnTargetTime)
                aiEntityCmp.sightOnTargetTime += Time.deltaTime;
            else if (!aiEntityCmp.sightOnTarget && 0 < aiEntityCmp.sightOnTargetTime)
                aiEntityCmp.sightOnTargetTime -= Time.deltaTime;

        }
    }

    public Vector2 GetDirectionToMove(int entity)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        ref var aiCmp = ref _creatureAIComponentsPool.Value.Get(entity);
        ref var moveCmp = ref _movementComponentsPool.Value.Get(entity);

        (danger, interest) = GetSteering(danger, interest, entity);
        (danger, interest) = GetSteeringSeek(danger, interest, entity);

        for (int i = 0; i < 8; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < 8; i++)
        {
            outputDirection += _sceneData.Value.eightDirections[i] * interest[i];
        }

        outputDirection.Normalize();

        aiCmp.resultDirection = outputDirection;

        Debug.DrawRay(moveCmp.entityTransform.position, aiCmp.resultDirection * aiCmp.followDistance, Color.cyan);
        return aiCmp.resultDirection;
    }
    public (float[] danger, float[] interest) GetSteeringSeek(float[] danger, float[] interest, int aiEntity)
    {
        ref var aiCmp = ref _creatureAIComponentsPool.Value.Get(aiEntity);
        ref var moveCmp = ref _movementComponentsPool.Value.Get(aiEntity);
        Vector2 creaturePosition = moveCmp.entityTransform.position;

        if (aiCmp.currentState == CreatureAIComponent.CreatureStates.idle)
        { //random move

            if (!aiCmp.isStoppedMoveInIdleState)
            {
                if (danger.Max() < 0.2f)
                    interest[aiCmp.randomMoveDirectionIndex] = 1;
                else
                {
                    int maxDanger = System.Array.IndexOf(danger, danger.Max());//тут продолжить

                    interest = new float[8];

                    int needNum = maxDanger;

                    if (maxDanger < 4)
                        needNum += 4;
                    else
                        needNum -= 4;

                    aiCmp.randomMoveDirectionIndex = needNum;

                    interest[aiCmp.randomMoveDirectionIndex] = 1;

                    Ray2D ray = new Ray2D(moveCmp.entityTransform.position, _sceneData.Value.eightDirections[aiCmp.randomMoveDirectionIndex]);//
                    moveCmp.pointToRotateInput = ray.origin + (ray.direction * 20);

                }
            }
            if (aiCmp.randomMoveTime <= 0)
            {
                aiCmp.randomMoveTime = Random.Range(0.3f, 1.5f);
                if (aiCmp.isStoppedMoveInIdleState)
                {
                    aiCmp.randomMoveDirectionIndex = Random.Range(0, 8);
                    aiCmp.isStoppedMoveInIdleState = false;
                    Ray2D ray = new Ray2D(moveCmp.entityTransform.position, _sceneData.Value.eightDirections[aiCmp.randomMoveDirectionIndex]);//
                    moveCmp.pointToRotateInput = ray.origin + (ray.direction * 20);//
                }

                else
                    aiCmp.isStoppedMoveInIdleState = true;
            }
            else
                aiCmp.randomMoveTime -= Time.deltaTime;

            return (danger, interest);
        }


        if (aiCmp.reachedLastTarget)
        {
            if (aiCmp.targets == null || aiCmp.targets.Count <= 0)
            {
                aiCmp.currentTarget = null;
                return (danger, interest);
            }
            else
            {
                aiCmp.reachedLastTarget = false;
                aiCmp.currentTarget = aiCmp.targets.OrderBy(target => Vector2.Distance(target.position, creaturePosition)).FirstOrDefault();
            }
        }

        if (aiCmp.currentTarget != null && aiCmp.targets != null && aiCmp.targets.Contains(aiCmp.currentTarget))
        {
            aiCmp.targetPositionCached = aiCmp.currentTarget.position;
        }

        if (Vector2.Distance(creaturePosition, aiCmp.targetPositionCached) < 0.5f)
        {
            aiCmp.reachedLastTarget = true;
            aiCmp.currentTarget = null;
            return (danger, interest);
        }

        Vector2 directionToTarget = (aiCmp.targetPositionCached - (Vector2)moveCmp.entityTransform.position);
        var creatureInv = _creatureInventoryComponentsPool.Value.Get(aiEntity);
        float result = 0;
        bool isHealing = creatureInv.healingItem != null && _healingItemComponentsPool.Value.Get(aiEntity).isHealing;
        moveCmp.isRun = false;
        if (((aiCmp.isTwoWeapon && _creatureInventoryComponentsPool.Value.Get(aiEntity).isSecondWeaponUsed) || aiCmp.currentState == CreatureAIComponent.CreatureStates.follow || (aiCmp.currentState != CreatureAIComponent.CreatureStates.idle && creatureInv.gunItem == null) || (aiCmp.isTwoWeapon && aiCmp.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget && aiCmp.teammatesCount > 1)) && !isHealing)
        {
            for (int i = 0; i < interest.Length; i++)
            {
                result = Vector2.Dot(directionToTarget.normalized, _sceneData.Value.eightDirections[i]);

                if (result > 0)
                {
                    float valueToPutIn = result;
                    if (valueToPutIn > interest[i])
                        interest[i] = valueToPutIn;
                }
            }
            if (!moveCmp.isRun && moveCmp.currentRunTime > 1)
                moveCmp.isRun = true;
        }
        else if ((aiCmp.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget && (!aiCmp.isTwoWeapon || (aiCmp.isTwoWeapon && aiCmp.teammatesCount <= 1)) || isHealing) && danger.Max() < 0.8f)
        {
            for (int i = 0; i < interest.Length; i++)
            {
                int needDirection = i;
                if (needDirection >= 4)
                    needDirection -= 4;
                else
                    needDirection += 4;

                result = Vector2.Dot(directionToTarget.normalized, _sceneData.Value.eightDirections[i]);

                interest[needDirection] = result > 0 ? result : 0;
            }
        }
        else if (aiCmp.currentState == CreatureAIComponent.CreatureStates.shootingToTarget || ((aiCmp.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget || (creatureInv.healingItem != null && _healingItemComponentsPool.Value.Get(aiEntity).isHealing)) && danger.Max() >= 0.8f))
        {
            for (int i = 0; i < interest.Length; i++)
            {
                int needDirection = i;
                if (aiCmp.isLeftMoveCircle)
                {
                    if (needDirection > 1)
                        needDirection -= 2;
                    else
                        needDirection += 6;
                }
                else
                {
                    if (needDirection < 6)
                        needDirection += 2;
                    else
                        needDirection -= 6;
                }

                result = Vector2.Dot(directionToTarget.normalized, _sceneData.Value.eightDirections[i]);

                interest[needDirection] = result > 0 ? result : 0;
            }
        }

        if (aiCmp.isTwoWeapon )
        {
            if (((aiCmp.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget && !_creatureInventoryComponentsPool.Value.Get(aiEntity).isSecondWeaponUsed && aiCmp.teammatesCount > 1) || (aiCmp.currentState == CreatureAIComponent.CreatureStates.shootingToTarget && _creatureInventoryComponentsPool.Value.Get(aiEntity).isSecondWeaponUsed)) && !_creatureChangeWeaponEventsPool.Value.Has(aiEntity) && !isHealing)
            {
                _creatureChangeWeaponEventsPool.Value.Add(aiEntity);
            }
        }

        if (aiCmp.currentState == CreatureAIComponent.CreatureStates.shootingToTarget && aiCmp.timeFromLastChangeLeftMoveCircle > 1 && danger.Max() >= 0.8f)
        {
            aiCmp.isLeftMoveCircle = !aiCmp.isLeftMoveCircle;
            aiCmp.timeFromLastChangeLeftMoveCircle = 0;
        }
        else
            aiCmp.timeFromLastChangeLeftMoveCircle += Time.deltaTime;
        /*  if (aiCmp.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget && danger.Max() >= 0.2f)
          {
              int maxDanger = Array.IndexOf(danger, danger.Max());//тут продолжить

              interest = new float[8];


              int curFirstNumToCount = 0;
              if (maxDanger != 7)
                  curFirstNumToCount = maxDanger + 1;

              int curSecondNumToCount = 7;
              if (maxDanger != 0)
                  curSecondNumToCount = maxDanger - 1;

              float minDangerValue = danger.Min();
              while (danger[curFirstNumToCount] != minDangerValue || danger[curSecondNumToCount] != minDangerValue)
              {
                  if (curFirstNumToCount != 7)
                      curFirstNumToCount++;
                  else
                      curFirstNumToCount = 0;

                  if (curSecondNumToCount != 0)
                      curSecondNumToCount--;
                  else
                      curSecondNumToCount = 7;

                  Debug.Log(minDangerValue + " minDanger");

                  if (danger[curFirstNumToCount] == minDangerValue)
                  {
                      interest[curFirstNumToCount] = 1;
                      break;
                  }

                  else if (danger[curSecondNumToCount] == minDangerValue)
                  {
                      interest[curSecondNumToCount] = 1;
                      break;
                  }

              }
          }*/

        aiCmp.interstsTemp = interest;


        //  if(aiCmp.targetPositionCached != null)
        // Gizmos.DrawSphere(aiCmp.targetPositionCached, 0.2f);

        if (aiCmp.interstsTemp != null)
        {
            for (int i = 0; i < aiCmp.interstsTemp.Length; i++)
            {
                Debug.DrawRay(creaturePosition, _sceneData.Value.eightDirections[i] * aiCmp.interstsTemp[i] * 2, Color.green);
            }
            if (aiCmp.reachedLastTarget == false)
            {
                //Debug.DrawSphere(aiCmp.targetPositionCached, 0.1f);
            }
        }



        return (danger, interest);
    }
    public (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, int aiEntity)
    {
        ref var aiCmp = ref _creatureAIComponentsPool.Value.Get(aiEntity);
        ref var moveCmp = ref _movementComponentsPool.Value.Get(aiEntity);

        foreach (Collider2D obstacleCollider in aiCmp.obstacles)
        {
            Vector2 directionToObstacle = obstacleCollider.ClosestPoint(moveCmp.entityTransform.position) - (Vector2)moveCmp.entityTransform.position;
            float distanceToObstacle = directionToObstacle.magnitude;
            float radius = 2f;

            float weight = distanceToObstacle <= 0.6f ? 1 : (radius - distanceToObstacle) / radius;

            Vector2 directionToObstacleNormalized = directionToObstacle.normalized;

            var directions = _sceneData.Value.eightDirections;
            for (int i = 0; i < directions.Count; i++)
            {
                float result = Vector2.Dot(directionToObstacleNormalized, directions[i]);
                float valueToPutIn = result * weight;

                if (valueToPutIn > danger[i])
                    danger[i] = valueToPutIn;
            }
        }
        aiCmp.dangersResultTemp = danger;

        for (int i = 0; i < aiCmp.dangersResultTemp.Length; i++)
        {
            Debug.DrawRay(moveCmp.entityTransform.position, _sceneData.Value.eightDirections[i] * aiCmp.dangersResultTemp[i], Color.red);
        }
        return (danger, interest);
    }
    public void Detect(int entity)
    {
        ref var aiCmp = ref _creatureAIComponentsPool.Value.Get(entity);
        ref var moveCmp = ref _movementComponentsPool.Value.Get(entity);

        Collider2D[] obstaclesColliders = Physics2D.OverlapCircleAll(moveCmp.entityTransform.position, /*расстояние нахождения препятствий*/1.2f, LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Enemy"));
        aiCmp.teammatesCount = Physics2D.OverlapCircleAll(moveCmp.entityTransform.position, /*расстояние нахождения препятствий*/8f, LayerMask.GetMask("Enemy")).Length;
        aiCmp.obstacles = obstaclesColliders;

        Collider2D playerCollider = Physics2D.OverlapCircle(moveCmp.entityTransform.position, aiCmp.followDistance, LayerMask.GetMask("Player")/*каких нибудь животных добавить*/);

        if (playerCollider != null)
        {
            var playerTransform = _movementComponentsPool.Value.Get(_sceneData.Value.playerEntity).movementView.transform;
            var needPlayerPosition = new Vector3(playerTransform.position.x, playerTransform.position.y - 0.2f);
            Vector2 direction = (needPlayerPosition - moveCmp.entityTransform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(moveCmp.entityTransform.position, direction, aiCmp.followDistance, LayerMask.GetMask("Obstacle", "Player", "InteractedCharacter"/*, "Enemy"*/));

            RaycastHit2D hitSightOnTarget = Physics2D.Raycast(moveCmp.entityTransform.position, aiCmp.creatureView.movementView.weaponContainer.up, aiCmp.followDistance, LayerMask.GetMask("Obstacle", "Player", "InteractedCharacter"/*, "Enemy"*/));//если что перенести в ран
            aiCmp.sightOnTarget = hitSightOnTarget.collider != null && hitSightOnTarget.collider.gameObject.layer == 6;
            float distanceToTarget = Vector2.Distance(needPlayerPosition, moveCmp.entityTransform.position);
            if (hit.collider != null && (LayerMask.GetMask("Player") & (1 << hit.collider.gameObject.layer)) != 0)
            {
                if (aiCmp.colliders == null)
                {
                    Collider2D[] closestEnemies = Physics2D.OverlapCircleAll(moveCmp.entityTransform.position, 6f, LayerMask.GetMask("Enemy"));
                    foreach (var enemy in closestEnemies)
                    {
                        int enemyEntity = enemy.gameObject.GetComponent<HealthView>()._entity;
                        if (enemyEntity != entity)
                        {
                            ref var teammateAiCmp = ref _creatureAIComponentsPool.Value.Get(enemyEntity);
                            if (teammateAiCmp.targets == null)
                            {
                                var dir = (playerCollider.transform.position - enemy.transform.position).normalized;
                                float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                                if (n < 0)
                                    n += 360;

                                float angleRad = n * (Mathf.PI / 180f);

                                RaycastHit2D hitOnTarget = Physics2D.Raycast(enemy.transform.position, new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)), teammateAiCmp.followDistance * 4, LayerMask.GetMask("Obstacle", "Player"/*, "Enemy"*/));

                                if (hitOnTarget.collider != null && hitOnTarget.collider.gameObject.layer == 6)
                                {
                                    teammateAiCmp.targets = new List<Transform>() { playerCollider.transform };
                                    teammateAiCmp.targetPositionCached = playerCollider.transform.position;
                                    teammateAiCmp.reachedLastTarget = false;
                                    if (teammateAiCmp.currentState == CreatureAIComponent.CreatureStates.idle)
                                    {
                                        teammateAiCmp.timeFromLastTargetSeen = 0f;
                                        teammateAiCmp.currentState = CreatureAIComponent.CreatureStates.follow;
                                    }
                                }
                            }
                        }
                    }
                }
                aiCmp.colliders = new List<Transform>() { playerCollider.transform };
                //Debug.DrawRay(moveCmp.entityTransform.position, direction * creatureAiCmp.followDistance, Color.green);
                if (distanceToTarget <= aiCmp.minSafeDistance)
                    aiCmp.currentState = CreatureAIComponent.CreatureStates.runAwayFromTarget;

                else if (distanceToTarget <= aiCmp.safeDistance && !aiCmp.isPeaceful)
                    aiCmp.currentState = CreatureAIComponent.CreatureStates.shootingToTarget;

                else if (distanceToTarget <= aiCmp.followDistance && !aiCmp.isPeaceful)
                    aiCmp.currentState = CreatureAIComponent.CreatureStates.follow;

                if (aiCmp.timeFromLastTargetSeen != 0)
                    aiCmp.timeFromLastTargetSeen = 0;

                aiCmp.reachedLastTarget = true;
            }
            else //не видит игрока за стенкой, но он в зоне досигаемости
            {
                aiCmp.colliders = null;

                if (aiCmp.currentState == CreatureAIComponent.CreatureStates.runAwayFromTarget)
                {
                    if (distanceToTarget >= aiCmp.safeDistance)
                        aiCmp.currentState = CreatureAIComponent.CreatureStates.shootingToTarget;
                }

                else if (aiCmp.currentState == CreatureAIComponent.CreatureStates.shootingToTarget)
                {
                    if (aiCmp.timeFromLastChangeLeftMoveCircle >= 1)
                    {
                        aiCmp.isLeftMoveCircle = !aiCmp.isLeftMoveCircle;
                        aiCmp.timeFromLastChangeLeftMoveCircle = 0;
                    }
                    else
                        aiCmp.timeFromLastChangeLeftMoveCircle += Time.deltaTime;
                }
                // aiCmp.currentState = CreatureAIComponent.CreatureStates.follow;
                if (aiCmp.currentState != CreatureAIComponent.CreatureStates.idle)
                {
                    if (aiCmp.reachedLastTarget)
                    {
                        aiCmp.currentState = CreatureAIComponent.CreatureStates.idle;
                        aiCmp.timeFromLastTargetSeen = 0;
                        //    Debug.Log("not detected targets and set idle state");

                    }
                    else if (aiCmp.timeFromLastTargetSeen >= 1.5f)
                    {
                        aiCmp.reachedLastTarget = true;
                    }
                    else
                        aiCmp.timeFromLastTargetSeen += Time.deltaTime;

                }
                Debug.DrawRay(moveCmp.entityTransform.position, direction * aiCmp.followDistance, Color.red);
                //     Debug.Log("Player now not detect");
            }
        }
        else
        {
            if (aiCmp.currentState != CreatureAIComponent.CreatureStates.idle)
            {
                if (aiCmp.reachedLastTarget)
                {
                    aiCmp.currentState = CreatureAIComponent.CreatureStates.idle;
                    aiCmp.timeFromLastTargetSeen = 0;

                }
                else if (aiCmp.timeFromLastTargetSeen >= 1.5f)
                    aiCmp.reachedLastTarget = true;
                else
                    aiCmp.timeFromLastTargetSeen += Time.deltaTime;
            }


            aiCmp.colliders = null;
        }
        aiCmp.targets = aiCmp.colliders;
    }


    /*  private void CheckPlayerDistance(int aiEntity, ref CreatureAIComponent aiEntityCmp)
      {
          ref var moveCmp = ref _movementComponentsPool.Value.Get(aiEntity);
          if (_healingItemComponentsPool.Value.Has(aiEntity) && _healingItemComponentsPool.Value.Get(aiEntity).isHealing) return;

          //тут потом менять
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
                  else if (distanceBetweenPlayer >= aiEntityCmp.safeDistance * 1.15f)
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

      }*/
}
