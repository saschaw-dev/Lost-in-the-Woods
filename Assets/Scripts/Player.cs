using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    GameObject equipedAxe;
    GameObject equipedBow;
    GameObject equipedMushroom;
    GameObject equipedStoneKnife;
    GameObject equipedStonePickaxe;
    GameObject equipedFiber;
    GameObject equipedStone;
    GameObject equipedWood;
    GameObject equipedLeave;
    GameObject wallPlaceholder;
    GameObject floorTilePlaceholder;
    GameObject foundationPlaceholder;
    GameObject roofPlaceholder;
    GameObject doorWallPlaceholder;
    GameObject chestPlaceholder;
    GameObject campFirePlaceholder;
    GameObject craftingPanel;
    CharacterController controller;
    PlayerStamina playerStamina;
    PlayerHealth playerHealth;
    Build buildScript;

    float moveSpeed = 5F;
    float gravity = 12F;
    float jumpPower = 6F;
    float run = 0.5F;
    private float xRotate, yRotate;
    public static bool isDying = false;
    //Dauer zwischen den Schritten
    float stepLength = 0.3F;
    //Zeit seit letztem Schritt
    float delay = 0;
    public AudioClip stepsSound;
    public AudioClip wheezeSound;
    public AudioClip jumpSound;
    //Lautstärke
    public float volume = 0.1F;
    public bool equiped = false;
    Vector3 moveDirection;
    Vector3 itemPos; // ItemPosition vor den Augen der Kamera
    Quaternion rotation;
    Camera myCamera;
    Ray ray;
    private float staminaUse = 5;
    public bool isWet = false;
    private bool isIndoor = false; // Read-only
    private bool isInFire = false; // Read-only
    public bool isInWarmingArea = false;
    public string test = "";
    Image indoorIcon;
    Color transparent;
    Color notTransparent;

    //wird zu Beginn ausgeführt, auch wenn Gameobject deaktiviert
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        craftingPanel = GameObject.Find("CraftingPanel");
    }


    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        rotation = transform.rotation;
        indoorIcon = GameObject.Find("Indoor").GetComponent<Image>();
        equipedAxe = GameObject.Find("EquipedAxe");
        equipedAxe.SetActive(equiped);
        equipedBow = GameObject.Find("EquipedBow");
        equipedBow.SetActive(equiped);
        equipedMushroom = GameObject.Find("EquipedMushroom");
        equipedMushroom.SetActive(equiped);
        equipedStoneKnife = GameObject.Find("EquipedStoneKnife");
        equipedStoneKnife.SetActive(equiped);
        equipedStonePickaxe = GameObject.Find("EquipedStonePickaxe");
        equipedStonePickaxe.SetActive(equiped);
        equipedFiber = GameObject.Find("EquipedFiber");
        equipedFiber.SetActive(false);
        equipedStone = GameObject.Find("EquipedStone");
        equipedStone.SetActive(false);
        equipedWood = GameObject.Find("EquipedWood");
        equipedWood.SetActive(false);
        equipedLeave = GameObject.Find("EquipedLeave");
        equipedLeave.SetActive(false);
        wallPlaceholder = GameObject.Find("WallPlaceholder");
        wallPlaceholder.SetActive(false);
        floorTilePlaceholder = GameObject.Find("FloorTilePlaceholder");
        floorTilePlaceholder.SetActive(false);
        foundationPlaceholder = GameObject.Find("FoundationPlaceholder");
        foundationPlaceholder.SetActive(false);
        roofPlaceholder = GameObject.Find("RoofPlaceholder");
        roofPlaceholder.SetActive(false);
        doorWallPlaceholder = GameObject.Find("DoorWallPlaceholder");
        doorWallPlaceholder.SetActive(false);
        chestPlaceholder = GameObject.Find("ChestPlaceholder");
        chestPlaceholder.SetActive(false);
        campFirePlaceholder = GameObject.Find("CampFirePlaceholder");
        campFirePlaceholder.SetActive(false);
        playerHealth = GetComponent<PlayerHealth>();
        playerStamina = GetComponent<PlayerStamina>();
        initializeStats();
        craftingPanel.SetActive(false);
        buildScript = GetComponentInChildren<Build>();
        //playerHealth.Testdead();
        transparent.a = 0.35f;
        notTransparent = new Color(1f, 1f, 1f, 1f);
        indoorIcon.color = transparent;
    }

    void initializeStats()
    {
        playerStamina.stamina = 100;
        PlayerHealth.playerHealth = PlayerHealth.playerMaxHealth;
        HungerController.RaiseHunger(100f);
        ThirstController.RaiseThirst(100f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStats();
        MovePlayer();
        LookAroundWithMouse();
        itemPos = transform.position + new Vector3(0.4F, 0, 0.4F);
        StepSounds();
        checkIfPlayerIsIndoor();
    }

    public bool getIsIndoor()
    {
        return isIndoor;
    }

    public bool getIsInFire()
    {
        return isInFire;
    }

    public void setIsInFire(bool isInFire)
    {
        this.isInFire = isInFire;
    }

    public void UpdateStats()  //Aktualisierung von Hunger, Durst, Ausdauer und Gesundheit
    {
        UpdateStamina();
        UpdateHunger();
        UpdateThirst();
        DieOfThirstOrHunger();
    
        if (PlayerHealth.playerHealth == 0 && isDying == false)
        {
            isDying = true;
            playerHealth.Dying();

        }
    }

    public void UpdateStamina()
    {
        playerStamina.RaiseStamina(run * Time.deltaTime);  //dann regeneriere Ausdauer durch Aufruf der Methode RaiseStamina
    }

    public void UpdateHunger()
    {    
        HungerController.LowerHunger(run * Time.deltaTime);
    }

    public void UpdateThirst()
    {    
        ThirstController.LowerThirst(run * Time.deltaTime);
    }

    public void DieOfThirstOrHunger()
    {
        //Schaden durch Verhungern oder Verdursten
        if (HungerController.getHunger() == 0 || ThirstController.getThirst() == 0)
        {
            if (PlayerHealth.playerHealth >= Time.deltaTime)
            {
                playerHealth.Damaging(2 * Time.deltaTime);
            }
            else
            {
                PlayerHealth.playerHealth = 0;
                playerStamina.stamina = 0;
                isDying = true;
                playerHealth.Dying();
            }
        }
    }

    InventoryItem prev = null;
    GameObject prevGameObject = null;
    GameObject currentGameObject = null;
    private RaycastHit hit;

    public void Equip(InventoryItem ip) //Objekt wird vor der Kamera aktiviert
    {
        if (ip.itemName == "bow") currentGameObject = equipedBow;
        if (ip.itemName == "Mushroom") currentGameObject = equipedMushroom;
        if (ip.itemName == "axe") currentGameObject = equipedAxe;
        if (ip.itemName == "StoneKnife") currentGameObject = equipedStoneKnife;
        if (ip.itemName == "stonePickaxe") currentGameObject = equipedStonePickaxe;
        if (ip.itemName == "Stone") currentGameObject = equipedStone;
        if (ip.itemName == "wood") currentGameObject = equipedWood;
        if (ip.itemName == "fiber") currentGameObject = equipedFiber;
        if (ip.itemName == "leave") currentGameObject = equipedLeave;
        if (ip.itemName == "wall") currentGameObject = wallPlaceholder;
        if (ip.itemName == "floorTile") currentGameObject = floorTilePlaceholder;
        if (ip.itemName == "foundation") currentGameObject = foundationPlaceholder;
        if (ip.itemName == "roof") currentGameObject = roofPlaceholder;
        if (ip.itemName == "doorWall") currentGameObject = doorWallPlaceholder;
        if (ip.itemName == "chest") currentGameObject = chestPlaceholder;
        if (ip.itemName == "campFire") currentGameObject = campFirePlaceholder;

        if (prevGameObject != currentGameObject && prevGameObject != null)
        {
            if(prevGameObject.CompareTag("buildItem"))
            {
                buildScript.CancelBlockPlacement();
            }
            prevGameObject.SetActive(false);
            InventoryItem.lastEquiped = true;
        }
        else
        {
            InventoryItem.lastEquiped = !InventoryItem.lastEquiped ;
        }

        prevGameObject = currentGameObject;
        ip.equiped = !ip.equiped;
        if (currentGameObject != null)
            currentGameObject.SetActive(InventoryItem.lastEquiped);     
        }

    private void StepSounds()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            stepLength = 0.35F;
        }
        else
        {
            stepLength = 0.5F;
        }

        if (controller.velocity.sqrMagnitude > 0.2F && controller.isGrounded)
        {
            if (delay >= stepLength)
            {
                AudioSource.PlayClipAtPoint(stepsSound, transform.position, volume);
                delay = 0;
            }
        }
        delay += Time.deltaTime;
    }

    private void LookAroundWithMouse()
    {
        if (Cursor.visible == false) //Umschauen mit der Maus nicht möglich, wenn beliebiges Menü (Inventar, Crafting..) geöffnet ist
        {
            xRotate += Input.GetAxis("Mouse X");
            yRotate = Mathf.Min(50, Mathf.Max(-50, yRotate + Input.GetAxis("Mouse Y")));
            gameObject.transform.localRotation = Quaternion.Euler(yRotate, xRotate, 0);
        }
    }

    private void MovePlayer()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift) && playerStamina.stamina > run / 10) run = 3F;
            else
            {
                run = 1F;
            }
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection *= moveSpeed * run;
            moveDirection = transform.TransformDirection(moveDirection);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpPower;
                controller.Move(moveDirection * Time.deltaTime);
                playerStamina.LowerStamina(jumpPower * 20f * Time.deltaTime); // Springen verbraucht auch Ausdauer
                AudioSource.PlayClipAtPoint(jumpSound, gameObject.transform.position);
            }

        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerStamina.LowerStamina(run * staminaUse*Time.deltaTime); //Beim Rennen verbraucht der Spieler Ausdauer
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) // Spieler hat aufgehört zu rennen
        {
            AudioSource.PlayClipAtPoint(wheezeSound, gameObject.transform.position);
        }
        if (!Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))) 
        {
            playerStamina.LowerStamina(run * Time.deltaTime); // Beim Gehen verbraucht Spieler soviel Ausdauer, wie er regeneriert
        }
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void checkIfPlayerIsIndoor()
    {
        if (Physics.Raycast(gameObject.transform.position, Vector3.up, out hit, 10f))
        {
            if (hit.collider.gameObject.CompareTag("roof"))
            {
                // Dach getroffen?
                isIndoor = true;
                indoorIcon.color = notTransparent;
                return;
            }
        }
        isIndoor = false;
        indoorIcon.color = transparent;
    }
}

