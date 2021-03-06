using UnityEngine;

public enum ItemType
{
    Default,
    TicketPurple,
    TicketGreen,
    TicketYellow,
    TicketPink,
    TicketBlue,
    Hat,
    TicketWhite
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType = default(ItemType);
    public Sprite itemIcon = null;
    public string itemName = string.Empty;
}
