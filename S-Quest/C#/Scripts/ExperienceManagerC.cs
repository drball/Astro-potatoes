/*

XP and Level system Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;

public class ExperienceManagerC : MonoBehaviour {
	
	//This is just a basic experience and level system, you can expand this however you want.
	//I just created this to show you how to reward a player with experience points after completing a quest.
	
	[HideInInspector]
	public int Level = 1; //Current player's level.
	[HideInInspector]
	public float XP = 0; //Current player's experience points.
	public int MaxLevel = 20; //Max level that the player could achieve.
	public int Level1XP = 100; //Level 1 experience points, need this to create all the other levels.
	
	
	public bool SaveAndLoad = true; //Save and load player's level and experience points?
	
	//Sounds:
	public AudioClip LevelUpSound;

	XPUI ExperienceUI;
	
	void  Awake (){
		XP = 0;
		ExperienceUI = FindObjectOfType (typeof(XPUI)) as XPUI;

		//Load player's level.
		if(SaveAndLoad == true)
		{
			XP = PlayerPrefs.GetFloat("XP");
			Level = PlayerPrefs.GetInt("Level");
			if(Level == 0)
			{
				Level = 1;
			}
		}

		ExperienceUI.SetXPBarUI ();
	}

	//Please use the fucntion when you want to add experience to the player.
	public void AddXP ( int Amount  ){
		XP += Amount;

		if(Level < MaxLevel) //If the player didn't reach the max level yet.
		{
			if(XP >= Level*Level1XP) //If the player's experience is high or equal to the required experience points to level up.
			{
				//Level up!
				XP -= Level*Level1XP; 
				Level++;
				
				//Play the level up sound.
				if(LevelUpSound) GetComponent<AudioSource>().PlayOneShot(LevelUpSound);
			}
		}
		
		//If this is the final level:
		if(Level == MaxLevel)
		{
			//Keep resetting the player's experience points.
			XP = 0;
		}
		
		//Save player's level and xp:
		if(SaveAndLoad == true)
		{
			PlayerPrefs.SetFloat("XP", XP);
			PlayerPrefs.SetInt("Level", Level);
		}

		ExperienceUI.SetXPBarUI ();
	}
}