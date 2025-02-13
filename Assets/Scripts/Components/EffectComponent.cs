using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EffectInfo;

public struct EffectComponent
{
    public float effectDuration;
    public Sprite effectIconSprite;
    public int effectLevel;
    public bool isFirstEffectCheck;
    public EffectType effectType;
    public int effectEntity;
    public EffectIconView effectIconView;
}
