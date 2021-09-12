using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFadeManager : MonoBehaviour
{
    private PlayerManager playerManager;
    [SerializeField] private float fadeInDuration = 1f;

    private Image _transitionImage;

    public delegate void PostFadeIn();

    public delegate void PostFadeOut();

    public void Start()
    {
        if (playerManager == null) playerManager = FindObjectOfType<PlayerManager>();

        _transitionImage = GetComponentInChildren<Image>();
        var transitionImageColor = _transitionImage.color;
        transitionImageColor.a = 1f;
        _transitionImage.color = transitionImageColor;

        if(SceneManager.GetActiveScene().buildIndex == 1)
            StartCoroutine(StaggeredGameStart());
    }

    public void StaggeredStart()
    {
        StartCoroutine(StaggeredGameStart());
    }

    public void FadeIn(PostFadeIn f)
    {
        _transitionImage.enabled = true;
        f += () => _transitionImage.enabled = false;
        _transitionImage.DOFade(0f, fadeInDuration).OnComplete(f.Invoke);
    }

    public void FadeOut(PostFadeOut f)
    {
        _transitionImage.enabled = true;
        f += () => _transitionImage.enabled = false;
        _transitionImage.DOFade(1f, fadeInDuration).OnComplete(f.Invoke);
    }

    public void FadeIn()
    {
        _transitionImage.enabled = true;
        _transitionImage.DOFade(0f, fadeInDuration).OnComplete(() => _transitionImage.enabled = false);
    }
    
    public void FadeOut()
    {
        _transitionImage.enabled = true;
        _transitionImage.DOFade(1f, fadeInDuration).OnComplete(() => _transitionImage.enabled = false);
    }

    private IEnumerator StaggeredGameStart()
    {
        yield return new WaitForSeconds(1f);
        FadeIn(playerManager.Activate);
    }
}