/*

Skill bar Script Created by Oussama Bouanani (SoumiDelRio).

*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour {
	
	public bool ShowSkillBar = true;
	
	//Skill bar slot
	[System.Serializable]
	public class SkillVars
	{
		public string Name;
		public bool  MultipleItems = false;
		public Image Icon;
		[HideInInspector]
		public bool  IsTaken = false;
		[HideInInspector]
		public Transform Item;
		[HideInInspector]
		public bool Dragged = false;
		public KeyCode TriggerKey = KeyCode.Alpha1;
		public bool DestroyOnUse = false;
		public float UseCoolDown = 2.0f;
		[HideInInspector]
		public float UseTimer = 0.0f;
	}
	public SkillVars[] SkillSlot;
	
	//Other scripts:
	[HideInInspector]
	public InventoryManager InvManager;
	[HideInInspector]
	public InventoryUI InvGUI;
	[HideInInspector]
	public InventoryEvents CustomEvents;
	
	[HideInInspector]
	public Transform MyTransform;
	
	
	public GameObject DefaultItem;
	public bool  SaveAndLoad = false;
	
	
	void  Start (){
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		InvGUI = FindObjectOfType(typeof(InventoryUI)) as InventoryUI; //Get the Inventory Manager object/script.
		CustomEvents = FindObjectOfType(typeof(InventoryEvents)) as InventoryEvents; //Get the Inventory Events script.
		
		MyTransform = gameObject.transform; //Set the object tranform.
		
		if(SaveAndLoad == true)
		{
			LoadSkillBar();
			
			for(int i = 0; i < SkillSlot.Length; i++) //Starting a loop in the skill bar slots.
			{
				if(SkillSlot[i].IsTaken == true)
				{
					Item ItemScript = SkillSlot[i].Item.GetComponent<Item>(); //Getting the item script.  
					
					SkillSlot[i].Icon.color = new Color(1.0f,1.0f,1.0f,1.0f);
					SkillSlot[i].Icon.sprite = ItemScript.Icon;
					
					if(SkillSlot[i].Icon.transform.Find("Text") != null && ItemScript.IsStackable == true) 
					{
						SkillSlot[i].Icon.transform.Find("Text").gameObject.SetActive(true);
						SkillSlot[i].Icon.transform.Find("Text").GetComponent<Text>().text = ItemScript.Amount.ToString();
					}
					else if(SkillSlot[i].Icon.transform.Find("Text") != null)
					{
						SkillSlot[i].Icon.transform.Find("Text").gameObject.SetActive(false);
					}
				}
				else
				{
					SkillSlot[i].Icon.sprite = null;
					SkillSlot[i].Icon.color = new Color(1.0f,1.0f,1.0f,0.0f);
				}
				SkillSlot[i].Icon.gameObject.GetComponent<SlotUI>().SlotID = i;
				SkillSlot[i].Icon.gameObject.SetActive(true);
			}
		}
	}
	
	
	void  Update (){
		
		//Always check for new items to add:
		if(InvGUI.MovingToSkillBar == true) //Check if the player is moving an item from the inventory to the skill bar.
		{
			for(int i  = 0; i < SkillSlot.Length; i++) //Starting a loop in the skill bar slots.
			{
				if(i == InvGUI.SkillSlotNumber) //If the current slot is the target slot.
				{
					//Equip the item and set the slot to taken.
					SkillSlot[i].Item = InvGUI.SkillItem; 
					SkillSlot[i].IsTaken = true;
					
					InvGUI.MovingToSkillBar = false;
					
					Item ItemScript = SkillSlot[i].Item.GetComponent<Item>(); //Getting the item script.  
					
					SkillSlot[i].Icon.color = new Color(1.0f,1.0f,1.0f,1.0f);
					SkillSlot[i].Icon.sprite = ItemScript.Icon;
					if(SkillSlot[i].Icon.transform.Find("Text") != null && ItemScript.Amount > 1) 
					{
						SkillSlot[i].Icon.transform.Find("Text").gameObject.SetActive(true);
						SkillSlot[i].Icon.transform.Find("Text").GetComponent<Text>().text = ItemScript.Amount.ToString();
					}
					else if(SkillSlot[i].Icon.transform.Find("Text") != null)
					{
						SkillSlot[i].Icon.transform.Find("Text").gameObject.SetActive(false);
					}   
					
					SaveSkillBar(); 
				}
			}
		} 

		//Trigger skill items keys:
		for(int k  = 0; k < SkillSlot.Length; k++) //Starting a loop in the skill bar slots.
		{
			if(SkillSlot[k].IsTaken == true && SkillSlot[k].UseTimer == 0)
			{
				if(Input.GetKeyDown(SkillSlot[k].TriggerKey))
				{
					SkillSlot[k].Icon.color = Color.gray;
					RemoveFromSkillSlot(k,1);
					SkillSlot[k].UseTimer = SkillSlot[k].UseCoolDown;
					CustomEvents.OnSkillBarItemUsed(SkillSlot[k].Item.gameObject.GetComponent<Item>());
					SaveSkillBar(); 
				}
			}    
			
			if(SkillSlot[k].UseTimer > 0)
			{
				SkillSlot[k].UseTimer -= Time.deltaTime;
			}
			else if(SkillSlot[k].UseTimer < 0)
			{
				SkillSlot[k].UseTimer = 0;
				if(SkillSlot[k].IsTaken == true) SkillSlot[k].Icon.color = Color.white;
			}
		}
	}
	
	
	
	public bool  CheckSkillSlot ( string Name  ){
		//Loop throught all the skill bar slots, search for a specfic item.
		for(int i = 0; i < SkillSlot.Length; i++)
		{
			if(SkillSlot[i].IsTaken == true)
			{
				if(SkillSlot[i].Name == Name)
				{
					return true;
				}
			}
		}
		return false;
	}
	
	public void RemoveFromSkillSlot (int i, int Amount) //Function used to remove items from the skill bar.
	{
		Item ItemScript = SkillSlot[i].Item.GetComponent<Item>();
		if(ItemScript.IsStackable == true && SkillSlot[i].DestroyOnUse == true)
		{
			ItemScript.CurrencyAmount -= Amount*(ItemScript.CurrencyAmount/(ItemScript.Amount)); //Reduce the price of the item since we reduced its amount.
			ItemScript.Amount -= Amount; //Reduce the amount of the item inside the bag.
		}    
		
		if((ItemScript.Amount <= 0 && ItemScript.IsStackable == true && SkillSlot[i].DestroyOnUse == true) ||(ItemScript.IsStackable == false && SkillSlot[i].DestroyOnUse == true)) //If the amount goes below 0 or equal to 0.
		{
			Destroy(SkillSlot[i].Item.gameObject); //Destroying the item.
			SkillSlot[i].IsTaken = false;
			
			SkillSlot[i].Icon.sprite = null;
			SkillSlot[i].UseTimer = 0;
			SkillSlot[i].Icon.color = new Color(1.0f,1.0f,1.0f,0.0f);
			
			if(SkillSlot[i].Icon.transform.Find("Text") != null)
			{
				SkillSlot[i].Icon.transform.Find("Text").gameObject.SetActive(false);
			} 
		}
		else
		{
			if(SkillSlot[i].Icon.transform.Find("Text") != null && ItemScript.IsStackable == true) 
			{
				SkillSlot[i].Icon.transform.Find("Text").gameObject.SetActive(true);
				SkillSlot[i].Icon.transform.Find("Text").GetComponent<Text>().text = ItemScript.Amount.ToString();
			}
			else if(SkillSlot[i].Icon.transform.Find("Text") != null)
			{
				SkillSlot[i].Icon.transform.Find("Text").gameObject.SetActive(false);
			} 
		}
	}
	
	
	//Saving and loading:
	public void  SaveSkillBar (){
		int TempVar = 1;
		for(int i = 0; i < SkillSlot.Length; i++) //Starting a loop in the skill slots.
		{
			if(SkillSlot[i].IsTaken == true)
			{
				Item ItemScript= SkillSlot[i].Item.GetComponent<Item>(); //Getting the item script.
				
				//I use Eq at the beginning so there player prefs don't get mixed with the inventory items player prefs.
				PlayerPrefs.SetInt("SkItemType" + TempVar.ToString(), ItemScript.ItemType); //Saving the item's type
				PlayerPrefs.SetFloat("SkMinDistance" + TempVar.ToString(), ItemScript.MinDistance); //Saving the item's pickup minimum distance.
				PlayerPrefs.SetString("SkName" + TempVar.ToString(), ItemScript.Name); //Saving the item's name.
				PlayerPrefs.SetInt("SkAmount" + TempVar.ToString(), ItemScript.Amount); //Saving the item's amount.
				PlayerPrefs.SetInt("SkWeight" + TempVar.ToString(), ItemScript.Amount); //Saving the item's weight
				PlayerPrefs.SetString("SkShortDescription" + TempVar.ToString(), ItemScript.ShortDescription); //Saving the item's short description.
				PlayerPrefs.SetString("SkCurrency" + TempVar.ToString(), ItemScript.Currency); //Saving the item's currency type.
				PlayerPrefs.SetInt("SkCurrencyAmount" + TempVar.ToString(), ItemScript.CurrencyAmount); //Saving the item's currency amount.
				PlayerPrefs.SetString("SkEquipmentSlot" + TempVar.ToString(), ItemScript.EquipmentSlot); //Saving the item's equipment slot.
				PlayerPrefs.SetString("SkSkillSlot" + TempVar.ToString(), ItemScript.SkillSlot); //Saving the item's skill bar slot.
				PlayerPrefs.SetInt("SkSkillSlotID" + TempVar.ToString(), i); //Saving the item's skill bar slot.

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
		PlayerPrefs.SetInt("SavedSkillSlots", TempVar);
	}
	
	void  LoadSkillBar (){
		int TempInt= PlayerPrefs.GetInt("SavedSkillSlots"); //Getting the number of the saved skill bar slots.
		GameObject TempObj;
		for(int i = 1; i < TempInt; i++) //Starting a loop in the saved skill slots.
		{
			if(Resources.Load (PlayerPrefs.GetString("SkName" + i.ToString())+"C", typeof(GameObject)))
			{
				TempObj = Instantiate(Resources.Load (PlayerPrefs.GetString("SkName" + i.ToString())+"C", typeof(GameObject)),MyTransform.position,MyTransform.rotation) as GameObject;
			}
			else
			{
				TempObj = Instantiate(DefaultItem,MyTransform.position,MyTransform.rotation) as GameObject;
			}
			Item ItemScript= TempObj.GetComponent<Item>(); //Getting the item script.
			ItemScript.ItemType = PlayerPrefs.GetInt("SkItemType" + i.ToString()); //Loading the item's type
			ItemScript.MinDistance = PlayerPrefs.GetFloat("SkMinDistance" + i.ToString()); //Loading the item's pickup minimum distance.
			ItemScript.Name = PlayerPrefs.GetString("SkName" + i.ToString()); //Loading the item's name.
			ItemScript.Amount = PlayerPrefs.GetInt("SkAmount" + i.ToString()); //Loading the item's amount.
			ItemScript.Weight = PlayerPrefs.GetInt("SkWeight" + i.ToString(), 1); //Loading the item's weight
			ItemScript.ShortDescription = PlayerPrefs.GetString("SkShortDescription" + i.ToString()); //Loading the item's short description.
			ItemScript.Icon = Resources.Load(ItemScript.Name, typeof(Sprite)) as Sprite;
			ItemScript.Currency = PlayerPrefs.GetString("SkCurrency" + i.ToString()); //Loading the item's currency type.
			ItemScript.CurrencyAmount = PlayerPrefs.GetInt("SkCurrencyAmount" + i.ToString()); //Loading the item's currency amount.
			ItemScript.EquipmentSlot = PlayerPrefs.GetString("SkEquipmentSlot" + i.ToString()); //Loading the item's equipment slot.
			ItemScript.SkillSlot = PlayerPrefs.GetString("SkSkillSlot" + i.ToString()); //Loading the item's skill bar slot.

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
			
			//Adding the loaded items to their skill bar slots:
			for(int t = 0; t < SkillSlot.Length; t++) //Starting a loop in the skill bar slots.
			{
				if(SkillSlot[t].Name == ItemScript.SkillSlot && PlayerPrefs.GetInt("SkSkillSlotID" + i.ToString()) == t) //If the current loaded item matches one of the skill bar slots.
				{
					//Set the item and set the slot to taken.
					SkillSlot[t].Item = TempObj.transform; 
					SkillSlot[t].IsTaken = true;
					
					CustomEvents.OnSkillBarAdd(ItemScript);
					
					//Changing the settings of the item and parenting it to the inventory manager again.
					if(SkillSlot[t].Item.GetComponent<Collider>() != null)
					{
						SkillSlot[t].Item.transform.GetComponent<Collider>().isTrigger = true;
					}
					SkillSlot[t].Item.transform.GetComponent<Renderer>().enabled = false;
					SkillSlot[t].Item.transform.parent = gameObject.transform;
					SkillSlot[t].Item.transform.localPosition  = Vector3.zero;
					SkillSlot[t].Item.gameObject.SetActive(false);
				}
			}
		}
	}
}