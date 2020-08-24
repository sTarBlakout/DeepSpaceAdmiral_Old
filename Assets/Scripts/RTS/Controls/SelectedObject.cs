using UnityEngine;

namespace RTS.Controls
{
    public class SelectedObject
    {
        private MonoBehaviour _monoBehaviour;
        
        private IMoveable _moveable;
        private IDamageable _damageable;

        private bool _isInit;
        public bool IsInit => _isInit;

        public void InitObject(MonoBehaviour monoBehaviourObj)
        {
            _monoBehaviour = monoBehaviourObj;
            
            _damageable = _monoBehaviour.GetComponent<IDamageable>();
            _moveable = _monoBehaviour.GetComponent<IMoveable>();

            if (_damageable != null)
            {
                if (!_damageable.IsFriend)
                {
                    UninitObject();
                    return;
                }
            }
            
            _isInit = true;
        }

        public void UninitObject()
        {
            _monoBehaviour = null;
            _damageable = null;
            _moveable = null;
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

        public bool TryInteractWithObject(MonoBehaviour monoBehaviourObj)
        {
            if (!_isInit) return false;
            
            var damageable = monoBehaviourObj.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (!damageable.IsFriend)
                    return true;
            }

            return false;
        }
    }
}