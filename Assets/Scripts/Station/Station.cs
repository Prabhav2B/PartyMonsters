using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    [SerializeField] private StationScriptableObject stationData;
    [SerializeField] private SpriteRenderer stationBackground;
    [SerializeField] private SpriteRenderer stationForeGround;

    public StationScriptableObject StationData => stationData;

    private string _stationName;
    
    private void Start()
    {
        _stationName = stationData.stationName;
        stationBackground.sprite = stationData.stationBackground;
        stationForeGround.sprite = stationData.stationForeground;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
