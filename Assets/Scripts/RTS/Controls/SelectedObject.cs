using UnityEngine;

namespace RTS.Controls
{
    public class SelectedObject
    {
        private IMoveable _moveable;

        public void InitObject(MonoBehaviour monoBehaviourObj)
        {
            _moveable = monoBehaviourObj.GetComponent<IMoveable>();
        }

        public void TryMoveToPos(Vector3 position)
        {
            _moveable?.MoveToPositon(position);
        }
    }
}