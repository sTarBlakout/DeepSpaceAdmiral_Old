using System;
using RTS.Controls;
using UnityEngine;

namespace RTS.Weapons
{
    public abstract class OnboardWeaponBase : WeaponBase
    {
        #region Data
        
        private IDamageable _preferredTarget;
        private IDamageable _currentTarget;
        
        private IDamageable _lastTraget;
        private float _timeNextDamage;

        #endregion

        #region Protected Methods
        
        public override void ProcessWeapon(bool process)
        {
            if (process)
            {
                ProcessTarget();
                DamageTarget(_currentTarget);
                ProcessVisuals();
            }
        }

        protected void ProcessTarget()
        {
            if (_currentTarget != null && !_currentTarget.IsFriend && _currentTarget.CanBeDamaged())
            {
                if (Vector3.Distance(transform.position, _currentTarget.Position) <= attackRange)
                    return;
                
                _currentTarget = null;
            }

            var possibleTargets = Physics.OverlapSphere(transform.position, attackRange);
            var minDist = float.MaxValue;
            IDamageable possibleTarget = null;
            foreach (var target in possibleTargets)
            {
                if (target.transform == ParentShip) continue;
                var targetDamageable = target.GetComponent<IDamageable>();
                if (targetDamageable == null || !targetDamageable.CanBeDamaged() || targetDamageable.IsFriend) continue;
                if (targetDamageable == _preferredTarget)
                {
                    possibleTarget = _preferredTarget;
                    break;
                }

                var distToTarget = Vector3.Distance(target.transform.position, transform.position); 
                if (distToTarget < minDist)
                {
                    minDist = distToTarget;
                    possibleTarget = targetDamageable;
                }
            }
            
            _currentTarget = possibleTarget;
        }
        
        protected void DamageTarget(IDamageable target)
        {
            if (target == null) return;

            if (_lastTraget != target)
            {
                _timeNextDamage = Time.time + fireRate;
                _lastTraget = target;
                return;
            }
            if (_timeNextDamage > Time.time || target.IsFriend || !target.CanBeDamaged()) return;
            _timeNextDamage = Time.time + fireRate;
            
            target.Damage(damage);
        }
        
        #endregion

        #region Abstract Methods
        
        protected abstract void ProcessVisuals();

        #endregion
    }
}