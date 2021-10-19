using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggleBehaviour : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    [SerializeField] bool initialState;
    [SerializeField] Sprite[] sprites;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ToggleSprite(initialState);
    }

    public void ToggleSprite(bool value)
    {
        if (value)
        {
            spriteRenderer.sprite = sprites[0];
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
        }
    }
}
