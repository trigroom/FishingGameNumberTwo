using UnityEngine;

public struct MeleeWeaponContactEvent
{
    public int attackedEntity;
    public bool isShield;
    public bool isHeadshot;
    public Vector2 contactPosition;
    public MeleeWeaponContactEvent(int attackedEntity, bool isShield, bool headshot, Vector2 contactPos)
    {
        this.attackedEntity = attackedEntity; 
        this.isShield = isShield;
        isHeadshot = headshot;
        contactPosition = contactPos;
    }
}
