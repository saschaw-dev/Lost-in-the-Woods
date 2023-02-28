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
    Animator deerAnimator;
    Animator boarAnimator;
    Animator wolfAnimator;
    bool switcher = false;
    float timer = 0f;

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
        deerAnimator = GameObject.Find("Deer").GetComponent<Animator>();
        boarAnimator = GameObject.Find("WildBoar").GetComponent<Animator>();
        wolfAnimator = GameObject.Find("Wolf").GetComponent<Animator>();
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
        if (Physics.Raycast(ray, out hit, 2f)) // Hat der Strahl etwas getroffen?
        {
            GameObject hitGO = hit.collider.gameObject;

            if (hitGO.GetComponent<WolfAI>() != null && !wolfAnimator.GetBool("isDying"))
            {
                hitGO.GetComponent<WolfAI>().getWeaponDamage(ip);
            }
            if (hitGO.GetComponent<WolfAI>() != null && ip.itemKindNumber == 7 && wolfAnimator.GetBool("isDying"))
            {
                // Wolf kann man nur mit dem Steinmesser abbauen
                hitGO.GetComponent<WolfAI>().getBlockDamage(ip);
            }
            if (hitGO.GetComponent<WildBoarAI>() != null && !boarAnimator.GetBool("isDying"))
            {
                hitGO.GetComponent<WildBoarAI>().getWeaponDamage(ip);
            }
            if (hitGO.GetComponent<WildBoarAI>() != null && ip.itemKindNumber == 7 && boarAnimator.GetBool("isDying"))
            {
                // Wildschwein kann man nur mit dem Steinmesser abbauen
                hitGO.GetComponent<WildBoarAI>().getBlockDamage(ip);
            }
            if (ip.damage > 0 && !deerAnimator.GetBool("isDying"))
            {
                if (hitGO.GetComponent<DeerAI>() != null)
                {
                    hitGO.GetComponent<DeerAI>().getWeaponDamage(ip);
                }
            }
            if (hitGO.GetComponent<DeerAI>() != null && ip.itemKindNumber == 7 && deerAnimator.GetBool("isDying"))
            {
                // Reh kann man nur mit dem Steinmesser abbauen
                hitGO.GetComponent<DeerAI>().getBlockDamage(ip);
            }
            ray = new Ray(); // Überschreiben des Rays damit getHit nur einmal ausgeführt wird!
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
                if (ip.itemName == "Mushroom" || ip.itemName == "meat")//falls es sich um Pilze oder rohes Fleisch handelt, Giftschaden an Selbst
                {
                    playerHealth.Damaging(ip.damage);
                }

                HungerController.RaiseHunger(ip.filling);//Sättigen      
                playerInventory.UseItem(ip);//Verringere Itemzahl um 1
                OnAllUnitsAreUsed(ip);
                switcher = true;
            }
            if(ip.isBuildItem && this.buildScript.GetIsBuildModeOn()) // falls es sich um ein Baumodus-Objekt handelt
            {
                this.buildScript.ReplaceWithRealBlock();
                this.playerInventory.UseItem(ip);
                OnAllUnitsAreUsed(ip);
                switcher = true;
            }
            if (ip.damage > 0 || ip.blockDamage > 0) // falls es sich um eine Waffe oder Werkzeug handelt
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
            if(!playerInventory.slotsContainsItem(ip))
            {
                if(handImage)
                handImage.sprite = null;//Entferne Item Pic aus Hand Pic
                InventoryItem.lastEquiped = false;//Setze Equiped wieder auf False
                gameObject.SetActive(false);//Entferne Item GO aus der Hand
                buildScript.SetIsBuildModeOn(false);
                buildScript.CancelBlockPlacement();
            }
        }
        else if (!playerInventory.slotsContainsItem(ip) && myAnimation.isPlaying == false)//Wenn Animation fertig gespielt und letztes Item verbraucht
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
                if(playerInventory.slotsContainsOnlyFullStacksOfItem(ip))
                {
                    switcher = false;
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
            else if (playerInventory.slotsContainsOnlyFullStacksOfItem(ip) && myAnimation.isPlaying == false)//Wenn aktiver Stapel verbraucht ist und Animation fertig gespielt
            {
                switcher = false;
                handImage.sprite = null;//Entferne Item Pic aus Hand Pic
                InventoryItem.lastEquiped = false;//Setze Equiped wieder auf False
                gameObject.SetActive(false);//Entferne Item GO aus der Hand
            }
        }
    }

}
