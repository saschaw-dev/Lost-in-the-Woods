using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CricketSound : NightSound
{
    GameObject dayAndNightController;
    
    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dayAndNightController = GameObject.Find("Day and Night Controller");
        if(dayAndNightController != null) {
            dayAndNightControl = dayAndNightController.GetComponent<DayAndNightControl>();
        }
        base.waitTimeInSeconds = 10f;
        base.StartCoroutine("playSoundEffectInLoop");
    }


    // Update is called once per frame
    void Update()
    {
    }
}
