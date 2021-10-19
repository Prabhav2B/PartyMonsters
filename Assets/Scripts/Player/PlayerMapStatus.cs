using System.Collections.Generic;
using System.Linq;
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

    public void CollectDrawnLines()
    {
        drawnLines.Clear();
        var mapLines = lineParent.GetComponentsInChildren<LineBehaviour>();

        foreach (var mapLine in mapLines)
        {
            var line = new LineConnection();
            line._stations.Add(mapLine.StationA.stationMapItem.myName);
            line._stations.Add(mapLine.StationB.stationMapItem.myName);

            line._lineColors.Add(mapLine.MyColor);
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
            
            evaluation = drawnLines.Exists(
                line => line._stations.Contains(nameA) && line._stations.Contains(nameB) && connection._lineColors.Intersect(line._lineColors).Any()
            );
        }

        return evaluation;
    }
}
