using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{

	public float moveSpeed = 3f;
	private Rigidbody2D rb;
	private Animator anim;
	private Vector3 target;
	private Vector2 dir;
	private bool isMoving;


	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		target = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		// Input
		if (Input.GetMouseButtonDown(1))
		{
			target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			target.z = transform.position.z;

			dir.x = (target - transform.position).normalized.x;
			dir.y = (target - transform.position).normalized.y;
		}

		SwitchAnimation();
	}

	private void FixedUpdate()
	{
		Movement();
	}

	void Movement()
	{
		transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.fixedDeltaTime);
	}
	private void SwitchAnimation()
	{
		isMoving = transform.position != target;
		anim.SetBool("isMoving", isMoving);
		if (isMoving)
		{
			anim.SetBool("Exit", true);
			anim.SetFloat("DirX", dir.x);
			anim.SetFloat("DirY", dir.y);
		}
		else
		{
			anim.SetBool("Exit", false);
		}
	}
}
