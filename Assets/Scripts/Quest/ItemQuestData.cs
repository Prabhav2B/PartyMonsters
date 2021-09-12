using UnityEngine;

[CreateAssetMenu(fileName = "ItemQuestData", menuName = "Data/ItemQuest")]
public class ItemQuestData : QuestData
{
    public ItemType requiredItemType = default(ItemType);

    protected override bool EvaluateRequirements() => PlayerInventory.ContainsItemOfType(requiredItemType);
}
