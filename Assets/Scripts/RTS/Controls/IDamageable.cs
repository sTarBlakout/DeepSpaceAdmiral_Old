using UnityEngine;

namespace RTS.Controls
{
    public interface IDamageable
    {
        bool IsFriend { get; }
        bool CanBeDamaged();
        void Damage(float damage);
    }
}