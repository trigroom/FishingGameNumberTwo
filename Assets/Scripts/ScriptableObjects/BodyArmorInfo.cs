using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BodyArmorInfo", menuName = "ScriptableObjects/BodyArmorInfo", order = 1)]
public class BodyArmorInfo : ScriptableObject
{
    public Sprite bodyArmorSprite;
    public int armorDurability;
    public float defenceMultiplayer;
    public Vector2 inGamePositionOnPlayer;

    public float bleedingResistance;

    public int recoveryCost;
}
