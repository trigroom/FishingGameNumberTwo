using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BulletInfo", menuName = "ScriptableObjects/BulletInfo", order = 4)]
public class BulletInfo : ItemInfo
{
    public enum bulletType
    {
        pistolBullet,
        rifleBullet,
        sniperBullet,
        shotgunBullet
    }
}
//мб не понадобитс€, нужно лишь записать id патронов
