using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LocationInfo", menuName = "ScriptableObjects/LocationInfo", order = 1)]
public class LocationSettingsInfo : ScriptableObject
{
    public LevelSettingsInfo[] levels;
    public string locationName;
    public int[] shoppers;
    public int[] nightShoppers;
}
