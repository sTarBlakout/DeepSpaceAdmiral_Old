using System.Collections;
using System.Collections.Generic;
using RTS.Interfaces;
using UnityEngine;

public class SquadronBase : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private int shipsAmount;
    [SerializeField] private float launchTime;
    [SerializeField] private float shipSize;
    [SerializeField] private GameObject shipPref;

    private ICarriable _parentCarrier;
    private readonly List<GameObject> _launchedShips = new List<GameObject>();

    public int ID => id;

    public void Launch(ICarriable parentCarrier)
    {
        _parentCarrier = parentCarrier;
        StartCoroutine(LaunchSequence());
    }

    private IEnumerator LaunchSequence()
    {
        var points = _parentCarrier.SquadronSpawnPoints;
        var amountOfIter = Mathf.CeilToInt((float)shipsAmount / points.Count);
        var timeBetwShips = launchTime / amountOfIter;
        var shipsLeft = shipsAmount;
        
        for (var i = 0; i < amountOfIter; i++)
        {
            foreach (var point in points)
            {
                if (shipsLeft <= 0) break;
                shipsLeft--;
                
                var newShip =Instantiate(shipPref, point.position, point.rotation);
                newShip.transform.localScale = new Vector3(shipSize, shipSize, shipSize);
                _launchedShips.Add(newShip);
            }

            yield return new WaitForSeconds(timeBetwShips);
        }
    }
}
