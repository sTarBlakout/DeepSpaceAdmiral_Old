using System.Collections.Generic;
using UnityEngine;

namespace RTS.Interfaces
{
    public interface ICarriable
    {
        void LaunchSquadron(GameObject squadronPrefab);
        List<int> SquadronIds { get; }
        List<Transform> SquadronSpawnPoints { get; }
    }
}