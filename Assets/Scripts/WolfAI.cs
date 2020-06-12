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
    public float lookDistance;
    public float attackDistance;
    public float lookSpeed;
    public NavMeshAgent agent;
    public float waitTimeInSeconds;
    public float attackPoints;
    public float runningSpeed;
    public float walkingSpeed;
    public AudioClip howl;
    /////////////////////////////////////////
    float distance = 0f;
    Vector3 playerDir;
    Quaternion lookRot;
    bool isStillWaiting = false;
    WolfHealth wolfHealth;
    PlayerHealth playerHealth;
    Player player;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        playerGO = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position);
        wolfHealth = GetComponent<WolfHealth>();
        player = playerGO.GetComponent<Player>();
        playerHealth = player.GetComponent<PlayerHealth>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!animator.GetBool("isDying"))
        {
            playerPos = playerGO.transform.position;
            distance = Vector3.Distance(transform.position, playerPos);
            if (wolfHealth.wolfHealth <= 0)
            {
                dying();
                return;
            }
            if (distance <= attackDistance)
            {
                agent.isStopped = true;
                lookAtPlayer();
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
    }

    void attack()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", true);
        playerHealth.Damaging(attackPoints * Time.deltaTime);
    }

    void lookAtPlayer()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        // Berechne Richtungsvektor. Dieser zeigt vom Wolf zum Spieler.
        playerDir = playerPos - transform.position;
        // Berechne die Rotation zur Spielerrichtung und speichere diese in ein Quaternion.
        lookRot = Quaternion.LookRotation(playerDir);
        // Erzeuge eine smoothe Bewegung von der Ursprungsrotation zur Zielrotation.
        lookRot = Quaternion.Euler(new Vector3(0f, lookRot.eulerAngles.y, 0f));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * lookSpeed);
    }

    protected IEnumerator Wander()
    {
        isStillWaiting = true;
        AudioSource.PlayClipAtPoint(howl, transform.position, 1.0f);
        int waitTime = Random.Range(10, 20);
        yield return new WaitForSeconds(waitTime);
        NavMeshPath path = new NavMeshPath();
        Vector3 rndPos = getRndPos();
        while (!agent.CalculatePath(rndPos, path))
        {
            rndPos = getRndPos();
        }
        isStillWaiting = false;
        agent.speed = walkingSpeed;
        agent.SetDestination(rndPos);
        animator.SetBool("isWalking", true);
        agent.isStopped = false;
        agent.SetDestination(getRndPos());
    }
    
    void hunt()
    {
        agent.isStopped = false;
        animator.SetBool("isRunning", true);
        agent.speed = runningSpeed;
        agent.SetDestination(playerPos);
    }

    void dying()
    {
        // Deaktiviere andere Animationen
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        // Starte isDying Animation
        animator.SetBool("isDying", true);
        agent.isStopped = true;
    }

    public void getWeaponDamage(InventoryItem weapon)
    {
        wolfHealth.Damaging(weapon.damage); // wird nicht pro Frame, sondern pro Mausklick ausgeführt => Kein DeltaTime!
    }

    protected IEnumerator WaitRndTime()
    {
        int waitTime = Random.Range(10, 20);
        yield return new WaitForSeconds(waitTime);
    }

    Vector3 getRndPos()
    {
        return new Vector3(transform.position.x + Random.Range(-80f, 80f), transform.position.y, transform.position.z + Random.Range(-80f, 80f));
    }
}
