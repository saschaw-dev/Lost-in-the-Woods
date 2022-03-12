using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItem : PickableObject {

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
        myCamera = mainCamera.GetComponent<Camera>();
        crosshair = GameObject.Find("Crosshair");
        crosshairPos = crosshair.GetComponent<Transform>();
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        myInventory = player.GetComponent<Inventory>();
        hand = GameObject.Find("Hand");
        handCursor = hand.GetComponent<Image>();
        crosshairImage = crosshair.GetComponent<Image>();
        handCursor.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUp(myCamera.ScreenPointToRay(crosshairPos.position));
        }

        //Hand-Cursor wird sichtbar, wenn man mit der Maus über einem Objekt ist, das man aufheben kann

        ray2= myCamera.ScreenPointToRay(crosshairPos.position);//Schicke jeden Frame einen Strahl raus

        if (Physics.Raycast(ray2, out hit2,5))//hat dieser Strahl etwas getroffen?
        {
            if (hit2.collider.gameObject == gameObject)//war es dieses GO?
            {
                crosshairImage.enabled = false;//Blende Fadenkreuz aus
                handCursor.enabled = true; //Blende Hand-Cursor ein
            }
        }else
        {
            handCursor.enabled = false;
            crosshairImage.enabled = true;
        }
    }
          
      
    

    public override void PickUp(Ray ray)
    {
        if (Physics.Raycast(ray, out hit,5))
        {
            if (hit.collider.gameObject == gameObject)
            {
                

                if (myInventory.AddItem(inventoryItem))
                {

                    if (inventoryItem.picSound != null)
                    {
                        AudioSource.PlayClipAtPoint(inventoryItem.picSound, player.transform.position);
                    }
                    GameObject.Destroy(gameObject);
                }
            }

        }
    }
}
