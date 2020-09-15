using GameGlobal;
using RTS.Controls;
using RTS.Ships;
using UnityEngine;

namespace RTS
{
    public class RTSGameController : MonoBehaviour
    {
        public GameObject shipToSpawn;
        
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

        public void TestShipBuild()
        {
            var spawnPos = new Vector3(0f, GlobalData.Instance.RtsShipsPosY, 0f);
            var ship = Instantiate(shipToSpawn, spawnPos, Quaternion.identity);
            ship.GetComponent<Battleship>().isFriend = false;
            ship.AddComponent<AIShipController>();
        }
    }
}