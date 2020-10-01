using UnityEngine;

namespace RTS.Controls
{
    public interface IExplosible
    {
        bool IsFriend { get; }
        float ExplosionForce { get; }
        float ExplosionRadius { get; }
        Vector3 Position { get; }
    }
}