using System;
using Actions;
using Actions.MoveAction;
using Editor.Scripts.Actions;
using Scripts.Unit;
using Systems.HealthStatus;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _unitAnimator;
    [SerializeField] private GameObject _arrowInHandPrefab;

    [SerializeField] private Transform _bow;
    [SerializeField]  private Transform _sword;

    private Unit _unit;
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsShortMovement = Animator.StringToHash("IsShortMovement");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int LongShot = Animator.StringToHash("LongShot");
    private static readonly int SwordSlash = Animator.StringToHash("SwordSlash");
    private static readonly int KnockdownSlash = Animator.StringToHash("KnockdownSlash");
    private static readonly int PushSlash = Animator.StringToHash("PushSlash");
    private static readonly int LightWarrior = Animator.StringToHash("LightWarrior");
    private static readonly int HeavyWarrior = Animator.StringToHash("HeavyWarrior");
    private static readonly int Archer = Animator.StringToHash("Archer");
    private static readonly int Pushed = Animator.StringToHash("Pushed");
    private static readonly int KnockedDown = Animator.StringToHash("KnockedDown");

    private void Awake()
    {
        if (TryGetComponent(out Unit unit))
        {
            _unit = unit;
        }
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
        
            
        if (TryGetComponent(out GreatSwordAction swordAction))
        {
            swordAction.OnActionStart += MeleeActionOnOnMeleeActionStarted;
        }
        
        if (TryGetComponent(out KnockDownAction knockDownAction))
        {
            knockDownAction.OnActionStart += KnockDownAction_OnActionStart;
        }
        
        if (TryGetComponent(out PushAction pushAction))
        {
            pushAction.OnActionStart += PushAction_OnActionStart;
            pushAction.OnUnitPushed += PushAction_OnUnitPushed;
        }

        if (TryGetComponent(out EffectSystem effectSystem))
        {
            effectSystem.OnKnockDownOver += EffectSystem_OnKnockDownOver;
            effectSystem.OnKnockDownStart += EffectSystem_OnKnockDownStart;
        }

        var archerAnimationsEvents = GetComponentInChildren<ArcherAnimationsEvents>();
        if (archerAnimationsEvents != null)
        {
            archerAnimationsEvents.OnGettingArrow += ArcherAnimationsEvents_OnOnGettingArrow;
            archerAnimationsEvents.OnReleaseArrow += ArcherAnimationsEvents_OnOnReleaseArrow;
        }
    }

    private void KnockDownAction_OnActionStart(object sender, EventArgs e)
    {
        _unitAnimator.SetTrigger(KnockdownSlash);
    }

    private void EffectSystem_OnKnockDownStart(object sender, EventArgs e)
    {
        _unitAnimator.SetBool(KnockedDown, true);
    }

    private void EffectSystem_OnKnockDownOver(object sender, EventArgs e)
    {
        _unitAnimator.SetBool(KnockedDown, false);
    }

    private void PushAction_OnUnitPushed(object sender, OnPushActionEventArgs e)
    {
        e.pushedUnitAnimator.SetTrigger(Pushed);
    }

    private void PushAction_OnActionStart(object sender, EventArgs e)
    {
        _unitAnimator.SetTrigger(PushSlash);
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
        
        if (TryGetComponent(out PushAction pushAction))
        {
            pushAction.OnActionStart -= PushAction_OnActionStart;
        }
            
        if (TryGetComponent(out MeleeAttackAction swordAction))
        {
            swordAction.OnActionStart -= MeleeActionOnOnMeleeActionStarted;
        }
        
        if (TryGetComponent(out EffectSystem effectSystem))
        {
            effectSystem.OnKnockDownOver -= EffectSystem_OnKnockDownOver;
            effectSystem.OnKnockDownStart -= EffectSystem_OnKnockDownStart;
        }
        if (TryGetComponent(out KnockDownAction knockDownAction))
        {
            knockDownAction.OnActionStart -= KnockDownAction_OnActionStart;
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
        switch (_unit.UnitType)
        {
            case UnitType.None:
                break;
            case UnitType.Archer:
                _unitAnimator.SetBool(Archer, true);
                EquipBow();
                break;
            case UnitType.HeavyWarrior:
                EquipSword();
                _unitAnimator.SetBool(HeavyWarrior, true);
                break;
            case UnitType.LightWarrior:
                _unitAnimator.SetBool(LightWarrior, true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MeleeActionOnOnMeleeActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        _unitAnimator.SetTrigger(SwordSlash);
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
