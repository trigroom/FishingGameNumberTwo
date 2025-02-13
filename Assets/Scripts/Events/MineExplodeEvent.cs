using UnityEngine;

public struct MineExplodeEvent
{
    public GrenadeInfo grenadeInfo;
    public Vector2 explodeSpawnPosition;

    public MineExplodeEvent(GrenadeInfo grenadeInfo, Vector2 spawnPos)
    {
        this.grenadeInfo = grenadeInfo;
        this.explodeSpawnPosition = spawnPos;
    }
}
