using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GrenadeInfo", menuName = "ScriptableObjects/GrenadeInfo", order = 2)]
public class GrenadeInfo : ScriptableObject
{
    [Header("Grenade settings")]
    public float explodeRadius;
    public int damage;
    public float maxThrowDistance;
    public float timeToExplode;
    public Sprite grenadeSprite;
    [Header("Audio settings")]
    public AudioClip explodeSound;
}
