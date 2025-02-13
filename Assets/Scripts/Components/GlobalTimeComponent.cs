using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GlobalTimeComponent
{
    public int currentDayTime;
    public int currentDay;
    public bool isNight;
    public bool lightStateIsChanged;
    public float changeGloabalLightTime;
    public float currentGlobalLightIntensity { get; set; }

    public float nightLightIntensity;
    public bool goToLightNight;

    public int levelsToRain;
    public bool changedToRain { get; set; }

    public float lastRainDropTime;
    // public float minutesToTimerTextCount;
}
