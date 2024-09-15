using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroppedItemView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public int itemEntity {  get; private set; }

    public void SetParametersToItem(Sprite sprite, int entity)
    {
        spriteRenderer.sprite = sprite;
        itemEntity = entity;
    }
    public void DestroyItemFromGround()
    {
        Destroy(gameObject);
    }
}
