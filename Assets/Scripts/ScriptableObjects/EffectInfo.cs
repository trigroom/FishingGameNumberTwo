using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EffectInfo", menuName = "ScriptableObjects/EffectInfo", order = 1)]
public class EffectInfo : ScriptableObject
{
    public Sprite effectIconSprite;
    public int effectLevel;
    public float effectTime;
    public EffectType effectType;
    public enum EffectType
    {
        mantrap,
        bleeding,
        painkillers,
        cheerfulness
    }

}
