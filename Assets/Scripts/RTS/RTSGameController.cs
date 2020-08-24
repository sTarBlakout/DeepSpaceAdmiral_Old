using UnityEngine;

namespace RTS
{
    public class RTSGameController : MonoBehaviour
    {
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