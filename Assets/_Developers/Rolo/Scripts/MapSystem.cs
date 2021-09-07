using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSystem : MonoBehaviour
{
    [SerializeField] private int tokenAmount;

    [Range(2, 64)]
    [SerializeField] private int gridWidth = 2;

    [Range(2, 64)]
    [SerializeField] private int gridHeight = 2;

    [SerializeField] GameObject mapBackgrouund;
    [SerializeField] GameObject dot;
    [SerializeField] GameObject gridHolder;
    [SerializeField] GameObject tokenHolder;

    private float mapWidth;
    private float mapHeight;
    private float offsetX;
    private float offsetY;

    // Start is called before the first frame update
    void Start()
    {        
        mapWidth =  Screen.width * mapBackgrouund.transform.localScale.x;
        mapHeight = Screen.height * mapBackgrouund.transform.localScale.y;

        offsetX = (Screen.width - mapWidth) / 2;
        offsetY = (Screen.height - mapHeight) / 2;

        DrawGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject go = Instantiate(dot, gridHolder.transform);

                float posX = (offsetX + (mapWidth / gridWidth) / 2) + x * (mapWidth / gridWidth);
                float posY = (offsetY + (mapHeight / gridHeight)  / 2) + y * (mapHeight / gridHeight);
                Vector2 pos = new Vector2(posX, posY);

                go.transform.position = pos;
                go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
        }
    }
}
