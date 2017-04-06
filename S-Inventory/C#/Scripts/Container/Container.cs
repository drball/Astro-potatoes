/*

Container script created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : MonoBehaviour {
	
	//Container vars:
	public int ContainerID = 0;
	public float MinDistance = 4.0f; //Minimum distance between the item group and the player for interaction.
	
	//Items:
	public class ContainerSlot
	{
		public bool  IsTaken = false;
		public Transform Item;
		public bool  Dragged = false;
		[HideInInspector]
		public GameObject UI;
	}
	[HideInInspector]
	public ContainerSlot[] Slots;//container slots
	public int MaxItems = 20; //Maximum number of items that can be held inside the bag.
	[HideInInspector]
	public int Items = 0; //Current number of items inside the bag.

	public bool ReArrangeItems = false;

	public Transform Player;
	
	//GUI:
	public ContainerUI Panel;
	[HideInInspector]
	public bool  ShowInfo = false; //Are we showing the instructions of interacting with a vendor or not.
	
	//Other vars:
	[HideInInspector]
	Transform MyTransform;
	public GameObject DefaultItem;
	
	public KeyCode Key= KeyCode.E; //The key used to show/hide container GUI.
	
	//Other scripts/objects:
	[HideInInspector]
	public InventoryManager InvManager;
	[HideInInspector]
	public InventoryEvents CustomEvents;
	
	//Sounds:
	public AudioClip OpenContainer;
	public AudioClip CloseContainer;
	
	public bool  SaveAndLoad = false;
	
	void  Awake (){
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the container Manager object/script.
		CustomEvents = FindObjectOfType(typeof(InventoryEvents)) as InventoryEvents;
		
		MyTransform = gameObject.transform; //Set the container transform.
		Items = 0;
		
		//Setting the container size:
		Slots = new ContainerSlot[MaxItems];
		
		for(int i = 0; i < MaxItems; i++)
		{
			Slots[i] = new ContainerSlot();
		}
		
		if(SaveAndLoad == true)
		{
			LoadItems();
		} 
	}
	
	void  Update (){
		float Distance;
		Distance = Vector3.Distance(MyTransform.position, InvManager.Player.transform.position); //Get the current distance between the player and the container.
		if(Distance <= MinDistance) //Check if that distance is below or equal to the minimum distance and the content still has items.
		{
			if(Input.GetKey(InvManager.ActionKey) && InvManager.PickupType == InventoryManager.PickupTypes.Keyboard && Panel.ActiveContainer == null) //Checking if the player pressed the key used to pick up items And the player is using the keyboard to pick up items.
			{
				TriggerContainer(); //Show the container GUI.
				if(OpenContainer) GetComponent<AudioSource>().PlayOneShot(OpenContainer);
			}
			if(ShowInfo == false)
			{
				Panel.ShowToggleInfo();//Show instructions on how to open the container menu.
				ShowInfo = true;
				if(CloseContainer) GetComponent<AudioSource>().PlayOneShot(CloseContainer);
			}
		}
		else //If you move away from the container it will no longer be displayed for you.
		{  
			if(Panel.ActiveContainer == this) 
			{
				if(Panel.ActiveContainer == this) TriggerContainer();
				if(CloseContainer) GetComponent<AudioSource>().PlayOneShot(CloseContainer);
			}
			if(ShowInfo == true)
			{
				Panel.HideToggleInfo();//Hide the instructions
				ShowInfo = false;
			}
		}
	}
	
	
	void  OnMouseDown (){
		float Distance;
		Distance = Vector3.Distance(MyTransform.position, InvManager.Player.transform.position); //Get the current distance between the player and the container.
		if(Distance <= MinDistance && InvManager.PickupType == InventoryManager.PickupTypes.Mouse && Panel.ActiveContainer == null) //Check if that distance is below or equal to the minimum distance. And the player is using the mouse to pick up items and the content still has items.
		{
			TriggerContainer(); //Show the container GUI.
			if(OpenContainer) GetComponent<AudioSource>().PlayOneShot(OpenContainer);
		}
	}
	/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
	
	public void TriggerContainer ()
	{
		if(Panel.ActiveContainer == null)
		{
			//Activate the container UI:
			Panel.ActiveContainer = this;
			Panel.Panel.SetActive(true);
			Panel.HideToggleInfo();
			ShowInfo = false;

			if(ReArrangeItems == true)
			{
				ArrangeItems();
			}
			//Creaet slots and show items:
			Panel.ChangeSlots ();
			Panel.RefreshItems();

			if(CustomEvents) CustomEvents.OnPlayerOpenContainer ();
		}
		else if(Panel.ActiveContainer == this)
		{
			if(Panel.DraggingItem == true)
			{
				Slots[Panel.DragID].UI.gameObject.GetComponent<SlotUI>().DisableItemDrag ();
			}

			SaveItems();
			
			//Desactivate the container UI:
			Panel.ActiveContainer = null;
			Panel.Panel.SetActive(false);    
			ShowInfo = false;

			if(CustomEvents) CustomEvents.OnPlayerCloseContainer ();
		}
	}
	
	/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

	
	/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

	public void ArrangeItems ()
	{
		int EmptySlot = -1;
		bool Found = false;
		if(Items < MaxItems)
		{
			for(int i = 0; i < MaxItems; i++) //Starting a loop in the slots of the inventory:
			{
				if(Slots[i].IsTaken == true)
				{
					if(EmptySlot != -1)
					{
						MoveItem(i, EmptySlot);
						ArrangeItems ();
					}
				}
				if(Found == false && Slots[i].IsTaken == false)
				{
					EmptySlot = i;
					Found = true;
				}
			}
		}
	}

	void  LoadItems (){
		int TempInt= PlayerPrefs.GetInt("ContainerSavedItems"+ContainerID.ToString()); //Getting the number of the saved items.
		GameObject TempObj;
		for(int i = 1; i < TempInt; i++) //Starting a loop 
		{
			if(Resources.Load (PlayerPrefs.GetString("ContainerName" + i.ToString())+"C", typeof(GameObject)))
			{
				TempObj = Instantiate(Resources.Load (PlayerPrefs.GetString("ContainerName" + i.ToString())+"C", typeof(GameObject)),MyTransform.position,MyTransform.rotation) as GameObject;
			}
			else
			{
				TempObj = Instantiate(DefaultItem,MyTransform.position,MyTransform.rotation) as GameObject;
			}
			Item ItemScript = TempObj.GetComponent<Item>(); //Getting the item script.
			ItemScript.ItemType = PlayerPrefs.GetInt("ContainerItemType" + i.ToString() + ContainerID.ToString()); //Loading the item's type
			ItemScript.MinDistance = PlayerPrefs.GetFloat("ContainerMinDistance" + i.ToString() + ContainerID.ToString()); //Loading the item's pickup minimum distance.
			ItemScript.Name = PlayerPrefs.GetString("ContainerName" + i.ToString() + ContainerID.ToString()); //Loading the item's name.
			ItemScript.Amount = PlayerPrefs.GetInt("ContainerAmount" + i.ToString() + ContainerID.ToString()); //Loading the item's amount.
			ItemScript.Weight = PlayerPrefs.GetInt("ContainerWeight" + i.ToString(), 1); //Loading the item's weight
			ItemScript.ShortDescription = PlayerPrefs.GetString("ContainerShortDescription" + i.ToString() + ContainerID.ToString()); //Loading the item's short description.
			ItemScript.Icon = Resources.Load(ItemScript.Name, typeof(Sprite)) as Sprite;
			ItemScript.Currency = PlayerPrefs.GetString("ContainerCurrency" + i.ToString() + ContainerID.ToString()); //Loading the item's currency type.
			ItemScript.CurrencyAmount = PlayerPrefs.GetInt("ContainerCurrencyAmount" + i.ToString() + ContainerID.ToString()); //Loading the item's currency amount.
			ItemScript.EquipmentSlot = PlayerPrefs.GetString("ContainerEquipmentSlot" + i.ToString() + ContainerID.ToString()); //Loading the item's equipment slot.
			ItemScript.SkillSlot = PlayerPrefs.GetString("ContainerSkillSlot" + i.ToString() + ContainerID.ToString()); //Loading the item's skill bar slot.
			//Is the item stackable? 
			if(PlayerPrefs.GetInt("ContainerIsStackable" + i.ToString() + ContainerID.ToString()) == 0)
			{
				ItemScript.IsStackable = false;
			}
			else
			{
				ItemScript.IsStackable = true;
			}   
			ItemScript.MaxAmount = PlayerPrefs.GetInt("ContainerMaxAmount" + i.ToString() + ContainerID.ToString()); //Loading the item's maximum amount.

			//Loading the item's equipment attributes:
			if(PlayerPrefs.GetString("EqHasAttributes" + i.ToString()) == "Yes")
			{
				ItemScript.Attributes = new Item.ItemAttributes[PlayerPrefs.GetInt("EqAttributesAmount" + i.ToString())];
				int a = 0;
				for(a = 0; a < ItemScript.Attributes.Length; a++)
				{
					ItemScript.Attributes[a] = new Item.ItemAttributes();
				}
				
				for(a = 0; a < ItemScript.Attributes.Length; a++)
				{
					ItemScript.Attributes[a].Name = PlayerPrefs.GetString("EqAttributeName" + a.ToString() + i.ToString());
					ItemScript.Attributes[a].Value = PlayerPrefs.GetInt("EqAttributeValue" + a.ToString() + i.ToString());
				}
			}
			
			LoadItem(TempObj.transform, PlayerPrefs.GetInt("ContainerItemSlot" + i.ToString() + ContainerID.ToString()));
		}
	}
	
	
	void LoadItem( Transform Item ,   int ItemSlot  ) //Adding an item inside the bag.
	{
		Item NewItemScript = Item.GetComponent<Item>(); //Getting the new item script.
		bool  Match = false;
		
		for(int t = 0; t < MaxItems; t++) //Starting a loop in the slots of the container:
		{
			if(Slots[t].IsTaken == true && NewItemScript.IsStackable == true) //Checking if there's an item in this slot and that the item is stackable.
			{
				Item ItemScript = Slots[t].Item.GetComponent<Item>(); //Getting the item script from the items inside the bag.
				if(NewItemScript.Name == ItemScript.Name) //Checking if the type of the new item matches with another item already in the bag.
				{
					if(ItemScript.Amount + NewItemScript.Amount <= ItemScript.MaxAmount)
					{
						ItemScript.Amount += NewItemScript.Amount; //Increase the amount of the item in the bag.
						Destroy(Item.gameObject); //Destroying the item.
						
						Match = true;
					}
				}
				if(Match)
				{
					return;
				}
			}
		}
		if(!Match) //Checking if there's no match with another item inside the bag.
		{
			if(Items < MaxItems) //Checking if the bag still have more space fore new items.
			{
				if(Slots[ItemSlot].IsTaken == false)
				{
					//Adding the item to this slot.
					Slots[ItemSlot].Item = Item;
					Slots[ItemSlot].IsTaken = true;
					Items++; //Increasing the items cout.
					
					//Changing the settings of the item and parenting it to the container object.
					if(Item.transform.GetComponent<Collider>() !=null)
					{
						Item.transform.GetComponent<Collider>().isTrigger = true;
					}
					Item.transform.GetComponent<Renderer>().enabled = false;
					Item.transform.parent = MyTransform;
					Item.transform.localPosition  = Vector3.zero;
					Item.gameObject.SetActive(false);
					
					return;
				}
			}  
		}                 
	}
	
	
	
	public void  SaveItems (){
		int TempVar = 1;
		for(int i = 0; i < MaxItems; i++) //Starting a loop in the slots of the container:
		{
			if(Slots[i].IsTaken == true)
			{
				Item ItemScript = Slots[i].Item.GetComponent<Item>(); //Getting the item script.
				
				PlayerPrefs.SetInt("ContainerItemSlot" + TempVar.ToString() + ContainerID.ToString(), i); //Saving the item's slot number.
				PlayerPrefs.SetInt("ContainerItemType" + TempVar.ToString() + ContainerID.ToString(), ItemScript.ItemType); //Saving the item's type
				PlayerPrefs.SetFloat("ContainerMinDistance" + TempVar.ToString() + ContainerID.ToString(), ItemScript.MinDistance); //Saving the item's pickup minimum distance.
				PlayerPrefs.SetString("ContainerName" + TempVar.ToString() + ContainerID.ToString(), ItemScript.Name); //Saving the item's name.
				PlayerPrefs.SetInt("ContainerAmount" + TempVar.ToString() + ContainerID.ToString(), ItemScript.Amount); //Saving the item's amount.
				PlayerPrefs.SetInt("ContaineWeight" + TempVar.ToString(), ItemScript.Amount); //Saving the item's weight
				PlayerPrefs.SetString("ContainerShortDescription" + TempVar.ToString() + ContainerID.ToString(), ItemScript.ShortDescription); //Saving the item's short description.
				PlayerPrefs.SetString("ContainerCurrency" + TempVar.ToString() + ContainerID.ToString(), ItemScript.Currency); //Saving the item's currency type.
				PlayerPrefs.SetInt("ContainerCurrencyAmount" + TempVar.ToString() + ContainerID.ToString(), ItemScript.CurrencyAmount); //Saving the item's currency amount.
				PlayerPrefs.SetString("ContainerEquipmentSlot" + TempVar.ToString() + ContainerID.ToString(), ItemScript.EquipmentSlot); //Saving the item's equipment slot.
				PlayerPrefs.SetString("ContainerSkillSlot" + TempVar.ToString() + ContainerID.ToString(), ItemScript.SkillSlot); //Saving the item's skill bar slot.
				//Is the item stackable? 
				if(ItemScript.IsStackable == true)
				{
					PlayerPrefs.SetInt("ContainerIsStackable" + TempVar.ToString() + ContainerID.ToString(), 1);
				}
				else
				{
					PlayerPrefs.SetInt("ContainerIsStackable" + TempVar.ToString() + ContainerID.ToString(), 0); 
				}   
				PlayerPrefs.SetInt("ContainerMaxAmount" + TempVar.ToString() + ContainerID.ToString(), ItemScript.MaxAmount); //Saving the item's maximum amount.

				//Saving the item's equipment attributes:
				if(ItemScript.Attributes.Length > 0)
				{ 
					PlayerPrefs.SetString("EqHasAttributes" + TempVar.ToString(), "Yes");
					PlayerPrefs.SetInt("EqAttributesAmount" + TempVar.ToString(), ItemScript.Attributes.Length);
					for(int x = 0; x < ItemScript.Attributes.Length; x++)
					{
						PlayerPrefs.SetString("EqAttributeName" + x.ToString() + TempVar.ToString(), ItemScript.Attributes[x].Name);
						PlayerPrefs.SetInt("EqAttributeValue" + x.ToString() + TempVar.ToString(), ItemScript.Attributes[x].Value);
					}
				}
				else
				{
					PlayerPrefs.SetString("EqHasAttributes" + TempVar.ToString(), "No");
				}

				TempVar++;
			}    
		}
		PlayerPrefs.SetInt("ContainerSavedItems"+ContainerID.ToString(), TempVar);
	}
	
	
	/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
	
	public void  AddItem ( Transform Item ,   int SlotID  ){
		Item NewItemScript = Item.GetComponent<Item>(); //Getting the new item script.
		bool  Match = false;
		
		for(int t = 0; t < MaxItems; t++) //Starting a loop in the slots of the inventory:
		{
			if(Slots[t].IsTaken == true && NewItemScript.IsStackable == true) //Checking if there's an item in this slot and that the item is stackable.
			{
				Item ItemScript = Slots[t].Item.GetComponent<Item>(); //Getting the item script from the items inside the bag.
				if(NewItemScript.Name == ItemScript.Name) //Checking if the type of the new item matches with another item already in the bag.
				{
					if(ItemScript.Amount + NewItemScript.Amount <= ItemScript.MaxAmount)
					{
						ItemScript.Amount += NewItemScript.Amount; //Increase the amount of the item in the bag.
						ItemScript.CurrencyAmount += NewItemScript.CurrencyAmount;
						
						InvManager.GetComponent<AudioSource>().PlayOneShot(InvManager.NewItem); //a sound.
						
						Destroy(Item.gameObject); //Destroying the item.
						
						Match = true;
					}
				}
				if(Match)
				{
					Panel.RefreshItems (); if(SaveAndLoad == true) SaveItems();
					return;
				}
			}
		}
		if(!Match) //Checking if there's no match with another item inside the bag.
		{
			if(Items < MaxItems) //Checking if the bag still have more space fore new items.
			{
				for(int b = 0; b < MaxItems; b++) //Starting a loop in the slots of the inventory:
				{
					if(Slots[b].IsTaken == false) //Checking if there is not an item in this slot.
					{
						//Adding the item to the content of the bag.
						Slots[b].Item = Item;
						Slots[b].IsTaken = true;
						Items++; //Increasing the items cout.
						
						//Changing the settings of the item and parenting it to the inventory object.
						if(Item.transform.GetComponent<Collider>() !=null)
						{
							Item.transform.GetComponent<Collider>().isTrigger = true;
						}
						Item.transform.GetComponent<Renderer>().enabled = false;
						Item.transform.parent = MyTransform;
						Item.transform.localPosition  = Vector3.zero;
						Item.gameObject.SetActive(false);
						
						InvManager.GetComponent<AudioSource>().PlayOneShot(InvManager.NewItem); //a sound.
						
						MoveItem(b,SlotID);
						Panel.RefreshItems (); if(SaveAndLoad == true) SaveItems();
						return;
					}
				}
			}  
		}
	}
	
	public void MoveItem ( int CurrentSlot ,   int TargetSlot  ) //Adding an item inside the bag.
	{
		if(TargetSlot > MaxItems || TargetSlot < 0) //If the slot does not exists
		{
			return;
		}
		if(Slots[TargetSlot].IsTaken == false) //If the target slot is empty.
		{
			//Move the item to a new slot:
			Slots[TargetSlot].IsTaken = true;
			Slots[TargetSlot].Item = Slots[CurrentSlot].Item;
			//Clear the current item's slot.
			Slots[CurrentSlot].IsTaken = false; 
			Slots[CurrentSlot].Item = null;
			Panel.RefreshItems (); if(SaveAndLoad == true) SaveItems();
			return;
		}
		else //If the target slot is full!
		{
			if(Slots[TargetSlot] == Slots[CurrentSlot])
			{
				return;
			}
			else
			{
				return;
			}    
		}        
	}
	
	public void  RemoveItem ( Transform Item ,   int Amount  ){
		bool  Removed = false;
		for(int a = 0; a < MaxItems; a++) //Starting a loop in the slots of the inventory:   
		{
			if(Slots[a].IsTaken == true) //Checking if there's an item in this slot.
			{
				Item ItemScript = Slots[a].Item.GetComponent<Item>(); //Getting the item script from the items inside the bag.   
				if(Slots[a].Item == Item) //Check if the item exists in the bag.
				{
					if(InvManager.DropType == InventoryManager.DropTypes.Spawn) 
					{
						
						Transform CloneItem; //Create a temporary object to move the removed item to.
						CloneItem = (Transform)Instantiate(Item, Item.position, Item.rotation); //Clone the original item.
						Item Script = CloneItem.GetComponent<Item>(); //Get the item script.
						
						//Set the cloned item settings:
						Script.Amount = Amount;
						ItemScript.CurrencyAmount -= Amount*(ItemScript.CurrencyAmount/(ItemScript.Amount));
						
						//Activate the cloned item and spawn it in real world.
						CloneItem.transform.GetComponent<Renderer>().enabled = true;
						CloneItem.transform.parent = null;
						CloneItem.transform.localPosition  = Player.transform.localPosition;
						CloneItem.gameObject.SetActive(true);
						
						ItemScript.CurrencyAmount -= ItemScript.CurrencyAmount/ItemScript.Amount;
						ItemScript.Amount -= Amount; //Reduce the amount of the item inside the bag. 
						
						InvManager.GetComponent<AudioSource>().PlayOneShot(InvManager.RemovedItem); //a sound
						
						if(ItemScript.Amount < 0 || ItemScript.Amount == 0) //If the amount goes below 0 or equal to 0.
						{
							//Remove the item from this slot.
							Slots[a].Item = null;
							Slots[a].IsTaken = false;

							Destroy(Item.gameObject); //Destroying the item.
							
							Removed = true;
							Items--;
						}
						
						Removed = true;
					}
					else if(InvManager.DropType == InventoryManager.DropTypes.Destroy)
					{
						ItemScript.CurrencyAmount -= Amount*(ItemScript.CurrencyAmount/(ItemScript.Amount));
						ItemScript.Amount -= Amount; //Reduce the amount of the item inside the bag.      
						
						InvManager.GetComponent<AudioSource>().PlayOneShot(InvManager.RemovedItem); //a sound.
						if(ItemScript.Amount < 0 || ItemScript.Amount == 0) //If the amount goes below 0 or equal to 0.
						{
							//Remove the item from this slot.
							Slots[a].Item = null;
							Slots[a].IsTaken = false;
							
							
							//Informing our player with:
							
							Destroy(Item.gameObject); //Destroying the item.
							
							Removed = true;
							Items--;
						}    
					}
				}
				if(Removed)
				{
					Panel.RefreshItems (); if(SaveAndLoad == true) SaveItems();
					return;
				}
			}    
		}
	}
	
	public void  MoveToInventory ( Transform Item ,  int Amount,  int SlotID  ){
		if(InvManager.Items > InvManager.MaxItems) return;
		for(int a = 0; a < MaxItems; a++) //Starting a loop in the slots of the inventory:   
		{
			if(Slots[a].IsTaken == true) //Checking if there's an item in this slot.
			{
				if(Slots[a].Item == Item) //Check if the item exists in the bag.
				{    
					Item ItemScript = Slots[a].Item.GetComponent<Item>(); //Getting the item script from the items inside the bag.
					if(ItemScript.Amount - Amount >= 1) //If the amount goes below 1.
					{
						ItemScript.CurrencyAmount -= Amount*(ItemScript.CurrencyAmount/(ItemScript.Amount)); //Reduce the price of the item since we reduced its amount.
						ItemScript.Amount -= Amount; //Reduce the amount of the item inside the bag.
						
						Transform CloneObj; //Create a temporary object to move the removed item to.
						CloneObj = (Transform)Instantiate(Item, Item.position, Item.rotation); //Clone the original item.
						Item CloneScript = CloneObj.GetComponent<Item>(); //Get the item script.
						//Set the cloned item settings:
						CloneScript.Amount = Amount;
						CloneScript.CurrencyAmount = Amount*(ItemScript.CurrencyAmount/ItemScript.Amount);
						
						//Reactivate the clone and spawn it in real world.
						CloneObj.gameObject.SetActive(true);
						CloneObj.transform.GetComponent<Renderer>().enabled = true;
						CloneObj.transform.parent = null;
						CloneObj.transform.localPosition  = Player.transform.localPosition;
						
						
						InvManager.GetComponent<AudioSource>().PlayOneShot(InvManager.RemovedItem); //a sound
						
						InvManager.AddItemToSlot(CloneObj.transform, SlotID);

						Panel.RefreshItems (); if(SaveAndLoad == true) SaveItems();
					}
					else
					{ 
						//Reactivate the item and spawn it in real world.
						Item.gameObject.SetActive(true);
						Item.transform.GetComponent<Renderer>().enabled = true;
						Item.transform.parent = null;
						Item.transform.localPosition  = Player.transform.localPosition;
						
						
						
						//Remove the item from this slot.
						Slots[a].Item = null;
						Slots[a].IsTaken = false;
						
						InvManager.GetComponent<AudioSource>().PlayOneShot(InvManager.RemovedItem); //a sound
						
						Items--;
						
						InvManager.AddItemToSlot(Item, SlotID);

						Panel.RefreshItems (); if(SaveAndLoad == true) SaveItems();
					}
				}
			}    
		}
	}
	
	
}