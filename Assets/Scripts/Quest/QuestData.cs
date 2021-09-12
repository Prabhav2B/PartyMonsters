using System;
using UnityEngine;

public abstract class QuestData : ScriptableObject
{
    public ItemData reward = null;

    [NonSerialized]
    public bool accomplished = false;

    public Action OnAccomplished = delegate {};
    public Action OnFailed = delegate {};

    public bool TryAccomplishQuest()
    {
        accomplished = EvaluateRequirements();
        if (accomplished && reward != null)
        {
            PlayerInventory.AddItem(reward);
            OnAccomplished.Invoke();
        }
        else
        {
            OnFailed.Invoke();
        }
        return accomplished;
    }

    protected abstract bool EvaluateRequirements();
}
