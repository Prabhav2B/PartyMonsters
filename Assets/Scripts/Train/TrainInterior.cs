using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainInterior : MonoBehaviour
{
    [SerializeField] private TrainInteriorData trainInteriorData;
    [SerializeField] private SpriteRenderer trainInteriorSprite;
    
    private List<BoxCollider2D> _interactionColliders = new List<BoxCollider2D>();
    private TrainRattle _trainRattle;
    
    private TrainScheduler _trainScheduler;
    
    private TrainLineColor _trainLineColor;
    private float _waitDuration;
    private bool _reversing;
    private bool _isEndStation;
    private bool _interactionPerformed;
    
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
        _interactionColliders = new List<BoxCollider2D>(GetComponentsInChildren<BoxCollider2D>());

        var colsToBeRemoved = _interactionColliders.Where(col => !col.isTrigger).ToList();

        foreach (var col in colsToBeRemoved)
        {
            _interactionColliders.Remove(col);
        }
        
        
        _waitDuration = trainInteriorData.waitDuration;
    }
    
    public void ArriveAtStation(bool reversing, bool isEndStation)
    {
        _reversing = reversing;
        _isEndStation = isEndStation;
        DisableInteractionTriggers();
        //transform.position = _reversing ? startPositionLeft : startPositionRight;
        //transform.DOLocalMove(destinationPosition, 12f).OnComplete(ArrivalComplete);
        _trainRattle.enabled = false;
        ArrivalComplete();

    }
    
    private void ArrivalComplete()
    {
        EnableInteractionTriggers();
        StartCoroutine(WaitAtStation());
    }
    
    public void DepartFromStation()
    {
        DisableInteractionTriggers();
        //StartCoroutine(DepartStationAnim());
        _trainRattle.enabled = true;
        _trainScheduler.ResumeTrain();
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
