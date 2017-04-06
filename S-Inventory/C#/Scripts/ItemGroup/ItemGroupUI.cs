/*

Item Group UI Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemGroupUI : MonoBehaviour {
	
	public GameObject UICanvas;
	[HideInInspector]
	public RectTransform UICanvasTrans;
	
	public GameObject Panel; //The inventory's backdrop.
	[HideInInspector]
	public RectTransform PanelTrans;
	
	//Item Slots:
	[System.Serializable]
	public class ItemVars 
	{
		public Image EmptySlot;
		public Image Icon;
		public Text Textfield;
	}
	public ItemVars[] Items;

	public bool EnableToggleInfo = true;
	public Text ToggleInfo;
	
	[HideInInspector]
	public Color LastTextColor;
	
	[HideInInspector]
	public int SelectedItem = -1; //Current selected item by the player.
	
	public enum ItemGroupPositions {TopRightCorner = 0, LowerRightCorner = 1, TopLeftCorner = 2, LowerLeftCorner = 3, Center = 4} //Inventory's starting position
	public ItemGroupPositions ItemGroupPosition= ItemGroupPositions.TopRightCorner & ItemGroupPositions.LowerRightCorner & ItemGroupPositions.TopLeftCorner & ItemGroupPositions.LowerLeftCorner & ItemGroupPositions.Center;
	
	//Scripts:
	[HideInInspector]
	InventoryManager InvManager;
	
	[HideInInspector]
	public ItemGroup ActiveItemGroup; //The current active vendor.
	[HideInInspector]
	NotificationUI MsgUI;

	public bool IsMovable = true; //Can the player change the position of the UI window?
	[HideInInspector]
	public bool  Dragging = false;
	
	void  Awake (){
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		MsgUI = FindObjectOfType(typeof(NotificationUI)) as NotificationUI;
		
		ActiveItemGroup = null; //Set the active vendor to null on start.
		
		//Setting up rect transforms for the vendor's UI panel and main canvas object:
		UICanvasTrans = UICanvas.GetComponent<RectTransform>();
		PanelTrans = Panel.GetComponent<RectTransform>();
		
		SetItemGroupPosition ();
		
		Panel.SetActive(false); //Disabling the panel.
		
		if(Items.Length > 0)
		{
			for(int i = 0; i < Items.Length; i++)
			{
				Items[i].Icon.gameObject.GetComponent<SlotUI>().SlotID = i;
			}
		}
	}

	void Update ()
	{
		if(Dragging == true)
		{
			Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition;
			InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
		}
	}
	
	void  SetItemGroupPosition (){
		PanelTrans.anchorMax = new Vector2(0.5f,0.5f);
		PanelTrans.anchorMin = new Vector2(0.5f,0.5f);
		PanelTrans.pivot = new Vector2(0f,0f);


		switch(ItemGroupPosition)
		{
		case ItemGroupPositions.TopRightCorner: //Top Right corner
			PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(PanelTrans.rect.height-Screen.height/2), 0);
			break;
			
		case ItemGroupPositions.LowerRightCorner: //Lower right corner
			PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(Screen.height/2), 0);
			break;
			
		case ItemGroupPositions.TopLeftCorner: //Top Left corner
			PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(PanelTrans.rect.height-Screen.height/2), 0);
			break;
			
		case ItemGroupPositions.LowerLeftCorner: //Lower Left corner
			PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(Screen.height/2), 0);
			break; 
			
		case ItemGroupPositions.Center: //Center
			PanelTrans.localPosition = new Vector3(-Panel.GetComponent<RectTransform>().rect.width/2,-Panel.GetComponent<RectTransform>().rect.height/2, 0);
			break;   
		}
	}
	

	public void  DesactivatePanel (){
		ActiveItemGroup.TriggerItemGroup();

		//Desactivate the panel:
		ActiveItemGroup = null;
	}
	
	public void  TakeSelectedItems (){
		if(SelectedItem >= 0) //Check if the player has selected an item before pressing this button.
		{ 
			Transform ItemSelected = ActiveItemGroup.Content[SelectedItem];
			if(InvManager.Items < InvManager.MaxItems || ItemSelected.gameObject.GetComponent<Item>().ItemType == 1) //Check if we still have room for new item:
			{
				Item ItemScript = ItemSelected.gameObject.GetComponent<Item>(); //Getting the item script.

				//Removing this item from the item group content.
				List<Transform> TempContent = new List<Transform>(ActiveItemGroup.Content);
				TempContent.Remove(ItemSelected);
				ActiveItemGroup.Content = TempContent.ToArray();

				if(ItemScript.ItemType == 0)
				{
					InvManager.AddItem(ItemSelected); //Add this item to the bag
				}
				if(ItemScript.ItemType == 1)
				{
					InvManager.AddCurrency(ItemSelected); //Add this currency to the inventory
				}
				
				//Reset the selected item var:
				if(SelectedItem >= 0) Items[SelectedItem].Textfield.color = LastTextColor;
				SelectedItem = -1;
				RefreshItems ();
			}    
			else
			{
				if(MsgUI != null) MsgUI.SendMsg("Inventory is full!");
				SelectedItem = -1;
				return;
			}
		}
		
		if(ActiveItemGroup.Content.Length == 0) DesactivatePanel();
	}
	
	public void  TakeAll (){
		foreach(Transform i in ActiveItemGroup.Content) //Starting a loop in the content of the item group.
		{
			if(ActiveItemGroup.Content.Length > 0 || i.gameObject.GetComponent<Item>().ItemType == 1) //If item group still has some items.
			{
				if(InvManager.Items < InvManager.MaxItems) //Check we still have room for new item:
				{
					Item ItemScript = i.GetComponent<Item>(); //Getting the item script.
					//Removing all the items from the item group content.

					List<Transform> TempContent = new List<Transform>(ActiveItemGroup.Content);
					TempContent.Remove(i);
					ActiveItemGroup.Content = TempContent.ToArray();

					if(ItemScript.ItemType == 0)
					{
						InvManager.AddItem(i); //Add this item to the bag
					}
					if(ItemScript.ItemType == 1)
					{
						InvManager.AddCurrency(i); //Add this currency to the inventory
					}
				}    
				else
				{
					if(MsgUI != null) MsgUI.SendMsg("Inventory is full!");
					SelectedItem = -1;
					RefreshItems();
					return;
				}
			}    
		}
		
		SelectedItem = -1;
		if(ActiveItemGroup.Content.Length == 0)DesactivatePanel();
	}
	
	public void ShowToggleInfo () //Showing the info to interact with the vendor here:
	{
		if(ToggleInfo != null && ActiveItemGroup == null && EnableToggleInfo == true) //Showing it only if the UI isn't active.
		{
			ToggleInfo.gameObject.SetActive(true);
			//Display the vendor interaction info for both control types:
			if(InvManager.PickupType == InventoryManager.PickupTypes.Mouse) //If we are using the mouse to pick up items:
			{
				ToggleInfo.text =  "Press the left mouse button to open the item group.";
			}
			if(InvManager.PickupType == InventoryManager.PickupTypes.Keyboard) //If we are using the keyboard to pick up items.
			{
				ToggleInfo.text = "Press " + InvManager.ActionKey.ToString() + " to open the item group.";
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
	
	public void  RefreshItems (){
		for(int i = 0; i < Items.Length; i++) //Starting a loop in the slots of the bag.
		{ 
			if(i < ActiveItemGroup.Content.Length)
			{
				if(Items[i].EmptySlot != null) Items[i].EmptySlot.gameObject.SetActive(true);
				Item ItemScript = ActiveItemGroup.Content[i].gameObject.GetComponent<Item>(); //Getting the item script.  
				if(Items[i].Icon != null) 
				{
					Items[i].Icon.gameObject.SetActive(true);
					Items[i].Icon.sprite = ItemScript.Icon;
				}    
				if(Items[i].Textfield != null) 
				{
					Items[i].Textfield.gameObject.SetActive(true);
					if(ItemScript.Amount == 1) Items[i].Textfield.text = ItemScript.Name;
					if(ItemScript.Amount > 1) Items[i].Textfield.text = ItemScript.Name+"("+ItemScript.Amount.ToString()+")";
				}
			}
			else
			{
				if(Items[i].EmptySlot != null) Items[i].EmptySlot.gameObject.SetActive(false);
				if(Items[i].Icon != null) Items[i].Textfield.gameObject.SetActive(false);
				if(Items[i].Textfield != null) Items[i].Icon.gameObject.SetActive(false);
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
}