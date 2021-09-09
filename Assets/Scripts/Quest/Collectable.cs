using UnityEngine;

[DisallowMultipleComponent]
public class Collectable : MonoBehaviour
{
    [SerializeField]
    private ItemData _item = null;

    public void Collect()
    {
        gameObject.SetActive(false);
        PlayerInventory.AddItem(_item);
    }
}
