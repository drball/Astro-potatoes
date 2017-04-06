/*

Slot UI Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour {
	//This script is used most item UI icons. It holds the item id in a specific window and eases the interaction with the player.

	public int SlotID = 0;
	
	//Scripts:
	[HideInInspector]
	public InventoryUI InvUI;
	[HideInInspector]
	public VendorUI VendorPanel;
	[HideInInspector]
	public SkillBarUI SkillBarPanel;
	[HideInInspector]
	public EquipmentUI EquipmentPanel;
	[HideInInspector]
	public ContainerUI InvContainer;
	[HideInInspector]
	CraftUI InvCraft;
	[HideInInspector]
	ItemGroupUI InvItemGroup;
	[HideInInspector]
	HoverItem HoverScript;

	public bool InventoryItem = true;
	public bool EquipmentItem = false;
	public bool SkillBarItem = false;
	public bool VendorItem = false;
	public bool CraftingItem = false;
	public bool RecipeSlot = false;
	public bool ContainerItem = false;
	public bool ItemGroupItem = false;

	//Double click:
	[HideInInspector]
	public int Clicks;
	[HideInInspector]
	public float DoubleClickTimer;
	[HideInInspector]
	public int LastClickID = -1;
	
	void  Awake ()
	{
		InvUI = FindObjectOfType(typeof(InventoryUI)) as InventoryUI;
		VendorPanel = FindObjectOfType(typeof(VendorUI)) as VendorUI;
		SkillBarPanel = FindObjectOfType(typeof(SkillBarUI)) as SkillBarUI;
		EquipmentPanel = FindObjectOfType(typeof(EquipmentUI)) as EquipmentUI;
		InvContainer = FindObjectOfType (typeof(ContainerUI)) as ContainerUI;
		InvCraft = FindObjectOfType (typeof(CraftUI)) as CraftUI;
		InvItemGroup = FindObjectOfType (typeof(ItemGroupUI)) as ItemGroupUI;
		HoverScript = FindObjectOfType (typeof(HoverItem)) as HoverItem;
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
	}
	
	//Inventory functions:
	public void  ActionMenu (){
		//Active when the player clicks on an item to show a menu with different actions to apply on the item:
		if(InvUI.InvManager.Slots[SlotID].IsTaken == true && InvUI.Action == false && InvUI.DraggingItem == false) 
		{
			//Enable the action menu and set the current item ID.
			InvUI.ActionID = SlotID;
			InvUI.Action = true;
			InvUI.ActionMenu.gameObject.SetActive(true);
			
			//Desactivate hovering info on item:
			HoverScript.HoverActive = false;
			if(HoverScript.ItemInfo.gameObject.active == true) HoverScript.ItemInfo.gameObject.SetActive(false);
			
			//Set the action menu positions in relation to the item.
			Vector3 TempPos = Input.mousePosition - InvUI.UICanvasTrans.localPosition;
			InvUI.ActionMenu.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x,TempPos.y,0);
		}

		/*//Right click to use the item if it's consuamble.
		if (Input.GetMouseButtonUp (1))
		{
			if(InvUI.InvManager.Slots[SlotID].IsTaken == true && InvUI.InvManager.Slots[SlotID].Item.GetComponent<Item>().Usable == true)
			{
				InvUI.CustomEvents.OnPlayerUseInventoryItem(InvUI.InvManager.Slots[SlotID].Item.GetComponent<Item>().Name);
				InvUI.InvManager.RemoveItem(InvUI.InvManager.Slots[SlotID].Item, 1);
			}
		}

		Click (SlotID);-*/
					
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
			
			Item ItemScript = InvUI.InvManager.Slots[SlotID].Item.GetComponent<Item>(); //Getting the item script.  
			InvUI.CustomEvents.OnPlayerUnEquipItem(ItemScript, -1);
			
			if(ItemScript.EquipmentSlot != "")
			{
				InvUI.UseItem(InvUI.InvManager.Slots[SlotID].Item, -1);
			}
			else if(ItemScript.SkillSlot != "")
			{
				InvUI.MoveToSkill(InvUI.InvManager.Slots[SlotID].Item, -1);
			}
		}
	}
	
	public void HoverON () //Enable showing item's info on hovering.
	{
		if(InvUI.DraggingItem == true || InvContainer.DraggingItem == true || InvUI.Action == true || InvUI.Dragging == true) return;
		if (InventoryItem == true && InvUI.InvManager.Slots [SlotID].IsTaken == true) {
			HoverScript.Item = InvUI.InvManager.Slots [SlotID].Item;
			HoverScript.ItemInfo.gameObject.SetActive (true);
			HoverScript.HoverActive = true; 

			HoverScript.ItemInfo.transform.SetParent (InvUI.Panel.transform, true);
		} else if (ContainerItem == true && InvContainer.ActiveContainer.Slots [SlotID].IsTaken == true) {
			HoverScript.Item = InvContainer.ActiveContainer.Slots [SlotID].Item;
			HoverScript.ItemInfo.gameObject.SetActive (true);
			HoverScript.HoverActive = true; 

			HoverScript.ItemInfo.transform.SetParent (InvContainer.Panel.transform, true);
		} else if (ItemGroupItem == true) {
			HoverScript.Item = InvItemGroup.ActiveItemGroup.Content [SlotID];
			HoverScript.ItemInfo.gameObject.SetActive (true);
			HoverScript.HoverActive = true; 

			HoverScript.ItemInfo.transform.SetParent (InvItemGroup.Panel.transform, true);
		} 
		else if (EquipmentItem == true && EquipmentPanel.InvEquipment.EquipmentSlots[SlotID].IsTaken == true) 
		{
			HoverScript.Item = EquipmentPanel.InvEquipment.EquipmentSlots[SlotID].Item;
			HoverScript.ItemInfo.gameObject.SetActive (true);
			HoverScript.HoverActive = true; 
			
			HoverScript.ItemInfo.transform.SetParent (EquipmentPanel.Panel.transform, true);
		}
		else if (VendorItem == true && VendorPanel.HoverInfoON == true) 
		{
			if(VendorPanel.TakeBackButton.active == true)
			{
				if(SlotID != VendorPanel.ActiveVendor.ItemsToSell.Length && VendorPanel.ActiveVendor.ItemsToSell.Length > 0) 
				{
					HoverScript.Item = VendorPanel.ActiveVendor.ItemsToSell[SlotID];
					HoverScript.ItemInfo.gameObject.SetActive (true);
					HoverScript.HoverActive = true; 
					
					HoverScript.ItemInfo.transform.SetParent (VendorPanel.Panel.transform, true);
				}
			}
			else
			{
				HoverScript.Item = VendorPanel.ActiveVendor.ForSale[SlotID];
				HoverScript.ItemInfo.gameObject.SetActive (true);
				HoverScript.HoverActive = true; 
				
				HoverScript.ItemInfo.transform.SetParent (VendorPanel.Panel.transform, true);
			}
		}
	}
	
	public void HoverOFF () //Hide item's info when hover is off.
	{
		HoverScript.HoverActive = false;
		if(HoverScript.ItemInfo.gameObject.active == true) HoverScript.ItemInfo.gameObject.SetActive(false);
		HoverScript.ItemInfo.transform.SetParent(InvUI.UICanvas.transform,true);
	}
	
	public void DragItem () //Start dragging the item.
	{
		if(InventoryItem == true)
		{
			if(InvUI.Action == false && InvUI.InvManager.Slots[SlotID].IsTaken == true)
			{
				//Drag settings for this specific item:
				InvUI.DraggingItem = true;
				InvUI.InvManager.Slots[SlotID].Dragged = true;
				InvUI.DragSlot.SetActive(true);
				InvUI.DragSlot.GetComponent<Image>().sprite = InvUI.InvManager.Slots[SlotID].Item.GetComponent<Item>().Icon;
				
				InvUI.DragID = SlotID;
				
				//Desactivate item description on mouse hover.
				HoverScript.HoverActive = false;
				if(HoverScript.ItemInfo.gameObject.active == true) HoverScript.ItemInfo.gameObject.SetActive(false);

				InvUI.RefreshItems();

				InvUI.Panel.transform.SetParent(InvUI.PanelDragPos.transform, true);
				InvUI.Panel.transform.SetParent(InvUI.UICanvas.transform, true);
			}
		}
		else if(ContainerItem == true && InvContainer.ActiveContainer != null && InvContainer.ActiveContainer.Slots[SlotID].IsTaken == true)
		{
			//Drag settings for this specific item:
			InvContainer.DraggingItem = true;
			InvContainer.ActiveContainer.Slots[SlotID].Dragged = true;

			InvContainer.DragSlot.SetActive(true);
			InvContainer.DragSlot.GetComponent<Image>().sprite = InvContainer.ActiveContainer.Slots[SlotID].Item.GetComponent<Item>().Icon;
			
			InvContainer.DragID = SlotID;
			
			//Desactivate item description on mouse hover.
			HoverScript.HoverActive = false;
			if(HoverScript.ItemInfo.gameObject.active == true) HoverScript.ItemInfo.gameObject.SetActive(false);
			
			InvContainer.RefreshItems();

			InvContainer.Panel.transform.SetParent(InvUI.PanelDragPos.transform, true);
			InvContainer.Panel.transform.SetParent(InvContainer.UICanvas.transform, true);

		}
		else if(EquipmentItem == true && EquipmentPanel.InvEquipment.EquipmentSlots[SlotID].IsTaken == true)
		{
			//Drag settings for this specific item:
			EquipmentPanel.DraggingItem = true;
			EquipmentPanel.InvEquipment.EquipmentSlots[SlotID].Dragged = true;
			
			EquipmentPanel.DragSlot.SetActive(true);
			EquipmentPanel.DragSlot.GetComponent<Image>().sprite = EquipmentPanel.InvEquipment.EquipmentSlots[SlotID].Item.GetComponent<Item>().Icon;
			
			EquipmentPanel.DragID = SlotID;
			EquipmentPanel.DragTarget = -1;
			
			EquipmentPanel.InvEquipment.EquipmentSlots[SlotID].Icon.color = new Color(1.0f,1.0f,1.0f,0.0f);

			EquipmentPanel.Panel.transform.SetParent(InvUI.PanelDragPos.transform, true);
			EquipmentPanel.Panel.transform.SetParent(EquipmentPanel.UICanvas.transform, true);
		}
		else if(SkillBarItem == true && SkillBarPanel.InvSkillBar.SkillSlot[SlotID].IsTaken == true && SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseTimer == 0)
		{
			//Drag settings for this specific item:
			SkillBarPanel.DraggingItem = true;
			SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Dragged = true;
			
			SkillBarPanel.DragSlot.SetActive(true);
			SkillBarPanel.DragSlot.GetComponent<Image>().sprite = SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Item.GetComponent<Item>().Icon;
			
			SkillBarPanel.DragID = SlotID;
			SkillBarPanel.DragTarget = -1;
			
			SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Icon.color = new Color(1.0f,1.0f,1.0f,0.0f);
			SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Icon.transform.Find("Text").GetComponent<Text>().text = "";

			SkillBarPanel.Panel.transform.SetParent(InvUI.PanelDragPos.transform, true);
			SkillBarPanel.Panel.transform.SetParent(SkillBarPanel.UICanvas.transform, true);
		}
	}
	
	public void DisableItemDrag () //Disable item drag when the player releases the mouse.
	{
		if(InvContainer.DraggingItem == true)
		{
			InvContainer.DraggingItem = false;
			InvContainer.ActiveContainer.Slots[SlotID].Dragged = false;
			InvContainer.DragSlot.transform.SetParent(InvContainer.Panel.transform,true);
			InvContainer.InvOpen = false;
			InvContainer.DragSlot.SetActive(false);

			HoverScript.HoverActive = false;
			if(HoverScript.ItemInfo.gameObject.active == true) HoverScript.ItemInfo.gameObject.SetActive(false);
			
			InvContainer.ChangeItemPos();
			InvContainer.RefreshItems();
			InvUI.RefreshItems();
		}
		else if(InvUI.DraggingItem == true)
		{
			InvUI.DraggingItem = false;
			InvUI.InvManager.Slots[SlotID].Dragged = false;
			InvUI.DragSlot.SetActive(false);

			HoverScript.HoverActive = false;
			if(HoverScript.ItemInfo.gameObject.active == true) HoverScript.ItemInfo.gameObject.SetActive(false);
			
			InvUI.ChangeItemPos();
			InvUI.RefreshItems();
		}
		else if(EquipmentPanel.DraggingItem == true)
		{
			EquipmentPanel.DraggingItem = false;
			EquipmentPanel.InvEquipment.EquipmentSlots[SlotID].Dragged = false;
			
			EquipmentPanel.DragSlot.transform.SetParent(EquipmentPanel.Panel.transform,true);
			EquipmentPanel.InvOpen = false;
			EquipmentPanel.DragSlot.SetActive(false);
			
			EquipmentPanel.ChangeItemPos();
		}
		else if(SkillBarPanel.DraggingItem == true)
		{
			SkillBarPanel.DraggingItem = false;
			SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Dragged = false;
			
			SkillBarPanel.DragSlot.transform.SetParent(SkillBarPanel.Panel.transform,true);
			SkillBarPanel.InvOpen = false;
			SkillBarPanel.DragSlot.SetActive(false);
			
			SkillBarPanel.ChangeItemPos();
		}

		InvUI.Panel.transform.SetParent(InvUI.PanelDragPos.transform, true);
		InvUI.Panel.transform.SetParent(InvUI.UICanvas.transform, true);
	}
	
	public void SetDragDestination () //When the drag item enters a slot.
	{  

		if(InvUI.DraggingItem == true)
		{
			if(VendorItem == true)
			{
				if(VendorPanel.SellDragSlot == SlotID && VendorPanel.ActiveVendor != null)
				{
					InvUI.DragTarget = -10; // -10 means targeting the vendor to sell menu.
				}    
			}
			else if(ContainerItem == true)
			{
				if(InvContainer.ActiveContainer != null && InvContainer.ActiveContainer.Slots[SlotID].IsTaken == false)
				{
					InvUI.DragToContainer = true;
					InvUI.DragTarget = SlotID;
				}    
			}
			else if(CraftingItem == true)
			{
				if(SlotID < InvCraft.InvCraft.Recipes[InvCraft.InvCraft.RecipeID].ItemsNeeded.Length)
				{
					InvUI.DragToCraft = true;
					InvUI.DragTarget = SlotID;
				}    
			}
			else if(EquipmentItem == true)
			{
				InvUI.DragToEquipment = true;
				InvUI.DragTarget = SlotID;
			}
			else if(SkillBarItem == true)
			{
				InvUI.DragToSkillBar = true;
				InvUI.DragTarget = SlotID;
			}
			else if(InventoryItem == true)
			{
				InvUI.DragTarget = SlotID;
			}
		}  
		
	}
	
	public void RemoveDragDestination () //When the drag item leaves a slot.
	{
		if(InvUI.DraggingItem == true) 
		{
			InvUI.DragTarget = -1;
			InvUI.DragToEquipment = false;
			InvUI.DragToContainer = false;
			InvUI.DragToCraft = false;
			InvUI.DragToSkillBar = false;
		}    

	}
	
	//Skill bar:
	
	public void SendSkillBarClick () //When the player clicks on a skill bar item:
	{
		if(SkillBarPanel.InvSkillBar.SkillSlot[SlotID].IsTaken == true && SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseTimer == 0) SkillBarPanel.Click(SlotID);
		/*if(SkillBarPanel.InvSkillBar.SkillSlot[SlotID].IsTaken == true && SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseTimer == 0)
		{
			SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Icon.color = Color.gray;
			SkillBarPanel.InvSkillBar.RemoveFromSkillSlot(SlotID,1);
			SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseTimer = SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseCoolDown;
			SkillBarPanel.InvSkillBar.CustomEvents.OnSkillBarItemUsed(SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Item.gameObject.GetComponent<Item>().Name, SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Name);
			SkillBarPanel.InvSkillBar.SaveSkillBar(); 
		}    
		
		if(SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseTimer > 0)
		{
			SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseTimer -= Time.deltaTime;
		}
		else if(SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseTimer < 0)
		{
			SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseTimer = 0;
			if(SkillBarPanel.InvSkillBar.SkillSlot[SlotID].IsTaken == true) SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Icon.color = Color.white;
		}*/
	}

	/*public void SendSkillBarClick () //When the player clicks on a skill bar item:
	{
		if(SkillBarPanel.InvSkillBar.SkillSlot[SlotID].IsTaken == true && SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseTimer == 0)
		{
			SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Icon.color = Color.gray;
			SkillBarPanel.InvSkillBar.RemoveFromSkillSlot(SlotID,1);
			SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseTimer = SkillBarPanel.InvSkillBar.SkillSlot[SlotID].UseCoolDown;
			SkillBarPanel.InvSkillBar.CustomEvents.OnSkillBarItemUsed(SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Item.gameObject.GetComponent<Item>().Name, SkillBarPanel.InvSkillBar.SkillSlot[SlotID].Name);
			SkillBarPanel.InvSkillBar.SaveSkillBar(); 
		}
	}*/
	
	//Equipment:
	
	public void SendEquipmentClick () //When the player clicks on an equipment item:
	{
		if(EquipmentPanel.InvEquipment.EquipmentSlots[SlotID].IsTaken == true) EquipmentPanel.Click(SlotID);
	}
	
	//Vendor:
	
	public void VendorSelection () //When the player selects the item in the vendor menu:
	{
		if(VendorPanel.SelectedItem >= 0) VendorPanel.UISlot[VendorPanel.SelectedItem].Textfield.color = VendorPanel.LastTextColor;
		VendorPanel.SelectedItem = SlotID;
		if(VendorPanel.SelectedItem != VendorPanel.SellDragSlot) VendorPanel.LastTextColor = VendorPanel.UISlot[SlotID].Textfield.color;
		if(VendorPanel.SelectedItem != VendorPanel.SellDragSlot) VendorPanel.UISlot[SlotID].Textfield.color = Color.green;
	}

	public void SendVendorClick () //When the player clicks on an equipment item:
	{
		VendorPanel.Click(SlotID);
	}

	//Crafting:
	
	public void SendCraftClick () //When the player clicks on an equipment item:
	{
		if(SlotID >= InvCraft.InvCraft.Recipes[InvCraft.InvCraft.RecipeID].ItemsNeeded.Length) return;
		if(InvCraft.InvCraft.Recipes[InvCraft.InvCraft.RecipeID].ItemsNeeded[SlotID].IsTaken == true) InvCraft.Click(SlotID);
	}
	
	public void SendRecipeClick () //When the player clicks on an equipment item:
	{
		InvCraft.ChangeRecipe(SlotID);
	}

	//item Group:
	
	public void SendItemGroupClick ()
	{
		if(InvItemGroup.SelectedItem >= 0) InvItemGroup.Items[InvItemGroup.SelectedItem].Textfield.color = InvItemGroup.LastTextColor;
		InvItemGroup.SelectedItem = SlotID;
		InvItemGroup.LastTextColor = InvItemGroup.Items[SlotID].Textfield.color;
		InvItemGroup.Items[SlotID].Textfield.color = Color.green;

	}

}