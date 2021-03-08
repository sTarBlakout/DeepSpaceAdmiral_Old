using UnityEngine;

namespace ScriptableObjects.OverallData
{
    [CreateAssetMenu(fileName = "Global Data", menuName = "Scriptable Objects/Overall Data/Global Data")]
    public class GlobalData : ScriptableObject
    {
        #region Data
        
        [SerializeField] private int fps;

        #endregion
        
        #region Getters
        public int FPS => fps;

        #endregion
    }
}
