﻿using RTS.Ships;
using UnityEngine;

namespace RTS.Controls
{
    public interface IMoveable
    {
        bool IsReachedDestination { get; }
        void MoveToPositon(Vector3 position, Stance stance = Stance.Empty);
    }
}
