using UnityEngine;

namespace RTS.Weapons
{
    public class OnboardTurretLaserBeam : OnboardTurretBase
    {
        [Header("Laser Beam")] 
        [SerializeField] private LineRenderer lineRenderer;

        public void MakeShot(Vector3 targetPos)
        {
            if (ReadyToShoot)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, lineRenderer.transform.position);
                lineRenderer.SetPosition(1, targetPos);
            }
            else
                lineRenderer.enabled = false;
        }
    }
}