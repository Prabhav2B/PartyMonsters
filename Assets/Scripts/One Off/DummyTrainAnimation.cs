using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DummyTrainAnimation : MonoBehaviour
{
    [SerializeField] private float positionA;
    [SerializeField] private float positionB;

    private float _pos;
    
    private RectTransform _trainTransform;
    
    void Start()
    {
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
}
