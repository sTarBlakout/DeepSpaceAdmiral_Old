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
        [SerializeField] private float beamMaxThickness;
        [SerializeField] private float beamMinThickness;
        [SerializeField] private float beamIncSpd;
        [SerializeField] private float beamDecSpd;
        [SerializeField] private GameObject laserBeamStart;
        [SerializeField] private GameObject laserBeamStream;
        [SerializeField] private GameObject laserBeamEnd;

        private LineRenderer _laserBeamRenderer;
        
        private bool _shouldAttackMain;
        private MonoBehaviour _currTarget;

        private bool _isMainGunFiring;
        private bool _isMainGunLocked;
        private float _mainGunTemp;

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

        private void InitWeaponSystem()
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
            var turnOnBeam = false;
            if (Physics.SphereCast(mainWeapon.position, beamMaxThickness, transform.forward, out var hit, attackRange))
            {
                var hitDamageable = hit.collider.GetComponent<IDamageable>();
                if (hitDamageable != null)
                {
                    if (!hitDamageable.IsFriend)
                    {
                        _isMainGunFiring = true;
                        if (_mainGunTemp < borderGunTemp) return;

                        turnOnBeam = true;
                        hitDamageable.Damage(damage);
                    }
                }
                _laserBeamRenderer.SetPosition(0, mainWeapon.position);
                _laserBeamRenderer.SetPosition(1, hit.point);
                laserBeamEnd.transform.position = hit.point;
            }
            
            _isMainGunFiring = turnOnBeam;
            ActivateLaserBeamVFX(turnOnBeam);
        }
        
        private void ActivateLaserBeamVFX(bool activate)
        {
            var isBeamActive = false;
            float width;
            if (activate)
            {
                width = Mathf.Min(_laserBeamRenderer.endWidth + beamIncSpd, beamMaxThickness);
                _laserBeamRenderer.startWidth = _laserBeamRenderer.endWidth = width;
                isBeamActive = true;
            }
            else
            {
                width = Mathf.Max(_laserBeamRenderer.endWidth - beamDecSpd, beamMinThickness);
                _laserBeamRenderer.startWidth = _laserBeamRenderer.endWidth = width;
                if (!Mathf.Approximately(_laserBeamRenderer.endWidth, beamMinThickness))
                    isBeamActive = true;
            }
            
            laserBeamStart.SetActive(isBeamActive);
            laserBeamStream.SetActive(isBeamActive);
            laserBeamEnd.SetActive(isBeamActive);
        }

        private void InitLaserBeam()
        {
            _laserBeamRenderer = laserBeamStream.GetComponent<LineRenderer>();
            _laserBeamRenderer.startWidth = _laserBeamRenderer.endWidth = beamMinThickness;
        }
        
        #endregion
    }
}
