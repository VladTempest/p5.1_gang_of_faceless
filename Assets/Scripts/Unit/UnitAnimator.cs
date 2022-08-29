using System;
using Actions;
using Actions.MoveAction;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _unitAnimator;
    [SerializeField] private GameObject _arrowInHandPrefab;

    [SerializeField] private Transform _bow;
    [SerializeField]  private Transform _sword;
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsShortMovement = Animator.StringToHash("IsShortMovement");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int LongShot = Animator.StringToHash("LongShot");
    private static readonly int SwordSlash = Animator.StringToHash("SwordSlash");

    private void Awake()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
        if (TryGetComponent(out DefaultShotAction defaultShootAction))
        {
            defaultShootAction.OnActionStart += DefaultShootAction_OnActionStart;
        }
        
        if (TryGetComponent(out LongShotAction longShotAction))
        {
            longShotAction.OnActionStart += LongShotAction_OnActionStart;
        }
        
            
        if (TryGetComponent(out SwordAction swordAction))
        {
            swordAction.OnSwordActionCompleted += SwordActionOn_OnSwordActionCompleted;
            swordAction.OnSwordActionStarted += SwordActionOn_OnSwordActionStarted;
        }

        var archerAnimationsEvents = GetComponentInChildren<ArcherAnimationsEvents>();
        if (archerAnimationsEvents != null)
        {
            archerAnimationsEvents.OnGettingArrow += ArcherAnimationsEvents_OnOnGettingArrow;
            archerAnimationsEvents.OnReleaseArrow += ArcherAnimationsEvents_OnOnReleaseArrow;
        }
    }

    private void OnDestroy()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving -= MoveAction_OnStartMoving;
            moveAction.OnStopMoving -= MoveAction_OnStopMoving;
        }
        if (TryGetComponent(out DefaultShotAction defaultShootAction))
        {
            defaultShootAction.OnActionStart -= DefaultShootAction_OnActionStart;
        }
        
        if (TryGetComponent(out LongShotAction longShotAction))
        {
            longShotAction.OnActionStart -= LongShotAction_OnActionStart;
        }
        
            
        if (TryGetComponent(out SwordAction swordAction))
        {
            swordAction.OnSwordActionCompleted -= SwordActionOn_OnSwordActionCompleted;
            swordAction.OnSwordActionStarted -= SwordActionOn_OnSwordActionStarted;
        }

        var archerAnimationsEvents = GetComponentInChildren<ArcherAnimationsEvents>();
        if (archerAnimationsEvents != null)
        {
            archerAnimationsEvents.OnGettingArrow -= ArcherAnimationsEvents_OnOnGettingArrow;
            archerAnimationsEvents.OnReleaseArrow -= ArcherAnimationsEvents_OnOnReleaseArrow;
        }
    }

    private void ArcherAnimationsEvents_OnOnReleaseArrow()
    {
        _arrowInHandPrefab.SetActive(false);
    }

    private void ArcherAnimationsEvents_OnOnGettingArrow()
    {
        _arrowInHandPrefab.SetActive(true);
    }

    private void DefaultShootAction_OnActionStart(object sender, EventArgs e)
    {
        _unitAnimator.SetTrigger(Shoot);
    }
    private void LongShotAction_OnActionStart(object sender, EventArgs e)
    {
        _unitAnimator.SetTrigger(LongShot);
    }
    

    private void Start()
    {
        EquipBow();
    }

    private void SwordActionOn_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        _unitAnimator.SetTrigger(SwordSlash);
    }

    private void SwordActionOn_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipBow();
    }
    
    private void MoveAction_OnStartMoving(object sender, OnStartMovingEventArgs e)
    {
        _unitAnimator.SetBool(IsWalking, true);
        _unitAnimator.SetBool(IsShortMovement, e.isMovementShort);

    }
    
    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        _unitAnimator.SetBool(IsWalking, false);
    }

    private void EquipSword()
    {
        _sword.gameObject.SetActive(true);
        _bow.gameObject.SetActive(false);
    }

    private void EquipBow()
    {
        _sword.gameObject.SetActive(false);
        _bow.gameObject.SetActive(true);
    }
        
}
