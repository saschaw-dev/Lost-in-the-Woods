using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warming : MonoBehaviour {

    GameObject player;
    Player playerScript;
    CampFire campFireScript;
    Image warmIcon;
    Color transparent;
    Color notTransparent;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        warmIcon = GameObject.Find("Warm").GetComponent<Image>();
        playerScript = player.GetComponent<Player>();
        transparent.a = 0.35f;
        notTransparent = new Color(1f, 1f, 1f, 1f);
        warmIcon.color = transparent;
        campFireScript = gameObject.GetComponentInParent<CampFire>();
	}
	
	// Update is called once per frame
	void Update () {
        fireWarming();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && campFireScript.isFireOn)
        {
            playerScript.isInWarmingArea = true; // Player is in warming area
            warmIcon.color = notTransparent;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerScript.isInWarmingArea = false; // Player is not in warming Area
            warmIcon.color = transparent;
        }
    }

    private void fireWarming()
    {
        if (playerScript.isInWarmingArea && !campFireScript.isFireOn)
        {
            playerScript.isInWarmingArea = false; /* falls der Spieler noch im Collider des Lagerfeuers steht, und Player.isInFire auf "true" ist, 
            aber das Feuer schon ausgeschaltet wurde, 
            dann setze Player.isInFire auf "false"*/
            warmIcon.color = transparent;
        }
    }

}
