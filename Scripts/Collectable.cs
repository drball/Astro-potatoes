using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--delete this object when player touches it
//--requires a box collider set to trigger

public class Collectable : MonoBehaviour {

	public TestScript script;
	
	void OnTriggerEnter2D(Collider2D other) {

		if (other.tag == "Player"){
			gameObject.SendMessage("Collected");
			Destroy(gameObject);
		}
	}
}
