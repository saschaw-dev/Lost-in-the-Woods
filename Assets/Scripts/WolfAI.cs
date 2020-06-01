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
    /////////////////////////////////////////
    float distance = 0f;
    Vector3 playerDir;
    Quaternion lookRot;

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
            animator.SetBool("isRunning", false);
            agent.isStopped = true;
            wander();
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

    void wander()
    {
        //agent.isStopped = false;
    }
    
    void hunt()
    {
        agent.isStopped = false;
        animator.SetBool("isRunning", true);
        agent.SetDestination(playerPos);
    }
}
