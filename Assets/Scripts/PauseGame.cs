using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klasse regelt das Pausieren des Spiels //

public class PauseGame : MonoBehaviour {

    public static bool isPaused = false;
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void toggleTimeScale()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
        } else
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }
}
