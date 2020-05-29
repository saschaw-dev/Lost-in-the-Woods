using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : HealthController {

    public GameObject healthSprite;
    public Image healthBar;
    //Es gibt nur einen Spieler, deshalb static
    public static float playerMaxHealth = 100;
    public static float playerHealth = 100f;

    //private void Awake()
    //{
    //}

    // Use this for initialization
    void Start () {
        healthSprite = GameObject.Find("Health");
        healthBar = healthSprite.GetComponent<Image>();
    }

    //private void Update()
    //{
    //}

    public override void Damaging(float amount)//Überschreiben der Damaging Methode aus der Basisklasse HealthController
    {
        playerHealth -= amount;
        if(playerHealth < 0)
        {
            playerHealth = 0;
        }
        healthBar.fillAmount = playerHealth / playerMaxHealth;
    }

    public override void Dying()
    {
        // hier müssen später noch Werte gespeichert werden, sodass das Spiel fortgesetzt werden kann
        SceneManager.LoadScene("gameover");
    }

    //Healing Methode ist nicht in der Basisklasse vorhanden, da nur der Spieler die Möglichkeit der Heilung haben soll
    public void Healing(float amount)//Healing Methode wird z.B. beim Verwenden von Verbänden etc. aufgerufen
    {
        playerHealth += amount;
        if(playerHealth > playerMaxHealth)
        {
            playerHealth = playerMaxHealth;
        }
        healthBar.fillAmount = playerHealth / playerMaxHealth;
    }

    public void Testdead()//Tötet Spieler zum Testen
    {
        playerHealth = 0;
        healthBar.fillAmount = playerHealth / playerMaxHealth;
        Dying();
    }   
}
