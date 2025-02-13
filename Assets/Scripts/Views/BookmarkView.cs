using UnityEngine;
using UnityEngine.EventSystems;

public class BookmarkView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
            animator.SetBool("MouseOnBookmark", true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
            animator.SetBool("MouseOnBookmark", false);
    }
}
