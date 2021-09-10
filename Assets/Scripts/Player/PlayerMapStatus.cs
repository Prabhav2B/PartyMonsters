using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMapStatus : MonoBehaviour
{
    [SerializeField] private Transform lineParent;

    private TrainScheduler _trainScheduler;
    
    private List<TrainLineColor> correctLines = new List<TrainLineColor>();

    private void Awake()
    {
        _trainScheduler = FindObjectOfType<TrainScheduler>();
    }

    void EvaluateMap()
    {
        correctLines.Clear();
        var mapLines = lineParent.GetComponentsInChildren<ConnectionLineBehaviour>();

        foreach (var mapLine in mapLines)
        {
            
        }
    }

    public static bool ContainsItemOfType(List<TrainLineColor> lineColorToBeFilled)
    {
        throw new System.NotImplementedException();
    }
}
