using UnityEngine;

namespace RTS.Controls
{
    public interface IAttackable
    {
        void AttackTarget(ITargetable target);
        void ForceLooseTarget();
    }
}