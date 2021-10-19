using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestManager : MonoBehaviour
{
    [SerializeField] private GameObject MapCanvas;

    public void OnToggleMap()
    {
        bool toggleMap = !MapCanvas.activeInHierarchy;
    }
}
