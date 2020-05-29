using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour {

    GameObject mainCamera;
    GameObject crosshair;
    GameObject player;
    GameObject hand;

    Camera myCamera;
    Transform crosshairPos;
    Image crosshairImage;
    Image handCursor;
    RaycastHit hit;
    Ray ray;
    Animation myAnimationComponent;
    AnimationClip doorOpenAnim;
    AnimationClip doorCloseAnim;
    public AudioClip doorSound;
    bool doorHasBeenHit = false;
    bool isDoorOpen = false;

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
        myCamera = mainCamera.GetComponent<Camera>();
        crosshair = GameObject.Find("Crosshair");
        crosshairPos = crosshair.GetComponent<Transform>();
    }

    // Use this for initialization
    void Start () {
        hand = GameObject.Find("Hand");
        handCursor = hand.GetComponent<Image>();
        crosshairImage = crosshair.GetComponent<Image>();
        handCursor.enabled = false;
        myAnimationComponent = GetComponentInParent<Animation>();
        doorOpenAnim = myAnimationComponent.GetClip("Door_Open");
        doorCloseAnim = myAnimationComponent.GetClip("Door_Close");

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (doorHasBeenHit)
            {
                interactWithDoor();
            }
        }

        ray = myCamera.ScreenPointToRay(crosshairPos.position);//Schicke jeden Frame einen Strahl raus

        if (Physics.Raycast(ray, out hit, 5))//hat dieser Strahl etwas getroffen?
        {
            if (hit.collider.gameObject == gameObject)//war es dieses GO?
            {
                crosshairImage.enabled = false;//Blende Fadenkreuz aus
                handCursor.enabled = true; //Blende Hand-Cursor ein
                doorHasBeenHit = true;
            }
        }
        else
        {
            handCursor.enabled = false;
            crosshairImage.enabled = true;
            doorHasBeenHit = false;
        }
    }

    public void interactWithDoor()
    {
        if (!isDoorOpen)
        {
            // Öffne die Türe
            myAnimationComponent.clip = doorOpenAnim;
            myAnimationComponent.Play();
            AudioSource.PlayClipAtPoint(doorSound, gameObject.transform.position);
            isDoorOpen = true;
        } else
        {
            // Schließe die Türe
            myAnimationComponent.clip = doorCloseAnim;
            myAnimationComponent.Play();
            AudioSource.PlayClipAtPoint(doorSound, gameObject.transform.position);
            isDoorOpen = false;
        }
    }
}
