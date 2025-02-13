[System.Serializable]
public struct NumAndIdForSafeData
{
    public int num;
    public int cellId;

    public NumAndIdForSafeData(int _id, int _num)
    {
        num = _num;
        cellId = _id;
    }
}
