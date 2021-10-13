using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "InteriorBackgroundData", menuName = "Data/InteriorBackground")]
public class InteriorBackgroundData : ScriptableObject
{
    public Sprite backSprite;
    public Sprite midSprite;
    public Sprite frontSprite;
    
    public float backSpeed = 1;
    public float midSpeed = 5;
    public float frontSpeed = 8;
}
