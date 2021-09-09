using System;
using UnityEngine;

[DisallowMultipleComponent]
public class TrainDoor : MonoBehaviour
{
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
}
