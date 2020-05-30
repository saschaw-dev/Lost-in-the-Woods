using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Klasse regelt das Draggen von Itempics im InventarUI //

public class DragItem : MonoBehaviour, IDragHandler, IEndDragHandler {

    RectTransform panelRectTransform;
    GameObject follower;

    void Awake()
    {
        follower = GameObject.Find("Follower");
    }

    // Use this for initialization
    void Start () {
        panelRectTransform = gameObject.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnDrag(PointerEventData eventData)
    {
        enableFollowerImage(eventData);
        eventData.pointerDrag.GetComponent<Image>().enabled = false;
        // lässt stattdessen das zehnte ItemImage die Maus folgen, da dieser im Layer über allen anderen steht
        follower.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        disableFollowerImage();
        eventData.pointerDrag.GetComponent<Image>().enabled = true;
        transform.localPosition = Vector3.zero;   
    }

    private void enableFollowerImage(PointerEventData eventData)
    {
        follower.SetActive(true);
        follower.GetComponent<Image>().enabled = true;
        follower.GetComponent<Image>().sprite = eventData.pointerDrag.GetComponent<Image>().sprite;
    }

    private void disableFollowerImage()
    {
        follower.SetActive(false);
        follower.GetComponent<Image>().enabled = false;
        follower.transform.position = Vector3.zero;
    }
}
