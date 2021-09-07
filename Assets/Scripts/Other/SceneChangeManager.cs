using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeManager : SingleInstance<SceneChangeManager>
{
    [SerializeField] private GameObject station;
    [SerializeField] private GameObject trainExterior;
    [SerializeField] private GameObject trainInterior;
    private SceneFadeManager _sceneFadeManager;

    protected override void Awake()
    {
        base.Awake();
        _sceneFadeManager = FindObjectOfType<SceneFadeManager>();
    }

    public void SwitchToTrainInterior()
    {
        station.SetActive(false);
        trainExterior.SetActive(false);
        trainInterior.SetActive(true);
        _sceneFadeManager.FadeIn();
    }
}
