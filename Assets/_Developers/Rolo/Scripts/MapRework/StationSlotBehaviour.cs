using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationSlotBehaviour : MonoBehaviour
{
    public StationMapItem stationMapItemOnThisSlot;

    private void Start()
    {
        stationMapItemOnThisSlot = new StationMapItem();
        stationMapItemOnThisSlot = null;
    }
}
