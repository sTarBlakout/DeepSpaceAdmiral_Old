using UnityEngine;

namespace RTS.Interfaces
{
    public interface IRamable
    {
        void Ramming(float damage, Vector3 ramPoint, IRamable rammer);
        void StopRamming(IRamable rammer);
    }
}