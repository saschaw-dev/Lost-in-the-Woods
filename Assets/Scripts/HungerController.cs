using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;     //Einbinden des Namespaces mit den UI Klassen Image etc.

public class HungerController : MonoBehaviour
{

    private static float hunger = 100; // darf man nur lesen von außen!
    private static float maxHunger = 100; // darf man nur lesen von außen!
    public static Image hungerBar;

    public GameObject hungerSprite;

    private void Awake()
    {
        hungerSprite = GameObject.Find("Hunger");
        hungerBar = hungerSprite.GetComponent<Image>();
    }

    // Use this for initialization
    void Start()
    {
    }


    public static void LowerHunger(float amount)   //Achtung!! LowerHunger vergrössert den Hunger (unglückliche Bezeichnung)
    {
        hunger -= amount;
        if(hunger < 0)
        {
            hunger = 0;
        }
        hungerBar.fillAmount = hunger / maxHunger;  
    }

    public static void RaiseHunger(float amount) //Achtung!! RaiseHunger verkleinert den Hunger //Sollte mit dem was man isst skalieren
    {
        hunger += amount;
        if(hunger > maxHunger)
        {
            hunger = maxHunger;
        }
        hungerBar.fillAmount = hunger / maxHunger; 
    }

    public static float getHunger()
    {
        return hunger;
    }

    public static float getMaxHunger()
    {
        return maxHunger;
    }
}
