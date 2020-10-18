using System;
using UnityEngine;

namespace RTS.Weapons
{
    public class OnboardTurretBase : MonoBehaviour
    {
        [SerializeField] private Transform graphicsContainer;
        [SerializeField] private Transform graphicsRotator;
        [SerializeField] private float maxRotatingAngleX;
        [SerializeField] private float maxRotatingAngleY;
        [SerializeField] private float rotatingSpeed;

        public void UpdateRotation(bool shouldProcess, Vector3 targetPos)
        {
            var turretTransform = transform;
            var turretForward = turretTransform.forward;
            
            var directionToTarget = (targetPos - turretTransform.position).normalized;
            var rotateAngle = Vector3.Angle(turretForward, directionToTarget);

            var process = rotateAngle < maxRotatingAngleY && shouldProcess;
            RotateGraphics(process ? directionToTarget : graphicsContainer.forward);
        }

        private void RotateGraphics(Vector3 targetDirection)
        {
            var direction = Vector3.RotateTowards(graphicsRotator.forward, targetDirection, rotatingSpeed, 0f);
            graphicsRotator.rotation = Quaternion.LookRotation(direction);

            var graphicsEulerAngles = graphicsRotator.localEulerAngles;
            var xClamped = ClampAngle(graphicsEulerAngles.x, maxRotatingAngleX);
            var correctedAngles = new Vector3(xClamped, graphicsEulerAngles.y, 0f);
            graphicsRotator.localEulerAngles = correctedAngles;
        }
        
        private static float ClampAngle(float angle, float neededAngle) 
        {
            if (angle > 180)
                angle = Mathf.Clamp(angle, 360f - neededAngle, 360f);
            else
                angle = Mathf.Clamp(angle, 0, neededAngle);

            return angle;
        }
    }
}
