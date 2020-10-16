using UnityEngine;

namespace RTS.Weapons
{
    public class OnboardLaserBeam : OnboardWeaponBase
    {
        protected override void Init()
        {
            
        }
        
        protected override void ProcessVisuals(Transform gunContainer, bool activate)
        {
            foreach (Transform gun in gunContainer)
            {
                Vector3 direction;
                if (activate)
                {
                    var targetDirection = CurrentTarget.Position - gun.position;
                    direction = Vector3.RotateTowards(gun.forward, targetDirection, 0.1f, 0f);
                }
                else
                {
                    direction = Vector3.RotateTowards(gun.forward, gunContainer.forward, 0.1f, 0f);
                }
                gun.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}