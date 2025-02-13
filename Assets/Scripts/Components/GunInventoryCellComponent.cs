using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GunInventoryCellComponent 
{
    public int[] gunPartsId;
    public bool isEquipedWeapon { get; set; }
    public int currentAmmo;
    public int gunDurability;

    public int bulletShellsToReload;

    public float currentGunWeight;
}
