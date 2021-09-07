using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TokenBehaviour : MonoBehaviour
{
    private bool isDragging;
    private bool isPressed;

    void OnDragToken(InputValue input)
    {
        isPressed = input.isPressed;        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isPressed);

        if (isDragging)
        {
            //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.mousePosition) - transform.position;
            //transform.Translate(mousePos);
        }
    }
}
