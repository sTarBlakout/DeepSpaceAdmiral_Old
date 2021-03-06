using RTS.Interfaces;
using UnityEngine;

namespace RTS.Ships
{
    public class Battleship : ShipBase, ICarriable
    {
        [Header("Carrier")] 
        [SerializeField] private int[] squadronIds;

        #region ICarriable Implementation

        public void LaunchSquadron(int id)
        {
            Debug.Log(id);
        }

        #endregion
    }
}
