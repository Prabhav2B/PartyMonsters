using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainExterior : MonoBehaviour
{
    [SerializeField] private TrainExteriorData trainExteriorData;
    [SerializeField] private SpriteRenderer trainExteriorSprite;

    private Line _line;
    
    void Start()
    {
        _line = trainExteriorData.trainLine;
        trainExteriorSprite.sprite = trainExteriorData.trainExterior;
    }

    
}
