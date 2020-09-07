using System.Collections.Generic;
using GameGlobal;
using UnityEngine;

namespace RTS.Ships
{
    public class EngineManager : MonoBehaviour
    {
        [SerializeField] private float startMainEngineMinDot; 
        
        private List<ParticleSystem> _mainEngines = new List<ParticleSystem>();

        private bool _isMainEngineActive;
        private float _slowDownEndPrec;

        #region Unity Events

        private void Awake()
        {
            foreach (Transform engineTransform in transform)
            {
                _mainEngines.Add(engineTransform.GetComponentInChildren<ParticleSystem>());
            }
        }
        
        #endregion
        

        #region Public API

        public void InitEngines(float slowDownEndPrec)
        {
            _slowDownEndPrec = slowDownEndPrec;
        }

        public void UpdateEngines(float dotForward, bool isShipMoving)
        {
            if (dotForward > startMainEngineMinDot && isShipMoving)
                ActivateMainEngines(true);
            else
                ActivateMainEngines(false);    
        }

        #endregion

        private void ActivateMainEngines(bool activate)
        {
            if (_isMainEngineActive == activate) return;
            _isMainEngineActive = activate;
            
            foreach (var engine in _mainEngines)
            {
                if (activate)
                    engine.Play();
                else
                    engine.Stop();
            }
        }
    }
}
