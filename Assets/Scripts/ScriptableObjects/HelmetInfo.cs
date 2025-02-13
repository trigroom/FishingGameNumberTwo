using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HelmetInfo", menuName = "ScriptableObjects/HelmetInfo", order = 1)]
public class HelmetInfo : ScriptableObject
{
    public Sprite helmetSprite;
    public int armorDurability;
    public float headDefenceMultiplayer;
    public Vector2 inGamePositionOnPlayer;
    public int hairSpriteIndex;
    public float audibilityMultiplayer = 1;

    public float fowAngleRemove;
    public float fowLenghtRemove;

    public bool autoBloodCleaning;
    public float dropTransparentMultiplayer;

    public int nightTimeModeDuration;
    public float addedLightIntancity;
    public float addedBloom;
    public Color visionColor;

    public int recoveryCost;
}
