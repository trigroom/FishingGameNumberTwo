using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FlashlightInfo", menuName = "ScriptableObjects/FlashlightInfo", order = 1)]
public class FlashlightInfo : ScriptableObject
{
    public float maxChargedTime;
    public bool isSpottedLight;
    public float lightRange;
    public float lightIntecnsity;
    public float spotAngle;
    public Color lightColor;
    public bool isElectric;
    public int chargeItem;//если электрическое то колво сжираемой энергии, иначе айди предмета для подзарядки
}
