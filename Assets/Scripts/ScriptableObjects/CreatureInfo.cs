using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CreatureInfo", menuName = "ScriptableObjects/CreatureInfo", order = 5)]
public class CreatureInfo : ScriptableObject
{
    public string creatureName;
    public Transform creaturePrafab;
}
