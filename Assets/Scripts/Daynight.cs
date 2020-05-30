using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klasse regelt den Tag- und Nachtzyklus //

public class Daynight : MonoBehaviour {

    public Material[] sky = new Material[24]; //erzeugt einen neuen Array für die Skytextures
    WaitForSeconds breakTime= new WaitForSeconds(1);
    GameObject sun;
    Light sunlight;
    int i = 0;

	// Use this for initialization
	void Start ()
    {
        RenderSettings.skybox = sky[0];
        StartCoroutine(Countdown());
        sun = GameObject.Find("Directional Light");
        sunlight = sun.GetComponent<Light>();
        sunlight.intensity = 0.3F;
        sunlight.shadowStrength = 0;
     }
	
	// Update is called once per frame
	void Update () {
        

	}

    IEnumerator Countdown()
    {
        
        yield return breakTime;
        if (i < 23)
        {
            switch(i)
            {
                    case 0:                       //Nachts
                    sunlight.intensity = 0.3F;
                    sunlight.shadowStrength = 0;
                    break;
                    case 1:                       //Nachts
                    sunlight.intensity = 0.3F;
                    sunlight.shadowStrength = 0;
                    break;
                    case 2:                       //Nachts
                    sunlight.intensity = 0.3F;
                    sunlight.shadowStrength = 0;
                    break;
                    case 3:                       //Nachts
                    sunlight.intensity = 0.3F;
                    sunlight.shadowStrength = 0;
                    break;
                    case 4:                       //Nachts
                    sunlight.intensity = 0.3F;
                    sunlight.shadowStrength = 0;
                    break;
                    case 5:                       //Nachts
                    sunlight.intensity = 0.3F;
                    sunlight.shadowStrength = 0;
                    break;
                    case 6:                       //Nachts
                    sunlight.intensity = 0.3F;
                    sunlight.shadowStrength = 0;
                    break;
                    case 7:                       //Nachts
                    sunlight.intensity = 0.3F;
                    sunlight.shadowStrength = 0;
                    break;
                    case 8:                       //Nachts
                    sunlight.intensity = 0.3F;
                    sunlight.shadowStrength = 0;
                    break;
                    case 9:                       //Nachts
                    sunlight.intensity = 0.3F;
                    sunlight.shadowStrength = 0;
                    break;
                    case 10:                         
                    sunlight.intensity = 0.5F; //Sonnenaufgang
                    sunlight.shadowStrength = 0.3F;
                    break;
                    case 11:
                    sunlight.intensity = 0.7F;  //Sonnenaufgang2
                    sunlight.shadowStrength = 0.4F;
                    break;
                    case 12:
                    sunlight.intensity = 0.9F;  //Sonnenaufgang3
                    sunlight.shadowStrength = 0.7F;
                    break;
                    case 21:
                    sunlight.intensity = 0.9F;  //Sonnenuntergang
                    sunlight.shadowStrength = 0.7F;
                    break;
                    case 22:
                    sunlight.intensity = 0.7F;   //Sonnenuntergang2
                    sunlight.shadowStrength = 0.4F;
                    break;
                    case 23:
                    sunlight.intensity = 0.5F;  //Sonnenuntergang3
                    sunlight.shadowStrength = 0.3F;
                    break;
                    default:
                    sunlight.intensity = 1F;   //Tag
                    sunlight.shadowStrength = 1F;
                    break;
            }
            i += 1;
            
        }
        else
        {
            i = 0;
            StartCoroutine(Countdown());
        }
        RenderSettings.skybox = sky[i];
        StartCoroutine(Countdown());
    }
}
