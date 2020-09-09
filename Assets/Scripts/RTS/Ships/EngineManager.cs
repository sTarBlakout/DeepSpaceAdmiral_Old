using System;
using System.Collections.Generic;
using GameGlobal;
using UnityEngine;

namespace RTS.Ships
{
    public class EngineManager : MonoBehaviour
    {
        [SerializeField] private Transform mainEngineSection;
        [SerializeField] private Transform rightEngineSection;
        [SerializeField] private Transform leftEngineSection;
        
        [SerializeField] private float startMainEngineMinDot; 
        
        private List<ParticleSystem> _mainEngines = new List<ParticleSystem>();
        private List<ParticleSystem> _rightEngines = new List<ParticleSystem>();
        private List<ParticleSystem> _leftEngines = new List<ParticleSystem>();

        private bool _isMainEngineActive;
        private bool _isRightEngineActive;
        private bool _isLeftEngineActive;

        private float _sideEngineActTrig;

        #region Unity Events

        private void Awake()
        {
            InitEngines();

            _sideEngineActTrig = GlobalData.Instance.BattleshipSideEngineTrigger;
        }
         
        #endregion
        
        #region Public API

        public void UpdateEngines(float dotForward, float dotSide, bool isShipMoving)
        {
            if (isShipMoving)
                ActivateEngineSection(EngineSection.Main, dotForward > startMainEngineMinDot);
            else
                ActivateEngineSection(EngineSection.Main, false);

            var shouldSides = dotForward < _sideEngineActTrig;
            if (dotSide > 0)
            {
                ActivateEngineSection(EngineSection.Right, shouldSides);
                ActivateEngineSection(EngineSection.Left, false);
            }
            else if (dotSide < 0)
            {
                ActivateEngineSection(EngineSection.Left, shouldSides);
                ActivateEngineSection(EngineSection.Right, false);
            }
            else
            {
                ActivateEngineSection(EngineSection.Left, false);
                ActivateEngineSection(EngineSection.Right, false);
            }
        }

        #endregion
        
        private void InitEngines()
        {
            foreach (Transform engineTransform in mainEngineSection)
            {
                _mainEngines.Add(engineTransform.GetComponentInChildren<ParticleSystem>());
            }
            foreach (Transform engineTransform in leftEngineSection)
            {
                _leftEngines.Add(engineTransform.GetComponentInChildren<ParticleSystem>());
            }
            foreach (Transform engineTransform in rightEngineSection)
            {
                _rightEngines.Add(engineTransform.GetComponentInChildren<ParticleSystem>());
            }
        }

        private void ActivateEngineSection(EngineSection section, bool activate)
        {
            switch (section)
            {
                case EngineSection.Main:
                    ActivateEngines(_mainEngines, activate, ref _isMainEngineActive);
                    break;
                case EngineSection.Right:
                    ActivateEngines(_rightEngines, activate, ref _isRightEngineActive);
                    break;
                case EngineSection.Left:
                    ActivateEngines(_leftEngines, activate, ref _isLeftEngineActive);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(section), section, null);
            }
        }

        private void ActivateEngines(List<ParticleSystem> engineList, bool activate, ref bool activeIndicator)
        {
            if (activeIndicator == activate) return;
            activeIndicator = activate;

            foreach (var particle in engineList)
            {
                if (activate)
                    particle.Play();
                else
                    particle.Stop();
            }
        }
    }
}
