using System.Collections.Generic;
using RTS.Ships;
using UnityEngine;

namespace RTS.Controls
{
    public interface IDamageable
    {
        bool IsEnemy(byte askerTeamId);
        bool CanBeDamaged();
        void Damage(float damage);
        byte TeamId { get; }
        Vector3 Position { get; }
        List<Transform> HitPositions { get; }
        List<DimensionPoint> DimensionPoints { get; }
    }
}