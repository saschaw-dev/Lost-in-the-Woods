using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creek : MonoBehaviour {

    GameObject player;
    Image wetIcon;
    Player playerScript;
    float waitTimeInSeconds = 15f;
    Color transparent;
    Color notTransparent;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        wetIcon = GameObject.Find("Wet").GetComponent<Image>();
        transparent.a = 0.35f;
        notTransparent = new Color(1f, 1f, 1f, 1f);
        wetIcon.color = transparent;
    }
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            // Spieler ist nass
            if (!playerScript.isWet)
            {
                playerScript.isWet = true;
            } else
            {
                // Spieler ist vor Ablauf der Coroutine wieder ins Wasser und raus aus dem Wasser
                // => Abbruch der alten Coroutine und neustart, damit die Nasszeit zurückgesetzt wird
                StopCoroutine("setIsWet");
            }
            wetIcon.color = notTransparent;
            StartCoroutine("setIsWet");
        }
    }

    protected IEnumerator setIsWet()
    {
        yield return new WaitForSeconds(waitTimeInSeconds);
        playerScript.isWet = false; // Spieler ist wieder trocken
        wetIcon.color = transparent;
    }
}
