/*

Quest Manager Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace S_Quest
{
	public class QuestManager : MonoBehaviour 
	{
		public GameObject Player; //Main player.

		//Quest controls:
		public enum ControlTypes {Mouse = 0, Keyboard = 1, MouseAndKeyboard = 2}
		public ControlTypes ControlType = ControlTypes.Mouse & ControlTypes.Keyboard & ControlTypes.MouseAndKeyboard;
		public KeyCode TogQuest = KeyCode.Q;
		public KeyCode AcceptQuest = KeyCode.A;
		public KeyCode AbandonQuest = KeyCode.D;
		public KeyCode CollectKey = KeyCode.C;
		public KeyCode EliminateKey = KeyCode.E;

		//Tog Quest button:
		public bool ShowTogQuestButton = false;
		public Button TogQuestButton;
		public Quest NearestQuestGiver;

		public float MinDistance = 4.0f;

		public bool  ProgressBar = true; //Use a progress bar in the objective bar UI?


		//Quest Log:
		public int MaxQuests = 10; //Maximum number of active quests at one time.
		[HideInInspector]
		public int Amount = 0; //Current amount of active quests.
		[HideInInspector]
		public bool  LogOpen = false;
		public KeyCode LogKey= KeyCode.L; //Key used to show the Quest log.
		[HideInInspector]
		public float Distance;

		public bool  ShowObjectiveBar = true; //Show the Objective bar?
		public bool  ShowQuestLog = true; //Show the quest log or not?
		[HideInInspector]
		public Quest TargetQuest = null;
		//GUI:
		[HideInInspector]
		public bool  QuestOpen = false;

		//Sounds:
		public AudioClip CompleteQuestSound;
		public AudioClip AcceptQuestSound;
		public AudioClip AbandonQuestSound;

		public bool  SaveAndLoad = false;

		public class QuestsGoalsVars 
		{
			public Quest.QuestTypes Type= Quest.QuestTypes.Goto & Quest.QuestTypes.Collection & Quest.QuestTypes.Meeting & Quest.QuestTypes.Elimination;
			public string Message;

			//Collecting/Elimination quest type vars:
			public string ItemName;
			public int AmountRequired = 5;
		}

		public class ActiveQuestsVars 
		{
			public bool  IsTaken = false;
			public int QuestOrder;
			public string QuestCode;
			public string QuestTitle; //Title of the quest.
			public string QuestObjective; //Please keep it short. Required. 
			public string Description; //This is used for detailed explanation.
			public string FinishedQuestMessage; //This is the message that the player sees when he talks to the quest giver after finishing the
			public string QuestGiverName;

			public QuestsGoalsVars[] Goals;
		}
		[HideInInspector]
		public ActiveQuestsVars[] ActiveQuests;

		//Scripts:
		[HideInInspector]
		public QuestLogUI LogUI;

		void  Awake ()
		{
			//Starting the active quests array:
			ActiveQuests = new ActiveQuestsVars[MaxQuests];

			for(int i = 0; i < MaxQuests; i++)
			{
				ActiveQuests[i] = new ActiveQuestsVars ();
			}

			LoadActiveQuests ();

			LogUI = FindObjectOfType (typeof(QuestLogUI)) as QuestLogUI;
			LogOpen = false;
		}

		void  Update (){
			if(Input.GetKeyDown(LogKey) && ShowQuestLog == true) //Show or hide the quest log.
			{
				LogOpen = !LogOpen;
				if(LogOpen == true) LogUI.OpenQuestLog();
				if(LogOpen == false) LogUI.CloseQuestLog();
			}
		}

		void  LoadActiveQuests (){
			for(int i = 0; i < MaxQuests; i++)
			{
				if(PlayerPrefs.GetInt("ActiveQuest"+"Taken"+i.ToString(), 0) == 1)
				{
					ActiveQuests[i].QuestCode = PlayerPrefs.GetString("ActiveQuest"+"Code"+i.ToString());
					ActiveQuests[i].QuestOrder = PlayerPrefs.GetInt("ActiveQuest"+"Order"+i.ToString());

					ActiveQuests[i].QuestTitle = PlayerPrefs.GetString("ActiveQuest"+"Title"+i.ToString());
					ActiveQuests[i].QuestObjective = PlayerPrefs.GetString("ActiveQuest"+"Objective"+i.ToString());
					ActiveQuests[i].Description = PlayerPrefs.GetString("ActiveQuest"+"Description"+i.ToString());
					ActiveQuests[i].FinishedQuestMessage = PlayerPrefs.GetString("ActiveQuest"+"FinishedQuestMessage"+i.ToString());

					ActiveQuests[i].QuestGiverName = PlayerPrefs.GetString("ActiveQuest"+"QuestGiverName"+i.ToString());

					ActiveQuests[i].Goals = new QuestsGoalsVars[PlayerPrefs.GetInt("ActiveQuest"+"GoalsAmount"+i.ToString())];
					for(int j = 0; j < ActiveQuests[i].Goals.Length; j++)
					{
						ActiveQuests[i].Goals[j] = new QuestsGoalsVars ();

						if(PlayerPrefs.GetInt("ActiveQuest"+"Type"+i.ToString()+j.ToString()) == 0)
						{
							ActiveQuests[i].Goals[j].Type = Quest.QuestTypes.Goto;
						}
						if(PlayerPrefs.GetInt("ActiveQuest"+"Type"+i.ToString()+j.ToString()) == 1)
						{
							ActiveQuests[i].Goals[j].Type = Quest.QuestTypes.Collection;
						}
						if(PlayerPrefs.GetInt("ActiveQuest"+"Type"+i.ToString()+j.ToString()) == 2)
						{
							ActiveQuests[i].Goals[j].Type = Quest.QuestTypes.Meeting;
						}
						if(PlayerPrefs.GetInt("ActiveQuest"+"Type"+i.ToString()+j.ToString()) ==3 )
						{
							ActiveQuests[i].Goals[j].Type = Quest.QuestTypes.Elimination;
						}

						ActiveQuests[i].Goals[j].Message = PlayerPrefs.GetString("ActiveQuest"+"Message"+i.ToString()+j.ToString());
						ActiveQuests[i].Goals[j].ItemName = PlayerPrefs.GetString("ActiveQuest"+"ItemName"+i.ToString()+j.ToString());
						ActiveQuests[i].Goals[j].AmountRequired = PlayerPrefs.GetInt("ActiveQuest"+"ItemAmount"+i.ToString()+j.ToString());

					}

					if(CheckIfQuestInScene (ActiveQuests[i].QuestCode, ActiveQuests[i].QuestOrder, i) == false)
					{
						GameObject Clone= new GameObject();
						Clone.transform.SetParent(this.gameObject.transform, true);
						Clone.name = "Quest"+i.ToString();

						Clone.AddComponent<Quest>();
						Clone.GetComponent<Quest>().QuestCode = ActiveQuests[i].QuestCode;
						Clone.GetComponent<Quest>().IsAvailable = true;
						Clone.GetComponent<Quest>().ActiveQuestID = i;
						Clone.GetComponent<Quest>().CanMeet = false;

						Clone.GetComponent<Quest>().QuestTitle = ActiveQuests[i].QuestTitle;
						Clone.GetComponent<Quest>().QuestObjective = ActiveQuests[i].QuestObjective;
						Clone.GetComponent<Quest>().Description = ActiveQuests[i].Description;
						Clone.GetComponent<Quest>().FinishedQuestMessage = ActiveQuests[i].FinishedQuestMessage;

						Clone.GetComponent<Quest>().QuestGiverName = ActiveQuests[i].QuestGiverName;
						Clone.GetComponent<Quest>().QuestGiverSprite = Resources.Load( Clone.GetComponent<Quest>().QuestGiverName, typeof(Sprite)) as Sprite;

						Clone.GetComponent<Quest>().Goal = new Quest.Objective[ActiveQuests[i].Goals.Length];
						for(int j = 0; j < ActiveQuests[i].Goals.Length; j++)
						{
							Clone.GetComponent<Quest>().Goal[j] = new Quest.Objective ();

							if(ActiveQuests[i].Goals[j].Type == Quest.QuestTypes.Goto)
							{
								Clone.GetComponent<Quest>().Goal[j].Type = Quest.QuestTypes.Goto;
							}
							if(ActiveQuests[i].Goals[j].Type == Quest.QuestTypes.Collection)
							{
								Clone.GetComponent<Quest>().Goal[j].Type = Quest.QuestTypes.Collection;
							}
							if(ActiveQuests[i].Goals[j].Type == Quest.QuestTypes.Meeting)
							{
								Clone.GetComponent<Quest>().Goal[j].Type = Quest.QuestTypes.Meeting;
							}
							if(ActiveQuests[i].Goals[j].Type == Quest.QuestTypes.Elimination)
							{
								Clone.GetComponent<Quest>().Goal[j].Type = Quest.QuestTypes.Elimination;
							}

							Clone.GetComponent<Quest>().Goal[j].Message = ActiveQuests[i].Goals[j].Message;
							Clone.GetComponent<Quest>().Goal[j].ItemName = ActiveQuests[i].Goals[j].ItemName;
							Clone.GetComponent<Quest>().Goal[j].AmountRequired = ActiveQuests[i].Goals[j].AmountRequired;
						}
					}

				}
			}
		}

		bool  CheckIfQuestInScene ( string Code ,   int Order ,  int ID){
			//Searching for the saved active quest in the scene:
			Quest[] Quests = FindObjectsOfType(typeof(Quest)) as Quest[];
			if(Quests.Length == 0) return false;
			foreach(Quest TempQuest in Quests) 
			{
				if(TempQuest.QuestCode == Code)
				{
					TempQuest.ActiveQuestID = ID;
					return true;
				}
			}

			return false;
		}

		public void  SaveActiveQuests (){
			for(int i = 0; i < MaxQuests; i++)
			{
				if(ActiveQuests[i].IsTaken == true)
				{
					PlayerPrefs.SetInt("ActiveQuest"+"Taken"+i.ToString(), 1);
					PlayerPrefs.SetString("ActiveQuest"+"Code"+i.ToString(), ActiveQuests[i].QuestCode);
					PlayerPrefs.SetInt("ActiveQuest"+"Order"+i.ToString(), ActiveQuests[i].QuestOrder);

					PlayerPrefs.SetString("ActiveQuest"+"Title"+i.ToString(), ActiveQuests[i].QuestTitle);
					PlayerPrefs.SetString("ActiveQuest"+"Objective"+i.ToString(), ActiveQuests[i].QuestObjective);
					PlayerPrefs.SetString("ActiveQuest"+"Description"+i.ToString(), ActiveQuests[i].Description);
					PlayerPrefs.SetString("ActiveQuest"+"FinishedQuestMessage"+i.ToString(), ActiveQuests[i].FinishedQuestMessage);

					PlayerPrefs.SetString("ActiveQuest"+"QuestGiverName"+i.ToString(), ActiveQuests[i].QuestGiverName);

					PlayerPrefs.SetInt("ActiveQuest"+"GoalsAmount"+i.ToString(), ActiveQuests[i].Goals.Length);
					for(int j = 0; j < ActiveQuests[i].Goals.Length; j++)
					{
						if(ActiveQuests[i].Goals[j].Type == Quest.QuestTypes.Goto)
						{
							PlayerPrefs.SetInt("ActiveQuest"+"Type"+i.ToString()+j.ToString(), 0);
						}
						if(ActiveQuests[i].Goals[j].Type == Quest.QuestTypes.Collection)
						{
							PlayerPrefs.SetInt("ActiveQuest"+"Type"+i.ToString()+j.ToString(), 1);
						}
						if(ActiveQuests[i].Goals[j].Type == Quest.QuestTypes.Meeting)
						{
							PlayerPrefs.SetInt("ActiveQuest"+"Type"+i.ToString()+j.ToString(), 2);
						}
						if(ActiveQuests[i].Goals[j].Type == Quest.QuestTypes.Elimination)
						{
							PlayerPrefs.SetInt("ActiveQuest"+"Type"+i.ToString()+j.ToString(), 3);
						}

						PlayerPrefs.SetString("ActiveQuest"+"Message"+i.ToString()+j.ToString(), ActiveQuests[i].Goals[j].Message);
						PlayerPrefs.SetString("ActiveQuest"+"ItemName"+i.ToString()+j.ToString(), ActiveQuests[i].Goals[j].ItemName);
						PlayerPrefs.SetInt("ActiveQuest"+"ItemAmount"+i.ToString()+j.ToString(), ActiveQuests[i].Goals[j].AmountRequired);
					}

				}
			}
		}

		public void  AddActiveQuest ( Quest Qu  ){
			for(int i = 0; i < MaxQuests; i++)
			{
				if(ActiveQuests[i].IsTaken == false)
				{
					ActiveQuests[i].QuestCode = Qu.QuestCode;

					ActiveQuests[i].QuestTitle = Qu.QuestTitle;
					ActiveQuests[i].QuestObjective = Qu.QuestObjective;
					ActiveQuests[i].Description = Qu.Description;
					ActiveQuests[i].FinishedQuestMessage = Qu.FinishedQuestMessage;

					ActiveQuests[i].QuestGiverName = Qu.QuestGiverName;

					ActiveQuests[i].Goals = new QuestsGoalsVars[Qu.Goal.Length];

					for(int j = 0; j < Qu.Goal.Length; j++)
					{
						ActiveQuests[i].Goals[j] = new QuestsGoalsVars ();

						ActiveQuests[i].Goals[j].Type = Qu.Goal[j].Type; 
						ActiveQuests[i].Goals[j].Message = Qu.Goal[j].Message;
						ActiveQuests[i].Goals[j].ItemName = Qu.Goal[j].ItemName ;
						ActiveQuests[i].Goals[j].AmountRequired = Qu.Goal[j].AmountRequired;
					}

					PlayerPrefs.SetInt("ActiveQuest"+"Taken"+i.ToString(), 1);
					ActiveQuests[i].IsTaken = true;

					Qu.ActiveQuestID = i;

					SaveActiveQuests ();

					return;
				}
			}
		}

		public void  RemoveActiveQuest ( int ID  ){
			PlayerPrefs.SetInt("ActiveQuest"+"Taken"+ID.ToString(), 0);
			ActiveQuests[ID].IsTaken = false;

			SaveActiveQuests ();

		}

		//tog quest button callback:
		public void OnTogQuestButton ()
		{
			if (NearestQuestGiver != null) {

				NearestQuestGiver.QuestOpen = true;
				NearestQuestGiver.QuestUI.ActiveQuest = NearestQuestGiver;
				NearestQuestGiver.QuestUI.OpenQuest ();

				TogQuestButton.gameObject.SetActive (false);
			}
		}
	}
}