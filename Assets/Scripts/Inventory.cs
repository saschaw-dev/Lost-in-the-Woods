using System;
using System.Collections.Generic; //Für das generische Objekt Dictionary
using UnityEngine;
using UnityEngine.UI; //Für UI-Elemente, wie z.B. Image etc.

[Serializable]
public class Inventory : MonoBehaviour
{
    public GameObject[] itemImageSlots = new GameObject[10]; // itemImageSlots sind nicht die Kacheln selbst, sondern das weiße GO darüber!
    public Dictionary<InventoryItem, int> items = new Dictionary<InventoryItem, int>();
    private GameObject player;
    GameObject inventoryPanel;
    GameObject chestPanel;
    private GameObject craftingPanel;
    private GameObject craftingErrorObject;
    public GameObject[] craftingItemPics = new GameObject[4]; //Item Bilder im Crafting Menue
    public bool switcher = false; //Bool für (Inventar/Mauszeiger) ein und ausblenden
    private int itemCount;     //für das Stapeln der Items
    PickableObject obj;
    Vector3 dropPosVec = new Vector3(0, 0, 0);
    GameObject dropPos;
    public Image handImage;
    PlayerHealth playerHealth;
    Text craftingErrorText;
    public AudioClip error;
    public AudioClip craftingSound;
    public Image itemImage;
    private int itemStackSize = 10; // Stapelgröße für Items
    Text tipLabelText;
    Text tipLabelText2;
    Player playerScript;
    public GameObject[] itemKeys = new GameObject[10]; // Ordnet jedem ItemImage das InventoryItem zu
    Text itemKeyText;
    Build buildScript;
    GameObject itemLabel;
    Text itemFrameText;
    GameObject tip;
    GameObject itemImageFollower;
    RaycastHit hit;
    Ray ray;
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

    public void setChestPanel(GameObject chestPanel)
    {
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
        for (int j = 0; j < itemImageSlots.Length; j++)
        {
            if (itemImageSlots[j] != null)
            {
                itemImage = itemImageSlots[j].GetComponent<Image>();
                itemImage.enabled = false;
                itemImage.GetComponentInChildren<Text>().text = "";
                itemImageSlots[j].SetActive(false);
            }
        }

        int i = 0;

        foreach (var current in items)
        {
            if (current.Value > itemStackSize) //Stapelgrenze erreicht?
            {
                itemCount = current.Value;

                while (itemCount > itemStackSize && i < itemImageSlots.Length)
                {
                    itemImageSlots[i].SetActive(true);//Aktiviere ItemImage GO
                    itemImage = itemImageSlots[i].GetComponent<Image>();
                    itemImage.enabled = true;
                    itemImage.sprite = current.Key.sprite;
                    itemImage.GetComponentInChildren<Text>().text = "10";
                    itemCount -= 10;
                    i++;
                }
              
                if (itemCount > 0 && i < itemImageSlots.Length)               //Item mit Menge 0 soll nicht angezeigt werden
                {
                    itemImageSlots[i].SetActive(true);
                    itemImage = itemImageSlots[i].GetComponent<Image>();
                    itemImage.enabled = true;
                    itemImage.sprite = current.Key.sprite;
                    itemKeys[i].GetComponent<Text>().text = current.Key.itemNumber.ToString();
                    itemImage.GetComponentInChildren<Text>().text = itemCount.ToString();
                    i++;
                }

            }
            else if (current.Value < 1) //falls Item gedroppt wurde und Anzahl jetzt Null ist, dann blende Item-Auswahlrahmen und Bild aus
            {
                GameObject.Find("ItemFrame").GetComponent<Image>().enabled = false;
                GameObject.Find("Text1").GetComponent<Text>().enabled = false;
                GameObject.Find("Text2").GetComponent<Text>().enabled = false;
                if (handImage != null)
                {
                    handImage.sprite = null;
                }
            }
            else
            {
                itemImageSlots[i].SetActive(true);
                itemImage = itemImageSlots[i].GetComponent<Image>();
                itemImage.enabled = true;
                itemImage.sprite = current.Key.sprite;
                itemKeys[i].GetComponent<Text>().text = current.Key.itemNumber.ToString();
   
                itemImage.GetComponentInChildren<Text>().text = current.Value.ToString();
                i++;
            }
        }
    }

