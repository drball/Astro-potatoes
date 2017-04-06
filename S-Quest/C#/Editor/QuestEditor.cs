using UnityEngine;
using UnityEditor;

namespace S_Quest
{
	[CustomEditor(typeof(Quest))] 
	public class QuestCEditor : Editor {

		public SerializedProperty Condition;
		public Rect MyRect;
		public bool showChildren;
		public int LastID;

		//Quest custom editor: makes creating quests easy and simple.
		public override void OnInspectorGUI() 
		{
			Quest MyTarget = (Quest) target;

			EditorGUILayout.Space();

			MyTarget.QuestCode = EditorGUILayout.TextField("Quest Code:", MyTarget.QuestCode);
			EditorGUILayout.LabelField("Parent Quest (Optional): ");

			MyTarget.ParentQuest = EditorGUILayout.ObjectField(MyTarget.ParentQuest, typeof(Quest), true) as Quest;
			MyTarget.ActivateOnStart = EditorGUILayout.Toggle("Activate On Start:", MyTarget.ActivateOnStart);
			MyTarget.Repeatable = EditorGUILayout.Toggle("Repeatable Quest?", MyTarget.Repeatable);
			MyTarget.NonAbandonable = EditorGUILayout.Toggle("Non Abandonable?", MyTarget.NonAbandonable);
			MyTarget.SaveQuestProgress = EditorGUILayout.Toggle("Save Quest Objective Progress?", MyTarget.SaveQuestProgress);

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			MyTarget.QuestTitle = EditorGUILayout.TextField("Quest Title:", MyTarget.QuestTitle);
			EditorGUILayout.LabelField("Quest Objective (Short):");
			MyTarget.QuestObjective = EditorGUILayout.TextArea(MyTarget.QuestObjective, GUILayout.Height(30));
			EditorGUILayout.LabelField("Quest Description/Story (Long):"); 
			MyTarget.Description = EditorGUILayout.TextArea(MyTarget.Description, GUILayout.Height(80));
			MyTarget.ShowAfterFinished = EditorGUILayout.Toggle("Show When Completed:", MyTarget.ShowAfterFinished);
			if(MyTarget.ShowAfterFinished == true)
			{
				EditorGUILayout.LabelField("Completed Quest Message (Short):");
				MyTarget.FinishedQuestMessage = EditorGUILayout.TextArea(MyTarget.FinishedQuestMessage, GUILayout.Height(30));
			}    
			EditorGUILayout.LabelField("Speech Audio:");
			MyTarget.Speech = EditorGUILayout.ObjectField(MyTarget.Speech, typeof(AudioClip), true) as AudioClip;

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			MyTarget.QuestGiverName = EditorGUILayout.TextField("Quest Giver Name:", MyTarget.QuestGiverName);
			EditorGUILayout.LabelField("Quest Giver Image:");
			MyTarget.QuestGiverSprite = EditorGUILayout.ObjectField(MyTarget.QuestGiverSprite, typeof(Sprite), true) as Sprite;

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();


			Condition = serializedObject.FindProperty("Goal");

			Condition.arraySize = EditorGUILayout.IntField("Condition(s):", Condition.arraySize);
			serializedObject.ApplyModifiedProperties();
			if(Condition.arraySize < 1) Condition.arraySize = 1; serializedObject.ApplyModifiedProperties();

			if(Condition.arraySize >= 1)
			{

				for(int i = 0; i < Condition.arraySize; i++)
				{
					int x=  i + 1;
					EditorGUILayout.LabelField("Condition " + x.ToString() + ":");
					EditorGUILayout.LabelField("Condition Type:");
					MyTarget.Goal[i].Type = (Quest.QuestTypes)EditorGUILayout.EnumPopup("", MyTarget.Goal[i].Type);
					MyTarget.Goal[i].Message = EditorGUILayout.TextField("Condition Message: ", MyTarget.Goal[i].Message); 
					if(MyTarget.Goal[i].Type == Quest.QuestTypes.Goto)
					{
						MyTarget.Goal[i].Destination = EditorGUILayout.Vector3Field("Destination:",MyTarget.Goal[i].Destination);
						MyTarget.Goal[i].Range = EditorGUILayout.FloatField("Range:",MyTarget.Goal[i].Range);
						MyTarget.Goal[i].TimeToStay = EditorGUILayout.FloatField("Time To Stay:",MyTarget.Goal[i].TimeToStay);
					} 
					else if(MyTarget.Goal[i].Type == Quest.QuestTypes.Collection || MyTarget.Goal[i].Type == Quest.QuestTypes.Elimination)
					{
						MyTarget.Goal[i].ItemName = EditorGUILayout.TextField("Item/Target Name:", MyTarget.Goal[i].ItemName);
						MyTarget.Goal[i].AmountRequired = EditorGUILayout.IntField("Amount:", MyTarget.Goal[i].AmountRequired);
					} 
					else if(MyTarget.Goal[i].Type == Quest.QuestTypes.Meeting)
					{
						EditorGUILayout.LabelField("Quest Giver To Meet:");

						MyTarget.Goal[i].Target = EditorGUILayout.ObjectField(MyTarget.Goal[i].Target, typeof(Quest), true) as Quest;

						EditorGUILayout.LabelField("Message On Arrival:");
						MyTarget.Goal[i].ArrivalMessage = EditorGUILayout.TextArea(MyTarget.Goal[i].ArrivalMessage, GUILayout.Height(30));
					}  
					else if(MyTarget.Goal[i].Type == Quest.QuestTypes.Follow)
					{
						EditorGUILayout.LabelField("Object To Follow:");
						MyTarget.Goal[i].ObjToFollow = EditorGUILayout.ObjectField(MyTarget.Goal[i].ObjToFollow, typeof(GameObject), true) as GameObject;

						MyTarget.Goal[i].FollowTime = EditorGUILayout.FloatField("Follow Time:",MyTarget.Goal[i].FollowTime);
						MyTarget.Goal[i].MinDistance = EditorGUILayout.FloatField("Min Follow Distance:",MyTarget.Goal[i].MinDistance);
						MyTarget.Goal[i].MaxDistance = EditorGUILayout.FloatField("Max Follow Distance:",MyTarget.Goal[i].MaxDistance);
					}  
					else if(MyTarget.Goal[i].Type == Quest.QuestTypes.PressAKey)
					{
						EditorGUILayout.LabelField("Key To Press:");
						MyTarget.Goal[i].KeyToPress = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.Goal[i].KeyToPress);
						MyTarget.Goal[i].HoldKey = EditorGUILayout.Toggle("Hold Key? ",MyTarget.Goal[i].HoldKey);
						if (MyTarget.Goal [i].HoldKey == true) {
							MyTarget.Goal[i].HoldTime = EditorGUILayout.FloatField("Hold Key Time:",MyTarget.Goal[i].HoldTime);
						}
					} 

					EditorGUILayout.Space();

					MyTarget.Goal[i].UseTimeLimit = EditorGUILayout.Toggle("Use Time Limit?", MyTarget.Goal[i].UseTimeLimit);
					if (MyTarget.Goal [i].UseTimeLimit == true) {
						MyTarget.Goal [i].TimeLimit = EditorGUILayout.FloatField ("Time Limit (in seconds):", MyTarget.Goal [i].TimeLimit);
						MyTarget.Goal[i].AbandonQuestOnTimeLimit = EditorGUILayout.Toggle("Abandon Quest When Time Is Over?", MyTarget.Goal[i].AbandonQuestOnTimeLimit);
					}
				}

			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			MyTarget.ReturnToGiver = EditorGUILayout.Toggle("Return To Quest Giver:", MyTarget.ReturnToGiver);
			MyTarget.RewardPlayer = EditorGUILayout.Toggle("Reward Player:", MyTarget.RewardPlayer);
			if(MyTarget.RewardPlayer == true)
			{
				MyTarget.XP = EditorGUILayout.IntField("XP:", MyTarget.XP);
			} 

			EditorGUILayout.LabelField("Reward Item: Requires S-Inventory");

			MyTarget.Item = EditorGUILayout.ObjectField(MyTarget.Item, typeof(GameObject), true) as GameObject;


			if(GUILayout.Button("Reset Quest"))
			{
				PlayerPrefs.SetInt(MyTarget.QuestCode, 0);
			}
		}
	}
}