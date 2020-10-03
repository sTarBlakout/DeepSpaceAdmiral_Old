﻿using System.Collections.Generic;
using UnityEngine;

namespace RTS.Controls
{
    public interface IExplosible
    {
        float ExplosionForce { get; }
        float ExplosionRadius { get; }
        Vector3 Position { get; }
        List<GameObject> CreatedSpaceDerbis { get; }
    }
}