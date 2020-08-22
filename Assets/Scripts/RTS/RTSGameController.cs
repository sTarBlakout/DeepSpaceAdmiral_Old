using UnityEngine;

namespace RTS
{
    public class RTSGameController : MonoBehaviour
    {
        [SerializeField] private float shipsPosY;

        public float ShipsPosY => shipsPosY;

        private static RTSGameController _instance;
        public static RTSGameController Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = FindObjectOfType<RTSGameController>();
                return _instance;
            }
        }
    }
}