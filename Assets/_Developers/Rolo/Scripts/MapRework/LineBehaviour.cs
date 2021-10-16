using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBehaviour : MonoBehaviour
{
    public StationBehaviour StationA { get; set; }

    public StationBehaviour StationB { get; set; }

    public TrainLineColor MyColor { get; set; }
}
