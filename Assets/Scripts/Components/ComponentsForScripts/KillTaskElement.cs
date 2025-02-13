using UnityEngine;
[System.Serializable]
public class KillTaskElement
{
    public int enemyGunType;//0-5 guns , 6 melee, 7 none
    public int playerGunType;
    public float maxDistanceToKill;
    public float minDistanceToKill;
    public bool isHeadshotKill;
    public int minTimeToKill;
    public int maxTimeToKill;
    public string locationToKill;
}
