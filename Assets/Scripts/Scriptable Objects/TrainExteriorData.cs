using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainExteriorData", menuName = "Data/TrainExterior")]
public class TrainExteriorData : ScriptableObject
{
    public TrainLineColor trainTrainLineColor;
    public Sprite trainExterior;
    public Sprite trainDoor;

    public float waitDuration = 5f;
}

