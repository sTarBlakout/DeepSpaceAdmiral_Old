﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.GlobalData
{
    [CreateAssetMenu(fileName = "RTS Data", menuName = "ScriptableObjects/GlobalData/RTS Data")]
    public class RtsGameData : ScriptableObject
    {
        #region Data

        [SerializeField] private float rtsShipsPosY;
        [SerializeField] private float unitSpeedMod;
        
        [SerializeField] private float battleshipSlowDownEndPrec = 0.1f;
        [SerializeField] private float battleshipFacingTargetPrec = 0.999f;
        [SerializeField] private float battleshipSideEngineTriggerMove = 0.5f;
        [SerializeField] private float battleshipSideEngineTriggerStay = 0.999f;
        
        #endregion

        #region Getters
        
        public float RtsShipsPosY => rtsShipsPosY;
        public float UnitSpeedMod => unitSpeedMod;
        public float BattleshipSlowDownEndPrec => battleshipSlowDownEndPrec * unitSpeedMod;
        public float BattleshipFacingTargetPrec => battleshipFacingTargetPrec;
        public float BattleshipSideEngineTriggerMove => battleshipSideEngineTriggerMove;
        public float BattleshipSideEngineTriggerStay => battleshipSideEngineTriggerStay;
        
        #endregion
    }
}