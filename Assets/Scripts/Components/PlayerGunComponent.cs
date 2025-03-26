using System.Collections.Generic;
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

    public float currentCameraSpreadDifference; //� ����� ��?

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
