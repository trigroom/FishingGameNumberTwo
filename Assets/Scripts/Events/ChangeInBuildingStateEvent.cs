using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChangeInBuildingStateEvent 
{
    public TilemapsGroup roofTilemaps;
    public bool isHideRoof;

    public ChangeInBuildingStateEvent(bool _isHideRoof, TilemapsGroup _tilemaps)
    {
        isHideRoof = _isHideRoof;
        roofTilemaps = _tilemaps;
    }
}
