using System.Collections;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class CanvasGroupFade : MonoBehaviour
{
    [
        SerializeField,
        Min(0f)
    ]
    private float _fadeInDuration = 0f;
    [
        SerializeField,
        Min(0f)
    ]
    private float _fadeOutDuration = 0f;
    [SerializeField]
    private CanvasGroup _canvasGroup = null;

    public void FadeIn()
    {
        if (gameObject.activeInHierarchy)
        {
            DOTween.Kill(_canvasGroup);
            StopAllCoroutines();
            StartCoroutine(FadeInCoroutine());
        }
    }

    public void FadeOut()
    {
        if (gameObject.activeInHierarchy)
        {
            DOTween.Kill(_canvasGroup);
            StopAllCoroutines();
            StartCoroutine(FadeOutCoroutine());
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        float currentFade = _canvasGroup.alpha;
        yield return FadeFromTo(currentFade, 1f, _fadeInDuration);
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOutCoroutine()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        float currentFade = _canvasGroup.alpha;
        yield return FadeFromTo(currentFade, 0f, _fadeOutDuration);
    }

    private IEnumerator FadeFromTo(float from, float to, float duration)
    {
        _canvasGroup.alpha = from;
        var fade = _canvasGroup.DOFade(to, duration);
        yield return fade.WaitForCompletion();
    }
}
