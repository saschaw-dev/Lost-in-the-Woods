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
    public AudioClip growl;
    public float blockDurability;
    public AudioClip dropSound;
    public InventoryItem droppableHide;
    public float beforeDestroyTime;
    public AudioClip whining;
    /////////////////////////////////////////
    float distance = 0f;
    Vector3 playerDir;
    Quaternion lookRot;
    bool isStillWaiting = false;
    WolfHealth wolfHealth;
    PlayerHealth playerHealth;
    Player player;
    bool lastStateWasNotAttackOrLook = true;
    bool isDropped = false;

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
                lastStateWasNotAttackOrLook = true;
                animator.SetBool("isAttacking", false);
                if (animator.GetBool("isRunning")) {
                    agent.isStopped = true;
                    animator.SetBool("isRunning", false);
                    //StartCoroutine("WaitRndTime");
                }
                if (!animator.GetBool("isWalking") && !isStillWaiting)
                {
                    Debug.Log("Starte Wandern");
                    StartCoroutine("Wander");
                }
                if ((agent.isStopped || agent.remainingDistance == 0f) && !isStillWaiting)
                {
                    Debug.Log("Stoppe Laufanimation");
                    animator.SetBool("isWalking", false);
                }
            }
        }
        else
        {
            // Verhalten bei Tod
            if (blockDurability == 0 && !isDropped)
            {
                dropItems();
            }
        }
    }

    void attack()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", true);
        if (lastStateWasNotAttackOrLook)
        {
            AudioSource.PlayClipAtPoint(growl, transform.position, 1f);
            lastStateWasNotAttackOrLook = false;
        } 
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
        agent.isStopped = true;
        AudioSource.PlayClipAtPoint(howl, transform.position, 1.0f);
        int waitTime = Random.Range(10, 20);
        Debug.Log("Warte");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Warten fertig");
        Debug.Log("Suche Pfad");
        NavMeshPath path = new NavMeshPath();
        Vector3 rndPos = getRndPos();
        while (!agent.CalculatePath(rndPos, path))
        {
            rndPos = getRndPos();
        }
        Debug.Log("Pfad gefunden");
        agent.speed = walkingSpeed;
        agent.SetDestination(rndPos);
        Debug.Log("Wandern");
        animator.SetBool("isWalking", true);
        agent.isStopped = false;
        agent.SetDestination(getRndPos());
        isStillWaiting = false;
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

    void dropItems()
    {
        GameObject.Instantiate(droppableHide.prefab, transform.position, droppableHide.prefab.transform.localRotation);
        GameObject.Instantiate(droppableHide.prefab, transform.position + new Vector3(1, 0, 0), droppableHide.prefab.transform.localRotation);
        AudioSource.PlayClipAtPoint(dropSound, transform.position);
        isDropped = true;
        StartCoroutine("DestroyYourselfAfterTime");
    }

    protected IEnumerator DestroyYourselfAfterTime()
    {
        yield return new WaitForSeconds(beforeDestroyTime);
        GameObject.Destroy(gameObject);
    }

    public void getWeaponDamage(InventoryItem weapon)
    {
        wolfHealth.Damaging(weapon.damage); // wird nicht pro Frame, sondern pro Mausklick ausgeführt => Kein DeltaTime!
        AudioSource.PlayClipAtPoint(whining, transform.position, 1.0f);
    }

    public void getBlockDamage(InventoryItem stoneKnife)
    {
        blockDurability -= stoneKnife.blockDamage;
        if (blockDurability < 0)
        {
            blockDurability = 0;
        }
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
