using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private float _startTime;
    private float _currentTime;

    public float CurrentTime => _currentTime;
    
    private static Clock _instance;
    public static Clock Instance => _instance;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        
        _startTime = Time.time;
    }
    
}
