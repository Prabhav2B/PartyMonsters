using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainExterior : MonoBehaviour
{
    [SerializeField] private TrainExteriorData trainExteriorData;
    [SerializeField] private SpriteRenderer trainExteriorSprite;

    [SerializeField] private TrainInterior trainInterior;

    private TrainLineColor _trainLineColor;
    
    void Start()
    {
        _trainLineColor = trainExteriorData.trainTrainLineColor;
        trainExteriorSprite.sprite = trainExteriorData.trainExterior;
    }

    
}
