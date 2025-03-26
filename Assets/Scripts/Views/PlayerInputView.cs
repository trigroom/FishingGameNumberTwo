using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using static InteractCharacterView;

public class PlayerInputView : MonoBehaviour
{
    [field: SerializeField] public float checkRadius { get; private set; }
    [field: SerializeField] public LayerMask interactedMask { get; private set; }
    //[field: SerializeField] public LayerMask interactCharacterMask { get; private set; }
    [field: SerializeField] public LayerMask spawnZoneMask { get; private set; }
    [field: SerializeField] public TMP_Text itemInfoText;
    [field: SerializeField] public TMP_Text charactersInteractText;
    // [field: SerializeField] public PolygonCollider2D visionZoneCollider;

    private EcsWorld _world;
    private int _entity;

    public bool eventIsSended = false;
    public bool isColliderInteract = false;

    public Vector2 lastInteractedObjectPosition = Vector2.zero;

   // [HideInInspector] public QuestCharacterView currentQuestCharacter;
    //[HideInInspector] public TrapView currentTrap;

   // private int currentDroppedItem = -1;
   // public int currentActiveShopper { get; private set; }

   // public InteractNPCType currentInteractNPCType;

    public enum InteractionType
    {
        none,
        droppedItem,
        interactedCharacter,
        trap
    }

    private void Start()
    {
        InvokeRepeating("CheckNearestDroppedItems", 3f, 0.15f);
    }
    public void Construct(EcsWorld world, int entity)
    {
        _world = world;
        _entity = entity;
    }
    private void CheckNearestDroppedItems()
    {

        Collider2D collidedObject = Physics2D.OverlapCircle(gameObject.transform.position, checkRadius, interactedMask);

      //  Debug.Log(collidedObject);
        if (collidedObject != null)
        {
            if (!isColliderInteract || lastInteractedObjectPosition != (Vector2)collidedObject.gameObject.transform.position)
            {
                lastInteractedObjectPosition = (Vector2)collidedObject.gameObject.transform.position;
                ref var interactEventCmp = ref _world.GetPool<CheckInteractedObjectsEvent>().Add(_entity);
                if (collidedObject.gameObject.layer == 3) //drop item layer
                {
                    interactEventCmp.currentDropItem = collidedObject.gameObject.GetComponent<DroppedItemView>();
                    interactEventCmp.interactionType = InteractionType.droppedItem;

                }
                else if (collidedObject.gameObject.layer == 8)//interactCharacter layer
                {
                    interactEventCmp.currentInteractCharacter = collidedObject.gameObject.GetComponent<InteractCharacterView>();
                    interactEventCmp.interactionType = InteractionType.interactedCharacter;
                }
                else if (collidedObject.gameObject.layer == 15)//trap layer
                {
                    interactEventCmp.currentTrap = collidedObject.gameObject.GetComponent<TrapView>();
                    interactEventCmp.interactionType = InteractionType.trap;
                }
            }
            isColliderInteract = true;
        }
        else
        {
            if (isColliderInteract)
                _world.GetPool<CheckInteractedObjectsEvent>().Add(_entity).interactionType = InteractionType.none;
            isColliderInteract = false;
        }

    }
    //проверка ближайших вещей для подбора

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            _world.GetPool<EntrySpawnZoneEvent>().Add(_entity).zoneView = collision.gameObject.GetComponent<SpawnZoneView>();
        }
    
        else if (collision.gameObject.layer == 14)
        {
            _world.GetPool<EntryInNewLocationEvent>().Add(_world.NewEntity()).location = collision.gameObject.GetComponent<LocationEntryView>().locationSettings;//поменять чтоб не только один объеккт перекрашивало в полупрозрачный
            Debug.Log("collider loacation is entry" + collision.gameObject.GetComponent<LocationEntryView>().locationSettings);
        }
        else if (collision.gameObject.layer == 15)
        {
            var trapView = collision.gameObject.GetComponent<TrapView>();
            trapView.trapCollider.enabled = false;
            //наложение всех эффектов
            foreach (var effect in trapView.effects)
            {
                ref var effCmp = ref _world.GetPool<EffectComponent>().Add(_world.NewEntity());
                effCmp.effectEntity = _entity;
                effCmp.effectLevel = effect.effectLevel;
                effCmp.isFirstEffectCheck = true;
                effCmp.effectIconSprite = effect.effectIconSprite;
                effCmp.effectType = effect.effectType;
                effCmp.effectDuration = effect.effectTime;
            }//подумать что можно с эффектом капкана сделать
            trapView.spriteRenderer.sprite = trapView.safetyTrapSprite;
            if (trapView.type == TrapView.TrapType.mantrap)
            {
                Debug.Log("mantrap is activated");
                //добавить звук капкана
            }
            else //если мина
            {
                _world.GetPool<MineExplodeEvent>().Add(_world.NewEntity()) = new MineExplodeEvent(trapView.mineInfo, trapView.gameObject.transform.position);
                Destroy(trapView.gameObject, 1f);
            }
            _world.DelEntity( trapView.entity);
        }
        else if (collision.gameObject.layer == 12)
            _world.GetPool<ChangeInBuildingStateEvent>().Add(_world.NewEntity()) = new ChangeInBuildingStateEvent(true, collision.gameObject.GetComponent<TilemapsGroup>());
        else if (collision.gameObject.layer == 19)
            _world.GetPool<ChangeUnderNightLightPlayerStateEvent>().Add(_world.NewEntity()).playerCheckColliderRadius = collision.GetComponent<CircleCollider2D>().radius;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            Debug.Log("no zone");
            _world.GetPool<ExitSpawnZoneEvent>().Add(_entity).zoneView = collision.gameObject.GetComponent<SpawnZoneView>();
        }
        else if (collision.gameObject.layer == 12)
            _world.GetPool<ChangeInBuildingStateEvent>().Add(_world.NewEntity()) = new ChangeInBuildingStateEvent(false, collision.gameObject.GetComponent<TilemapsGroup>());
        else if (collision.gameObject.layer == 19)
            _world.GetPool<ChangeUnderNightLightPlayerStateEvent>().Add(_world.NewEntity());

    }

}
