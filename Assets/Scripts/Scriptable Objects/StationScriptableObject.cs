using UnityEngine;

[CreateAssetMenu(fileName = "StationData", menuName = "Data/Station")]
public class StationScriptableObject : ScriptableObject
{
    public string stationName;
    public Sprite stationBackground;
    public Sprite stationForeground;
    public int musicIndex;
}
