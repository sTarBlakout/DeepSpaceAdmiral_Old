using UnityEngine;

namespace RTS.Controls
{
    public interface IDamageable
    {
        bool IsEnemy(byte askerTeamId);
        bool CanBeDamaged();
        void Damage(float damage);
        Vector3 Position { get; }
    }
}