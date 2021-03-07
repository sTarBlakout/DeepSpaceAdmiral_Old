using System;
using RTS.Controls;
using UnityEngine;

namespace RTS.UI
{
    public class SquadLaunchLayout : MonoBehaviour
    {
        private void Start()
        {
            foreach (var VARIABLE in InputManager.I.CurrSelectedObject.GetSquadronIds())
            {
                Debug.Log(VARIABLE);
            }
        }
    }
}
