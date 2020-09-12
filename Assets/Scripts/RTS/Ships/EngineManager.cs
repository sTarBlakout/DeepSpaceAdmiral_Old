using System;
using System.Collections.Generic;
using GameGlobal;
using UnityEngine;

namespace RTS.Ships
{
    public class EngineManager : MonoBehaviour
    {
        #region Data

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

        private float _sideEngineActTrigMove;
        private float _sideEngineActTrigStay;

        #endregion

        #region Unity Events

        private void Awake()
        {
            InitEngines();

            _sideEngineActTrigMove = GlobalData.Instance.BattleshipSideEngineTriggerMove;
            _sideEngineActTrigStay = GlobalData.Instance.BattleshipSideEngineTriggerStay;
        }

        #endregion

        #region Public API

        public void UpdateEngines(float dotForward, float dotSide, bool isShipMoving)
        {
            bool shouldSideEnginesWork;
                
            if (isShipMoving)
            {
                ActivateEngineSection(EngineSection.Main, dotForward > startMainEngineMinDot);
                shouldSideEnginesWork = dotForward < _sideEngineActTrigMove;
            }
            else
            {
                ActivateEngineSection(EngineSection.Main, false);
                shouldSideEnginesWork = dotForward < _sideEngineActTrigStay;
            }
            
            if (dotSide > 0)
            {
                ActivateEngineSection(EngineSection.Right, shouldSideEnginesWork);
                ActivateEngineSection(EngineSection.Left, false);
            }
            else if (dotSide < 0)
            {
                ActivateEngineSection(EngineSection.Left, shouldSideEnginesWork);
                ActivateEngineSection(EngineSection.Right, false);
            }
            else
            {
                ActivateEngineSection(EngineSection.Left, false);
                ActivateEngineSection(EngineSection.Right, false);
            }
        }

        #endregion

        #region Private Functions
        
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

        #endregion
    }
}
