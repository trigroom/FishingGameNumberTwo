using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementView : MonoBehaviour
{
    [field: SerializeField] public float moveSpeed { get; private set; }

    public void MoveUnit(Vector2 direction)
    {
        transform.Translate(direction);
    }
}
