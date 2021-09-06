using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleInstance<T> : MonoBehaviour
{
    private static bool _instantiated;
    protected virtual void Awake()
    {
        Debug.Assert(!_instantiated, this.gameObject);
        if (_instantiated)
        {
            Destroy(this);
        }

        _instantiated = true;
    }
    
    private void DestroyInstance()
    {
        _instantiated = false;
        this.enabled = false;
        Destroy(this);
    }
}