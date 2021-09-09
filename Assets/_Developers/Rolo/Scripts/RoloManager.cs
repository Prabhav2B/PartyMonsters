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
    private bool isDragTokenMode;

    private GameObject line;
    private LineRenderer lineRenderer;
    private Transform tokenTransform;

    private Vector2 lineStartPos;


    private List<GameObject> tokens;

    private Dictionary<Transform, int> connectedLines;
    

    private void Start()
    {
        tokens = new List<GameObject>();
        tokens.AddRange(GameObject.FindGameObjectsWithTag("Token"));

        isDragTokenMode = true;
    }

    public void OnToggleMap()
    {
        bool toggleMap = MapCanvas.activeInHierarchy ? false : true;
        MapCanvas.SetActive(toggleMap);
    }

    public void OnResetMapItem()
    {
        Transform transformToCheck;

        transformToCheck = CheckIfCursorOnMapItem("Token");
        if (transformToCheck != null)
        {
            //delete connected lines from UI
            DeleteConnectedLinesFromGUI(transformToCheck);

            //remove connections in tokens

            //reset token placement
            ResetTokenPlacement(transformToCheck);    

            return;
        }

        transformToCheck = CheckIfCursorOnMapItem("ConnectionLine");
        if (transformToCheck != null)
        {
            Destroy(transformToCheck.gameObject);
        }
    }

    void ResetTokenPlacement(Transform t)
    {
        TokenBehaviour tb = t.GetComponent<TokenBehaviour>();
        t.localPosition = tb.defaultPos;
        tb.previousPos = tb.defaultPos;       
    }

    void DeleteConnectedLinesFromGUI(Transform t)
    {
        GameObject[] connectionLines = GameObject.FindGameObjectsWithTag("ConnectionLine");

        if (connectedLines != null)
        {
            foreach (GameObject connectionLine in connectionLines)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (connectionLine.GetComponent<LineRenderer>().GetPosition(i) == t.position)
                    {
                        Destroy(connectionLine);
                        break;
                    }
                }
            }
        }        
    }

    public void OnDragToken(InputValue input)
    {
        isPressed = input.isPressed;

        if (isPressed)
        {
            tokenTransform = CheckIfCursorOnMapItem("Token");

            if (!isDragTokenMode && tokenTransform != null)
            {
                StartDrawingLine();
            }

            else if (isDragTokenMode && tokenTransform != null)
            {
                UpdateLineEndingsOnDrag();
            }
        }       
        else
        {
            isPressed = false;
        }
    }

    void StartDrawingLine()
    {
        lineStartPos = new Vector2(Mathf.Floor(tokenTransform.localPosition.x) + 0.5f, Mathf.Floor(tokenTransform.localPosition.y) + 0.5f);
        line = Instantiate(LinePrefab, LineHolder.transform, LineHolder);
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, lineStartPos);
    }

    void UpdateLineEndingsOnDrag()
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

    public void OnChangeMode()
    {
        isDragTokenMode = !isDragTokenMode;
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

   

    void MoveLineEndAlongCursor()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -10f));
        lineRenderer.SetPosition(1, mousePos);
    }

    void SnapLineToGrid()
    {
        Vector2 snapTo = new Vector2(Mathf.Floor(tokenTransform.localPosition.x) + 0.5f, Mathf.Floor(tokenTransform.localPosition.y) + 0.5f);
        lineRenderer.SetPosition(1, snapTo);
        EdgeCollider2D col2D = line.GetComponent<EdgeCollider2D>();
        List<Vector2> pointList = new List<Vector2>() { lineStartPos, snapTo };
        col2D.SetPoints(pointList);
        col2D.edgeRadius = 0.1f;
    }

    void UpdateLineColliders()
    {
        Debug.Log("Update colliders");
        if (connectedLines != null)
        {
            foreach(var connectedLine in connectedLines)
            {
                EdgeCollider2D col2D = connectedLine.Key.gameObject.GetComponent<EdgeCollider2D>();
                Vector2 startPos = connectedLine.Key.gameObject.GetComponent<LineRenderer>().GetPosition(0);
                Vector2 endPos = connectedLine.Key.gameObject.GetComponent<LineRenderer>().GetPosition(1);
                List<Vector2> pointList = new List<Vector2>() { startPos, endPos };
                col2D.SetPoints(pointList);
            }
        }
    }


    private void Update()
    {
        if (tokenTransform != null)
        {
            //token drag functionality
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
                       
                        UpdateLineColliders();
                    }

                    tokenTransform = null;
                }
            }

            //line drawing functionality
            else
            {
                if (isPressed)
                {
                    MoveLineEndAlongCursor();
                }
                else
                {
                    tokenTransform = CheckIfCursorOnMapItem("Token");
                    if (tokenTransform != null)
                    {
                        SnapLineToGrid();
                        //add connection to the token color, connection end id, connected stations
                    }
                    else
                    {
                        Destroy(line);
                    }

                    line = null;
                    tokenTransform = null;
                }
            }
        }
    }
}
