using UnityEngine;

namespace RTS.Controls
{
    public class SelectedObject
    {
        private IMoveable _moveable;
        private IDamageable _damageable;

        private bool _isInit;
        public bool IsInit => _isInit;

        public void InitObject(MonoBehaviour monoBehaviourObj)
        {
            _damageable = monoBehaviourObj.GetComponent<IDamageable>();
            _moveable = monoBehaviourObj.GetComponent<IMoveable>();

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
            _damageable = null;
            _moveable = null;
            _isInit = false;
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