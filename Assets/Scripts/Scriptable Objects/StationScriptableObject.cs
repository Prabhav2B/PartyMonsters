using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StationData", menuName = "Data/Station")]
public class StationScriptableObject : ScriptableObject
{
    public string stationName;
    public Sprite stationBackground;
    public Sprite stationForeground;
}
