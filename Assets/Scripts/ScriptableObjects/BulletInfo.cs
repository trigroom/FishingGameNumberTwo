using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BulletInfo", menuName = "ScriptableObjects/BulletInfo", order = 4)]
public class BulletInfo : ItemInfo
{
    public float addedSpreadMultiplayer;
    public float addedLenghtMultiplayer;
    public float addedDamageMultiplayer;
    public int bulletCount = 1;
    public BulletType bulletType;

    public enum BulletType
    {
        bullet38special,
        bullet357magnum,
        bullet12gauge,
        bullet9x19,
        bullet556x45,
        bullet762Rus,
        bullet762Nato,
    }
}
//мб не понадобитс€, нужно лишь записать id патронов
