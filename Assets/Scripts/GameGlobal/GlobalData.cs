using System;
using UnityEngine;

namespace GameGlobal
{
    public class GlobalData : MonoBehaviour
    {
        [SerializeField] private float rtsShipsPosY;

        public float RtsShipsPosY => rtsShipsPosY;
        
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

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static bool VectorsApproxEqual(Vector3 v1, Vector3 v2, float precision)
        {
            return Vector3.SqrMagnitude(v1 - v2) < precision;
        }
    }
}