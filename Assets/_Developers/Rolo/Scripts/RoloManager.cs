using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoloManager : MonoBehaviour
{
    [SerializeField] GameObject MapCanvas;
    [SerializeField] GameObject LinePrefab;
    [SerializeField] GameObject LineHolder;

    private bool isPressed;
    private bool isDragMode = true;

    private Vector2 lineStartPos;

    private Transform tokenTransform;
    private GameObject line;
    private LineRenderer lineRenderer;

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

        if (isPressed)
        {
            tokenTransform = CheckIfCursorOnToken();

            if (!isDragMode && tokenTransform != null)
            {
                lineStartPos = new Vector2(Mathf.Floor(tokenTransform.localPosition.x) + 0.5f, Mathf.Floor(tokenTransform.localPosition.y) + 0.5f);

                line = Instantiate(LinePrefab, LineHolder.transform, LineHolder);
                lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, lineStartPos);
            }
            else
            { 
                //check and store connected line endings
            }
        }
    }

    void OnChangeMode()
    {
        isDragMode = !isDragMode;
    }

    Transform CheckIfCursorOnToken()
    {
        Transform tCollider = null;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -10f));
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Token"))
            {
                tCollider =  hit.collider.transform;
            }
        }

        return tCollider;
    }

    private void Update()
    {
        if (tokenTransform != null)
        {
            if (line == null)
            {
                if (isPressed)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -10f));
                    Vector2 tokenPos = mousePos - tokenTransform.position;

                    //update connected lines if any

                    tokenTransform.Translate(tokenPos);
                }
                else
                {
                    Vector2 snapTo = new Vector2(Mathf.Floor(tokenTransform.localPosition.x) + 0.5f, Mathf.Floor(tokenTransform.localPosition.y) + 0.5f);
                    tokenTransform.localPosition = snapTo;

                    //snap connected lines as well if any

                    tokenTransform = null;
                }
            }

            else
            {
                if (isPressed)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -10f));
                    Vector2 endPos = mousePos;
                    lineRenderer.SetPosition(1, endPos);
                }
                else if (!isPressed)
                {
                    tokenTransform = CheckIfCursorOnToken();
                    if (tokenTransform != null)
                    {
                        Vector2 snapTo = new Vector2(Mathf.Floor(tokenTransform.localPosition.x) + 0.5f, Mathf.Floor(tokenTransform.localPosition.y) + 0.5f);
                        lineRenderer.SetPosition(1, snapTo);
                    }
                    else
                    {
                        //info that line must end above token and inside the field
                        Destroy(line);
                    }

                    line = null;
                    tokenTransform = null;
                }
            }            
        }
    }
}
