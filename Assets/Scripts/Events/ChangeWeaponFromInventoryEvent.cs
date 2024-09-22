using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChangeWeaponFromInventoryEvent
{
    public bool isDeleteWeapon;
    public int weaponCellNumberToChange;


    public void SetValues(bool isDelete, int weaponNum)
    {
        isDeleteWeapon = isDelete;
        weaponCellNumberToChange = weaponNum;
    }
}
