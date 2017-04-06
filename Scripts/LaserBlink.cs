using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBlink : MonoBehaviour {

	public float blinkRate = 3f;
	public float delay = 0f;
	public GameObject laser;
	public ParticleSystem sparks;
	private float initialParticleEmission;

	// Use this for initialization
	void Start () {
		InvokeRepeating("Blink", delay, blinkRate);
		initialParticleEmission = sparks.emissionRate;
	}

	void Blink() {
		if(laser.activeSelf == true){
	    	laser.SetActive(false);
	    	sparks.emissionRate = 0;
	    	// Debug.Log("set laser to off");
		}else {
			laser.SetActive(true);
			sparks.emissionRate = initialParticleEmission;
			// Debug.Log("set laser to on");
		}

	}
}

