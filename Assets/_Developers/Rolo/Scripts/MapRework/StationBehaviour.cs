using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class StationBehaviour : MonoBehaviour
{
    public StationName myName;
    public StationMapItem stationMapItem;

    private void Start()
    {
        stationMapItem = new StationMapItem();

        stationMapItem.defaultPosition = transform.localPosition;
        stationMapItem.previousPosition = transform.localPosition;

        stationMapItem.slotStationIsOn = null;

        stationMapItem.StationTitleText = gameObject.GetComponentInChildren<Text>();
        stationMapItem.StationTitleText.text = myName.ToString().ToUpper().Replace('_', ' ');
    }
}
