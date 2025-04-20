using System.Collections.Generic;
using UnityEngine.UI;

public struct PlayerGunComponent
{
    public GunInfo gunInfo;

    public int bulletCountToReload { get; set; }
    public int bulletIdToreload;

    public bool isAuto;
    public bool isContinueReload;

    public int misfirePercent;
    public float durabilityGunMultiplayer;

    public float maxSpread;
    public float minSpread;
    public float addedSpread;

    public float currentCameraSpreadDifference; //а нужна ли?

    public float currentScopeMultiplicity;
    public bool changedInScopeState { get; set; }
    public float timeAfterChangedInScopeState;
    public float cameraOrtograficalSizeDifference;
    public bool inScope;

    public List<Image> bulletUIObjects;

    public float sumAddedSpreadMultiplayer;
    public float sumAddedCameraSpreadMultiplayer;
    public float sumAddedWeaponChangeSpeedMultiplayer;
    public float sumAddedAttackLenghtMultiplayer;
    public float currentShotSoundLenght;
    public float sumAddedReloadSpeedMultiplayer { get; set; }
}
