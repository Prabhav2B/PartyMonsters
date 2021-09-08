using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainExteriorData", menuName = "Data/TrainExterior")]
public class TrainExteriorData : ScriptableObject
{
    public Line trainLine;
    public Sprite trainExterior;
    public Sprite trainDoor;
    public Sprite trainDoorOpen;
}

public enum Line
{
    purple,
    green,
    yellow,
    pink,
    blue
}