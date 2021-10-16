using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationSlotBehaviour : MonoBehaviour
{
    public StationMapItem stationMapItemOnThisSlot { get; set; }

    private void Start()
    {
        stationMapItemOnThisSlot = default(StationMapItem);
    }
}
