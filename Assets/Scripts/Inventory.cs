using System;
using System.Collections.Generic; //Für das generische Objekt Dictionary
using UnityEngine;
using UnityEngine.UI; //Für UI-Elemente, wie z.B. Image etc.

[Serializable]
public class Inventory : MonoBehaviour
{
    public GameObject[] itemImageSlots = new GameObject[10]; // itemImageSlots sind nicht die Kacheln selbst, sondern das weiße GO darüber!
    public List<InventoryTile> slots = new List<InventoryTile>();
    private GameObject player;
    GameObject inventoryPanel;
    GameObject chestPanel;
    GameObject dropPos;
    public Image handImage;
    PlayerHealth playerHealth;
    public Image currentItemImage;
    Player playerScript;
    public GameObject[] itemKeys = new GameObject[10]; // Ordnet jedem ItemImage das InventoryItem zu
    Build buildScript;
    GameObject itemLabel;
    Text itemFrameText;
    GameObject tip;
    GameObject itemImageFollower;
    Transform crosshairPos;
    Image crosshairImage;
    Image handCursor;
    GameObject crosshair;
    GameObject hand;
    Camera myCamera;
    GameObject mainCamera;
    public bool isInventoryOpen = false;

    private void Awake()
    {
        tip = GameObject.Find("Tips");
        player = GameObject.Find("Player");
        dropPos = GameObject.Find("DropPos");
        itemLabel = GameObject.Find("ItemLabel");
        itemImageFollower = GameObject.Find("Follower");
        crosshair = GameObject.Find("Crosshair");
        mainCamera = GameObject.Find("Main Camera");
        if (gameObject == player)
        {
            inventoryPanel = GameObject.Find("InventoryPanel");
        } else
        {
            if (chestPanel == null) chestPanel = GameObject.Find("ChestPanel");
        }
        hand = GameObject.Find("Hand");
        crosshairPos = crosshair.GetComponent<Transform>();
        if (itemImageFollower != null)
        {
            itemImageFollower.GetComponent<Image>().enabled = false;
        }
        myCamera = mainCamera.GetComponent<Camera>();
        handCursor = hand.GetComponent<Image>();
        crosshairImage = crosshair.GetComponent<Image>();
    }

