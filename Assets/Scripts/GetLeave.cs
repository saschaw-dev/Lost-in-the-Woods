using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Klasse regelt das Abbauen und Aufnehmen von Blättern //

public class GetLeave : PickableObject
{ //Dieses Skript wird den Sträuchern angehängt

    AudioSource myAudioSource;
    public GameObject progressChart;
    GameObject progressGO;
    Image progressImage;
    bool switcher = false;
    float progress = 0;
    float maxProgress = 100;
    int randomNumber = 0;

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
        myCamera = mainCamera.GetComponent<Camera>();
        crosshair = GameObject.Find("Crosshair");
        crosshairPos = crosshair.GetComponent<Transform>();
        progressChart = GameObject.Find("ProgressChart");
        progressGO = GameObject.Find("Progress");
        progressImage = progressGO.GetComponent<Image>();
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        myInventory = player.GetComponent<Inventory>();
        hand = GameObject.Find("Hand");
        handCursor = hand.GetComponent<Image>();
        crosshairImage = crosshair.GetComponent<Image>();
        handCursor.enabled = false;
        progressChart.SetActive(false);
        myAudioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (switcher)
        {
            fillChart();//Fortschrittsbalken auffüllen
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUp(myCamera.ScreenPointToRay(crosshairPos.position));
        }

        //Hand-Cursor wird sichtbar, wenn man mit der Maus über einem Objekt ist, das man aufheben kann
        ray2 = myCamera.ScreenPointToRay(crosshairPos.position);//Schicke jeden Frame einen Strahl raus

        if (Physics.Raycast(ray2, out hit2, 5))//hat dieser Strahl etwas getroffen?
        {
            if (hit2.collider.gameObject == gameObject)//war es dieses GO?
            {
                crosshairImage.enabled = false;//Blende Fadenkreuz aus
                handCursor.enabled = true; //Blende Hand-Cursor ein
            }
        }
        else
        {
            handCursor.enabled = false;
            crosshairImage.enabled = true;
        }
    }




    public override void PickUp(Ray ray)
    {
        if (Physics.Raycast(ray, out hit, 5))
        {
            if (hit.collider.gameObject == gameObject)
            {
                progressChart.SetActive(true);
                progressGO.SetActive(true);
                progressChart.transform.position = crosshairPos.position;
                progressGO.transform.position = crosshairPos.position;
                switcher = true;
                myAudioSource.Play();      
            }
        }
    }

    void fillChart()
    {
        if (progress < maxProgress)
        {
            progress=progress+20*Time.deltaTime;
            if (progress >= maxProgress)
            {
                progress = maxProgress;
            }
            progressImage.fillAmount = progress / maxProgress;
        }
        else
        {
            progress = maxProgress;
            progressImage.fillAmount = maxProgress;
            switcher = false;
            myAudioSource.Stop();
            if (myInventory.AddItem(inventoryItem))//Füge ein Blatt dem Inventar hinzu
            {
                randomNumber = getRndNumber();

                for (int i = 0; i < randomNumber; i++)
                {
                    myInventory.AddItem(inventoryItem);//Füge Blätter hinzu abhängig von der gewürfelten Zufallszahl (1-7 Blätter)
                }
                //Es werden also insgesamt 2 bis 8 Blätter pro Busch gedroppt
                if (inventoryItem.picSound != null)
                {
                    AudioSource.PlayClipAtPoint(inventoryItem.picSound, player.transform.position);
                }
                progressChart.SetActive(false);
                GameObject.Destroy(gameObject);
            }
        }
    }

    int getRndNumber()
    {
        int rndNumber;
        rndNumber = Random.Range(1, 8);
        return rndNumber;
    }
}
