using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainInteriorData", menuName = "Data/TrainInterior")]
public class TrainInteriorData : ScriptableObject
{
    public Line trainLine;
    public Sprite trainInterior;
}
