using GameGlobal;
using RTS.Controls;
using UnityEngine;

namespace RTS.Weapons
{
    public class MainLaserBeam : MainWeaponBase
    {
        [Header("Laser Beam")]
        
        [SerializeField] private float beamMaxThickness;
        [SerializeField] private float beamMinThickness;
        [SerializeField] private float beamIncSpd;
        [SerializeField] private float beamDecSpd;
        [SerializeField] private GameObject laserBeamStart;
        [SerializeField] private GameObject laserBeamEnd;
        [SerializeField] private GameObject laserBeamStream;
        [SerializeField] private GameObject laserBeamHit;
        [SerializeField] private Transform targetPoint;
        
        private LineRenderer _laserBeamRenderer;
        private ParticleSystem _laserBeamStart;
        private ParticleSystem _laserBeamHit;
        private ParticleSystem _laserBeamEnd;

        private bool _isBeamActive;
        private bool _shouldDamage;
        private float _nextDamageTime;

        protected override void Init()
        {
            _laserBeamStart = laserBeamStart.GetComponent<ParticleSystem>();
            _laserBeamEnd = laserBeamEnd.GetComponent<ParticleSystem>();
            _laserBeamHit = laserBeamHit.GetComponent<ParticleSystem>();
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
                    var hitTargetable = hit.collider.GetComponent<ITargetable>();
                    if (hitTargetable != null)
                    {
                        if (hitTargetable.IsEnemy(SelectableShip.TeamId))
                        {
                            ShouldHeat = true;
                            if (MainGunTemp >= borderGunTemp)
                            {
                                turnOnBeam = process;
                                if (_nextDamageTime <= Time.time && _shouldDamage) 
                                {
                                    _nextDamageTime = Time.time + fireRate;
                                    hitTargetable.Damageable.Damage(damage);
                                }
                            }
                        }
                    }
                }
                UpdateBeamPoints(transform.position, beamEndPoint);
            }
            
            ProcessVFX(turnOnBeam, beamEndPoint != targetPoint.position);
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
            laserBeamHit.transform.position = endPos;
        }

        private void ProcessVFX(bool activate, bool hitSmth)
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
            
            _shouldDamage = Mathf.Approximately(_laserBeamRenderer.startWidth, beamMaxThickness);
            
            if (_laserBeamRenderer.enabled != _isBeamActive)
                _laserBeamRenderer.enabled = activate;

            GlobalData.ActivateParticle(_laserBeamStart, _isBeamActive);
            GlobalData.ActivateParticle(_laserBeamHit, _isBeamActive && hitSmth);
            GlobalData.ActivateParticle(_laserBeamEnd, _isBeamActive && !hitSmth);
        }
    }
}