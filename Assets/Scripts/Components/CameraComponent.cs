using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraComponent 
{
    public float cursorPositonPart { get; set; }
    public float playerPositonPart;
    public float blurValue { get; set; }

    public Vector2 currentRunCameraOffset;
    public Vector2 needRunCameraOffset;
    public Vector2 lastPlayerInput;
    public float runCameraOffsetLenght;

    public float currentMaxCameraSpread;
    public float currentRecoveryCameraSpread;
}
