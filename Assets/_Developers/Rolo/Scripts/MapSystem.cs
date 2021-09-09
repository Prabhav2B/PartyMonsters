using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : MonoBehaviour
{
    [SerializeField] private int tokenAmount;

    [Range(2, 20)]
    public int gridWidth = 2;

    [Range(2, 10)]
    public int gridHeight = 2;

    [SerializeField] GameObject mapBackgrouund;
    [SerializeField] GameObject dot;
    [SerializeField] GameObject gridHolder;
    [SerializeField] GameObject lineHolder;
    [SerializeField] GameObject tokenHolder;

    private float offsetX;
    private float offsetY;

    public float localOffsetX;
    public float localOffsetY;

    void Start()
    {
        //mapBackgrouund.transform.localScale = new Vector2(gridWidth, gridHeight);
        localOffsetX = mapBackgrouund.transform.position.x;
        localOffsetY = mapBackgrouund.transform.position.y;

        if(gridWidth % 2 == 0)
            offsetX = -gridWidth / 2 + 0.5f;
        else
            offsetX = -gridWidth / 2;

        if (gridHeight % 2 == 0)
            offsetY = -gridHeight / 2 + 0.5f;
        else
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

                float posX = offsetX + x + localOffsetX;
                float posY = offsetY + y + localOffsetY;

                go.transform.localPosition = new Vector2(posX, posY);
                go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
        }
    }
}
