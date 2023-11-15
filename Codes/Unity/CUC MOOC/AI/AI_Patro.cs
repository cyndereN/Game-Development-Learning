using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AI_Patrol : MonoBehaviour
{
    public float patrolSpeed = 2f;
    public float patrolWaitTime = 1f;
    public Transform patrolWayPoints;
    private NavMeshAgent agent;
    private float patrolTimer;
    private int wayPointIndex;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Patrolling();
    }


    void Patrolling()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                if (wayPointIndex == patrolWayPoints.childCount - 1)
                {
                    wayPointIndex = 0;
                }
                else
                {
                    wayPointIndex++;
                }
                patrolTimer = 0;
            }          
        }
        else
        {
            patrolTimer = 0;
        }

        agent.destination = patrolWayPoints.GetChild(wayPointIndex).position;
    }
}
