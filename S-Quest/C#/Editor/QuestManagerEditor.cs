using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace S_Quest
{
	[CustomEditor(typeof(QuestManager))] 
	public class QuestManagerEditor : Editor {

		//Quest Manager custom editor:
		public override void OnInspectorGUI() 
		{
			QuestManager MyTarget = (QuestManager) target;

			
			
			EditorGUILayout.Space();

			//Quests settings:
			EditorGUILayout.LabelField("Main Player Object:");
			
			MyTarget.Player = EditorGUILayout.ObjectField(MyTarget.Player, typeof(GameObject), true) as GameObject;
			

			//Control settings:
			MyTarget.ControlType = (QuestManager.ControlTypes)EditorGUILayout.EnumPopup("Quest Control Type:", MyTarget.ControlType);

			//Tog Quest Button:
			MyTarget.ShowTogQuestButton = EditorGUILayout.Toggle("Show Tog Quest Button:", MyTarget.ShowTogQuestButton);
			if (MyTarget.ShowTogQuestButton == true) {
				EditorGUILayout.LabelField ("Tog Quest Button:");

				MyTarget.TogQuestButton = EditorGUILayout.ObjectField (MyTarget.TogQuestButton, typeof(Button), true) as Button;

			}

			EditorGUILayout.LabelField("Tog (Open/Close) Quest Key:");
			MyTarget.TogQuest = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.TogQuest);
			EditorGUILayout.LabelField("Accept/Finish Quest Key:");
			MyTarget.AcceptQuest = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.AcceptQuest);
			EditorGUILayout.LabelField("Abandon Quest Key:");
			MyTarget.AbandonQuest = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.AbandonQuest);
			EditorGUILayout.LabelField("Collect Item Key:");
			MyTarget.CollectKey = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.CollectKey);
			EditorGUILayout.LabelField("Eliminate Item Key:");
			MyTarget.EliminateKey = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.EliminateKey);

			EditorGUILayout.LabelField("Minimum Distance Between Player And Quest Giver:");
			MyTarget.MinDistance = EditorGUILayout.FloatField("Distance:", MyTarget.MinDistance);

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			//Quest Log:
			EditorGUILayout.LabelField("Quest Log:");
			MyTarget.ShowQuestLog = EditorGUILayout.Toggle("Show Quest Log:", MyTarget.ShowQuestLog);
			if(MyTarget.ShowQuestLog == true)
			{
				MyTarget.MaxQuests = EditorGUILayout.IntField("Max Quests:", MyTarget.MaxQuests);
				EditorGUILayout.LabelField("Quest Log Key:");
				MyTarget.LogKey = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.LogKey);
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			//Objective Bar:
			EditorGUILayout.LabelField("Objectives Bar:");
			MyTarget.ShowObjectiveBar = EditorGUILayout.Toggle("Show Objective Bar:", MyTarget.ShowObjectiveBar);
			/*if(MyTarget.ShowObjectiveBar == true)
	    	{
			MyTarget.ProgressBar = EditorGUILayout.Toggle("Show Progress Bar:", MyTarget.ProgressBar);
		    }*/

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			//Audio:
			EditorGUILayout.LabelField("Sound When Player Accepts Quest:");
			
			MyTarget.AcceptQuestSound = EditorGUILayout.ObjectField(MyTarget.AcceptQuestSound, typeof(AudioClip), true) as AudioClip;
			 

			EditorGUILayout.LabelField("Sound When Player Abandons Quest:");

			MyTarget.AbandonQuestSound = EditorGUILayout.ObjectField(MyTarget.AbandonQuestSound, typeof(AudioClip), true) as AudioClip;
			 

			EditorGUILayout.LabelField("Sound When Player Completes Quest:");
			
			MyTarget.CompleteQuestSound = EditorGUILayout.ObjectField(MyTarget.CompleteQuestSound, typeof(AudioClip), true) as AudioClip;
			 

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			MyTarget.SaveAndLoad = EditorGUILayout.Toggle("Save And Load:", MyTarget.SaveAndLoad);

			if(GUILayout.Button("Reset All Quests (WARNING: will remove all player prefs)."))
			{
				PlayerPrefs.DeleteAll();
			}

			EditorGUILayout.LabelField("Make sure that each quest have a unique quest code to save and load its stats properly.");
			if(GUILayout.Button("Check for multiple quests with the same code."))
			{
				Quest[] Quests = FindObjectsOfType(typeof(Quest)) as Quest[];
				if(Quests.Length > 0)
				{
					foreach(Quest TempQuest in Quests) 
					{
						foreach(Quest CompareQuest in Quests) 
						{
							if(TempQuest.QuestCode == CompareQuest.QuestCode && CompareQuest != TempQuest)
							{
								Debug.LogError("Quest on object '"+TempQuest.gameObject.name+"' and object '"+CompareQuest.gameObject.name+"' have the same quest code: '"+CompareQuest.QuestCode+"'");
								return;
							}
						}
					}
				}
				Debug.Log("All is good, each quest has a unique quest code.");
			}


		}
	}
}