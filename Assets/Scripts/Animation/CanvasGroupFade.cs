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
            StartCoroutine(FadeInCoroutine());
        }
    }

    public void FadeOut()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeOutCoroutine());
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        yield return FadeFromTo(0f, 1f, _fadeInDuration);
    }

    private IEnumerator FadeOutCoroutine()
    {
        yield return FadeFromTo(1f, 0f, _fadeOutDuration);
    }

    private IEnumerator FadeFromTo(float from, float to, float duration)
    {
        _canvasGroup.alpha = from;
        var fade = _canvasGroup.DOFade(to, duration);
        yield return fade.WaitForCompletion();
    }
}
