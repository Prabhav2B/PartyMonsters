using System.Collections;
using UnityEngine;

[
    DisallowMultipleComponent,
    RequireComponent(typeof(AudioSource))
]
public class MusicManager : SingleInstance<MusicManager>
{
    [SerializeField]
    private float _fadeOutDuration = 0f;
    [SerializeField]
    private AudioClip[] _clips = new AudioClip[0];

    private int _currentIndex = -1;
    public int CurrentIndex => _currentIndex;

    private AudioSource _source = null;

    protected override void Awake()
    {
        base.Awake();
        _source = GetComponent<AudioSource>();
    }

    public void Play(int index)
    {
        if (IsValidClipIndex(index) && index != _currentIndex)
        {
            _currentIndex = index;
            var clip = _clips[index];
            _source.clip = clip;
            _source.Play();
        }
    }

    public void FadeOutAndPlay(int index)
    {
        if (IsValidClipIndex(index) && index != _currentIndex)
        {
            _currentIndex = index;
            var clip = _clips[index];
            StartCoroutine(FadeOutAndPlay(_source, clip, _fadeOutDuration));
        }
    }

    private bool IsValidClipIndex(int index) => index >= 0 && index < _clips.Length;

    private static IEnumerator FadeOutAndPlay(AudioSource source, AudioClip clip, float duration)
    {
        yield return FadeOut(source, duration);
        source.clip = clip;
        source.Play();
    }

    private static IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }

        source.Stop();
        source.volume = startVolume;
    }
}
