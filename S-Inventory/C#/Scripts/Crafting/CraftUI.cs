/*
    Craft UI script created by SoumiDelRio.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CraftUI : MonoBehaviour {

	public GameObject UICanvas;
	[HideInInspector]
	public RectTransform UICanvasTrans;
	
	public GameObject Panel; //The equipment's backdrop.
	[HideInInspector]
	public RectTransform PanelTrans;
	
	//Position in screen:
	public enum CraftPositions {TopRightCorner = 0, LowerRightCorner = 1, TopLeftCorner = 2, LowerLeftCorner = 3, Center = 4} //Inventory's starting position
	public CraftPositions CraftPosition= CraftPositions.TopRightCorner & CraftPositions.LowerRightCorner & CraftPositions.TopLeftCorner & CraftPositions.LowerLeftCorner & CraftPositions.Center;
	
	//Crafting Slots:
	[System.Serializable]
	public class CraftSlotsVars 
	{
		[HideInInspector]
		public bool  IsTaken = false;
		public Image EmptySlot;
		public Image Icon;
		public Text Text;
	}
	public CraftSlotsVars[] CraftSlots;
	
	public Image ResultIcon; //Result item.
	
	public GameObject[] RecipeButtons;

	public Text CraftingInfo;

	//If the recipe requires currency, then we'll show the currency icon and the amount with the Image and Text UI elements below:
	public Text CurrencyText;
	public Image CurrencyIcon;
	
	//Scripts:
	[HideInInspector]
	public InventoryManager InvManager;
	[HideInInspector]
	public CraftManager InvCraft;
	[HideInInspector]
	public InventoryEvents CustomEvents;
	
	//Double click:
	[HideInInspector]
	public int Clicks;
	[HideInInspector]
	public float DoubleClickTimer;
	[HideInInspector]
	public int LastClickID = -1;

	public bool IsMovable = true; //Can the player change the position of the UI window?
	[HideInInspector]
	public bool  Dragging = false;

	public RectTransform FullBar;
	public RectTransform EmptyBar;

	[HideInInspector]
	public float BarWidth = 0;
	[HideInInspector]
	public float PosX = 0;
	
	void  Awake (){
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		InvCraft = FindObjectOfType(typeof(CraftManager)) as CraftManager; //Get the Equipment script. 
		CustomEvents = FindObjectOfType(typeof(InventoryEvents)) as InventoryEvents;
		
		//Setting up rect transforms:
		UICanvasTrans = UICanvas.GetComponent<RectTransform>();
		PanelTrans = Panel.GetComponent<RectTransform>();
		
		SetCraftPosition();
		
		Panel.SetActive(false);
		
		//Set craft slots:
		if(CraftSlots.Length > 0)
		{
			for(int i = 0; i < CraftSlots.Length; i++)
			{
				CraftSlots[i].EmptySlot.gameObject.GetComponent<SlotUI>().SlotID = i;
			}
		}
		
		//Set Recipe IDs:
		if(RecipeButtons.Length > 0)
		{
			for(int a = 0; a < RecipeButtons.Length; a++)
			{
				RecipeButtons[a].GetComponent<SlotUI>().SlotID = a;
				InvCraft.Recipes[a].ItemReady = false;
				InvCraft.Recipes[a].Timer = 0;
			}
		}
		
		RefreshItems();

		BarWidth = FullBar.rect.width;
		PosX = FullBar.localPosition.x;
	}
	
	void  Update (){
		if(DoubleClickTimer > 0)
		{
			DoubleClickTimer -= Time.deltaTime;
		}
		if(DoubleClickTimer < 0)
		{
			DoubleClickTimer = 0;
			Clicks = 0;
			LastClickID = -1;
		}

		for(int i = 0; i < InvCraft.Recipes.Length; i++) //Loop trhough all the items needed.
		{
			if(InvCraft.Recipes[i].Timer > 0)
			{
				InvCraft.Recipes[i].Timer -= Time.deltaTime;

				if(i == InvCraft.RecipeID)
				{
					FullBar.gameObject.SetActive(true);
					EmptyBar.gameObject.SetActive(true);
					
					FullBar.sizeDelta = new Vector2(((InvCraft.Recipes[i].TimeRequired-InvCraft.Recipes[i].Timer)/(InvCraft.Recipes[i].TimeRequired)) * BarWidth, FullBar.sizeDelta.y);
					FullBar.localPosition = new Vector3 (-(BarWidth-FullBar.rect.width)/2 + PosX,FullBar.localPosition.y,FullBar.localPosition.z);
				}

				if(i == InvCraft.RecipeID)
				{
					float Percentage = 100-((InvCraft.Recipes[i].Timer*100)/InvCraft.Recipes[i].TimeRequired); 
					int Int;
					Int = Mathf.CeilToInt(Percentage);
					CraftingInfo.text = Int.ToString()+"%";
				}
			}
			if(InvCraft.Recipes[i].Timer < 0)
			{
				InvCraft.Recipes[i].Timer = 0;
				InvCraft.Recipes[i].ItemReady = true;
				if(InvManager.AudioSC) InvManager.AudioSC.PlayOneShot(InvCraft.FinishedCrafting);
				RefreshItems ();
			}
		}

		if(Dragging == true)
		{
			Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition;
			InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
		}
	}
	
	void  SetCraftPosition (){   
		PanelTrans.anchorMax = new Vector2(0.5f,0.5f);
		PanelTrans.anchorMin = new Vector2(0.5f,0.5f);
		PanelTrans.pivot = new Vector2(0f,0f);

		switch(CraftPosition)
		{
		case CraftPositions.TopRightCorner: //Top Right corner
			PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(PanelTrans.rect.height-Screen.height/2), 0);
			break;
			
		case CraftPositions.LowerRightCorner: //Lower right corner
			PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(Screen.height/2), 0);
			break;
			
		case CraftPositions.TopLeftCorner: //Top Left corner
			PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(PanelTrans.rect.height-Screen.height/2), 0);
			break;
			
		case CraftPositions.LowerLeftCorner: //Lower Left corner
			PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(Screen.height/2), 0);
			break; 
			
		case CraftPositions.Center: //Center
			PanelTrans.localPosition = new Vector3(-Panel.GetComponent<RectTransform>().rect.width/2,-Panel.GetComponent<RectTransform>().rect.height/2, 0);
			break; 
		}
	}
	
	//Toggle Craft Menu:
	public void  ToggleCrafting (){
		Panel.SetActive(!Panel.active);

		if(CustomEvents)
		{
			if(Panel.active == true)
			{
				CustomEvents.OnPlayerOpenCrafting ();
			}
			else
			{
				CustomEvents.OnPlayerCloseCrafting();
			}
		}
	}
	
	public void  Click ( int ID  ){
		if(Clicks == 0 || ID != LastClickID)
		{
			Clicks++;
			DoubleClickTimer = 0.5f; //Change this if you want to decrease or increase the time between the mouse double clicks.
			LastClickID = ID;
		}
		else if(Clicks == 1 && ID == LastClickID)
		{
			DoubleClickTimer = 0;
			Clicks = 0;
			LastClickID = -1;
			
			if(InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[ID].IsTaken == true && InvCraft.Recipes[InvCraft.RecipeID].Timer == 0 && InvCraft.Recipes[InvCraft.RecipeID].ItemReady == false)
			{
				//we move it back to the inventory.
				InvManager.AddItem(InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[ID].Item); //Add it to the inventory.
				
				InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[ID].IsTaken = false;
				InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[ID].Item = null;
				
				InvCraft.Recipes[InvCraft.RecipeID].ItemsAmount--;
				
				RefreshItems(); 
			}   
		}
	}
	
	public void  CraftItem (){
		if(InvCraft.Recipes[InvCraft.RecipeID].ItemsAmount == InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded.Length && InvCraft.Recipes[InvCraft.RecipeID].Timer == 0 && InvCraft.Recipes[InvCraft.RecipeID].ItemReady == false) //Check if we have all the needed items for the craft:
		{
			if((InvCraft.Recipes[InvCraft.RecipeID].CurrencyRequired == true && InvManager.HaveCurrency(InvCraft.Recipes[InvCraft.RecipeID].CurrencyName, InvCraft.Recipes[InvCraft.RecipeID].CurrencyAmount)) || InvCraft.Recipes[InvCraft.RecipeID].CurrencyRequired == false)
			{
				if(InvCraft.Recipes[InvCraft.RecipeID].TimeRequired > 0)
				{
					InvCraft.Recipes[InvCraft.RecipeID].Timer = InvCraft.Recipes[InvCraft.RecipeID].TimeRequired;
				}
				else
				{
					InvCraft.Recipes[InvCraft.RecipeID].ItemReady = true;
					if(InvManager.AudioSC) InvManager.AudioSC.PlayOneShot(InvCraft.FinishedCrafting);
					RefreshItems ();
				}

				if(InvCraft.Recipes[InvCraft.RecipeID].CurrencyRequired == true)
				{
					InvManager.RemoveCurrency(InvCraft.Recipes[InvCraft.RecipeID].CurrencyName, InvCraft.Recipes[InvCraft.RecipeID].CurrencyAmount);
				
				}
			}
		}
	}

	public void TakeCraftingItem ()
	{
		if(InvCraft.Recipes[InvCraft.RecipeID].ItemReady == true)
		{
			//Loop through all the crafting items and remove them and give the player the craft result:
			for(int i = 0; i < InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded.Length; i++) //Loop trhough all the items needed.
			{
				if(InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].Destroy == true)
				{
					Destroy (InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].Item.gameObject);
					InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].IsTaken = false;
					InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].Item = null;
				}
				else
				{
					//we move it back to the inventory.
					InvManager.AddItem(InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].Item); //Add it to the inventory.
					
					InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].IsTaken = false;
					InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].Item = null;
					
					InvCraft.Recipes[InvCraft.RecipeID].ItemsAmount--;
					
					RefreshItems(); 
				}  
			}
			
			GameObject ResultObj = Instantiate(InvCraft.Recipes[InvCraft.RecipeID].Result, Vector3.zero, Quaternion.identity) as GameObject;
			InvManager.AddItem(ResultObj.transform); //Add the result item to the inventory.
			InvCraft.Recipes[InvCraft.RecipeID].ItemsAmount = 0;
			
			Item ResultScript = InvCraft.Recipes[InvCraft.RecipeID].Result.gameObject.GetComponent<Item>();
			CustomEvents.OnPlayerCraftItem(ResultScript);
			
			InvCraft.Recipes[InvCraft.RecipeID].ItemReady = false;

			RefreshItems();
		}
	}
	
	public void  ChangeRecipe ( int ID  ){
		InvCraft.RecipeID = ID;
		RefreshItems();
	}
	
	public void  RefreshItems (){
		for(int i = 0; i < InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded.Length; i++) //Starting a loop in the slots of the craft menu:
		{ 
			if(InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].IsTaken == true) //Check if the item in the current combination is taken.
			{
				Item ItemScript = InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].Item.GetComponent<Item>(); //Getting the item script.  
				
				CraftSlots[i].Icon.gameObject.SetActive(true);
				CraftSlots[i].Icon.sprite = ItemScript.Icon;
				
				if(CraftSlots[i].Text != null)
				{
					if(ItemScript.IsStackable == true) 
					{
						CraftSlots[i].Text.gameObject.SetActive(true);
						CraftSlots[i].Text.text = ItemScript.Amount.ToString();
					}   
					else
					{
						CraftSlots[i].Text.gameObject.SetActive(false);
					} 
				}    
			}
			else
			{
				CraftSlots[i].Icon.sprite = null;
				CraftSlots[i].Icon.gameObject.SetActive(false);
				
				if(CraftSlots[i].Text != null) 
				{
					CraftSlots[i].Text.gameObject.SetActive(true);
					
					if(InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].Quantity == 1)
					{
						CraftSlots[i].Text.text = InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].Name;
					}
					else
					{
						CraftSlots[i].Text.text = InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].Name + "("+InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[i].Quantity.ToString()+")";
					}
				}    
			}   
		}   

        for(int i = 0; i < CraftSlots.Length; i++)
        {
            if(i >= InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded.Length)
            {
                CraftSlots[i].Text.gameObject.SetActive(false);
                CraftSlots[i].Icon.gameObject.SetActive(false);
            }
        }
		
		Item ResultScript = InvCraft.Recipes[InvCraft.RecipeID].Result.gameObject.GetComponent<Item>();
		
		ResultIcon.gameObject.SetActive(true);
		ResultIcon.sprite = ResultScript.Icon;

		if(InvCraft.Recipes[InvCraft.RecipeID].ItemReady == true)
		{
			CraftingInfo.text = "Ready";
		}
		else
		{
			CraftingInfo.text = "";
		}

		if(InvCraft.Recipes[InvCraft.RecipeID].ItemReady == true)
		{
			FullBar.gameObject.SetActive(true);
			EmptyBar.gameObject.SetActive(true);
		}
		else
		{
			if(InvCraft.Recipes[InvCraft.RecipeID].Timer == 0)
			{
				FullBar.gameObject.SetActive(false);
				EmptyBar.gameObject.SetActive(false);
			}
		}

		if(InvCraft.Recipes[InvCraft.RecipeID].CurrencyRequired == true)
		{
			CurrencyIcon.gameObject.SetActive(true);
			CurrencyIcon.sprite = InvManager.GetCurrencyIcon(InvCraft.Recipes[InvCraft.RecipeID].CurrencyName);
			CurrencyText.gameObject.SetActive(true);
			CurrencyText.text = InvCraft.Recipes[InvCraft.RecipeID].CurrencyAmount.ToString();
		}
		else
		{
			CurrencyIcon.gameObject.SetActive(false);
			CurrencyText.gameObject.SetActive(false);
		}
	}

	public void  DragStarted (){
		if(IsMovable == true && Dragging == false)
		{
			Dragging = true;
			InvManager.InvUI.PanelDragPos.gameObject.SetActive(true);
			Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition;
			InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
			Panel.transform.SetParent(InvManager.InvUI.PanelDragPos.transform, true);
			InvManager.InvUI.CloseActionMenu();
		}
	}
	
	public void  DragEnded (){
		if(IsMovable == true)
		{
			Dragging = false;
			InvManager.InvUI.PanelDragPos.gameObject.SetActive(false);
			Panel.transform.SetParent(UICanvas.transform, true);
			InvManager.InvUI.Panel.transform.SetParent(InvManager.InvUI.PanelDragPos.transform, true);
			InvManager.InvUI.Panel.transform.SetParent(UICanvas.transform, true);
			
		}
	}
}