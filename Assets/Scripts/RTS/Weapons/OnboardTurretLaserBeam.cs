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

        private bool _shooted;

        public void Init(float minTimeBetwShots, float laserFadeSpeed)
        {
            _minTimeBetwShots = minTimeBetwShots;
            _laserFadeSpeed = laserFadeSpeed;
            _maxBeamWidth = lineRenderer.startWidth;
        }
        
        protected override void TurretUpdater()
        {
            if (!_shooted) return;
            
            if (lineRenderer.startWidth != 0f)
            {
                var width = Mathf.Max(lineRenderer.startWidth - _laserFadeSpeed, 0f);
                lineRenderer.startWidth = lineRenderer.endWidth = width;
            }
            else
            {
                _lastTimeShot = Time.time;
                lineRenderer.enabled = false;
                _shooted = false;
            }
        }

        public void MakeShot(Vector3 targetPos)
        {
            if (ReadyToShoot && !_shooted && Time.time > _lastTimeShot + _minTimeBetwShots)
            {
                lineRenderer.enabled = true;
                lineRenderer.startWidth = lineRenderer.endWidth = _maxBeamWidth;
                lineRenderer.SetPosition(0, lineRenderer.transform.position);
                lineRenderer.SetPosition(1, targetPos);
                Instantiate(explosionPrefab, lineRenderer.GetPosition(1), Quaternion.identity);
                _shooted = true;
            }
        }
    }
}