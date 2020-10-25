using System.Collections.Generic;
using System.Linq;
using RTS.Controls;
using UnityEngine;

namespace RTS.Weapons
{
    public abstract class OnboardWeaponBase : WeaponBase
    {
        #region Data

        [Header("Onboard Weapon")] [SerializeField]
        protected Transform turretsContainer;

        private IDamageable _preferredTarget;
        private IDamageable _currentTarget;

        private IDamageable _lastTraget;
        private float _timeNextDamage;

        private List<OnboardTurretBase> _turrets = new List<OnboardTurretBase>();

        protected List<Transform> TurretTransforms { get; private set; }
        protected IDamageable CurrentTarget => _currentTarget;

        #endregion

        #region Public Methods

        protected override void Init()
        {
            TurretTransforms = turretsContainer.Cast<Transform>().ToList();
            foreach (var turretTransform in TurretTransforms)
            {
                var turretBase = turretTransform.GetComponent<OnboardTurretBase>();
                if (turretBase != null) _turrets.Add(turretBase);
            }
        }

        public override void ProcessWeapon(bool process)
        {
            if (!process) return;

            ProcessTarget();
            DamageTarget(_currentTarget);

            ProcessTurretRotating();
            ProcessVisuals();
        }

        #endregion

        #region Private Methods

        private void ProcessTurretRotating()
        {
            var targetPos = Vector3.zero;
            var hasTarget = _currentTarget != null;
            if (hasTarget) targetPos = _currentTarget.Position;

            foreach (var turret in _turrets)
                turret.UpdateTurret(hasTarget, targetPos);
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
        
        protected abstract void ProcessVisuals();

        #endregion
    }
}