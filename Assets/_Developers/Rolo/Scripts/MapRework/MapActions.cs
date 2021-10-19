using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapActions : MonoBehaviour
{
    [SerializeField] private Transform Map; //This must be uniform!!!

    [SerializeField] private CursorManager cursorManager;

    [SerializeField] private GameObject ColorPicker;

    [SerializeField] private LayerMask RaycastLayers = default(LayerMask);

    [SerializeField] private int lineSortingOrder;

    [SerializeField] private Transform DrawIcon;
    [SerializeField] private Transform MoveIcon;
    [SerializeField] private Transform LineHolder;

    private Dictionary<LineRenderer, int> connectedLines;

    private LineRenderer lineRenderer; //recheck how often this is used, maybe no need to keep it like this
    
    private string lineSortingLayer;

    private Transform colorPickerTransform;
    private Transform lineTransform;
    private Transform stationTransform;
    private Transform uiButtonTransform;
    
    private bool isDragMode;
    private bool isDrawing;
    private bool isPressed;

    private int layerColorPicker;
    private int layerLine;
    private int layerStation;
    private int layerSlot;
    private int layerUI;

    // Start is called before the first frame update
    void Start()
    {
        lineSortingLayer = "UI";

        isDragMode = true;
        isDrawing = false;
        isPressed = false;

        colorPickerTransform = null;
        lineTransform = null;
        stationTransform = null;
        uiButtonTransform = null;

        layerColorPicker = LayerMask.NameToLayer("ColorPicker");
        layerLine = LayerMask.NameToLayer("ConnectionLine");
        layerStation = LayerMask.NameToLayer("Token");
        layerSlot = LayerMask.NameToLayer("StationSlot");
        layerUI = LayerMask.NameToLayer("UI");
    }

    // Update is called once per frame
    void Update()
    {
        if (stationTransform == null)
            return;

        switch (isPressed)
        {
            case true when isDragMode:
                DragStation();
                break;

            case false when isDragMode:
                SnapStationInPlace();
                SnapConnectedLinesInPlace();
                UpdateLineColliders();

                stationTransform = null;
                break;

            case true when !isDragMode && !isDrawing:
                StartDrawingLine();
                break;

            case true when !isDragMode && isDrawing:
                UpdateLineEndPoint();
                break;

            case false when !isDragMode && isDrawing:
                EndDrawingLine();
                break;
        }
    }

    private void UpdateLineColliders()
    {
        foreach (var line in connectedLines)
        {
            EdgeCollider2D edgeCollider2D = line.Key.gameObject.GetComponent<EdgeCollider2D>();
            SetEdgeCollider2DPoints(line.Key, edgeCollider2D);           
        }
    }

    private void SetEdgeCollider2DPoints(LineRenderer lineRenderer, EdgeCollider2D edgeCollider2D)
    {
        List<Vector2> pointList = new List<Vector2>()
                {
                    lineRenderer.GetPosition(0),
                    lineRenderer.GetPosition(1)
                };

        edgeCollider2D.SetPoints(pointList);
    }

    private bool CheckIfConnectionAlreadyExists(StationBehaviour stationA, StationBehaviour stationB)
    {
        LineBehaviour[] lines = FindObjectsOfType<LineBehaviour>();

        foreach (LineBehaviour line in lines)
        {
            if ((line.StationA == stationA || line.StationA == stationB) &&
               (line.StationB == stationA || line.StationB == stationB))
                return true;
        }

        return false;
    }

    private void SnapConnectedLinesInPlace()
    {
        if (connectedLines != null)
        {
            foreach (var line in connectedLines)
            {
                line.Key.useWorldSpace = false;
                line.Key.SetPosition(line.Value, stationTransform.localPosition);
            }
        }
    }

    private void EndDrawingLine()
    {
        //check if button released over station AND map slot
        Transform station = CheckIfCursorOnMapItem(layerStation);
        Transform slot = CheckIfCursorOnMapItem(layerSlot);
        if ((station == null || slot == null))
        {
            Destroy(lineRenderer.gameObject);
        }
        else
        {
            LineBehaviour lineBehaviour = lineRenderer.gameObject.GetComponent<LineBehaviour>();
            StationBehaviour stationB = station.GetComponent<StationBehaviour>();
            bool connectionExists = CheckIfConnectionAlreadyExists(lineBehaviour.StationA, stationB);
            bool isTheSameStation = lineBehaviour.StationA == stationB;

            if (connectionExists || isTheSameStation)
            {
                Destroy(lineRenderer.gameObject);
            }
            else
            {
                lineBehaviour.StationB = stationB;

                //snaps ending right under the station position
                lineRenderer.SetPosition(1, station.transform.localPosition);

                //sets EdgeCollider2D in place
                EdgeCollider2D edgeCollider2D = lineRenderer.gameObject.GetComponent<EdgeCollider2D>();
                SetEdgeCollider2DPoints(lineRenderer, edgeCollider2D);

                ToggleColorPicker();
                if (ColorPicker.activeInHierarchy)
                    SetColorPickerPosition();
            }
        }

        isDrawing = false;
        lineRenderer = null;
    }

    private void UpdateLineEndPoint()
    {
        Vector3 cursorPosition = GetCursorPositionInWorld();
        Vector3 cameraPosition = Camera.main.transform.position;

        cursorPosition = new Vector3(cursorPosition.x - cameraPosition.x, cursorPosition.y - cameraPosition.y, 0f);

        lineRenderer.SetPosition(1, cursorPosition * 1f/Map.localScale.x);
    }

    private void StartDrawingLine()
    {
        Transform station = CheckIfCursorOnMapItem(layerStation);
        Transform slot = CheckIfCursorOnMapItem(layerSlot);
        if (slot == null || station == null)
            return;

        GameObject line = new GameObject();
        line.name = "Line";
        line.transform.parent = LineHolder;
        line.transform.localPosition = LineHolder.localPosition;
        line.transform.localScale = new Vector3(1, 1, 1);
        line.layer = layerLine;

        LineBehaviour lineBehaviour = line.AddComponent<LineBehaviour>();
        lineBehaviour.StationA = station.gameObject.GetComponent<StationBehaviour>();
        lineBehaviour.MyColor = TrainLineColor.blue;

        lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.sortingLayerName = lineSortingLayer;
        lineRenderer.sortingOrder = lineSortingOrder;
        lineRenderer.SetPosition(0, stationTransform.localPosition);
        lineRenderer.SetPosition(1, stationTransform.localPosition);
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = GetLineColor(lineBehaviour.MyColor);
        lineRenderer.endColor = GetLineColor(lineBehaviour.MyColor);

        EdgeCollider2D edgeCollider2D = line.AddComponent<EdgeCollider2D>();
        edgeCollider2D.isTrigger = true;
        edgeCollider2D.edgeRadius = 0.1f;

        lineTransform = line.transform;
        isDrawing = true;
    }

    private Color GetLineColor(TrainLineColor lineColor)
    {
        switch (lineColor)
        {
            case (TrainLineColor.blue):
                return new Color(0f, 0.36f, 0.89f);
            case (TrainLineColor.green):
                return new Color(0.05f, 0.3f, 0.15f);
            case (TrainLineColor.pink):
                return new Color(1f, 0.54f, 0.78f);
            case (TrainLineColor.purple):
                return new Color(0.26f, 0f, 0.48f);
            case (TrainLineColor.yellow):
                return new Color(0.86f, 0.9f, 0.17f);
            case (TrainLineColor.white):
                return new Color(1f, 1f, 1f);
            default:
                return new Color(0f, 0f, 0f);
        }
    }

    private void SnapStationInPlace()
    {
        StationBehaviour stationBehaviour = stationTransform.GetComponent<StationBehaviour>();
        Transform stationSlot = CheckIfCursorOnMapItem(layerSlot);

        if (stationSlot != null)
        {
            StationSlotBehaviour stationSlotBehaviour = stationSlot.GetComponent<StationSlotBehaviour>();
            bool isStationFree = stationSlotBehaviour.stationMapItemOnThisSlot.Equals(default(StationMapItem));

            if (isStationFree)
            {
                stationTransform.localPosition = stationSlot.localPosition;
                stationBehaviour.stationMapItem.previousPosition = stationTransform.localPosition;

                //reset previous slot to be available
                ResetPreviousSlot(stationBehaviour);

                //set slot to be taken
                stationSlotBehaviour.stationMapItemOnThisSlot = stationBehaviour.stationMapItem;
                stationBehaviour.stationMapItem.slotStationIsOn = stationSlot; //adds current slot to Station's variable

                return;
            }                        
        }

        stationTransform.localPosition = stationBehaviour.stationMapItem.previousPosition;
    }

    private void ResetPreviousSlot(StationBehaviour stationBehaviour)
    {
        if (stationBehaviour.stationMapItem.slotStationIsOn != null)
        {
            stationBehaviour.stationMapItem.slotStationIsOn.GetComponent<StationSlotBehaviour>().stationMapItemOnThisSlot = default(StationMapItem);
        }
    }

    private void DragStation()
    {
        Vector3 mousePosition = GetCursorPositionInWorld();
        Vector2 stationTranslation = mousePosition - stationTransform.position;
        stationTransform.Translate(stationTranslation);
        UpdateConnectedLines();
    }

    private void UpdateConnectedLines()
    {
        foreach (var line in connectedLines)
        {
            line.Key.SetPosition(line.Value, stationTransform.localPosition);
        }
    }

    private void GetConnectedLines()
    {
        StationBehaviour stationBehaviour = stationTransform.gameObject.GetComponent<StationBehaviour>();

        connectedLines = new Dictionary<LineRenderer, int>();

        LineBehaviour[] lines = FindObjectsOfType<LineBehaviour>();
        foreach (LineBehaviour line in lines)
        {
            if (line.StationA == stationBehaviour || line.StationB == stationBehaviour)
            {
                LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
                int endingId = line.StationA == stationBehaviour ? 0 : 1;
                connectedLines.Add(lineRenderer, endingId);
            }
        }
    }

    private Vector3 GetCursorPositionInWorld()
    {
        Camera camera = Camera.main;
        Vector2 mouseCoordinates = Mouse.current.position.ReadValue();
        Vector3 mousePosition = new Vector3(mouseCoordinates.x, mouseCoordinates.y, -camera.transform.position.z);

        return camera.ScreenToWorldPoint(mousePosition);
    }

    public void OnDragStation(InputAction.CallbackContext context)
    {
        if (gameObject.activeSelf == false)
            return;

        isPressed = context.performed;

        if (!isPressed) 
            return;
        
        colorPickerTransform = CheckIfCursorOnMapItem(layerColorPicker);
        
        if (ColorPicker.activeInHierarchy && colorPickerTransform == null)
            ToggleColorPicker();
        else if (ColorPicker.activeInHierarchy && colorPickerTransform != null)
        {
            SetLineColor();
            ToggleColorPicker();
            return;
        }

        stationTransform = CheckIfCursorOnMapItem(layerStation);
        if (stationTransform != null)
        {
            GetConnectedLines();
            return;
        }

        uiButtonTransform = CheckIfCursorOnMapItem(layerUI);
        if (uiButtonTransform != null) 
        {
            SwitchCursorMode();
            return;
        }

        lineTransform = CheckIfCursorOnMapItem(layerLine);
        if (lineTransform != null)
        {
            ToggleColorPicker();
            if (ColorPicker.activeInHierarchy)
            {
                SetColorPickerPosition();
            }

            return;
        }
    }

    private void SetLineColor()
    {
        TrainLineColor trainLineColor = colorPickerTransform.GetComponent<ColorCircleBehaviour>().CircleColor;
        LineRenderer lineRenderer = lineTransform.GetComponent<LineRenderer>();

        lineRenderer.startColor = GetLineColor(trainLineColor);
        lineRenderer.endColor = GetLineColor(trainLineColor);
        lineTransform.GetComponent<LineBehaviour>().MyColor = trainLineColor;
    }

    private void ToggleColorPicker()
    {
        ColorPicker.SetActive(!ColorPicker.activeInHierarchy);
    }

    private void SetColorPickerPosition()
    {
        if(lineTransform != null)
            lineRenderer = lineTransform.gameObject.GetComponent<LineRenderer>();

        Vector3 midPointBetweenTwoPoints = (lineRenderer.GetPosition(0) + lineRenderer.GetPosition(1)) / 2;
        ColorPicker.transform.localPosition = midPointBetweenTwoPoints;
    }

    public void OnResetMapItem(InputAction.CallbackContext context)
    {
        if (gameObject.activeSelf == false)
            return;

        if(ColorPicker.activeInHierarchy)
        {
            ToggleColorPicker();
            return;
        }    

        Transform objectToReset;

        //place station to default position
        objectToReset = CheckIfCursorOnMapItem(layerStation);
        if (objectToReset != null)
        {            
            DisconnectConnectedLines(objectToReset);
            ResetStationPosition(objectToReset);

            StationBehaviour stationBehaviour = objectToReset.GetComponent<StationBehaviour>();
            ResetPreviousSlot(stationBehaviour);

            return;
        }

        objectToReset = CheckIfCursorOnMapItem(layerLine);
        if (objectToReset != null)
        {
            Destroy(objectToReset.gameObject);
            return;
        }
    }

    private void DisconnectConnectedLines(Transform station)
    {
        LineBehaviour[] lines = FindObjectsOfType<LineBehaviour>();
        StationBehaviour stationBehaviour = station.gameObject.GetComponent<StationBehaviour>();

        foreach (LineBehaviour line in lines)
        {
            if (line.StationA == stationBehaviour || line.StationB == stationBehaviour) 
            {
                Destroy(line.gameObject);
            }
        }
    }

    void SwitchCursorMode()
    {
        isDragMode = uiButtonTransform == MoveIcon ? true : false;
        ToggleToolSprite();
        ToggleToolCursor();
    }

    void ToggleToolSprite()
    {
        MoveIcon.GetComponent<UIToggleBehaviour>().ToggleSprite(isDragMode);
        DrawIcon.GetComponent<UIToggleBehaviour>().ToggleSprite(!isDragMode);
    }

    void ToggleToolCursor()
    {
        int cursorIndex = (isDragMode) ? 0 : 1;
        cursorManager.SetCursor(cursorIndex);
    }

    void ResetStationPosition(Transform objectToReset)
    {
        StationBehaviour stationBehaviour = objectToReset.GetComponent<StationBehaviour>();
        objectToReset.transform.localPosition = stationBehaviour.stationMapItem.defaultPosition;
        stationBehaviour.stationMapItem.previousPosition = stationBehaviour.stationMapItem.defaultPosition;
    }

    Transform CheckIfCursorOnMapItem(int layer)
    {
        Transform targetTransform = null;
        Vector3 mousePosition = GetCursorPositionInWorld();
        Collider2D[] colliders = Physics2D.OverlapPointAll((Vector2)mousePosition, RaycastLayers);

        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer == layer)
            {
                targetTransform = collider.transform;
                break;
            }
        }

        return targetTransform;
    }
}