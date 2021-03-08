using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.UI;
using RTS.Controls;
using UnityEngine;
using UnityEngine.UI;

public class LaunchSquadPopup : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private GameObject squadButtonPrefab;

    private UIPopup _popup;
    private LaunchSquadButton _selectedBtn;
    private List<LaunchSquadButton> _squadBtns = new List<LaunchSquadButton>();

    public int SelectedBtnId => _selectedBtn.ID;

    private void Awake()
    {
        _popup = GetComponent<UIPopup>();
        InputManager.I.OnObjectReselect += GenerateSquadBtns;
    }

    private void GenerateSquadBtns()
    {
        foreach (var btn in _squadBtns)
        {
            _popup.Data.Buttons.Remove(btn.UIButton);
            Destroy(btn);
        }
        _squadBtns.Clear();
        
        foreach (var id in InputManager.I.GetSquadronIds())
        {
            var newBtn = Instantiate(squadButtonPrefab, gridLayout.transform);
            var lsBtn = newBtn.GetComponent<LaunchSquadButton>();
            lsBtn.Init(id);
            lsBtn.UIButton.Button.onClick.AddListener(() => OnSquadButtonClick(lsBtn));
            _squadBtns.Add(lsBtn);
            _popup.Data.Buttons.Add(lsBtn.UIButton);
        }
    }

    private void OnSquadButtonClick(LaunchSquadButton clickedButton)
    {
        foreach (var btn in _squadBtns) btn.ActivateSelectParticle(false);
        clickedButton.ActivateSelectParticle(true);
        _selectedBtn = clickedButton;
    }
}
