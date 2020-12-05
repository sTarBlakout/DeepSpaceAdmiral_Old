using System.Collections.Generic;
using System.Linq;
using RTS.Controls;
using UnityEngine;

namespace RTS.Weapons
{
    public abstract class OnboardWeaponBase : WeaponBase
    {
        #region Data

        [Header("Onboard Weapon")] 
        [SerializeField] protected Transform turretsContainer;

        private ITargetable _preferredTarget;
        private ITargetable _currentTarget;
        private ITargetable _lastTraget;
        private float _timeNextDamage;
        private List<OnboardTurretBase> _turrets = new List<OnboardTurretBase>();

        protected List<Transform> TurretTransforms { get; private set; }
        protected ITargetable CurrentTarget => _currentTarget;

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
                _lastTraget = null;
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
            if (RTSGameController.TargetExistAndReachable(TargetableShip, _currentTarget, attackRange))
                return;

            var target = RTSGameController.GetClosestTarget(TargetableShip, attackRange, _preferredTarget);
            if (target != null) _currentTarget = target.GetComponent<ITargetable>();
        }
        
        private void DamageTarget(ITargetable target)
        {
            if (target == null) return;

            if (_lastTraget != target)
            {
                _timeNextDamage = Time.time + fireRate;
                _lastTraget = target;
                return;
            }
            if (_timeNextDamage > Time.time || !target.IsEnemy(SelectableShip.TeamId) || !target.Damageable.CanBeDamaged()) return;
            _timeNextDamage = Time.time + fireRate;
            
            target.Damageable.Damage(damage);
        }
        
        #endregion

        #region Abstract Methods
        
        protected abstract void ProcessVisuals();

        #endregion
    }
}