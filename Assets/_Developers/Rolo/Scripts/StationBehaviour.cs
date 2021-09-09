using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StationBehaviour : MonoBehaviour
{
    public Vector2 defaultPos;
    public Vector2 previousPos;

    public List<GameObject> connectedLines;

    private void Start()
    {
        connectedLines = new List<GameObject>();
        defaultPos = transform.position;
        previousPos = defaultPos;
    }
}
