using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapStationActions : MonoBehaviour
{
    [SerializeField] private CursorManager cursorManager;

    [SerializeField] private LayerMask RaycastLayers = default(LayerMask);

    [SerializeField] private Transform DrawIcon;
    [SerializeField] private Transform MoveIcon;
    [SerializeField] private Transform LineHolder;

    private EdgeCollider2D edgeCollider2D;

    private LineRenderer lineRenderer;

    private Transform stationTransform;
    private Transform uiButtonTransform;
    
    private bool isDragMode;
    private bool isDrawing;
    private bool isPressed;

    private int layerLine;
    private int layerStation;
    private int layerSlot;
    private int layerUI;

    // Start is called before the first frame update
    void Start()
    {
        isDragMode = true;
        isDrawing = false;
        stationTransform = null;

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

        if (isPressed && isDragMode)
        {
            DragStation();
        }
        else if (!isPressed && isDragMode)
        {
            SnapStationInPlace();
        }
        else if (isPressed && !isDragMode && !isDrawing)
        {
            StartDrawingLine();
        }
        else if (isPressed && !isDragMode && isDrawing)
        {
            UpdateLineEndPoint();
        }
        else if (!isPressed && !isDragMode && isDrawing)
        {
            EndDrawingLine();
        }
    }

    private void EndDrawingLine()
    {
        Transform station = CheckIfCursorOnMapItem(layerStation);
        Transform slot = CheckIfCursorOnMapItem(layerSlot);
        if (station == null || slot == null)
            Destroy(lineRenderer.gameObject);

        lineRenderer.SetPosition(1, station.transform.position);

        List<Vector2> pointList = new List<Vector2>() 
        {
            lineRenderer.GetPosition(0),
            lineRenderer.GetPosition(1)
        };

        edgeCollider2D.SetPoints(pointList);

        isDrawing = false;
        edgeCollider2D = null;
        lineRenderer = null;
    }

    private void UpdateLineEndPoint()
    {
        Vector3 cursorPosition = GetCursorPositionInWorld();
        lineRenderer.SetPosition(1, cursorPosition);
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
        line.layer = layerLine;

        lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.SetPosition(0, stationTransform.position);
        lineRenderer.startWidth = 0.4f;
        lineRenderer.endWidth = 0.4f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        edgeCollider2D = line.AddComponent<EdgeCollider2D>();
        edgeCollider2D.isTrigger = true;
        edgeCollider2D.edgeRadius = 0.25f;

        isDrawing = true;
    }

    private void SnapStationInPlace()
    {
        StationBehaviour stationBehaviour = stationTransform.GetComponent<StationBehaviour>();
        Transform stationSlot = CheckIfCursorOnMapItem(layerSlot);

        if (stationSlot != null)
        {
            StationSlotBehaviour stationSlotBehaviour = stationSlot.GetComponent<StationSlotBehaviour>();
            bool isStationFree = stationSlotBehaviour.stationMapItemOnThisSlot == null;

            if (isStationFree)
            {
                stationTransform.position = stationSlot.position;
                stationBehaviour.stationMapItem.previousPosition = stationTransform.position;

                //reset previous slot to be available
                ResetPreviousSlot(stationBehaviour);

                //set slot to be taken //making a function with 3 variables here doesn't make sense to me :/
                stationSlotBehaviour.stationMapItemOnThisSlot = stationBehaviour.stationMapItem;
                stationBehaviour.stationMapItem.slotStationIsOn = stationSlot; //adds current slot to Station's variable

                stationTransform = null;
                return;
            }                        
        }

        stationTransform.position = stationBehaviour.stationMapItem.previousPosition;
        stationTransform = null;
    }

    private void ResetPreviousSlot(StationBehaviour stationBehaviour)
    {
        if (stationBehaviour.stationMapItem.slotStationIsOn != null)
        {
            stationBehaviour.stationMapItem.slotStationIsOn.GetComponent<StationSlotBehaviour>().stationMapItemOnThisSlot = null;
        }
    }

    private void DragStation()
    {
        Vector3 mousePosition = GetCursorPositionInWorld();
        Vector2 tokenTranslation = mousePosition - stationTransform.position;
        stationTransform.Translate(tokenTranslation);
    }

    private Vector3 GetCursorPositionInWorld()
    {
        Vector2 mouseCoordinates = Mouse.current.position.ReadValue();
        Vector3 mousePosition = new Vector3(mouseCoordinates.x, mouseCoordinates.y, -Camera.main.transform.position.z);

        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public void OnDragStation(InputAction.CallbackContext context)
    {
        if (gameObject.activeSelf == false)
            return;

        isPressed = context.performed;

        if (isPressed)
        {
            stationTransform = CheckIfCursorOnMapItem(layerStation);
            if (stationTransform != null) return;

            uiButtonTransform = CheckIfCursorOnMapItem(layerUI);
            if (uiButtonTransform != null) 
            {
                SwitchCursorMode();
                return;
            }
        }
    }

    public void OnResetMapItem(InputAction.CallbackContext context)
    {
        if (gameObject.activeSelf == false)
            return;

        Transform objectToReset;

        //place station to default position
        objectToReset = CheckIfCursorOnMapItem(layerStation);
        if (objectToReset != null)
        {
            ResetStationPosition(objectToReset);
            //TODO disconnect all connected lines
            return;
        }

        objectToReset = CheckIfCursorOnMapItem(layerLine);
        if (objectToReset != null)
        {
            //TODO remove connections in connected stations
            Destroy(objectToReset.gameObject);
            return;
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
        objectToReset.transform.position = stationBehaviour.stationMapItem.defaultPosition;
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
