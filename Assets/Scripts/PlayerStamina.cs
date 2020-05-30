using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Klasse regelt die Ausdauerverwaltung des Spielers //

public class PlayerStamina : StaminaController {

    public static Image staminaBar;
    public GameObject staminaSprite;

    // Use this for initialization
    void Start()
    {
        staminaSprite = GameObject.Find("Stamina");
        staminaBar = staminaSprite.GetComponent<Image>();
    }


    public override void LowerStamina(float amount)    //Methode wird aufgerufen, wenn der Spieler eine Aktion ausführt welche die Ausdauer reduziert z.B. Rennen
    {
        stamina -= amount;
        if(stamina < 0)
        {
            stamina = 0;
        }
        staminaBar.fillAmount = stamina / maxStamina;  //Ausdaueranzeige aktualisieren
    }

    public override void RaiseStamina(float amount) //Methode wird immer aufgerufen und regeneriert die Ausdauer vom Spieler. Wenn der Spieler gleichzeitig rennt, dann verliert er mehr Ausdauer als er durch diese Methode regeneriert.
    {
        stamina += amount;
        if(stamina > maxStamina)
        {
            stamina = maxStamina;
        }
        staminaBar.fillAmount = stamina / maxStamina; //Ausdauerleiste aktualisieren
    }
}
