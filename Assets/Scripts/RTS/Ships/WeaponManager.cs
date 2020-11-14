using System.Collections.Generic;
using System.Linq;
using RTS.Controls;
using UnityEngine;
using RTS.Weapons;

namespace RTS.Ships
{
    public class WeaponManager : MonoBehaviour
    {
        #region Data

        private bool _shouldAttackMain;
        private bool _shouldAttackOnboard;
        private MonoBehaviour _currTargetControls;
        private MonoBehaviour _currTargetAuto;
        private IDamageable _currTargetAutoDamageable;

        private List<MainWeaponBase> _mainWeapons;
        private OnboardWeaponBase _onboardWeapon;

        private ISelectable _parentSelectable;

        private FireMode _fireMode;

        public ActiveDirection ActiveDirection => _mainWeapons[0].ActiveDirection;
        public float AttackRange => _mainWeapons[0].AttackRange;

        #endregion

        #region Unity Events

        private void Awake()
        {
            _mainWeapons = transform.GetChild(0).GetComponentsInChildren<MainWeaponBase>().ToList();
            _onboardWeapon = transform.GetChild(1).GetComponentInChildren<OnboardWeaponBase>();
            _parentSelectable = transform.parent.GetComponent<ISelectable>();
        }

        private void Start()
        {
            InitWeaponSystem();
        }

        private void FixedUpdate()
        {
            ProcessFireMode();
            UpdateWeaponTemperature();
            ProcessMainWeapon(_shouldAttackMain);
            _onboardWeapon.ProcessWeapon(_shouldAttackOnboard);
        }
        
        #endregion

        #region Public API

        public void UpdateWeaponSystem(bool shouldAttackMain, bool shouldAttackOnboard, MonoBehaviour target = null)
        {
            _currTargetControls = target;
            _shouldAttackMain = shouldAttackMain;
            _shouldAttackOnboard = shouldAttackOnboard;
        }

        public Vector3 CalculateRequiredRotation()
        {
            Vector3 rotation = Vector3.zero;
            switch (ActiveDirection)
            {
                case ActiveDirection.Front:
                    rotation = (_currTargetControls.transform.position - transform.position).normalized;
                    break;
                case ActiveDirection.Sides:
                    var directToTarget = (_currTargetControls.transform.position - transform.position).normalized;
                    var angle = 90f;
                    if (Vector3.Angle(transform.right, directToTarget) < 90f) angle = -90f;
                    rotation = Quaternion.AngleAxis(angle, Vector3.up) * directToTarget;
                    break;
            }
            return rotation;
        }

        public void SwitchFireMode(FireMode fireMode)
        {
            _fireMode = fireMode;
        }
        
        #endregion

        #region Private Functions

        private void InitWeaponSystem()
        {
            var parent = transform.parent;
            _onboardWeapon.InitWeapon(parent);
            foreach (var mainWeapon in _mainWeapons)
                mainWeapon.InitWeapon(parent);
        }

        private void ProcessFireMode()
        {
            if (_currTargetControls != null)
            {
                _currTargetAuto = null;
                _currTargetAutoDamageable = null;
                return;
            }

            switch (_fireMode)
            {
                case FireMode.OnlyMain: 
                    AutoTargetFind(); 
                    _shouldAttackOnboard = false;
                    break;
                case FireMode.AllGuns: 
                    AutoTargetFind();
                    _shouldAttackOnboard = true;
                    break;
                case FireMode.OnlyOnboard: 
                    _shouldAttackOnboard = true; 
                    _currTargetAuto = null;
                    _currTargetAutoDamageable = null;
                    break;
                case FireMode.NoGuns:
                    _shouldAttackOnboard = false;
                    _currTargetAuto = null;
                    _currTargetAutoDamageable = null;
                    break;
            }
        }

        private void AutoTargetFind()
        {
            if (RTSGameController.TargetExistAndReachable(transform.parent, _parentSelectable.TeamId, AttackRange, _currTargetAutoDamageable))
                return;
            
            _currTargetAuto = RTSGameController.GetClosestTarget(transform.parent, _parentSelectable.TeamId, AttackRange);
            _currTargetAutoDamageable = _currTargetAuto != null ? _currTargetAuto.GetComponent<IDamageable>() : null;
        }
        

        #endregion

        #region Main Weapon Logic
        
        private void ProcessMainWeapon(bool process)
        {
            foreach (var mainWeapon in _mainWeapons)
                mainWeapon.ProcessWeapon(process && !mainWeapon.IsMainGunLocked);
        }

        private void UpdateWeaponTemperature()
        {
            foreach (var mainWeapon in _mainWeapons)
            {
                mainWeapon.ProcessWeaponTemperature();
                if (mainWeapon.ShouldLock && mainWeapon.LockGunCoroutine == null)
                    mainWeapon.LockGunCoroutine = StartCoroutine(mainWeapon.LockMainGun());
            }
        }

        #endregion
    }
}
