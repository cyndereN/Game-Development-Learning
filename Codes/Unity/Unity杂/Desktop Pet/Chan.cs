using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chan : MonoBehaviour
{
    public float weight = 1f;
    public float headWeight = 1f;
    public float bodyWeight = 1f;
    public float eyesWeight = 1f;
    public float clampWeight = 1f;

    Animator anim;
    Vector3 lookPosition;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        lookPosition = new Vector3(-pos.x*2f+1f, pos.y*1.85f, 0.5f);
		if (Input.GetMouseButtonDown(0))
		{
            anim.Play("Idle2");
		}
    }

	private void OnAnimatorIK(int layerIndex)
	{
        anim.SetLookAtPosition(lookPosition);
        anim.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
	}
}
