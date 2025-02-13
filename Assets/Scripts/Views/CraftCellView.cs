using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftCellView : MonoBehaviour
{
    [field: SerializeField] public Image craftCellItemImage { get; set; }
    [field: SerializeField] public ItemCraftingRecipeInfo craftRecipeInfo;

    private EcsWorld _world;

    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnClickCraftInfo);
    }
    public void Construct(EcsWorld world)
    {
        _world = world;
        this.gameObject.transform.localScale = Vector3.one;
    }
    public void OnClickCraftInfo()
    {
            _world.GetPool<ShowCraftItemRecipeEvent>().Add(_world.NewEntity())
                .craftInfo = craftRecipeInfo;
    }
}
