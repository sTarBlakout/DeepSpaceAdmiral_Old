using System.Collections.Generic;
using System.Linq;
using RTS.Interfaces;
using UnityEngine;

namespace RTS.Ships
{
    public class Battleship : ShipBase, ICarriable
    {
        [Header("Carrier")] 
        [SerializeField] private List<int> squadronIds;
        [SerializeField] private Transform squadSpawnPointsTransform;
        
        #region ICarriable Implementation
        
        public List<int> SquadronIds => squadronIds;
        public List<Transform> SquadronSpawnPoints => squadSpawnPointsTransform.Cast<Transform>().ToList();

        public void LaunchSquadron(GameObject squadronPrefab)
        {
            var squadron = Instantiate(squadronPrefab, transform);
            var squadronBase = squadron.GetComponent<SquadronBase>();
            squadronBase.Launch(this);
        }

        #endregion
    }
}
