using System.Collections.Generic;
using UnityEngine;

namespace RTS.Weapons
{
    public class OnboardLaserBeam : OnboardWeaponBase
    {
        [Header("Onboard Laser Weapon")] 
        
        [SerializeField] [Range(0, 1)] private float shootChance;
        [SerializeField] private float minTimeBetwShots;
        [SerializeField] private float laserFadeSpeed;
        
        private List<OnboardTurretLaserBeam> _laserTurrets = new List<OnboardTurretLaserBeam>();

        protected override void Init()
        {
            base.Init();
            foreach (var turretTransform in TurretTransforms)
            {
                var turretBeam = turretTransform.GetComponent<OnboardTurretLaserBeam>();
                if (turretBeam == null) continue;
                turretBeam.Init(minTimeBetwShots, laserFadeSpeed);
                _laserTurrets.Add(turretBeam);
            }
        }
        
        protected override void ProcessVisuals()
        {
            if (CurrentTarget == null) return;
            
            foreach (var laserTurret in _laserTurrets)
            {
                if (!(Random.value <= shootChance)) continue;
                var randomPointId = Random.Range(0, CurrentTarget.HitPositions.Count);
                laserTurret.Shoot(CurrentTarget.HitPositions[randomPointId].position);
            }
        }
    }
}