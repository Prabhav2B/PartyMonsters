using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIImageFade : MonoBehaviour
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
    private Image _image = null;

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
        _image.enabled = true;
        yield return FadeFromTo(0f, 1f, _fadeInDuration);
    }

    private IEnumerator FadeOutCoroutine()
    {
        yield return FadeFromTo(1f, 0f, _fadeOutDuration);
        _image.enabled = false;
    }

    private IEnumerator FadeFromTo(float from, float to, float duration)
    {
        var color = _image.color;
        color.a = from;
        _image.color = color;
        var fade = _image.DOFade(to, duration);
        yield return fade.WaitForCompletion();
    }
}
