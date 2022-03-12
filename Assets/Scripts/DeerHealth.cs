using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klasse regelt die Gesundheitsverwaltung eines Rehs //

public class DeerHealth : HealthController {

    public static float maxHealth = 100f;
    public float deerHealth = 100f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Damaging(float amount)
    {
        deerHealth -= amount;
        if (deerHealth < 0)
        {
            deerHealth = 0;
        }
    }
}
