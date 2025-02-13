using UnityEngine;
[System.Serializable]
public class GunLevelInfoElement 
{
    public int weaponExpLevel;
    public float weaponCurrentExp;

    public GunLevelInfoElement(int _weaponCurrentExp, int _weaponExpLevel)
    {
        weaponExpLevel = _weaponExpLevel;
        weaponCurrentExp = _weaponCurrentExp;
    }
}
