using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeManager : SingleInstance<SceneChangeManager>
{
    [SerializeField] private Vector3 playerStartPosition = new Vector3(19.49f, 7.04f, 0f);
    
    [SerializeField] private GameObject station;
    [SerializeField] private GameObject trainExterior;
    [SerializeField] private GameObject trainInterior;
    
    private SceneFadeManager _sceneFadeManager;
    private PlayerManager _player;

    protected override void Awake()
    {
        base.Awake();
        _sceneFadeManager = FindObjectOfType<SceneFadeManager>();
        _player = FindObjectOfType<PlayerManager>();
    }

    public void SwitchToTrainInterior()
    {
        station.SetActive(false);
        trainExterior.SetActive(false);
        trainInterior.SetActive(true);
        _player.transform.position = playerStartPosition; // ignoring rigidbody here hehehe
        _sceneFadeManager.FadeIn();
    }
}
