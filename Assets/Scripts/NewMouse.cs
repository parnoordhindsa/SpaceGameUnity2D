﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMouse : MonoBehaviour
{

    public Texture2D cursorArrow;
    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        Vector2 hotspot = new Vector2(cursorArrow.width / 2, cursorArrow.height / 2);
        Cursor.SetCursor(cursorArrow, hotspot, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
