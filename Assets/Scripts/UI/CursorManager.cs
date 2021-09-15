using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct CursorSettings
{
    public Texture2D Icon;
    public Vector2 HotSpot;
}

[DisallowMultipleComponent]
public class CursorManager : SingleInstance<CursorManager>
{
    [SerializeField]
    private bool _cursorVisible = true;
    [SerializeField]
    private CursorSettings[] _cursors = new CursorSettings[0];

    protected override void Awake()
    {
        base.Awake();
        SetCursorVisible(_cursorVisible);
    }

    public void SetCursor(int index)
    {
        if (index >= 0 && index < _cursors.Length)
        {
            var cursor = _cursors[index];
            Cursor.SetCursor(cursor.Icon, cursor.HotSpot, CursorMode.Auto);
        }
    }

    public void ResetCursor() => Cursor.SetCursor(PlayerSettings.defaultCursor, Vector2.zero, CursorMode.Auto);

    public void SetCursorVisible(bool value) => Cursor.visible = value;
}
