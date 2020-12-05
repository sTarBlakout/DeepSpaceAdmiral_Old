using System.Collections.Generic;
using RTS.Ships;
using UnityEngine;

namespace RTS.Controls
{
    public interface ITargetable
    {
        bool IsEnemy(byte askerTeamId);
        byte TeamId { get; }
        Vector3 Position { get; }
        DimensionPoints DimensionPoints { get; }
        List<Transform> HitPositions { get; }
        IDamageable Damageable { get; }
    }
}