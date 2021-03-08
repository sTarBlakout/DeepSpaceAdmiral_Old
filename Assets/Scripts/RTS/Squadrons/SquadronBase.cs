using System.Collections;
using System.Collections.Generic;
using RTS.Interfaces;
using UnityEngine;

public class SquadronBase : MonoBehaviour
{
    [SerializeField] private int id;

    private ICarriable _parentCarrier;

    public int ID => id;

    public void Launch(ICarriable parentCarrier)
    {
        _parentCarrier = parentCarrier;
    }
}
