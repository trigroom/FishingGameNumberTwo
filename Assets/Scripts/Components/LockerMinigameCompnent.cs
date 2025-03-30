using System.Collections.Generic;
using UnityEngine;

struct LockerMinigameCompnent 
{
    public List<LockCellView> lockerCells;
    public int needCount;
    public bool inGame;
    public float stopedTime;
    public bool inLeft;
    public float curSpeed;
    public InteractCharacterView interactCharacter;
    }