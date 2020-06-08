using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klasse regelt die Gesundheitsverwaltung eines Wolfes //

public class WolfHealth : HealthController {

    public static float maxHealth = 100f;
    public float wolfHealth = 100f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Damaging(float amount)
    {
        wolfHealth -= amount;
        if (wolfHealth < 0)
        {
            wolfHealth = 0;
        }
    }
}
