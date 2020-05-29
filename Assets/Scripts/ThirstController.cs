using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;     //Einbinden des Namespaces mit den UI Klassen Image etc.

public class ThirstController : MonoBehaviour
{

    private static float thirst = 100; // darf man nur lesen von außen!
    private static float maxThirst = 100; // darf man nur lesen von außen!
    public static Image thirstBar;

    public GameObject thirstSprite;

    private void Awake()
    {
        thirstSprite = GameObject.Find("Thirst");
        thirstBar = thirstSprite.GetComponent<Image>();
    }

    // Use this for initialization
    void Start()
    {
    }


    public static void LowerThirst(float amount)   //Achtung!! LowerThirst vergrössert den Durst (unglückliche Bezeichnung)
    {
        thirst -= amount;
        if(thirst < 0)
        {
            thirst = 0;
        }
        thirstBar.fillAmount = thirst / maxThirst;
    }

    public static void RaiseThirst(float amount) //Achtung!! RaiseThirst verkleinert den Durst //Sollte mit dem was man trinkt skalieren
    {
        thirst += amount;
        if(thirst > maxThirst)
        {
            thirst = maxThirst;
        }
        thirstBar.fillAmount = thirst / maxThirst;
    }

    public static float getThirst()
    {
        return thirst;
    }

    public static float getMaxThirst()
    {
        return maxThirst;
    }
}
