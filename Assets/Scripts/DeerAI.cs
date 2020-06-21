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
    public float maxBushDistance;
    public float blockDurability;
    public AudioClip dropSound;
    public InventoryItem droppableMeat;
    public float beforeDestroyTime;
    public AudioClip hitSound;
    //////////////////////////////
    float distance = 0f;
    Vector3 playerDir;
    Vector3 playerLookDir;
    Quaternion lookRot;
    bool isStillWaiting = false;
    DeerHealth deerHealth;
    PlayerHealth playerHealth;
    Player player;
    GameObject[] bushes;
    Vector3[] bushPositions;
    bool isLookingForFood = false;
    bool isDropped = false;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        deerHealth = GetComponent<DeerHealth>();
        playerGO = GameObject.FindWithTag("Player");
        player = playerGO.GetComponent<Player>();
        playerHealth = player.GetComponent<PlayerHealth>();
        bushes = GameObject.FindGameObjectsWithTag("bush");
        storeBushPositions();
    }
	
	// Update is called once per frame
	void Update () {
        if (!animator.GetBool("isDying"))
        {
            playerPos = playerGO.transform.position;
            distance = Vector3.Distance(transform.position, playerPos);
            if (deerHealth.deerHealth <= 0)
            {
                isLookingForFood = false;
                dying();
                return;
            }
            if (distance <= runAwayDistance)
            {
                isLookingForFood = false;
                if (!animator.GetBool("isRunning"))
                {
                    runAway();
                }
                if (animator.GetBool("isRunning") && (agent.remainingDistance == 0f || agent.isStopped))
                {
                    animator.SetBool("isRunning", false);
                    runAway();
                }
            }
            else if (distance <= lookDistance)
            {
                isLookingForFood = false;
                animator.SetBool("isEating", false);
                if (!animator.GetBool("isRunning"))
                {
                    lookAtPlayer();
                    idle();
                }
                else
                {
                    if (agent.remainingDistance == 0f || agent.isStopped)
                    {
                        runAway();
                    }
                }
            }
            else
            {
                //Debug.Log("Aus den Augen");
                if (animator.GetBool("isRunning"))
                {
                    agent.isStopped = true;
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isRunning", false);
                    StartCoroutine("WaitRndTime");
                }
                if (!animator.GetBool("isWalking") && !isStillWaiting && !isLookingForFood)
                {
                    StartCoroutine("Wander");
                }
                if (agent.isStopped || agent.remainingDistance == 0f)
                {
                    animator.SetBool("isWalking", false);
                    if (isLookingForFood)
                    {
                        eat();
                    }
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

    void storeBushPositions()
    {
        bushPositions = new Vector3[bushes.Length];

        for (int i = 0; i < bushPositions.Length; i++)
        {
            bushPositions[i] = bushes[i].transform.position;
        }
    }

    void dropItems()
    {
        GameObject.Instantiate(droppableMeat.prefab, transform.position, droppableMeat.prefab.transform.localRotation);
        GameObject.Instantiate(droppableMeat.prefab, transform.position + new Vector3(1,0,0), droppableMeat.prefab.transform.localRotation);
        AudioSource.PlayClipAtPoint(dropSound, transform.position);
        isDropped = true;
        StartCoroutine("DestroyYourselfAfterTime");
    }

    Vector3 getNearestBushPos()
    {
        float distanceToBush;
        Vector3 nearestBush = bushPositions[0];
        for (int i = 1; i < bushPositions.Length; i++)
        {
            distanceToBush = Vector3.Distance(transform.position, bushPositions[i]);
            if (distanceToBush < Vector3.Distance(transform.position, nearestBush))
            {
                nearestBush = bushPositions[i];
            }
        }
        return nearestBush;
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
        animator.SetBool("isRunning", true);
        agent.SetDestination(rndEscapePos);
        agent.speed = runningSpeed;
    }

    void eat()
    {
        animator.SetBool("isEating", true);
    }

    public void getWeaponDamage(InventoryItem weapon)
    {
        deerHealth.Damaging(weapon.damage); // wird nicht pro Frame, sondern pro Mausklick ausgeführt => Kein DeltaTime!
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
    }

    public void getBlockDamage(InventoryItem stoneKnife)
    {
        blockDurability -= stoneKnife.blockDamage;
        if (blockDurability < 0)
        {
            blockDurability = 0;
        }
    }

    protected IEnumerator DestroyYourselfAfterTime()
    {
        yield return new WaitForSeconds(beforeDestroyTime);
        GameObject.Destroy(gameObject);
    }

    protected IEnumerator Wander()
    {
        /* Warten */
        isStillWaiting = true;
        int waitTime = Random.Range(10, 20);
        yield return new WaitForSeconds(waitTime);
        ////////////////////////////////////////////////
        /* Wandern */
        /* Etwas zu essen in der Nähe? */
        Vector3 nearestBushPos = getNearestBushPos();
        if (Vector3.Distance(transform.position, nearestBushPos) <= maxBushDistance)
        {  
            isStillWaiting = false;
            agent.speed = walkingSpeed;
            animator.SetBool("isWalking", true);
            agent.isStopped = false;
            agent.SetDestination(nearestBushPos);
            isLookingForFood = true;
            isStillWaiting = false;
        } else
        {
            NavMeshPath path = new NavMeshPath();
            Vector3 rndPos = getRndPos();
            while (!agent.CalculatePath(rndPos, path))
            {
                rndPos = getRndPos();
            }
            isStillWaiting = false;
            agent.speed = walkingSpeed;
            animator.SetBool("isWalking", true);
            agent.isStopped = false;
            agent.SetDestination(rndPos);
        }
        /////////////////////////////////////////////////
    }

    Vector3 getRndPos()
    {
        return new Vector3(transform.position.x + Random.Range(-80f, 80f), transform.position.y, transform.position.z + Random.Range(-80f, 80f));
    }
}
