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
        [SerializeField] private GameObject laserBeamHit;
        [SerializeField] private Transform targetPoint;
        
        private LineRenderer _laserBeamRenderer;
        private ParticleSystem _laserBeamStart;
        private ParticleSystem _laserBeamHit;

        private bool _isBeamActive;

        public override void InitWeapon()
        {
            _laserBeamStart = laserBeamStart.GetComponent<ParticleSystem>();
            _laserBeamHit = laserBeamHit.GetComponent<ParticleSystem>();
            _laserBeamRenderer = laserBeamStream.GetComponent<LineRenderer>();
            _laserBeamRenderer.startWidth = _laserBeamRenderer.endWidth = beamMinThickness;

            MainGunTemp = minGunTemp;
            targetPoint.Translate(Vector3.forward * attackRange);
        }
        
        public override void ProcessWeapon(bool process)
        {
            var turnOnBeam = false;
            var processHit = true;
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
                processHit = beamEndPoint != targetPoint.position;
                UpdateBeamPoints(transform.position, beamEndPoint, processHit);
            }
            
            ActivateVFX(turnOnBeam, processHit);
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

        private void UpdateBeamPoints(Vector3 startPos, Vector3 endPos, bool updateHit = true)
        {
            _laserBeamRenderer.SetPosition(0, startPos);
            _laserBeamRenderer.SetPosition(1, endPos);
            if (updateHit)
                laserBeamHit.transform.position = endPos;
        }

        private void ActivateVFX(bool activate, bool playHit = true)
        { 
            float width;
            if (activate)
            {
                width = Mathf.Min(_laserBeamRenderer.startWidth + beamIncSpd, beamMaxThickness);
                _laserBeamRenderer.startWidth = _laserBeamRenderer.endWidth = width;
                _isBeamActive = true;
            }
            else
            {
                width = Mathf.Max(_laserBeamRenderer.startWidth - beamDecSpd, beamMinThickness);
                _laserBeamRenderer.startWidth = _laserBeamRenderer.endWidth = width;
                _isBeamActive = !Mathf.Approximately(_laserBeamRenderer.startWidth, beamMinThickness);
            }

            if (_isBeamActive)
            {
                if (!_laserBeamStart.isPlaying)
                    _laserBeamStart.Play();

                if (playHit)
                {
                    if (!_laserBeamHit.isPlaying)
                        _laserBeamHit.Play();
                }
                else
                {
                    if (_laserBeamHit.isPlaying)
                        _laserBeamHit.Stop();
                }

                if (!_laserBeamRenderer.enabled)
                    _laserBeamRenderer.enabled = true;
            }
            else
            {
                if (_laserBeamStart.isPlaying)
                    _laserBeamStart.Stop();
                if (_laserBeamHit.isPlaying)
                    _laserBeamHit.Stop();
                if (_laserBeamRenderer.enabled)
                    _laserBeamRenderer.enabled = false;
            }
        }
    }
}