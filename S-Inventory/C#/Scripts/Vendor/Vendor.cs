/*

Vendor Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vendor : MonoBehaviour {

	public string VendorName;
	public float MinDistance = 8.0f; //Minimum distance between the player and the vendor for interaction.
	
	//Item for sell:
	public Transform[] ForSale; //The items for sell.
	[HideInInspector]
	public Transform[] ItemsToSell;
	public string[] AcceptedItems;
	
	//GUI:
	public VendorUI Panel;
	[HideInInspector]
	public bool ShowInfo = false; //Are we showing the instructions of interacting with a vendor or not.
	
	
	//Sounds:
	public AudioClip Opening;
	public AudioClip Closing;
	public AudioClip ItemSold;
	public AudioClip CantAfford;
	
	[HideInInspector]
	public Transform MyTransform;
	
	//Other scripts:
	[HideInInspector]
	InventoryManager InvManager;
	[HideInInspector]
	InventoryEvents CustomEvents;
	[HideInInspector]
	NotificationUI MsgUI;
	
	
	void  Start (){
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		CustomEvents = FindObjectOfType(typeof(InventoryEvents)) as InventoryEvents;
		MsgUI = FindObjectOfType(typeof(NotificationUI)) as NotificationUI;
		MyTransform = gameObject.transform; //Set the object tranform.
	}
	
	
	void  OnMouseDown (){
		float Distance;
		Distance = Vector3.Distance(MyTransform.position, InvManager.Player.transform.position); //Get the current distance between the player and the vendor.
		if(Distance <= MinDistance && InvManager.PickupType == InventoryManager.PickupTypes.Mouse && ForSale.Length > 0 && Panel.ActiveVendor == null) //Check if that distance is below or equal to the minimum distance. And the player is using the mouse to pick up items and there are items to buy.
		{
			TriggerVendor(); //Show the vendor GUI.
			GetComponent<AudioSource>().PlayOneShot(Opening);
		}
	}
	
	void  Update (){  
		float Distance;
		Distance = Vector3.Distance(MyTransform.position, InvManager.Player.transform.position); //Get the current distance between the player and the vendor.
		if(Distance <= MinDistance && ForSale.Length > 0) //Check if that distance is below or equal to the minimum distance and the content still has items.
		{
			if(Panel.ActiveVendor == null)
			{
				if(Input.GetKey(InvManager.ActionKey) && InvManager.PickupType == InventoryManager.PickupTypes.Keyboard) //Checking if the player pressed the key used to pick up items And the player is using the keyboard to pick up items.
				{
					TriggerVendor(); //Show the vendor GUI.
					GetComponent<AudioSource>().PlayOneShot(Opening);
				}
				if(ShowInfo == false)
				{
					Panel.ShowToggleInfo();//Show instructions on how to open the vendor menu.
					ShowInfo = true;
				}
			}
		}
		else //If you move away from the vendor it will no longer be displayed for you.
		{ 
			if(Panel.ActiveVendor == this)
			{
				TriggerVendor();
				GetComponent<AudioSource>().PlayOneShot(Closing);

			}
			if(ShowInfo == true)
			{
				Panel.HideToggleInfo();//Hide the instructions
				ShowInfo = false;
			}
		}
	}
	
	public void  TriggerVendor (){
		if(Panel.ActiveVendor == null)
		{
			//Activate the panel:
			Panel.ActiveVendor = this;
			Panel.BuyMenu = true;
			Panel.Panel.SetActive(true);
			Panel.OpenBuyMenu();
			Panel.HideToggleInfo();
			if(Panel.VendorName) Panel.VendorName.text = VendorName;
			ShowInfo = false;

			if(CustomEvents) CustomEvents.OnPlayerOpenVendor();
		}
		else if(Panel.ActiveVendor == this)
		{
			SellItems();
			
			//Desactivate the panel:
			Panel.ActiveVendor = null;
			Panel.Panel.SetActive(false);    
			ShowInfo = false;

			if(CustomEvents) CustomEvents.OnPlayerCloseVendor();
		}
	}
	
	//Purchasing items:
	public void  PurchaseItem ( Transform ItemSelected  ){
		Item SelectedItemScript = ItemSelected.GetComponent<Item>(); //Getting the item script.
		bool  HasCurrency = true;
		bool  HasAmount = true;
		bool  InvSpace = true;
		for(int i = 0; i < InvManager.Riches.Length; i++) //Starting a loop in the currencies types:
		{
			if(InvManager.Items < InvManager.MaxItems) //If we still have room for new item:  
			{
				if(InvManager.Riches[i].Name == SelectedItemScript.Currency)  //If the required currency type for this item exists in the inventory.
				{
					if(InvManager.Riches[i].Amount >= SelectedItemScript.CurrencyAmount) //And if the amount is enough 
					{
						InvManager.RemoveCurrency(SelectedItemScript.Currency, SelectedItemScript.CurrencyAmount); //Reduce the amount of this currency.
						Transform ItemClone;
						ItemClone = Instantiate (ItemSelected, ItemSelected.transform.position, ItemSelected.transform.rotation) as Transform;
						GetComponent<AudioSource>().PlayOneShot(ItemSold);
						if(SelectedItemScript.ItemType == 0)
						{
							InvManager.AddItem(ItemClone); //Add this item to the bag
						}
						if(SelectedItemScript.ItemType == 1)
						{
							InvManager.AddCurrency(ItemClone); //Add this currency to the inventory
						}
						
						HasCurrency = true;
						HasAmount = true;
						InvSpace = true;
						
						CustomEvents.OnPlayerBuyItem (ItemClone.GetComponent<Item>());
						return;
					}
					else
					{
						HasAmount = false;
					}
				} 
				else
				{
					HasCurrency = false;
				}
			}
			else
			{
				InvSpace = false;
			}
		}
		if(InvSpace == false) //If there isn't room for the new item:
		{
			if(MsgUI != null) MsgUI.SendMsg("Inventory is full!");
		}  
		else if(HasCurrency == false) //If we don't have the required type of currency.
		{
			if(MsgUI != null) MsgUI.SendMsg("You don't have " + SelectedItemScript.Currency + ".");
		}
		else if(HasAmount == false) //If we don't have the enough amount of currency.
		{
			if(MsgUI != null) MsgUI.SendMsg("You don't have enough " + SelectedItemScript.Currency + ".");
			if(InvManager.AudioSC) InvManager.AudioSC.PlayOneShot(CantAfford);
		}
	}
	
	//Selling items:
    public void  SellItems (){
		foreach(Transform s in ItemsToSell) //Starting a loop in the items to sell content:
		{
			if(ItemsToSell.Length > 0) //If there are items to sell
			{
				Item SoldScript = s.GetComponent<Item>(); //Getting the item script.
				//Give currency for everything
				for(int c = 0; c < InvManager.Riches.Length; c++) //Starting a loop in the currencies types:
				{
					if(InvManager.Riches[c].Name == SoldScript.Currency)  //If the required currency type for this item exists in the inventory.
					{
						s.gameObject.SetActive(true);
						GetComponent<AudioSource>().PlayOneShot(ItemSold);
						InvManager.RemoveCurrency(SoldScript.Currency, -SoldScript.CurrencyAmount); //Increase the currency amount!
						
						CustomEvents.OnPlayerSellItem (SoldScript);
					}
					else
					{
						Debug.LogError("The currency you want to add to the inventory isn't defined, please define it in the InventoryManager.js.");
					}    
				}    
				//Removing all the items:
				List<Transform> TempContent3 = new List<Transform>(ItemsToSell);
				TempContent3.Remove(s);
				ItemsToSell = TempContent3.ToArray();
				
				Destroy(s.gameObject);
			}    
		}
	}
	
	public void  TakeBackItem ( Transform ItemSelected  ){
		if(InvManager.Items < InvManager.MaxItems) //If we still have room for new item:  
		{ 
			//Remove the item from the items to sell:
			List<Transform> TempContent2 = new List<Transform>(ItemsToSell);
			TempContent2.Remove(ItemSelected);
			ItemsToSell = TempContent2.ToArray();
			
			InvManager.AddItem(ItemSelected); //Add this item back to the inventory.
		}
		else
		{
			if(MsgUI != null) MsgUI.SendMsg("Inventory is full!");
		}
	}
	
}