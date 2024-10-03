public struct PlayerGunComponent
{
    public bool isAuto;
    public int bulletCountToReload;
    public int bulletTypeId;
    public bool isContinueReload;

    public float maxSpread;//убрать к ганкомпоненту игрока
    public float minSpread;//убрать к ганкомпоненту игрока
    public float addedSpread;//убрать к ганкомпоненту игрока

    public int scopeMultiplicity;
    public bool inScope;
}
