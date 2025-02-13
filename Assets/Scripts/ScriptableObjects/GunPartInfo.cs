using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunPartInfo", menuName = "ScriptableObjects/GunPartInfo", order = 2)]
public class GunPartInfo : ScriptableObject
{
    public GunPartType gunPartType;

    [Header("Stats")]
    public float spreadMultiplayer;
    public float cameraSpreadMultiplayer;
    public float weaponChangeSpeedMultiplayer;
    public float attackLenght;
    public float reloadSpeedMultiplayer;
    public int neededLevelToEquip;
    [Header("Visual")]
    public Sprite spriteForAddOnGunModel;
    [Header("Scope")]
    public float scopeMultiplicity;
    public Sprite centreScopeSprite;
    public float blackBGSize;
    [Header("LaserPointer")]
    public float laserMaxLenght;
    public Vector2 laserPosition;
    public float laserLightTime;
    public float energyToCharge;
    public Gradient laserColor;
    public enum GunPartType
    {
        butt,
        scope,
        downPart,
        barrelPart
    }
}
