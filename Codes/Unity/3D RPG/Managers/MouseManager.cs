using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;
using System;

//[System.Serializable]
// class EventVector3 : UnityEvent<Vector3> { }

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;  // 单例模式
    RaycastHit hitInfo;

    public Texture2D point, doorway, attack, target, arrow;

    private void Awake()
	{
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(this);
	}

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    // public EventVector3 OnMouseClicked;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;
    void SetCursorTexture()
	{
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(ray, out hitInfo))
		{
			// 切换鼠标贴图
			switch (hitInfo.collider.gameObject.tag)
			{
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;                
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;                
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Button":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
		}
	}

    void MouseControl()
	{
        if(Input.GetMouseButtonDown(0) && hitInfo.collider != null)
		{
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Button"))
                OnMouseClicked?.Invoke(hitInfo.point);
        }
	}
    public static bool IsInitialized
    {
        get { return Instance != null; }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

}
