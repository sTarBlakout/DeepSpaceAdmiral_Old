using System.Collections;
using System.Collections.Generic;
using RTS.Interfaces;
using UnityEngine;

public class SquadronBase : MonoBehaviour
{
    private ICarriable _parentCarrier;

    public void Launch(ICarriable parentCarrier)
    {
        _parentCarrier = parentCarrier;
    }
}
