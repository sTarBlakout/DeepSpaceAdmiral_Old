using RTS.Controls;
using UnityEngine;

namespace RTS.Weapons
{
    public abstract class OnboardWeaponBase : WeaponBase
    {
        #region Data
        
        [Header("Onboard Weapon")]
        [SerializeField] protected Transform leftSidedGuns;
        [SerializeField] protected Transform rightSidedGuns;
        [SerializeField] protected Transform bothSidedGuns;
        
        private IDamageable _preferredTarget;
        private IDamageable _currentTarget;
        
        private IDamageable _lastTraget;
        private float _timeNextDamage;

        private float _rightSideDot;

        protected IDamageable CurrentTarget => _currentTarget;

        #endregion

        #region Normal Methods
        
        public override void ProcessWeapon(bool process)
        {
            if (process)
            {
                if (_currentTarget != null)
                {
                    var weaponTransform = transform;
                    var rotation = (_currentTarget.Position - weaponTransform.position).normalized;
                    _rightSideDot = Vector3.Dot(rotation.normalized, weaponTransform.right);
                }

                ProcessTarget();
                DamageTarget(_currentTarget);
                
                ProcessVisuals(bothSidedGuns, _currentTarget != null);
                ProcessVisuals(rightSidedGuns, _rightSideDot > 0f && _currentTarget != null);
                ProcessVisuals(leftSidedGuns, _rightSideDot < 0f && _currentTarget != null);
            }
        }

        private void ProcessTarget()
        {
            if (_currentTarget != null && _currentTarget.IsEnemy(SelectableShip.TeamId) && _currentTarget.CanBeDamaged())
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
                if (targetDamageable == null) continue;
                if (!targetDamageable.CanBeDamaged() || !targetDamageable.IsEnemy(SelectableShip.TeamId)) continue;
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
        
        private void DamageTarget(IDamageable target)
        {
            if (target == null) return;

            if (_lastTraget != target)
            {
                _timeNextDamage = Time.time + fireRate;
                _lastTraget = target;
                return;
            }
            if (_timeNextDamage > Time.time || !target.IsEnemy(SelectableShip.TeamId) || !target.CanBeDamaged()) return;
            _timeNextDamage = Time.time + fireRate;
            
            target.Damage(damage);
        }
        
        #endregion

        #region Abstract Methods
        
        protected abstract void ProcessVisuals(Transform gunContainer, bool activate);

        #endregion
    }
}