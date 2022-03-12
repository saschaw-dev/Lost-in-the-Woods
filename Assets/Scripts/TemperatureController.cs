using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Klasse regelt die Temperatur des Spielers //

public class TemperatureController : MonoBehaviour {

    GameObject thermoGO;
    GameObject dayAndNightControllerGo;
    private float temperature = 100f; // darf man nur von außen lesen!
    private float maxTemperature = 100f; // darf man nur von außen lesen!
    static Image thermoImage;
    Image thermoFill;
    private float wetFreeze = 0; // wenn Spieler nass ist
    private float nightFreeze = 0.5f; // wenn Nacht ist, und Spieler nicht unter einem Dach ist?
    private int indoorFactor = 0;
    private float temperatureDecrease = 0f;
    private float temperatureIncrease = 0; // wenn im Lagerfeuer-Wärmebereich
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
        updateTemperature();
	}

    public void updateTemperature()
    {
        // Es soll nur Schäden durch Kälte geben, nicht durch Wärme

        // Weise den Temperaturvariablen Werte zu //
        if (playerScript != null)
        {

            if (playerScript.getIsIndoor())
            {
                indoorFactor = 0;
            } else
            {
                indoorFactor = 1;
            }
            if (playerScript.isWet)
            {
                wetFreeze = 0.5f;
            } else
            {
                wetFreeze = 0;
            }
            if (playerScript.isInWarmingArea)
            {
                temperatureIncrease = 1f;
            } else
            {
                temperatureIncrease = 0;
            }
            if (dayAndNightControl.isCurrentlyNight())
            {
                nightFreeze = 0.5f;
            } else
            {
                nightFreeze = 0;
            }

            temperatureDecrease = wetFreeze + nightFreeze * indoorFactor;

            if (temperature < 0f)
            {
                temperature = 0;
            }

            // Berechne die aktuelle Temperatur //

            temperature = temperature - temperatureDecrease * Time.deltaTime + temperatureIncrease * Time.deltaTime;

            if (temperature > maxTemperature)
            {
                temperature = maxTemperature;
            }

            // Aktualisiere das UI-Thermometer entsprechend der Temperatur //

            updateThermometerUIState(temperature);
        }
    }

    public float getTemperature()
    {
        return temperature;
    }

    public float getMaxTemperature()
    {
        return maxTemperature;
    }

    private void updateThermometerUIState(float temperature)
    {
        if (temperature < 50f)
        {
            // thermoGO.GetComponentInChildren<Image>().image = thermoFillCold;
        }
        else
        {
            //thermoImage.GetComponentInChildren<Image>().sprite = thermoFillSprite;
        }
        // thermoImage.fillAmount = temperature / maxTemperature;
    }
}
