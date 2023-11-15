using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	protected Animator Anim;

	protected void Start()
	{
		Anim.GetComponent<Animator>();
	}

	public void Death()
	{
		Destroy(gameObject);
	}

	public void JumpOn()
	{
		Anim.SetTrigger("death");
	}
}
