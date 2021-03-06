using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainInteriorData", menuName = "Data/TrainInterior")]
public class TrainInteriorData : ScriptableObject
{
    public TrainLineColor trainTrainLineColor;
    public Sprite trainInterior;
    public float waitDuration = 5f;
}
