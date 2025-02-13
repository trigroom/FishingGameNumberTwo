using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuView : MonoBehaviour
{
    [SerializeField]private Animator menuAnimator;

    private void Awake()
    {
        menuAnimator = GetComponent<Animator>();
    }

    public void ChangeMenuState(bool isOpen)
    {
        menuAnimator.SetBool("isShowed", isOpen);
    }
}
