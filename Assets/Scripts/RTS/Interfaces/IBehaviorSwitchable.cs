using System;
using RTS.Controls;

namespace RTS.Interfaces
{
    public interface IBehaviorSwitchable
    {
        void SwitchBehavior(Enum behavior);
        Enum GetCurrBehavior(BehaviorType type);
    }
}