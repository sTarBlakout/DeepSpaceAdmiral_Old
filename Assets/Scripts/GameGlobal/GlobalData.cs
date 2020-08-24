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
    }
}