using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainScheduler : MonoBehaviour
{
    [SerializeField] private DummyStation dummyStation;
    
    [SerializeField] private TrainLine[] trainLines;
    [SerializeField] private TrainLookupTable[] trainLookup;
    [SerializeField] private StationLookupTable[] stationLookup;

    [SerializeField] private StationName startStation;

    private Dictionary<TrainLineColor, TrainExterior> _trainDict = new Dictionary<TrainLineColor, TrainExterior>();
    private Dictionary<StationName, Station> _stationDict = new Dictionary<StationName, Station>();
    private Dictionary<TrainLineColor, TrainLine> _trainLinesDict = new Dictionary<TrainLineColor, TrainLine>();

    private Queue<TrainLine> _waitList = new Queue<TrainLine>();

    private StationName _currentStation;
    private TrainExterior _trainOnStation;
    private TrainLine _currentTrain;
    private SceneChangeManager _sceneChangeManager;
    private MusicManager _musicManager;

    private PlayerLocation _playerLocation;

    private TrainLine _lineOnStation;
    //private bool _isTrainOnStation;

    private static bool _stationOccupied;
    private static bool _initialized = false;

    public PlayerLocation PlayerLocation
    {
        set => _playerLocation = value;
    }

    public bool CurrentTrainReversing => _currentTrain.Reversing;
    public bool OnTrain { get; set; }

    public void Initialize()
    {
        if (!_initialized)
        {
            _playerLocation = PlayerLocation.station;
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
                _trainLinesDict.Add(trainLine.trainLine, trainLine);
            }

            foreach (var station in _stationDict)
            {
                station.Value.Deactivate();
            }

            _stationDict[_currentStation].Activate();

            _sceneChangeManager = FindObjectOfType<SceneChangeManager>();
            _musicManager = FindObjectOfType<MusicManager>();
            _musicManager.Play(_stationDict[_currentStation].StationData.musicIndex);
            _sceneChangeManager.CurrentStation = _stationDict[_currentStation];

            _initialized = true;
        }
    }

    private void Update()
    {
        if (!_initialized) return;

        switch (_playerLocation)
        {
            case PlayerLocation.station:
            {
                if (_stationOccupied)
                {
                    if (_trainOnStation.Departed)
                    {
                        FlushStation();
                    }
                }

                if (!_stationOccupied && _waitList.Count > 0)
                {
                    _stationOccupied = true;
                    _lineOnStation = _waitList.Dequeue();
                    _lineOnStation.TrainOnCurrentStation = true;

                    _trainOnStation = _trainDict[_lineOnStation.trainLine];
                    _trainOnStation.Activate();
                    _trainOnStation.ArriveAtStation(_lineOnStation.Reversing, _lineOnStation.IsEndStation);
                    _sceneChangeManager.CurrentTrain = _trainOnStation;
                    _currentTrain = _lineOnStation;
                }

                foreach (var trainLine in trainLines)
                {
                    trainLine.Tick();

                    if (trainLine.CurrentStation == _currentStation)
                    {
                        if (!_stationOccupied)
                        {
                            _stationOccupied = true;
                            _lineOnStation = trainLine;
                            _lineOnStation.TrainOnCurrentStation = true;

                            _trainOnStation = _trainDict[_lineOnStation.trainLine];
                            _trainOnStation.Activate();
                            _trainOnStation.ArriveAtStation(_lineOnStation.Reversing, _lineOnStation.IsEndStation);
                            _sceneChangeManager.CurrentTrain = _trainOnStation;
                            _currentTrain = _lineOnStation;
                        }
                        else
                        {
                            if (_waitList.Contains(trainLine) || _trainOnStation == _trainDict[trainLine.trainLine])
                                continue;

                            trainLine.Waiting = true;
                            _waitList.Enqueue(trainLine);
                        }
                    }
                }

                break;
            }
            case PlayerLocation.train:
            {
                foreach (var trainLine in trainLines)
                {
                    trainLine.Tick();
                }

                if (_currentTrain.ReachedNewStation)
                {
                    _currentTrain.ReachedNewStation = false;
                    _currentTrain.TrainOnCurrentStation = true;

                    var stationArrivingAt = _stationDict[_currentTrain.CurrentStation];
                    
                    var currentTrainInterior = _trainDict[_currentTrain.trainLine].CurrentTrainInterior;

                    //Setup the props for the station arriving at 
                    dummyStation.SetDummyStation(stationArrivingAt);
                    //Execute arrival at Station
                    currentTrainInterior.ArriveAtStation(_currentTrain.Reversing, _currentTrain.IsEndStation);
                }

                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ResumeTrain()
    {
        _currentTrain.TrainOnCurrentStation = false;
    }

    public void SetCurrentStation()
    {
        _currentStation = _currentTrain.CurrentStation;
        _sceneChangeManager.CurrentStation = _stationDict[_currentStation];
        int musicIndex = _stationDict[_currentStation].StationData.musicIndex;
        if (musicIndex != _musicManager.CurrentIndex)
        {
            _musicManager.FadeOutAndPlay(musicIndex);
        }
    }

    public void PushTrainsForward()
    {
        foreach (var trainLine in trainLines)
        {
            if (trainLine.CurrentStation == _currentStation)
            {
                trainLine.MoveToNextStation();
            }
        }
    }

    public void FlushStation()
    {
        _stationOccupied = false;
        _lineOnStation.TrainOnCurrentStation = false;
        _lineOnStation.VoidCurrentStation();
        _lineOnStation.Waiting = false;
        _lineOnStation = null;
        _trainOnStation = null;
    }

    public void FlushWaitList()
    {
        if (_waitList.Count == 0)
            return;

        int i = 0;
        foreach (var item in _waitList)
        {
            item.Waiting = false;
            item.OffsetTimer(i++ * -10);
        }

        _waitList.Clear();
    }

    public void ClearNewStationFlag()
    {
        _currentTrain.ReachedNewStation = false;
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
public class TrainLine
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

    private bool _reversing;
    private bool _isEndStation;

    private float _timer;

    public bool TrainOnCurrentStation
    {
        get => _trainOnCurrentStation;
        set => _trainOnCurrentStation = value;
    }

    public bool Waiting { get; set; }
    public bool ReachedNewStation { get; set; }
    public bool IsEndStation => _isEndStation;
    public bool Reversing => _reversing;
    public StationName CurrentStation => _currentStation;

    public void Initialize()
    {
        _stationMax = stationsInOrder.Length;

        _stationsList.AddRange(stationsInOrder);
        _currentStation = _stationsList[_counter];
        _reversing = false;
        _isEndStation = true;
        _counter += _incrementor;
        _nextStation = _stationsList[_counter];
    }

    public void MoveToNextStation()
    {
        _timer = 0f;
        _isEndStation = _counter == 0 || _counter == _stationMax - 1;
        _currentStation = _nextStation;

        if (_counter + _incrementor >= _stationMax)
        {
            _incrementor = -1;
        }
        else if (_counter + _incrementor < 0)
        {
            _incrementor = 1;
        }

        _counter += _incrementor;
        _nextStation = _stationsList[_counter];

        _reversing = _incrementor < 0;

        ReachedNewStation = true;
        //DebugCurrentPlatform();
    }

    public void Tick()
    {
        if (_trainOnCurrentStation || Waiting)
            return;

        _timer += Time.deltaTime;
        if (_timer > intervalBetweenStations)
        {
            _timer = 0f;
            MoveToNextStation();
        }
    }

    public void OffsetTimer(float offset)
    {
        _timer = offset;
    }

    public void VoidCurrentStation()
    {
        _currentStation = StationName._null;
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
    blue,
    white,
    _null
}

public enum StationName
{
    avalon,
    lilith,
    eden,
    lotus,
    zion,
    pantheon,
    samsara,
    utopia,
    new_moon,
    elysia,
    _null
}

public enum PlayerLocation
{
    station,
    train
}