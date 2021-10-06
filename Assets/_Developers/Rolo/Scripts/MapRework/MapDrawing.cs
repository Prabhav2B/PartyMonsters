using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDrawing : MonoBehaviour
{ 
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float tokenScale;
    [SerializeField] private GameObject MapSlot;
    [SerializeField] private Transform MapSlotHolder;

    private float gridOffsetX;
    private float gridOffsetY;

    // Start is called before the first frame update
    void Start()
    {
        gridOffsetX = SetGridOffset(mapWidth);
        gridOffsetY = SetGridOffset(mapHeight);
        DrawMap();
    }

    private float SetGridOffset(int value)
    {
        if (value % 2 == 0)
        {
            return 0.5f;
        }
        else
        {
            return 0f;
        }
    }

    private void DrawMap()
    {
        float widthWithOffset = (float)mapWidth / 2f;
        float heightWithOffset = (float)mapHeight / 2f;

        for (float x = -widthWithOffset; x < widthWithOffset; x++)
        {
            for (float y = -heightWithOffset; y < heightWithOffset; y++)
            {
                float posX = x + gridOffsetX;
                float posY = y + gridOffsetY;
                float collider2DScale = 1f / tokenScale;
                Vector2 slotPosition = new Vector2(posX, posY);

                GameObject slot = Instantiate(MapSlot, MapSlotHolder);
                slot.transform.localPosition = slotPosition;
                slot.transform.localScale = new Vector3(tokenScale, tokenScale, tokenScale);

                BoxCollider2D collider = slot.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(collider2DScale, collider2DScale);
            }
        }
    }
}
