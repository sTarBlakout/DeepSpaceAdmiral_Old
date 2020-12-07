using System.Collections.Generic;
using RTS.Ships;
using UnityEngine;

namespace RTS.Controls
{
    public interface ITargetable
    {
        bool IsEnemy(byte askerTeamId);
        byte TeamId { get; }
        Transform Transform { get; }
        List<Transform> HitPositions { get; }
        IDamageable Damageable { get; }
    }
}