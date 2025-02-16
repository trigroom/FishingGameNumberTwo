using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidedOutsidePlayerFovView : MonoBehaviour
{
    public Transform[] objectsToHide;

    private void Awake()
    {
        if(objectsToHide == null || objectsToHide.Length == 0)
            objectsToHide = new Transform[] { gameObject.transform.GetChild(0)};

        foreach(var obj in objectsToHide)
            obj.gameObject.SetActive(false);
    }
}
