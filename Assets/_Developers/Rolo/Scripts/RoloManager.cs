using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoloManager : MonoBehaviour
{
    [SerializeField] GameObject MapCanvas;

    private bool isPressed;

    void OnToggleMap()
    {
        if (MapCanvas.activeInHierarchy)
        {
            MapCanvas.SetActive(false);
        }
        else
        {
            MapCanvas.SetActive(true);
        }
    }    

    void OnDragToken(InputValue input)
    {
        isPressed = input.isPressed;   
    }

    private void Update()
    {
        if (isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -10f));
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Token"))
                {
                    Vector2 tokenPos = mousePos - hit.collider.transform.position;
                    hit.collider.transform.Translate(tokenPos);
                }
            }
        }
        else
        {
            //if above dot
                //snap it
            //else if out of the map
                //reset back to top
        }
    }
}
