using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameStart : MonoBehaviour
{
    private PlayerManager _playerManger;

    public UnityEvent OnGameStart;
    
    void Awake()
    {
        _playerManger = FindObjectOfType<PlayerManager>();
        _playerManger.GetComponent<PlayerInput>().enabled = false;
    }

    public void StartGame()
    {
        OnGameStart?.Invoke();
    }
}
