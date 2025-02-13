using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ParticlesInfo", menuName = "ScriptableObjects/ParticlesInfo", order = 4)]
public class ParticleSettingsInfo : ScriptableObject
{
    public float startSpeedMin, startSpeedMax;
    public float startLifetimeMin, startLifetimeMax;
    public float startSizeMin, startSizeMax;
    public Color startColor;
    public Sprite particleSprite;
}
