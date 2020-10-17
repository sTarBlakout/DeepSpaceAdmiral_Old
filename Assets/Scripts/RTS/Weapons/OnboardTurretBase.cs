using System;
using UnityEngine;

namespace RTS.Weapons
{
    public class OnboardTurretBase : MonoBehaviour
    {
        [SerializeField] private Transform graphicsTransform;
        [SerializeField] private float rotatingAreaAngle;

        public void UpdateRotation(bool shoulProcess, Vector3 targetPos)
        {
            var turretTransform = transform;
            var turretForward = turretTransform.forward;
            
            var directionToTarget = (targetPos - turretTransform.position).normalized;
            var rotateAngle = Vector3.Angle(turretForward, directionToTarget);

            var process = rotateAngle < rotatingAreaAngle / 2 && shoulProcess;
            var targetDirection = process ? directionToTarget : turretForward;

            var direction = Vector3.RotateTowards(graphicsTransform.forward, targetDirection, 0.1f, 0f);
            graphicsTransform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
