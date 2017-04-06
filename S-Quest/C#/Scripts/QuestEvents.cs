/*
    Custom Quest Events script created by Oussama Bouanani (SoumiDelRio)
*/

using UnityEngine;
using System.Collections;

public class QuestEvents : MonoBehaviour {

	public void OnPlayerAcceptQuest (string QuestCode) 
	{
		Debug.Log("Just accepted "+QuestCode);
	}
	
	public void OnPlayerAbandonQuest (string QuestCode) 
	{
		Debug.Log("Just abandonned "+QuestCode);
	}

	public void OnPlayerCompleteObjective (string QuestCode,int ObjectiveID,bool LastObjective)
	{
		Debug.Log("Just completed "+QuestCode+" objective ID: "+ObjectiveID.ToString()+" (Last objective: "+LastObjective.ToString()+").");
	}

	public void OnPlayerCompleteQuest (string QuestCode)  
	{
		Debug.Log("Just completed "+QuestCode);
	}
		
	public void OnObjectiveTimeOver (string QuestCode,int ObjectiveID) 
	{
		Debug.Log("Timer just went off for the objective ID: "+ObjectiveID.ToString()+ "from quest with code: "+QuestCode);
	}
}