    // Use this for initialization
    void Start()
    {
        InitializeItems();
        UpdateView();
        if (handImage != null)
        {
            handImage.transform.GetChild(1).GetComponent<Text>().enabled = false;
        }
        playerScript = player.GetComponent<Player>();
        buildScript = player.GetComponentInChildren<Build>();
        playerHealth = gameObject.GetComponent<PlayerHealth>();
        if (itemLabel != null)
        {
            itemFrameText = itemLabel.GetComponent<Text>();
            itemFrameText.enabled = false;
        }

        for (int i = 0; i < itemKeys.Length; i++)
        {
            itemKeys[i].GetComponent<Text>().enabled = false;
        }

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        if (chestPanel != null)
        {
            chestPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameObject == player) // nur für Spielerinventar!
        {
            //Mit Taste I das Inventar ein und ausblenden und damit auch den Mauszeiger
            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenOrCloseInventory();              
            }
        }
    }

    private void InitializeItems()
    {
        // Create an inventory with 9 empty tiles
        for (int j = 0; j < itemImageSlots.Length - 1; j++)
        {
            this.slots.Add(new InventoryTile(j));
        }
    }

    public void setChestPanel(GameObject chestPanel)
    {
        if (chestPanel == null)
        {
            return;
        }
        this.chestPanel = chestPanel;
    }

    public void OpenOrCloseInventory()
    {
        if (gameObject == player)
        {
            if (inventoryPanel.activeSelf == true)
            {
                PauseGame.toggleTimeScale();
                inventoryPanel.SetActive(false);
                isInventoryOpen = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                if (!Craft.isCraftingTableOpen && !Shortkeys.isCommandWindowVisible)
                {
                    PauseGame.toggleTimeScale();
                    inventoryPanel.SetActive(true);
                    isInventoryOpen = true;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        } else
        {
            if (chestPanel == null)
            {
                buildScript.instantiateChestUI();
            }
            if (chestPanel.activeSelf == true)
            {
                chestPanel.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                chestPanel.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }       
    }

    void UpdateView() //Methode die die Items im Inventar darstellt, falls welche vorhanden sind und ansonsten leere Felder darstellt
    {
        for (int j = 0; j < itemImageSlots.Length - 1; j++)
        {
            if (itemImageSlots[j] != null)
            {
                currentItemImage = itemImageSlots[j].GetComponent<Image>();
                currentItemImage.enabled = false;
                currentItemImage.sprite = null;
                currentItemImage.GetComponentInChildren<Text>().text = "";
                // Set item image pos
                itemImageSlots[j].GetComponentsInChildren<Text>()[1].text = j.ToString();
                itemImageSlots[j].SetActive(false);
            }
        }

        int i = 0;

        foreach (InventoryTile slot in this.slots)
        {
            //falls Item gedroppt wurde und Anzahl jetzt Null ist, dann blende Item-Auswahlrahmen und Bild aus
            if (slot.getInventoryItem() != null && slot.getNumberOfItems() < 1) 
            {
                if (handImage != null)
                {
                    handImage.sprite = null;
                }
            }
            else
            {
                itemImageSlots[i].SetActive(true);
                currentItemImage = itemImageSlots[i].GetComponent<Image>();
                currentItemImage.enabled = true;
                if (slot.getInventoryItem() != null)
                {
                    currentItemImage.sprite = slot.getInventoryItem().sprite;
                    itemKeys[i].GetComponent<Text>().text = slot.getInventoryItem().itemKindNumber.ToString();
                }
                // Set item image pos
                //if (gameObject != GameObject.Find("Chest"))
                //{
                //    // If condition can be removed when the chest panel is fixed
                //}
                itemImageSlots[i].GetComponentsInChildren<Text>()[2].text = slot.getTilePos().ToString();
                currentItemImage.GetComponentInChildren<Text>().text = slot.getNumberOfItems().ToString();
                i++;
            }
        }
    }

    // falls Wahr: Jeder Slot ist mit maximaler Stapelgröße besetzt
    private bool IsCompleteFull()
    {
        foreach(InventoryTile slot in slots)
        {
            if (!slot.isInventorySlotFull())
            {
                return false;
            }
        }
        return true;
    }

    // Später noch ersetzen mit inventory tile logik
    public bool IsInventoryFull(InventoryItem itemToBeAdded)
    {
        if(IsCompleteFull())
        {
            return true;
        }
        else
        {  
            for (int i = 0; i < itemImageSlots.Length - 1; i++)
            {
                currentItemImage = itemImageSlots[i].GetComponent<Image>();
                if (currentItemImage.GetComponentInChildren<Text>().text != "10")
                {
                    if(currentItemImage.sprite == itemToBeAdded.sprite || currentItemImage.sprite == null)
                    {
                        return false;
                    }
                }  
                if(currentItemImage.enabled == false || currentItemImage.sprite == null)
                {
                    return false;
                }
            }
            return true;
        }   
    }

    public bool AddItem(InventoryItem ip) //Rückgabetyp bool um zu prüfen ob Item hinzugefügt werden konnte
    {
        if(IsInventoryFull(ip))
        {
            return false;
        }
        // Falls noch
        if (!addingToExistingTilePossible(ip))
        {

            foreach (InventoryTile slot in slots)
            {
                if (slot.getIsEmptyTile()) {
                    if (slot.addOneItem(ip))
                    {
                        UpdateView();
                        return true;
                    }
                }
            }
            return false;
        }
        else
        {
            foreach (InventoryTile slot in slots)
            {
                if (slot.getInventoryItem() != null && slot.getInventoryItem().itemKindNumber == ip.itemKindNumber && !slot.isInventorySlotFull())
                {
                    slot.addOneItem(ip);
                    UpdateView();
                    return true;
                }
            }
            return false;
        }
    }

    private bool RemoveItem(InventoryItem ip)
    {
        InventoryTile lastTile = null;
        InventoryTile lastFullTile = null;
        // Find the last not full inventory tile with the item 'ip'
        // If there are only full inventory tiles with this 'ip' remove from the last full tile
        foreach (InventoryTile slot in slots)
        {
            if (slot.getInventoryItem() != null && slot.getInventoryItem().itemKindNumber == ip.itemKindNumber)
            {
                if (slot.isInventorySlotFull())
                {
                    lastFullTile = slot;
                }
                else
                {
                    lastTile = slot;
                }
            }
        }

        bool isRemovedSuccessfully = false;
        if (lastTile != null)
        {
            isRemovedSuccessfully = slots.Find(slot => slot == lastTile).removeOneItem(ip);
        }
        else if (lastFullTile != null)
        {
            isRemovedSuccessfully = slots.Find(slot => slot == lastFullTile).removeOneItem(ip);
        }
        else
        {
            // Item does not exist in inventory!
            return false;
        }
        if (isRemovedSuccessfully)
        {
            UpdateView();
        }
        return isRemovedSuccessfully;
    }

    public bool DropFromInventory(InventoryItem ip) //Droppt ein Item
    {
        if (this.RemoveItem(ip))
        {
            Instantiate(ip.prefab, dropPos.transform.position, Quaternion.identity);
            return true;
        }
        return false;
    }

    private bool addingToExistingTilePossible(InventoryItem ip)
    {
        foreach(InventoryTile slot in slots)
        {
            if (slot.getInventoryItem() != null && (slot.getInventoryItem().itemKindNumber == ip.itemKindNumber && !slot.isInventorySlotFull()))
            {
                return true;
            }
        }
        return false;
    }

    public bool UseItem(InventoryItem ip) //Verbraucht ein Item (nicht Droppen!)
    {
        // verhindert Fehler nach Entfernen des letzten Items aus der Truhe
        disableItemFollower();

        return this.RemoveItem(ip);
    }

    public void removeFollowerItemFromInventorySlots(GameObject followerItemImage)
    {
        List<Text> childrenTexts2 = new List<Text>(followerItemImage.GetComponentsInChildren<Text>());
        Text followerItemKey = childrenTexts2[1];
        Text followerItemImagePos = childrenTexts2[2];

        InventoryTile slotToBeEmptied = this.slots.Find(slot => findItem(slot, followerItemKey, followerItemImagePos));
        slotToBeEmptied.emptyStack();

        UpdateView();
    }

    private void disableItemFollower()
    {
        if (itemImageFollower != null)
        {
            itemImageFollower.SetActive(false);
            itemImageFollower.GetComponent<Image>().enabled = false;
            itemImageFollower.transform.position = Vector3.zero;
        }
    }

    public void AddItemToYourHand(InventoryItem ip)
    {
        playerScript.Equip(ip);

        if (InventoryItem.lastEquiped == true) // diesmal auf anderes Item geklickt?
        {
            handImage.sprite = ip.sprite; //Gewähltes ItemImage in Hand legen
            itemFrameText.text = ip.itemLabel;
            itemFrameText.enabled = true;
            if (ip.isBuildItem)
            {
                buildScript.startBuildMode(ip);
            }
        }
        else // auf das selbe item nochmal geklickt?
        {
            handImage.sprite = null;
            itemFrameText.enabled = false;
            if (ip.isBuildItem)
            {
                buildScript.cancelBuildMode();
            }
        }
    }

    public bool isHandEmpty()
    {
        return !InventoryItem.lastEquiped;
    }

    public bool disableContainsHandItemIfHandItem(GameObject followerItemImage)
    {
        InventoryTile followerItemSlot = getInventoryTileByItemImageGO(followerItemImage);
        bool isHandItem = false; 

        if(isHandItemSlot(followerItemSlot))
        {
            disableContainsHandItemForSlot(followerItemSlot);
            isHandItem = true;
        }
        return isHandItem;
    }

    public void disableContainsHandItemInInventory()
    {
        disableContainsHandItemForSlot(getInventoryTileThatContainsHandItem());
    }

    private void disableContainsHandItemForSlot(InventoryTile itemSlot)
    {
        foreach (InventoryTile slot in slots)
        {
            if (slot == itemSlot)
            {
                slot.setContainsHandItem(false);
            }
        }
    }

    public void updateContainsHandItemForHandItemSlot(GameObject followerItemImage, GameObject handItemSlot) {
        disableContainsHandItemForSlot(getInventoryTileThatContainsHandItem());
        enableContainsHandItemInInventory(followerItemImage);
    }

    public void enableContainsHandItemInInventory(GameObject followerItemImage)
    {
        enableContainsHandItemForSlot(getInventoryTileByItemImageGO(followerItemImage));
    }

    private void enableContainsHandItemForSlot(InventoryTile followerItemSlot)
    {
        foreach (InventoryTile slot in slots)
        {
            if (slot == followerItemSlot)
            {
                slot.setContainsHandItem(true);
            }
        }
    }

    private InventoryTile getInventoryTileThatContainsHandItem()
    {
        foreach (InventoryTile slot in slots)
        {
            if (slot.getContainsHandItem())
            {
                return slot;
            }
        }
        return null;
    }

    private bool isHandItemSlot(InventoryTile followerItemSlot)
    {
        bool isHandItemSlot = false;
        foreach (InventoryTile slot in slots)
        {
            if (slot == followerItemSlot)
            {
                if (slot.getContainsHandItem())
                {
                    isHandItemSlot = true;
                }
            }
        }
        return isHandItemSlot;
    }

    private InventoryTile getInventoryTileByItemImageGO(GameObject followerItemImage)
    {
        List<Text> childrenTexts2 = new List<Text>(followerItemImage.GetComponentsInChildren<Text>());
        Text followerItemKey = childrenTexts2[1];
        Text followerItemImagePos = childrenTexts2[2];

        return this.slots.Find(slot => findItem(slot, followerItemKey, followerItemImagePos));
    }

    public void HideTipText()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            tip.GetComponent<Text>().enabled = false;
        }
    }

    public InventoryItem getInventoryItemByItemImageText(String itemImageText, List<InventoryTile> slots)
    {
        InventoryItem invItemWithNumber = null;
        foreach (InventoryTile slot in slots)
        {
            if (slot.getInventoryItem() != null && slot.getInventoryItem().itemKindNumber.ToString() == itemImageText)
            {
                invItemWithNumber = slot.getInventoryItem();
            }
        }
        return invItemWithNumber;
    }

    public InventoryItem getInventoryItemByItemImageText(String itemImageText)
    {
        InventoryItem invItemWithNumber = null;
        foreach (InventoryTile slot in this.slots)
        {
            if (slot.getInventoryItem() != null && slot.getInventoryItem().itemKindNumber.ToString() == itemImageText)
            {
                invItemWithNumber = slot.getInventoryItem();
            }
        }
        return invItemWithNumber;
    }

    private InventoryItem getInventoryItemByTilePos(String tilePos)
    {
        foreach(InventoryTile slot in this.slots)
        {
            if (tilePos.Equals(slot.getTilePos().ToString()))
            {
                return slot.getInventoryItem();
            }
        }
        return null;
    }

    /// <summary>
    /// Checks if <code>this.slots</code> contains the <code>InventoryItem</code> ip.
    /// If it does, it delivers true, otherwise it would return false.
    /// </summary>
    /// <param name="ip">The inventory item 'ip' to be looked for</param>
    /// <returns>A boolean</returns>
    public bool slotsContainsItem(InventoryItem ip)
    {
        foreach(InventoryTile slot in this.slots)
        {
            if (slot.getInventoryItem() != null && slot.getInventoryItem().itemKindNumber == ip.itemKindNumber)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks whether <code>this.slots</code> only contains full stacks of the inventory item 'ip'.
    /// If it does, it delivers true, otherwise it would return false.
    /// </summary>
    /// <param name="ip">The inventory item 'ip' to be looked for</param>
    /// <returns>A boolean</returns>
    public bool slotsContainsOnlyFullStacksOfItem(InventoryItem ip)
    {
        if (this.slotsContainsItem(ip))
        {
            foreach (InventoryTile slot in this.slots)
            {
                if (slot.getInventoryItem().itemKindNumber == ip.itemKindNumber && !slot.isInventorySlotFull())
                {
                    return false;
                }
            }
            return true;

        } else
        {
            return false;
        }
    }

    /// <summary>
    /// Swaps the position of the dropped item (image) with the targeted item (image) of the inventory ui. 
    /// This method is executed when the user drops an item (image) on a non-empty tile of the inventory ui.
    /// </summary>
    /// <param name="itemImage">Inventory tile GO holding an item pic</param>
    /// <param name="followerItemImage"></param>
    public void onItemDroppedOnItem(GameObject itemImage, GameObject followerItemImage)
    {
        List<Text> childrenTexts = new List<Text>(itemImage.GetComponentsInChildren<Text>());
        Text inventoryItemKey = childrenTexts[1];
        Text itemImagePos = childrenTexts[2];

        List<Text> childrenTexts2 = new List<Text>(followerItemImage.GetComponentsInChildren<Text>());
        Text followerItemKey = childrenTexts2[1];
        Text followerItemImagePos = childrenTexts2[2];

        InventoryItem item = this.getInventoryItemByItemImageText(inventoryItemKey.text);
        InventoryItem followerItem = this.getInventoryItemByItemImageText(followerItemKey.text);

        if (item != null && followerItem != null && isNotSameItemSlot(itemImagePos, followerItemImagePos))
        {
            InventoryTile itemSlot = this.slots.Find(slot => findItem(slot, inventoryItemKey, itemImagePos));

            InventoryTile followerItemSlot = this.slots.Find(slot => findItem(slot, followerItemKey, followerItemImagePos));

            if (itemsStackable(item, followerItem, itemSlot, followerItemSlot))
            {
                this.stackItems(itemSlot, followerItemSlot, followerItemImage);
            }
            else
            {
                switchPositionOfItems(itemSlot, followerItemSlot);
            }
        }


        this.UpdateView();
    }

    private bool isNotSameItemSlot(Text itemImagePos, Text followerItemImagePos)
    {
        return itemImagePos != followerItemImagePos;
    }

    private void switchPositionOfItems(InventoryTile itemSlot, InventoryTile followerItemSlot)
    {
        List<InventoryTile> slotsCopies = new List<InventoryTile>();

        foreach (InventoryTile slot in this.slots)
        {
            if (slot == itemSlot)
            {
                /* Loop is at index of 'itemSlot' in 'this.slots',
                so set tile pos of 'followerItemSlot' to this index */
                followerItemSlot.setTilePos(slotsCopies.Count);
                // Also overwrite marker for slot containing the hand item
                followerItemSlot.setContainsHandItem(slot.getContainsHandItem());
                // Then add it to the new slots list
                slotsCopies.Add(followerItemSlot);
            }
            else if (slot == followerItemSlot)
            {
                /* Loop is at index of 'followerItemSlot' in 'this.slots',
                 * so set tile pos of 'itemSlot' to this index */
                itemSlot.setTilePos(slotsCopies.Count);
                itemSlot.setContainsHandItem(slot.getContainsHandItem());
                // Then add it to the new slots list
                slotsCopies.Add(itemSlot);
            }
            else
            {
                // Add slot to new slots list
                slotsCopies.Add(slot);
            }
        }
        this.slots = getIdenticalCopyOfInventoryTiles(slotsCopies);
    }

    private bool itemsStackable(InventoryItem item, InventoryItem followerItem, InventoryTile itemSlot, InventoryTile followerItemSlot)
    {
        return isSameKindOfItem(item, followerItem) && itemSlot.getNumberOfItems() < 10 && followerItemSlot.getNumberOfItems() < 10;
    }

    private bool isSameKindOfItem(InventoryItem item, InventoryItem followerItem)
    {
        return item.itemKindNumber == followerItem.itemKindNumber;
    }

    private void stackItems(InventoryTile itemSlot, InventoryTile followerItemSlot, GameObject followerItemImage)
    {
        //DragItem dragItemScript = followerItemImage.GetComponent<DragItem>();
        //UnityEngine.EventSystems.PointerEventData dragEvent = new UnityEngine.EventSystems.PointerEventData()
        int stackSize = itemSlot.getNumberOfItems() + followerItemSlot.getNumberOfItems(); // <= 18 && >1
        if (stackSize > 10)
        {
            mergeBothStacksIntoTwoStacks(itemSlot, followerItemSlot, stackSize);
        }
        else
        {
            mergeBothStacksIntoOneStack(itemSlot, followerItemSlot, stackSize);
        }
    }

    private void stackItemsOfDifferentInventories(InventoryTile itemSlot, InventoryTile followerItemSlot, GameObject followerItemImage, List<InventoryTile> originInventorySlots)
    {
        //DragItem dragItemScript = followerItemImage.GetComponent<DragItem>();
        //UnityEngine.EventSystems.PointerEventData dragEvent = new UnityEngine.EventSystems.PointerEventData()
        int stackSize = itemSlot.getNumberOfItems() + followerItemSlot.getNumberOfItems(); // <= 18 && >1
        if (stackSize > 10)
        {
            mergeBothStacksFromDifferentInventoriesIntoTwoStacks(itemSlot, followerItemSlot, stackSize, originInventorySlots);
        }
        else
        {
            mergeBothStacksFromDifferentInventoriesIntoOneStack(itemSlot, followerItemSlot, stackSize, originInventorySlots);
        }
    }

    private void mergeBothStacksIntoOneStack(InventoryTile itemSlot, InventoryTile followerItemSlot, int stackSize)
    {
        List<InventoryTile> slotsCopies = new List<InventoryTile>();

        foreach (InventoryTile slot in this.slots)
        {
            if (slot == itemSlot)
            {
                slot.setStackSize(stackSize);
                slotsCopies.Add(slot);
            }
            else if (slot == followerItemSlot)
            {
                itemSlot = new InventoryTile(slot.getTilePos());
                slotsCopies.Add(itemSlot);
            }
            else
            {
                // Add slot to new slots list
                slotsCopies.Add(slot);
            }
        }
        this.slots = getIdenticalCopyOfInventoryTiles(slotsCopies);
    }

    private void mergeBothStacksFromDifferentInventoriesIntoOneStack(InventoryTile itemSlot, InventoryTile followerItemSlot, int stackSize,
        List<InventoryTile> originInventorySlots)
    {
        List<InventoryTile> slotsCopies = new List<InventoryTile>();

        foreach (InventoryTile slot in this.slots)
        {
            if (slot == itemSlot)
            {
                slot.setStackSize(stackSize);
                slotsCopies.Add(slot);
            }
            else
            {
                // Add slot to new slots list
                slotsCopies.Add(slot);
            }
        }
        this.slots = getIdenticalCopyOfInventoryTiles(slotsCopies);

        List<InventoryTile> originSlotsCopy = new List<InventoryTile>();

        foreach (InventoryTile slot in this.slots)
        {
            if (slot == followerItemSlot)
            {
                itemSlot = new InventoryTile(slot.getTilePos());
                originSlotsCopy.Add(itemSlot);
            }
            else
            {
                // Add slot to new slots list
                originSlotsCopy.Add(slot);
            }
        }
        originInventorySlots = getIdenticalCopyOfInventoryTiles(originSlotsCopy);
    }

    private void mergeBothStacksIntoTwoStacks(InventoryTile itemSlot, InventoryTile followerItemSlot, int stackSize)
    {
        // keep followerItemImage in the hand with count = sum - 10 
        // dragItemScript.OnDrag(UnityEngine.EventSystems.PointerEventData);
        int maxStackSize = 10;
        List<InventoryTile> slotsCopies = new List<InventoryTile>();

        foreach (InventoryTile slot in this.slots)
        {
            if (slot == itemSlot)
            {
                slot.setStackSize(maxStackSize);
                slotsCopies.Add(slot);
            }
            else if (slot == followerItemSlot)
            {
                slot.setStackSize(stackSize - maxStackSize);
                slotsCopies.Add(slot);
            }
            else
            {
                // Add slot to new slots list
                slotsCopies.Add(slot);
            }
        }
        this.slots = getIdenticalCopyOfInventoryTiles(slotsCopies);
    }

    private void mergeBothStacksFromDifferentInventoriesIntoTwoStacks(InventoryTile itemSlot, InventoryTile followerItemSlot, int stackSize, List<InventoryTile> targetSlots)
    {
        // keep followerItemImage in the hand with count = sum - 10 
        // dragItemScript.OnDrag(UnityEngine.EventSystems.PointerEventData);
        int maxStackSize = 10;
        List<InventoryTile> slotsCopies = new List<InventoryTile>();

        foreach (InventoryTile slot in this.slots)
        {
            if (slot == itemSlot)
            {
                slot.setStackSize(maxStackSize);
                slotsCopies.Add(slot);
            }
            else
            {
                // Add slot to new slots list
                slotsCopies.Add(slot);
            }
        }
        this.slots = getIdenticalCopyOfInventoryTiles(slotsCopies);

        List<InventoryTile> targetSlotsCopies = new List<InventoryTile>();

        foreach (InventoryTile slot in targetSlots)
        {
            if (slot == followerItemSlot)
            {
                slot.setStackSize(stackSize - maxStackSize);
                targetSlotsCopies.Add(slot);
            }
            else
            {
                // Add slot to new slots list
                targetSlotsCopies.Add(slot);
            }
        }
        targetSlots = getIdenticalCopyOfInventoryTiles(targetSlotsCopies);
    }

    /// <summary>
    /// Swaps the position of the dropped item (image) with the targeted item (image) of the inventory ui. 
    /// This method is executed when the user drops an item (image) from the player inventory on a non-empty tile 
    /// of the chest inventory ui.
    /// </summary>
    /// <param name="itemImage">Inventory tile GO holding an item pic</param>
    /// <param name="followerItemImage">The image of the dragged player item GO</param>
    public void onPlayerItemDroppedOnChestItem(GameObject itemImage, GameObject followerItemImage)
    {
        List<Text> childrenTexts = new List<Text>(itemImage.GetComponentsInChildren<Text>());
        Text inventoryItemKey = childrenTexts[1];
        Text itemImagePos = childrenTexts[2];

        List<Text> childrenTexts2 = new List<Text>(followerItemImage.GetComponentsInChildren<Text>());
        Text followerItemKey = childrenTexts2[1];
        Text followerItemImagePos = childrenTexts2[2];

        InventoryItem item = this.getInventoryItemByItemImageText(inventoryItemKey.text);
        InventoryItem followerItem = player.GetComponent<Inventory>().getInventoryItemByItemImageText(followerItemKey.text);

        if (item != null && followerItem != null)
        {
            InventoryTile itemSlot = this.slots.Find(slot => findItem(slot, inventoryItemKey, itemImagePos));
            List<InventoryTile> playerSlots = player.GetComponent<Inventory>().slots;
            InventoryTile followerItemSlot = playerSlots.Find(slot => findItem(slot, followerItemKey, followerItemImagePos));

            if (itemsStackable(item, followerItem, itemSlot, followerItemSlot))
            {
                this.stackItemsOfDifferentInventories(itemSlot, followerItemSlot, followerItemImage, playerSlots);
            } else
            {
                switchPositionOfPlayerItemAndChestItem(itemSlot, followerItemSlot);
            }
        }
        player.GetComponent<Inventory>().UpdateView();
        this.UpdateView();
    }

    private void switchPositionOfPlayerItemAndChestItem(InventoryTile itemSlot, InventoryTile followerItemSlot)
    {
        updateThisChestInventorySlots(itemSlot, followerItemSlot);

        updatePlayerInventorySlots(itemSlot, followerItemSlot);
    }

    private void updateThisChestInventorySlots(InventoryTile itemSlot, InventoryTile followerItemSlot)
    {
        List<InventoryTile> slotsCopy = new List<InventoryTile>();

        foreach (InventoryTile slot in this.slots)
        {
            if (slot.getTilePos() == itemSlot.getTilePos())
            {
                addFollowerItemToSlotsCopy(followerItemSlot, slotsCopy);
            }
            else
            {
                // Add slot to new slots list
                slotsCopy.Add(slot);
            }
        }
        this.slots = getIdenticalCopyOfInventoryTiles(slotsCopy);
    }

    private void addFollowerItemToSlotsCopy(InventoryTile followerItemSlot, List<InventoryTile> slotsCopies)
    {
        /* Loop is at index of 'itemSlot' in 'this.slots',
                        so set tile pos of 'followerItemSlot' to this index */
        InventoryTile copy = followerItemSlot.getIdenticalCopy();
        copy.setTilePos(slotsCopies.Count);
        // Then add it to the new slots list
        slotsCopies.Add(copy);
    }

    private void updatePlayerInventorySlots(InventoryTile itemSlot, InventoryTile followerItemSlot)
    {
        List<InventoryTile> playerSlotsCopies = new List<InventoryTile>();
        List<InventoryTile> playerSlots = player.GetComponent<Inventory>().slots;

        foreach (InventoryTile slot in playerSlots)
        {
            if (slot.getTilePos() == followerItemSlot.getTilePos())
            {
                /* Loop is at index of 'followerItemSlot' in 'this.slots',
                 * so set tile pos of 'itemSlot' to this index */
                InventoryTile copy = itemSlot.getIdenticalCopy();
                copy.setTilePos(playerSlotsCopies.Count);
                // Then add it to the new slots list
                playerSlotsCopies.Add(copy);
            }
            else
            {
                // Add slot to new slots list
                playerSlotsCopies.Add(slot);
            }
        }
        player.GetComponent<Inventory>().slots = getIdenticalCopyOfInventoryTiles(playerSlotsCopies);
    }

    /// <summary>
    /// Swaps the position of the dropped item (image) with the targeted item (image) of the inventory ui. 
    /// This method is executed when the user drops an item (image) of the chest inventory on a non-empty tile 
    /// of the player inventory ui.
    /// </summary>
    /// <param name="itemImage">Inventory tile GO holding an item pic</param>
    /// <param name="followerItemImage">The image of the dragged chest item GO</param>
    public void OnChestItemDroppedOnPlayerItem(GameObject itemImage, GameObject followerItemImage, Inventory chestInventory)
    {
        List<Text> childrenTexts = new List<Text>(itemImage.GetComponentsInChildren<Text>());
        Text inventoryItemKey = childrenTexts[1];
        Text itemImagePos = childrenTexts[2];

        List<Text> childrenTexts2 = new List<Text>(followerItemImage.GetComponentsInChildren<Text>());
        Text followerItemKey = childrenTexts2[1];
        Text followerItemImagePos = childrenTexts2[2];

        InventoryItem item = this.getInventoryItemByItemImageText(inventoryItemKey.text);
        InventoryItem followerItem = chestInventory.getInventoryItemByItemImageText(followerItemKey.text);

        if (item != null && followerItem != null)
        {
            InventoryTile itemSlot = this.slots.Find(slot => findItem(slot, inventoryItemKey, itemImagePos));
            List<InventoryTile> chestSlots = chestInventory.slots;
            InventoryTile followerItemSlot = chestSlots.Find(slot => findItem(slot, followerItemKey, followerItemImagePos));

            if (itemsStackable(item, followerItem, itemSlot, followerItemSlot))
            {
                this.stackItemsOfDifferentInventories(itemSlot, followerItemSlot, followerItemImage, chestSlots);
            }
            else
            {
                switchPositionOfChestItemAndPlayerItem(chestInventory, itemSlot, followerItemSlot);
            }
        }
        chestInventory.UpdateView();
        this.UpdateView();
    }

    private void switchPositionOfChestItemAndPlayerItem(Inventory chestInventory, InventoryTile itemSlot, InventoryTile followerItemSlot)
    {
        updateThisPlayerInventorySlots(itemSlot, followerItemSlot);
        updateChestInventorySlots(chestInventory, itemSlot, followerItemSlot);
    }

    private void updateThisPlayerInventorySlots(InventoryTile itemSlot, InventoryTile followerItemSlot)
    {       
        List<InventoryTile> slotsCopies = new List<InventoryTile>();

        foreach (InventoryTile slot in this.slots)
        {
            if (slot.getTilePos() == itemSlot.getTilePos())
            {
                addFollowerItemToSlotsCopy(followerItemSlot, slotsCopies);
            }
            else
            {
                // Add slot to new slots list
                slotsCopies.Add(slot);
            }
        }
        this.slots = getIdenticalCopyOfInventoryTiles(slotsCopies);
    }

    private void updateChestInventorySlots(Inventory chestInventory, InventoryTile itemSlot, InventoryTile followerItemSlot)
    {
        List<InventoryTile> chestSlotsCopy = new List<InventoryTile>();

        foreach (InventoryTile slot in chestInventory.slots)
        {
            if (slot.getTilePos() == followerItemSlot.getTilePos())
            {
                addItemToChestSlotsCopy(itemSlot, chestSlotsCopy);
            }
            else
            {
                // Add slot to new slots list
                chestSlotsCopy.Add(slot);
            }
        }
        chestInventory.slots = getIdenticalCopyOfInventoryTiles(chestSlotsCopy);
    }

    private void addItemToChestSlotsCopy(InventoryTile itemSlot, List<InventoryTile> chestSlotsCopies)
    {
        /* Loop is at index of 'followerItemSlot' in 'this.slots',
                         * so set tile pos of 'itemSlot' to this index */
        InventoryTile copy = itemSlot.getIdenticalCopy();
        copy.setTilePos(chestSlotsCopies.Count);
        // Then add it to the new slots list
        chestSlotsCopies.Add(copy);
    }

    private List<InventoryTile> getIdenticalCopyOfInventoryTiles(List<InventoryTile> tiles)
    {
        List<InventoryTile> copy = new List<InventoryTile>();

        for (int j = 0; j < tiles.Count; j++)
        {
            copy.Add(tiles[j]);
        }
        return copy;
    }

    /// <summary>
    /// Placing an item (image) on an empty tile GO of the inventory ui. This method is executed when the user
    /// drops an item (image) on an empty tile of the inventory ui.
    /// </summary>
    /// <param name="emptyItemImage">The inventory tile GO holding no item pic</param>
    /// <param name="followerItemImage">The inventory tile GO holding the item pic of the follower item</param>
    public void placeItemOnEmptyTile(GameObject emptyItemImage, GameObject followerItemImage, Boolean itemFromPlayerInv)
    {
        List<Text> childrenTexts = new List<Text>(emptyItemImage.GetComponentsInChildren<Text>());
        Text itemImagePos = childrenTexts[2];

        List<Text> childrenTexts2 = new List<Text>(followerItemImage.GetComponentsInChildren<Text>());
        Text followerItemKey = childrenTexts2[1];
        Text followerItemImagePos = childrenTexts2[2];

        InventoryItem item = this.getInventoryItemByTilePos(itemImagePos.text);
        InventoryItem followerItem = null;
        if (itemFromPlayerInv && gameObject.name.Equals("Chest"))
        {
            followerItem = this.getInventoryItemByItemImageText(followerItemKey.text, player.GetComponent<Inventory>().slots);
        } else if (!itemFromPlayerInv && gameObject.name.Equals("Player"))
        {
            followerItem = this.getInventoryItemByItemImageText(followerItemKey.text, handImage.GetComponent<DropItem>().getChestInventory().slots);
        }
        else
        {
            followerItem = this.getInventoryItemByItemImageText(followerItemKey.text, this.slots);
        }

        // Do not allow placing empty images on item images, because dragging of empty tiles should be suppressed!
        if (followerItem != null)
        {
            InventoryTile emptySlot = findEmptySlotInSlots(itemImagePos.text);
            InventoryTile followerItemSlot = null;
            if (itemFromPlayerInv && gameObject.name.Equals("Chest"))
            {
                followerItemSlot = player.GetComponent<Inventory>().slots.Find(slot => findItem(slot, followerItemKey, followerItemImagePos));
            }
            else if (!itemFromPlayerInv && gameObject.name.Equals("Player"))
            {
                followerItemSlot = handImage.GetComponent<DropItem>().getChestInventory().slots.Find(slot => findItem(slot, followerItemKey, followerItemImagePos));
            }
            else
            {
                followerItemSlot = this.slots.Find(slot => findItem(slot, followerItemKey, followerItemImagePos));  
            }

            List<InventoryTile> slotsCopies = new List<InventoryTile>();
            int followerNumberOfItems = followerItemSlot.getNumberOfItems();

            foreach (InventoryTile slot in this.slots)
            {
                if (slot.getIsEmptyTile() == true && slot.getTilePos().ToString().Equals(itemImagePos.text))
                {
                    // Creates a new inventory tile and fills it with the follower item units
                    InventoryTile newEmptyTile = new InventoryTile(slotsCopies.Count);
 
                    for (int i = 0; i < followerNumberOfItems; i++)
                    {
                        newEmptyTile.addOneItem(followerItem);
                        
                    }
                    slotsCopies.Add(newEmptyTile);
                }
                else
                {
                    slotsCopies.Add(slot);
                }
            }
            this.slots = getIdenticalCopyOfInventoryTiles(slotsCopies);
        }
        this.UpdateView();
    }

    private bool findItem(InventoryTile slot, Text followerItemKey, Text itemImagePos)
    {
        if (slot.getInventoryItem() != null) {
            if (slot.getInventoryItem().itemKindNumber.ToString().Equals(followerItemKey.text))
            {
                if (itemImagePos.text.Equals(slot.getTilePos().ToString()))
                {
                    return true;
                }
            }
        }
    return false;
    }

    private InventoryTile findEmptySlotInSlots(String followerItemImagePos)
    {
        foreach (InventoryTile slot in this.slots)
        {
            if (followerItemImagePos.Equals(slot.getTilePos().ToString()))
            {
                return slot;
            }
        }
        return null;
    }
}
