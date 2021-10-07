using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDrawing : MonoBehaviour
{
    [Range(2, 20)]
    [SerializeField] private int mapWidth;
    [Range(2, 10)]
    [SerializeField] private int mapHeight;

    [SerializeField] private float tokenScale;
    [SerializeField] private GameObject MapSlot;
    [SerializeField] private Transform MapSlotHolder;

    private float gridOffset;

    // Start is called before the first frame update
    void Start()
    {
        gridOffset = 0.5f; // cosmetitc value that ensures that slots are centered nicely
        DrawMap();
    }

    private void DrawMap()
    {
        float widthWithOffset = (float)mapWidth / 2f;
        float heightWithOffset = (float)mapHeight / 2f;

        for (float x = -widthWithOffset; x < widthWithOffset; x++)
        {
            for (float y = -heightWithOffset; y < heightWithOffset; y++)
            {
                float posX = x + gridOffset;
                float posY = y + gridOffset;
                float collider2DScale = 1f / tokenScale; // ensures that collider is 1 unit high and wide
                Vector2 slotPosition = new Vector2(posX, posY);

                GameObject slot = Instantiate(MapSlot, MapSlotHolder);
                slot.transform.localPosition = slotPosition;
                slot.transform.localScale = new Vector3(tokenScale, tokenScale, tokenScale);

                BoxCollider2D collider = slot.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.size = new Vector2(collider2DScale, collider2DScale);
            }
        }
    }
}
