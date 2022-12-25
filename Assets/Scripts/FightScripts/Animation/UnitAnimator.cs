using System;
using Actions;
using Actions.MoveAction;
using Editor.Scripts.Actions;
using Editor.Scripts.Animation;
using Scripts.Unit;
using Systems.HealthStatus;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _unitAnimator;
    [SerializeField] private GameObject _arrowInHand;
    [SerializeField] private GameObject _bombInHand;
    [SerializeField] private GameObject _rightDaggerInHand;

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
    private static readonly int BackStab = Animator.StringToHash("BackStab");
    private static readonly int GettingHit = Animator.StringToHash("GettingHit");
    private static readonly int RandomHitIndex = Animator.StringToHash("RandomHitIndex");
    private static readonly int ParalyzeShot = Animator.StringToHash("ParalyzeShot");
    private static readonly int GrenadeThrow = Animator.StringToHash("GrenadeThrow");

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
        
        if (TryGetComponent(out ParalyzeShotAction paralyzeShootAction))
        {
            paralyzeShootAction.OnActionStart += ParalyzeShotAction_OnActionStart;
        }
        
        if (TryGetComponent(out LongShotAction longShotAction))
        {
            longShotAction.OnActionStart += LongShotAction_OnActionStart;
        }
        
            
        if (TryGetComponent(out GreatSwordAction swordAction))
        {
            swordAction.OnActionStart += MeleeActionOnOnMeleeActionStarted;
        }
        
        if (TryGetComponent(out DualSwordsAction dualSwordsAction))
        {
            dualSwordsAction.OnActionStart += MeleeActionOnOnMeleeActionStarted;
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
            
            effectSystem.OnParalyzeOver += EffectSystem_OnParalyzeOver;
            effectSystem.OnParalyzeStart += EffectSystem_OnKParalyzeStart;
        }
        
        if (TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.OnDamaged += HealthSystem_OnDamaged;
        }
        
        if (TryGetComponent(out BackStabAction backStabAction))
        {
            backStabAction.OnStartTeleporting += BackStabAction_OnActionStart;
        }
        
        if (TryGetComponent(out GrenadeAction grenadeAction))
        {
            grenadeAction.OnActionStart += GrenadeAction_OnActionStart;
        }
        
        var archerAnimationsEvents = GetComponentInChildren<ArcherAnimationsEvents>();
        if (archerAnimationsEvents != null)
        {
            archerAnimationsEvents.OnGettingArrow += ArcherAnimationsEvents_OnGettingArrow;
            archerAnimationsEvents.OnReleaseArrow += ArcherAnimationsEvents_OnReleaseArrow;
        }
        var lightWarriorAnimationsEvents = GetComponentInChildren<LightWarriorAnimationEvents>();
        if (archerAnimationsEvents != null)
        {
            lightWarriorAnimationsEvents.OnGettingBomb += LightWarriorAnimationsEvents_OnGettingBomb;
            lightWarriorAnimationsEvents.OnReleaseBomb += LightWarriorAnimationsEvents_OnReleaseBomb;
            lightWarriorAnimationsEvents.OnEquipDagger += LightWarriorAnimationsEvents_OnEquipDagger;
        }
        
    }

    private void GrenadeAction_OnActionStart(object sender, EventArgs e)
    {
        _unitAnimator.SetTrigger(GrenadeThrow);
    }

    private void LightWarriorAnimationsEvents_OnEquipDagger()
    {
        _rightDaggerInHand.SetActive(true);
    }

    private void LightWarriorAnimationsEvents_OnReleaseBomb()
    {
        _bombInHand.SetActive(false);
    }

    private void LightWarriorAnimationsEvents_OnGettingBomb()
    {
        _bombInHand.SetActive(true);
        _rightDaggerInHand.SetActive(false);
    }

    private void ParalyzeShotAction_OnActionStart(object sender, EventArgs e)
    {
        _unitAnimator.SetTrigger(ParalyzeShot);
    }

    private void EffectSystem_OnKParalyzeStart(object sender, EventArgs e)
    {
        _unitAnimator.SetBool("IsParalyzed", true);
    }

    private void EffectSystem_OnParalyzeOver(object sender, EventArgs e)
    {
        _unitAnimator.SetBool("IsParalyzed", false);
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        _unitAnimator.SetInteger(RandomHitIndex, Random.Range(0,3));
        _unitAnimator.SetTrigger(GettingHit);
    }

    private void BackStabAction_OnActionStart(object sender, EventArgs e)
    {
        _unitAnimator.SetTrigger(BackStab);
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
        
        if (TryGetComponent(out ParalyzeShotAction paralyzeShootAction))
        {
            paralyzeShootAction.OnActionStart -= ParalyzeShotAction_OnActionStart;
        }
        
        if (TryGetComponent(out LongShotAction longShotAction))
        {
            longShotAction.OnActionStart -= LongShotAction_OnActionStart;
        }
        
            
        if (TryGetComponent(out GreatSwordAction swordAction))
        {
            swordAction.OnActionStart -= MeleeActionOnOnMeleeActionStarted;
        }
        
        if (TryGetComponent(out DualSwordsAction dualSwordsAction))
        {
            dualSwordsAction.OnActionStart -= MeleeActionOnOnMeleeActionStarted;
        }
        
        if (TryGetComponent(out KnockDownAction knockDownAction))
        {
            knockDownAction.OnActionStart -= KnockDownAction_OnActionStart;
        }
        
        if (TryGetComponent(out PushAction pushAction))
        {
            pushAction.OnActionStart -= PushAction_OnActionStart;
            pushAction.OnUnitPushed -= PushAction_OnUnitPushed;
        }

        if (TryGetComponent(out EffectSystem effectSystem))
        {
            effectSystem.OnKnockDownOver -= EffectSystem_OnKnockDownOver;
            effectSystem.OnKnockDownStart -= EffectSystem_OnKnockDownStart;
            
            effectSystem.OnParalyzeOver -= EffectSystem_OnParalyzeOver;
            effectSystem.OnParalyzeStart -= EffectSystem_OnKParalyzeStart;
        }
        
        if (TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.OnDamaged -= HealthSystem_OnDamaged;
        }
        
        if (TryGetComponent(out BackStabAction backStabAction))
        {
            backStabAction.OnStartTeleporting -= BackStabAction_OnActionStart;
        }
        
        if (TryGetComponent(out GrenadeAction grenadeAction))
        {
            grenadeAction.OnActionStart -= GrenadeAction_OnActionStart;
        }
        
        var archerAnimationsEvents = GetComponentInChildren<ArcherAnimationsEvents>();
        if (archerAnimationsEvents != null)
        {
            archerAnimationsEvents.OnGettingArrow -= ArcherAnimationsEvents_OnGettingArrow;
            archerAnimationsEvents.OnReleaseArrow -= ArcherAnimationsEvents_OnReleaseArrow;
        }
        var lightWarriorAnimationsEvents = GetComponentInChildren<LightWarriorAnimationEvents>();
        if (archerAnimationsEvents != null)
        {
            lightWarriorAnimationsEvents.OnGettingBomb -= LightWarriorAnimationsEvents_OnGettingBomb;
            lightWarriorAnimationsEvents.OnReleaseBomb -= LightWarriorAnimationsEvents_OnReleaseBomb;
            lightWarriorAnimationsEvents.OnEquipDagger -= LightWarriorAnimationsEvents_OnEquipDagger;
        }
    }

    private void ArcherAnimationsEvents_OnReleaseArrow()
    {
        _arrowInHand.SetActive(false);
    }

    private void ArcherAnimationsEvents_OnGettingArrow()
    {
        _arrowInHand.SetActive(true);
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
        SetAnimationCoreAccordingToClass();
    }

    private void SetAnimationCoreAccordingToClass()
    {
        switch (_unit.UnitType)
        {
            case UnitType.None:
                break;
            case UnitType.Archer:
                _unitAnimator.SetBool(Archer, true);
                break;
            case UnitType.HeavyWarrior:
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
}
