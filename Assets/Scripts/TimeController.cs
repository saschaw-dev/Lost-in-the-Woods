using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Berechnet eigenes deltaTime, welches auch bei pausiertem Spiel funktioniert (mit TimeController.deltaTime nutzbar!)
public class TimeController : MonoBehaviour {

    private static float timeSinceLastFrame = 0f; // Zeit die vom Spielstart bis zum vorherigen Frame vergangen ist
    public static float deltaTime = 0f;

	// Use this for initialization
	void Start () {
        timeSinceLastFrame = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
        deltaTime = Time.realtimeSinceStartup - timeSinceLastFrame;
        timeSinceLastFrame = Time.realtimeSinceStartup;
	}
}
