/*

Quest Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;

namespace S_Quest 
{
	public class Quest : MonoBehaviour {

		[HideInInspector]
		public bool CanMeet = true;
		[HideInInspector]
		public int ActiveQuestID = -1;
		//Quest Info:
		public string QuestCode; //Quest's code.
		public Quest ParentQuest = null;
		//The order is used to make a progress of quests. Use the same code and different order to make a story.
		//And use other codes and 1 as the order to make side quests for example.
		public string QuestTitle; //Title of the quest.
		public string QuestObjective; //Please keep it short. Required. 
		public string Description; //This is used for detailed explanation.
		public string FinishedQuestMessage; //This is the message that the player sees when he talks to the quest giver after finishing the quest.
		public bool  RewardPlayer = true; //Reward the player when he finishes the quest?
		public int XP = 50; //How many experience points to give the player when he finishes the quest?
		[HideInInspector]
		public bool  QuestActive = false;
		[HideInInspector]
		public bool  QuestFinished = false; //Has the player done the quest?
		[HideInInspector]
		public bool  IsAvailable = false; //Is the quest available for player to do?
		[HideInInspector]
		public bool  ReturningToGiver = false; //Are we returning to the quest giver after completing the quest.
		[HideInInspector]
		public bool  Meeting = false; //This is true if the quest giver is expecting someone to meet him.

		//QuestGiver:
		public string QuestGiverName; //You can have a quest giver if you would like to. 
		public Sprite QuestGiverSprite;
		public Texture2D QuestGiverImage; //Leave this blank if you don't want an image.
		//GUI:
		[HideInInspector]
		public bool QuestOpen = false;
		public AudioClip Speech;

		public enum QuestTypes {Goto = 0, Collection = 1, Meeting = 2, Elimination = 3, PressAKey = 4, Follow = 5, Talk = 6} //Quest's types.
		//Quest Objective:
		[System.Serializable]
		public class Objective
		{
			public QuestTypes Type= QuestTypes.Goto & QuestTypes.Collection & QuestTypes.Meeting & QuestTypes.Elimination & QuestTypes.PressAKey & QuestTypes.Follow;
			public string Message;
			public Texture2D Icon;
			[HideInInspector]
			public bool  Show = false;

			//Goto quest type vars:
			public Vector3 Destination;
			public float TimeToStay = 0;
			public float Range = 1;

			//Meeting quest type vars:
			public Quest Target;
			public string ArrivalMessage;

			//Collecting/Elimination quest type vars:
			public string ItemName;
			public int AmountRequired = 5;
			[HideInInspector]
			public int Amount = 0;

			//Press a key vars:
			public KeyCode KeyToPress = KeyCode.A;
			public bool HoldKey = false;
			public float HoldTime = 3.0f;

			//Follow quest type vars:
			public GameObject ObjToFollow;
			public float FollowTime = 3.0f;
			public float MinDistance = 2.0f;
			public float MaxDistance = 2.0f;

			[HideInInspector]
			public bool  Achieved = false;

			//Objective time:
			public bool UseTimeLimit = false;
			public float TimeLimit = 120f; //Time limit for this objective, in seconds (if player fails to complete quest before this time, a custom event is called)
			public bool AbandonQuestOnTimeLimit = true; //If true, the quest will be abandonned when the time limit is over.

			[HideInInspector]
			public float Timer = 0;
		}
		public Objective[] Goal;

		public float ObjectiveTimer; //Objective current timer.

		public bool ReturnToGiver = true; //If true, the player is required to go back to the quest giver when it's completed.
		public bool ShowAfterFinished = false; //Please don't tick this if the quest giver has more quests for the player.
		[HideInInspector]
		public int Progress = 0; //Player's progress in the quest.


		//Meeting another quest giver:
		[HideInInspector]
		public int MeetingProgress; 
		[HideInInspector]
		public Quest Sender;

		[HideInInspector]
		public Transform MyTransform;

		//Scripts:
		[HideInInspector]
		public QuestManager Manager;
		[HideInInspector]
		public ExperienceManagerC XPManager;
		[HideInInspector]
		public InventoryQuest InvQuest;
		[HideInInspector]
		public QuestUIManager QuestUI;
		[HideInInspector]
		public InventoryManager InventoryManager; //--added by DRB
		QuestEvents CustomEvents;

		public GameObject Item;

		public bool ActivateOnStart = false; //if true, the quest will be activated as soon as the game starts0
		public bool Repeatable = false; //if true, the player will be able to repeat the quest even after it is completed.

		public bool NonAbandonable = false; //If true, the player can not abandon this quest.
		public bool SaveQuestProgress = false; //if true, the objectives progress for this quest will be saved when abandonned.

		void  Start ()
		{
			if(QuestCode == "")
			{
				Debug.LogError("S-Quest Error: Quest desactivated - Reason: Please give the quest a code.");
				gameObject.SetActive (false);
			}
			if(QuestObjective == "")
			{
				Debug.LogError("S-Quest Error: Quest desactivated - Reason: The quest objective is required.");
				gameObject.SetActive (false);
			}
			if(Goal.Length < 1)
			{
				Debug.LogError("S-Quest Error: Quest desactivated - Reason: Your quest must have at least one goal.");
				gameObject.SetActive (false);
			}


			Manager = FindObjectOfType(typeof(QuestManager)) as QuestManager;
			XPManager = FindObjectOfType(typeof(ExperienceManagerC)) as ExperienceManagerC;
			InvQuest = FindObjectOfType(typeof(InventoryQuest)) as InventoryQuest;
			QuestUI = FindObjectOfType (typeof(QuestUIManager)) as QuestUIManager;
			CustomEvents = FindObjectOfType (typeof(QuestEvents)) as QuestEvents;
			InventoryManager = FindObjectOfType (typeof(InventoryManager)) as InventoryManager;

			MyTransform = gameObject.transform; //Set the object tranform.
			QuestActive = false; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests();
			QuestOpen = false;
			Progress = 0;

			if(Manager.SaveAndLoad == true)
			{
				if(PlayerPrefs.GetInt(QuestCode) == 1)
				{
					if (Repeatable == false) {
						QuestFinished = true;
					}

					//Make the next quest available for the player if it's not already finished.
					Quest[] Quests = FindObjectsOfType(typeof(Quest)) as Quest[];
					foreach(Quest NextQuest in Quests) 
					{
						if(NextQuest.ParentQuest == this)
						{
							if(NextQuest.QuestFinished == false)
							{
								NextQuest.IsAvailable = true;
							}
						}
					}
				}
				else if(PlayerPrefs.GetInt(QuestCode+"Active",0) == 1)
				{
					AcceptQuest ();
					if(PlayerPrefs.GetInt(QuestCode+"ReturnToGiver",0) == 1)
					{
						ReturningToGiver = true;
					}
					else
					{
						Progress = PlayerPrefs.GetInt(QuestCode+"Progress",0);
						Goal[Progress].Amount = PlayerPrefs.GetInt(QuestCode+"Amount",0);
					}
				}
			}

			if(ParentQuest == null && QuestFinished == false)
			{
				IsAvailable = true;
			}

			if(QuestFinished == false && ActivateOnStart == true)
			{
				QuestActive = true; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests();
			}
		}


		void  OnMouseDown ()
		{
			//If the quest isn't available and nobody sent the player to meet this quest giver then don't show the quest.
			if((IsAvailable == false && Meeting == false ) || CanMeet == false)
			{
				return;
			}
			//If we choose not to show anything after finishing the quest and the quest is already finished, return;
			if(ShowAfterFinished == false && QuestFinished == true)
			{
				return; 
			}

			float Distance;
			Distance = Vector3.Distance(MyTransform.position, Manager.Player.transform.position); //Get the current distance between the player and the quest holder.
			if(Distance <= Manager.MinDistance && QuestOpen == false && Manager.ControlType == QuestManager.ControlTypes.Mouse || Manager.ControlType == QuestManager.ControlTypes.MouseAndKeyboard) //Check if that distance is below or equal to the minimum distance.
			{
				//Open the quest and play an audio if it exists.
				QuestOpen = true;
				QuestUI.ActiveQuest = this; QuestUI.OpenQuest();
				if (Manager.ShowTogQuestButton == true) {
					Manager.TogQuestButton.gameObject.SetActive (false);
				}
			} 
		}

		void  Update ()
		{

			//If the quest isn't available and nobody sent the player to meet this quest giver then don't show the quest.
			if(IsAvailable == false && Meeting == false)
			{
				return;
			}
			//If we choose not to show anything after finishing the quest and the quest is already finished, return;
			if(ShowAfterFinished == false && QuestFinished == true)
			{
				return; 
			}

			float Distance;
			Distance = Vector3.Distance(MyTransform.position, Manager.Player.transform.position); //Get the current distance between the player and the quest holder.

			if(Distance > Manager.MinDistance) //Check if the distance is no longer below or equal to the minimum distance required to open the quest.
			{
				if (QuestOpen == true) { //If the quest is already open:
					//The player moves away so we close the quest.
					QuestOpen = false;
					QuestUI.CloseQuest ();
				}

				if (Manager.ShowTogQuestButton == true && Manager.TogQuestButton.gameObject.activeInHierarchy == true && Manager.NearestQuestGiver == this) {
					Manager.TogQuestButton.gameObject.SetActive (false);
					Manager.NearestQuestGiver = null;
				}

			}

			if (Distance < Manager.MinDistance) {
				if (Manager.ShowTogQuestButton == true && QuestOpen == false && Manager.TogQuestButton.gameObject.activeInHierarchy == false) {
					Manager.NearestQuestGiver = this;
					Manager.TogQuestButton.gameObject.SetActive (true);
				}
				if (Input.GetKeyDown (Manager.TogQuest) && CanMeet == true) { //Checking if the player pressed the key used to open/close the quest.
					if (Manager.ControlType == QuestManager.ControlTypes.Keyboard || Manager.ControlType == QuestManager.ControlTypes.MouseAndKeyboard) {//Check if the player is in range of the quest giver.
						//Open the quest or close it and play an audio if it exists.
						QuestOpen = !QuestOpen;
						if (QuestOpen == true)
							QuestUI.ActiveQuest = this;
						QuestUI.OpenQuest ();
						if (QuestOpen == false)
							QuestUI.CloseQuest ();
					}
				}
			}


			if(QuestOpen == true && CanMeet == true) //If the quest window is open:
			{
				if(Manager.ControlType == QuestManager.ControlTypes.Keyboard || Manager.ControlType == QuestManager.ControlTypes.MouseAndKeyboard) //If we are controlling the quest with the keyboard/joystick.
				{
					if(Meeting == true)
					{
						if(Input.GetKey(Manager.AcceptQuest)) 
						{
							FinishMeetingQuest();
						}
					}
					//If we are returning to the quest giver after finishing the quest.
					else if(ReturningToGiver == true)
					{
						if(Input.GetKey(Manager.AcceptQuest)) 
						{
							FinishQuest();
						}
					}
					//If the quest is already finished.
					else if(QuestFinished == true)
					{
						if(Input.GetKey(Manager.AcceptQuest) || Input.GetKey(Manager.AbandonQuest)) 
						{
							QuestOpen = false;
							QuestUI.CloseQuest();
						}
					}
					else if(QuestActive == false) //If the quest is not active.
					{
						if(Input.GetKey(Manager.AcceptQuest)) 
						{
							Manager.AddActiveQuest(this);
							AcceptQuest ();    
						}
					}  
					else if(QuestActive == true) //If the quest is inactive.
					{
						if(Input.GetKey(Manager.AbandonQuest)) 
						{
							AbandonQuest ();
						}
					}
				}
			}
			if(QuestActive == true) //If the quest is active.
			{
				if(Goal[Progress].Achieved == false) //If the goal is not achieved?
				{
					if (Goal [Progress].Type == QuestTypes.Goto) { //If the type is a quest requires going to a destination.

						if (IsObjectInRangeOfPosition (Manager.Player.transform, Goal [Progress].Destination, Goal [Progress].Range) == true) { //If the player is in range of the destination.
							if (Goal [Progress].Timer > 0) {
								Goal [Progress].Timer -= Time.deltaTime; //Time required to stay in the destination in a goto goal. 
							}
							if (Goal [Progress].Timer < 0) {
								Goal [Progress].Timer = 0;
							}

							if (Goal [Progress].Timer == 0) { //If we finished the time required to stay.
								Goal [Progress].Achieved = true;
								MoveToNextObjective ();
							} 
						} else {
							if (Goal [Progress].Timer != Goal [Progress].TimeToStay) {
								Goal [Progress].Timer = Goal [Progress].TimeToStay; //Reset the timer if the player moves away from the destination.
							}
						}
					} else if (Goal [Progress].Type == QuestTypes.Meeting && CanMeet == true && Goal [Progress].Target.Sender != this) { //If the current quest type is a meeting.
						Goal [Progress].Target.Sender = this;
						Goal [Progress].Target.MeetingProgress = Progress;
						Goal [Progress].Target.Meeting = true;
					} else if (Goal [Progress].Type == QuestTypes.Follow) { //if it's a follow quest type:
						if (Goal [Progress].ObjToFollow != null) {
							if (Vector3.Distance (Manager.Player.transform.position, Goal [Progress].ObjToFollow.transform.position) < Goal [Progress].MaxDistance) { //If the player position distance from the target is less than the max allowed distance
								if (Vector3.Distance (Manager.Player.transform.position, Goal [Progress].ObjToFollow.transform.position) > Goal [Progress].MinDistance) { //If the player position distance from the target is less than the max allowed distance
									if (Goal [Progress].Timer > 0) {
										Goal [Progress].Timer -= Time.deltaTime;
									}
									if (Goal [Progress].Timer < 0) {
										Goal [Progress].Timer = 0;
									}

									if (Goal [Progress].Timer == 0) { //If we finished the time required to stay following the object
										Goal [Progress].Achieved = true;
										MoveToNextObjective ();
									} 
								} else {
									if (Goal [Progress].Timer != Goal [Progress].TimeToStay) {
										Goal [Progress].Timer = Goal [Progress].FollowTime; //Reset the timer if the player gets too close to the target object.
									}
								}
							} else {
								if (Goal [Progress].Timer != Goal [Progress].TimeToStay) {
									Goal [Progress].Timer = Goal [Progress].FollowTime; //Reset the timer if the player moves away from the targrt object.
								}
							}
						} else {
							//If the object was destroyed, cancel this quest:
							AbandonQuest ();
						}
					} else if (Goal [Progress].Type == QuestTypes.PressAKey) { //if this quest requires the player to press a key
						if (Goal [Progress].HoldKey == true) {
							
							if (Input.GetKey (Goal [Progress].KeyToPress)) {
								if (Goal [Progress].Timer > 0) {
									Goal [Progress].Timer -= Time.deltaTime;
								}
								if (Goal [Progress].Timer < 0) {
									Goal [Progress].Timer = 0;
								}

								if (Goal [Progress].Timer == 0) { //If we finished the time required to keep pressing the key.
									Goal [Progress].Achieved = true;
									MoveToNextObjective ();
								} 
							} else {
								Goal [Progress].Timer = Goal [Progress].HoldTime; //Reset the timer if the player stops pressing the key.
							}
						} else {
							if (Input.GetKeyUp (Goal [Progress].KeyToPress)) {
								Goal [Progress].Achieved = true;
								MoveToNextObjective ();
							}
						}
					} else if (Goal [Progress].Type == QuestTypes.Talk) { //if this quest just requires the panel to be shown
						
					}

					//If the quest has a time limit:
					if (Goal [Progress].UseTimeLimit == true) {
						if (ObjectiveTimer > 0) {
							ObjectiveTimer -= Time.deltaTime;
						}
						if (ObjectiveTimer < 0) {
							//if there's no time left for this quest:
							if (CustomEvents) {
								CustomEvents.OnObjectiveTimeOver (QuestCode, Progress);
							}
							//call the custom event so you can do whatever you want at this stage:
							if (Goal [Progress].AbandonQuestOnTimeLimit == true) {
								//If the abandon quest when the time limit is over, is enabled then:
								//abandon the quest:
								AbandonQuest();
							}

						}
					}
				}

			}
		}


		public void FinishMeetingQuest () //called when your meeting type quest has finished.
		{
			if(Manager.CompleteQuestSound) Manager.GetComponent<AudioSource>().PlayOneShot(Manager.CompleteQuestSound);
			if(Manager.SaveAndLoad == true)
			{
				PlayerPrefs.SetInt(Sender.QuestCode, 1);
				PlayerPrefs.SetInt(Sender.QuestCode+"Active", 0);
				PlayerPrefs.SetInt(Sender.QuestCode+"ReturnToGiver", 0);

				Manager.RemoveActiveQuest(Sender.ActiveQuestID);
				Sender.ActiveQuestID = -1;
			}

			Sender.ReturnToGiver = false;
			Sender.Goal[MeetingProgress].Achieved = true;
			Sender.MoveToNextObjective ();
			QuestOpen = false;
			Meeting = false;

			InventoryManager.ClearItems(); //--added by DRB

			if(CustomEvents) CustomEvents.OnPlayerCompleteQuest(Sender.QuestCode);
		}

		public void FinishQuest () //called when the whole quest was finished.
		{
			if(Manager.CompleteQuestSound) Manager.GetComponent<AudioSource>().PlayOneShot(Manager.CompleteQuestSound);
			if(Manager.SaveAndLoad == true)
			{
				PlayerPrefs.SetInt(QuestCode, 1);
				PlayerPrefs.SetInt(QuestCode+"Active", 0);
				Manager.RemoveActiveQuest(ActiveQuestID);
				ActiveQuestID = -1;
				PlayerPrefs.SetInt(QuestCode+"ReturnToGiver", 0);
			}

			//Reward the player with XP.
			if(RewardPlayer == true)
			{
				XPManager.AddXP(XP);

				if(Item != null)
				{
					InvQuest.Reward(Item); 
				}
			}

			QuestOpen = false;
			ReturningToGiver = false;
			if (Repeatable == false) {
				QuestFinished = true;
			}
			NextQuest();
			QuestActive = false; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests();
			Manager.Amount--;

			if(CustomEvents) CustomEvents.OnPlayerCompleteQuest(QuestCode);
		}

		public void  AcceptQuest (){
			if(Manager.Amount < Manager.MaxQuests) //If we still have space for new quests.
			{
				//Accept the quest.
				QuestActive = true; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests();
				if(Manager.AcceptQuestSound) Manager.GetComponent<AudioSource>().PlayOneShot(Manager.AcceptQuestSound);
				QuestOpen = false;
				QuestUI.CloseQuest();
				Manager.Amount++;

				PlayerPrefs.SetInt(QuestCode+"Active", 1);
				PlayerPrefs.SetInt(QuestCode+"Progress", Progress);

				if(CustomEvents) CustomEvents.OnPlayerAcceptQuest(QuestCode);

				if (Goal [Progress].Type == QuestTypes.Goto) {
					Goal [Progress].Timer = Goal [Progress].TimeToStay;
				}
				else if (Goal [Progress].Type == QuestTypes.Follow) {
					Goal [Progress].Timer = Goal [Progress].FollowTime;
				}
				else if (Goal [Progress].Type == QuestTypes.PressAKey && Goal [Progress].HoldKey == true) {
					Goal [Progress].Timer = Goal [Progress].HoldTime;
				}

				//If the quest has a time limit:
				if (Goal [Progress].UseTimeLimit == true) {
					ObjectiveTimer = Goal [Progress].TimeLimit; // start the timer:
				}
			}
			else
			{
				//QUESTS FULL
			}
		}

		public void  AbandonQuest (){
			//Can only abandon if the is abandonable:
			if (NonAbandonable == false) {
				Goal [Progress].Timer = 0.0f;
				if (Goal [Progress].Type == QuestTypes.Meeting) { //If the current quest type is a meeting.
					Goal [Progress].Target.Sender = null;
					Goal [Progress].Target.Meeting = false;
				}
				//Abandon the quest.
				QuestActive = false;
				if (Manager.LogOpen == true && Manager.ShowQuestLog == true)
					Manager.LogUI.LoadActiveQuests ();
				if (Manager.AbandonQuestSound)
					Manager.GetComponent<AudioSource> ().PlayOneShot (Manager.AbandonQuestSound);
				QuestOpen = false;
				QuestUI.CloseQuest ();
				Manager.Amount--;

				PlayerPrefs.SetInt (QuestCode + "Active", 0);
				PlayerPrefs.SetInt (QuestCode + "ReturnToGiver", 0);

				Manager.RemoveActiveQuest (ActiveQuestID);
				ActiveQuestID = -1;

				if (CustomEvents)
					CustomEvents.OnPlayerAbandonQuest (QuestCode);

				//Reset quest progress if we choose not to save it:
				if (SaveQuestProgress == false) {
					Progress = 0;

					if (Goal.Length > 0) {
						for(int i = 0; i < Goal.Length; i++)
						{
							Goal [i].Amount = 0;
						}
					}
				}
			}
		}

		//This function determines if two objects are in range of each other or not.
		bool  IsObjectInRangeOfPosition (Transform Obj, Vector3 Pos, float Range)
		{
			float Distance;
			Distance = Vector3.Distance(Obj.position, Pos); 
			if(Distance > Range) 
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		//This function determines the text out put for the quest for both of the classic themes.
		public string  GetQuestString ()
		{
			string QuestString; 
			if(Meeting == true)
			{
				QuestString = Sender.Goal[MeetingProgress].ArrivalMessage;
			}   
			else if(ReturningToGiver == true)
			{
				QuestString = "Objective Achieved:\n" + FinishedQuestMessage;
			}
			else if(QuestFinished == true)
			{
				QuestString = FinishedQuestMessage;
			}
			else
			{
				// QuestString = Description + "\n" + "Objective:\n" + QuestObjective;
				QuestString = Description;
			} 

			return QuestString;
		}

		public void NextQuest ()
		{
			//Make the next quest available for the player if it's not already finished.
			Quest[] Quests = FindObjectsOfType(typeof(Quest)) as Quest[];
			foreach(Quest NextQuest in Quests) 
			{
				if(NextQuest.ParentQuest == this)
				{
					if(NextQuest.QuestFinished == false)
					{
						NextQuest.IsAvailable = true;
					}
				}
			}
		}

		public void IncreaseAmount (int ObjectiveID, int ValueToAdd)
		{
			Goal[ObjectiveID].Amount += ValueToAdd;

			PlayerPrefs.SetInt(QuestCode+"Amount",Goal[ObjectiveID].Amount);
			if(Goal[ObjectiveID].Amount >= Goal[ObjectiveID].AmountRequired && ObjectiveID == Progress) //If the player collected/eliminated the required amount.
			{
				Goal[ObjectiveID].Achieved = true;
				MoveToNextObjective ();
			}
		}

		public void MoveToNextObjective ()
		{
			if(QuestActive == true)
			{
				//Check if the quest is finished or not.
				if(Goal.Length <= Progress+1)
				{
					if(ReturnToGiver == true && ReturningToGiver != true) //If we have to get back to the quest giver to end the 
					{
						if(CustomEvents) CustomEvents.OnPlayerCompleteObjective(QuestCode, Progress, true);
						if(ReturningToGiver != true)
						{
							ReturningToGiver = true;
							PlayerPrefs.SetInt(QuestCode+"ReturnToGiver", 1);
						}
					}
					else if(ReturnToGiver == false) //Else, we finish the quest here. :D
					{
						if(CustomEvents) CustomEvents.OnPlayerCompleteObjective(QuestCode, Progress, true);
						if(Manager.CompleteQuestSound) Manager.GetComponent<AudioSource>().PlayOneShot(Manager.CompleteQuestSound);

						if(Manager.SaveAndLoad == true)
						{
							PlayerPrefs.SetInt(QuestCode, 1);
							PlayerPrefs.SetInt(QuestCode+"Active", 0);
							Manager.RemoveActiveQuest(ActiveQuestID);
							ActiveQuestID = -1;
							PlayerPrefs.SetInt(QuestCode+"ReturnToGiver", 0);
						}

						//Reward the player with XP.
						if(RewardPlayer == true)
						{
							XPManager.AddXP(XP);

							if(Item != null)
							{
								InvQuest.Reward(Item);
							}
						}

						if (Repeatable == false) {
							QuestFinished = true;
						}
						NextQuest(); 
						if(CustomEvents) CustomEvents.OnPlayerCompleteQuest(QuestCode);
						QuestActive = false; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests();
						Manager.Amount--;
					}    
				}
				else //If we didn't, then move to the next goal.
				{
					if(CustomEvents) CustomEvents.OnPlayerCompleteObjective(QuestCode, Progress, false);
					Progress++;
					PlayerPrefs.SetInt(QuestCode+"Active", 1);
					PlayerPrefs.SetInt(QuestCode+"Progress", Progress);
					PlayerPrefs.SetInt(QuestCode+"ReturnToGiver", 0);

					if (Goal [Progress].Type == QuestTypes.Goto) {
						Goal [Progress].Timer = Goal [Progress].TimeToStay;
					}
					else if (Goal [Progress].Type == QuestTypes.Follow) {
						Goal [Progress].Timer = Goal [Progress].FollowTime;
					}
					else if (Goal [Progress].Type == QuestTypes.PressAKey && Goal [Progress].HoldKey == true) {
						Goal [Progress].Timer = Goal [Progress].HoldTime;
					}

					//If the quest has a time limit:
					if (Goal [Progress].UseTimeLimit == true) {
						ObjectiveTimer = Goal [Progress].TimeLimit; // start the timer:
					}
				}
			}
		}
	}
}
