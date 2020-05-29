using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSound : MonoBehaviour {

    AudioClip birdSound;
    AudioSource audioSource;
   

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        
    }
	
	// Update is called once per frame
	void Update () {

      
       if(!audioSource.isPlaying)
            audioSource.Play();
       
      
    }

    }

