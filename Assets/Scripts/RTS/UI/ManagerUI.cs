using Doozy.Engine.UI;
using GameGlobal;
using UnityEngine;

namespace RTS.UI
{
    public class ManagerUI : MonoBehaviour
    {
        [SerializeField] private UIPopup shipControlPanelPopup;
        [SerializeField] private UIPopup changeFireModePopup;

        public void DisableAllControlUI()
        {
            // TODO: Finish this
        }

        public void ActivatePopup(PopupType type, bool activate)
        {
            switch (type)
            {
                case PopupType.ShipControl: ShowPopup(shipControlPanelPopup, activate); break;
                case PopupType.ChangeFireMode: ShowPopup(changeFireModePopup, activate); break;
            }
        }
        
        public void ChangeSelectedButton(PopupType type, int buttonIdx)
        {
            switch (type)
            {
                case PopupType.ShipControl: ChangeSelectedButtonInPopup(shipControlPanelPopup, buttonIdx); break;
                case PopupType.ChangeFireMode: ChangeSelectedButtonInPopup(changeFireModePopup, buttonIdx); break;
            }
        }
        
        public static void ShowPopup(UIPopup popup, bool show)
        {
            foreach (var button in popup.Data.Buttons) 
                button.Interactable = show;
            
            if (show)
                popup.Show();
            else
                popup.Hide();
        }

        public static void ChangeSelectedButtonInPopup(UIPopup popup, int buttonIdx)
        {
            if (popup.Data.ButtonsCount  <= buttonIdx) return;
            popup.SelectedButton = popup.Data.Buttons[buttonIdx].gameObject;
        }
    }
}