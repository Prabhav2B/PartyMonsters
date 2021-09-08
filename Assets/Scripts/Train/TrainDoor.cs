using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainDoor : MonoBehaviour
{

    [SerializeField] private LayerMask _playerLayer;
    private SpriteRenderer _interactionSprite; 
    
    
    void Start()
    {
        _interactionSprite = GetComponentInChildren<SpriteRenderer>();
        _interactionSprite.enabled = false;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_playerLayer & 1 << other.gameObject.layer) != 0)
        {
            _interactionSprite.enabled = true;
            other.GetComponent<PlayerManager>().AtDoor = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if ((_playerLayer & 1 << other.gameObject.layer) != 0)
        {
            _interactionSprite.enabled = false;
            other.GetComponent<PlayerManager>().AtDoor = false;
        }
    }
}
