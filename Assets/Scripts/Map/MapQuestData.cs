using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapQuestData : QuestData
{
    public List<TrainLineColor> lineColorToBeFilled = new List<TrainLineColor>();

    protected override bool EvaluateRequirements() => PlayerMapStatus.ContainsItemOfType(lineColorToBeFilled);
}
