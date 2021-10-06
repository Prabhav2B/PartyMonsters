using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class StationBehaviour : MonoBehaviour
{
    public Vector2 defaultPos;
    public Vector2 previousPos;

    public StationName myName;

    [SerializeField] private Text StationTitleText;

    private void Start()
    {
        StationTitleText.text = myName.ToString().ToUpper().Replace('_', ' ');
        defaultPos = transform.localPosition;
        previousPos = defaultPos;
    }
}
