using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private Image transitionImage;
    [SerializeField] private float fadeInDuration = 0.5f;

    private PlayerManager _playerManager;
    
    private bool _menuActive;

    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    public void TriggerMainMenu()
    {
        _menuActive = !_menuActive;

        if (_menuActive)
        {
            AudioListener.pause = true;
            var tScale = Time.timeScale;
            DOTween.To(() => tScale, x => Time.timeScale = x, 0f, 0.5f).SetUpdate((true));
            transitionImage.DOFade(.95f, fadeInDuration).SetUpdate(true).OnComplete(ActiveMainMenu);
            _playerManager.Pause();
        }
        else
        {
            AudioListener.pause = false;
            var tScale = Time.timeScale;
            DOTween.To(() => tScale, x => Time.timeScale = x, 1f, 0.5f).SetUpdate(true);
            DeactiveMainMenu();
            transitionImage.DOFade(0f, fadeInDuration).SetUpdate(true);
            _playerManager.UnPause();
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    private void ActiveMainMenu()
    {
        mainMenuUI.SetActive(true);
    }

    private void DeactiveMainMenu()
    {
        mainMenuUI.SetActive(false);
    }
}