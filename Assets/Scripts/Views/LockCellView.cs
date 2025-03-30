using UnityEngine;

public class LockCellView : MonoBehaviour
{
    public Animator animator;
    public void LockActivate(bool isLock)
    {
        animator.SetBool("activateLocker", isLock);
    }
}