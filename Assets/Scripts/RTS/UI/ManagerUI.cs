using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.UI;
using UnityEngine;

namespace RTS.UI
{
    public class ManagerUI : MonoBehaviour
    {
        [SerializeField] private UIPopup shipControlPanelPopup;
        [SerializeField] private UIPopup changeFireModePopup;
        [SerializeField] private UIPopup launchSquadronPopup;

        private UICanvas _rtsCanvas;
        private readonly List<UIPopup> _controlsPopups = new List<UIPopup>();

        private bool _waitUntilAllControlsHidden;

        private void Start()
        {
            InitGameUI();
        }

        private void InitGameUI()
        {
            _rtsCanvas = FindObjectsOfType<UICanvas>().First(canvas => canvas.name == "Canvas - RTS Game");
            FindAllControlsPopups();
        }

        private void FindAllControlsPopups()
        {
            _controlsPopups.Clear();
            foreach (Transform child in _rtsCanvas.transform)
            {
                var popup = child.GetComponent<UIPopup>();
                if (popup != null && child.CompareTag("ControlsUI"))
                    _controlsPopups.Add(popup);
            }
        }

        public void HideAllControlsUI(bool waitAllHide = false)
        {
            _waitUntilAllControlsHidden = waitAllHide;
            foreach (var popup in _controlsPopups.Where(popup => !popup.IsHidden && !popup.IsHiding))
                ShowPopup(popup, false);
        }

        public void ActivatePopup(PopupType type, bool activate)
        {
            switch (type)
            {
                case PopupType.ShipControl: ShowPopup(shipControlPanelPopup, activate); break;
                case PopupType.ChangeFireMode: ShowPopup(changeFireModePopup, activate); break;
                case PopupType.LaunchSquadron: ShowPopup(launchSquadronPopup, activate); break;
            }
        }
        
        public void ChangeSelectedButton(PopupType type, int buttonIdx)
        {
            switch (type)
            {
                case PopupType.ShipControl: shipControlPanelPopup.SetSelectedButton(buttonIdx); break;
                case PopupType.ChangeFireMode: changeFireModePopup.SetSelectedButton(buttonIdx); break;
                case PopupType.LaunchSquadron: launchSquadronPopup.SetSelectedButton(buttonIdx); break;
            }
        }
        
        public void ShowPopup(UIPopup popup, bool show)
        {
            if (show && _waitUntilAllControlsHidden)
            {
                StartCoroutine(ShowPopupAfterAllControlsHide(popup));
                return;
            }
            
            foreach (var button in popup.Data.Buttons) 
                button.Interactable = show;
            
            if (show)
                popup.Show();
            else
                popup.Hide();
        }

        private IEnumerator ShowPopupAfterAllControlsHide(UIPopup popup)
        {
            yield return new WaitUntil(AreAllControlsHidden);
            _waitUntilAllControlsHidden = false;
            ShowPopup(popup, true);
        }

        private bool AreAllControlsHidden()
        {
            return !_controlsPopups.Any(popup => popup.IsHiding || popup.IsVisible);
        }
    }
}