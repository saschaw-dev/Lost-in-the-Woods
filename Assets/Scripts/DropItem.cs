using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Klasse regelt das Droppen von Inventarpics //

public class DropItem : MonoBehaviour, IDropHandler {

    public List<GameObject> itemImages;
    public List<GameObject> chestItemImages;
    Inventory playerInventory;
    Inventory chestInventory;
    RectTransform myRectTransform;
    RectTransform rectTransformInvPanel;
    RectTransform rectTransformChest;
    RectTransform[] rectTransformChestTiles = new RectTransform[9];
    GameObject player;
    GameObject invPanel;
    GameObject chest;
    List<GameObject> chestPanels = new List<GameObject>();
    Dictionary<GameObject, GameObject> chests = new Dictionary<GameObject, GameObject>();
    GameObject activeChestPanel;

    void Awake()
    {
        chestPanels.Add(GameObject.Find("ChestPanel"));
        if (chestPanels[0] != null)
        {
            rectTransformChest = chestPanels[0].GetComponent<RectTransform>();
        }
    }

    // Use this for initialization
    void Start() {
        player = GameObject.Find("Player");
        playerInventory = player.GetComponent<Inventory>();
        chest = GameObject.Find("Chest");

        this.rectTransformChestTiles = getRectTransformChestTiles(chestPanels[0]);

        if (chest != null)
        {
            chestInventory = chest.GetComponent<Inventory>();
            if (chestPanels[0] != null)
            {
                chests.Add(chestPanels[0], chest);
            }
        }
        myRectTransform = GetComponent<RectTransform>();
        if (invPanel == null)
        {
            invPanel = GameObject.Find("InventoryPanel");
        }
        rectTransformInvPanel = invPanel.GetComponent<RectTransform>();
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public Inventory getChestInventory()
    {
        return this.chestInventory;
    }

    public void setChestInventory(Inventory chestInventory)
    {
        this.chestInventory = chestInventory;
    }

    public void OnDrop(PointerEventData eventData)
    {
        string itemKey = eventData.pointerDrag.transform.GetChild(1).GetComponent<Text>().text;
        InventoryItem ip; 
        if (eventData.pointerDrag.CompareTag("chestImage"))
        {
            // Hole dir das Truhenitem
            ip = chestInventory.getInventoryItemByItemImageText(itemKey);
        } else
        {
            // Hole dir das Spieleritem
            ip = playerInventory.getInventoryItemByItemImageText(itemKey);
        }

        // Wurde dieses GO (HandImage) gedroppt?
        if (eventData.pointerDrag == gameObject)
        {
            // außerhalb des Hand-Slots?
            if (!RectTransformUtility.RectangleContainsScreenPoint(myRectTransform, Input.mousePosition, null))
            {
                // Aus der Hand entfernen
                RemoveItemFromHand(ip);
            }
        }
        else if (eventData.pointerDrag.CompareTag("chestImage"))
        {
            // es handelt sich um ein Item aus der Truhe, das gedroppt wurde!
            // innerhalb der Truhe gedroppt?
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransformChest, Input.mousePosition, null))
            {
                foreach (GameObject chestItemImage in chestItemImages)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(chestItemImage.GetComponent<RectTransform>(), Input.mousePosition, null))
                    {
                        if (chestItemImage.GetComponent<Image>().sprite != null)
                        {
                            chestInventory.onItemDroppedOnItem(chestItemImage, eventData.pointerDrag);
                        }
                        else
                        {
                            // item dropped in an empty tile
                            chestInventory.placeItemOnEmptyTile(chestItemImage, eventData.pointerDrag, false);
                            chestInventory.removeFollowerItemFromInventorySlots(eventData.pointerDrag);
                        }
                    }
                }
                    // nichts
                    // dropped within itemImage 'itemImage'

