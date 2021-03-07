using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

public class LaunchSquadButton : MonoBehaviour
{
    [SerializeField] private UIButton uiButton;
    [SerializeField] private Text label;
    [SerializeField] private ParticleSystem particle; 
    
    private int _id;

    public int ID => _id;
    public UIButton UIButton => uiButton;

    public void Init(int id)
    {
        _id = id;
        label.text = id.ToString();
    }

    public void ActivateSelectParticle(bool activate)
    {
        if (activate) particle.Play();
        else particle.Stop(true,  ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
