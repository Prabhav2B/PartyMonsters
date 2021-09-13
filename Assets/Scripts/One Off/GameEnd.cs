using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEnd : MonoBehaviour
{
    public UnityEvent onGameOver;
    
    [SerializeField] GameObject _endScene;
    private SceneFadeManager _sceneFadeManager;

    private bool _endTriggered;

    private void Awake()
    {
        _endScene.SetActive(false);

        _sceneFadeManager = FindObjectOfType<SceneFadeManager>();

        onGameOver.AddListener(()=>_sceneFadeManager.WhiteIn(() => { }));
        onGameOver.AddListener(()=>_endScene.SetActive(true));
    }

    public void EndGame()
    {
        if (PlayerInventory.ContainsItemOfType(ItemType.TicketWhite) && !_endTriggered)
        {
            _endTriggered = true;
            _sceneFadeManager.WhiteOut(() => onGameOver?.Invoke());
        }
    }
}
