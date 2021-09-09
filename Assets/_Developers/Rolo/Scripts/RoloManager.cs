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
    [SerializeField] GameObject Palette;
    [SerializeField] MapSystem MapSystem;

    private bool isPressed;
    private bool isPickingColor;
    private bool isDragTokenMode;

    private GameObject line;
    private LineRenderer lineRenderer;
    private Transform tokenTransform;
    private Transform lineTransform;

    private Vector2 lineStartPos;


    private List<GameObject> tokens;

    private Dictionary<Transform, int> connectedLines;
    

    private void Start()
    {
        tokens = new List<GameObject>();
        tokens.AddRange(GameObject.FindGameObjectsWithTag("Token"));

        isDragTokenMode = true;
        isPickingColor = false;
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

            //TODO remove connections in tokens

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

        if (isPressed && !isPickingColor)
        {
            tokenTransform = CheckIfCursorOnMapItem("Token");
            if (tokenTransform != null)
            {
                if (!isDragTokenMode)
                {
                    StartDrawingLine();
                }

                else if (isDragTokenMode && tokenTransform != null)
                {
                    UpdateLineEndingsOnDrag();
                }

                return;
            }

            lineTransform = CheckIfCursorOnMapItem("ConnectionLine");
            if (lineTransform != null)
            {
                isPickingColor = true;
                float posX = (lineTransform.gameObject.GetComponent<LineRenderer>().GetPosition(0).x + lineTransform.gameObject.GetComponent<LineRenderer>().GetPosition(1).x) / 2;
                float posY = (lineTransform.gameObject.GetComponent<LineRenderer>().GetPosition(0).y + lineTransform.gameObject.GetComponent<LineRenderer>().GetPosition(1).y) / 2;
                Vector2 paletteplacement = new Vector2(posX, posY);
                Palette.transform.position = paletteplacement;
                isPressed = false;
                Palette.SetActive(true);
                return;
            }
        }

        else if (isPressed && isPickingColor)
        {
            Debug.Log("Set Color");
            Transform paletteTransform = CheckIfCursorOnMapItem("PaletteColor");
            if (paletteTransform != null)
            {
                Debug.Log(paletteTransform);
                Color lineColor = paletteTransform.gameObject.GetComponent<SpriteRenderer>().color;
                lineTransform.gameObject.GetComponent<LineRenderer>().startColor = lineColor;
                lineTransform.gameObject.GetComponent<LineRenderer>().endColor = lineColor;
            }

            lineTransform = null;
            isPickingColor = false;
            isPressed = false;
            Palette.SetActive(false);
            return;
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

    void SnapLineToToken(Vector2 target)
    {
        if (connectedLines != null)
        {
            foreach (var connectedLine in connectedLines)
            {
                connectedLine.Key.GetComponent<LineRenderer>().SetPosition(connectedLine.Value, target);
            }
        }
    }

    void UpdateLineColliders()
    {
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
            if (line == null && !isPickingColor)
            {
                if (isPressed)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -10f));
                    Vector2 tokenPos = mousePos - tokenTransform.position;

                    SnapLineToToken(tokenTransform.localPosition);
                    tokenTransform.Translate(tokenPos);
                }
                else if (!isPressed)
                {
                    Vector2 snapTo = new Vector2(Mathf.Floor(tokenTransform.localPosition.x) + 0.5f, Mathf.Floor(tokenTransform.localPosition.y) + 0.5f);

                    if (snapTo.x > -MapSystem.gridWidth / 2 && snapTo.x < MapSystem.gridWidth / 2 &&
                        snapTo.y > -MapSystem.gridHeight / 2 && snapTo.y < MapSystem.gridHeight / 2)
                    {
                        tokenTransform.localPosition = snapTo;
                        tokenTransform.GetComponent<TokenBehaviour>().previousPos = snapTo;

                        SnapLineToToken(snapTo);
                        UpdateLineColliders();
                    }
                    else
                    {
                        snapTo = tokenTransform.gameObject.GetComponent<TokenBehaviour>().previousPos;
                        tokenTransform.localPosition = snapTo;
                        SnapLineToToken(snapTo);
                    }                    

                    tokenTransform = null;
                }
            }

            //line drawing functionality
            else
            {
                if (isPressed && !isPickingColor)
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
