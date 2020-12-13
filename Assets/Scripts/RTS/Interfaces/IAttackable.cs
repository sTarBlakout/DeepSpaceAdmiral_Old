using RTS.Controls;

namespace RTS.Interfaces
{
    public interface IAttackable
    {
        void AttackTarget(ITargetable target);
        void ForceLooseTarget();
    }
}