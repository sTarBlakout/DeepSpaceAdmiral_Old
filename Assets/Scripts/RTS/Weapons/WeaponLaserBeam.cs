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
        [SerializeField] private Transform targetPoint;
        
        private LineRenderer _laserBeamRenderer;
        private ParticleSystem _laserBeamStart;
        private ParticleSystem _laserBeamEnd;

        private bool _isBeamActive;

        public override void InitWeapon()
        {
            _laserBeamStart = laserBeamStart.GetComponent<ParticleSystem>();
            _laserBeamEnd = laserBeamEnd.GetComponent<ParticleSystem>();
            _laserBeamRenderer = laserBeamStream.GetComponent<LineRenderer>();
            _laserBeamRenderer.startWidth = _laserBeamRenderer.endWidth = beamMinThickness;

            MainGunTemp = minGunTemp;
            targetPoint.Translate(Vector3.forward * attackRange);
        }
        
        public override void ProcessWeapon(bool process)
        {
            var turnOnBeam = false;
            var beamEndPoint = targetPoint.position;
            ShouldHeat = false;

            if (process || _isBeamActive)
            {
                if (Physics.SphereCast(transform.position, beamMaxThickness, transform.forward, out var hit, attackRange))
                {
                    beamEndPoint = hit.point;
                    var hitDamageable = hit.collider.GetComponent<IDamageable>();
                    if (hitDamageable != null)
                    {
                        if (!hitDamageable.IsFriend)
                        {
                            ShouldHeat = true;
                            if (MainGunTemp >= borderGunTemp)
                            {
                                turnOnBeam = process;
                                hitDamageable.Damage(damage);
                            }
                        }
                    }
                }
                UpdateBeamPoints(transform.position, beamEndPoint);
            }
            
            ActivateVFX(turnOnBeam, beamEndPoint != targetPoint.position);
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

        private void UpdateBeamPoints(Vector3 startPos, Vector3 endPos)
        {
            _laserBeamRenderer.SetPosition(0, startPos);
            _laserBeamRenderer.SetPosition(1, endPos);
            laserBeamEnd.transform.position = endPos;
        }

        private void ActivateVFX(bool activate, bool playEnd = true)
        {
            float width;
            if (activate)
            {
                width = Mathf.Min(_laserBeamRenderer.endWidth + beamIncSpd, beamMaxThickness);
                _laserBeamRenderer.startWidth = _laserBeamRenderer.endWidth = width;
                _isBeamActive = true;
            }
            else
            {
                width = Mathf.Max(_laserBeamRenderer.endWidth - beamDecSpd, beamMinThickness);
                _laserBeamRenderer.startWidth = _laserBeamRenderer.endWidth = width;
                _isBeamActive = !Mathf.Approximately(_laserBeamRenderer.endWidth, beamMinThickness);
            }

            if (_isBeamActive)
            {
                if (!_laserBeamStart.isPlaying)
                    _laserBeamStart.Play();

                if (playEnd)
                {
                    if (!_laserBeamEnd.isPlaying)
                        _laserBeamEnd.Play();
                }
                else
                {
                    if (_laserBeamEnd.isPlaying)
                        _laserBeamEnd.Stop();
                }

                if (!_laserBeamRenderer.enabled)
                    _laserBeamRenderer.enabled = true;
            }
            else
            {
                if (_laserBeamStart.isPlaying)
                    _laserBeamStart.Stop();
                if (_laserBeamEnd.isPlaying)
                    _laserBeamEnd.Stop();
                if (_laserBeamRenderer.enabled)
                    _laserBeamRenderer.enabled = false;
            }
        }
    }
}