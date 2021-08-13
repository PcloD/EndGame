using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    public enum CursorType
    {
        Default = 0,
        Attack = 1
    }
    
    [Serializable]
    public class CursorData
    {
        public CursorType cursorType;
        public Texture2D texture;
    }

    public static MouseCursorManager Instance;
    public CursorData DefaultCursor;
    public ParticleSystem ClickMoveParticle;

    private void Awake()
    {
        Instance = this;
        Cursor.SetCursor(DefaultCursor.texture, Vector3.zero, CursorMode.Auto);
        MouseCursorManager.Instance.ClickMoveParticle.Stop(true);
    }
}
