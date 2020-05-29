using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightSound : MonoBehaviour {

    protected AudioSource audioSource;
    protected DayAndNightControl dayAndNightControl;
    protected float waitTimeInSeconds; // wird von Kind-Klasse gesetzt

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected IEnumerator playSoundEffectInLoop()
    {
        // Ist es entweder Nacht oder Mitternacht?
        if (dayAndNightControl.isCurrentlyNight())
        {
            // Dann spiele Nachtgeräuche
            audioSource.Play();
        }
        yield return new WaitForSeconds(waitTimeInSeconds);
        StartCoroutine("playSoundEffectInLoop"); // Rekursiver Aufruf
    }
}
