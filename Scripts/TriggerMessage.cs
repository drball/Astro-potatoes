using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--shows a message when triggered, hides it again when out of trigger

public class TriggerMessage : MonoBehaviour {

	public GameObject Message;

	// Use this for initialization
	void Start () {
		Message.SetActive(false);
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		//--is the other trigger a vehicle? 

		if (other.tag == "Player" || other.tag == "PlayerVehicle"){
			
			Message.SetActive(true);
		} 

	}

	void OnTriggerExit2D(Collider2D other) {
		
		if (other.tag == "Player"){
			
			Message.SetActive(false);
		} 

	}
}
