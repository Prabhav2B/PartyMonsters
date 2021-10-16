using UnityEngine;
using UnityEngine.UI;

public struct StationMapItem
{
    public StationName myName;
    public Transform slotStationIsOn;
    public TMPro.TextMeshProUGUI stationTitleText;
    public Vector2 defaultPosition;
    public Vector2 previousPosition;
}
