using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerView : MonoBehaviour
{
    //[field: SerializeField] public Transform playerComponentsContainer { get; private set; }
    [field: SerializeField] public float maxCameraSpread { get; private set; }
    [field: SerializeField] public float recoveryCameraSpread { get; private set; }
    [field: SerializeField] public ShieldView shieldView { get; private set; }
    [field: SerializeField] public HealthView healthView { get; private set; }
    [field: SerializeField] public MovementView movementView { get; private set; }
    [field: SerializeField] public PlayerInputView playerInputView { get; private set; }
    [field: SerializeField] public MeleeWeaponColliderView meleeColliderView { get; private set; }
    //[field: SerializeField] public Light flashLight { get; private set; }
   // [field: SerializeField] public SpriteRenderer visionZoneSprite { get; private set; }
   // [field: SerializeField] public SpriteMask visionZoneSpriteMask { get; private set; }
    [field: SerializeField] public Light2D flashLightObject { get; private set; }
    [field: SerializeField] public float runTime { get; private set; }
    [field: SerializeField] public float runTimeRecoverySpeed { get; private set; }
    [field: SerializeField] public float runSpeedMultiplayer { get; private set; }
    [field: SerializeField] public Transform playerTransform { get; private set; }
    [field: SerializeField] public Transform firepointTransform { get; private set; }
    //[field: SerializeField] public Transform playerRotionWithoutWeaponTransform { get; private set; }
    //[field: SerializeField] public BoxCollider2D meleeWeaponCollider { get; private set; }
    //[field: SerializeField] public SpriteRenderer weaponSpriteRenderer { get; private set; }
  //  [field: SerializeField] public SpriteRenderer hairSpriteRenderer { get; private set; }
    //[field: SerializeField] public Transform weaponTransform { get; private set; }
    [field: SerializeField] public Transform laserPointerTransform { get; private set; }
    [field: SerializeField] public Light2D lightFromGunShot { get; private set; }
    [field: SerializeField] public float defaultFOV { get; private set; }//потом возможно для каждого оружия своя
    [field: SerializeField] public float viewDistance { get; private set; }
    [field: SerializeField] public LineRenderer laserPointerLineRenderer { get; private set; }
    [field: SerializeField] public Transform rightRecoilTracker { get; private set; }
    [field: SerializeField] public Transform leftRecoilTracker { get; private set; }
    [field: SerializeField] public float gunRecoverySpreadSpeed { get; private set; }
    [field: SerializeField] public float maxHungerPoints { get; private set; }
    [field: SerializeField] public float defaultHealthRecoverySpeed { get; private set; }
    // [field: SerializeField] public SpriteRenderer helmetSpriteRenderer { get; private set; }
    // [field: SerializeField] public SpriteRenderer bodyArmorSpriteRenderer { get; private set; }
}
