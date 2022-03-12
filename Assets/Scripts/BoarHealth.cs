using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Klasse regelt die Gesundheitsverwaltung eines Wildschweins */

public class BoarHealth : HealthController {

    public static float maxHealth = 100f;
    public float boarHealth = 100f;

	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Damaging(float amount)
    {
        boarHealth -= amount;
        if (boarHealth < 0)
        {
            boarHealth = 0;
        }
    }
}
