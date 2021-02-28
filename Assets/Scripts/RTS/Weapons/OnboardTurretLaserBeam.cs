using System;
using GameGlobal;
using UnityEngine;

namespace RTS.Weapons
{
    public class OnboardTurretLaserBeam : OnboardTurretBase
    {
        [Header("Laser Beam")] 
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private GameObject explosionPrefab;

        private float _minTimeBetwShots;
        private float _laserFadeSpeed;
        private float _lastTimeShot;
        private float _maxBeamWidth;

        private bool _readyToShoot;
        private bool _shooted;

        private Vector3 _posToShoot;

        public void Init(float minTimeBetwShots, float laserFadeSpeed)
        {
            _minTimeBetwShots = minTimeBetwShots;
            _laserFadeSpeed = laserFadeSpeed;
            _maxBeamWidth = lineRenderer.startWidth;
        }

        protected override void TurretUpdater()
        {
            _readyToShoot = RotationBehavior();
            ProcessBeam();
        }

        private void ProcessBeam()
        {
            if (!_shooted) return;
            if (lineRenderer.startWidth != 0f)
            {
                var width = Mathf.Max(lineRenderer.startWidth - _laserFadeSpeed, 0f);
                lineRenderer.startWidth = lineRenderer.endWidth = width;
            }
            else
            {
                _posToShoot = Vector3.zero;
                _lastTimeShot = Time.time;
                lineRenderer.enabled = false;
                _shooted = false;
            }
        }

        protected override bool RotationBehavior()
        {
            var directionToTarget = (_posToShoot - transform.position).normalized;
            RotateGraphics(ShouldAttack ? directionToTarget : graphicsContainer.forward);
            var dotProd = Vector3.Dot(directionToTarget, graphicsRotator.forward);
            return dotProd > AllData.Instance.RtsGameData.BattleshipFacingTargetPrec && ShouldAttack;
        }

        public void Shoot(Vector3 targetPos)
        {
            if (_posToShoot == Vector3.zero)
                _posToShoot = targetPos;

            if (_readyToShoot && !_shooted && Time.time > _lastTimeShot + _minTimeBetwShots)
            {
                lineRenderer.enabled = true;
                lineRenderer.startWidth = lineRenderer.endWidth = _maxBeamWidth;
                lineRenderer.SetPosition(0, lineRenderer.transform.position);
                lineRenderer.SetPosition(1, _posToShoot);
                Instantiate(explosionPrefab, lineRenderer.GetPosition(1), Quaternion.identity);
                _shooted = true;
            }
        }
    }
}