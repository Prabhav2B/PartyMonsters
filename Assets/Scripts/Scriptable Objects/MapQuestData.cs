using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapQuestData", menuName = "Data/MapQuest")]
public class MapQuestData : QuestData
{
    public List<LineConnection> requiredConnections = new List<LineConnection>();

    protected override bool EvaluateRequirements() => PlayerMapStatus.ContainsItemOfType(requiredConnections);
}

[Serializable]
public class LineConnection
{
    public List<StationName> _stations = new List<StationName>();

    public List<TrainLineColor> _lineColors = new List<TrainLineColor>();
}