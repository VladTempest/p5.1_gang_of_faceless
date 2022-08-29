using System;
using Actions;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileLauncher : MonoBehaviour
    {

        [SerializeField] private GameObject _arrowProjectiilePrefab;
        [SerializeField] private Transform _shootPoint;
        
        private Vector3 _verticalShotOffset = new Vector3(0, 50, 0);
        
        private void Awake()
        {
            if (TryGetComponent(out DefaultShotAction defaultShotAction))
            {
                defaultShotAction.OnDefaultShot += ShootAction_OnDefaultShot;
            }
            
            if (TryGetComponent(out LongShotAction longShootAction))
            {
                longShootAction.OnLongShot += ShootAction_OnLongShot;
            }
        }
        
        
        
        
        private void ShootAction_OnDefaultShot(object sender, OnShootEventArgs e)
        {
            var position = _shootPoint.position;
            Transform projectiile = Instantiate(_arrowProjectiilePrefab, position, transform.rotation).transform;
            var targetWorldPosition = e.TargetUnit.WorldPosition;

            targetWorldPosition.y = position.y;
            projectiile.GetComponent<ArrowProjectile>().Setup(targetWorldPosition, e.HitCallback);
        }
        
        private void ShootAction_OnLongShot(object sender, OnShootEventArgs e)
        {
            ShootUpside(e);
        }

        private void ShootUpside(OnShootEventArgs e)
        {
            var position = _shootPoint.position;
            var targetWorldPosition = position + _verticalShotOffset;
            
            Transform projectile = Instantiate(_arrowProjectiilePrefab, position, Quaternion.LookRotation(Vector3.up)).transform;
            
            projectile.GetComponent<ArrowProjectile>().Setup(targetWorldPosition, () => ShootDownside(e));
        }
        
        private void ShootDownside(OnShootEventArgs e)
        {
            var position = e.TargetUnit.WorldPosition + _verticalShotOffset;
            var targetWorldPosition = e.TargetUnit.WorldPosition;
            targetWorldPosition.y = _shootPoint.position.y;
            
            Transform projectile = Instantiate(_arrowProjectiilePrefab, position, Quaternion.LookRotation(Vector3.down)).transform;
            
            projectile.GetComponent<ArrowProjectile>().Setup(targetWorldPosition, e.HitCallback);
        }

        
    }
}