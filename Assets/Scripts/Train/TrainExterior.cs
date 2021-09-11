using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrainExterior : MonoBehaviour
{
    [SerializeField] private TrainExteriorData trainExteriorData;
    [SerializeField] private SpriteRenderer trainExteriorSprite;

    [Space(10)]
    
    [SerializeField] private Vector3 startPositionLeft;
    [SerializeField] private Vector3 startPositionRight;
    [SerializeField] private Vector3 destinationPosition;

    [Space(10)]

    [SerializeField] private TrainInterior trainInterior;

    private TrainLineColor _trainLineColor;
    private float _waitDuration;
    private bool _reversing;
    private bool _isEndStation;
    private bool _interactionPerformed;
    private bool _departed;
    private List<BoxCollider2D> _interactionColliders = new List<BoxCollider2D>();
    private TrainDoor[] _doors;


    public bool Departed => _departed;
    public TrainInterior CurrentTrainInterior => trainInterior; 
    
    public bool InteractionPerformed
    {
        get => _interactionPerformed;
        set => _interactionPerformed = value;
    }

    void Awake()
    {
        _trainLineColor = trainExteriorData.trainTrainLineColor;
        trainExteriorSprite.sprite = trainExteriorData.trainExterior;
        _interactionColliders = new List<BoxCollider2D>(GetComponentsInChildren<BoxCollider2D>());
        
        _doors = GetComponentsInChildren<TrainDoor>();

        _waitDuration = trainExteriorData.waitDuration;
    }

    private void Start()
    {
        foreach (var door in _doors)
        {
            door.SetDoorSprite(trainExteriorData.trainDoor);
        }
    }

    private void OnEnable()
    {
        foreach (var door in _doors)
        {
            door.ResetDoors();
        }
    }


    public void ArriveAtStation(bool reversing, bool isEndStation)
    {
        _departed = false;
        _reversing = reversing;
        _isEndStation = isEndStation;
        DisableInteractionTriggers();
        
        if(!isEndStation)
            transform.position = _reversing ? startPositionLeft : startPositionRight;
        else
            transform.position = !_reversing ? startPositionLeft : startPositionRight;

        transform.DOLocalMove(destinationPosition, 12f).OnComplete(ArrivalComplete);
    }

    private void ArrivalComplete()
    {
        EnableInteractionTriggers();
        StartCoroutine(WaitAtStation());
        
        foreach (var door in _doors)
        {
            door.OpenDoorAnim();
        }
    }

    public void DepartFromStation()
    {
        DisableInteractionTriggers();
        foreach (var door in _doors)
        {
            door.CloseDoorAnim();
        }
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

        //exitDirection = _isEndStation ? exitDirection * -1 : exitDirection;
        
        while (true)
        {
            if (timeExpired >= 10f)
            {
                break;
            }

            transform.position = new Vector3(transform.position.x + (exitDirection * timeExpired * Time.deltaTime), transform.position.y, transform.position.z);

            timeExpired += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _departed = true;
        Deactivate();
    }
    
    public void Activate()
    {
        gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void EnableInteractionTriggers()
    {
        foreach (var col in _interactionColliders)
        {
            col.enabled = true;
        }
    }
    
    private void DisableInteractionTriggers()
    {
        foreach (var col in _interactionColliders)
        {
            col.enabled = false;
        }
    }
}
