using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject CurrentVehicle;
	public Vector2 respawnPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey("r")){
	        CurrentVehicle.SendMessage("Respawn");
	    }
	}

	public void RespawnPressed(){
		CurrentVehicle.SendMessage("Respawn");
	}


}
