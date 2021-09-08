using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainScheduler : MonoBehaviour
{
    [SerializeField] private TrainLines[] trainLines;
    [SerializeField] private TrainLookupTable[] trainLookup;
    [SerializeField] private StationLookupTable[] stationLookup;

    [SerializeField] private StationName startStation;
    
    private Dictionary<TrainLineColor, TrainExterior> _trainDict = new Dictionary<TrainLineColor, TrainExterior>();
    private Dictionary<StationName, Station> _stationDict = new Dictionary<StationName, Station>();

    private StationName _currentStation;
    private bool _isTrainOnStation;

    private void Awake()
    {
        _currentStation = startStation;
        
        foreach (var train in trainLookup)
        {
            _trainDict.Add(train.trainTrainLineColor, train.trainExterior);
        }

        foreach (var station in stationLookup)
        {
            _stationDict.Add(station.stationName, station.station);
        }

        foreach (var trainLine in trainLines)
        {
            trainLine.Initialize();
        }

        foreach (var station in _stationDict)
        {
            station.Value.Deactivate();
        }
        
        _stationDict[_currentStation].Activate();
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
    public float intervalBetweenStations = 10f;
    public StationName[] stationsInOrder;
    
    private StationName _currentStation;
    private StationName _nextStation;
    private List<StationName> _stationsList = new List<StationName>();

    private bool _trainOnCurrentStation;
    private int _stationMax;
    
    private int _incrementor = 1;
    private int _counter = 0;
    
    public void Initialize()
    {
        _stationMax = stationsInOrder.Length;
        
        _stationsList.AddRange(stationsInOrder);
        _currentStation = _stationsList[_counter];
        _counter += _incrementor ;
        _nextStation = _stationsList[_counter];
    }

    public void MoveToNextStation()
    {
        _currentStation = _nextStation;

        if (_counter + _incrementor >= _stationMax)
        {
            _incrementor = -1;
        }
        else if (_counter + _incrementor < 0)
        {
            _incrementor = 1;
        }
        
        _counter += _incrementor ;

        _nextStation = _stationsList[_counter];
    }

    public void DebugCurrentPlatform()
    {
        Debug.Log(trainLine + " Arriving at " + _currentStation);
    }

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