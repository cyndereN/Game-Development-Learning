using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerOverUI : MonoBehaviour
{
    public Texture2D cursorTexture;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
		}
		else
		{
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
