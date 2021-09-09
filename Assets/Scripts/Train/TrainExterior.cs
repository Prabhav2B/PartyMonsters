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
    
    void Start()
    {
        _trainLineColor = trainExteriorData.trainTrainLineColor;
        trainExteriorSprite.sprite = trainExteriorData.trainExterior;
    }


    public void ArriveAtStation(bool reversing)
    {
        transform.position = reversing ? startPositionLeft : startPositionRight;

        transform.DOLocalMove(destinationPosition, 12f);
    }
    
    public void DepartFromStation(bool reversing)
    {
        var exitPosition = reversing ? startPositionRight : startPositionLeft;

        transform.DOLocalMove(exitPosition, 12f);
    }
    
    

}
