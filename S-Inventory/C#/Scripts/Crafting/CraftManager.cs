/*
    Craft Manager script created by SoumiDelRio.
*/

using UnityEngine;
using System.Collections;

public class CraftManager : MonoBehaviour {

	public KeyCode Key = KeyCode.R; //Key that shows/hides the crafting menu.
	
	//Recipes:
	[System.Serializable]
	public class Combinations //Items needed for crafting.
	{
		public string Name;
		public int Quantity = 1; //Only for consumable items, for non-consumable items, leave this value set to 1.
		[HideInInspector]
		public bool IsTaken = false; //Did we put an inventory item on this slot?
		[HideInInspector]
		public Transform Item; //The item's game object.
		public bool Destroy = true;
	}
	
	//Each recipe requires combinations and the result item + a little description.
	[System.Serializable]
	public class RecipeVars
	{
		public Combinations[] ItemsNeeded;
		public GameObject Result;
		public string Name = "Recipe";
		[HideInInspector]
		public int ItemsAmount = 0;
		public float TimeRequired = 1.0f;
		[HideInInspector]
		public float Timer = 0.0f;
		[HideInInspector]
		public bool ItemReady = false;
		public bool CurrencyRequired = false;
		public string CurrencyName;
		public int CurrencyAmount;
	}
	public RecipeVars[] Recipes;
	
	[HideInInspector]
	public int RecipeID = 0; //Current recipe ID.

	[HideInInspector]
	public CraftUI InvCraft;

	public AudioClip FinishedCrafting;

	void Awake()
	{
		InvCraft = FindObjectOfType (typeof(CraftUI)) as CraftUI;
	}

	void  Update (){
		if(Input.GetKeyDown(Key)) //Check if the player pressed the I key which enables/disables the craft menu.
		{
			InvCraft.ToggleCrafting();
		}
	}
}