using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [field: SerializeField] public MovementView movementView { get; private set; }
}
