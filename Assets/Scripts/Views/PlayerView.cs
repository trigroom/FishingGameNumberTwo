using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [field: SerializeField] public HealthView healthView { get; private set; }
    [field: SerializeField] public MovementView movementView { get; private set; }
    [field: SerializeField] public PlayerInputView playerInputView { get; private set; }
    //[field: SerializeField] public Light flashLight { get; private set; }
    [field: SerializeField] public SpriteRenderer visionZoneSprite { get; private set; }
    [field: SerializeField] public SpriteMask visionZoneSpriteMask { get; private set; }
    [field: SerializeField] public Transform flashLightObject { get; private set; }
    [field: SerializeField] public float runTime { get; private set; }
    [field: SerializeField] public float runTimeRecoverySpeed { get; private set; }
    [field: SerializeField] public float runSpeedMultiplayer { get; private set; }
    public Vector2 GetPlayerPosition()
    {
        return gameObject.transform.position;
    }
}
