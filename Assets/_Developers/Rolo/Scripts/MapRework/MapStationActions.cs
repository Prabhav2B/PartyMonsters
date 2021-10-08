using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapStationActions : MonoBehaviour
{
    [SerializeField] private LayerMask RaycastLayers = default(LayerMask);

    Transform stationTransform;

    bool isPressed;

    int layerStation;
    int layerSlot;

    // Start is called before the first frame update
    void Start()
    {
        stationTransform = null;
        layerStation = LayerMask.NameToLayer("Token");
        layerSlot = LayerMask.NameToLayer("StationSlot");
    }

    // Update is called once per frame
    void Update()
    {
        if (stationTransform == null)
            return;

        if (isPressed)
        {
             DragStation();
        }
        else if (!isPressed)
        {
            SnapStationInPlace();
        }
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
                stationSlotBehaviour.isStationTaken = true; //TODO remove this line after the rework is done

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
            stationBehaviour.stationMapItem.slotStationIsOn.GetComponent<StationSlotBehaviour>().isStationTaken = false; //TODO remove this line and variable after rework is done, might be useful later
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
        }
    }

    public void OnResetMapItem(InputAction.CallbackContext context)
    {
        if (gameObject.activeSelf == false)
            return;

        Transform objectToReset;

        //place station to default pos
        objectToReset = CheckIfCursorOnMapItem(layerStation);
        if (objectToReset != null)
        {
            ResetStationPosition(objectToReset);
            ResetMapSlot(objectToReset);
            //release map slot
            //disconnect all connected lines
            return;
        }        

        //delete line
    }

    void ResetMapSlot(Transform objectToReset)
    { 

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
