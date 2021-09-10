using UnityEngine;

[DisallowMultipleComponent]
public class ObjectToggler : MonoBehaviour
{
    public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);
}
