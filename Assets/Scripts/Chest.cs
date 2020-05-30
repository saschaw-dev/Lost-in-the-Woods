﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Klasse regelt die Interaktion mit einer Truhe //

public class Chest : MonoBehaviour {

    GameObject handSymbol;
    GameObject crosshair;
    Inventory chestInventory;
    Image handCursor;
    Image crosshairImage;
    Ray ray;
    RaycastHit hit;
    Transform crosshairPos;
    Camera myCamera;
    public AudioClip chestOpeningClosing;

    private void Awake()
    {
        crosshair = GameObject.Find("Crosshair");
        handSymbol = GameObject.Find("Hand");
        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        handCursor = handSymbol.GetComponent<Image>();
        crosshairPos = crosshair.GetComponent<Transform>();
        crosshairImage = crosshair.GetComponent<Image>();
    }

    // Use this for initialization
    void Start () {
        chestInventory = gameObject.GetComponent<Inventory>();
	}
	
	// Update is called once per frame
	void Update () {
        ray = myCamera.ScreenPointToRay(crosshairPos.position);//Schicke jeden Frame einen Strahl raus
        interactWithChest();
    }

    public void interactWithChest()
    {
        if (Physics.Raycast(ray, out hit, 5))//hat dieser Strahl etwas getroffen?
        {
            if (hit.collider.gameObject == gameObject)//war es dieses GO?
            {
                crosshairImage.enabled = false;//Blende Fadenkreuz aus
                handCursor.enabled = true; //Blende Hand-Cursor ein

                if (Input.GetKeyDown(KeyCode.E))
                {
                    chestInventory.OpenOrCloseInventory();
                    AudioSource.PlayClipAtPoint(chestOpeningClosing, gameObject.transform.position);
                }
            }
        }
        else
        {
            handCursor.enabled = false;
            crosshairImage.enabled = true;
        }
    }
}
