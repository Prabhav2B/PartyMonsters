using System.Collections;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class SpriteFade : MonoBehaviour
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
    private SpriteRenderer _renderer = null;

    public void FadeIn()
    {
        if (gameObject.activeInHierarchy)
        {
            DOTween.Kill(_renderer);
            StopAllCoroutines();
            StartCoroutine(FadeInCoroutine());
        }
    }

    public void FadeOut()
    {
        if (gameObject.activeInHierarchy)
        {
            DOTween.Kill(_renderer);
            StopAllCoroutines();
            StartCoroutine(FadeOutCoroutine());
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        float currentFade = _renderer.color.a;
        _renderer.enabled = true;
        yield return FadeFromTo(currentFade, 1f, _fadeInDuration);
    }

    private IEnumerator FadeOutCoroutine()
    {
        float currentFade = _renderer.color.a;
        yield return FadeFromTo(currentFade, 0f, _fadeOutDuration);
        _renderer.enabled = false;
    }

    private IEnumerator FadeFromTo(float from, float to, float duration)
    {
        var color = _renderer.color;
        color.a = from;
        _renderer.color = color;
        var fade = _renderer.DOFade(to, duration);
        yield return fade.WaitForCompletion();
    }
}
