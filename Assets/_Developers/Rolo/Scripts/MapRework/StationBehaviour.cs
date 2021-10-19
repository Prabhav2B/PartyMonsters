using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StationBehaviour : MonoBehaviour
{
    [SerializeField] private StationName _myName;
    public StationMapItem stationMapItem;

    private void Start()
    {
        stationMapItem.myName = _myName;

        stationMapItem.defaultPosition = transform.localPosition;
        stationMapItem.previousPosition = transform.localPosition;

        stationMapItem.slotStationIsOn = null;

        stationMapItem.stationTitleText = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        stationMapItem.stationTitleText.text = _myName.ToString().ToUpper().Replace('_', ' ');
    }
}
