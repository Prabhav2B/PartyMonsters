using System;
using UnityEngine;

public abstract class QuestData : ScriptableObject
{
    public ItemData reward = null;

    [NonSerialized]
    public bool accomplished = false;

    public bool TryAccomplishQuest()
    {
        accomplished = EvaluateRequirements();
        if (accomplished && reward != null)
        {
            PlayerInventory.AddItem(reward);
        }
        return accomplished;
    }

    protected abstract bool EvaluateRequirements();
}
