using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// Klasse regelt das Abbauen und Aufnehmen von Stein //

public class GetStone : PickableObject
{ 
    GameObject equipedStonePickaxe;
    public GameObject textGO;
    public GameObject[] smallRock = new GameObject[6];
    public Text text;

    string tipText = "Zum Abbauen wird eine Axt oder Spitzhacke benötigt!";
    int Outcome = 10;
    AudioClip stoneBreak;
    Animation pickaxeAnim;

    private void Awake()
    {
        equipedStonePickaxe = GameObject.Find("EquipedStonePickaxe");
        mainCamera = GameObject.Find("Main Camera");
        myCamera = mainCamera.GetComponent<Camera>();
        crosshair = GameObject.Find("Crosshair");
        crosshairPos = crosshair.GetComponent<Transform>();
    }

    // Use this for initialization
    void Start()
    {
        textGO = GameObject.Find("Tips");
        text = textGO.GetComponent<Text>();
        player = GameObject.Find("Player");
        myInventory = player.GetComponent<Inventory>();
        pickaxeAnim = equipedStonePickaxe.GetComponent<Animation>();

        for (int i = 0; i < 6; i++)
        {
            smallRock[i].SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !pickaxeAnim.isPlaying)
        {       
            PickUp(myCamera.ScreenPointToRay(crosshairPos.position));
        }     
        if (Input.GetKeyDown(KeyCode.Delete)) {
            DisableText();
        }
    }

    public override void PickUp(Ray ray)
    {
        if (Physics.Raycast(ray, out hit, 1))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                if (equipedStonePickaxe.activeSelf == true)
                {
                    text.enabled = false;
                    Outcome -= 1;

                    if (inventoryItem.picSound != null)
                    {
                        AudioSource.PlayClipAtPoint(inventoryItem.picSound, player.transform.position);
                    }
                    if (Outcome <= 0)
                    {
                        GameObject.Destroy(gameObject);

                        for (int i = 0; i < 6; i++)
                        {                          
                                smallRock[i].SetActive(true);                      
                        }
                        AudioSource.PlayClipAtPoint(stoneBreak, player.transform.position);
                    }
                }
                else
                {
                    text.enabled = true;
                    text.text = tipText;
                }
            }
        }
    }

    public override void DisableText()
    {
        if (text != null && text.enabled == true)
        {
            text.enabled = false;
        }
    }
}
