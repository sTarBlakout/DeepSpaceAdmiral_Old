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
        IDamageable Damageable { get; }
        List<Transform> HitPositions { get; }
        List<DimensionPoint> DimensionPoints { get; }
    }
}