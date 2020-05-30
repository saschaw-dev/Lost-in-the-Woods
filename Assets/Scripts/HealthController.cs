using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Basisklasse für die Gesundheitsverwaltung aller SCs (=Spielercharaktere) und NSCs (=Nichtspielercharaktere) im Spiel //

public class HealthController : MonoBehaviour {

    //Health unterscheidet sich nach Objekt (z.B. kann Spieler eine andere Gesundheit haben als Npc)
    public float health = 100;
    

  

	// Use this for initialization
	void Start () {
        
	}
	

    public virtual void Damaging(float amount)
    {
       
    }

    public virtual void Dying()
    {
       
    }
}
