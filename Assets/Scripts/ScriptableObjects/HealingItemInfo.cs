using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HealingItemInfo", menuName = "ScriptableObjects/HealingItemInfo", order = 2)]

public class HealingItemInfo : ScriptableObject
{
    public int healingHealthPoints;
    public int recoveringHungerPoints;
    public float healingTime;
    public int maxBleedingRemoveLevel;
    public float addedBlur;
    public EffectInfo effectInfo;

    public Sprite inGameHealingItemSprite;
    public float scaleMultplayer;
    public float rotationZ;

}
