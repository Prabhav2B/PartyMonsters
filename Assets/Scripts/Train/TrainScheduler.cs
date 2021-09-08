using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainScheduler : MonoBehaviour
{
    [SerializeField] private TrainLines[] trainLines;
    [SerializeField] private TrainLookupTable[] trainLookup;
    [SerializeField] private StationLookupTable[] stationLookup;

    private Dictionary<TrainLineColor, TrainExterior> _trainDict = new Dictionary<TrainLineColor, TrainExterior>();
    private Dictionary<StationName, Station> _stationDict = new Dictionary<StationName, Station>();

    private void Awake()
    {
        foreach (var train in trainLookup)
        {
            _trainDict.Add(train.trainTrainLineColor, train.trainExterior);
        }

        foreach (var station in stationLookup)
        {
            _stationDict.Add(station.stationName, station.station);
        }
        
    }
}

[Serializable]
public struct TrainLookupTable
{
    public TrainExterior trainExterior;
    public TrainLineColor trainTrainLineColor;
}

[Serializable]
public struct StationLookupTable
{
    public Station station;
    public StationName stationName;
}

[Serializable]
public class TrainLines
{
    public TrainLineColor trainLine;
    public StationName[] stationsInOrder;
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