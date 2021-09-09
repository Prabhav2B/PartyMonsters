using System.Collections;
using System.Collections.Generic;
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
        //Communication Interaction to TrainScheduler
        _trainScheduler.PlayerLocation = PlayerLocation.train;
        _trainScheduler.FlushStation();
        _trainScheduler.FlushWaitList();
        
        
        _station.gameObject.SetActive(false);
        _trainExterior.gameObject.SetActive(false);
        _trainExterior.CurrentTrainInterior.gameObject.SetActive(true);
        _player.transform.position = playerStartPosition; // ignoring rigidbody here hehehe
        _sceneFadeManager.FadeIn();
    }

    public void SwitchToStation()
    {
        
    }
}
