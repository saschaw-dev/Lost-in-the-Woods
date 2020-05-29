using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GetWood : PickableObject {

    
    GameObject equipedAxe;
    GameObject textGO;
    public GameObject[] woodLog = new GameObject[6];
    int Outcome = 15;
    Text text;
    string tipText = "Zum Abbauen wird eine Axt oder Spitzhacke benötigt!";
    public AudioClip treeFall;
    Animation fallingTree;
    GetStone getStone;
    Animation stoneAxeAnim;

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
        myCamera = mainCamera.GetComponent<Camera>();
        crosshair = GameObject.Find("Crosshair");
        crosshairPos = crosshair.GetComponent<Transform>();
        equipedAxe = GameObject.Find("EquipedAxe");
    }

    // Use this for initialization
    void Start()
    {
        textGO = GameObject.Find("Tips");
        text = textGO.GetComponent<Text>();
        fallingTree = GetComponent<Animation>();
        player = GameObject.Find("Player");
        stoneAxeAnim = equipedAxe.GetComponent<Animation>();

        for (int i = 0; i < 6; i++)
        {
            woodLog[i].SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !stoneAxeAnim.isPlaying)
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
            if (hit.collider.gameObject == gameObject)
            {
                if (equipedAxe.activeSelf==true)
                {

                    text.enabled = false;
                    Outcome -= 1;

                    if (inventoryItem.picSound != null)
                    {
                        AudioSource.PlayClipAtPoint(inventoryItem.picSound, player.transform.position);

                    }
                    if (Outcome == 6)
                    {
                        AudioSource.PlayClipAtPoint(treeFall, player.transform.position);
                        fallingTree.Play();

                    }if (Outcome <= 0)
                    {

                        GameObject.Destroy(gameObject);
                        for (int i = 0; i < 6; i++)
                        {
                            woodLog[i].SetActive(true);
                        }
                        

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
