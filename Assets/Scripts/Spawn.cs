using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basisklasse für das Spawnen von allen SCs und NSCs //

public class Spawn : MonoBehaviour {

    public int rndValue = 0;


    public virtual void SpawnAtPosition(int rndValue)
    {
        //wird unterschiedlich überschrieben je nachdem ob es sich um den Player oder einen NPC handelt der Spawnen soll
    }

    public virtual void Respawn()
    {
        //wird unterschiedlich überschrieben je nachdem ob es sich um den Player oder einen NPC handelt der Respawnen soll
    }

    public virtual int GetRndNumber(int rndValue) //Methode zum Würfeln einer Zufallszahl integer
    {
        return rndValue;
    }
}
