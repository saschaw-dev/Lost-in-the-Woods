using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickableObject : MonoBehaviour {

    protected GameObject crosshair;
    public GameObject player;
    public GameObject mainCamera;
    public GameObject hand;

    public InventoryItem inventoryItem;
    protected Text text1;
    protected RaycastHit hit;
    protected RaycastHit hit2;
    public Camera myCamera;
    protected Transform crosshairPos;
    protected Ray ray;
    protected Ray ray2;
    public Inventory myInventory;
    public Image handCursor;
    public Image crosshairImage;



    

    public virtual void PickUp(Ray ray)
    {
        //Muss von den Subklassen implementiert werden
        //Subklassen sind z.B. GetItem, GetWood, GetStone etc.
    }

    public virtual void DisableText()
    {

    }
}




           
      