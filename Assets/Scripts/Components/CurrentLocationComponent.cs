using System.Collections.Generic;
using UnityEngine;

public struct CurrentLocationComponent
{
    public int levelNum;
    public LocationSettingsInfo currentLocation;
    public Transform currentLevelPrefab;
    public List<Vector2> currentEnemySpawns;
    public Transform[] trapsPrefabs;
}
