using UnityEngine;
using Lean.Touch;
using RTS.Ships;

namespace RTS.Controls
{
    public class InputManager : MonoBehaviour
    {
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
            if (Physics.Raycast(finger.GetRay(), out var hitInfo, Mathf.Infinity))
            {
                var battleship = hitInfo.collider.GetComponent<BattleshipBase>();
                if (battleship != null)
                {
                    Debug.Log("Tapped on Battleship!");
                    return;
                }
                
                Debug.Log("Tapped on Infinite Space!");
            }
        }
    }
}
