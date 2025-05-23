using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelInfo", menuName = "ScriptableObjects/LevelInfo", order = 1)]
public class LevelSettingsInfo : ScriptableObject
{
    public LevelSceneView levelPrefab;
    public LevelTarget levelTarget;
   // public EnemyForSpawnInfoElement[] enemies;
    public ItemInfo[] bodyArmorsInfo;
    public ItemInfo[] helmetsInfo;
    public ItemInfo[] gunsInfo;
    public ItemInfoArrayElement[] bulletsInfoArray = new ItemInfoArrayElement[7];
    public ItemInfo[] meleeWeaponsInfo;
    public ItemInfo[] healingItemsInfo;
    public ItemInfo[] shieldItemsInfo;
    public float chanceToOneWeapon;
    public float chanceToMeleeWeapon;
    public float chanceToHealingItem;
    public float chanceToBodyArmorItem;
    public float chanceToHelmetItem;
    public float chanceToShieldItem;
    public Transform[] traps;
    public int enemiesCount;
    public int trapsCount;
    public int shoppersCount;
    public bool hasStartLocationEntry;

    public float[] chancesToEnemyClass;
    public EnemyClassSettingsInfo[] enemyClassSettingsInfo;
    public CreatureDropElement[] droopedItems;

    public enum LevelTarget
    {
        escape,
        killAll,
        escapeForTime,
        interactWithSomething
    }
}
