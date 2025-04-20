using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GunInventoryCellComponent 
{
    public int[] gunPartsId;
    public bool isEquipedWeapon ;
    public List<int> currentAmmo{ get; set; }
    public float gunDurability;

    public List<int> bulletShellsToReload;

    public float currentGunWeight;
}
