using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StaminaController : MonoBehaviour {

    //Statische Variablen die für alle Objekte (z.B.Spieler,Gegner,NPC..) gleich sind
    public static float maxStamina = 100;
    //
    public float stamina = 100;
    

    public virtual void LowerStamina(float amount)    //Methode wird aufgerufen, wenn der Spieler oder NPC eine Aktion ausführt welche die Ausdauer reduziert z.B. Rennen
    {
        
    }

    public virtual void RaiseStamina(float amount) //Methode wird immer aufgerufen und regeneriert die Ausdauer. Wenn der Spieler gleichzeitig rennt, dann verliert er mehr Ausdauer als er durch diese Methode regeneriert.
    {
      
    }
}
