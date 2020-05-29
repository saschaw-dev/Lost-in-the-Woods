using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shortkeys : MonoBehaviour {

    public InventoryItem item1;
    public InventoryItem item2;
    public InventoryItem item3;
    public InventoryItem item4;
    public InventoryItem item5;
    public InventoryItem item6;
    public InventoryItem item7;
    public InventoryItem item8;
    public InventoryItem item9;
    public InventoryItem item10;
    Inventory playerInventory;
    GameObject commandWindow;
    int rndNumber;
    public static bool isCommandWindowVisible = false;

    // Use this for initialization
    void Start() {
        playerInventory = gameObject.GetComponent<Inventory>();
        commandWindow = GameObject.Find("CommandWindow");
        commandWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        cheatItems();
        //SpawnPlayerAtRndPos();
        //Testheal();
    }

    private void cheatItems() {
        if (Input.GetKeyDown(KeyCode.B)) {
            if (playerInventory.AddItem(item1))
            {

            }
            if (playerInventory.AddItem(item2))
            {

            }
            if (playerInventory.AddItem(item3))
            {

            }
            if (playerInventory.AddItem(item4))
            {

            }
            if (playerInventory.AddItem(item6))
            {

            }
            if (playerInventory.AddItem(item7))
            {

            }
            if (playerInventory.AddItem(item8))
            {

            }
            if (playerInventory.AddItem(item9))
            {

            }
            if (playerInventory.AddItem(item10))
            {

            }
        }   
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
        {
            ToggleCommandWindow();
        }
    }

    private void ToggleCommandWindow()
    {
        if (isCommandWindowVisible)
        {
            isCommandWindowVisible = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            PauseGame.toggleTimeScale();
            commandWindow.SetActive(false);
        }
        else
        {
            if (!Craft.isCraftingTableOpen && !playerInventory.isInventoryOpen)
            {
                isCommandWindowVisible = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                PauseGame.toggleTimeScale();
                commandWindow.SetActive(true);
            }
        }
    }
}
