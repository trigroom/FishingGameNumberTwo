using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using static InteractCharacterView;

public class PlayerView : MonoBehaviour
{
    [field: SerializeField] public HealthView healthView { get; private set; }
    [field: SerializeField] public MovementView movementView { get; private set; }
    [field: SerializeField] public PlayerInputView playerInputView { get; private set; }
    [field: SerializeField] public Light flashLight { get; private set; }
    public Vector2 GetPlayerPosition()
    {
        return gameObject.transform.position;
    }
}
