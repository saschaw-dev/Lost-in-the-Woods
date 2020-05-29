using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cheat : MonoBehaviour {

    PlayerSpawning playerSpawning;
    GameObject player;
    int rndNumber;
    PlayerHealth playerHealth;
    Inventory playerInventory;
    PlayerStamina playerStamina;
    public InputField command;
    public InventoryItem wood;
    public InventoryItem stone;
    public InventoryItem axe;
    public InventoryItem wall;
    public InventoryItem doorWall;
    public InventoryItem foundation;
    public InventoryItem roof;
    public InventoryItem chest;
    public InventoryItem leave;
    public InventoryItem fiber;
    int i;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        playerSpawning = player.GetComponent<PlayerSpawning>();
        playerStamina = player.GetComponent<PlayerStamina>();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerInventory = player.GetComponent<Inventory>();
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void OnCheatEnter(InputField command)
    {
        switch (command.text)
        {
            case "port":
                SpawnPlayerAtRndPos();
                break;
            case "port forest":
                SpawnPlayerAtPos(1);
                break;
            case "port hills":
                SpawnPlayerAtPos(2);
                break;
            case "port water fall":
                SpawnPlayerAtPos(3);
                break;
            case "health":
                PlayerFullHealth();
                break;
            case "die":
                PlayerDeath();
                break;
            case "death":
                PlayerDeath();
                break;
            case "wood":
                GetItems(10, wood);
                break;
            case "stone":
                GetItems(10, stone);
                break;
            case "wall":
                GetItems(10, wall);
                break;
            case "doorWall":
                GetItems(10, doorWall);
                break;
            case "foundation":
                GetItems(10, foundation);
                break;
            case "roof":
                GetItems(10, roof);
                break;
            case "fiber":
                GetItems(10, fiber);
                break;
            case "leave":
                GetItems(10, leave);
                break;
            case "chest":
                GetItems(10, chest);
                break;
            case "axe":
                GetItems(10, axe);
                break;
            case "hunger":
                HungerController.RaiseHunger(100f);
                break;
            case "thirst":
                ThirstController.RaiseThirst(100f);
                break;
            case "stamina":
                playerStamina.RaiseStamina(100f);
                break;
            default:
                break;
        }
    }

    private void SpawnPlayerAtRndPos()
    {
         playerSpawning.SpawnAtPosition(rndNumber = playerSpawning.GetRndNumber(rndNumber));
    }

    private void SpawnPlayerAtPos(int pos)
    {
        playerSpawning.SpawnAtPosition(pos);
    }

    private void PlayerFullHealth()
    {
         playerHealth.Healing(100f);
    }

    private void PlayerDeath()
    {
            playerHealth.Testdead();
    }

    private void GetItems(int stackSize, InventoryItem ip)
    {
        for (int i = 0; i < stackSize; i++)
        {
            if (playerInventory.AddItem(ip))
            {

            }
        }
    }
}
