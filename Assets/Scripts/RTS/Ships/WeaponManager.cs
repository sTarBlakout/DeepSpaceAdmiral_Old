using System;
using System.Collections;
using UnityEngine;
using RTS.Weapons;

namespace RTS.Ships
{
    public class WeaponManager : MonoBehaviour
    {
        #region Data
        
        private Transform _mainWeaponTransform;

        private bool _shouldAttackMain;
        private MonoBehaviour _currTarget;
        
        private bool _isMainGunLocked;
        private Coroutine _lockGunCoroutine;
        
        private MainWeaponBase _mainWeapon;

        public ActiveDirection ActiveDirection => _mainWeapon.ActiveDirection;
        public float AttackRange => _mainWeapon.AttackRange;

        #endregion

        #region Unity Events

        private void Awake()
        {
            _mainWeaponTransform = transform.GetChild(0);
            _mainWeapon = _mainWeaponTransform.GetComponentInChildren<MainWeaponBase>();
        }

        private void Start()
        {
            InitWeaponSystem();
        }

        private void FixedUpdate()
        {
            UpdateWeaponTemperature();
            ProcessMainWeapon(_shouldAttackMain && !_isMainGunLocked);
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
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return rotation;
        }
        
        #endregion

        #region Private Functions

        private void InitWeaponSystem()
        {
            _mainWeapon.InitWeapon();
        }

        #endregion

        #region Main Weapon Logic
        
        private void ProcessMainWeapon(bool process)
        {
            switch (_mainWeapon.ActiveDirection)
            {
                case ActiveDirection.Front:
                    _mainWeapon.ProcessWeapon(process);
                    break;
                
                case ActiveDirection.Sides:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
