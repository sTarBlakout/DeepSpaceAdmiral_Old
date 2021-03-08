using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableObjects.OverallData
{
    [CreateAssetMenu(fileName = "RTS Data", menuName = "Scriptable Objects/Overall Data/RTS Data")]
    public class RtsGameData : ScriptableObject
    {
        #region Data

        [Header("General")]
        [SerializeField] private float rtsShipsPosY;
        [SerializeField] private float unitSpeedMod;
        
        [Header("Battleship")]
        [SerializeField] private float battleshipSlowDownEndPrec = 0.1f;
        [SerializeField] private float battleshipFacingTargetPrec = 0.999f;
        [SerializeField] private float battleshipSideEngineTriggerMove = 0.5f;
        [SerializeField] private float battleshipSideEngineTriggerStay = 0.999f;

        [Header("Squadrons")] 
        [SerializeField] private List<GameObject> squadrons;
        
        #endregion

        #region Getters
        public float RtsShipsPosY => rtsShipsPosY;
        public float UnitSpeedMod => unitSpeedMod;
        public float BattleshipSlowDownEndPrec => battleshipSlowDownEndPrec * unitSpeedMod;
        public float BattleshipFacingTargetPrec => battleshipFacingTargetPrec;
        public float BattleshipSideEngineTriggerMove => battleshipSideEngineTriggerMove;
        public float BattleshipSideEngineTriggerStay => battleshipSideEngineTriggerStay;

        #endregion

        #region Public Methods

        public GameObject GetSquadron(int id)
        {
            return squadrons.FirstOrDefault(sq => sq.GetComponent<SquadronBase>().ID == id);
        }

        #endregion
    }
}