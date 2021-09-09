using UnityEngine;

[DisallowMultipleComponent]
public class TrainDoor : MonoBehaviour
{
    private SceneFadeManager _sceneFadeManager;
    private SceneChangeManager _sceneChangeManager;

    public void EnterTrain() => _sceneFadeManager.FadeOut(_sceneChangeManager.SwitchToTrainInterior);
}