                // innerhalb des Spielerinventars gedroppt?
            } else if (RectTransformUtility.RectangleContainsScreenPoint(rectTransformInvPanel, Input.mousePosition, null))
            {

                foreach (GameObject itemImage in itemImages)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(itemImage.GetComponent<RectTransform>(), Input.mousePosition, null))
                    {
                        if (!playerInventory.isHandEmpty())
                        {
                            if (playerInventory.disableContainsHandItemIfHandItem(itemImage))
                            {
                                List<Text> childrenTexts = new List<Text>(itemImage.GetComponentsInChildren<Text>());
                                Text itemText = childrenTexts[1];
                                ip = playerInventory.getInventoryItemByItemImageText(itemText.text);
                                playerInventory.AddItemToYourHand(ip);
                            }
                        }
                        if (itemImage.GetComponent<Image>().sprite != null)
                        {
                            // ToDo: switchItemsFromChestToPlayer
                            playerInventory.OnChestItemDroppedOnPlayerItem(itemImage, eventData.pointerDrag, chestInventory);
                        }
                        else
                        {
                            // item dropped in an empty tile
                            playerInventory.placeItemOnEmptyTile(itemImage, eventData.pointerDrag, false);
                            chestInventory.removeFollowerItemFromInventorySlots(eventData.pointerDrag);
                        }
                    }
                }
            } else
            {
                // Item irgendwo außerhalb gedroppt?
                chestInventory.DropFromInventory(ip);
            }
        }
        else
        {
            //Es handelt sich weder um ein HandImageItem noch um ein ChestImageItem das gedroppt wurde!
            // Innerhalb des Hand-Slots gedroppt?
            if (RectTransformUtility.RectangleContainsScreenPoint(myRectTransform, Input.mousePosition, null))
            {
                AddItemToHand(eventData, ip);
                // Packe Key in den Hand-Slot
                gameObject.transform.GetChild(1).GetComponent<Text>().enabled = true;
                gameObject.transform.GetChild(1).GetComponent<Text>().text = itemKey;
                gameObject.transform.GetChild(1).GetComponent<Text>().enabled = false;
            }
            else
            {
                // Außerhalb des Inventars gedroppt?
                if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransformInvPanel, Input.mousePosition, null))
                {
                    // innerhalb der Truhe gedroppt?
                    if (isInChestRectTransform())
                    {
                        foreach (GameObject chestItemImage in chestItemImages)
                        {
                            if (RectTransformUtility.RectangleContainsScreenPoint(chestItemImage.GetComponent<RectTransform>(), Input.mousePosition, null))
                            {
                                if(!playerInventory.isHandEmpty())
                                {
                                    if(playerInventory.disableContainsHandItemIfHandItem(eventData.pointerDrag))
                                    {
                                        playerInventory.AddItemToYourHand(ip);
                                    }
                                }
                                if (chestItemImage.GetComponent<Image>().sprite != null)
                                {
                                    // ToDo: switchItemsFromPlayerToChest
                                    chestInventory.onPlayerItemDroppedOnChestItem(chestItemImage, eventData.pointerDrag);
                                }
                                else
                                {
                                    // item dropped in an empty tile
                                    chestInventory.placeItemOnEmptyTile(chestItemImage, eventData.pointerDrag, true);
                                    playerInventory.removeFollowerItemFromInventorySlots(eventData.pointerDrag);
                                }
                            }
                        }


                    }
                    else
                    {
                        // außerhalb der Truhe und außerhalb des Inventars gedroppt 
                        if (!playerInventory.isHandEmpty())
                        {
                            if(playerInventory.disableContainsHandItemIfHandItem(eventData.pointerDrag))
                            {
                                playerInventory.AddItemToYourHand(ip);
                            }
                        }
                        playerInventory.DropFromInventory(ip);
                    }
                } else
                {
                    int i = 0;
                    // dropped within the inventory, but outside the hand tile
                    foreach (GameObject itemImage in itemImages)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(itemImage.GetComponent<RectTransform>(), Input.mousePosition, null))
                        {
                            

                            // dropped within itemImage 'itemImage'
                            if (itemImage.GetComponent<Image>().sprite != null)
                            {
                                playerInventory.onItemDroppedOnItem(itemImage, eventData.pointerDrag);
                            } else
                            {
                                // item dropped in an empty tile
                                playerInventory.placeItemOnEmptyTile(itemImage, eventData.pointerDrag, true);
                                playerInventory.removeFollowerItemFromInventorySlots(eventData.pointerDrag);
                            }
                        }
                        i++;
                    }
                }
            }
        }       
    }

    private void AddItemToHand(PointerEventData eventData, InventoryItem ip)
    {
        playerInventory.updateContainsHandItemForHandItemSlot(eventData.pointerDrag, gameObject);
        playerInventory.AddItemToYourHand(ip);
    }

    private void RemoveItemFromHand(InventoryItem ip)
    {
        playerInventory.disableContainsHandItemInInventory();
        playerInventory.AddItemToYourHand(ip);
    }

    private bool isInChestRectTransform()
    {
        int arrayLength = 9;
        this.rectTransformChestTiles = getRectTransformChestTiles(this.activeChestPanel = getActiveChestPanel());
        if (activeChestPanel != null)
        {
            for (int i = 0; i < arrayLength; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransformChestTiles[i], Input.mousePosition, null))
                {
                    return true;
                }
            }
            if (RectTransformUtility.RectangleContainsScreenPoint(activeChestPanel.GetComponent<RectTransform>(), Input.mousePosition, null))
            {
                return true;
            }
        }
        return false;
    }

    private GameObject getActiveChestPanel()
    {
        GameObject chestPanel = null;
        foreach (var x in chestPanels)
        {
            if (x.activeSelf)
            {
                return x;
            }
        }
        return chestPanel;
    }

    public void addChest(GameObject chestPanel, GameObject chest)
    {
        this.chests.Add(chestPanel, chest);
        this.chestPanels.Add(chestPanel);
    }

    private RectTransform[] getRectTransformChestTiles(GameObject activeChestPanel)
    {
        if (activeChestPanel == null)
        {
            return null;
        }
        RectTransform[] rectTransformChestTiles = new RectTransform[9];
        for (int i = 0; i < rectTransformChestTiles.Length; i++)
        {
            rectTransformChestTiles[i] = activeChestPanel.transform.GetChild(0).GetChild(2 + i).gameObject.GetComponent<RectTransform>();
        }
        return rectTransformChestTiles;
    }
}
