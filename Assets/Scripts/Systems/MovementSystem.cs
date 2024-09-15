using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class MovementSystem : IEcsRunSystem
{
    //private EcsCustomInject<SceneService> _sceneData;
    private EcsPoolInject<MovementComponent> _movementComponentPool;

    private EcsFilterInject<Inc<MovementComponent>> _movementComponentFilter;

    public void Run(IEcsSystems systems)
    {
        foreach (var movableObject in _movementComponentFilter.Value)
        {
            var moveCmp = _movementComponentPool.Value.Get(movableObject);
            moveCmp.movementView.MoveUnit(moveCmp.moveSpeed * moveCmp.moveInput * Time.deltaTime);


            Vector2 direction = (moveCmp.pointToRotateInput - (Vector2)moveCmp.entityTransform.position).normalized;
            float rotateZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            moveCmp.movementView.RotateWeaponCentre(rotateZ);

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
