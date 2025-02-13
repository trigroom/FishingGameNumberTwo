using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SheildInfo", menuName = "ScriptableObjects/SheildInfo", order = 1)]
public class SheildInfo : ScriptableObject
{
    public Sprite sheildSprite;//добавить спрайт полуразрушенного
    public Vector2 sheildColliderScale;
    public Vector2 sheildInHandsPosition;
    public Vector2 sheildColliderPositionOffset;
    public int sheildDurability;
    public int sheildRecoveryCost;
    public float damagePercent;
    public float recoilPercent;//от 1 до 0, отдача умножается на это число, поэтому чем ближе к еденице тем меньше изменений
}
