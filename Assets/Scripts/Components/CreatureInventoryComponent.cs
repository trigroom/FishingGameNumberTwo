using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CreatureInventoryComponent 
{
    public bool isSecondWeaponUsed;//

    public ItemInfo meleeWeaponItem;
    public ItemInfo gunItem;
    public ItemInfo healingItem;
    public ItemInfo bodyArmorItem;
    public ItemInfo helmetItem;
    public ItemInfo shieldItem;
    public EnemyClassSettingsInfo enemyClassSettingInfo;
}
