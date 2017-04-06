/*

Inventory Manager Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {
	
	//Pickup items/money vars: Mouse or keyboard : 0 for mouse, 1 for keyboard.
	public enum PickupTypes {Mouse = 0, Keyboard = 1}
	public PickupTypes PickupType = PickupTypes.Mouse & PickupTypes.Keyboard;

	public bool OnCollisionPickup = false;
	
	//Drop items type: 0 for spawn, 1 for destroy.
	public enum DropTypes {Spawn = 0, Destroy = 1}
	public DropTypes DropType = DropTypes.Spawn & DropTypes.Destroy;
	public float DropDistance = 0.5f;
	
	public KeyCode ActionKey= KeyCode.E; //The key used to pick items.
	public Transform Player;
	
	//Items:
	[System.Serializable]
	public class Slot
	{
		public bool IsTaken = false;
		public Transform Item;
		public bool Dragged = false;
		
		public GameObject UI;
	}
	[HideInInspector]
	public Slot[] Slots;//Inventory slots
	public int MaxItems = 20; //Maximum number of items that can be held inside the bag.
	[HideInInspector]
	public int Items = 0; //Current number of items inside the bag.

	//Weight:
	public float MaxWeight = 50; //Maximum weight 
	[HideInInspector]
	public float Weight = 0; //Current weight of total items in the inventory.

    public bool ReArrangeItems = false;
	
	//Currencies:
	[System.Serializable]
	public class Currency
	{
		public string Name;
		public int Amount; 
		public Sprite Icon;
		public GameObject UI;
	}
	public Currency[] Riches;  //All the info about currencies exist in this variable
	
	
	//Sounds:
	public AudioClip NewItem;
	public AudioClip RemovedItem;
	public AudioClip NewCurrency;

	public AudioSource AudioSC;
	
	//Other vars:
	[HideInInspector]
	public Transform MyTransform;
	public GameObject DefaultItem;
	
	public bool SaveAndLoad = false;
	
	
	//Other scripts:
	[HideInInspector]
	public NotificationUI MsgUI;
	[HideInInspector]
	public InventoryUI InvUI;
	[HideInInspector]
	public InventoryEvents CustomEvents;
	
	//Sounds:
	public AudioClip InventoryFull;
	
	//Containers:
	[HideInInspector]
	public Container[] Containers;

	//public Transform StartingItem;
	/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
	
	
	public void  Awake ()
	{
		InvUI = FindObjectOfType(typeof(InventoryUI)) as InventoryUI;
		CustomEvents = FindObjectOfType (typeof(InventoryEvents)) as InventoryEvents;
		MyTransform = gameObject.transform; //Set the inventory transform.
		Items = 0;
		
		//Setting the inventory size:
		Slots = new Slot[MaxItems];
		
		for(int i = 0; i < MaxItems; i++)
		{
			Slots[i] = new Slot();
		}
 
		
		InvUI.CreateSlots (); //Create all slots.

		if(SaveAndLoad == true)
		{
			LoadCurrencies();
			LoadItems();
		}  

		InvUI.RefreshItems ();
		
		MsgUI = FindObjectOfType(typeof(NotificationUI)) as NotificationUI;

		/*if(PlayerPrefs.GetInt("HasStartingItedm",0) == 0)
		{
			PlayerPrefs.SetInt("HasStartingItem",1);
			Transform StartingItemInstance = Instantiate(StartingItem, new Vector3 (0, 0, 10), Quaternion.identity) as Transform;
			AddItem(StartingItemInstance);
			InvUI.MoveToSkill(StartingItemInstance, -1);
		}*/
	}
	
	/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
	
	public void  LoadItems (){
		int TempInt = PlayerPrefs.GetInt("SavedItems"); //Getting the number of the saved items.
		GameObject TempObj;
		for(int i = 1; i < TempInt; i++) //Starting a loop 
		{
			if(Resources.Load (PlayerPrefs.GetString("Name" + i.ToString())+"C", typeof(GameObject)))
			{
				TempObj = Instantiate(Resources.Load (PlayerPrefs.GetString("Name" + i.ToString())+"C", typeof(GameObject)),MyTransform.position,MyTransform.rotation) as GameObject;
			}
			else
			{
				TempObj = Instantiate(DefaultItem,MyTransform.position,MyTransform.rotation) as GameObject;
			}
			Item ItemScript= TempObj.GetComponent<Item>(); //Getting the item script.
			ItemScript.ItemType = PlayerPrefs.GetInt("ItemType" + i.ToString()); //Loading the item's type
			ItemScript.MinDistance = PlayerPrefs.GetFloat("MinDistance" + i.ToString()); //Loading the item's pickup minimum distance.
			ItemScript.Name = PlayerPrefs.GetString("Name" + i.ToString()); //Loading the item's name.
			ItemScript.Amount = PlayerPrefs.GetInt("Amount" + i.ToString(), 1); //Loading the item's amount.
			ItemScript.Weight = PlayerPrefs.GetInt("Weight" + i.ToString(), 1); //Loading the item's weight
			ItemScript.ShortDescription = PlayerPrefs.GetString("ShortDescription" + i.ToString()); //Loading the item's short description.
			ItemScript.Icon = Resources.Load(ItemScript.Name, typeof(Sprite)) as Sprite;
			ItemScript.Currency = PlayerPrefs.GetString("Currency" + i.ToString()); //Loading the item's currency type.
			ItemScript.CurrencyAmount = PlayerPrefs.GetInt("CurrencyAmount" + i.ToString()); //Loading the item's currency amount.
			ItemScript.EquipmentSlot = PlayerPrefs.GetString("EquipmentSlot" + i.ToString()); //Loading the item's equipment slot.
			ItemScript.SkillSlot = PlayerPrefs.GetString("SkillSlot" + i.ToString()); //Loading the item's skill bar slot.
			//Is the item stackable? 
			if(PlayerPrefs.GetInt("IsStackable" + i.ToString()) == 0)
			{
				ItemScript.IsStackable = false;
			}
			else
			{
				ItemScript.IsStackable = true;
			}   

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

			ItemScript.MaxAmount = PlayerPrefs.GetInt("MaxAmount" + i.ToString()); //Loading the item's maximum amount.
			LoadItem(TempObj.transform, PlayerPrefs.GetInt("ItemSlot" + i.ToString()));
		}
	}

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
	
	
	public void LoadItem( Transform Item ,   int ItemSlot  ) //Adding an item inside the bag.
	{
		Item NewItemScript= Item.GetComponent<Item>(); //Getting the new item script.
		bool  Match = false;
		
		for(int t = 0; t < MaxItems; t++) //Starting a loop in the slots of the inventory:
		{
			if(Slots[t].IsTaken == true && NewItemScript.IsStackable == true) //Checking if there's an item in this slot and that the item is stackable.
			{
				Item ItemScript= Slots[t].Item.GetComponent<Item>(); //Getting the item script from the items inside the bag.
				if(NewItemScript.Name == ItemScript.Name) //Checking if the type of the new item matches with another item already in the bag.
				{
					if(ItemScript.Amount + NewItemScript.Amount <= ItemScript.MaxAmount)
					{
						ItemScript.Amount += NewItemScript.Amount; //Increase the amount of the item in the bag.
						Destroy(Item.gameObject); //Destroying the item.
						
						Match = true;

						Weight += NewItemScript.Weight;
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

					Weight += NewItemScript.Weight;
					
					//Changing the settings of the item and parenting it to the inventory object.
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
			else
			{
				if(MsgUI != null) MsgUI.SendMsg("Inventory is full!");
				
				if(AudioSC) AudioSC.PlayOneShot(InventoryFull);
			}
		}     
		
	}
	
	
	
	public void  SaveItems (){
		int TempVar = 1;
		for(int i = 0; i < MaxItems; i++) //Starting a loop in the slots of the inventory:
		{
			if(Slots[i].Item == null && Slots[i].IsTaken == true)
			{
				Slots[i].IsTaken = false;
			}
			if(Slots[i].IsTaken == true)
			{
				Item ItemScript= Slots[i].Item.GetComponent<Item>(); //Getting the item script.
				
				PlayerPrefs.SetInt("ItemSlot" + TempVar.ToString(), i); //Saving the item's slot number.
				PlayerPrefs.SetInt("ItemType" + TempVar.ToString(), ItemScript.ItemType); //Saving the item's type
				PlayerPrefs.SetFloat("MinDistance" + TempVar.ToString(), ItemScript.MinDistance); //Saving the item's pickup minimum distance.
				PlayerPrefs.SetString("Name" + TempVar.ToString(), ItemScript.Name); //Saving the item's name.
				PlayerPrefs.SetInt("Amount" + TempVar.ToString(), ItemScript.Amount); //Saving the item's amount.
				PlayerPrefs.SetInt("Weight" + TempVar.ToString(), ItemScript.Amount); //Saving the item's weight
				PlayerPrefs.SetString("ShortDescription" + TempVar.ToString(), ItemScript.ShortDescription); //Saving the item's short description.
				PlayerPrefs.SetString("Currency" + TempVar.ToString(), ItemScript.Currency); //Saving the item's currency type.
				PlayerPrefs.SetInt("CurrencyAmount" + TempVar.ToString(), ItemScript.CurrencyAmount); //Saving the item's currency amount.
				PlayerPrefs.SetString("EquipmentSlot" + TempVar.ToString(), ItemScript.EquipmentSlot); //Saving the item's equipment slot.
				PlayerPrefs.SetString("SkillSlot" + TempVar.ToString(), ItemScript.SkillSlot); //Saving the item's skill bar slot.
				//Is the item stackable? 
				if(ItemScript.IsStackable == true)
				{
					PlayerPrefs.SetInt("IsStackable" + TempVar.ToString(), 1);
				}
				else
				{
					PlayerPrefs.SetInt("IsStackable" + TempVar.ToString(), 0); 
				}   
				PlayerPrefs.SetInt("MaxAmount" + TempVar.ToString(), ItemScript.MaxAmount); //Saving the item's maximum amount.

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
		PlayerPrefs.SetInt("SavedItems", TempVar);
	}
	
	
	public void  ClearSavedItems (){
		PlayerPrefs.SetInt("SavedItems", 0);
	}
	
	
	public void  ClearItems (){
		Debug.Log("cleaeing items"); //--DRB
		for(int i = 0; i < MaxItems; i++) //Starting a loop in the slots of the inventory:
		{
			if(Slots[i].IsTaken == true)
			{
				Item ItemScript= Slots[i].Item.GetComponent<Item>(); //Getting the item script.
				
				RemoveItem(Slots[i].Item, ItemScript.Amount);
			}               
		}
	}
	
	
	public void  ReloadItems (){
		ClearItems();
		LoadItems();
	}
	
	public bool Exist(Transform Item) //Adding an item inside the bag.
	{
		Item NewItemScript= Item.GetComponent<Item>(); //Getting the new item script.
		bool  Match = false;
		
		for(int t = 0; t < MaxItems; t++) //Starting a loop in the slots of the inventory:
		{
			if(Slots[t].IsTaken == true) //Checking if there's an item in this slot.
			{
				Item ItemScript= Slots[t].Item.GetComponent<Item>(); //Getting the item script from the items inside the bag.
				if(NewItemScript.Name == ItemScript.Name) //Checking if the type of the new item matches with another item already in the bag.
				{
					Match = true;
				}
				if(Match)
				{
					return true;
				}
			}
		}
		if(!Match) //Checking if there's no match with another item inside the bag.
		{
			return false;
		}         
		else
		{
			return Match;
		}
		
	}
	
	
	public void AddItem( Transform Item  ) //Adding an item inside the bag.
	{
		Item NewItemScript= Item.GetComponent<Item>(); //Getting the new item script.
		bool  Match = false;

		if (NewItemScript.Weight + Weight > MaxWeight) 
		{
			if(MsgUI != null) MsgUI.SendMsg("Maximum weight can't be exceeded!");
			
			if(AudioSC) AudioSC.PlayOneShot(InventoryFull);
			return;
		}

		
		for(int t = 0; t < MaxItems; t++) //Starting a loop in the slots of the inventory:
		{
			if(Slots[t].IsTaken == true && NewItemScript.IsStackable == true) //Checking if there's an item in this slot and that the item is stackable.
			{
				Item ItemScript= Slots[t].Item.GetComponent<Item>(); //Getting the item script from the items inside the bag.
				if(NewItemScript.Name == ItemScript.Name) //Checking if the type of the new item matches with another item already in the bag.
				{
					if(ItemScript.Amount + NewItemScript.Amount <= ItemScript.MaxAmount)
					{
						ItemScript.Amount += NewItemScript.Amount; //Increase the amount of the item in the bag.
						ItemScript.CurrencyAmount += NewItemScript.CurrencyAmount;
						
						if(AudioSC) AudioSC.PlayOneShot(NewItem); //a sound.
						
						Destroy(Item.gameObject); //Destroying the item.

						CustomEvents.OnPlayerAddItem(ItemScript);

						Match = true;

						Weight += NewItemScript.Weight;
					}
				}
				if(Match)
				{
					InvUI.RefreshItems(); if(SaveAndLoad == true) SaveItems();  
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
						Weight += NewItemScript.Weight;
						
						//Changing the settings of the item and parenting it to the inventory object.
						if(Item.transform.GetComponent<Collider>() !=null)
						{
							Item.transform.GetComponent<Collider>().isTrigger = true;
						}
						Item.transform.GetComponent<Renderer>().enabled = false;
						Item.transform.parent = MyTransform;
						Item.transform.localPosition  = Vector3.zero;
						Item.gameObject.SetActive(false);
						
						if(AudioSC) AudioSC.PlayOneShot(NewItem); //a sound.
						InvUI.RefreshItems(); if(SaveAndLoad == true) SaveItems();  

						CustomEvents.OnPlayerAddItem(Item.GetComponent<Item>());

						return;
					}
				}
			}  
			else
			{
				if(MsgUI != null) MsgUI.SendMsg("Inventory is full!");
				
				if(AudioSC) AudioSC.PlayOneShot(InventoryFull);
			}
		}    
	}
	
	public void AddItemToSlot( Transform Item, int SlotID ) //Adding an item inside the bag.
	{
		Item NewItemScript= Item.GetComponent<Item>(); //Getting the new item script.
		bool  Match = false;

		if (NewItemScript.Weight + Weight > MaxWeight) 
		{
			if(MsgUI != null) MsgUI.SendMsg("Maximum weight can't be exceeded!");
			
			if(AudioSC) AudioSC.PlayOneShot(InventoryFull);
			return;
		}
		
		for(int t = 0; t < MaxItems; t++) //Starting a loop in the slots of the inventory:
		{
			if(Slots[t].IsTaken == true && NewItemScript.IsStackable == true) //Checking if there's an item in this slot and that the item is stackable.
			{
				Item ItemScript= Slots[t].Item.GetComponent<Item>(); //Getting the item script from the items inside the bag.
				if(NewItemScript.Name == ItemScript.Name) //Checking if the type of the new item matches with another item already in the bag.
				{
					if(ItemScript.Amount + NewItemScript.Amount <= ItemScript.MaxAmount)
					{
						ItemScript.Amount += NewItemScript.Amount; //Increase the amount of the item in the bag.
						ItemScript.CurrencyAmount += NewItemScript.CurrencyAmount;
						
						if(AudioSC) AudioSC.PlayOneShot(NewItem); //a sound.
						
						Destroy(Item.gameObject); //Destroying the item.

						CustomEvents.OnPlayerAddItem(ItemScript);
						
						Match = true;

						Weight += NewItemScript.Weight;
					}
				}
				if(Match)
				{
					InvUI.RefreshItems(); if(SaveAndLoad == true) SaveItems();  
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

						Weight += NewItemScript.Weight;
						
						//Changing the settings of the item and parenting it to the inventory object.
						if(Item.transform.GetComponent<Collider>() !=null)
						{
							Item.transform.GetComponent<Collider>().isTrigger = true;
						}
						Item.transform.GetComponent<Renderer>().enabled = false;
						Item.transform.parent = MyTransform;
						Item.transform.localPosition  = Vector3.zero;
						Item.gameObject.SetActive(false);
						
						if(AudioSC) AudioSC.PlayOneShot(NewItem); //a sound.
						
						MoveItem (b, SlotID);

						CustomEvents.OnPlayerAddItem(Item.GetComponent<Item>());

						return;
					}
				}
			}  
			else
			{
				if(MsgUI != null) MsgUI.SendMsg("Inventory is full!");
				
				if(AudioSC) AudioSC.PlayOneShot(InventoryFull);
			}
		}         
	}
	
	public void  RemoveItem ( Transform Item ,   int Amount  ){
		bool  Removed = false;
		for(int a = 0; a < MaxItems; a++) //Starting a loop in the slots of the inventory:   
		{
			if(Slots[a].IsTaken == true) //Checking if there's an item in this slot.
			{
				Item ItemScript= Slots[a].Item.GetComponent<Item>(); //Getting the item script from the items inside the bag.
				if(Slots[a].Item == Item) //Check if the item exists in the bag.
				{
					if((DropType == InventoryManager.DropTypes.Spawn && ItemScript.ItemDropType == global::Item.ItemDropTypes.Global) || (ItemScript.ItemDropType == global::Item.ItemDropTypes.Spawn))  
					{
						
						Transform CloneItem; //Create a temporary object to move the removed item to.
						CloneItem = Instantiate(Item, Item.position, Item.rotation) as Transform; //Clone the original item.
						Item Script= CloneItem.GetComponent<Item>(); //Get the item script.
						
						//Set the cloned item settings:
						Script.Amount = Amount;
						ItemScript.CurrencyAmount -= Amount*(ItemScript.CurrencyAmount/(ItemScript.Amount));
						
						//Activate the cloned item and spawn it in real world.
						CloneItem.transform.GetComponent<Renderer>().enabled = true;
						CloneItem.transform.parent = null;
						CloneItem.transform.localPosition  = Player.transform.position + Player.transform.forward*DropDistance;
						CloneItem.gameObject.SetActive(true);
						
						ItemScript.CurrencyAmount -= ItemScript.CurrencyAmount/ItemScript.Amount;
						ItemScript.Amount -= Amount; //Reduce the amount of the item inside the bag. 
						
						if(AudioSC) AudioSC.PlayOneShot(RemovedItem); //a sound
						
						if(ItemScript.Amount < 0 || ItemScript.Amount == 0) //If the amount goes below 0 or equal to 0.
						{
							//Remove the item from this slot.
							Slots[a].Item = null;
							Slots[a].IsTaken = false;
							
							Destroy(Item.gameObject); //Destroying the item.
							Items--;
						}
						
						Removed = true;
					}
					else if((DropType == InventoryManager.DropTypes.Destroy && ItemScript.ItemDropType == global::Item.ItemDropTypes.Global) || (ItemScript.ItemDropType == global::Item.ItemDropTypes.Destroy))  
					{
						ItemScript.CurrencyAmount -= Amount*(ItemScript.CurrencyAmount/(ItemScript.Amount));
						ItemScript.Amount -= Amount; //Reduce the amount of the item inside the bag.      
						
						if(AudioSC) AudioSC.PlayOneShot(RemovedItem); //a sound.
						if(ItemScript.Amount < 0 || ItemScript.Amount == 0) //If the amount goes below 0 or equal to 0.
						{
							//Remove the item from this slot.
							Slots[a].Item = null;
							Slots[a].IsTaken = false;
							
							
							//Informing our player with:
							
							Destroy(Item.gameObject); //Destroying the item.
							Items--;
						}   
						Removed = true;
					}
				} 
				if(Removed)
				{
					InvUI.RefreshItems(); if(SaveAndLoad == true) SaveItems(); 
					return;
				}
			}    
		}
	}
	
	public Transform FreeItem( Transform Item ,   int Amount  ) //This is the only public void that returns a transform, which is the item that is now free.
	{
		for(int a = 0; a < MaxItems; a++) //Starting a loop in the slots of the inventory:   
		{
			if(Slots[a].IsTaken == true) //Checking if there's an item in this slot.
			{
				if(Slots[a].Item == Item) //Check if the item exists in the bag.
				{    
					Item ItemScript= Slots[a].Item.GetComponent<Item>(); //Getting the item script from the items inside the bag.
					if(ItemScript.Amount - Amount >= 1) //If the amount goes below 1.
					{
						ItemScript.CurrencyAmount -= Amount*(ItemScript.CurrencyAmount/(ItemScript.Amount)); //Reduce the price of the item since we reduced its amount.
						ItemScript.Amount -= Amount; //Reduce the amount of the item inside the bag.
						
						Transform CloneObj; //Create a temporary object to move the removed item to.
						CloneObj = Instantiate(Item, Item.position, Item.rotation) as Transform; //Clone the original item.
						Item CloneScript= CloneObj.GetComponent<Item>(); //Get the item script.
						//Set the cloned item settings:
						CloneScript.Amount = Amount;
						CloneScript.CurrencyAmount = Amount*(ItemScript.CurrencyAmount/ItemScript.Amount);
						
						//Reactivate the clone and spawn it in real world.
						CloneObj.gameObject.SetActive(true);
						CloneObj.transform.GetComponent<Renderer>().enabled = true;
						CloneObj.transform.parent = null;
						CloneObj.transform.localPosition  = Player.transform.localPosition;
						
						
						if(AudioSC) AudioSC.PlayOneShot(RemovedItem); //a sound
						InvUI.RefreshItems(); if(SaveAndLoad == true) SaveItems(); 
						return CloneObj;
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
						
						if(AudioSC) AudioSC.PlayOneShot(RemovedItem); //a sound
						
						Items--;
						InvUI.RefreshItems(); if(SaveAndLoad == true) SaveItems(); 
						return Item;   
						
						
					}
				}
			}    
		}
		InvUI.RefreshItems(); if(SaveAndLoad == true) SaveItems(); 
		return null;
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
			
			InvUI.RefreshItems(); if(SaveAndLoad == true) SaveItems(); 
			
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
	/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
	
	public void AddCurrency( Transform Currency  ) //Adding currency to the inventory.
	{
		Item NewCurrencyScript= Currency.GetComponent<Item>(); //Getting the new currency script.
		bool  Match = false;
		
		for(int i = 0; i < Riches.Length; i++) //Starting a loop in the content of the inventory    
		{
			if(NewCurrencyScript.Name == Riches[i].Name) //Checking if the type of the new currency matches with another currency already in the inventory.
			{
				Riches[i].Amount += NewCurrencyScript.Amount; //Increase the amount of the currency in the inventory. 
				
				if(AudioSC) AudioSC.PlayOneShot(NewCurrency); //a sound.
				
				Destroy(Currency.gameObject); //Destroying the currency object.
				
				Match = true;
			}
			if(Match)
			{
				InvUI.RefreshRiches(); if(SaveAndLoad == true) SaveCurrencies();
				return;
			}
		}    
		if(!Match) //Checking if there's no match with another currency inside the inventory.
		{
			Debug.LogError("The currency you want to add to the inventory isn't defined, please define it in the InventoryManager.js.");
		}
	}
	
	public void  RemoveCurrency ( string Name ,   int Amount  ){
		bool  Removed = false;
		
		for(int i = 0; i < Riches.Length; i++) //Starting a loop in the content of the inventory        
		{
			if(Riches[i].Name == Name) //Check if the currency exists in the inventory.
			{
				Riches[i].Amount -= Amount; //Reduce the amount of the currency inside the inventory.
				
				Removed = true;
				
				if(Riches[i].Amount < 0 || Riches[i].Amount == 0) //If the amount goes below 0 or equal to 0.
				{
					Riches[i].Amount = 0;
					Debug.Log("You can't have a negative amount on a currency, amount is automatically set to 0.");
				}    
				if(Removed)
				{
					InvUI.RefreshRiches(); if(SaveAndLoad == true) SaveCurrencies();
					return;
				}
			}
		}
	}
 
	public bool  HaveCurrency (string Name, int Amount) //Check if we have the amount of a currency in the riches.
	{
		bool Result = false;
		for(int i = 0; i < Riches.Length; i++) //Starting a loop in the content of the inventory        
		{
			if(Riches[i].Name == Name) //Check if the currency exists in the inventory.
			{
				if(Riches[i].Amount >= Amount) //If we the required amount
				{
					Result = true;
					return Result;
				}    
			}
		}
		if(Result == false)
		{
			if(MsgUI != null) MsgUI.SendMsg("You don't have enough "+Name);
		}
		return Result;
	}

	public Sprite GetCurrencyIcon (string Name) //Check if we have the amount of a currency in the riches.
	{
		Sprite Result = null;
		for(int i = 0; i < Riches.Length; i++) //Starting a loop in the content of the inventory        
		{
			if(Riches[i].Name == Name) //Check if the currency exists in the inventory.
			{
				Result = Riches[i].Icon;
				return Result;
			}
		}
		return Result;
	}
	
	public void  LoadCurrencies (){
		for(int i = 0; i < Riches.Length; i++) //Starting a loop in the content of the inventory
		{
			Riches[i].Amount = PlayerPrefs.GetInt("CurrencyAmount" + i.ToString()); //Loading the currency amount
			InvUI.RefreshRiches();
		}
	}
	
	public void  SaveCurrencies (){
		for(int i = 0; i < Riches.Length; i++) //Starting a loop in the content of the inventory
		{
			PlayerPrefs.SetInt("CurrencyAmount" + i.ToString(), Riches[i].Amount); //Saving the currency amount
		}
	}
	
	public void  ClearSavedCurrenices (){
		for(int i = 0; i < Riches.Length; i++) //Starting a loop in the content of the inventory
		{
			PlayerPrefs.SetInt("CurrencyAmount" + i.ToString(), 0); //Saving the currency amount  
		}
	}
	
	public void  ClearCurrencies (){
		for(int i = 0; i < Riches.Length; i++) //Starting a loop in the content of the inventory
		{
			Riches[i].Amount = 0;
		}
	}
	
	public void  ReloadCurrencies (){
		ClearCurrencies();
		LoadCurrencies();
	}
}
