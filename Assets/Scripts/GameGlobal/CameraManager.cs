using System;
using System.Collections;
using Lean.Touch;
using UnityEngine;

namespace GameGlobal
{
    public class CameraManager : MonoBehaviour
    {
        private const float VectorEqualityPrecision = 0.5f;
        
        private LeanChase _leanChase;
        
        private Camera _mainCamera;
        public Camera MainCamera => _mainCamera;
        
        private static CameraManager _instance;
        public static CameraManager Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = FindObjectOfType<CameraManager>();
                return _instance;
            }
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _leanChase = GetComponent<LeanChase>();
        }

        private void Start()
        {
            _leanChase.enabled = false;
            
            var initPos = new Vector3(transform.position.x, GlobalData.Instance.RtsShipsPosY, transform.position.z);
            transform.position = initPos;
        }

        private void Update()
        {
            if (_leanChase.isActiveAndEnabled)
            {
                if (Vector3.SqrMagnitude(transform.position - _leanChase.Destination.position) < VectorEqualityPrecision)
                    _leanChase.enabled = false;
            }
        }
        
        public void SetLeanChaseDestination(Transform destTrans)
        {
            _leanChase.Destination = destTrans;
            _leanChase.enabled = true;
        }
    }
}
