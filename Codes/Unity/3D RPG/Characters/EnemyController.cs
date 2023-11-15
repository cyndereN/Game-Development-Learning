using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }

[RequireComponent(typeof(NavMeshAgent))] // ȷ����ק���������ϴ˱������һ������
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private NavMeshAgent agent;
    private EnemyStates enemyStates;
    private Animator anim;
    private Collider coll;
    protected CharacterStats characterStats;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    protected GameObject attackTarget; // ������Է���
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;
    private Quaternion guardRotation;

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;
    // bool��϶���
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead;

	private void Awake()
	{
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
        //lastAttackTime = characterStats.attackData.coolDown;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        GameManager.Instance.AddObserver(this);
    }

    //�л�����ʱ����
/*    void OnEnable()
	{
        GameManager.Instance.AddObserver(this);
	}*/

    void OnDisable()
	{
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (characterStats.CurrentHealth == 0)
            isDead = true;
		if (!playerDead)
		{
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
        
    }

    void SwitchStates()
	{
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        else if (FoundPlayer())
		{
            enemyStates = EnemyStates.CHASE;
            Debug.Log("Found Player");
		}
        switch (enemyStates)
		{
            case EnemyStates.GUARD:
                isChase = false;

                if (transform.position != guardPos)
				{
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position)<= agent.stoppingDistance)
					{
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
					}
				}
                break;


            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;

                // �ж��Ƿ������Ѳ�ߵ�
                if(Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
				{
                    isWalk = false;
                    //��һ��
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    GetNewWayPoint();
				}
                else
				{
                    isWalk = true;
                    agent.destination = wayPoint;
				}

                break;


            case EnemyStates.CHASE:
                // ׷player
                
                isWalk = false;
                isChase = true;

                agent.speed = speed;
				
                if (!FoundPlayer())
				{
                    // ���ѻص��ϸ�״̬
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                    
                }
				else
				{
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
				}

                // �ڹ�����Χ���򹥻�
                if (TargetInAttackRange() || TargetInSkillRange())
				{
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
					{
                        lastAttackTime = characterStats.attackData.coolDown;

                        //�����ж�
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //ִ�й���
                        Attack();
					}
				}

                break;


            case EnemyStates.DEAD:
                coll.enabled = false;
                // agent.enabled = false; // �ر�agent
                agent.radius = 0;  // ����һ��Ч��һ�������ⱨ��
                Destroy(gameObject, 2f);
                break;

		}
	}
    
    void Attack()
	{
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
		{
            // ������������
            anim.SetTrigger("Attack");
		}
		if (TargetInSkillRange())
		{
            anim.SetTrigger("Skill");
        }
        Debug.Log("Attack");


    }

    bool FoundPlayer()
	{
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
		{
            if (target.CompareTag("Player"))
			{
                attackTarget = target.gameObject;
                return true;
			}
		}

        attackTarget = null;
        return false;
	}

    bool TargetInAttackRange()
	{
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
	}
    bool TargetInSkillRange()
	{
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }

    void SwitchAnimation()
	{
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
	}


    void GetNewWayPoint()
	{
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        // ��ֹ�ƶ���unwalkable����
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

	private void OnDrawGizmosSelected()
	{
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
	}

    void Hit()
    {
        if (attackTarget != null)
		{
            // û�ܿ�
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

	public void EndNotify()
	{
        // ��ʤ����
        // ֹͣ�ƶ�
        // ֹͣagent
        anim.SetBool("Win", true);
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
	}
}
