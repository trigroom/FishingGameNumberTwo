using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementView : MonoBehaviour
{
    [field: SerializeField] public float moveSpeed { get; private set; }
    [field: SerializeField] public float weaponCentre { get; private set; }
    [field: SerializeField] public Transform objectTransform { get; private set; }
    [field: SerializeField] public Transform firePoint { get; private set; }
    [field: SerializeField] public Transform characterSpriteTransform { get; private set; }
    [field: SerializeField] public Transform weaponContainer { get; private set; }
    [field: SerializeField] public Transform nonWeaponContainer { get; private set; }
    [field: SerializeField] public Transform weaponSprite { get; private set; }
    [field: SerializeField] public float offsetToWeapon { get; private set; }
    [field: SerializeField] public SpriteRenderer weaponSpriteRenderer { get; private set; }
    [field: SerializeField] public AudioSource weaponAudioSource { get; private set; }
    [field: SerializeField] public float weaponRotateSpeed { get; private set; }
    [field: SerializeField] public Animator characterAnimator { get; private set; }
    [field: SerializeField] public SpriteRenderer bodyArmorSpriteRenderer { get; private set; }
    [field: SerializeField] public SpriteRenderer helmetSpriteRenderer { get; private set; }
    [field: SerializeField] public SpriteRenderer hairSpriteRenderer { get; private set; }
    [field: SerializeField] public Collider2D weaponCollider { get; private set; }

    public void MoveUnit(Vector2 direction)
    {
        transform.Translate(direction);
    }

    public void RotateWeaponCentre(Quaternion needRotation)
    {
        weaponContainer.rotation = needRotation;
    }
    public void RotateNonWeaponCentre(Quaternion needRotation)
    {
        nonWeaponContainer.rotation = needRotation;
    }
}
