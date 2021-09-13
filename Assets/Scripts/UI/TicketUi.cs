using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketUi : MonoBehaviour
{
    [SerializeField] Sprite missingTicket;
    [SerializeField] Sprite blueTicket;
    [SerializeField] Sprite pinkTicket;
    [SerializeField] Sprite greenTicket;
    [SerializeField] Sprite yellowTicket;
    [SerializeField] Sprite purpleTicket;
    [SerializeField] Sprite whiteTicket;

    
    private SpriteRenderer[] _ticketSprites;
    
    void Awake()
    {
        _ticketSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public void UpdateTicketSprites()
    {
        _ticketSprites[0].sprite = PlayerInventory.ContainsItemOfType(ItemType.TicketBlue) ? blueTicket : missingTicket;
        _ticketSprites[1].sprite = PlayerInventory.ContainsItemOfType(ItemType.TicketPink) ? pinkTicket : missingTicket;
        _ticketSprites[2].sprite = PlayerInventory.ContainsItemOfType(ItemType.TicketGreen) ? greenTicket : missingTicket;
        _ticketSprites[3].sprite = PlayerInventory.ContainsItemOfType(ItemType.TicketYellow) ? yellowTicket : missingTicket;
        _ticketSprites[4].sprite = PlayerInventory.ContainsItemOfType(ItemType.TicketPurple) ? purpleTicket : missingTicket;
        _ticketSprites[5].sprite = PlayerInventory.ContainsItemOfType(ItemType.TicketWhite) ? whiteTicket : missingTicket;

    }
}
