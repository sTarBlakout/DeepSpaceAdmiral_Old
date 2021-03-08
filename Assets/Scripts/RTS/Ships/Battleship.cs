using System.Collections.Generic;
using RTS.Interfaces;
using UnityEngine;

namespace RTS.Ships
{
    public class Battleship : ShipBase, ICarriable
    {
        [Header("Carrier")] 
        [SerializeField] private List<int> squadronIds;

        #region ICarriable Implementation
        
        public List<int> SquadronIds => squadronIds;

        public void LaunchSquadron(GameObject squadronPrefab)
        {
            Debug.Log("Launching squadron: " + squadronPrefab);
        }

        #endregion
    }
}
