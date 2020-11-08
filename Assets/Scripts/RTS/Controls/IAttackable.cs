using UnityEngine;

namespace RTS.Controls
{
    public interface IAttackable
    {
        void AttackTarget(MonoBehaviour target);
        void ForceLooseTarget();
    }
}