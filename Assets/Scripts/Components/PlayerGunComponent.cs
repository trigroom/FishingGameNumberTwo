using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PlayerGunComponent
{
    public GunInfo gunInfo;

    public bool isAuto;
    public int bulletCountToReload;
    public int bulletTypeId;
    public bool isContinueReload;

    public int misfirePercent;
    public float durabilityGunMultiplayer;

    public float maxSpread;
    public float minSpread;
    public float addedSpread;

    public float currentCameraSpreadDifference; //а нужна ли?

    public float currentScopeMultiplicity;
    public bool changedInScopeState;
    public float timeAfterChangedInScopeState;
    public float cameraOrtograficalSizeDifference;
    public bool inScope { get; set; }

    public List<Image> bulletUIObjects;

    public float sumAddedSpreadMultiplayer;
    public float sumAddedCameraSpreadMultiplayer;
    public float sumAddedWeaponChangeSpeedMultiplayer;
    public float sumAddedAttackLenght;
    public float sumAddedReloadSpeedMultiplayer{ get; set; }
}
