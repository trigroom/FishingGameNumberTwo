using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFromCreatureView : MonoBehaviour
{
    public int maxDroppedItemsCount;
    [field: SerializeField] public CreatureDropElement[] droopedItems { get; private set; }//������� ������ � ������� �� ���� ���� �����������

}
