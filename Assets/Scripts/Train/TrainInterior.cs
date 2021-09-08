using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainInterior : MonoBehaviour
{
    [SerializeField] private TrainInteriorData trainInteriorData;
    [SerializeField] private SpriteRenderer trainInteriorSprite;

    private Line _line;
    
    void Start()
    {
        _line = trainInteriorData.trainLine;
        trainInteriorSprite.sprite = trainInteriorData.trainInterior;
    }
}
