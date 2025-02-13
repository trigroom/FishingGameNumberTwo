using UnityEngine;
using static PlayerInputView;

public struct CurrentInteractedCharactersComponent
{
    public DroppedItemView dropItemView;
    public TrapView trapView;
    public InteractCharacterView interactCharacterView;

    //public int currentDroppedItem;
   // public int currentActiveShopper;

    //public bool isColliderInteract;
    //public InteractNPCType currentInteractNPCType;
    public bool isNPCNowIsUsed;
    public InteractionType interactionType;
}