using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MovementView : MonoBehaviour
{
    [field: SerializeField] public float moveSpeed { get; private set; }
    [field: SerializeField] public float weaponCentre { get; private set; }
    [field: SerializeField] public Transform objectTransform { get; private set; }
    [field: SerializeField] public Transform firePoint { get; private set; }
    [field: SerializeField] public Transform weaponContainer { get; private set; }
    [field: SerializeField] public float offsetToWeapon { get; private set; }

    public void MoveUnit(Vector2 direction)
    {
        transform.Translate(direction);
    }

    public void RotateWeaponCentre(float rotateZ)
    {
        weaponContainer.rotation = Quaternion.Euler(0f, 0f, rotateZ + offsetToWeapon);
    }

}
