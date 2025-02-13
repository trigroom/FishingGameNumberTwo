using UnityEngine;

public class SpriteGroupView : MonoBehaviour
{
   [HideInInspector] public SpriteRenderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }
}
