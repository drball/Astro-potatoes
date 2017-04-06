// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace S_Quest
{
	public class ObjectiveBarUI : MonoBehaviour {

		public Text ObjectivesText;

		//Scripts:
		QuestManager Manager;

		void  Awake (){
			Manager = FindObjectOfType(typeof(QuestManager)) as QuestManager;
			if(Manager.ShowObjectiveBar == true)
			{
				ObjectivesText.gameObject.SetActive(true);
				RefreshObjectives();
			}
			else
			{
				ObjectivesText.gameObject.SetActive(false);
			}
		}

		void Update ()
		{
			RefreshObjectives ();
		}

		public void  RefreshObjectives (){
			ObjectivesText.text = "";
			string TempText = "";
			//Search for all the active quests of the player.
			Quest[] Quests = FindObjectsOfType(typeof(Quest)) as Quest[];
			int ActiveQuests = 0; //This variable will hold the amount of the current active quests.

			foreach(Quest Quest in Quests) 
			{
				if(Quest.QuestActive == true) //Check if the current quest is active.
				{
					ActiveQuests++;
					//Classic objectives bar style: using text.
					//Let's add the color yellow for the quest title. but keep the color you chose for the rest.
					TempText += "\n"+Quest.QuestTitle+":\n";
					//Determine the current objective of this quest.
					string Goal = Quest.Goal[Quest.Progress].Message;

					//Calculating the distance if we are returning to the quest giver, collecting or eliminating items.
					float Distance = 0.0f;
					Vector3 Target;
					if(Quest.ReturningToGiver == true)
					{
						Target = Quest.transform.position;
						Distance = Vector3.Distance(Target, Manager.Player.transform.position); //Get the current distance between the player and the target.
					}
					else if(Quest.Goal[Quest.Progress].Type == Quest.QuestTypes.Goto && Quest.CanMeet == true)
					{
						Target = Quest.Goal[Quest.Progress].Destination;
						Distance = Vector3.Distance(Target, Manager.Player.transform.position); //Get the current distance between the player and the target.
						Distance -= Quest.Goal[Quest.Progress].Range;
					}
					else if(Quest.Goal[Quest.Progress].Type == Quest.QuestTypes.Meeting && Quest.CanMeet == true)
					{
						Target = Quest.Goal[Quest.Progress].Target.transform.position;

						Distance = Vector3.Distance(Target, Manager.Player.transform.position); //Get the current distance between the player and the target.
					}
					else if(Quest.Goal[Quest.Progress].Type == Quest.QuestTypes.Follow)
					{
						Target = Quest.Goal[Quest.Progress].ObjToFollow.transform.position;
						Distance = Vector3.Distance(Target, Manager.Player.transform.position); //Get the current distance between the player and the target.
						Distance -= Quest.Goal[Quest.Progress].MaxDistance;
					}
					int Int;
					Int = Mathf.CeilToInt(Distance);

					if(Quest.ReturningToGiver == true)
					{
						if(Quest.CanMeet == true)
						{
							Goal = "Return to " + Quest.QuestGiverName + ":  "+Int.ToString()+ " m left.";
						}
						else
						{
							Goal = "Return to " + Quest.QuestGiverName;
						}
					}
					else if(Quest.Goal[Quest.Progress].Type == Quest.QuestTypes.Collection || Quest.Goal[Quest.Progress].Type == Quest.QuestTypes.Elimination)
					{
						Goal += ": "+Quest.Goal[Quest.Progress].Amount.ToString()+"/"+Quest.Goal[Quest.Progress].AmountRequired.ToString();
					}
					else if(Quest.Goal[Quest.Progress].Type == Quest.QuestTypes.Goto || Quest.Goal[Quest.Progress].Type == Quest.QuestTypes.Meeting)
					{
						if(Quest.CanMeet == true)
						{
							Goal += ": "+Int.ToString()+ " m left.";
						}
						if(Quest.Goal[Quest.Progress].Type == Quest.QuestTypes.Goto )
						{
							if (Quest.Goal [Quest.Progress].TimeToStay > 0 && Distance <= Quest.Goal[Quest.Progress].Range) {
								Goal += "Stay for: "+GetTimeText(Quest.Goal[Quest.Progress].Timer);
							}
						}
					}
					else if(Quest.Goal[Quest.Progress].Type == Quest.QuestTypes.Follow)
					{
						if (Vector3.Distance (Manager.Player.transform.position, Quest.Goal [Quest.Progress].ObjToFollow.transform.position) < Quest.Goal [Quest.Progress].MaxDistance) { //If the player position distance from the target is less than the max allowed distance
							if (Vector3.Distance (Manager.Player.transform.position, Quest.Goal [Quest.Progress].ObjToFollow.transform.position) > Quest.Goal [Quest.Progress].MinDistance) { //If the player position distance from the target is less than the max allowed distance
								Goal += "Follow for: "+GetTimeText(Quest.Goal[Quest.Progress].Timer);
							} else {
								Goal += "You are too close!";
							}
						} else {
							Goal += ": "+Int.ToString()+ " m left.";
						}

					}
					else if(Quest.Goal[Quest.Progress].Type == Quest.QuestTypes.PressAKey)
					{
						if (Quest.Goal [Quest.Progress].HoldKey == true) {
							Goal += "Hold '" + Quest.Goal [Quest.Progress].KeyToPress.ToString () + "' for " + GetTimeText (Quest.Goal [Quest.Progress].Timer);
						} else {
							Goal += Goal += "Press '" + Quest.Goal [Quest.Progress].KeyToPress.ToString () + "' once.";
						}

					}
					TempText += Goal+"\n";

					//Quest time limit:
					if (Quest.Goal [Quest.Progress].UseTimeLimit == true) {
						
						TempText += "Time left: " + GetTimeText(Quest.ObjectiveTimer);
					}
				}
			}

			// ObjectivesText.text = "Objectives("+ActiveQuests.ToString()+"):"+TempText;
			ObjectivesText.text = "";
		}

		public string GetTimeText (float TimeInSeconds)
		{
			string Output = "";

			int TimeLeftMinutes = (int)(TimeInSeconds / 60.0f);
			int TimeLeftSeconds = (int)(TimeInSeconds - TimeLeftMinutes * 60.0f);

			string Minutes = TimeLeftMinutes.ToString ();
			if (TimeLeftMinutes < 10) {
				Minutes = "0" + TimeLeftMinutes.ToString ();
			}
			string Seconds = TimeLeftSeconds.ToString ();
			if (TimeLeftSeconds < 10) {
				Seconds = "0" + TimeLeftSeconds.ToString ();
			}
			Output = Minutes + ":" + Seconds;

			return Output;
		}
	}

}