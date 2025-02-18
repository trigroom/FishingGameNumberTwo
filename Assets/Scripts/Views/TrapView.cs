using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapView : MonoBehaviour
{
    //public bool isActivated;
    public TrapType type;
    public SpriteRenderer spriteRenderer;
    public Sprite safetyTrapSprite;
    public float neutralizeTime;
    public EffectInfo[] effects;
    public Collider2D trapCollider;
    public int entity;

    public GrenadeInfo mineInfo;
   public enum TrapType
    {
        mantrap,
        mine
    }
}
