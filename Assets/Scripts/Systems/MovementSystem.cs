using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class MovementSystem : IEcsRunSystem
{
    //private EcsCustomInject<SceneService> _sceneData;
    private EcsPoolInject<MovementComponent> _movementComponentPool;
    //private EcsPoolInject<HealthComponent> _healthComponentsPool;
    private EcsPoolInject<MeleeWeaponComponent> _meleeWeaponComponentsPool;
    private EcsPoolInject<PlayerWeaponsInInventoryComponent> _playerWeaponsInInventoryComponentsPool;
    private EcsPoolInject<PlayerComponent> _playerComponentsPool;

    private EcsFilterInject<Inc<MovementComponent>> _movementComponentFilter;

    private EcsCustomInject<SceneService> _sceneService;

    public void Run(IEcsSystems systems)
    {
        foreach (var movableObject in _movementComponentFilter.Value)
        {
            //if (!_movementComponentPool.Value.Has(movableObject)) continue;
            ref var moveCmp = ref _movementComponentPool.Value.Get(movableObject);

            if (moveCmp.isStunned)
            {
                moveCmp.stunTime -= Time.deltaTime;
                if (moveCmp.stunTime <= 0)
                {
                    moveCmp.stunTime = 0;
                    moveCmp.isStunned = false;
                    moveCmp.moveSpeed = moveCmp.movementView.moveSpeed;//временно и для игрока другое будет уравнение
                    moveCmp.moveInput = Vector2.zero;
                }
            }

            if (moveCmp.canMove)
                moveCmp.movementView.MoveUnit(moveCmp.moveSpeed * moveCmp.moveInput * Time.deltaTime);

            if (_meleeWeaponComponentsPool.Value.Has(movableObject) && _meleeWeaponComponentsPool.Value.Get(movableObject).isHitting)  continue; //возможно что то поправить

            Vector2 direction = (moveCmp.pointToRotateInput - (Vector2)moveCmp.entityTransform.position).normalized;
            float rotateZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            moveCmp.movementView.RotateWeaponCentre(rotateZ);

            if (_playerComponentsPool.Value.Has(movableObject))
            {
                ref var playerCmp = ref _playerComponentsPool.Value.Get(movableObject);
            playerCmp.view.playerVisionZoneTransform.rotation = Quaternion.Euler(0f, 0f, rotateZ + playerCmp.view.movementView.offsetToWeapon);
            }
            /* Vector2 scale = transform.localScale;
             if (direction.x < 0)
             {
                 scale.x = 1;
             }
             else if (direction.x > 0)
             {
                 scale.x = -1;
             }

             transform.localScale = scale;

             Vector3 scale = transform.localScale;
             if (rotationDegrees < 180)
             {
                 isLeft = true;
                 scale.x = 1;
             }
             else
             {
                 isLeft = false;
                 scale.x = -1;
             }
             transform.localScale = scale;*/
        }
        //передвижение объекта
    }

}
