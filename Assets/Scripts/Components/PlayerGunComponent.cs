public struct PlayerGunComponent
{
    public bool isAuto;
    public int bulletCountToReload;
    public int bulletTypeId;
    public bool isContinueReload;

    public float maxSpread;//������ � ������������� ������
    public float minSpread;//������ � ������������� ������
    public float addedSpread;//������ � ������������� ������

    public int scopeMultiplicity;
    public bool inScope;
}
