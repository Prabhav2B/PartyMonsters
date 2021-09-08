using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : MonoBehaviour
{
    [SerializeField] private int tokenAmount;

    [Range(2, 20)]
    [SerializeField] private int gridWidth = 2;

    [Range(2, 10)]
    [SerializeField] private int gridHeight = 2;

    [SerializeField] GameObject mapBackgrouund;
    [SerializeField] GameObject dot;
    [SerializeField] GameObject gridHolder;
    [SerializeField] GameObject lineHolder;
    [SerializeField] GameObject tokenHolder;

    private float offsetX;
    private float offsetY;

    void Start()
    {
        mapBackgrouund.transform.localScale = new Vector2(gridWidth, gridHeight);

        offsetX = -gridWidth / 2;
        offsetY = -gridHeight / 2;

        DrawGrid();
    }

    void DrawGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject go = Instantiate(dot, gridHolder.transform);

                float posX = offsetX + x + 0.5f;
                float posY = offsetY + y + 0.5f;
                Vector2 pos = new Vector2(posX, posY);

                go.transform.position = pos;
                go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
        }
    }
}
