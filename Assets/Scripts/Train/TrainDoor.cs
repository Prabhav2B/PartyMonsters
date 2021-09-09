using UnityEngine;

[DisallowMultipleComponent]
public class TrainDoor : MonoBehaviour
{
    private SceneFadeManager _sceneFadeManager;
    private SceneChangeManager _sceneChangeManager;

    private void Awake()
    {
        _sceneFadeManager = FindObjectOfType<SceneFadeManager>();
        _sceneChangeManager = FindObjectOfType<SceneChangeManager>();
    }

    public void EnterTrain() => _sceneFadeManager.FadeOut(_sceneChangeManager.SwitchToTrainInterior);
}
