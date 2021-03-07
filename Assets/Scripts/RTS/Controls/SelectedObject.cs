using System;
using System.Collections.Generic;
using RTS.Interfaces;
using RTS.Ships;
using UnityEngine;

namespace RTS.Controls
{
    public class SelectedObject
    {
        #region Data

        private MonoBehaviour _monoBehaviour;
        
        private IMoveable _moveable;
        private IDamageable _damageable;
        private ITargetable _targetable;
        private IAttackable _attackable;
        private ISelectable _selectable;
        private IBehaviorSwitchable _behaviorSwitchable;
        private ICarriable _carriable;

        private bool _isInit;
        
        #endregion

        #region Getters
        
        public bool IsInit => _isInit;
        public MonoBehaviour Mono => _monoBehaviour;

        #endregion

        #region Selection & Init Logic

        public void InitObject(MonoBehaviour monoBehaviourObj)
        {
            UninitObject();
            
            _monoBehaviour = monoBehaviourObj;
            _selectable = _monoBehaviour.GetComponent<ISelectable>();

            if (_selectable == null || !_selectable.CanSelect())
            {
                UninitObject();
                return;
            }

            _damageable = _monoBehaviour.GetComponent<IDamageable>();
            _targetable = _monoBehaviour.GetComponent<ITargetable>();
            _moveable = _monoBehaviour.GetComponent<IMoveable>();
            _attackable = _monoBehaviour.GetComponent<IAttackable>();
            _behaviorSwitchable = _monoBehaviour.GetComponent<IBehaviorSwitchable>();
            _carriable = _monoBehaviour.GetComponent<ICarriable>();

            if (_damageable != null)
            {
                // If enemy, don't select.
                if (_targetable.IsEnemy(_targetable.TeamId))
                {
                    UninitObject();
                    return;
                }
            }

            _selectable.Select();
            
            _isInit = true;
        }

        public void UninitObject()
        {
            if (_isInit)
                _selectable.Unselect();
            
            _monoBehaviour = null;
            _damageable = null;
            _moveable = null;
            _attackable = null;
            _behaviorSwitchable = null;
            _carriable = null;
            _isInit = false;
        }
        
        public bool SameObject(MonoBehaviour monoBehaviourObj)
        {
            return _monoBehaviour == monoBehaviourObj;
        }
        
        #endregion

        #region Selected Object logic

        public bool TryInteractWithObject(MonoBehaviour monoBehaviourObj)
        {
            if (!_isInit) return false;
            
            var damageable = (IDamageable)monoBehaviourObj;
            var targetable = (ITargetable)monoBehaviourObj;
            if (damageable != null && targetable != null)
            {
                if (targetable.IsEnemy(_targetable.TeamId) && damageable.CanBeDamaged())
                {
                    _attackable.AttackTarget(targetable);
                    return true;
                }
            }

            return false;
        }

        public void TryMoveToPos(Vector3 position)
        {
            if (!_isInit) return;
            _moveable?.MoveToPositon(position, State.Order);
        }

        public void StopAllActions()
        {
            _moveable?.ForceStop();
            _attackable?.ForceLooseTarget();
        }

        public Enum GetCurrBehavior(BehaviorType type)
        {
            return _behaviorSwitchable.GetCurrBehavior(type);
        }

        public void SwitchBehavior(Enum behavior)
        {
            _behaviorSwitchable?.SwitchBehavior(behavior);
        }

        public List<int> GetSquadronIds()
        {
            return _carriable != null ? new List<int>(_carriable.SquadronIds) : new List<int>();
        }

        #endregion
    }
}