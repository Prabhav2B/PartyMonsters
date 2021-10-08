using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationSlotBehaviour : MonoBehaviour
{
    public StationMapItem stationMapItemOnThisSlot;
    public bool isStationTaken;

    private void Start()
    {
        stationMapItemOnThisSlot = new StationMapItem();
        stationMapItemOnThisSlot = null;
        isStationTaken = false;
    }
}
