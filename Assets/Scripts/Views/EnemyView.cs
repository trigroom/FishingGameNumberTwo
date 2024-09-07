using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    [field: SerializeField] public ItemInfo droopedItem { get; private set; }//сделать список с шансами на дроп если понадобится

}
