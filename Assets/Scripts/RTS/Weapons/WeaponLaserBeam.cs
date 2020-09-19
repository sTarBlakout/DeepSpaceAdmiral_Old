using RTS.Controls;
using UnityEngine;

namespace RTS.Weapons
{
    public class WeaponLaserBeam : MainWeaponBase
    {
        [SerializeField] private float beamMaxThickness;
        [SerializeField] private float beamMinThickness;
        [SerializeField] private float beamIncSpd;
        [SerializeField] private float beamDecSpd;
        [SerializeField] private GameObject laserBeamStart;
        [SerializeField] private GameObject laserBeamStream;
        [SerializeField] private GameObject laserBeamEnd;
        
        private LineRenderer _laserBeamRenderer;

        public override void InitWeapon()
        {
            _laserBeamRenderer = laserBeamStream.GetComponent<LineRenderer>();
            _laserBeamRenderer.startWidth = _laserBeamRenderer.endWidth = beamMinThickness;

            MainGunTemp = minGunTemp;
        }

        public override void ProcessWeapon(bool process)
        {
            var turnOnBeam = false;
            ShouldHeat = false;

            if (process)
            {
                if (Physics.SphereCast(transform.position, beamMaxThickness, transform.forward, out var hit, attackRange))
                {
                    var hitDamageable = hit.collider.GetComponent<IDamageable>();
                    if (hitDamageable != null)
                    {
                        if (!hitDamageable.IsFriend)
                        {
                            ShouldHeat = true;
                            if (MainGunTemp < borderGunTemp) return;

                            turnOnBeam = true;
                            hitDamageable.Damage(damage);
                        }
                    }

                    _laserBeamRenderer.SetPosition(0, transform.position);
                    _laserBeamRenderer.SetPosition(1, hit.point);
                    laserBeamEnd.transform.position = hit.point;
                }
            }
            
            ActivateVFX(turnOnBeam);
        }

        public override void ProcessWeaponTemperature()
        {
            float value;
            if (ShouldHeat)
                value = mainGunWarmFactor;
            else
                value = -mainGunCoolFactor;

            MainGunTemp = Mathf.Clamp(MainGunTemp + value, minGunTemp, maxGunTemp);

            if (ShouldLock && MainGunTemp > borderGunTemp)
                return;

            ShouldLock = Mathf.Approximately(MainGunTemp, maxGunTemp);
        }

        protected void ActivateVFX(bool activate)
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
    }
}