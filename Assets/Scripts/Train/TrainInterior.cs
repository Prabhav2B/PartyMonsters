using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TrainInterior : MonoBehaviour
{
    [SerializeField] private TrainInteriorData trainInteriorData;
    [SerializeField] private SpriteRenderer trainInteriorSprite;
    
    private List<BoxCollider2D> _interactionColliders = new List<BoxCollider2D>();
    private TrainBackground[] _backgrounds;
    private TrainRattle _trainRattle;
    
    private TrainScheduler _trainScheduler;
    
    private TrainLineColor _trainLineColor;
    private float _waitDuration;
    private bool _reversing;
    private bool _isEndStation;
    private bool _interactionPerformed;

    public UnityEvent OnTrainStop;
    public UnityEvent OnTrainStart;
    
    public bool InteractionPerformed
    {
        get => _interactionPerformed;
        set => _interactionPerformed = value;
    }
    
    void Awake()
    {
        _trainScheduler = FindObjectOfType<TrainScheduler>();
        
        _trainLineColor = trainInteriorData.trainTrainLineColor;
        trainInteriorSprite.sprite = trainInteriorData.trainInterior;

        _trainRattle = GetComponent<TrainRattle>();
        _backgrounds = GetComponentsInChildren<TrainBackground>();
        var doors = GetComponentsInChildren<TrainDoor>();
        _interactionColliders = new List<BoxCollider2D>();

        foreach (var door in doors)
        {
            _interactionColliders.Add(door.GetComponentInChildren<BoxCollider2D>());
        }
        
        
        _waitDuration = trainInteriorData.waitDuration;
    }

    public void SetInitialReverseValue(bool reversing)
    {
        foreach (var background in _backgrounds)
        {
            background.Reversing = reversing;
            background.Init();
        }
    }

    public void ArriveAtStation(bool reversing, bool isEndStation)
    {
        _reversing = reversing;
        _isEndStation = isEndStation;
        DisableInteractionTriggers();
        //transform.position = _reversing ? startPositionLeft : startPositionRight;
        //transform.DOLocalMove(destinationPosition, 12f).OnComplete(ArrivalComplete);
        _trainRattle.Stopped = true;
        foreach (var background in _backgrounds)
        {
            background.Halt();
        }

        StartCoroutine(WaitTillHalt());
    }
    
    private void ArrivalComplete()
    {
        foreach (var background in _backgrounds)
        {
            background.Reversing = _isEndStation ? !_reversing : _reversing;
        }

        OnTrainStop?.Invoke();
        EnableInteractionTriggers();
        StartCoroutine(WaitAtStation());
    }
    
    public void DepartFromStation()
    {
        OnTrainStart?.Invoke();
        DisableInteractionTriggers();
        foreach (var background in _backgrounds)
        {
            background.UnHalt();
        }

        StartCoroutine(WaitTillUnHalt());
        
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
    IEnumerator WaitTillHalt()
    {
        var timeExpired = 0f;
        
        while (true)
        {
            if (timeExpired >= 6f)
            {
                break;
            }
            timeExpired += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ArrivalComplete();
    }
    IEnumerator WaitTillUnHalt()
    {
        var timeExpired = 0f;
        
        while (true)
        {
            if (timeExpired >= 6f)
            {
                break;
            }
            timeExpired += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _trainRattle.Stopped = false;
        _trainScheduler.ResumeTrain();
    }
    
    // IEnumerator DepartStationAnim()
    // {
    //     var timeExpired = 0f;
    //     var exitDirection = _reversing ? 2f : -2f;
    //
    //     exitDirection = _isEndStation ? exitDirection * -1 : exitDirection;
    //     
    //     while (true)
    //     {
    //         if (timeExpired >= 10f)
    //         {
    //             break;
    //         }
    //
    //         transform.position = new Vector3(transform.position.x + (exitDirection * timeExpired * Time.deltaTime), transform.position.y, transform.position.z);
    //
    //         timeExpired += Time.deltaTime;
    //         yield return new WaitForEndOfFrame();
    //     }
    //     _departed = true;
    //     Deactivate();
    // }
    
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
    
    public void DisableInteractionTriggers()
    {
        foreach (var col in _interactionColliders)
        {
            col.enabled = false;
        }
    }
}
