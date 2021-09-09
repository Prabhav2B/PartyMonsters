using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainInterior : MonoBehaviour
{
    [SerializeField] private TrainInteriorData trainInteriorData;
    [SerializeField] private SpriteRenderer trainInteriorSprite;
    
    
    private TrainLineColor _trainLineColor;
    
    void Start()
    {
        _trainLineColor = trainInteriorData.trainTrainLineColor;
        trainInteriorSprite.sprite = trainInteriorData.trainInterior;
    }
}
