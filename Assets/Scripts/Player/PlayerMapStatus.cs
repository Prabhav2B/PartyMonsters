using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMapStatus : MonoBehaviour
{
    [SerializeField] private Transform lineParent;

    private TrainScheduler _trainScheduler;
    
    private static List<LineConnection> drawnLines = new List<LineConnection>();

    private void Awake()
    {
        _trainScheduler = FindObjectOfType<TrainScheduler>();
    }

    void Update()
    {
        drawnLines.Clear();
        var mapLines = lineParent.GetComponentsInChildren<ConnectionLineBehaviour>();

        foreach (var mapLine in mapLines)
        {
            var line = new LineConnection();
            line._stations.Add(mapLine.connectedStations[0].myName);
            line._stations.Add(mapLine.connectedStations[1].myName);

            line._lineColor = mapLine.myColor;
            drawnLines.Add(line);

        }
    }

    public static bool ContainsItemOfType(List<LineConnection> requiredConnections)
    {
        bool evaluation = false;

        foreach (var connection in requiredConnections)
        {
            StationName nameA = connection._stations[0];
            StationName nameB = connection._stations[1];

            TrainLineColor color = connection._lineColor;
            
            evaluation = drawnLines.Exists(line => line._stations.Contains(nameA) && line._stations.Contains(nameB) && line._lineColor == color );
        }

        return evaluation;

    }
}