    // falls Wahr: Jeder Slot ist mit maximaler Stapelgröße besetzt
    private bool IsCompleteFull()
    {
        for (int j = 0; j < itemImageSlots.Length; j++)
        {
            itemImage = itemImageSlots[j].GetComponent<Image>();
            if (itemImage.sprite == null || itemImage.GetComponentInChildren<Text>().text != "10")
            {
                return false;
            }
        }
        return true;
    }


    public bool IsInventoryFull(InventoryItem ip)
    {
        if(IsCompleteFull())
        {
            return true;
        }
        else
        {  
            for (int i = 0; i < itemImageSlots.Length; i++)
            {
                itemImage = itemImageSlots[i].GetComponent<Image>();
                if (itemImage.GetComponentInChildren<Text>().text != "10")
                {
                    if(itemImage.sprite == ip.sprite || itemImage.sprite == null)
                    {
                        return false;
                    }
                }  
                if(itemImage.enabled == false || itemImage.sprite == null)
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
        if (!items.ContainsKey(ip))
        {
            if (items.Count < itemImageSlots.Length)
            {
                items.Add(ip, 1);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (items[ip] < 90)
            {
                items[ip]++;
            }
            else
            {
                return false;
            }
        }
        UpdateView();
        return true;
    }

    public bool RemoveItem(InventoryItem ip) //Droppt ein Item
    {
        if (items.ContainsKey(ip))
        {
            if(items[ip] <= 0)
            {
                return false;
            }
            else if(items[ip] == 1)
            {
                Instantiate(ip.prefab, dropPos.transform.position, Quaternion.identity);
                items[ip]--;
                items.Remove(ip);
            }
            else
            {
                Instantiate(ip.prefab, dropPos.transform.position, Quaternion.identity);
                items[ip]--;
            }
            UpdateView();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool UseItem(InventoryItem ip) //Verbraucht ein Item (nicht Droppen!)
    {
        // verhindert Fehler nach Entfernen des letzten Items aus der Truhe
        disableItemFollower();

        if (items.ContainsKey(ip))
        {
            if (items[ip] <= 0)
            {
                return false;
            }
            else if (items[ip] == 1)
            {
                items[ip]--;
                items.Remove(ip);
            }
            else
            {
                items[ip]--;
            }
            UpdateView();
            return true;
        }
        else
        {
            return false;
        }
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
        /*string s = transform.name;
        Transform t = transform.parent;
        while (t != null)
        {
            s = t.name + "/" + s;
            t = t.parent;
        }
        Debug.Log("MouseDown " + Time.frameCount + " : " + s);*/

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
        if (InventoryItem.lastEquiped == false)
        {
            if (tip.GetComponent<Text>().enabled)
            {
                tip.GetComponent<Text>().enabled = false;
            }

            return true;
        } else
        {
            tip.GetComponent<Text>().enabled = true;
            tip.GetComponent<Text>().text = "Wenn du ein Gegenstand ausgerüstet hast, kannst du nichts droppen lassen!";
            return false;
        }
    }

    public void HideTipText()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            tip.GetComponent<Text>().enabled = false;
        }
    }

    public InventoryItem getInventoryItemByItemImageText(String itemImageText)
    {
        InventoryItem invItem = null;
        foreach (InventoryItem key in items.Keys)
        {
            if (key.itemNumber.ToString() == itemImageText)
            {
                invItem = key;
            }
        }
        return invItem;
    }

    public void switchItems(GameObject itemImage, GameObject followerItemImage)
    {
        List<Text> childrenTexts = new List<Text>(itemImage.GetComponentsInChildren<Text>());
        Text inventoryItemKey = childrenTexts[1];

        List<Text> childrenTexts2 = new List<Text>(followerItemImage.GetComponentsInChildren<Text>());
        Text followerItemKey = childrenTexts2[1];

        InventoryItem item = this.getInventoryItemByItemImageText(inventoryItemKey.text);
        InventoryItem followerItem = this.getInventoryItemByItemImageText(followerItemKey.text);

        if (this.items.ContainsKey(item))
        {
            if (this.items.ContainsKey(followerItem))
            {
                Dictionary<InventoryItem, int> newItems = new Dictionary<InventoryItem, int>();
                foreach (InventoryItem key in this.items.Keys) {
                    if (key == item)
                    {
                        newItems.Add(followerItem, this.items[followerItem]);
                    }
                    else if (key == followerItem)
                    {
                        newItems.Add(item, this.items[item]);
                    }
                    else
                    {
                        newItems.Add(key, this.items[key]);
                    }
                }
                this.items = newItems;

            }
        }
        this.UpdateView();
    }
}
