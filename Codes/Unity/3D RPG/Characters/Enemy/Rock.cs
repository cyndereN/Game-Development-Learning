using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer, HitEnemy, HitNothing}
    private Rigidbody rb;
    public RockStates rockStates;

    [Header("Basic Settings")]
    public float force;
    public int damage;
    public GameObject target;
    private Vector3 direction;

	void Start()
	{
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
	}

	void FixedUpdate()
	{
		if(rb.velocity.sqrMagnitude < 1f)
		{
            rockStates = RockStates.HitNothing;
		}
	}

	public void FlyToTarget()
	{
        if (target == null)
            target = FindObjectOfType<PlayerController>().gameObject;
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
	}

	void OnCollisionEnter(Collision other)
	{
		switch (rockStates)
		{
            case RockStates.HitPlayer:
				if (other.gameObject.CompareTag("Player"))
				{
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction*force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());

                    rockStates = RockStates.HitNothing;
				}
                break;

            case RockStates.HitEnemy:
				if (other.gameObject.GetComponent<Golem>())   // 也可以改成enemy tag的，个人喜好
				{
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);

                    Destroy(gameObject);
				}
                break;
		}
	}
}
