using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;

    private GameObject attackTarget;
    private float lastAttackTime;

    private bool isDead;
    private float stopDistance;

	private void Awake()
	{
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        stopDistance = agent.stoppingDistance;
	}
	// Start is called before the first frame update
	void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }

	private void OnEnable()
	{
        MouseManager.Instance.OnMouseClicked += MoveToTarget; // 注册action到方法
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RegisterPlayer(characterStats);
    }

    // 换到另一个场景
    private void OnDisable()
	{
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }

	// Update is called once per frame
	void Update()
    {
        isDead = characterStats.CurrentHealth == 0; //简便写法
        if (isDead)
            GameManager.Instance.NotifyObservers();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
	{
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude); // 将Vector3的值转换为浮点
        anim.SetBool("Death", isDead);
        
	}

    private void EventAttack(GameObject target)
    {
        if (isDead) return;

        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
	{
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;
        transform.LookAt(attackTarget.transform);
        
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
		{
            agent.destination = attackTarget.transform.position;
            yield return null;
		}

        agent.isStopped = true;
		// Attack

		if (lastAttackTime < 0)
		{
            anim.SetTrigger("Attack");
            anim.SetBool("Critical", characterStats.isCritical);
            // 重制冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
		}
	}

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines(); // 打断攻击动作
        if (isDead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }

    // Animation Event
    void Hit()
	{
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
		{
            if (attackTarget.CompareTag("Attackable"))
            {
                if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
				{
                    attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                    attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                    attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
                }
			}
			else
			{
                var targetStats = attackTarget.GetComponent<CharacterStats>();

                targetStats.TakeDamage(characterStats, targetStats);
            }
        }
	}
}
