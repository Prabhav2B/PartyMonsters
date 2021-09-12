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
        _renderer.enabled = true;
        yield return FadeFromTo(0f, 1f, _fadeInDuration);
    }

    private IEnumerator FadeOutCoroutine()
    {
        yield return FadeFromTo(1f, 0f, _fadeOutDuration);
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
