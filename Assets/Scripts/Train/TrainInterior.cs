using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class handles every operation that is supposed to take place
/// When the player is inside the train
/// </summary>
public class TrainInterior : MonoBehaviour
{
    [SerializeField] private TrainInteriorData trainInteriorData; 
    [SerializeField] private InteriorBackgroundData interiorBackgroundData; 
    [SerializeField] private SpriteRenderer trainInteriorSprite;

    [SerializeField] private Sprite stationSprite;
    [SerializeField] private Sprite transitionSprite;
    
    [SerializeField] private float timeTillHalt = 6f;
    
    private List<BoxCollider2D> _interactionColliders = new List<BoxCollider2D>(); //the door colliders to exit the train
    private TrainBackground[] _backgrounds;
    private TrainRattle _trainRattle;

    private TrainScheduler _trainScheduler;

    private float _waitDuration;
    private bool _reversing;
    private TrainDoor[] _doors;

    //Events that fire when the train starts and comes to a stop
    public UnityEvent OnTrainStop;
    public UnityEvent OnTrainStart;

    private bool InteractionPerformed { get; set; }

    private void Awake()
    {
        _trainScheduler = FindObjectOfType<TrainScheduler>();
        
        trainInteriorSprite.sprite = trainInteriorData.trainInterior;

        _trainRattle = GetComponent<TrainRattle>();
        _backgrounds = GetComponentsInChildren<TrainBackground>();
        _interactionColliders = new List<BoxCollider2D>();
        _doors = GetComponentsInChildren<TrainDoor>();

        var backgrounds = GetComponentsInChildren<TrainBackground>();
        
        backgrounds[0].SetSpriteAndSpeed(interiorBackgroundData.backSprite, interiorBackgroundData.backSpeed);
        backgrounds[1].SetSpriteAndSpeed(interiorBackgroundData.midSprite, interiorBackgroundData.midSpeed);
        backgrounds[2].SetSpriteAndSpeed(interiorBackgroundData.frontSprite, interiorBackgroundData.frontSpeed);
        
        
        


        foreach (var door in _doors)
        {
            _interactionColliders.Add(door.GetComponentInChildren<BoxCollider2D>());
        }


        _waitDuration = trainInteriorData.waitDuration;
    }

    private void OnEnable()
    {
        //delete this later
        var backgrounds = GetComponentsInChildren<TrainBackground>();
        backgrounds[3].SetAsTransitionLayer();
        //THIS NEEDS TO BE REFACTORED GOOD GOD
        backgrounds[3].SetSpriteAndSpeed(null, 100f);
        backgrounds[3].SetTransitionSprite(transitionSprite);
        backgrounds[3].SetStationSprite(stationSprite);
        
        //backgrounds[4].SetSpriteAndSpeed(stationSprite, 20f);


        foreach (var door in _doors)
        {
            door.OnDoorClose();
        }
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
        DisableInteractionTriggers();
        //transform.position = _reversing ? startPositionLeft : startPositionRight;
        //transform.DOLocalMove(destinationPosition, 12f).OnComplete(ArrivalComplete);
        _trainRattle.Stopped = true;
        foreach (var background in _backgrounds)
        {
            background.Halt(timeTillHalt - 1f);
        }

        StartCoroutine(WaitTillHalt());
    }

    private void ArrivalComplete()
    {
        foreach (var background in _backgrounds)
        {
            background.Reversing = _reversing;
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
            background.UnHalt(timeTillHalt);
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
            else if (InteractionPerformed)
            {
                InteractionPerformed = false;
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
            if (timeExpired >= timeTillHalt)
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