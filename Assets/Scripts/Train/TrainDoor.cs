using System;
using UnityEngine;

[DisallowMultipleComponent]
public class TrainDoor : MonoBehaviour
{
    [SerializeField]
    private GameObject _closedSign = null;
    [SerializeField]
    private GameObject _openSign = null;

    private SceneFadeManager _sceneFadeManager;
    private SceneChangeManager _sceneChangeManager;

    private void Awake()
    {
        _sceneChangeManager = FindObjectOfType<SceneChangeManager>();
        _sceneFadeManager = FindObjectOfType<SceneFadeManager>();
    }

    public void EnterTrain()
    {
        SceneFadeManager.PostFadeOut f = () => { };
        f += _sceneChangeManager.SwitchToTrainInterior;
        
        _sceneFadeManager.FadeOut(f);
    }
    
    public void ExitTrain()
    {
        SceneFadeManager.PostFadeOut f = () => { };
        f += _sceneChangeManager.SwitchToStation;
        
        _sceneFadeManager.FadeOut(f);
    }

    public void OnDoorOpen()
    {
        _closedSign.SetActive(false);
        _openSign.SetActive(true);
    }

    public void OnDoorClose()
    {
        _closedSign.SetActive(true);
        _openSign.SetActive(false);
    }
}
