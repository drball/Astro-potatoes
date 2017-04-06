/*

Elimination Item Script Created by Oussama Bouanani (SoumiDelRio).
This is just an example. You can copy some of the code here then paste it in your own combat/elimination scripts.

*/

using UnityEngine;
using System.Collections;

namespace S_Quest
{
	public class EliminationItemC : MonoBehaviour {

		//This script is used to identify items that the player has to eliminate.

		public string Name; //Item name.
		QuestManager Manager;


		void  Awake (){
			Manager = FindObjectOfType(typeof(QuestManager)) as QuestManager;
		}


		void  OnMouseDown (){

			if(Manager.ControlType == QuestManager.ControlTypes.Mouse || Manager.ControlType == QuestManager.ControlTypes.MouseAndKeyboard) //Mouse controls.
			{
				//Elimination Quest:
				//Search for all quests.
				Quest[] Quests;
				Quests = FindObjectsOfType(typeof(Quest)) as Quest[];
				//Loop through all the quests.
				foreach(Quest i in Quests)
				{
					if(i.QuestActive == true && i.Goal.Length > 0) //Check if the current quest is active.
					{
						if(i.Goal[i.Progress].Achieved == false && i.Goal[i.Progress].Type == Quest.QuestTypes.Elimination && i.ReturningToGiver == false) //If the goal is not achieved and we are dealing with a elimination quest.
						{
							float Distance;
							Distance = Vector3.Distance(gameObject.transform.position, Manager.Player.transform.position); //Get the current distance between the player and the item.
							if(Distance > Manager.MinDistance) //Check if that distance is higher than the minimum distance required to eliminate the item.
							{
								return; //Stop here.
							}
							else if(i.Goal[i.Progress].Amount < i.Goal[i.Progress].AmountRequired) //If we still didn't achieve the required amount of this item.
							{
								//Destroy the object.
								Destroy(this.gameObject);
								i.IncreaseAmount (i.Progress, 1);
							}
						}
					}
				}
			}
		}

		void Update ()
		{
			if(Manager.ControlType == QuestManager.ControlTypes.Keyboard || Manager.ControlType == QuestManager.ControlTypes.MouseAndKeyboard) //Keyboard controls:
			{
				float Distance;
				Distance = Vector3.Distance(gameObject.transform.position, Manager.Player.transform.position); //Get the current distance between the player and the item.
				if(Distance < Manager.MinDistance) //Check if that distance is lower than the minimum distance required to eliminate the item.
				{
					if(Input.GetKeyDown(Manager.EliminateKey)) //Player presses the pick up key.
					{
						//Elimination Quest:
						//Search for all quests.
						Quest[] Quests;
						Quests = FindObjectsOfType(typeof(Quest)) as Quest[];
						//Loop through all the quests.
						foreach(Quest i in Quests)
						{
							if(i.QuestActive == true && i.Goal.Length > 0) //Check if the current quest is active.
							{
								if(i.Goal[i.Progress].Amount < i.Goal[i.Progress].AmountRequired) //If we still didn't achieve the required amount of this item.
								{
									//Destroy the object.
									Destroy(this.gameObject);
									i.Goal[i.Progress].Amount++;
								}
							}
						}
					}
				}
			}
		}
	}
}