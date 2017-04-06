using UnityEngine;
using System.Collections;

public class RespawnPoint : MonoBehaviour {

	public Animator animator;
	public GameObject player;
	public GameObject point;
	public GameController GameController;

	// Use this for initialization
	void Start () {
		// player = GameObject.Find("Player");

	}
	
	void OnTriggerEnter2D(Collider2D other) {

		// Debug.Log("other = "+other.name);

        if (other.tag == "PlayerVehicle"){

            animator.SetTrigger("Active");

            Vector2 pos = point.transform.position;

            player = GameController.CurrentVehicle;

            player.SendMessage("UpdateRespawnPoint", pos);


        }
    }

    
}
