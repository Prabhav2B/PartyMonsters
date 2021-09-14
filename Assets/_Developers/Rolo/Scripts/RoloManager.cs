using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class RoloManager : MonoBehaviour
{
    [SerializeField] private LayerMask RaycastLayers = default(LayerMask);
    [SerializeField] private GameObject MapCanvas;
    [SerializeField] private GameObject LinePrefab;
    [SerializeField] private GameObject LineHolder;
    [SerializeField] private GameObject Palette;
    [SerializeField] private GameObject MoveIcon;
    [SerializeField] private GameObject DrawIcon;
    [SerializeField] private MapSystem MapSystem;
    [SerializeField] private Transform BackgroundImage;
    [SerializeField] private ObjectToggler objectToggler;

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

    public void OnResetMapItem(InputAction.CallbackContext context)
    {
        if (objectToggler.gameObject.activeSelf == false)
            return;

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
        t.localPosition = sb.defaultPos;
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

    public void OnDragToken(InputAction.CallbackContext context)
    {
        if (objectToggler.gameObject.activeSelf == false)
            return;

        isPressed = context.performed;

        switch (isPressed)
        {
            case true when !isPickingColor:
            {
                stationTransform = CheckIfCursorOnMapItem("Token");
                if (stationTransform != null)
                {
                    if (!isDragTokenMode)
                    {
                        //check if line is within boundaries
                        if (stationTransform.position.x >= -MapSystem.gridWidth / 2 + transform.position.x &&
                            stationTransform.position.x <= MapSystem.gridWidth / 2 + transform.position.x &&
                            stationTransform.position.y >= -MapSystem.gridHeight / 2 + transform.position.y &&
                            stationTransform.position.y <= MapSystem.gridHeight / 2 + transform.position.y)
                        {
                            StartDrawingLine();
                            // AddStationToTheLine(stationTransform);
                        }
                        else
                        {
                            isDragTokenMode = true;
                            ToggleTool();
                            // line = new GameObject();
                            // line.name = "dummy";
                        }
                        // StartDrawingLine();
                        AddStationToTheLine(stationTransform);
                    }

                    if (isDragTokenMode && stationTransform != null)
                    {
                        UpdateLineEndingsOnDrag();
                    }

                    return;
                }

                lineTransform = CheckIfCursorOnMapItem("ConnectionLine");
                if (lineTransform != null)
                {
                    isPickingColor = true;
                    var lr = lineTransform.GetComponent<LineRenderer>();
                    var displacement = lr.GetPosition(0) - lr.GetPosition(1);
                    Palette.transform.position = lineTransform.TransformPoint(lr.GetPosition(1) + (displacement * 0.5f));
                    isPressed = false;
                    Palette.SetActive(true);
                    return;
                }

                toggleTransform = CheckIfCursorOnMapItem("UI");
                if (toggleTransform != null)
                {
                    isDragTokenMode = toggleTransform.gameObject == MoveIcon ? true : false;
                    ToggleTool();

                    return;
                }

                break;
            }
            case true when isPickingColor:
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
                            lineTransform.gameObject.GetComponent<ConnectionLineBehaviour>().myColor =
                                TrainLineColor.purple;
                            break;
                        case ("DCE62C"):
                            lineTransform.gameObject.GetComponent<ConnectionLineBehaviour>().myColor =
                                TrainLineColor.yellow;
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
    }

    void ToggleTool()
    {
        MoveIcon.GetComponent<UIToggleBehaviour>().ToggleSprite(isDragTokenMode);
        DrawIcon.GetComponent<UIToggleBehaviour>().ToggleSprite(!isDragTokenMode);
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
        lineStartPos = new Vector2(stationTransform.position.x, stationTransform.position.y);
        line = Instantiate(LinePrefab, LineHolder.transform, LineHolder);
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, lineStartPos);
    }

    void UpdateLineEndingsOnDrag()
    {
        connectedLines = new Dictionary<Transform, int>();
        ConnectionLineBehaviour[] lines = FindObjectsOfType<ConnectionLineBehaviour>();

        foreach (var line in lines)
        {
            for (int i = 0; i < 2; i++)
            {
                float distance = Vector2.Distance((Vector2) line.connectedStations[i].transform.position,
                    (Vector2) stationTransform.position );
                if (distance <= 0.01f)
                {
                    connectedLines.Add(line.transform, i);
                    break;
                }
            }
        }

        // if (gameObjects != null)
        // {
        //     foreach (GameObject gameObject in gameObjects)
        //     {
        //         LineRenderer lr = gameObject.GetComponent<LineRenderer>();
        //
        //         if (lr != null)
        //         {
        //             for (int i = 0; i < 2; i++)
        //             {
        //                 float distance = Vector2.Distance((Vector2) stationTransform.position,
        //                     (Vector2) lr.GetPosition(i) );
        //                 if (distance <= 0.01f)
        //                 {
        //                     connectedLines.Add(gameObject.transform, i);
        //                     break;
        //                 }
        //             }
        //         }
        //     }
        //}
    }

    Transform CheckIfCursorOnMapItem(string layerName)
    {
        Transform tCollider = null;
        Vector3 mousePos = mainCam.ScreenToWorldPoint(new Vector2(Mouse.current.position.x.ReadValue(),
            Mouse.current.position.y.ReadValue()));
        Collider2D[] colliders = Physics2D.OverlapPointAll((Vector2) mousePos, RaycastLayers);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                tCollider = collider.transform;
                return tCollider;
            }
        }
        return tCollider;
    }

    void MoveLineEndAlongCursor()
    {
        Vector2 mousePos = mainCam.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(),
            Mouse.current.position.y.ReadValue(), -10f));
        lineRenderer.SetPosition(1, mousePos);
    }

    void SnapLineToGrid()
    {
        Vector2 snapTo = new Vector2(stationTransform.position.x, stationTransform.position.y);
        lineRenderer.SetPosition(1, snapTo);
        EdgeCollider2D col2D = line.GetComponent<EdgeCollider2D>();
        List<Vector2> pointList = new List<Vector2>() {lineStartPos, snapTo};
        col2D.SetPoints(pointList);
        col2D.edgeRadius = 0.1f;
    }

    void SnapLineToToken(Vector2 target)
    {
        if (connectedLines != null)
        {
            foreach (var connectedLine in connectedLines)
            {
                connectedLine.Key.GetComponent<LineRenderer>().SetPosition(connectedLine.Value, connectedLine.Key.InverseTransformPoint(target));
            }
        }
    }

    void UpdateLineColliders()
    {
        if (connectedLines != null)
        {
            foreach (var connectedLine in connectedLines)
            {
                EdgeCollider2D col2D = connectedLine.Key.gameObject.GetComponent<EdgeCollider2D>();
                Vector2 startPos = connectedLine.Key.gameObject.GetComponent<LineRenderer>().GetPosition(0);
                Vector2 endPos = connectedLine.Key.gameObject.GetComponent<LineRenderer>().GetPosition(1);
                List<Vector2> pointList = new List<Vector2>() {startPos, endPos};
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
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(),
                        Mouse.current.position.y.ReadValue(), -10f));
                    Vector2 tokenTranslation = mousePos - stationTransform.position;

                    SnapLineToToken(stationTransform.position);
                    stationTransform.Translate(tokenTranslation);
                }
                else if (!isPressed)
                {
                    Vector2 snapTo = new Vector2(Mathf.Round(stationTransform.localPosition.x),
                        Mathf.Round(stationTransform.localPosition.y));

                    Debug.Log("Released at: " + snapTo);
                    Debug.Log("(float)((float)-MapSystem.gridWidth / 2 + BackgroundImage.transform.localPosition.x) = " + (float)(-MapSystem.gridWidth / 2f + BackgroundImage.transform.localPosition.x));
                    Debug.Log("(float)((float)MapSystem.gridWidth / 2 + BackgroundImage.transform.localPosition.x) = " + (float)(MapSystem.gridWidth / 2f + BackgroundImage.transform.localPosition.x));
                    Debug.Log("(float)((float)-MapSystem.gridHeight / 2 + BackgroundImage.transform.localPosition.y) = " + (float)(-MapSystem.gridHeight / 2f + BackgroundImage.transform.localPosition.y));
                    Debug.Log("(float)((float)MapSystem.gridHeight / 2 + BackgroundImage.transform.localPosition.y) = " + (float)(MapSystem.gridHeight / 2f + BackgroundImage.transform.localPosition.y));

                    if (snapTo.x >= (float)(-MapSystem.gridWidth / 2f + BackgroundImage.transform.localPosition.x) &&
                        snapTo.x <= (float)(MapSystem.gridWidth / 2f + BackgroundImage.transform.localPosition.x) &&
                        snapTo.y >= (float)(-MapSystem.gridHeight / 2f + BackgroundImage.transform.localPosition.y) &&
                        snapTo.y <= (float)(MapSystem.gridHeight / 2f + BackgroundImage.transform.localPosition.y))
                    {
                        float delta = 0f;

                        delta = snapTo.x < 0 ? 0.5f : -0.5f;
                        stationTransform.localPosition = new Vector2(snapTo.x + delta, snapTo.y);
                        stationTransform.GetComponent<StationBehaviour>().previousPos = new Vector2(snapTo.x + delta, snapTo.y);

                        SnapLineToToken(new Vector2(snapTo.x + delta, snapTo.y));
                        UpdateLineColliders();
                    }
                    else
                    {
                        snapTo = stationTransform.gameObject.GetComponent<StationBehaviour>().previousPos;                        
                        stationTransform.localPosition = snapTo;
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
                        float distance = Vector2.Distance((Vector2) stationTransform.position,
                            (Vector2) line.GetComponent<LineRenderer>().GetPosition(0));
                        if (distance <= 0.01f)
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