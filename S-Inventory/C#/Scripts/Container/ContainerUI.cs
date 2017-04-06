/*

Container UI script created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ContainerUI : MonoBehaviour {
	
	public GameObject UICanvas;
	[HideInInspector]
	public RectTransform UICanvasTrans;
	
	public GameObject Panel; //The inventory's backdrop.
	[HideInInspector]
	public RectTransform PanelTrans;

	public bool EnableToggleInfo = true;
	public Text ToggleInfo;
	
	public enum ContainerPositions {TopRightCorner = 0, LowerRightCorner = 1, TopLeftCorner = 2, LowerLeftCorner = 3, Center = 4} //Inventory's starting position
	public ContainerPositions ContainerPosition= ContainerPositions.TopRightCorner & ContainerPositions.LowerRightCorner & ContainerPositions.TopLeftCorner & ContainerPositions.LowerLeftCorner & ContainerPositions.Center;
	
	//Scripts:
	[HideInInspector]
	public InventoryManager InvManager;
	
	[HideInInspector]
	public Container ActiveContainer; //The current active vendor.
	
	public GameObject EmptySlot; //The inventory's empty slot.
	public GameObject DragSlot;
	[HideInInspector]
	public bool InvOpen = false;
	public int MaxContainerSlots = 40;
	public int SlotsPerRow= 4; 
	public int SlotsSpace= 4;
	
	//Item Drag:
	[HideInInspector]
	public bool  DraggingItem = false;
	[HideInInspector]
	public int DragID;
	[HideInInspector]
	public int DragTarget = -1;
	[HideInInspector]
	public bool  DragToInventory = false;
	
	[HideInInspector]
	public GameObject[] ContainerSlots;

	public bool IsMovable = true; //Can the player change the position of the UI window?
	[HideInInspector]
	public bool  Dragging = false;
	
	void  Awake (){
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		
		ActiveContainer = null; //Set the active vendor to null on start.
		
		//Setting up rect transforms for the vendor's UI panel and main canvas object:
		UICanvasTrans = UICanvas.GetComponent<RectTransform>();
		PanelTrans = Panel.GetComponent<RectTransform>();
		
		SetContainerPosition ();
		
		Panel.SetActive(false); //Disabling the panel.
		
		CreateSlots ();
		DragSlot.gameObject.SetActive(false);
		DragSlot.transform.SetParent(null, true);
		DragSlot.transform.SetParent(Panel.transform, true);
		DragSlot.transform.localScale = new Vector3(1,1,1);
	}
	
	void  Update (){
		if(DraggingItem)
		{
			Vector3 TempPos3 = Input.mousePosition - UICanvasTrans.localPosition - PanelTrans.localPosition;
			DragSlot.GetComponent<RectTransform>().localPosition = new Vector3(TempPos3.x-DragSlot.GetComponent<RectTransform>().rect.width/2,TempPos3.y-DragSlot.GetComponent<RectTransform>().rect.height,0);
		}

		if(Dragging == true)
		{
			Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition;
			InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
		}
	
		
		//RefreshItems();
	}
	
	void  SetContainerPosition (){
		PanelTrans.anchorMax = new Vector2(0.5f,0.5f);
		PanelTrans.anchorMin = new Vector2(0.5f,0.5f);
		PanelTrans.pivot = new Vector2(0f,0f);

		switch(ContainerPosition)
		{
		case ContainerPositions.TopRightCorner: //Top Right corner
			PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(PanelTrans.rect.height-Screen.height/2), 0);
			break;
			
		case ContainerPositions.LowerRightCorner: //Lower right corner
			PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(Screen.height/2), 0);
			break;
			
		case ContainerPositions.TopLeftCorner: //Top Left corner
			PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(PanelTrans.rect.height-Screen.height/2), 0);
			break;
			
		case ContainerPositions.LowerLeftCorner: //Lower Left corner
			PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(Screen.height/2), 0);
			break; 
			
		case ContainerPositions.Center: //Center
			PanelTrans.localPosition = new Vector3(-Panel.GetComponent<RectTransform>().rect.width/2,-Panel.GetComponent<RectTransform>().rect.height/2, 0);
			break;  
		}
	}
	
	public void  DesactivatePanel (){
		if(DraggingItem == true)
		{
			ActiveContainer.Slots[DragID].UI.gameObject.GetComponent<SlotUI>().DisableItemDrag ();
		}

		ActiveContainer.SaveItems();
		ActiveContainer.TriggerContainer ();
		
		//Desactivate the panel:
		ActiveContainer = null;
	}
	
	//Toggle info:
	
	public void ShowToggleInfo () //Showing the info to interact with the vendor here:
	{
		if(ToggleInfo != null && ActiveContainer == null && EnableToggleInfo == true) //Showing it only if the UI isn't active.
		{
			ToggleInfo.gameObject.SetActive(true);
			//Display the vendor interaction info for both control types:
			if(InvManager.PickupType == InventoryManager.PickupTypes.Mouse) //If we are using the mouse to pick up items:
			{
				ToggleInfo.text =  "Press the left mouse button to open the container.";
			}
			if(InvManager.PickupType == InventoryManager.PickupTypes.Keyboard) //If we are using the keyboard to pick up items.
			{
				ToggleInfo.text = "Press " + InvManager.ActionKey.ToString() + " to open the container.";
			}
			if(ToggleInfo.gameObject.transform.parent != null)
			{
				ToggleInfo.gameObject.transform.parent.gameObject.SetActive(true);
			}
		}
	}
	
	public void  ChangeItemPos (){
		if(DragTarget >= 0 && DragToInventory == false)
		{
			if(ActiveContainer.Slots[DragTarget].Dragged == false) //Check if the item inside the slot is not being dragged.
			{
				if(ActiveContainer.Slots[DragTarget].IsTaken == false) //Check if the slot is taken.
				{
					ActiveContainer.MoveItem(DragID, DragTarget); //We move this item to the target slot.
					ActiveContainer.Slots[DragID].Dragged = false; //Stop dragging the item.
					DraggingItem = false;
				}
			}
		}
		else if(DragTarget >= 0 && DragToInventory == true && InvManager.Slots[DragTarget].IsTaken == false) //Move to container menu:
		{
			ActiveContainer.MoveToInventory(ActiveContainer.Slots[DragID].Item, ActiveContainer.Slots[DragID].Item.GetComponent<Item>().Amount,DragTarget);
			ActiveContainer.Slots[DragID].Dragged = false; //Stop dragging the item.
			DraggingItem = false;
			DragToInventory = false;
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
	
	//Items:
	public void  ChangeSlots (){
		for(int i = 0; i < ContainerSlots.Length; i++) //Starting a loop in the slots of the bag.
		{
			if(i < ActiveContainer.MaxItems)
			{
				ContainerSlots[i].gameObject.SetActive(true);
				ActiveContainer.Slots[i].UI = ContainerSlots[i].gameObject;
			}
		}
		
		Destroy(EmptySlot.gameObject);
	}
	
	public void  CreateSlots (){
		float X= EmptySlot.GetComponent<RectTransform>().localPosition.x;
		float Y= EmptySlot.GetComponent<RectTransform>().localPosition.y;
		
		int CurrentSlotsInRow = 0;
		for(int i = 0; i < MaxContainerSlots; i++) //Starting a loop in the slots of the bag.
		{
			GameObject Slot = Instantiate(EmptySlot) as GameObject;
			GameObject ItemIcon = Instantiate(EmptySlot) as GameObject;
			
			Slot.transform.SetParent(Panel.transform, false);
			if(Slot.transform.Find("Text").gameObject) Destroy(Slot.transform.Find("Text").gameObject);
			
			ItemIcon.transform.SetParent(Panel.transform, false);
			ItemIcon.GetComponent<Image>().sprite = null;
			Destroy(ItemIcon.GetComponent<EventTrigger>());
			Destroy(ItemIcon.GetComponent<SlotUI>());
			
			Slot.name = "Slot "+ i.ToString();
			ItemIcon.name = i.ToString();
			
			Slot.GetComponent<SlotUI>().SlotID = i;
			
			Slot.GetComponent<RectTransform>().localPosition = new Vector3(X,Y,0);
			ItemIcon.GetComponent<RectTransform>().localPosition = new Vector3(X,Y,0);
			
			ItemIcon.transform.SetParent(Slot.transform, false);
			ItemIcon.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
			
			X += EmptySlot.GetComponent<RectTransform>().rect.width+SlotsSpace;
			CurrentSlotsInRow++;
			
			if(CurrentSlotsInRow == SlotsPerRow)
			{
				X = EmptySlot.GetComponent<RectTransform>().localPosition.x;
				Y -= EmptySlot.GetComponent<RectTransform>().rect.height+SlotsSpace;
				
				CurrentSlotsInRow = 0;
			}
			
			Slot.gameObject.SetActive(false);
			ItemIcon.gameObject.SetActive(true);

			List<GameObject> TempContent = new List<GameObject>(ContainerSlots);
			TempContent.Add(Slot);
			ContainerSlots = TempContent.ToArray();

			Slot.transform.localScale = EmptySlot.transform.localScale;
			ItemIcon.transform.localScale = EmptySlot.transform.localScale;
		}
		
		Destroy(EmptySlot.gameObject);
	}
	
	
	public void  RefreshItems (){
		for(int i = 0; i < ActiveContainer.MaxItems; i++) //Starting a loop in the slots of the bag.
		{ 
			Image ItemIcon = ActiveContainer.Slots[i].UI.transform.Find(i.ToString()).GetComponent<Image>();
			if(ActiveContainer.Slots[i].Dragged == false) //Check if the item inside the slot is not being dragged.
			{
				if(ActiveContainer.Slots[i].IsTaken == true) //Check if the slot is taken.
				{
					Item ItemScript = ActiveContainer.Slots[i].Item.GetComponent<Item>(); //Getting the item script.  
					
					ItemIcon.gameObject.SetActive(true);
					ItemIcon.sprite = ItemScript.Icon;
					if(ItemScript.IsStackable == true && ItemIcon.transform.Find("Text") != null)
					{
						ItemIcon.transform.Find("Text").gameObject.SetActive(true);
						ItemIcon.transform.Find("Text").GetComponent<Text>().text = ItemScript.Amount.ToString();
					}
					else if(ItemIcon.transform.Find("Text") != null)
					{
						ItemIcon.transform.Find("Text").gameObject.SetActive(false);
					}
				}
				else
				{
					ItemIcon.sprite = null;
					ItemIcon.gameObject.SetActive(false);
				}
			} 
			else
			{
				ItemIcon.gameObject.SetActive(false);
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