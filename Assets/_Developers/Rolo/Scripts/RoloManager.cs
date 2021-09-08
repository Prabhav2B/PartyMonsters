using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class RoloManager : MonoBehaviour
{
    [SerializeField] GameObject MapCanvas;
    [SerializeField] GameObject LinePrefab;
    [SerializeField] GameObject LineHolder;
    [SerializeField] MapSystem mapSystem;

    private bool isPressed;
    private bool isDragMode = true;

    private Vector2 lineStartPos;

    private Transform tokenTransform;
    private GameObject line;
    private LineRenderer lineRenderer;
    

    private List<GameObject> tokens;

    private Dictionary<Transform, int> connectedLines;
    

    private void Start()
    {
        tokens = new List<GameObject>();
        tokens.AddRange(GameObject.FindGameObjectsWithTag("Token"));
    }

    public void OnToggleMap(InputAction.CallbackContext context)
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

    public void OnDoubleClickTest(InputAction.CallbackContext context)
    {
        if (context.interaction is MultiTapInteraction && context.performed)
        {
            Debug.Log("double click");

            Transform transformToCheck;
            transformToCheck = CheckIfCursorOnMapItem("Token");

            if (transformToCheck != null)
            {
                transformToCheck.localPosition = transformToCheck.GetComponent<TokenBehaviour>().defaultPos;
                //delete all connections

                return;
            }

            transformToCheck = CheckIfCursorOnMapItem("ConnectionLine");

            if (transformToCheck != null)
            {
                Destroy(transformToCheck.gameObject);
            }
        }
    }

    public void OnDragToken(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction && context.performed)
        {
            isPressed = true;
            tokenTransform = CheckIfCursorOnMapItem("Token");

            if (!isDragMode && tokenTransform != null)
            {
                Debug.Log("Token Found");
                lineStartPos = new Vector2(Mathf.Floor(tokenTransform.localPosition.x) + 0.5f, Mathf.Floor(tokenTransform.localPosition.y) + 0.5f);

                line = Instantiate(LinePrefab, LineHolder.transform, LineHolder);
                lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, lineStartPos);
            }
            else if (tokenTransform != null)
            {
                connectedLines = new Dictionary<Transform, int>();
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("ConnectionLine");

                if (gameObjects != null)
                {
                    foreach (GameObject gameObject in gameObjects)
                    {
                        LineRenderer lr = gameObject.GetComponent<LineRenderer>();

                        if (lr != null)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if (lr.GetPosition(i) == tokenTransform.position)
                                {
                                    connectedLines.Add(gameObject.transform, i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            isPressed = false;
        }
    }

    public void OnChangeMode(InputAction.CallbackContext context)
    {
        isDragMode = !isDragMode;
    }

    Transform CheckIfCursorOnMapItem(string layerName)
    {
        Transform tCollider = null;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -10f));
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(layerName))
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

                    if (connectedLines != null)
                    {
                        foreach (var connectedLine in connectedLines)
                        {
                            connectedLine.Key.GetComponent<LineRenderer>().SetPosition(connectedLine.Value, tokenTransform.localPosition);
                        }
                    }

                    tokenTransform.Translate(tokenPos);
                }
                else
                {
                    Vector2 snapTo = new Vector2(Mathf.Floor(tokenTransform.localPosition.x) + 0.5f, Mathf.Floor(tokenTransform.localPosition.y) + 0.5f);

                    if (snapTo.x > -mapSystem.gridWidth / 2 && snapTo.x < mapSystem.gridWidth / 2 &&
                        snapTo.y > -mapSystem.gridHeight / 2 && snapTo.y < mapSystem.gridHeight / 2)
                    {
                        tokenTransform.localPosition = snapTo;
                        tokenTransform.GetComponent<TokenBehaviour>().previousPos = snapTo;
                    }
                    else
                    {
                        tokenTransform.localPosition = tokenTransform.gameObject.GetComponent<TokenBehaviour>().previousPos;
                    }
                    

                    if (connectedLines != null)
                    {
                        foreach (var connectedLine in connectedLines)
                        {
                            connectedLine.Key.GetComponent<LineRenderer>().SetPosition(connectedLine.Value, snapTo);
                        }
                    }

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
                    tokenTransform = CheckIfCursorOnMapItem("Token");
                    if (tokenTransform != null)
                    {
                        Vector2 snapTo = new Vector2(Mathf.Floor(tokenTransform.localPosition.x) + 0.5f, Mathf.Floor(tokenTransform.localPosition.y) + 0.5f);
                        lineRenderer.SetPosition(1, snapTo);
                        EdgeCollider2D col2D = line.GetComponent<EdgeCollider2D>();
                        List<Vector2> pointList = new List<Vector2>() { lineStartPos, snapTo };
                        col2D.SetPoints(pointList);
                        col2D.edgeRadius = 0.1f;
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
