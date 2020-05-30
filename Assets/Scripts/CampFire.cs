using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Klasse regelt die Interaktion mit dem Campfeuer //

public class CampFire : MonoBehaviour {

    ParticleSystem fireEffect;
    AudioSource audioSource;
    Ray ray;
    RaycastHit hit;
    Camera myCamera;
    Image handCursor;
    Image crosshairImage;
    Transform crosshairPos;
    Light light;
    GameObject player;
    PlayerHealth playerHealth;
    Player playerScript;
    public AudioClip getDamageSound;
    public AudioClip fireHitSound;
    public bool isFireOn = false;
    bool isPlayingDmgSound = false;
    Image warmIcon;
    Color transparent;
    Color notTransparent;
    bool isInArea = false;

    private void Awake()
    {
        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        handCursor = GameObject.Find("Hand").GetComponent<Image>();
        crosshairPos = GameObject.Find("Crosshair").GetComponent<Transform>();
        crosshairImage = GameObject.Find("Crosshair").GetComponent<Image>();
        player = GameObject.Find("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        light = GetComponentInChildren<Light>();
        fireEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        warmIcon = GameObject.Find("Warm").GetComponent<Image>();
        fireEffect.Stop();
        light.enabled = false;
        playerScript = player.GetComponent<Player>();
        transparent.a = 0.35f;
        notTransparent = new Color(1f, 1f, 1f, 1f);
        warmIcon.color = transparent;
    }
	
	// Update is called once per frame
	void Update () {
        ray = myCamera.ScreenPointToRay(crosshairPos.position);//Schicke jeden Frame einen Strahl raus
        fireDamage();
        interactWithCampFire();
    }

    public void startFire()
    {
        fireEffect.Play();
        audioSource.Play();
        light.enabled = true;
        playerScript.isInWarmingArea = true;
        warmIcon.color = notTransparent;
    }

    public void stopFire()
    {
        fireEffect.Stop();
        audioSource.Stop();
        light.enabled = false;
        playerScript.isInWarmingArea = false;
        warmIcon.color = transparent;
    }

    public void interactWithCampFire()
    {
        if (Physics.Raycast(ray, out hit, 1))//hat dieser Strahl etwas getroffen? MaxDistance muss niedrig (<=1) sein, weil es sonst möglich ist, diesen von außerhalb zu aktivieren
        {
            if (hit.collider.gameObject == gameObject)//war es dieses GO?
            {
                crosshairImage.enabled = false;//Blende Fadenkreuz aus
                handCursor.enabled = true; //Blende Hand-Cursor ein

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (!isFireOn)
                    {
                        startFire();
                        isFireOn = true;
                    } else
                    {
                        stopFire();
                        isFireOn = false;
                    }
                }
            }
        }
        else
        {
            handCursor.enabled = false;
            crosshairImage.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && isFireOn)
        {
            playerScript.setIsInFire(true);
            isInArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerScript.setIsInFire(false);
            isInArea = false;
        }
    }

    private void fireDamage()
    {
        if (playerScript.getIsInFire() && !isFireOn)
        {
            playerScript.setIsInFire(false);
        }
        if (playerScript.getIsInFire())
        {
            playerHealth.Damaging(Time.deltaTime * 15f);

            if (!isPlayingDmgSound)
            {
                AudioSource.PlayClipAtPoint(getDamageSound, player.transform.position);
                isPlayingDmgSound = true;
                StartCoroutine("WaitUntilFinishedClip");
            }
        }
    }

    protected IEnumerator WaitUntilFinishedClip()
    {
        yield return new WaitForSeconds(getDamageSound.length);
        isPlayingDmgSound = false;
    }
}
