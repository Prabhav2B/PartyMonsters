using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class DummyTrainAnimation : MonoBehaviour
{
    [SerializeField] private GameObject menuVisuals;
    
    [SerializeField] private float positionA;
    [SerializeField] private float positionB;

    private float _pos;
    
    private RectTransform _trainTransform;

    bool _scriptedTrainEvent;
    
    void Start()
    {
        FindObjectOfType<PlayerManager>().GetComponent<PlayerInput>().enabled = false;
        _trainTransform = GetComponent<RectTransform>();

        _trainTransform.anchoredPosition = new Vector2(positionA, _trainTransform.anchoredPosition.y);
        _trainTransform.DOAnchorPosX(positionB, 10f).OnComplete(DummyWait);

        _pos = positionA;

    }

    void DummyWait()
    {
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        var timeExpired = 0f;
        var _waitDuration = UnityEngine.Random.Range(8f, 15f);
        
        while (true)
        {
            if (timeExpired >= _waitDuration)
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
        
        _trainTransform.DOAnchorPosX(_pos, 7f).OnComplete(DummyWait);
        _pos = tempPos;

    }

    private void Update()
    {
        if(!_scriptedTrainEvent)
            return;

        if (_trainTransform.anchoredPosition.x < -14000f)
        {
            menuVisuals.SetActive(false);
        }

    }

    public void ScriptedTrainEvent()
    {
        DOTween.Kill(_trainTransform);
         StopAllCoroutines();
         _trainTransform.anchoredPosition = new Vector2(positionA, _trainTransform.anchoredPosition.y);
        _trainTransform.DOAnchorPosX(positionB, 10f).OnComplete(()=> transform.parent.gameObject.SetActive(false));
        _scriptedTrainEvent = true;
    }
}
