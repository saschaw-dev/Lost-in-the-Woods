using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// Klasse regelt die Nutzung / den Verbrauch von Gegenständen in der Spielerhand //

public class ItemUse : MonoBehaviour {

    GameObject player;
    GameObject hand;
    Player playerScript;
    AudioSource myAudioSource;
    AudioClip myAudioClip;
    Transform playerPos;
    RaycastHit hit;
    Ray ray;
    Image handImage;
    Build buildScript;
    Camera myCamera;
    Transform crosshairPos;
    WaitForSeconds breakTime = new WaitForSeconds(3);

    public Animation myAnimation;
    public InventoryItem ip;
    Inventory playerInventory;
    PlayerHealth playerHealth;
    bool switcher = false;

    private void Awake()
    {
        hand = GameObject.Find("HandImage");      
        handImage = hand.GetComponent<Image>();  
        player = GameObject.Find("Player");
        playerInventory = player.GetComponent<Inventory>(); 
    }

    // Use this for initialization
    void Start() {
        myAnimation = GetComponent<Animation>();
        playerPos = player.GetComponent<Transform>();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerScript = player.GetComponent<Player>();
        myAudioSource = GetComponent<AudioSource>();
        myAudioClip = ip.useSound;
        buildScript = player.GetComponentInChildren<Build>();
        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        crosshairPos = GameObject.Find("Crosshair").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update() {
        UseItemOnMouseLeft();
        OnAllUnitsAreUsed(ip);
        UseItemOnMouseRight();
        if (switcher)
        {
            OnStackIsUsed();
        }
        if (Physics.Raycast(ray, out hit, 1)) // Hat der Strahl etwas getroffen?
        {
            if (hit.collider.gameObject.GetComponent<WolfAI>() != null)
            {
                hit.collider.gameObject.GetComponent<WolfAI>().getHit(ip);
            }
        }
    }
    
    void UseItemOnMouseLeft()
    {
        if (Input.GetMouseButtonDown(0) && !Cursor.visible)
        {
            if (myAnimation)
            {
                myAnimation.Play();
            }
            if (myAudioClip != null) AudioSource.PlayClipAtPoint(myAudioClip, player.transform.position);//Spiele Use Item Sound ab

            if (ip.filling > 0)//falls es sich um Essen handelt
            {
                if (ip.itemName == "Mushroom")//falls es sich um Pilze handelt Giftschaden an Selbst
                {
                    playerHealth.Damaging(ip.damage);
                }

                HungerController.RaiseHunger(ip.filling);//Sättigen      
                playerInventory.UseItem(ip);//Verringere Itemzahl um 1
                OnAllUnitsAreUsed(ip);
                switcher = true;//Schalter an
            }
            if(ip.isBuildItem && this.buildScript.GetIsBuildModeOn()) // falls es sich um ein Baumodus-Objekt handelt
            {
                this.buildScript.ReplaceWithRealBlock();
                this.playerInventory.UseItem(ip);
                OnAllUnitsAreUsed(ip);
                switcher = true;//Schalter an
            }
            if (ip.damage > 0) // falls es sich um eine Waffe handelt
            {
                ray = myCamera.ScreenPointToRay(crosshairPos.position);
            }
                
        }
    }

    void UseItemOnMouseRight()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if (ip.isBuildItem && this.buildScript.GetIsBuildModeOn()) // falls es sich um ein Baumodus-Objekt handelt
            {
                buildScript.RotateBlockOnMouseClick();
            }
        }
    }

    private void OnAllUnitsAreUsed(InventoryItem ip)//Item aus Hand entfernen
    {
        /* falls es sich um ein GO handelt, welches keine Animation hat, wie z.B. Bauitems
         */
        if (!myAnimation)
        {
            if(!playerInventory.items.ContainsKey(ip) || playerInventory.items[ip] == 0)
            {
                if(handImage)
                handImage.sprite = null;//Entferne Item Pic aus Hand Pic
                InventoryItem.lastEquiped = false;//Setze Equiped wieder auf False
                gameObject.SetActive(false);//Entferne Item GO aus der Hand
                buildScript.SetIsBuildModeOn(false);
                buildScript.CancelBlockPlacement();
            }
        }
        else if ((!playerInventory.items.ContainsKey(ip) && myAnimation.isPlaying == false) || 
            (playerInventory.items[ip] == 0 && myAnimation.isPlaying == false))//Wenn Animation fertig gespielt und letztes Item verbraucht
        {
            handImage.sprite = null;//Entferne Item Pic aus Hand Pic
            InventoryItem.lastEquiped = false;//Setze Equiped wieder auf False
            gameObject.SetActive(false);//Entferne Item GO aus der Hand
        }
    }

    private void OnStackIsUsed()//Item aus Hand entfernen 
    {
        // falls es sich um ein GO handelt, welches keine Animation hat, wie z.B. Bauitems
        if(ip)
        {
            if (!myAnimation)
            {
                if(playerInventory.items[ip] % 10 == 0)
                {
                    switcher = false;//Schalter aus
                    handImage.sprite = null;//Entferne Item Pic aus Hand Pic
                    InventoryItem.lastEquiped = false;//Setze Equiped wieder auf False
                    gameObject.SetActive(false);//Entferne Item GO aus der Hand
                    if (ip.isBuildItem)
                    {
                        buildScript.SetIsBuildModeOn(false);
                        buildScript.CancelBlockPlacement();
                    }
                }
            }
            else if (playerInventory.items[ip] % 10 == 0 && myAnimation.isPlaying == false)//Wenn aktiver Stapel verbraucht ist und Animation fertig gespielt
            {
                switcher = false;//Schalter aus
                handImage.sprite = null;//Entferne Item Pic aus Hand Pic
                InventoryItem.lastEquiped = false;//Setze Equiped wieder auf False
                gameObject.SetActive(false);//Entferne Item GO aus der Hand
            }
        }
    }

}
