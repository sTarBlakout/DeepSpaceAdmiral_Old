using System;
using System.Collections;
using UnityEngine;
using RTS.Controls;

namespace RTS.Ships
{
    public class WeaponManager : MonoBehaviour
    {
        #region Data

        [Header("Main")]
        [SerializeField] private Transform mainWeapon;
        [SerializeField] private MainWeaponType mainWeaponType;
        [SerializeField] private ProjectileType mainWeaponProjectileType;
        
        [SerializeField] private float damage = 1f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float fireRate = 1f;
        
        [SerializeField] private float minGunTemp = -1;
        [SerializeField] private float maxGunTemp = 1;
        [SerializeField] private float borderGunTemp;
        [SerializeField] private float mainGunWarmFactor;
        [SerializeField] private float mainGunCoolFactor;


        [Header("Laser Beam")] 
        [SerializeField] private float beamThickness;
        [SerializeField] private GameObject laserBeamStart;
        [SerializeField] private GameObject laserBeamStream;
        [SerializeField] private GameObject laserBeamEnd;
        
        private bool _shouldAttackMain;
        private MonoBehaviour _currTarget;

        private bool _isMainGunFiring;
        private float _mainGunTemp;
        private bool _isMainGunLocked;

        private Coroutine _lockGunCoroutine;

        public MainWeaponType MainWeaponType => mainWeaponType;
        public float AttackRange => attackRange;
        
        #endregion

        #region Unity Events

        private void Start()
        {
            _mainGunTemp = minGunTemp;
            
            InitWeaponSystem();
        }

        private void FixedUpdate()
        {
            ChangeGunTemp(_isMainGunFiring);

            if (_shouldAttackMain && !_isMainGunLocked)
                ProcessMainWeapon();
            else
            {
                _isMainGunFiring = false;
                ActivateLaserBeamVFX(false);
            }
        }
        
        #endregion

        #region Public API

        public void UpdateWeaponSystem(bool shouldAttackMain, MonoBehaviour target = null)
        {
            _currTarget = target;
            _shouldAttackMain = shouldAttackMain;
        }
        
        #endregion

        #region Private Functions

        public void InitWeaponSystem()
        {
            switch (mainWeaponProjectileType)
            {
                case ProjectileType.LaserBeam:
                    InitLaserBeam();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Main Weapon
        
        private void ProcessMainWeapon()
        {
            switch (mainWeaponType)
            {
                case MainWeaponType.Front:
                    ProcessMainFrontWeapon();
                    break;
                
                case MainWeaponType.Sides:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void ProcessMainFrontWeapon()
        {
            switch (mainWeaponProjectileType)
            {
                case ProjectileType.LaserBeam:
                    ProcessLaserBeamMain();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeGunTemp(bool increase)
        {
            if (Mathf.Approximately(_mainGunTemp, maxGunTemp) && _lockGunCoroutine == null)
                _lockGunCoroutine = StartCoroutine(LockMainGun());

            float value;
            if (increase)
                value = mainGunWarmFactor;
            else
                value = -mainGunCoolFactor;

            _mainGunTemp = Mathf.Clamp(_mainGunTemp + value, minGunTemp, maxGunTemp);
        }

        private IEnumerator LockMainGun(float seconds = 0f)
        {
            _isMainGunLocked = true;
            if (seconds == 0f)
                yield return new WaitUntil(() => _mainGunTemp <= borderGunTemp);
            else
                yield return new WaitForSeconds(seconds);
            _isMainGunLocked = false;
            _lockGunCoroutine = null;
        }

        #endregion

        #region Laser Beam

        private void ProcessLaserBeamMain()
        {
            if (Physics.SphereCast(mainWeapon.position, beamThickness, transform.forward, out var hit, attackRange))
            {
                var hitDamageable = hit.collider.GetComponent<IDamageable>();
                if (hitDamageable != null)
                {
                    if (!hitDamageable.IsFriend)
                    {
                        _isMainGunFiring = true;
                        if (_mainGunTemp < borderGunTemp) return;
                        
                        var lineRenderer = laserBeamStream.GetComponent<LineRenderer>();
                        lineRenderer.SetPosition(0, mainWeapon.position);
                        lineRenderer.SetPosition(1, hit.point);

                        laserBeamEnd.transform.position = hit.point;

                        ActivateLaserBeamVFX(true);
                                
                        hitDamageable.Damage(damage);
                    }
                    else
                    {
                        _isMainGunFiring = false;
                        ActivateLaserBeamVFX(false);
                    }
                }
            }
            else
            {
                _isMainGunFiring = false;
                ActivateLaserBeamVFX(false);
            }
        }
        
        private void ActivateLaserBeamVFX(bool activate)
        {
            laserBeamStart.SetActive(activate);
            laserBeamStream.SetActive(activate);
            laserBeamEnd.SetActive(activate);
        }

        private void InitLaserBeam()
        {
            var lineRenderer = laserBeamStream.GetComponent<LineRenderer>();
            lineRenderer.startWidth = lineRenderer.endWidth = beamThickness;
        }
        
        #endregion
    }
}
