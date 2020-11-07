using System;
using Doozy.Engine.UI;
using UnityEngine;

namespace RTS.UI
{
    public class ManagerUI : MonoBehaviour
    {
        [SerializeField] private UIPopup shipControlPanelPopup;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                shipControlPanelPopup.Show();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                shipControlPanelPopup.Hide();
            }
        }
    }
}