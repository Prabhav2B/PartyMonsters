using UnityEngine;

public enum ItemType
{
    Default,
    TicketPurple,
    TicketGreen,
    TicketYellow,
    TicketPink,
    TicketBlue,
    Hat
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType type = default(ItemType);
}
