using System;
using UnityEngine;

namespace RTS.Controls
{
    public class SelectedObject
    {
        private MonoBehaviour _monoBehaviour;
        
        private IMoveable _moveable;
        private IDamageable _damageable;
        private IAttackable _attackable;
        private ISelectable _selectable;
        private IBehaviorSwitchable _behaviorSwitchable;

        private bool _isInit;
        public bool IsInit => _isInit;
        public MonoBehaviour Mono => _monoBehaviour;

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
            _moveable = _monoBehaviour.GetComponent<IMoveable>();
            _attackable = _monoBehaviour.GetComponent<IAttackable>();
            _behaviorSwitchable = _monoBehaviour.GetComponent<IBehaviorSwitchable>();

            if (_damageable != null)
            {
                // If enemy, don't select.
                if (_damageable.IsEnemy(1))
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
            _isInit = false;
        }

        public bool SameObject(MonoBehaviour monoBehaviourObj)
        {
            return _monoBehaviour == monoBehaviourObj;
        }

        public void TryMoveToPos(Vector3 position)
        {
            if (!_isInit) return;

            _moveable?.MoveToPositon(position);
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

        public bool TryInteractWithObject(MonoBehaviour monoBehaviourObj)
        {
            if (!_isInit) return false;
            
            var damageable = monoBehaviourObj.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (damageable.IsEnemy(1) && damageable.CanBeDamaged())
                {
                    _attackable.AttackTarget(monoBehaviourObj);
                    return true;
                }
            }

            return false;
        }

        public void SwitchBehavior(Enum behavior)
        {
            _behaviorSwitchable?.SwitchBehavior(behavior);
        }
    }
}