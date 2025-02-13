using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CreatureInventoryComponent 
{
    public bool isSecondWeaponUsed;//

    public MeleeWeaponInfo meleeWeaponItem;
    public GunInfo gunItem;
    public HealingItemInfo healingItem;
    public BodyArmorInfo bodyArmorItem;
    public HelmetInfo helmetItem;

    public EnemyClassSettingsInfo enemyClassSettingInfo;
}
