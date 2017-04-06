/*

Vendor UI Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VendorUI : MonoBehaviour {
	
	public GameObject UICanvas; //Main canvas.
	[HideInInspector]
	public RectTransform UICanvasTrans;
	
	public GameObject Panel; //The vendor's UI.
	[HideInInspector]
	public RectTransform PanelTrans;

	public enum VendorPositions {TopRightCorner = 0, LowerRightCorner = 1, TopLeftCorner = 2, LowerLeftCorner = 3, Center = 4} //Vendor's starting position
	public VendorPositions VendorPosition= VendorPositions.TopRightCorner & VendorPositions.LowerRightCorner & VendorPositions.TopLeftCorner & VendorPositions.LowerLeftCorner & VendorPositions.Center;
	
	public bool IsMovable = true; //Can the player change the position of the UI window?

	public bool EnableToggleInfo = true;
	public Text ToggleInfo; //Toggle info is a text UI element that will show the required action for the player to interact with the vendor whether by mouse or keyboard. It should be a child object of the main canvas only.
	public Text VendorName;

	public GameObject BuyButton; //The button that will allow the player to purchase items.
	public GameObject SellButton; //The button that will allow the player to sell his items.
	public GameObject TakeBackButton; //The button that will allow the player to take back his item(s) to the inventory after putting them on sale.
	
	//This class will hold the available slots 
	[System.Serializable]
	public class UIVars
	{
		public Image EmptySlot; //The empty slot placed behind the item's icon (Optional).
		public Image Icon; //The item's icon (Required).
		public Text Textfield; //The item's name and amount (Optional).
		public Text PriceText;
		public Image CurrencyIcon;
	}
	[HideInInspector]
	public UIVars[] UISlot;

	public UIVars Slot;
	public int MaxItems = 3;
	public int SlotsPerRow= 4; 
	public int SlotsVerticalSpacing= 4;
	public int SlotsHorizontalSpacing= 4;
	
	[HideInInspector]
	public int SelectedItem = -1; //Current selected item by the player.
	[HideInInspector]
	public int SellDragSlot = -1;
	[HideInInspector]
	public Color LastTextColor;
	
	[HideInInspector]
	public bool  BuyMenu = true; //If true, we are on the buy menu. If not, we are on the sell menu.
	[HideInInspector]
	public Vendor ActiveVendor; //The current active vendor.

	public bool TakeBackOnDoubleClick = true;
	public bool PurchaseOnDoubleClick = true;

	public bool HoverInfoON = true;
	//Scripts:
	[HideInInspector]
	public InventoryManager InvManager;
	
	//Double click: for purchase and taking back items.
	[HideInInspector]
	public int Clicks;
	[HideInInspector]
	public float DoubleClickTimer;
	[HideInInspector]
	public int LastClickID = -1;

	[HideInInspector]
	public bool  Dragging = false;
	
	
	void  Awake (){
		ActiveVendor = null; //Set the active vendor to null on start.
		
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		
		//Setting up rect transforms for the vendor's UI panel and main canvas object:
		UICanvasTrans = UICanvas.GetComponent<RectTransform>();
		PanelTrans = Panel.GetComponent<RectTransform>();
		
		SetVendorPosition(); // Set the default position of the inventory backdrop.
		
		Panel.SetActive(false); //Disabling the panel.

		CreateSlots ();
	}

	public void  CreateSlots (){

		UISlot = new UIVars[MaxItems];
		
		for(int i = 0; i < MaxItems; i++)
		{
			UISlot[i] = new UIVars();
		}

		float X= Slot.EmptySlot.GetComponent<RectTransform>().localPosition.x;
		float Y= Slot.EmptySlot.GetComponent<RectTransform>().localPosition.y;

		Slot.EmptySlot.name = "EmptySlot";
		Slot.Icon.name = "Icon";
		Slot.Textfield.name = "Name";
		Slot.CurrencyIcon.name = "CurrencyIcon";
		Slot.PriceText.name = "Price";
		
		int CurrentSlotsInRow = 0;
		int SlotsCount = 0;
		for(int i = 0; i < MaxItems; i++) //Starting a loop in the slots of the bag.
		{
			GameObject ItemSlot = Instantiate(Slot.EmptySlot.gameObject) as GameObject;

			UISlot[i].EmptySlot = ItemSlot.GetComponent<Image>();
			for(int j = 0; j < UISlot[i].EmptySlot.transform.childCount; j++)
			{

				switch (UISlot[i].EmptySlot.transform.GetChild(j).gameObject.name)
				{
				case "Icon":
					UISlot[i].Icon = UISlot[i].EmptySlot.transform.GetChild(j).gameObject.GetComponent<Image>();
					break;
				case "Name":
					UISlot[i].Textfield = UISlot[i].EmptySlot.transform.GetChild(j).gameObject.GetComponent<Text>();
					break;
				case "CurrencyIcon":
					UISlot[i].CurrencyIcon = UISlot[i].EmptySlot.transform.GetChild(j).gameObject.GetComponent<Image>();
					break;
				case "Price":
					UISlot[i].PriceText = UISlot[i].EmptySlot.transform.GetChild(j).gameObject.GetComponent<Text>();
					break;
				}
			}


			ItemSlot.transform.SetParent(Panel.transform, false);

			ItemSlot.GetComponent<RectTransform>().localPosition = new Vector3(X,Y,0);

			X += ItemSlot.GetComponent<RectTransform>().rect.width+SlotsHorizontalSpacing;
			CurrentSlotsInRow++;
			
			if(CurrentSlotsInRow == SlotsPerRow)
			{
				X = Slot.EmptySlot.GetComponent<RectTransform>().localPosition.x;
				Y -= ItemSlot.GetComponent<RectTransform>().rect.height+SlotsVerticalSpacing;
				
				CurrentSlotsInRow = 0;
			}
			
			ItemSlot.gameObject.SetActive(true);
			
			ItemSlot.transform.localScale = Slot.EmptySlot.transform.localScale;

		}

		Destroy(Slot.EmptySlot.gameObject);
	}

	void Update ()
	{
		if(Dragging == true)
		{
			Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition;
			InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
		}
	}
	
	void  SetVendorPosition (){
		PanelTrans.anchorMax = new Vector2(0.5f,0.5f);
		PanelTrans.anchorMin = new Vector2(0.5f,0.5f);
		PanelTrans.pivot = new Vector2(0f,0f);

		switch(VendorPosition)
		{
		case VendorPositions.TopRightCorner: //Top Right corner
			PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(PanelTrans.rect.height-Screen.height/2), 0);
			break;
			
		case VendorPositions.LowerRightCorner: //Lower right corner
			PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(Screen.height/2), 0);
			break;
			
		case VendorPositions.TopLeftCorner: //Top Left corner
			PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(PanelTrans.rect.height-Screen.height/2), 0);
			break;
			
		case VendorPositions.LowerLeftCorner: //Lower Left corner
			PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(Screen.height/2), 0);
			break; 
			
		case VendorPositions.Center: //Center
			PanelTrans.localPosition = new Vector3(-Panel.GetComponent<RectTransform>().rect.width/2,-Panel.GetComponent<RectTransform>().rect.height/2, 0);
			break;  
		}
	}
	
	public void  DesactivatePanel (){
		SellItems();
		ActiveVendor.TriggerVendor();
		
		//Desactivate the panel:
		ActiveVendor = null;
		
		SellDragSlot = -1;
	}
	
	public void  OpenBuyMenu (){
		//Switching to items buy menu by triggering the required buttons.
		BuyButton.SetActive(true);
		SellButton.SetActive(false);
		TakeBackButton.SetActive(false);

		if(SelectedItem >= 0) UISlot[SelectedItem].Textfield.color = LastTextColor;
		SelectedItem = -1;
		SellDragSlot = -1;
		
		int UISlotsCount = 0;
		if(UISlot.Length >= ActiveVendor.ForSale.Length) //Only works if we have enough slots on the vendor UI.
		{
			for(int i = 0; i < UISlot.Length; i++) //Loop through all the slots.
			{
				if(UISlotsCount < ActiveVendor.ForSale.Length)
				{
					if(UISlot[i].EmptySlot) //Enabling the empty if it exists
					{
						UISlot[i].EmptySlot.gameObject.SetActive(true);
					}
					if(UISlot[i].Icon) //Setting the icon of the item for sale.
					{
						UISlot[i].Icon.gameObject.SetActive(true);
						UISlot[i].Icon.sprite = ActiveVendor.ForSale[i].gameObject.GetComponent<Item>().Icon;
						UISlot[i].Icon.gameObject.GetComponent<SlotUI>().SlotID = i;
					}
					if(UISlot[i].Textfield) //Putting the name and quantity for the item for sale.
					{
						UISlot[i].Textfield.gameObject.SetActive(true);
						UISlot[i].Textfield.text = ActiveVendor.ForSale[i].gameObject.GetComponent<Item>().Name + ": " + ActiveVendor.ForSale[i].gameObject.GetComponent<Item>().Amount.ToString();
					}
					if(UISlot[i].CurrencyIcon) //Putting the name and quantity for the item for sale.
					{
						UISlot[i].CurrencyIcon.gameObject.SetActive(true);
						UISlot[i].CurrencyIcon.sprite = GetCurrencyIcon(ActiveVendor.ForSale[i].gameObject.GetComponent<Item>().Currency);
					}
					if(UISlot[i].PriceText) //Putting the name and quantity for the item for sale.
					{
						UISlot[i].PriceText.gameObject.SetActive(true);
						UISlot[i].PriceText.text = ActiveVendor.ForSale[i].gameObject.GetComponent<Item>().CurrencyAmount.ToString();
					}
					UISlotsCount++;
				}
				else //If we have placed all the items for sale, disable the rest of the slots.
				{
					if(UISlot[i].EmptySlot) UISlot[i].EmptySlot.gameObject.SetActive(false);
					if(UISlot[i].Icon) UISlot[i].Icon.gameObject.SetActive(false);
					if(UISlot[i].Textfield) UISlot[i].Textfield.gameObject.SetActive(false);
					if(UISlot[i].CurrencyIcon) UISlot[i].CurrencyIcon.gameObject.SetActive(false);
					if(UISlot[i].PriceText) UISlot[i].PriceText.gameObject.SetActive(false);
				}
			}
		}
		else 
		{
			Debug.LogError("Vendor UI: There aren't enough item slots to display all the items for sale from Vendor: '" + ActiveVendor.gameObject.name + "'.");
		}
	}

	public Sprite GetCurrencyIcon (string Name)
	{
		for(int i = 0; i < InvManager.Riches.Length; i++)
		{
			if(Name == InvManager.Riches[i].Name) return InvManager.Riches[i].Icon;
		}

		return null;
	}
	
	public void  OpenSellMenu (){
		//Switching to items sell menu by triggering the required buttons.
		BuyButton.SetActive(false);
		SellButton.SetActive(true);
		TakeBackButton.SetActive(true);
		
		//Setting the items icons to sell:
		RefreshSellItems();

		if(SelectedItem >= 0) UISlot[SelectedItem].Textfield.color = LastTextColor;
		SelectedItem = -1;
		
	}
	
	public void  RefreshSellItems (){
		int UISlotsCount = 0;
		if(UISlot.Length >= ActiveVendor.ItemsToSell.Length) //Only works if we have enough slots on the vendor UI.
		{
			for(int i = 0; i < UISlot.Length; i++) //Loop through all the slots.
			{
				if(UISlotsCount == ActiveVendor.ItemsToSell.Length && UISlotsCount <= UISlot.Length)
				{
					if(UISlot[i].EmptySlot) //Enabling the empty slot if it exists
					{
						UISlot[i].EmptySlot.gameObject.SetActive(true);
					}
					if(UISlot[i].Icon)
					{
						UISlot[i].Icon.gameObject.SetActive(true);
						UISlot[i].Icon.sprite = UISlot[i].EmptySlot.sprite;
						UISlot[i].Icon.gameObject.GetComponent<SlotUI>().SlotID = i;
					}
					if(UISlot[i].Textfield)
					{
						UISlot[i].Textfield.gameObject.SetActive(true);
						UISlot[i].Textfield.text = "Drag here to sell (Max: " + UISlot.Length+")";
					}
					if(UISlot[i].CurrencyIcon) UISlot[i].CurrencyIcon.gameObject.SetActive(false);
					if(UISlot[i].PriceText) UISlot[i].PriceText.gameObject.SetActive(false);

					
					//Slot to drop item for sale
					SellDragSlot = i;
					
					UISlotsCount++;
				}
				else if(UISlotsCount < ActiveVendor.ItemsToSell.Length)
				{
					UISlotsCount++;
					if(UISlot[i].EmptySlot) //Enabling the empty slot if it exists
					{
						UISlot[i].EmptySlot.gameObject.SetActive(true);
					}
					if(UISlot[i].Icon) //Setting the icon of the item for sale.
					{
						UISlot[i].Icon.gameObject.SetActive(true);
						UISlot[i].Icon.sprite = ActiveVendor.ItemsToSell[i].gameObject.GetComponent<Item>().Icon;
						UISlot[i].Icon.gameObject.GetComponent<SlotUI>().SlotID = i;
					}
					if(UISlot[i].Textfield) //Putting the name and quantity for the item for sale.
					{
						UISlot[i].Textfield.gameObject.SetActive(true);
						UISlot[i].Textfield.text = ActiveVendor.ItemsToSell[i].gameObject.GetComponent<Item>().Name + ": " + ActiveVendor.ItemsToSell[i].gameObject.GetComponent<Item>().Amount.ToString();
					}
					if(UISlot[i].CurrencyIcon) //Putting the name and quantity for the item for sale.
					{
						UISlot[i].CurrencyIcon.gameObject.SetActive(true);
						UISlot[i].CurrencyIcon.sprite = GetCurrencyIcon(ActiveVendor.ItemsToSell[i].gameObject.GetComponent<Item>().Currency);
					}
					if(UISlot[i].PriceText) //Putting the name and quantity for the item for sale.
					{
						UISlot[i].PriceText.gameObject.SetActive(true);
						int Price = ActiveVendor.ItemsToSell[i].gameObject.GetComponent<Item>().CurrencyAmount*ActiveVendor.ItemsToSell[i].gameObject.GetComponent<Item>().Amount;
						UISlot[i].PriceText.text = Price.ToString();
					}
				}
				else //If we have placed all the items for sale, disable the rest of the slots.
				{
					if(UISlot[i].EmptySlot) UISlot[i].EmptySlot.gameObject.SetActive(false);
					if(UISlot[i].Icon) UISlot[i].Icon.gameObject.SetActive(false);
					if(UISlot[i].Textfield) UISlot[i].Textfield.gameObject.SetActive(false);
					if(UISlot[i].CurrencyIcon) UISlot[i].CurrencyIcon.gameObject.SetActive(false);
					if(UISlot[i].PriceText) UISlot[i].PriceText.gameObject.SetActive(false);
				}

				if(UISlot.Length == ActiveVendor.ItemsToSell.Length) SellDragSlot = -1;
			}
		}
		else 
		{
			Debug.LogError("Vendor UI: There aren't enough item slots to display all the items for sale from Vendor: '" + ActiveVendor.gameObject.name + "'.");
		}
	}
	
	public void ShowToggleInfo () //Showing the info to interact with the vendor here:
	{
		if(ToggleInfo != null && ActiveVendor == null && EnableToggleInfo == true) //Showing it only if the UI isn't active.
		{
			ToggleInfo.gameObject.SetActive(true);
			//Display the vendor interaction info for both control types:
			if(InvManager.PickupType == InventoryManager.PickupTypes.Mouse) //If we are using the mouse to pick up items:
			{
				ToggleInfo.text =  "Press the left mouse button to talk to the vendor.";
			}
			if(InvManager.PickupType == InventoryManager.PickupTypes.Keyboard) //If we are using the keyboard to pick up items.
			{
				ToggleInfo.text = "Press " + InvManager.ActionKey.ToString() + " to talk to the vendor.";
			}
			if(ToggleInfo.gameObject.transform.parent != null)
			{
				ToggleInfo.gameObject.transform.parent.gameObject.SetActive(true);
			}
		}
	}
	
	public void HideToggleInfo () //Hiding the toggle info message when we aren't in range of the vendor anymore.
	{
		if(ToggleInfo != null)
		{
			ToggleInfo.gameObject.SetActive(false);
			if(ToggleInfo.gameObject.transform.parent != null)
			{
				ToggleInfo.gameObject.transform.parent.gameObject.SetActive(false);
			}
		}
	}
	
	public void PurchaseItem () //Called when the player clicks on the purchase item button:
	{
		if(SelectedItem >= 0) //If the player has selected an item:
		{
			ActiveVendor.PurchaseItem(ActiveVendor.ForSale[SelectedItem]);
		}
	}
	
	public void SellItems () //When player clicks on the sell item script:
	{
		ActiveVendor.SellItems();
		if(SelectedItem >= 0) UISlot[SelectedItem].Textfield.color = LastTextColor;
		SelectedItem = -1;
		
		RefreshSellItems();
	}
	
	public void  TakeBackItem (){
		if(SelectedItem >= 0 && SelectedItem != SellDragSlot) //If the player has selected an item:
		{
			ActiveVendor.TakeBackItem(ActiveVendor.ItemsToSell[SelectedItem]);
			if(SelectedItem >= 0) UISlot[SelectedItem].Textfield.color = LastTextColor;
			SelectedItem = -1;
			
			RefreshSellItems();
		}    
	}

	public void  TakeBackAllItems (){
	    if(ActiveVendor.ForSale.Length > 0)
		{
			for(int i = 0; i < ActiveVendor.ForSale.Length; i++)
			{
				SelectedItem = 0;
				TakeBackItem();
			}
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
			
			if(TakeBackButton.active == true) 
			{
				if(TakeBackOnDoubleClick && ID != ActiveVendor.ItemsToSell.Length && ActiveVendor.ItemsToSell.Length > 0) TakeBackItem();
			}
			else
			{
				if(PurchaseOnDoubleClick) PurchaseItem();
			}
		
		}
	}
}