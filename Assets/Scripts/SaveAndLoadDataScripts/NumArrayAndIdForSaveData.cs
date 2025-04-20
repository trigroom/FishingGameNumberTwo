[System.Serializable]
public struct NumArrayAndIdForSaveData
{
    public int[] numArray;
    public int cellId;

    public NumArrayAndIdForSaveData(int _id, int[] _num)
    {
        numArray = _num;
        cellId = _id;
    }
}