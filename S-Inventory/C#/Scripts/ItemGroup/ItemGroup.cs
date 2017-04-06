/*

Item Group Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemGroup : MonoBehaviour {

	[HideInInspector]
	public Transform MyTransform;
	
	public float MinDistance = 4.0f; //Minimum distance between the item group and the player for interaction.
	
	//Content:
	public bool RandomItemGroup = false; //If true, the script will randomly choose items from the array below for the item group.
	public int MaxAmount = 3; //Maximum amount of items to be randomly chosen for the item group.
	public class GroupItemVars
	{
		public Transform Obj;
		public int MaxRange = 2;
	}
	public GroupItemVars[] ContentItems;
	public Transform[] Content; //The items inside this group.
	
	[HideInInspector]
	public Transform ItemSelected; //This variable holds the selected item by the player in the GUI.
	
	//GUI:
	public ItemGroupUI Panel;
	[HideInInspector]
	public bool  ShowInfo = false; //Are we showing the instructions of opening the item group or not.
	
	//Sounds:
	public AudioClip Opening;
	public AudioClip Closing;
	
	
	//Other scripts:
	[HideInInspector]
	public InventoryManager InvManager;
	[HideInInspector]
	public InventoryUI InvGUI;
	[HideInInspector]
	public InventoryEvents CustomEvents;
	[HideInInspector]
	Animator ChestAnimator;
	
	
	void  Start (){
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		InvGUI = FindObjectOfType(typeof(InventoryUI)) as InventoryUI; //Get the Inventory Manager object/script.
		CustomEvents = FindObjectOfType(typeof(InventoryEvents)) as InventoryEvents;

		if(gameObject.GetComponent<Animator>())
		{
			ChestAnimator = gameObject.GetComponent<Animator>();
		}

		MyTransform = gameObject.transform; //Set the object tranform.

		if(RandomItemGroup == true)
		{
			Content = new Transform[0];
			for(int i = 0; i < ContentItems.Length; i++)
			{
				if(Content.Length < MaxAmount)
				{
					if(Random.Range(0,ContentItems[i].MaxRange) == 0)
					{
						List<Transform> TempContent = new List<Transform>(Content);
						TempContent.Remove(ContentItems[i].Obj);
						Content = TempContent.ToArray();
					}
				}
			}
		}
	}
	
	
	void  OnMouseDown (){
		float Distance;
		Distance = Vector3.Distance(MyTransform.position, InvManager.Player.transform.position); //Get the current distance between the player and the item group.
		if(Distance <= MinDistance && InvManager.PickupType == InventoryManager.PickupTypes.Mouse && Content.Length > 0 && Panel.ActiveItemGroup == null) //Check if that distance is below or equal to the minimum distance. And the player is using the mouse to pick up items and the content still has items.
		{
			TriggerItemGroup(); //Show the container GUI.
			if(Opening) GetComponent<AudioSource>().PlayOneShot(Opening);
		}
	}
	
	void  Update (){  
		float Distance;
		Distance = Vector3.Distance(MyTransform.position, InvManager.Player.transform.position); //Get the current distance between the player and the item group.
		if(Distance <= MinDistance) //Check if that distance is below or equal to the minimum distance and the content still has items.
		{
			if(Input.GetKey(InvManager.ActionKey) && InvManager.PickupType == InventoryManager.PickupTypes.Keyboard && Panel.ActiveItemGroup == null && Content.Length > 0) //Checking if the player pressed the key used to pick up items And the player is using the keyboard to pick up items.
			{
				TriggerItemGroup(); //Show the container GUI.
				if(Opening) GetComponent<AudioSource>().PlayOneShot(Opening);
			}
			if(ShowInfo == false)
			{
				Panel.ShowToggleInfo();//Show instructions on how to open the container menu.
				ShowInfo = true;
				if(Closing) GetComponent<AudioSource>().PlayOneShot(Closing);
			}
		}
		else //If you move away from the container it will no longer be displayed for you.
		{  
			if(Panel.ActiveItemGroup == this) 
			{
				if(Panel.ActiveItemGroup == this) TriggerItemGroup();
				if(Closing) GetComponent<AudioSource>().PlayOneShot(Closing);
			}
			if(ShowInfo == true)
			{
				Panel.HideToggleInfo();//Hide the instructions
				ShowInfo = false;
			}
		}
	}
	
	public void TriggerItemGroup ()
	{
		if(Panel.ActiveItemGroup == null)
		{
			//Activate the container UI:
			Panel.ActiveItemGroup = this;
			Panel.Panel.SetActive(true);
			Panel.HideToggleInfo();
			ShowInfo = false;
			
			Panel.RefreshItems();

			if(CustomEvents) CustomEvents.OnPlayerOpenItemGroup ();

			if(ChestAnimator != null)
			{
				ChestAnimator.SetBool("Open", true);
			}
		}
		else if(Panel.ActiveItemGroup == this)
		{
			//Desactivate the container UI:
			Panel.ActiveItemGroup = null;
			Panel.Panel.SetActive(false);    
			ShowInfo = false;

			if(CustomEvents) CustomEvents.OnPlayerCloseItemGroup ();

			if(ChestAnimator != null)
			{
				ChestAnimator.SetBool("Open", false);
			}
		}
	}
}