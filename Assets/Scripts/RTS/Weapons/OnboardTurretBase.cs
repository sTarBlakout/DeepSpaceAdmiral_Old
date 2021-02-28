using System.Collections.Generic;
using GameGlobal;
using UnityEngine;

namespace RTS.Weapons
{
    public abstract class OnboardTurretBase : MonoBehaviour
    {
        [Header("General Stats")]
        [SerializeField] protected Transform graphicsContainer;
        [SerializeField] protected Transform graphicsRotator;
        [SerializeField] private float maxRotatingAngleX;
        [SerializeField] private float maxRotatingAngleY;
        [SerializeField] private float rotatingSpeed;

        private Vector3 _directionToTarget;

        protected bool ShouldAttack { get; private set; }

        public void UpdateTurret(bool shouldProcess, Vector3 targetPosition)
        {
            var turretTransform = transform;
            var turretForward = turretTransform.forward;
            _directionToTarget = (targetPosition - turretTransform.position).normalized;
            var rotateAngle = Vector3.Angle(turretForward, _directionToTarget);
            ShouldAttack = rotateAngle < maxRotatingAngleY && shouldProcess;
            
            TurretUpdater();
        }
        
        protected virtual void TurretUpdater()
        {
            RotationBehavior();
        }

        protected virtual bool RotationBehavior()
        {
            RotateGraphics(ShouldAttack ? _directionToTarget : graphicsContainer.forward);
            var dotProd = Vector3.Dot(_directionToTarget, graphicsRotator.forward);
            var readyToShoot = dotProd > AllData.I.RtsGameData.BattleshipFacingTargetPrec && ShouldAttack;
            return readyToShoot;
        }

        protected void RotateGraphics(Vector3 targetDirection)
        {
            var direction = Vector3.RotateTowards(graphicsRotator.forward, targetDirection, rotatingSpeed, 0f);
            graphicsRotator.rotation = Quaternion.LookRotation(direction);

            var graphicsEulerAngles = graphicsRotator.localEulerAngles;
            var xClamped = AllData.ClampAngle(graphicsEulerAngles.x, maxRotatingAngleX);
            var correctedAngles = new Vector3(xClamped, graphicsEulerAngles.y, 0f);
            graphicsRotator.localEulerAngles = correctedAngles;
        }
    }
}
