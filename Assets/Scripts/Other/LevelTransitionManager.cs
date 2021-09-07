using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionManager : SingleInstance<LevelTransitionManager>
{
    public void LoadNextLevel()
    {
        var nextSceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Assert(nextSceneBuildIndex <= SceneManager.sceneCount, this.gameObject);
        if (nextSceneBuildIndex > SceneManager.sceneCount)
        {
            return;
        }

        SceneManager.LoadScene(nextSceneBuildIndex);
    }

    public void LoadLevel(int buildIndex)
    {
        Debug.Assert(buildIndex >= SceneManager.sceneCount, this.gameObject);
        if (buildIndex >= SceneManager.sceneCount)
        {
            return;
        }

        SceneManager.LoadScene(buildIndex);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}