using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class GameStart : MonoBehaviour
{
    public UnityEvent OnGameStart;

    private bool _started = false;

    public void StartGame()
    {
        _started = true;
        OnGameStart?.Invoke();
    }

    private void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame && !_started)
        {
            StartGame();
        }
    }
}
