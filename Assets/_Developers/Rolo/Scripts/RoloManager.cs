using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class RoloManager : MonoBehaviour
{
    [SerializeField] private GameObject MapCanvas;
    [SerializeField] private GameObject LinePrefab;
    [SerializeField] private GameObject LineHolder;
    [SerializeField] private GameObject Palette;
    [SerializeField] private GameObject MoveIcon;
    [SerializeField] private GameObject DrawIcon;
    [SerializeField] private MapSystem MapSystem;
    [SerializeField] private Transform BackgroundImage;

    private bool isPressed;
    private bool isPickingColor;
    private bool isDragTokenMode;

    private GameObject line;
    private LineRenderer lineRenderer;
    private Transform stationTransform;
    private Transform lineTransform;
    private Transform toggleTransform;

    private Vector2 lineStartPos;
    private Camera mainCam;

    private List<GameObject> tokens;

    private Dictionary<Transform, int> connectedLines;
    

    private void Start()
    {
        mainCam = Camera.main;
        
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
            DeleteConnectedLinesFromGUI(transformToCheck);
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
        StationBehaviour sb = t.GetComponent<StationBehaviour>();
        t.position = sb.defaultPos;
        sb.previousPos = sb.defaultPos;       
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
            stationTransform = CheckIfCursorOnMapItem("Token");
            if (stationTransform != null)
            {
                if (!isDragTokenMode)
                {
                    //check if line is within boundaries
                    if (stationTransform.position.x >= -MapSystem.gridWidth / 2 + BackgroundImage.position.x && stationTransform.position.x <= MapSystem.gridWidth / 2 + BackgroundImage.position.x &&
                        stationTransform.position.y >= -MapSystem.gridHeight / 2 + BackgroundImage.position.y && stationTransform.position.y <= MapSystem.gridHeight / 2 + BackgroundImage.position.y)
                    {
                        StartDrawingLine();
                        AddStationToTheLine(stationTransform);
                    }
                    else
                    {
                        line = new GameObject();
                        line.name = "dummy";
                    }
                }

                else if (isDragTokenMode && stationTransform != null)
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

            toggleTransform = CheckIfCursorOnMapItem("UI");
            if (toggleTransform != null)
            {
                isDragTokenMode = toggleTransform.gameObject == MoveIcon ? true : false;
                MoveIcon.GetComponent<UIToggleBehaviour>().ToggleSprite(isDragTokenMode);
                DrawIcon.GetComponent<UIToggleBehaviour>().ToggleSprite(!isDragTokenMode);

                return;
            }
        }

        else if (isPressed && isPickingColor)
        {
            Transform paletteTransform = CheckIfCursorOnMapItem("PaletteColor");
            if (paletteTransform != null)
            {
                Color lineColor = paletteTransform.gameObject.GetComponent<SpriteRenderer>().color;
                lineTransform.gameObject.GetComponent<LineRenderer>().startColor = lineColor;
                lineTransform.gameObject.GetComponent<LineRenderer>().endColor = lineColor;

                switch (ColorUtility.ToHtmlStringRGB(lineColor))
                {
                    case ("005DE4"):
                        lineTransform.gameObject.GetComponent<ConnectionLineBehaviour>().myColor = TrainLineColor.blue;
                        break;
                    case ("0D4D26"):
                        lineTransform.gameObject.GetComponent<ConnectionLineBehaviour>().myColor = TrainLineColor.green;
                        break;
                    case ("FF89C8"):
                        lineTransform.gameObject.GetComponent<ConnectionLineBehaviour>().myColor = TrainLineColor.pink;
                        break;
                    case ("42007B"):
                        lineTransform.gameObject.GetComponent<ConnectionLineBehaviour>().myColor = TrainLineColor.purple;
                        break;
                    case ("DCE62C"):
                        lineTransform.gameObject.GetComponent<ConnectionLineBehaviour>().myColor = TrainLineColor.yellow;
                        break;
                    default:
                        lineTransform.gameObject.GetComponent<ConnectionLineBehaviour>().myColor = TrainLineColor._null;
                        break;
                }
            }

            lineTransform = null;
            isPickingColor = false;
            isPressed = false;
            Palette.SetActive(false);
            return;
        }     
    }

    void AddStationToTheLine(Transform t)
    {
        if (line != null)
        {
            StationBehaviour station = t.gameObject.GetComponent<StationBehaviour>();
            line.gameObject.GetComponent<ConnectionLineBehaviour>().connectedStations.Add(station);
        }
    }


    void StartDrawingLine()
    {
        lineStartPos = new Vector2(stationTransform.position.x, stationTransform.position.y); ;
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
                        if (lr.GetPosition(i) == stationTransform.position)
                        {
                            connectedLines.Add(gameObject.transform, i);
                            break;
                        }
                    }
                }
            }
        }
    }

    Transform CheckIfCursorOnMapItem(string layerName)
    {
        Transform tCollider = null;
        Vector3 mousePos = mainCam.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -10f));
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
        Vector2 mousePos = mainCam.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -10f));
        lineRenderer.SetPosition(1, mousePos);
    }

    void SnapLineToGrid()
    {
        Vector2 snapTo = new Vector2(stationTransform.position.x, stationTransform.position.y);
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
        if (stationTransform != null)
        {
            //token drag functionality
            if (line == null && !isPickingColor)
            {
                if (isPressed)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -10f));
                    Vector2 tokenPos = mousePos - stationTransform.position;

                    SnapLineToToken(stationTransform.position);
                    stationTransform.Translate(tokenPos);
                }
                else if (!isPressed)
                {
                    Vector2 snapTo = new Vector2(Mathf.Floor(stationTransform.position.x) + 0.5f, Mathf.Floor(stationTransform.position.y + 0.5f));

                    if (snapTo.x >= -MapSystem.gridWidth / 2 + BackgroundImage.position.x && snapTo.x <= MapSystem.gridWidth / 2 + BackgroundImage.position.x &&
                        snapTo.y >= -MapSystem.gridHeight / 2 + BackgroundImage.position.y && snapTo.y <= MapSystem.gridHeight / 2 + BackgroundImage.position.y)
                    {
                        stationTransform.position = snapTo;
                        stationTransform.GetComponent<StationBehaviour>().previousPos = snapTo;

                        SnapLineToToken(snapTo);
                        UpdateLineColliders();
                    }
                    else
                    {
                        snapTo = stationTransform.gameObject.GetComponent<StationBehaviour>().previousPos;
                        stationTransform.position = snapTo;
                        SnapLineToToken(snapTo);
                    }                    

                    stationTransform = null;
                }
            }

            //line drawing functionality
            else
            {
                if (isPressed && !isPickingColor && line.name != "dummy")
                {
                    MoveLineEndAlongCursor();
                }
                else
                {
                    stationTransform = CheckIfCursorOnMapItem("Token");
                    if (stationTransform != null && line.name != "dummy")
                    {
                        if (stationTransform.position == line.GetComponent<LineRenderer>().GetPosition(0))
                        {
                            Destroy(line);
                        }
                        else
                        {
                            SnapLineToGrid();
                            AddStationToTheLine(stationTransform);
                        }
                    }
                    else
                    {
                        Destroy(line);
                    }

                    line = null;
                    stationTransform = null;
                }
            }
        }
    }
}
