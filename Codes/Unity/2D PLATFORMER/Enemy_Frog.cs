using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Frog : MonoBehaviour
{
	private Rigidbody2D rb;
	private Animator Anim;
	private Collider2D Coll;

	public LayerMask Ground;
	public Transform leftpoint, rightpoint; // 移动范围，子物体
	private float leftx, rightx;
	public float Speed, jumpForce;

	private bool Faceleft = true;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		Anim = GetComponent<Animator>();
		Coll = GetComponent<Collider2D>();

		transform.DetachChildren();
		leftx = leftpoint.position.x;
		rightx = rightpoint.position.x;
		Destroy(leftpoint.gameObject);
		Destroy(rightpoint.gameObject);
	}

	// Update is called once per frame
	void Update()
	{
		// Movement(); // 用animation event
		SwitchAnim();
	}

	void Movement()
	{
		if (Faceleft)
		{
			if (Coll.IsTouchingLayers(Ground))
			{
				Anim.SetBool("jumping", true);
				rb.velocity = new Vector2(-Speed, jumpForce);
				if (transform.position.x < leftx)  //超过左侧点掉头
				{
					transform.localScale = new Vector3(-1, 1, 1);
					Faceleft = false;
				}
			}



		}
		else
		{
			if (Coll.IsTouchingLayers(Ground))
			{
				Anim.SetBool("jumping", true);
				rb.velocity = new Vector2(Speed, jumpForce);
				if (transform.position.x > rightx)
				{
					transform.localScale = new Vector3(1, 1, 1);
					Faceleft = true;
				}
			}


		}
	}

	void SwitchAnim()
	{
		if (Anim.GetBool("jumping"))
		{
			if (rb.velocity.y < 0.1)
			{
				Anim.SetBool("jumping", false);
				Anim.SetBool("falling", true);
			}

		}
		else if (Coll.IsTouchingLayers(Ground) && Anim.GetBool("falling"))
		{
			Anim.SetBool("falling", false); // 回到idle
		}
	}

	// Enemy_Frog frog = collision.gameObject.GetComponent<Enemy_Frog>();

	void Death()
	{
		Destroy(gameObject);
	}

	public void JumpOn()
	{
		Anim.SetTrigger("death");  // Animation Event先播放动画再死亡
	}
}
