using UnityEngine;

[CreateAssetMenu(fileName = "EnemyClassSettingsInfo", menuName = "Scriptable Objects/EnemyClassSettingsInfo")]
public class EnemyClassSettingsInfo : ScriptableObject
{
    public int healthPoint;
    public float staminaMultiplayer;
    public float visionLenghtMultiplayer;
    public float movementSpeed;
    public float dropPercentMultiplayer;
    public float recoverySpreadMultiplayer;
    public float needSightOnTargetTime;
    public float weaponRotationSpeedMultiplayer;
    public int expPointsForKill;
}
