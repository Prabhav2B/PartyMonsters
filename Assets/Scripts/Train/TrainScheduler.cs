using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainScheduler : MonoBehaviour
{
    [SerializeField] private TrainLookupTable[] trainLookup;
    [SerializeField] private StationLookupTable[] stationLookup;
}

[Serializable]
public struct TrainLookupTable {
    public TrainExterior trainExterior;
    public TrainLineColor trainTrainLineColor;
}

[Serializable]
public struct StationLookupTable {
    public Station station;
    public StationName stationName;
}


public enum TrainLineColor
{
    purple,
    green,
    yellow,
    pink,
    blue
}

public enum StationName
{
    avalon,
    lilith,
    eden,
    lotus,
    zion
}

