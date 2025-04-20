using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BulletInfo", menuName = "ScriptableObjects/BulletInfo", order = 4)]
public class BulletInfo : ScriptableObject
{
    public float addedSpreadMultiplayer;
    public float addedLenghtMultiplayer;
    public float addedBleedingMultiplayer;
    public float addedStunMultiplayer;
    public float addedDamageMultiplayer;
    public float removedGunDurability;
    public int bulletCount = 1;
    public BulletType bulletType;
    public Sprite bulletCase;
    public Color uiBulletColor;

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
