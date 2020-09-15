using System.Collections;
using UnityEngine;
using Lean.Touch;
using GameGlobal;

namespace RTS.Controls
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private float doubleTapThreshold;
        
        private readonly SelectedObject _selectedObject = new SelectedObject();

        private float _lastTappedTime;
        
        private byte _doubleTapCounter;
        private Coroutine _doubleTapResetCoroutine;

        private void OnEnable()
        {
            LeanTouch.OnFingerTap += HandleFingerTap;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= HandleFingerTap;
        }

        private bool CheckForDoubleTap()
        {
            var isDoubleTap = _lastTappedTime + doubleTapThreshold >= Time.time;
            
            _doubleTapCounter++;
            if (_doubleTapCounter == 2)
                _doubleTapCounter = 0;

            if (_doubleTapResetCoroutine != null)
                StopCoroutine(_doubleTapResetCoroutine);
            _doubleTapResetCoroutine = StartCoroutine(ResetTapStatsInSeconds(doubleTapThreshold));
            
            return isDoubleTap;
        }

        private IEnumerator ResetTapStatsInSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _doubleTapCounter = 0;
        }

        private void HandleFingerTap(LeanFinger finger)
        {
            if (!Physics.Raycast(finger.GetRay(), out var hitInfo, Mathf.Infinity) || finger.IsOverGui) return;

            var isDoubleTap = CheckForDoubleTap();
            _lastTappedTime = Time.time;
            
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
                if (!_selectedObject.SameObject(monoBehaviorObj))
                    _selectedObject.InitObject(monoBehaviorObj);
                else
                {
                    // Zoom on selected
                    if (isDoubleTap)
                    {
                        CameraManager.Instance.SetLeanChaseDestination(monoBehaviorObj.transform);
                    }
                }
            }
            else
            {
                // Touched map, try to move ship there, if selected.
                var moveToPos = new Vector3(hitInfo.point.x, GlobalData.Instance.RtsShipsPosY, hitInfo.point.z);
                _selectedObject.TryMoveToPos(moveToPos);
            }
        }
    }
}
