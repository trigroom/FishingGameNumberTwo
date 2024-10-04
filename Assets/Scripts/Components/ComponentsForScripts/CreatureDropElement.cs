using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct CreatureDropElement
{
    [field: SerializeField] public ItemInfo droopedItem { get; private set; }
    [field: SerializeField] public int itemsCountMax { get; private set; }
    [field: SerializeField] public int itemsCountMin { get; private set; }
    [field: SerializeField] public int dropPercent { get; private set; }
}
