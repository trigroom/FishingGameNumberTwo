using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerComponent 
{
    public PlayerView view;
    public bool useFlashlight{ get;set; }
    public bool canDeffuseMines;
    public bool hasForestGuide;
    public bool nvgIsUsed ;
    public int levelOfPainkillers;
    public float currentAudibility;
    public float underNightLightTime;
    public float underNightLightRadius;
    // public SpriteRenderer visionZoneMask;
    //public PolygonCollider2D visionZoneCollider;
}
