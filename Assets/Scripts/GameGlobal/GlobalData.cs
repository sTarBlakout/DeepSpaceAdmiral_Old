using System;
using UnityEngine;

namespace GameGlobal
{
    public class GlobalData : MonoBehaviour
    {
        #region Data

        [SerializeField] private float rtsShipsPosY;
        
        [SerializeField] private float battleshipSlowDownEndPrec = 0.1f;
        [SerializeField] private float battleshipFacingTargetPrec = 0.999f;
        [SerializeField] private float battleshipSideEngineTrigger = 0.5f;
        
        #endregion

        #region Getters
        public float RtsShipsPosY => rtsShipsPosY;
        public float BattleshipSlowDownEndPrec => battleshipSlowDownEndPrec;
        public float BattleshipFacingTargetPrec => battleshipFacingTargetPrec;
        public float BattleshipSideEngineTrigger => battleshipSideEngineTrigger;
        #endregion
        
        #region Singleton Implementation

        private static GlobalData _instance;
        public static GlobalData Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = FindObjectOfType<GlobalData>();
                return _instance;
            }
        }

        #endregion

        #region Unity Events

        private void Awake() 
        { 
            DontDestroyOnLoad(gameObject);
        }
        
        #endregion

        #region Static Functions

        public static bool VectorsApproxEqual(Vector3 v1, Vector3 v2, float precision)
        {
            return Vector3.SqrMagnitude(v1 - v2) < precision;
        }
        
        #endregion
    }
}