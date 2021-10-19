using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DummyStation : MonoBehaviour
{
    [SerializeField] private Sprite tunnelSprite;
    [SerializeField] private SpriteRenderer[] tunnelSpriteRenderer;

    [SerializeField] private Transform dummyStationParent;

    [Space(10)] [SerializeField] private Vector3 startPositionLeft;
    [SerializeField] private Vector3 startPositionRight;
    [SerializeField] private Vector3 destinationPosition;


    private TrainLineColor _trainLineColor;
    private float _waitDuration;
    private bool _reversing;
    private bool _isEndStation;
    private bool _interactionPerformed;
    private List<BoxCollider2D> _interactionColliders = new List<BoxCollider2D>();
    private int _backgroundSortingLayerID;


    public bool InteractionPerformed
    {
        get => _interactionPerformed;
        set => _interactionPerformed = value;
    }

    void Awake()
    {
        _backgroundSortingLayerID = SortingLayer.NameToID("TrainBackground");
        tunnelSpriteRenderer[0].sprite = tunnelSprite;
        tunnelSpriteRenderer[1].sprite = tunnelSprite;
        _interactionColliders = new List<BoxCollider2D>(GetComponentsInChildren<BoxCollider2D>());

        //_waitDuration = trainExteriorData.waitDuration;
    }
    
    public void SetDummyStation(Station stationArrivingAt)
    {
        
        foreach (Transform child in dummyStationParent)
        {
            Destroy(child.gameObject);
        }
        
        var dummyStation = Instantiate(stationArrivingAt.gameObject, dummyStationParent);
        dummyStation.SetActive(true);
        dummyStation.transform.localPosition = Vector3.zero;
        
        //pruning the Virtual Camera
        Destroy(dummyStation.transform.GetChild(0).gameObject);

        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        var cols = dummyStation.GetComponentsInChildren<Collider2D>(true);
        foreach (var col in cols)
        {
            col.enabled = false;
        }
        
        var lights = dummyStation.GetComponentsInChildren<Light2D>(true);
        foreach (var light in lights)
        {
            light.enabled = false;
        }

        var srs = dummyStation.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in srs)
        {
            sr.sortingLayerID = _backgroundSortingLayerID;
            //sr.sortingOrder = 0;
        }
        
        
    }

    public void SetWaitDuration(float waitDuration)
    {
        _waitDuration = waitDuration;
    }

    public void ArriveAtStation(bool reversing, bool isEndStation, float timeToStop)
    {
        Activate();
        _reversing = !reversing;
        _isEndStation = isEndStation;

        if (!isEndStation)
            transform.position = _reversing ? startPositionLeft : startPositionRight;
        else
            transform.position = !_reversing ? startPositionLeft : startPositionRight;

        transform.DOLocalMove(destinationPosition, timeToStop).OnComplete(ArrivalComplete);
    }

    private void ArrivalComplete()
    {
        StartCoroutine(WaitAtStation());
    }

    public void DepartFromStation()
    {
        StartCoroutine(DepartStationAnim());
    }

    IEnumerator WaitAtStation()
    {
        var timeExpired = 0f;

        while (true)
        {
            if (timeExpired >= _waitDuration)
            {
                DepartFromStation();
                break;
            }
            else if (_interactionPerformed)
            {
                _interactionPerformed = false;
                break;
                //trigger transition;
            }

            timeExpired += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DepartStationAnim()
    {
        var timeExpired = 0f;
        var exitDirection = _reversing ? 2f : -2f;

        while (true)
        {
            if (timeExpired >= 6f)
            {
                break;
            }

            transform.position = new Vector3(transform.position.x + (exitDirection * timeExpired * timeExpired * Time.deltaTime),
                transform.position.y, transform.position.z);

            timeExpired += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        transform.position = startPositionLeft;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}