using System.Collections.Generic;
using UnityEngine;

namespace RTS.Weapons
{
    public class OnboardLaserBeam : OnboardWeaponBase
    {
        private List<OnboardTurretLaserBeam> _laserTurrets = new List<OnboardTurretLaserBeam>();
        
        protected override void Init()
        {
            base.Init();
            foreach (var turretTransform in TurretTransforms)
            {
                var turretBeam = turretTransform.GetComponent<OnboardTurretLaserBeam>();
                if (turretBeam != null) _laserTurrets.Add(turretBeam);
            }
        }
        
        protected override void ProcessVisuals()
        {
            
        }
    }
}