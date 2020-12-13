using System.Collections.Generic;
using System.Linq;
using RTS.Controls;
using RTS.Interfaces;
using RTS.Ships;
using UnityEngine;

namespace RTS.Weapons
{
    public abstract class OnboardWeaponBase : WeaponBase
    {
        #region Data

        [Header("Onboard Weapon")] 
        [SerializeField] protected Transform turretsContainer;

        private ITargetable _prefTarget;
        private ITargetable _currTarget;
        private ITargetable _lastTraget;
        private float _timeNextDamage;
        private List<OnboardTurretBase> _turrets = new List<OnboardTurretBase>();

        protected List<Transform> TurretTransforms { get; private set; }
        protected ITargetable CurrTarget => _currTarget;

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
                DamageTarget(_currTarget);
            }
            else
            {
                _currTarget = null;
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
            var hasTarget = _currTarget != null;
            if (hasTarget) targetPos = _currTarget.Transform.position;

            foreach (var turret in _turrets)
                turret.UpdateTurret(hasTarget, targetPos);
        }

        private void ProcessTarget()
        {
            if (RTSGameController.TargetExistAndReachable(TargetableShip, _currTarget, attackRange))
                return;

            _currTarget = RTSGameController.GetClosestTarget(TargetableShip, attackRange, _prefTarget);
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