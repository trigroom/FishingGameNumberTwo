using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerWeaponsInInventoryComponent
{
    public GunInfo gunFirstObject;
    public GunInfo gunSecondObject;
    public MeleeWeaponInfo meleeWeaponObject;

    public int curFirstWeaponAmmo;
    public int curSecondWeaponAmmo;

    public int curFirstWeaponDurability;
    public int curSecondWeaponDurability;

    public int curEquipedWeaponsCount;

    public int curWeapon;
}
