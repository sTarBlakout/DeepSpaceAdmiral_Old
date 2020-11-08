using Doozy.Engine.UI;
using GameGlobal;
using UnityEngine;

namespace RTS.UI
{
    public class ManagerUI : MonoBehaviour
    {
        [SerializeField] private UIPopup shipControlPanelPopup;
        [SerializeField] private UIPopup changeFireModePopup;

        public void ActivatePopup(PopupType type, bool activate)
        {
            switch (type)
            {
                case PopupType.ShipControl: GlobalData.ShowPopup(shipControlPanelPopup, activate); break;
                case PopupType.ChangeFireMode: GlobalData.ShowPopup(changeFireModePopup, activate); break;
            }
        }
    }
}