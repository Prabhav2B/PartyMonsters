using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLineBehaviour : MonoBehaviour
{
    List<GameObject> ConnectedStatoins;

    private void Start()
    {
        ConnectedStatoins = new List<GameObject>();
    }
}
