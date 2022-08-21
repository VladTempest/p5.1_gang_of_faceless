using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _unitAnimator;
    [SerializeField] private GameObject _bulletProjectiilePrefab;
    [SerializeField] private GameObject _arrowInHandPrefab;
    [SerializeField] private Transform _shootPoint;

    [SerializeField] private Transform _rifle;
    [SerializeField]  private Transform _sword;
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int SwordSlash = Animator.StringToHash("SwordSlash");

    private void Awake()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
        if (TryGetComponent(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
            shootAction.OnAiming += ShootAction_OnOnAiming;
        }
        if (TryGetComponent(out SwordAction swordAction))
        {
            swordAction.OnSwordActionCompleted += SwordActionOn_OnSwordActionCompleted;
            swordAction.OnSwordActionStarted += SwordActionOn_OnSwordActionStarted;
        }

        if (TryGetComponent(out ArcherAnimationsEvents archerAnimationsEvents))
        {
            archerAnimationsEvents.OnGettingArrow += ArcherAnimationsEvents_OnOnGettingArrow;
            archerAnimationsEvents.OnReleaseArrow += ArcherAnimationsEvents_OnOnReleaseArrow;
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

    private void ShootAction_OnOnAiming(object sender, EventArgs e)
    {
        _unitAnimator.SetTrigger(Shoot);
    }

    private void Start()
    {
        EquipRifle();
    }

    private void SwordActionOn_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        _unitAnimator.SetTrigger(SwordSlash);
    }

    private void SwordActionOn_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        Transform _bulletProjectiile = Instantiate(_bulletProjectiilePrefab, _shootPoint.position, Quaternion.identity).transform;
        var targetWorldPosition = e.targetUnit.WorldPosition;

        targetWorldPosition.y = _shootPoint.position.y;
        _bulletProjectiile.GetComponent<BulletProjectile>().Setup(targetWorldPosition);
        
        
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        _unitAnimator.SetBool(IsWalking, true);
    }
    
    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        _unitAnimator.SetBool(IsWalking, false);
    }

    private void EquipSword()
    {
        _sword.gameObject.SetActive(true);
        _rifle.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        _sword.gameObject.SetActive(false);
        _rifle.gameObject.SetActive(true);
    }
        
}
