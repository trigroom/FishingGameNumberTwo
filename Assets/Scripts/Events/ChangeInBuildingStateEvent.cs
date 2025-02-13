using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChangeInBuildingStateEvent 
{
    public SpriteGroupView roofSpriteRenderer;
    public bool isHideRoof;

    public ChangeInBuildingStateEvent(bool _isHideRoof, SpriteGroupView _spriteRenderer)
    {
        isHideRoof = _isHideRoof;
        roofSpriteRenderer = _spriteRenderer;
    }
}
