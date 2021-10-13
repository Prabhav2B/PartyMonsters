using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class DummyTrainAnimation : MonoBehaviour
{
    [SerializeField] private float positionA;
    [SerializeField] private float positionB;
    [SerializeField] private UnityEvent onStartGame = new UnityEvent();
    [SerializeField] private UnityEvent onScreenRevealed = new UnityEvent();

    private float _pos;
    
    private RectTransform _trainTransform;

    private bool _scriptedTrainEvent;
    private bool _finished = false;

    private IEnumerator Start()
    {
        _trainTransform = GetComponent<RectTransform>();
        _trainTransform.anchoredPosition = new Vector2(positionA, _trainTransform.anchoredPosition.y);
        _pos = positionA;

        yield return new WaitForSeconds(1f);

        _trainTransform.DOAnchorPosX(positionB, 2f).OnComplete(StartScreenRevealed);
    }

    private void StartScreenRevealed()
    {
        onScreenRevealed.Invoke();
        DummyWait();
    }

    private void DummyWait()
    {
        if (_finished)
        {
            StopAnimation();
            return;
        }
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        var timeExpired = 0f;
        var _waitDuration = UnityEngine.Random.Range(20f, 30f);
        
        while (true)
        {
            if (timeExpired >= _waitDuration || _scriptedTrainEvent)
            {
                GoToOtherDirection();
                break;
            }

            timeExpired += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void GoToOtherDirection()
    {
        var tempPos = _trainTransform.anchoredPosition.x;
        _trainTransform.DOAnchorPosX(_pos, 2f).OnComplete(DummyWait);
        _pos = tempPos;
    }

    private void Update()
    {
        if(!_scriptedTrainEvent || _finished)
            return;

        if (_trainTransform.anchoredPosition.x < 2400f && _trainTransform.anchoredPosition.x > -2400)
        {
            onStartGame.Invoke();
            _finished = true;
        }
    }

    public void ScriptedTrainEvent()
    {
        // DOTween.Kill(_trainTransform);
        // StopAllCoroutines();
        // _trainTransform.anchoredPosition = new Vector2(positionA, _trainTransform.anchoredPosition.y);
        // _trainTransform.DOAnchorPosX(positionB, 10f).OnComplete(()=> transform.parent.gameObject.SetActive(false));
        _scriptedTrainEvent = true;
    }

    private void StopAnimation()
    {
        DOTween.Kill(_trainTransform);
        StopAllCoroutines();
        transform.parent.gameObject.SetActive(false);
    }
}
