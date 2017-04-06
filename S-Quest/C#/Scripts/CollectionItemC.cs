/*

Collection Item Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;

namespace S_Quest
{
	public class CollectionItemC : MonoBehaviour {

		//This script is used to identify items that the player has to collect.

		public string Name; //Item name.
		QuestManager Manager;

		public bool CollectOnCollision = false;

		void  Awake (){
			Manager = FindObjectOfType(typeof(QuestManager)) as QuestManager;
		}

		void Start(){

			Debug.Log("player = "+Manager.Player);
		}

		void  OnMouseDown (){
			if (CollectOnCollision == false) {
				if (Manager.ControlType == QuestManager.ControlTypes.Mouse || Manager.ControlType == QuestManager.ControlTypes.MouseAndKeyboard) { //Mouse controls.
					//Collection Quest:
					//Search for all quests.
					Quest[] Quests;
					Quests = FindObjectsOfType (typeof(Quest)) as Quest[];
					//Loop through all the quests.
					foreach (Quest i in Quests) {
						if (i.QuestActive == true && i.Goal.Length > 0) { //Check if the current quest is active.
							if (i.Goal [i.Progress].Achieved == false && i.Goal [i.Progress].Type == Quest.QuestTypes.Collection && i.ReturningToGiver == false) { //If the goal is not achieved and we are dealing with a collection quest.
								float Distance;
								Distance = Vector3.Distance (gameObject.transform.position, Manager.Player.transform.position); //Get the current distance between the player and the item.
								Debug.Log(Name+" distance from player = "+Distance);
								if (Distance > Manager.MinDistance) { //Check if that distance is higher than the minimum distance required to collect the item.
									return; //Stop here.
								} else if (i.Goal [i.Progress].Amount < i.Goal [i.Progress].AmountRequired) { //If we still didn't achieve the required amount of this item.
									//Destroy the object.
									Destroy (this.gameObject);
									i.IncreaseAmount (i.Progress, 1);
								}
							}
						}
					}
				}
			}
		}

		void Update ()
		{
			// if (CollectOnCollision == false) {
			// 	if (Manager.ControlType == QuestManager.ControlTypes.Keyboard || Manager.ControlType == QuestManager.ControlTypes.MouseAndKeyboard) { //Keyboard controls:
			// 		float Distance;
			// 		Distance = Vector3.Distance (gameObject.transform.position, Manager.Player.transform.position); //Get the current distance between the player and the item.
			// 		if (Distance < Manager.MinDistance) { //Check if that distance is lower than the minimum distance required to collect the item.
			// 			if (Input.GetKeyDown (Manager.CollectKey)) { //Player presses the pick up key.
			// 				AttemptToCollect ();
			// 			}
			// 		}
			// 	}
			// }
		}

		void OnTriggerEnter (Collider other)
		{
			Debug.Log("A collection trigger has happened 1");
			if (CollectOnCollision == true) {
				Debug.Log("A collection trigger has happened 2");
				if (other.gameObject == Manager.Player) {
					AttemptToCollect ();
				}
			}
		}

		void AttemptToCollect ()
		{
			//Collection Quest:
			//Search for all quests.
			Debug.Log("Attempt to collect "+Name);
			Quest[] Quests;
			Quests = FindObjectsOfType (typeof(Quest)) as Quest[];
			//Loop through all the quests.
			foreach (Quest i in Quests) {
				Debug.Log(Name+" i = "+i);
				if (i.QuestActive == true && i.Goal.Length > 0) { //Check if the current quest is active.
					if (i.Goal [i.Progress].Amount < i.Goal [i.Progress].AmountRequired) { //If we still didn't achieve the required amount of this item.
						//Destroy the object.
						Destroy (this.gameObject);
						i.Goal [i.Progress].Amount++;
					}
				}
			}
		}
	}
}