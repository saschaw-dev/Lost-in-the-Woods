using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class day : MonoBehaviour {

    DayAndNightControl dayAndNightControl;
    public Material[] sky = new Material[5];
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        updateSkybox();
	}

    void updateSkybox()
    {
        if (dayAndNightControl.currentTime > 0f && dayAndNightControl.currentTime < 0.1f)
        {
            //Mitternacht
        }
        if (dayAndNightControl.currentTime < 0.5f && dayAndNightControl.currentTime > 0.1f)
        {
            //Morgen
            RenderSettings.skybox = sky[1];

        }
        if (dayAndNightControl.currentTime > 0.5f && dayAndNightControl.currentTime < 0.6f)
        {
            //Mittag
            RenderSettings.skybox = sky[2];
        }
        if (dayAndNightControl.currentTime > 0.6f && dayAndNightControl.currentTime < 0.8f)
        {
            //Abend
            RenderSettings.skybox = sky[3];

        }
        if (dayAndNightControl.currentTime > 0.8f && dayAndNightControl.currentTime < 1f)
        {
            //Nacht
            RenderSettings.skybox = sky[4];
        }

    }
}
