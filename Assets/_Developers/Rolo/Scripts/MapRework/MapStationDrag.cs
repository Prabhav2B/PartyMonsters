using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapStationDrag : MonoBehaviour
{
    [SerializeField] private LayerMask RaycastLayers = default(LayerMask);

    Transform stationTransform;

    bool isPressed;
    // Start is called before the first frame update
    void Start()
    {
        stationTransform = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
        {
            if (stationTransform != null)
            {
                DragStation();
            }
        }
        else if (!isPressed && stationTransform != null)
        {
            SnapStationInPlace();
        }
    }

    private void SnapStationInPlace()
    {
        StationBehaviour sb = stationTransform.gameObject.GetComponent<StationBehaviour>();
        Transform stationSlot = CheckIfCursorOnMapItem("StationSlot");
        if (stationSlot != null)
        {
            stationTransform.position = stationSlot.position;
            sb.previousPos = stationTransform.position;
        }
        else
        {
            stationTransform.position = sb.previousPos;
        }

        stationTransform = null;
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

    public void OnDragToken(InputAction.CallbackContext context)
    {
        if (gameObject.activeSelf == false)
            return;

        isPressed = context.performed;

        if (isPressed)
        {
            stationTransform = CheckIfCursorOnMapItem("Token");
            if (stationTransform != null)
            {
                
            }
        }
    }

    Transform CheckIfCursorOnMapItem(string layerName)
    {
        Transform targetTransform = null;
        Vector3 mousePosition = GetCursorPositionInWorld();
        Collider2D[] colliders = Physics2D.OverlapPointAll((Vector2)mousePosition, RaycastLayers);

        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                targetTransform = collider.transform;
                break;
            }
        }

        return targetTransform;
    }
}
