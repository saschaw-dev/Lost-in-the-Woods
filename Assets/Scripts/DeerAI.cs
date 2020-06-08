using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Klasse regelt das Verhalten eines Rehs //

public class DeerAI : MonoBehaviour {

    Animator animator;
    Vector3 playerPos;
    GameObject playerGO;
    // Inspektorvariablen //
    public float lookDistance;
    public float runAwayDistance;
    public float lookSpeed;
    public NavMeshAgent agent;
    public float waitTimeInSeconds;
    public float runningSpeed;
    public float walkingSpeed;
    //////////////////////////////
    float distance = 0f;
    Vector3 playerDir;
    Vector3 playerLookDir;
    Quaternion lookRot;
    bool isStillWaiting = false;
    DeerHealth deerHealth;
    PlayerHealth playerHealth;
    Player player;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        deerHealth = GetComponent<DeerHealth>();
        playerGO = GameObject.FindWithTag("Player");
        player = playerGO.GetComponent<Player>();
        playerHealth = player.GetComponent<PlayerHealth>();
    }
	
	// Update is called once per frame
	void Update () {
        playerPos = playerGO.transform.position;
        distance = Vector3.Distance(transform.position, playerPos);
        if (deerHealth.deerHealth <= 0)
        {
            dying();
        }
        if (distance <= runAwayDistance)
        {
            if (!animator.GetBool("isRunning"))
            {
                runAway();
            }
            if (animator.GetBool("isRunning") && (agent.remainingDistance == 0f || agent.isStopped))
            {
                animator.SetBool("isRunning", false);
                runAway();
            }
        } else if (distance <= lookDistance)
        {
            Debug.Log("Gesehen");
            animator.SetBool("isEating", false);
            if (!animator.GetBool("isRunning"))
            {
                lookAtPlayer();
                idle();
            } else
            {
                if (agent.remainingDistance == 0f || agent.isStopped)
                {
                    runAway();
                }
            }
        }
        else
        {
            Debug.Log("Aus den Augen");
            animator.SetBool("isEating", false);
            if (animator.GetBool("isRunning"))
            {
                agent.isStopped = true;
                animator.SetBool("isWalking", false);
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

    void dying()
    {
        // Deaktiviere andere Animationen
        animator.SetBool("isEating", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        // Starte isDying Animation
        animator.SetBool("isDying", true);
        agent.isStopped = true;
    }

    void lookAtPlayer()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        // Berechne Richtungsvektor. Dieser zeigt vom Reh zum Spieler.
        playerDir = playerPos - transform.position;
        // Berechne die Rotation zur Spielerrichtung und speichere diese in ein Quaternion.
        lookRot = Quaternion.LookRotation(playerDir);
        // Erzeuge eine smoothe Bewegung von der Ursprungsrotation zur Zielrotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * lookSpeed);
    }

    void idle()
    {
        Debug.Log("Warte");
        agent.isStopped = true;
    }

    void runAway()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isEating", false);
        // Berechne Richtungsvektor. Dieser zeigt vom Spieler zum Reh.
        playerLookDir = transform.position - playerPos;
        // Berechne die Rotation zur Spielerrichtung und speichere diese in ein Quaternion.
        lookRot = Quaternion.LookRotation(playerLookDir);
        // Erzeuge eine smoothe Bewegung von der Ursprungsrotation zur Zielrotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * lookSpeed);
        NavMeshPath path = new NavMeshPath();
        Vector3 rndEscapePos = getRndPos();
        if (!agent.CalculatePath(rndEscapePos, path))
        {
            rndEscapePos = getRndPos();
        }
        agent.isStopped = false;
        Debug.Log("Renne los");
        animator.SetBool("isRunning", true);
        agent.SetDestination(rndEscapePos);
        agent.speed = runningSpeed;
    }

    void eat()
    {

    }

    protected IEnumerator Wander()
    {
        isStillWaiting = true;
        int waitTime = Random.Range(10, 20);
        yield return new WaitForSeconds(waitTime);
        isStillWaiting = false;
        NavMeshPath path = new NavMeshPath();
        Vector3 rndPos = getRndPos();
        while (!agent.CalculatePath(rndPos, path))
        {
            rndPos = getRndPos();
        }
        agent.speed = walkingSpeed;
        agent.SetDestination(rndPos);
        animator.SetBool("isWalking", true);
        agent.isStopped = false;
        agent.SetDestination(getRndPos());
        Debug.Log("Wandern");
    }

    Vector3 getRndPos()
    {
        return new Vector3(transform.position.x + Random.Range(-80f, 80f), transform.position.y, transform.position.z + Random.Range(-80f, 80f));
    }
}
