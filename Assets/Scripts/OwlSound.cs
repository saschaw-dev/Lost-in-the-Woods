using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlSound : NightSound {

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        dayAndNightControl = GameObject.Find("Day and Night Controller").GetComponent<DayAndNightControl>();
        base.waitTimeInSeconds = 20f;
        base.StartCoroutine("playSoundEffectInLoop");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
