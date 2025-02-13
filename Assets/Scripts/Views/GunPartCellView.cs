using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class GunPartCellView : MonoBehaviour
{
    public Image gunPartImage;
    public Button gunPartButton;
    public GunPartInfo.GunPartType cellGunPartType;
    public Sprite defaultGunPartSprite;
    public bool isUsed;
    public bool isSelected;

    private EcsWorld _world;

    public void Construct(EcsWorld world)
    {
        _world = world;
    }

    private void Awake()
    {
        gunPartButton.onClick.AddListener(GunPartCellTry);
    }

    private void GunPartCellTry()
    {
        if (!isUsed)
        {
            var pool = _world.GetPool<TryEquipGunPartEvent>();
            ref var evt = ref pool.Add(_world.NewEntity());
            evt.cellViewGunPart = this;
        }
        else
        {
            isSelected = !isSelected;
            _world.GetPool<ChangeGunPartDescriptionEvent>().Add(_world.NewEntity()).gunPartCellView = this;
        }
    }
    //public GunPartType
}
