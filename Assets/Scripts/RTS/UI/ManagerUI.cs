using Doozy.Engine.UI;
using GameGlobal;
using UnityEngine;

namespace RTS.UI
{
    public class ManagerUI : MonoBehaviour
    {
        [SerializeField] private UIPopup shipControlPanelPopup;

        public void ActivatePopup(PopupType type, bool activate)
        {
            switch (type)
            {
                case PopupType.ShipControlPanel: GlobalData.ShowPopup(shipControlPanelPopup, activate); break;
            }
        }
    }
}