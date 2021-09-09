using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLineBehaviour : MonoBehaviour
{
    public TrainLineColor myColor;
    public List<StationBehaviour> connectedStations;

    private void Awake()
    {
        connectedStations = new List<StationBehaviour>();
        myColor = TrainLineColor._null;
    }
}
