using UnityEngine;

namespace RTS.Controls
{
    public interface IDamageable
    {
        bool IsFriend { get; }
        void Damage(float damage);
    }
}