using System;
using UnityEngine;
using RTS.Controls;

namespace RTS.Ships
{
    public class WeaponManager : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private MainWeaponType mainWeaponType;
        [SerializeField] private ProjectileType mainWeaponProjectileType;
        [SerializeField] private float damage = 1f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private Transform mainWeapon;

        [Header("Laser Beam VFX")] 
        [SerializeField] private GameObject laserBeamStart;
        [SerializeField] private GameObject laserBeamStream;
        [SerializeField] private GameObject laserBeamEnd;

        private float _facingTargetPrec;
        private float _dotForward;
        private bool _shouldAttack;
        private MonoBehaviour _currTarget;

        public MainWeaponType MainWeaponType => mainWeaponType;
        public float AttackRange => attackRange;

        private void FixedUpdate()
        {
            if (!_shouldAttack) return;
            
            ProcessMainWeapon();
        }

        public void InitWeaponSystem(float facingTargetPrec)
        {
            _facingTargetPrec = facingTargetPrec;
        }

        public void UpdateWeaponSystem(bool shouldAttack, float dotForward = 0f, MonoBehaviour target = null)
        {
            _dotForward = dotForward;
            _currTarget = target;
            _shouldAttack = shouldAttack;
            
            if (!shouldAttack)
                StopAllFire();
        }

        private void StopAllFire()
        {
            laserBeamStart.SetActive(false);
            laserBeamStream.SetActive(false);
            laserBeamEnd.SetActive(false);
        }

        private void ProcessMainWeapon()
        {
            switch (mainWeaponType)
            {
                case MainWeaponType.Front:
                    if (_dotForward >= _facingTargetPrec)
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
                    if (Physics.Raycast(mainWeapon.position, transform.forward, out var hit, attackRange))
                    {
                        var hitDamageable = hit.collider.GetComponent<IDamageable>();
                        if (hitDamageable != null)
                        {
                            if (!hitDamageable.IsFriend)
                            {
                                var lineRenderer = laserBeamStream.GetComponent<LineRenderer>();
                                lineRenderer.SetPosition(0, mainWeapon.position);
                                lineRenderer.SetPosition(1, hit.point);

                                laserBeamEnd.transform.position = hit.point;
                                
                                laserBeamStream.SetActive(true);
                                laserBeamStart.SetActive(true);
                                laserBeamEnd.SetActive(true);
                                
                                hitDamageable.Damage(damage);
                            }
                        }
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
