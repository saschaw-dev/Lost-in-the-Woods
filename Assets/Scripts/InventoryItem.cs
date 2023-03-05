using System.Collections;
using UnityEngine;


public class InventoryItem : ScriptableObject
{ //Es handelt sich um eine Scriptable Object-Subklasse
  //speichert Daten zentral ausserhalb der Szene

    //Daten die jedes Item unseres Inventorys als Key mitgegeben wird

    public string itemName;
    public string itemLabel;
    public Sprite sprite;
    public AudioClip picSound;
    public GameObject prefab;
    public bool equiped = false;//Equiped Schalter
    public static bool lastEquiped = false;//Statischer Equiped Schalter

    //Spezielle Daten
    public float filling = 0; //Sättigungswert 0-100 (100 = ganz satt)
    public float damage = 0; //Schaden an Lebewesen 0-100 (100 = tod)
    public float blockDamage = 0; // Abbaugeschwindigkeit ist proportional zu Blockschaden
    public Animation useAnimation; //Animation die abgespielt wird, wenn Spieler Gegenstand in der Hand hält und Aktionstaste drückt
    public AnimationClip useAnima;
    public AudioClip useSound;


    //Kennung
    public int itemKindNumber;

    //Crafting Rezept
    public float craftingSpeed = 0f;
    public string craftingText = "";
    public int ingredient1;
    public int ingredient2;
    public int ingredient3;

    public int units1;
    public int units2;
    public int units3;

    //Baumodus
    public bool isBuildItem;

    /*Itemarten
     * Lebensmittel
     * -Essen 
     * -Trinken
     * Werkzeuge
     * -Axt
     * -Spitzhacke
     * Waffen
     * -Messer
     * -Bogen
     * Baumaterialien
     * -Holz
     * -Stein
     * Medizin
     * -Verband
     * -Pillen
     * */


}
