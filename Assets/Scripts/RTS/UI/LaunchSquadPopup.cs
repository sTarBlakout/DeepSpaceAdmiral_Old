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
    
    
    private List<GameObject> _squadBtns = new List<GameObject>();

    private void Awake()
    {
        InputManager.I.OnObjectReselect += GenerateSquadBtns;
    }

    private void GenerateSquadBtns()
    {
        foreach (var btn in _squadBtns) Destroy(btn);
        _squadBtns.Clear();
        
        foreach (var id in InputManager.I.CurrSelectedObject.GetSquadronIds())
        {
            var newBtn = Instantiate(squadButtonPrefab, gridLayout.transform);
            _squadBtns.Add(newBtn);
        }
    }
}
