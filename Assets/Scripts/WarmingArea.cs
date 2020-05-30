using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Klasse regelt das Betreten und Verlassen des Wärmeradius //

public class WarmingArea : MonoBehaviour {

    GameObject playerGO;
    Player player;
    CampFire campFire;
    Image warmIcon;
    Color transparent;
    Color notTransparent;

	// Use this for initialization
	void Start () {
        playerGO = GameObject.Find("Player");
        warmIcon = GameObject.Find("Warm").GetComponent<Image>();
        player = playerGO.GetComponent<Player>();
        transparent.a = 0.35f;
        notTransparent = new Color(1f, 1f, 1f, 1f);
        warmIcon.color = transparent;
        campFire = gameObject.GetComponentInParent<CampFire>();
	}
	
	// Update is called once per frame
	void Update () {
        fireWarming();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerGO && campFire.isFireOn)
        {
            player.isInWarmingArea = true;
            warmIcon.color = notTransparent;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerGO)
        {
            player.isInWarmingArea = false;
            warmIcon.color = transparent;
        }
    }
}
