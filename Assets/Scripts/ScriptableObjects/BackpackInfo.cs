using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BackpackInfo", menuName = "ScriptableObjects/BackpackInfo", order = 1)]
public class BackpackInfo : ScriptableObject
{
    public Sprite backgroundSprite;
    public int cellsCount;
    public float yPosition;
    public Vector2 backgroundSize;
}
