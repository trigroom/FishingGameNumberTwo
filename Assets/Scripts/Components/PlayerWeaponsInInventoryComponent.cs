using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerWeaponsInInventoryComponent
{
    public GunInfo gunFirstObject;
    public GunInfo gunSecondObject;
    public GameObject meleeWeaponObject;

    public int curFirstWeaponAmmo;
    public int curSecondWeaponAmmo;
    public int curEquipedWeaponsCount;

    public int curWeapon;
}
