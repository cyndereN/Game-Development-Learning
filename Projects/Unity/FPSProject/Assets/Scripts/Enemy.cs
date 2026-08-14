using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//*****************************************
//创建人： Trigger 
//功能说明：
//***************************************** 
public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int HP;
    public PlayerController pc;
    public NavMeshAgent agent;
    public float attackCD;
    private float attackTimer;
    public int attackValue;
    public AudioSource audioSource;
    public AudioClip attackAudio;
    private bool isDead;
    public AudioClip dieAudio;
    private bool hasTarget;

    void Start()
    {

    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        if (Vector3.Distance(transform.position, pc.transform.position) >=5)
        {
            //5米外，敌人不做任何事情
            return;
        }
        else
        {
            //5米内看到玩家，获得目标
            hasTarget = true;
        }
        if (hasTarget)//有目标后
        {
            if (Vector3.Distance(transform.position, pc.transform.position) <= 1)
            {
                //攻击
                Attack();
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                agent.isStopped = false;
                agent.SetDestination(pc.transform.position);
                animator.SetFloat("MoveState", 1);
            }
        }
       
    }

    /// <summary>
    /// 受到攻击
    /// </summary>
    public void TakeDamage(int attackValue)
    {
        animator.SetTrigger("Hit");
        HP -= attackValue;
        if (HP<=0)
        {
            animator.SetBool("Die", true);
            agent.isStopped = true;
            isDead = true;
            audioSource.Stop();
            audioSource.PlayOneShot(dieAudio);
        }
    }
    /// <summary>
    /// 敌人攻击
    /// </summary>
    private void Attack()
    {
        agent.isStopped = true;
        animator.SetFloat("MoveState", 0);
        if (Time.time - attackTimer >= attackCD)
        {
            animator.SetTrigger("Attack");
            attackTimer = Time.time;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            Invoke("DelayPlayAttackSound",1);
        }
    }
    /// <summary>
    /// 延时播放攻击音效
    /// </summary>
    private void DelayPlayAttackSound()
    {
        pc.TakeDamage(attackValue);
        audioSource.PlayOneShot(attackAudio);
    }
}
