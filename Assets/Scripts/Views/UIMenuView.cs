using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuView : MonoBehaviour
{
    [SerializeField]private Animator menuAnimator;
    [SerializeField] private Transform hidedUI;

    private void Awake()
    {
        if (menuAnimator == null)
            menuAnimator = GetComponent<Animator>();
        if(hidedUI == null)
            hidedUI = gameObject.transform.GetChild(0);
    }

    public void ChangeMenuState(bool isOpen)
    {
        menuAnimator.SetBool("isShowed", isOpen);
    }

    public void ChangeIsActiveStateToFalse()
    {
        hidedUI.gameObject.SetActive(false);
    }
    public void ChangeIsActiveStateToTrue()
    {
        hidedUI.gameObject.SetActive(true);
    }
}
