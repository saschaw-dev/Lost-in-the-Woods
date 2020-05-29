using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Craft : MonoBehaviour {

    private GameObject craftingPanel;
    private GameObject craftingErrorObject;
    public InventoryItem craftableItem;
    public AudioClip error;
    public AudioClip craftingSound;
    private Inventory playerInventory;
    GameObject itemInfo;
    GameObject itemInfoTextGO;
    GameObject itemLabelTextGO;
    GameObject craftingBarFilled;
    GameObject craftingBar;
    Text itemInfoText;
    Text itemLabelText;
    private bool mouseIsOver = false;
    GameObject player;
    Image craftingProgressBar;
    private bool isCrafting = false;
    float craftingProgress = 0f;
    float maxCrafting = 100f;
    List<InventoryItem> craftingChain = new List<InventoryItem>();
    public GameObject[] itemSite1;
    public GameObject[] itemSite2;
    public GameObject[] itemSite3;
    private int siteNumber;
    public static bool isCraftingTableOpen = false;

    private void Awake()
    {    
        player = GameObject.Find("Player");
        craftingPanel = GameObject.Find("CraftingPanel");
        itemInfo = GameObject.Find("ItemInfo");
        itemInfoTextGO = GameObject.Find("ItemText");
        itemLabelTextGO = GameObject.Find("ItemLabelText");
        craftingErrorObject = GameObject.Find("CraftingError");
        playerInventory = player.GetComponent<Inventory>();
        craftingBar = GameObject.Find("CraftingBar");
        craftingBarFilled = GameObject.Find("CraftingProgress");
    }

    // Use this for initialization
    void Start () {
        itemInfoText = itemInfoTextGO.GetComponent<Text>();
        itemLabelText = itemLabelTextGO.GetComponent<Text>();
        craftingErrorObject.SetActive(false);
        craftingProgressBar = craftingBarFilled.GetComponentInChildren<Image>(); 
        craftingBarFilled.SetActive(false);
        itemInfo.SetActive(false);
        try
        {
            craftableItem.craftingText = craftableItem.craftingText.Replace("NEWLINE", "\n");
        }
        catch(NullReferenceException ex)
        {   
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OpenOrCloseCraftingTable();
        }
        RemoveErrorMessage();
        if (isCrafting)
        {
            CraftingProgress();
        }
    }

    private void RemoveErrorMessage()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            craftingErrorObject.SetActive(false);
        }
    }

    private void setActivityFalse()
    {
        for (int i = 0; i < itemSite2.Length; i++)
        {
            itemSite2[i].SetActive(false);
        }
    }

    private void CraftingProgress()
    {
        craftingProgress +=  this.craftableItem.craftingSpeed * TimeController.deltaTime;
        craftingProgressBar.fillAmount = craftingProgress / maxCrafting;
        if (craftingProgress >= maxCrafting)
        {
            craftingProgress = maxCrafting;
            craftingProgressBar.fillAmount = craftingProgress / maxCrafting;
            craftingProgress = 0f;
            craftingBarFilled.SetActive(false);
            craftingBar.SetActive(false);
            isCrafting = false;
            playerInventory.AddItem(craftableItem);
        }
    }

    public void OpenOrCloseCraftingTable()
    {
        
        if (craftingPanel.activeSelf)
        {
            // Herstellungsmenü schließen
            PauseGame.toggleTimeScale();
            craftingPanel.SetActive(false);
            isCraftingTableOpen = false;
            itemInfo.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // Herstellungsmenü öffnen
            if (!playerInventory.isInventoryOpen && !Shortkeys.isCommandWindowVisible)
            {
                craftingPanel.SetActive(true);
                isCraftingTableOpen = true;
                this.siteNumber = 3;
                setActivityFalse();
                OnButtonNextSite();
                PauseGame.toggleTimeScale();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    

    public void OnButtonNextSite()
    {
        if (this.siteNumber == 1)
        {
            setActivityOfItems(itemSite1, itemSite2);
            this.siteNumber = 2;
            return;
        }
        if (this.siteNumber == 2)
        {
            setActivityOfItems(itemSite2, itemSite3);
            this.siteNumber = 3;
            return;
        }
        if (this.siteNumber == 3)
        {
            setActivityOfItems(itemSite3, itemSite1);
            this.siteNumber = 1;
            return;
        }
    }

    private void setActivityOfItems(GameObject[] currentItemSite, GameObject[] nextItemSite)
    {
        for (int i = 0; i < currentItemSite.Length; i++)
        {
            currentItemSite[i].SetActive(false);
        }
        for (int i = 0; i < nextItemSite.Length; i++)
        {
            nextItemSite[i].SetActive(true);
        }
    }

    public void OnButtonCraft()
    {
        //Methode die ausgelöst wird, wenn Button "Herstellen" gedrückt wird
        if(!isCrafting)
        {
            if (CraftItem(craftableItem))
            {
                //Herstellen geklappt
                isCrafting = true;
                craftingBarFilled.SetActive(true);
                craftingBar.SetActive(true);
                // hier war AddItem()
                AudioSource.PlayClipAtPoint(craftingSound, player.transform.position);
            }
            else
            {
                //Herstellen nicht geklappt
                craftingErrorObject.SetActive(true);
                //AudioSource.PlayClipAtPoint
            }
        }    
    }

    public bool CraftItem(InventoryItem ip)
    {
        //if(!playerInventory.IsInventoryFull(ip))
        //{
            foreach (var current in playerInventory.items)
            {
                if (ip.ingredient1 == current.Key.itemNumber)
                {
                    //richtiges Item im Inventar gefunden

                    if (ip.units1 <= current.Value)
                    {
                        craftingChain.Add(current.Key);

                        //dann wiederhole das Ganze für Zutat2:
                        foreach (var current2 in playerInventory.items)
                        {
                            if (ip.ingredient2 == 0)
                            {
                                //wenn es keine Zutat2 gibt, dann auch keine Zutat3 somit haben wir alle notwendigen Zutaten
                                return IsInventoryNotFull(craftingChain, ip);
                            }

                            if (ip.ingredient2 == current2.Key.itemNumber)
                            {
                                //richtiges Item im Inventar gefunden
                                if (ip.units2 <= current2.Value)
                                {
                                    craftingChain.Add(current2.Key);
                                    //dann wiederhole das Ganze für Zutat3:
                                    foreach (var current3 in playerInventory.items)
                                    {
                                        if (ip.ingredient3 == 0)
                                        {
                                            //wenn es keine dritte Zutat gibt und hierhin gekommen bist, dann hast du bereits alle notwendigen Zutaten   
                                            return IsInventoryNotFull(craftingChain, ip);
                                        }

                                        if (ip.ingredient3 == current3.Key.itemNumber)
                                        {
                                            //richtiges Item im Inventar gefunden
                                            if (ip.units3 <= current3.Value)
                                            {
                                                craftingChain.Add(current3.Key);
                                                //Menge ist vorhanden
                                                return IsInventoryNotFull(craftingChain, ip);
                                            }
                                            else
                                            {
                                                //Menge ist nicht vorhanden
                                                craftingChain.Clear();
                                                return false;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //Menge ist nicht vorhanden
                                    craftingChain.Clear();
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Menge ist nicht vorhanden
                        craftingChain.Clear();
                        return false; //zu wenig Einheiten des Items im Inventar
                    }
                }
            }
        //falls Compiler aus der Schleife rausgeht ohne das jeweilige Item gefunden zu haben, dann gebe false zurück,dann besitzt der Spieler das Item garnicht
        craftingChain.Clear();
        return false;
    }

    private bool IsInventoryNotFull(List<InventoryItem> craftingChain, InventoryItem itemToCraft)
    {
        // Methode die überprüft, ob das Inventar nach dem Verbrauchen der Zutaten trotzdem voll ist
        for (int i = 0; i < itemToCraft.units1; i++)
        {
            playerInventory.UseItem(craftingChain[0]);
        }
        if (craftingChain.Count > 1)
        {
            for (int i = 0; i < itemToCraft.units2; i++)
            {
                playerInventory.UseItem(craftingChain[1]);
            }
        }
        if (craftingChain.Count > 2)
        {
            for (int i = 0; i < itemToCraft.units3; i++)
            {
                playerInventory.UseItem(craftingChain[2]);
            }
        }
        // Ist Inventar nach Verbrauchen der Zutaten voll?
        if (!playerInventory.IsInventoryFull(itemToCraft))
        {
            //Nein => return Inventar ist nicht voll
            return true;
        } else
        {
            // Ja => füge verbrauchte Items wieder dem Inventar hinzu
            playerInventory.AddItem(craftingChain[0]);
            if (craftingChain.Count > 1)
            {
                playerInventory.AddItem(craftingChain[1]);
            }
            if (craftingChain.Count > 2)
            {
                playerInventory.AddItem(craftingChain[2]);
            }
            // return Inventar ist voll
            return false;
        }
    }

    //Infotexte im Crafting Menü für jedes Item

    public void MouseIsOverItemPic()
    {
        if (!mouseIsOver)
        {
            try
            {
                itemInfo.SetActive(true);
                itemInfoText.text = craftableItem.craftingText;
                itemLabelText.text = craftableItem.itemLabel;
            }
            catch (NullReferenceException ex)
            {
            }
            mouseIsOver = true;
        }
    }

    public void MouseIsNotOverItemPic()
    {
        if (mouseIsOver)
        {
            itemInfo.SetActive(false);
            mouseIsOver = false;
        }
    }
}

