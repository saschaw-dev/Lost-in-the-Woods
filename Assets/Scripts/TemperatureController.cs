using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Es soll nur Schäden durch Kälte geben, nicht durch Wärme
public class TemperatureController : MonoBehaviour {

    GameObject thermoGO;
    GameObject dayAndNightControllerGo;
    private float temperature = 100f; // darf man nur von außen lesen!
    private float maxTemperature = 100f; // darf man nur von außen lesen!
    static Image thermoImage;
    Image thermoFill;
    private float wetFreeze = 0.5f; // wenn Spieler nass ist
    private float nightFreeze = 0.5f; // wenn Nacht ist, und Spieler nicht unter einem Dach ist?
    private float warming = 0; // wenn im Lagerfeuer-TriggerCollider
    private int indoorFactor = 0;
    private int wetFactor = 0;
    DayAndNightControl dayAndNightControl;
    Player playerScript;
    public Sprite thermoFillCold;
    public Sprite thermoFillSprite;

    // blau:
    // Temperatur unter 50 => mehr Hunger
    // Temperatur unter 20 => Ohnmacht

    // Use this for initialization
    void Start () {
        dayAndNightControllerGo = GameObject.Find("Day and Night Controller");
        thermoGO = GameObject.Find("ThermoFill");
        thermoImage = thermoGO.GetComponent<Image>();
        thermoFill = GameObject.Find("ThermoFill").GetComponent<Image>();
        dayAndNightControl = dayAndNightControllerGo.GetComponent<DayAndNightControl>();
        playerScript = GameObject.Find("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
        CalculateTemperature();
	}

    public void CalculateTemperature()
    {
        // If-Abfragen
        if (playerScript.getIsIndoor())
        {
            indoorFactor = 0;
        } else
        {
            indoorFactor = 1;
        }
        if (playerScript.isWet)
        {
            wetFactor = 1;
        } else
        {
            wetFactor = 0;
        }
        if (playerScript.isInWarmingArea)
        {
            warming = 1f;
        } else
        {
            warming = 0;
        }
        if (dayAndNightControl.isCurrentlyNight())
        {
            nightFreeze = 0.5f;
        } else
        {
            nightFreeze = 0;
        }
        temperature = temperature - (wetFreeze * wetFactor * Time.deltaTime + nightFreeze * indoorFactor * Time.deltaTime);
        if (temperature < 0f)
        {
            temperature = 0;
        }
        temperature = temperature + (warming * Time.deltaTime);
        if (temperature > maxTemperature)
        {
            temperature = maxTemperature;
        }
        if (temperature < 50f)
        {
            thermoImage.GetComponentInChildren<Image>().sprite = thermoFillCold;
        } else
        {
            thermoImage.GetComponentInChildren<Image>().sprite = thermoFillSprite;
        }
        // if (temperature < 20) {
            // Spiele Frieren Sound ab
            // StartCoroutine  nach x Sekunden Ohnmacht
        //}
        // else {
        // Stoppe Coroutine wieder
        //}
        thermoImage.fillAmount = temperature / maxTemperature;
    }

    public float getTemperature()
    {
        return temperature;
    }

    public float getMaxTemperature()
    {
        return maxTemperature;
    }
}
