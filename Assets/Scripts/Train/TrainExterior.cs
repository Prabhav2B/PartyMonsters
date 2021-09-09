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
    private bool _interactionPerformed;
    private List<BoxCollider2D> _interactionColliders = new List<BoxCollider2D>();

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

        _waitDuration = trainExteriorData.waitDuration;
    }


    public void ArriveAtStation(bool reversing)
    {
        _reversing = reversing;
        DisableInteractionTriggers();
        transform.position = _reversing ? startPositionLeft : startPositionRight;
        transform.DOLocalMove(destinationPosition, 12f).OnComplete(ArrivalComplete);
    }

    private void ArrivalComplete()
    {
        EnableInteractionTriggers();
        StartCoroutine(WaitAtStation());
    }

    public void DepartFromStation()
    {
        DisableInteractionTriggers();
        var exitPosition = _reversing ? startPositionRight : startPositionLeft;
        exitPosition.Scale(new Vector3(.5f, 1f, 1f));
        transform.DOLocalMove(exitPosition, 12f).OnComplete(Deactivate);
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
