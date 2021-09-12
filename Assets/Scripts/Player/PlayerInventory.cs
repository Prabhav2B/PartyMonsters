using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private List<ItemData> _startItems = new List<ItemData>();

    public static event Action<ItemData> OnAddItem = delegate {};
    public static event Action<ItemData> OnRemoveItem = delegate {};
    
    private static readonly List<ItemData> _items = new List<ItemData>();

    private void Awake() => _items.AddRange(_startItems);

    public static void AddItem(ItemData item)
    {
        _items.Add(item);
        OnAddItem.Invoke(item);
    }

    public static void RemoveItem(ItemData item)
    {
        _items.Remove(item);
        OnRemoveItem.Invoke(item);
    }
    

    public static bool ContainsItemOfType(ItemType type) => _items.Exists(item => item.type == type);
}
