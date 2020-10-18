using UnityEngine;

namespace RTS.Weapons
{
    public class OnboardTurretLaserBeam : OnboardTurretBase
    {
        [Header("Laser Beam")] 
        [SerializeField] private LineRenderer lineRenderer;
    }
}