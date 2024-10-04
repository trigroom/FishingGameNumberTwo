public struct PlayerGunComponent
{
    public GunInfo gunInfo;

    public bool isAuto;
    public int bulletCountToReload;
    public int bulletTypeId;
    public bool isContinueReload;

    public int misfirePercent;
    public float durabilityGunMultiplayer;

    public float maxSpread;
    public float minSpread;
    public float addedSpread;

    public int durabilityPoints;

    public int scopeMultiplicity;
    public bool inScope;
}
