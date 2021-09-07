using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoloManager : MonoBehaviour
{
    [SerializeField] GameObject MapCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void ToggleMap()
    {
        if (MapCanvas.activeInHierarchy)
        {
            MapCanvas.SetActive(false);
        }
        else
        {
            MapCanvas.SetActive(true);
        }
    }
}
