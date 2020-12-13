using System.Collections.Generic;
using UnityEngine;

namespace RTS.Interfaces
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