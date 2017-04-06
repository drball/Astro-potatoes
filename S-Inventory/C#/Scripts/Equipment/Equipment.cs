/*

Equipment Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Equipment : MonoBehaviour {

	[System.Serializable]
	public class AttributesVars
	{
		public string Name;
		public int Value;
	}
	public AttributesVars[] Attributes;
	//Equipment slots info:
	[System.Serializable]
	public class SlotVars
	{
		public string Name;
		public bool  IsTaken = false;
		public Image Icon;
	    [HideInInspector]
		public bool Dragged = false;
		[HideInInspector]
		public Transform Item;
		public int GroupID;
	}
	public SlotVars[] EquipmentSlots;
	public KeyCode Key= KeyCode.C; //The key used to show/hide inventory GUI.

	[System.Serializable]
	public class EqObjVars
	{
		public string Name;
		public GameObject Obj;
	}
	public EqObjVars[] EquipmentObjects;
	
	[HideInInspector]
	public Transform MyTransform;
	
	//Other scripts:
	[HideInInspector]
	InventoryUI InvUI;
	[HideInInspector]
	InventoryEvents CustomEvents;
	[HideInInspector]
	EquipmentUI EqUI;
	
	public Transform DefaultItem;
	public bool  SaveAndLoad = false;
	
	void  Awake (){
		InvUI = FindObjectOfType(typeof(InventoryUI)) as InventoryUI; //Get the Inventory Manager object/script.
		CustomEvents = FindObjectOfType(typeof(InventoryEvents)) as InventoryEvents;
		EqUI = FindObjectOfType (typeof(EquipmentUI)) as EquipmentUI;
		MyTransform = gameObject.transform; //Set the object tranform.
		
		if(SaveAndLoad == true)
		{
			LoadEquipments();
		}

		for(int i = 0; i < EquipmentSlots.Length; i++) //Starting a loop in the slots of the bag.
		{
			if(EquipmentSlots[i].IsTaken == true)
			{
				Item ItemScript = EquipmentSlots[i].Item.GetComponent<Item>(); //Getting the item script.  

				EquipmentSlots[i].Icon.color = new Color(1.0f,1.0f,1.0f,1.0f);
				EquipmentSlots[i].Icon.sprite = ItemScript.Icon;
			}
			else
			{
				EquipmentSlots[i].Icon.sprite = null;
				EquipmentSlots[i].Icon.color = new Color(1.0f,1.0f,1.0f,0.0f);
			}
			EquipmentSlots[i].Icon.gameObject.GetComponent<SlotUI>().SlotID = i;
			EquipmentSlots[i].Icon.gameObject.SetActive(true);
		}
	}
	
	void  Update (){

		if(Input.GetKeyDown(Key)) //Check if the player pressed the key that shows/hides the equipment window.
		{
			FindObjectOfType<EquipmentUI>().ToggleEquipment ();
		}

		//Always check for new items to add:
		if(InvUI.IsEquippingItem == true) //Check if the player is equipping an item.
		{
			for(int i = 0; i < EquipmentSlots.Length; i++) //Starting a loop in the slots of the bag.
			{
				if(i == InvUI.EquippedItemSlot) //If the current slot is the target slot for equipping this item.
				{
					//Equip the item and set the slot to taken.
					EquipmentSlots[i].Item = InvUI.EquippedItem; 
					EquipmentSlots[i].IsTaken = true;
					
					Item ItemScript = EquipmentSlots[i].Item.GetComponent<Item>(); //Getting the item script.  
					
					EquipmentSlots[i].Icon.color = new Color(1.0f,1.0f,1.0f,1.0f);
					EquipmentSlots[i].Icon.sprite = ItemScript.Icon;
					
					InvUI.IsEquippingItem = false;

					ActivateEqObj (ItemScript.Name);

					for(int x = 0; x < ItemScript.Attributes.Length; x++)
					{
						for(int j = 0; j < Attributes.Length; j++)
						{
							if(Attributes[j].Name == ItemScript.Attributes[x].Name)
							{
								Attributes[j].Value += ItemScript.Attributes[x].Value;
							}
						}
					}

					SaveEquipments();
				}
			}
		}        
		
	}    
	
	//Saving and loading equipment items:
	
	public void  SaveEquipments (){
		int TempVar = 1;
		for(int i = 0; i < EquipmentSlots.Length; i++) //Starting a loop in the equipment slots.
		{
			if(EquipmentSlots[i].IsTaken == true)
			{
				Item ItemScript = EquipmentSlots[i].Item.GetComponent<Item>(); //Getting the item script.
				
				//I use Eq at the beginning so there player prefs don't get mixed with the inventory items player prefs.
				PlayerPrefs.SetInt("EqItemType" + TempVar.ToString(), ItemScript.ItemType); //Saving the item's type
				PlayerPrefs.SetFloat("EqMinDistance" + TempVar.ToString(), ItemScript.MinDistance); //Saving the item's pickup minimum distance.
				PlayerPrefs.SetString("EqName" + TempVar.ToString(), ItemScript.Name); //Saving the item's name.
				PlayerPrefs.SetInt("EqAmount" + TempVar.ToString(), ItemScript.Amount); //Saving the item's amount.
				PlayerPrefs.SetInt("EqWeight" + TempVar.ToString(), ItemScript.Amount); //Saving the item's weight
				PlayerPrefs.SetString("EqShortDescription" + TempVar.ToString(), ItemScript.ShortDescription); //Saving the item's short description.
				PlayerPrefs.SetString("EqCurrency" + TempVar.ToString(), ItemScript.Currency); //Saving the item's currency type.
				PlayerPrefs.SetInt("EqCurrencyAmount" + TempVar.ToString(), ItemScript.CurrencyAmount); //Saving the item's currency amount.
				PlayerPrefs.SetString("EqEquipmentSlot" + TempVar.ToString(), ItemScript.EquipmentSlot); //Saving the item's equipment slot.
				PlayerPrefs.SetString("EqSkillSlot" + TempVar.ToString(), ItemScript.SkillSlot); //Saving the item's skill bar slot.
				PlayerPrefs.SetInt("EqSlotID" + TempVar.ToString(), i); //Saving the item's skill bar slot.

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
		PlayerPrefs.SetInt("SavedEquipments", TempVar);

		EqUI.RefreshAttributes ();
	}
	static int GetAttributeValue (string Name)
	{
		var InvEqu = FindObjectOfType(typeof(Equipment)) as Equipment;
		for(int i = 0; i < InvEqu.Attributes.Length; i++)
		{
			if(InvEqu.Attributes[i].Name == Name)
			{
				return InvEqu.Attributes[i].Value;
			}
		}
		return -1;
	}
	
	void  LoadEquipments (){
		int TempInt= PlayerPrefs.GetInt("SavedEquipments"); //Getting the number of the saved equipment slots
		GameObject TempObj;
		for(int i = 1; i < TempInt; i++) //Starting a loop in the equipment slots.
		{
			if(Resources.Load (PlayerPrefs.GetString("EqName" + i.ToString())+"C", typeof(GameObject)))
			{
				TempObj = Instantiate(Resources.Load (PlayerPrefs.GetString("EqName" + i.ToString())+"C", typeof(GameObject)),MyTransform.position,MyTransform.rotation) as GameObject;
			}
			else
			{
				TempObj = Instantiate(DefaultItem, MyTransform.position, MyTransform.rotation).gameObject;
			}
			Item ItemScript = TempObj.GetComponent<Item>(); //Getting the item script.
			ItemScript.ItemType = PlayerPrefs.GetInt("EqItemType" + i.ToString()); //Loading the item's type
			ItemScript.MinDistance = PlayerPrefs.GetFloat("EqMinDistance" + i.ToString()); //Loading the item's pickup minimum distance.
			ItemScript.Name = PlayerPrefs.GetString("EqName" + i.ToString()); //Loading the item's name.
			ItemScript.Amount = PlayerPrefs.GetInt("EqAmount" + i.ToString()); //Loading the item's amount.
			ItemScript.Weight = PlayerPrefs.GetInt("EqWeight" + i.ToString(), 1); //Loading the item's weight
			ItemScript.ShortDescription = PlayerPrefs.GetString("EqShortDescription" + i.ToString()); //Loading the item's short description.
			ItemScript.Icon = Resources.Load(ItemScript.Name, typeof(Sprite)) as Sprite;
			ItemScript.Currency = PlayerPrefs.GetString("EqCurrency" + i.ToString()); //Loading the item's currency type.
			ItemScript.CurrencyAmount = PlayerPrefs.GetInt("EqCurrencyAmount" + i.ToString()); //Loading the item's currency amount.
			ItemScript.EquipmentSlot = PlayerPrefs.GetString("EqEquipmentSlot" + i.ToString()); //Loading the item's equipment slot.
			ItemScript.SkillSlot = PlayerPrefs.GetString("EqSkillSlot" + i.ToString()); //Loading the item's skill bar slot.
			ItemScript.IsStackable = false;

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
			
			//Adding the loaded items to their equipment slots:
			for(int t = 0; t < EquipmentSlots.Length; t++) //Starting a loop in the equipment slots.
			{
				if(EquipmentSlots[t].Name == ItemScript.EquipmentSlot && PlayerPrefs.GetInt("EqSlotID" + i.ToString()) == t) //If the current loaded item matches one of the equipment slots.
				{
					//Equip the item and set the slot to taken.
					EquipmentSlots[t].Item = TempObj.transform; 
					EquipmentSlots[t].IsTaken = true;
					
					CustomEvents.OnPlayerEquipItem(ItemScript, EquipmentSlots[t].GroupID);
					
					//Changing the settings of the item and parenting it to the inventory manager again.
					if(EquipmentSlots[t].Item.GetComponent<Collider>() !=null)
					{
						EquipmentSlots[t].Item.transform.GetComponent<Collider>().isTrigger = true;
					}
					EquipmentSlots[t].Item.transform.GetComponent<Renderer>().enabled = false;
					EquipmentSlots[t].Item.transform.parent = gameObject.transform;
					EquipmentSlots[t].Item.transform.localPosition  = Vector3.zero;
					EquipmentSlots[t].Item.gameObject.SetActive(false);

					ActivateEqObj (ItemScript.Name);
				}
			}

			for(int x = 0; x < ItemScript.Attributes.Length; x++)
			{
				for(int j = 0; j < Attributes.Length; j++)
				{
					if(Attributes[j].Name == ItemScript.Attributes[x].Name)
					{
						Attributes[j].Value += ItemScript.Attributes[x].Value;
					}
				}
			}
		}
		
	}

	public void ActivateEqObj (string Name)
	{
		for(int i = 0; i < EquipmentObjects.Length; i++) 
		{
			if(Name == EquipmentObjects[i].Name)
			{
				EquipmentObjects[i].Obj.SetActive(true);
			}
		}
	}
	
	public void DesactivateEqObj (string Name)
	{
		for(int i = 0; i < EquipmentObjects.Length; i++) 
		{
			if(Name == EquipmentObjects[i].Name)
			{
				EquipmentObjects[i].Obj.SetActive(false);
			}
		}
	}
}