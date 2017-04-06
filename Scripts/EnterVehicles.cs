using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--script on player to allow activation of vehicles

namespace S_Quest
{

	public class EnterVehicles : MonoBehaviour {

		public GameObject EnterButton;
		public GameObject ExitButton;
		public Animator animator;
		public GameObject TheVehicle;
		public GameController GameController;
		public CameraController CameraController;
		public QuestManager QM;
		public InventoryManager IM;

		// private Rigidbody2D rb;

		// Use this for initialization
		void Start () {
			// rb = GetComponent<Rigidbody2D>();
		}
		
		void OnTriggerEnter2D(Collider2D other) {
			//--is the other trigger a vehicle? 

			Debug.Log("Player touching something "+other.name);

			if (other.tag == "PlayerVehicle"){
				TheVehicle = other.transform.parent.gameObject;
				Debug.Log("Player touching"+other.name);

				EnterButton.SetActive(true);
			} else if (other.tag == "Get out platform"){

				ExitButton.SetActive(true);
			}

			//--show enter dialog
		}

		void OnTriggerExit2D(Collider2D other) {
			//--is the other trigger a vehicle? 

			// Debug.Log("Player left trigger");

			if (other.tag == "PlayerVehicle"){
				Debug.Log("Player left van");
				EnterButton.SetActive(false);
			}

			//--show enter dialog
		}

		public void EnterVehicle(){

			//--hide the player
			gameObject.SetActive(false);
			// rb.isKinematic = false;

			//--parentt player inside vehicle
			gameObject.transform.parent = TheVehicle.transform; 

			GameController.CurrentVehicle = TheVehicle;

			//--make vehicle active

			Debug.Log("Enter vehicle");
			EnterButton.SetActive(false);

			//--flash vehicle
			
			TheVehicle.SendMessage("ActivateVehicle");

			CameraController.SwitchFollow(TheVehicle);

			QM.Player = TheVehicle; //--switch so we know this is the active player now
			IM.Player = TheVehicle.transform;
		}

		public void ExitVehicle(){
			//--put player back in the world

			gameObject.transform.parent = null;

			gameObject.transform.position = TheVehicle.transform.position;

			gameObject.transform.rotation = Quaternion.identity;

			gameObject.SetActive(true);

			ExitButton.SetActive(false);

			TheVehicle.SendMessage("DeactivateVehicle");

			CameraController.SwitchFollow(gameObject);
		}
	}
}
