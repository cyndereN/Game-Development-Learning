using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDown : MonoBehaviour
{
    private Vector3 joyStickPosition, mousePosition, oldMousePosition, targetPosition;
    private Vector2 oldVector, newVector;
    private bool joystickFlag = false;
    public GameObject prefab;
    private GameObject phantomController;
    private int count = 0;
    private float oldDistance, newDistance;
    private float minScale = 0.3f;
    private float maxScale = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
		{
            Destroy(phantomController);
            joyStickPosition = Input.mousePosition;
            targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(joyStickPosition.x, joyStickPosition.y, 1000));
            phantomController = GameObject.Instantiate(prefab, targetPosition, prefab.transform.rotation);
            joystickFlag = true;
        } 
        else if (Input.GetMouseButtonDown(2))
		{
            Destroy(phantomController);
            joystickFlag = false;
        }
        else if (Input.GetMouseButton(1))
		{
			if (joystickFlag)
			{
                count++;
				if (count == 1)
				{
                    oldMousePosition = Input.mousePosition;
                    oldDistance = Vector2.Distance(oldMousePosition, joyStickPosition);
                    oldVector = oldMousePosition - joyStickPosition;
                    return;
                }


                mousePosition = Input.mousePosition;
                newDistance = Vector2.Distance(mousePosition, joyStickPosition);
                newVector = mousePosition - joyStickPosition;
                Vector2 deltaPos = newVector - oldVector;
                float angle = Vector2.SignedAngle(newVector, oldVector);
                float offset = newDistance - oldDistance;
                oldDistance = newDistance;
                oldMousePosition = mousePosition;
                oldVector = newVector;
                float scaleFactor = offset / 500f;
                Vector3 localScale = transform.localScale;
                Vector3 scale = new Vector3(localScale.x + scaleFactor,
                                            localScale.y + scaleFactor,
                                            localScale.z + scaleFactor);
                if (scale.x > minScale && scale.y > minScale && scale.z > minScale && scale.x < maxScale && scale.y < maxScale && scale.z < maxScale)
                {
                    transform.localScale = scale;
                }

                // Spinning
                transform.RotateAround(transform.position, Vector3.forward, -angle);
                Debug.Log(deltaPos);
            }
        }
        else if (Input.GetMouseButtonUp(1))
		{
            count = 0;
            oldDistance = 0;
            newDistance = 0;
        }
    }
}
