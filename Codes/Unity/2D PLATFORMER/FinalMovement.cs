using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalMovement : MonoBehaviour
{

	private Rigidbody2D rb;
	private Animator anim;
	private Collider2D coll;

	public float speed;
	public float jumpForce;

	public Transform groundCheck;
	public LayerMask ground;

	private bool isGround, isJump;
	bool jumpPressed;
	int jumpCount;


	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<Collider2D>();
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetButtonDown("Jump") && jumpCount > 0)
		{
			jumpPressed = true;
		}
	}

	private void FixedUpdate()
	{
		isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
		GroundMovement();
		Jump();
		SwitchAnim();
	}

	void GroundMovement()
	{
		float horizontalMove = Input.GetAxisRaw("Horizontal");

		rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);

		if (horizontalMove != 0)
		{
			transform.localScale = new Vector3(horizontalMove, 1, 1);
		}
	}

	void Jump() // fixupdate里调用，每0.02s执行一次
	{
		if (isGround)
		{
			jumpCount = 2; // 二段跳
			isJump = false;
		}

		if (jumpPressed && isGround)
		{
			isJump = true;
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			jumpCount--;
			jumpPressed = false; // 确保命令被执行完了然后回到上面再次判断按下按键
		}
		else if (jumpPressed && jumpCount > 0 && isJump)
		{
			// 二段跳
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			jumpCount--;
			jumpPressed = false;
		}
	}

	void SwitchAnim()
	{
		anim.SetFloat("running", Mathf.Abs(rb.velocity.x));

		if (isGround)
		{
			anim.SetBool("idle", true);
			anim.SetBool("falling", false);
		}
		else if (!isGround && rb.velocity.y > 0) // 上升过程
		{
			anim.SetBool("jumping", true);
		}
		else if (rb.velocity.y < 0) // 下降过程
		{
			anim.SetBool("jumping", false);
			anim.SetBool("falling", true);
		}
	}
}
