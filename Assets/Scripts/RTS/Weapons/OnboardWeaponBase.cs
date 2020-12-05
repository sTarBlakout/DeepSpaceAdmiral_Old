using System.Collections.Generic;
using System.Linq;
using GameGlobal;
using RTS.Controls;
using UnityEngine;

namespace RTS.Weapons
{
    public abstract class OnboardWeaponBase : WeaponBase
    {
        #region Data

        [Header("Onboard Weapon")] 
        [SerializeField] protected Transform turretsContainer;

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
            if (process)
            {
                ProcessTarget();
                DamageTarget(_currentTarget);
            }
            else
            {
                _currentTarget = null;
            }

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
            if (RTSGameController.TargetExistAndReachable(DamageableShip, _currentTarget, attackRange))
                return;

            var target = RTSGameController.GetClosestTarget(DamageableShip, attackRange, _preferredTarget);
            if (target != null) _currentTarget = target.GetComponent<IDamageable>();
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