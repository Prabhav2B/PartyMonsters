using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SceneChangeManager : SingleInstance<SceneChangeManager>
{
    [SerializeField] private Vector3 playerStartPosition = new Vector3(19.49f, 7.04f, 0f);
    
    private Station _station;
    private TrainExterior _trainExterior;
    private GameObject _trainInterior;
    private TrainScheduler _trainScheduler;

    public Station CurrentStation
    {
        set => _station = value;
    }
    
    public TrainExterior CurrentTrain
    {
        set => _trainExterior = value;
    }

    private SceneFadeManager _sceneFadeManager;
    private PlayerManager _player;

    protected override void Awake()
    {
        base.Awake();
        _sceneFadeManager = FindObjectOfType<SceneFadeManager>();
        _trainScheduler = FindObjectOfType<TrainScheduler>();
        _player = FindObjectOfType<PlayerManager>();
    }

    public void SwitchToTrainInterior()
    {
        _trainScheduler.PlayerLocation = PlayerLocation.train;
        _trainScheduler.FlushStation();
        _trainScheduler.FlushWaitList();
        _trainScheduler.ClearNewStationFlag();
        
        
        _station.gameObject.SetActive(false);
        _trainExterior.gameObject.SetActive(false);
        _trainExterior.CurrentTrainInterior.gameObject.SetActive(true);
        _trainExterior.CurrentTrainInterior.SetInitialReverseValue(_trainScheduler.CurrentTrainReversing);
        _trainExterior.CurrentTrainInterior.DisableInteractionTriggers();
        _player.transform.position = playerStartPosition; // ignoring rigidbody here hehehe
        _sceneFadeManager.FadeIn();
    }

    public void SwitchToStation()
    {
        _trainScheduler.PlayerLocation = PlayerLocation.station;
        _trainScheduler.SetCurrentStation();
        _trainScheduler.PushTrainsForward();
        _trainScheduler.ResumeTrain();


        _station.gameObject.SetActive(true);
        //_trainExterior.gameObject.SetActive(false);
        _trainExterior.CurrentTrainInterior.gameObject.SetActive(false);
        //_trainExterior.CurrentTrainInterior.DisableInteractionTriggers();
        _player.transform.position = playerStartPosition; // ignoring rigidbody here hehehe
        _sceneFadeManager.FadeIn();
    }
}
