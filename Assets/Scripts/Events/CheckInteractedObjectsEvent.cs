using static PlayerInputView;

struct CheckInteractedObjectsEvent
{
    public InteractCharacterView currentInteractCharacter;
    public TrapView currentTrap;
    public DroppedItemView currentDropItem;

    public InteractionType interactionType;
}
