using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawning : Spawn {

    GameObject spawnPoint1;
    GameObject spawnPoint2;
    GameObject spawnPoint3;
    GameObject player;
    GameObject gameController;
    Transform sp1Pos;
    Transform sp2Pos;
    Transform sp3Pos;
    Transform playerPos;

    PlayerHealth playerHealth;
    PlayerStamina playerStamina;

    //private void Update()
    //{
    
    //}

    //private void Awake()//wird noch vor Start ausgeführt
    //{
    //}

    private void Start()
    {
        spawnPoint1 = GameObject.Find("SpawnPoint1");
        spawnPoint2 = GameObject.Find("SpawnPoint2");
        spawnPoint3 = GameObject.Find("SpawnPoint3");
        gameController = GameObject.Find("GameController");
        player = GameObject.Find("Player");
        sp1Pos = spawnPoint1.GetComponent<Transform>();
        sp2Pos = spawnPoint2.GetComponent<Transform>();
        sp3Pos = spawnPoint3.GetComponent<Transform>();
        playerPos = player.GetComponent<Transform>();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerStamina = player.GetComponent<PlayerStamina>();

        rndValue = GetRndNumber(rndValue);//Würfeln
    }

    public override void SpawnAtPosition(int rndValue)//Daten werden geladen
    {//wird beim Laden der Szene aufgerufen
        if (rndValue == 1)
        {
            playerPos.position = sp1Pos.position;
        }
        else if (rndValue == 2)
        {
            playerPos.position = sp2Pos.position;
        }
        else
        {
            playerPos.position = sp3Pos.position;
        }
    }

    public override void Respawn()//löscht bestimmte Daten z.B. manche Items
    {//wird nach Tod aufgerufen
        int rndValue = 0;
        rndValue = GetRndNumber(rndValue);

        if (rndValue == 1)
        {
            playerPos.position = sp1Pos.position;

        }
        else if (rndValue == 2)
        {
            playerPos.position = sp2Pos.position;
        }
        else
        {
            playerPos.position = sp3Pos.position;
        }
        //Mache Maus wieder aus
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Stats zurücksetzen
        playerHealth.Healing(100);//füllt hp UIanzeige wieder auf
        playerStamina.RaiseStamina(20);//füllt stamina und UIanzeige wieder auf
        HungerController.RaiseHunger(100f);
        ThirstController.RaiseThirst(100f);
        Player.isDying = false; 
        //GameOver Screen ausblenden
    }

    public override int GetRndNumber(int rndValue) //Methode zum Würfeln einer Zufallszahl integer
    {
        rndValue = Random.Range(1, 4);
        return rndValue;
    }
}
