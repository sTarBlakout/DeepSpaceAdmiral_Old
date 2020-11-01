using System;
using System.Collections;
using UnityEngine;
using RTS.Weapons;

namespace RTS.Ships
{
    public class WeaponManager : MonoBehaviour
    {
        #region Data

        private bool _shouldAttackMain;
        private MonoBehaviour _currTarget;
        
        private bool _isMainGunLocked;
        private Coroutine _lockGunCoroutine;
        
        private MainWeaponBase _mainWeapon;
        private OnboardWeaponBase _onboardWeapon;

        public ActiveDirection ActiveDirection => _mainWeapon.ActiveDirection;
        public float AttackRange => _mainWeapon.AttackRange;

        #endregion

        #region Unity Events

        private void Awake()
        {
            _mainWeapon = transform.GetChild(0).GetComponentInChildren<MainWeaponBase>();
            _onboardWeapon = transform.GetChild(1).GetComponentInChildren<OnboardWeaponBase>();
        }

        private void Start()
        {
            InitWeaponSystem();
        }

        private void FixedUpdate()
        {
            UpdateWeaponTemperature();
            ProcessMainWeapon(_shouldAttackMain && !_isMainGunLocked);
            _onboardWeapon.ProcessWeapon(false);
        }
        
        #endregion

        #region Public API

        public void UpdateWeaponSystem(bool shouldAttackMain, MonoBehaviour target = null)
        {
            _currTarget = target;
            _shouldAttackMain = shouldAttackMain;
        }

        public Vector3 CalculateRequiredRotation()
        {
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
            _mainWeapon.InitWeapon(parent);
            _onboardWeapon.InitWeapon(parent);
        }

        #endregion

        #region Main Weapon Logic
        
        private void ProcessMainWeapon(bool process)
        {
            _mainWeapon.ProcessWeapon(process);
        }

        private void UpdateWeaponTemperature()
        {
            _mainWeapon.ProcessWeaponTemperature();
            if (_mainWeapon.ShouldLock && _lockGunCoroutine == null)
                _lockGunCoroutine = StartCoroutine(LockMainGun());
        }

        private IEnumerator LockMainGun(float seconds = 0f)
        {
            _isMainGunLocked = true;
            if (seconds == 0f)
                yield return new WaitUntil(() => _mainWeapon.ShouldLock);
            else
                yield return new WaitForSeconds(seconds);
            _isMainGunLocked = false;
            _lockGunCoroutine = null;
        }

        #endregion
    }
}
