using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCircleBehaviour : MonoBehaviour
{
    [SerializeField] private TrainLineColor _lineColor;
    public TrainLineColor CircleColor { get => _lineColor; }
}
