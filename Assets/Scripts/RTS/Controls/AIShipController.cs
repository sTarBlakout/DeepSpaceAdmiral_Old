using System;
using GameGlobal;
using RTS.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RTS.Controls
{
    public class AIShipController : MonoBehaviour
    {
        private IMoveable _moveable;
        private IAttackable _attackable;

        private Vector3 _targetPos;

        private void Awake()
        {
            _moveable = GetComponent<IMoveable>();
            _attackable = GetComponent<IAttackable>();
        }

        private void Update()
        {
            if (!_moveable.IsReachedDestination) return;

            var randomPoint = Random.insideUnitSphere * 100;
            _targetPos = new Vector3(randomPoint.x, AllData.I.RtsGameData.RtsShipsPosY, randomPoint.z);
            _moveable.MoveToPositon(_targetPos);
        }
    }
}