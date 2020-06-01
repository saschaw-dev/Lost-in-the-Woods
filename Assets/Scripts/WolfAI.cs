using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Klasse regelt das Verhalten eines Wolfes //

public class WolfAI : MonoBehaviour {

    Animator animator;
    Vector3 playerPos;
    GameObject playerGO;
    // Inspektorvariablen/////////////////////
    public float lookDistance = 20f;
    public float attackDistance = 5f;
    public float lookSpeed = 1f;
    public NavMeshAgent agent;
    public float waitTimeInSeconds = 5f;
    /////////////////////////////////////////
    float distance = 0f;
    Vector3 playerDir;
    Quaternion lookRot;
    bool isStillWaiting = false;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        playerGO = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
        playerPos = playerGO.transform.position;
        distance = Vector3.Distance(transform.position, playerPos);
        if (distance <= attackDistance)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            agent.isStopped = true;
            attack();
        } else if (distance <= lookDistance)
        {
            animator.SetBool("isAttacking", false);
            lookAtPlayer();
            hunt();
        }
        else
        {
            animator.SetBool("isAttacking", false);
            if (animator.GetBool("isRunning")) {
                agent.isStopped = true;
                animator.SetBool("isRunning", false);
                StartCoroutine("WaitRndTime");
            }
            if (!animator.GetBool("isWalking") && !isStillWaiting)
            {
                StartCoroutine("Wander");
            }
            if (agent.isStopped || agent.remainingDistance == 0f)
            {
                animator.SetBool("isWalking", false);
            }
        }
    }

    void attack()
    {
        animator.SetBool("isAttacking", true);
    }

    void lookAtPlayer()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        // Berechne Richtungsvektor. Dieser zeigt vom Wolf zum Spieler.
        playerDir = playerPos - transform.position;
        // Berechne die Rotation zur Spielerrichtung und speichere diese in ein Quaternion.
        lookRot = Quaternion.LookRotation(playerDir);
        // Erzeuge eine smoothe Bewegung von der Ursprungsrotation zur Zielrotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * lookSpeed);
    }

    protected IEnumerator Wander()
    {
        isStillWaiting = true;
        int waitTime = Random.Range(10, 20);
        yield return new WaitForSeconds(waitTime);
        print("I waited for " + waitTime + "sec");
        isStillWaiting = false;
        NavMeshPath path = new NavMeshPath();
        Vector3 rndPos = getRndPos();
        while (!agent.CalculatePath(rndPos, path))
        {
            rndPos = getRndPos();
        }
        agent.SetDestination(rndPos);
        animator.SetBool("isWalking", true);
        agent.isStopped = false;
        agent.SetDestination(getRndPos());
    }
    
    void hunt()
    {
        agent.isStopped = false;
        animator.SetBool("isRunning", true);
        agent.SetDestination(playerPos);
    }

    protected IEnumerator WaitRndTime()
    {
        int waitTime = Random.Range(10, 20);
        yield return new WaitForSeconds(waitTime);
        print("I waited for " + waitTime + "sec");
    }

    Vector3 getRndPos()
    {
        return new Vector3(transform.position.x + Random.Range(-80f, 80f), transform.position.y, transform.position.z + Random.Range(-80f, 80f));
    }
}
