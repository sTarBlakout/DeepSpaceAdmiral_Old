using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RTS.Weapons;

namespace RTS.Ships
{
    public class WeaponManager : MonoBehaviour
    {
        #region Data

        private bool _shouldAttackMain;
        private bool _shouldAttackOnboard;
        private MonoBehaviour _currTarget;

        private List<MainWeaponBase> _mainWeapons;
        private OnboardWeaponBase _onboardWeapon;

        public ActiveDirection ActiveDirection => _mainWeapons[0].ActiveDirection;
        public float AttackRange => _mainWeapons[0].AttackRange;

        #endregion

        #region Unity Events

        private void Awake()
        {
            _mainWeapons = transform.GetChild(0).GetComponentsInChildren<MainWeaponBase>().ToList();
            _onboardWeapon = transform.GetChild(1).GetComponentInChildren<OnboardWeaponBase>();
        }

        private void Start()
        {
            InitWeaponSystem();
        }

        private void FixedUpdate()
        {
            UpdateWeaponTemperature();
            ProcessMainWeapon(_shouldAttackMain);
            _onboardWeapon.ProcessWeapon(_shouldAttackOnboard);
        }
        
        #endregion

        #region Public API

        public void UpdateWeaponSystem(bool shouldAttackMain, bool shouldAttackOnboard, MonoBehaviour target = null)
        {
            _currTarget = target;
            _shouldAttackMain = shouldAttackMain;
            _shouldAttackOnboard = shouldAttackOnboard;
        }

        public Vector3 CalculateRequiredRotation()
        {
            if (_currTarget == null) return Vector3.up;
            
            Vector3 rotation = Vector3.zero;
            switch (ActiveDirection)
            {
                case ActiveDirection.Front:
                    rotation = (_currTarget.transform.position - transform.position).normalized;
                    break;
                case ActiveDirection.Sides:
                    var directToTarget = (_currTarget.transform.position - transform.position).normalized;
                    var angle = 90f;
                    if (Vector3.Angle(transform.right, directToTarget) < 90f) angle = -90f;
                    rotation = Quaternion.AngleAxis(angle, Vector3.up) * directToTarget;
                    break;
            }
            return rotation;
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
