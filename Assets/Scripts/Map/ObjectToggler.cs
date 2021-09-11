using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class ObjectToggler : MonoBehaviour
{
    public UnityEvent OnToggle = new UnityEvent();
    public UnityEvent OnOpen = new UnityEvent();
    public UnityEvent OnClose = new UnityEvent();

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf)
        {
            OnOpen.Invoke();
        }
        else
        {
            OnClose.Invoke();
        }
        OnToggle.Invoke();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        OnOpen.Invoke();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        OnClose.Invoke();
    }
}
