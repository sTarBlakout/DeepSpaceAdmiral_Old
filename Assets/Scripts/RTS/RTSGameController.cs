using GameGlobal;
using RTS.Controls;
using RTS.Ships;
using UnityEngine;

namespace RTS
{
    public class RTSGameController : MonoBehaviour
    {
        [SerializeField] private float upwardExplosionModifier;
        
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

        #region Public Methods

        public Collider[] CreateExplosionAtPos(Vector3 position, float radius, float force)
        {
            var collidersInRadius = Physics.OverlapSphere(position, radius);
            foreach (var colliderInRadius in collidersInRadius)
            {
                var randomModifier = Random.Range(-upwardExplosionModifier, upwardExplosionModifier);
                var rb = colliderInRadius.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddExplosionForce(force, position, radius, randomModifier);
            }

            return collidersInRadius;
        }
        
        #endregion
    }
}