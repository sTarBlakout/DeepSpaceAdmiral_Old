using UnityEngine;
using Lean.Touch;

namespace RTS.Controls
{
    public class InputManager : MonoBehaviour
    {
        private SelectedObject _selectedObject = new SelectedObject();
        
        private void OnEnable()
        {
            LeanTouch.OnFingerTap += HandleFingerTap;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= HandleFingerTap;
        }

        private void HandleFingerTap(LeanFinger finger)
        {
            if (!Physics.Raycast(finger.GetRay(), out var hitInfo, Mathf.Infinity)) return;
            
            var monoBehaviorObj = hitInfo.collider.GetComponent<MonoBehaviour>();
            if (monoBehaviorObj != null)
            {
                if (_selectedObject.IsInit)
                {
                    // If can interact somehow, don't reselect.
                    if (_selectedObject.TryInteractWithObject(monoBehaviorObj))
                        return;
                }
                
                // Can't interact in any way, reselect.
                _selectedObject.InitObject(monoBehaviorObj);
            }
            else
            {
                // Touched map, try to move ship there, if selected.
                var moveToPos = new Vector3(hitInfo.point.x, RTSGameController.Instance.ShipsPosY, hitInfo.point.z);
                _selectedObject.TryMoveToPos(moveToPos);
            }
        }
    }
}
