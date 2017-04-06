/*

Inventory UI Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour {
	
	public GameObject UICanvas;
	[HideInInspector]
	public RectTransform UICanvasTrans;
	
	public GameObject Panel; //The inventory's backdrop.
	[HideInInspector]
	public RectTransform PanelTrans;
	
	public KeyCode Key = KeyCode.I; //The key used to show/hide inventory GUI.

	public bool DestroyOnDragToWorld = false;
	
	public enum InvPositions {TopRightCorner = 0, LowerRightCorner = 1, TopLeftCorner = 2, LowerLeftCorner = 3, Center = 4} //Inventory's starting position
	public InvPositions InvPosition= InvPositions.TopRightCorner & InvPositions.LowerRightCorner & InvPositions.TopLeftCorner & InvPositions.LowerLeftCorner & InvPositions.Center;
	
	[HideInInspector]
	public bool  ShowInv = false; //The var that will show/hide the inv.
	[HideInInspector]
	public int InvID = 1;
	[HideInInspector]
	public static bool PickingUP= false;
	
	//Drag and drop GUI:
	public bool IsMovable = true; //Can the player change the position of the inventory GUI in game?
	[HideInInspector]
	public bool  Dragging = false;
	public GameObject PanelDragPos;
	
	public GameObject EmptySlot; //The inventory's empty slot.
	public GameObject DragSlot;
	public int SlotsPerRow= 4; 
	public int SlotsSpace= 4;
	
	//Item Drag:
	[HideInInspector]
	public bool  DraggingItem = false;
	[HideInInspector]
	public int DragID;
	[HideInInspector]
	public int DragTarget = -1;
	
	//Action:
	public GameObject ActionMenu;
	public GameObject DropMenu;
	public GameObject DropInput;
	[HideInInspector]
	public bool  Action = false;
	[HideInInspector]
	public int ActionID;
	[HideInInspector]
	public int ActionType;
	
	//Container:
	[HideInInspector]
	public bool DragToContainer = false;
	
	//Crafting:
	[HideInInspector]
	public bool DragToCraft = false;
	
	//Crafting:
	[HideInInspector]
	public bool  IsCraftingItem = false;
	[HideInInspector]
	public Rect ItemToCraftRect;
	[HideInInspector]
	public Transform ItemToCraft;
	
	//Equipment script:
	[HideInInspector]
	public bool DragToEquipment = false;
	[HideInInspector]
	public int EquippedItemSlot;
	[HideInInspector]
	public bool  IsEquippingItem = false;
	[HideInInspector]
	public Transform EquippedItem;
	
	//Skill bar vars:
	[HideInInspector]
	public bool  MovingToSkillBar = false;
	[HideInInspector]
	public int SkillSlotNumber;
	[HideInInspector]
	public Transform SkillItem;
	[HideInInspector]
	public bool DragToSkillBar = false;

	//Show riches in bag panel:
	public Image RichesInBag;
	public int RichesInBagID = -1;

	//Show the weight in bag panel:
	public Text WeightText;

	public KeyCode CloseAllPanelsKey = KeyCode.Escape;

	//Tabs:
	public bool UseTabs = false;
	public int Tabs = 3;
	public int SlotsPerTab = 10;
	[HideInInspector]
	public int CurrentTab = 0;

	//Sounds:
	public AudioClip OpenBag;
	public AudioClip CloseBag;
	
	
	//Scripts:
	[HideInInspector]
	public InventoryManager InvManager;
	[HideInInspector]
	Equipment InvEquipment;
	[HideInInspector]
	SkillBar InvSkillBar;
	[HideInInspector]
	public InventoryEvents CustomEvents;
	[HideInInspector]
	VendorUI InvVendor;
	[HideInInspector]
	CraftManager InvCraft;
	[HideInInspector]
	ItemGroupUI InvItemGroup;
	[HideInInspector]
	ContainerUI InvContainer;
	[HideInInspector]
	EquipmentUI InvEquipmentUI;
	
	void  Awake ()
	{
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		InvEquipment = FindObjectOfType(typeof(Equipment)) as Equipment; //Get the Equipment script. 
		InvSkillBar = FindObjectOfType(typeof(SkillBar)) as SkillBar; //Get the skill bar script.
		CustomEvents = FindObjectOfType(typeof(InventoryEvents)) as InventoryEvents;
		InvVendor = FindObjectOfType(typeof(VendorUI)) as VendorUI;
		InvCraft = FindObjectOfType (typeof(CraftManager)) as CraftManager;
		InvContainer = FindObjectOfType (typeof(ContainerUI)) as ContainerUI;
		InvEquipmentUI = FindObjectOfType (typeof(EquipmentUI)) as EquipmentUI;
		InvItemGroup = FindObjectOfType (typeof(ItemGroupUI)) as ItemGroupUI;
		
		//Setting up rect transforms:
		UICanvasTrans = UICanvas.GetComponent<RectTransform>();
		PanelTrans = Panel.GetComponent<RectTransform>();
		
		SetInvPosition(); // Set the default position of the inventory backdrop.
		CreateRiches ();//Set all riches slots.
		
		ActionMenu.gameObject.SetActive(false);
		DropMenu.gameObject.SetActive(false);
		
		
		ShowInv = false;
		// Panel.SetActive(false); //--hide at start
	}
	
	void  SetInvPosition (){
		// PanelTrans.anchorMax = new Vector2(0.5f,1);
		// PanelTrans.anchorMin = new Vector2(0.5f,0);
		// PanelTrans.pivot = new Vector2(0f,0f);

		// switch(InvPosition)
		// {
		// case InvPositions.TopRightCorner: //Top Right corner
		// 	PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(PanelTrans.rect.height-Screen.height/2), 0);
		// 	break;
			
		// case InvPositions.LowerRightCorner: //Lower right corner
		// 	PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(Screen.height/2), 0);
		// 	break;
			
		// case InvPositions.TopLeftCorner: //Top Left corner
		// 	PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(PanelTrans.rect.height-Screen.height/2), 0);
		// 	break;
			
		// case InvPositions.LowerLeftCorner: //Lower Left corner
		// 	PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(Screen.height/2), 0);
		// 	break; 
			
		// case InvPositions.Center: //Center
		// 	PanelTrans.localPosition = new Vector3(-Panel.GetComponent<RectTransform>().rect.width/2,-Panel.GetComponent<RectTransform>().rect.height/2, 0);
		// 	break; 
		// }
	}
	
	public void  CreateSlots (){
		
		if(InvManager == null) InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		float X= EmptySlot.GetComponent<RectTransform>().localPosition.x;
		float Y= EmptySlot.GetComponent<RectTransform>().localPosition.y;
		
		int CurrentSlotsInRow = 0;
		int SlotsCount = 0;
		for(int i = 0; i < InvManager.MaxItems; i++) //Starting a loop in the slots of the bag.
		{
			GameObject Slot = Instantiate(EmptySlot) as GameObject;
			GameObject ItemIcon = Instantiate(EmptySlot) as GameObject;
			
			Slot.transform.SetParent(Panel.transform, false);
			if(Slot.transform.Find("Text").gameObject) Destroy(Slot.transform.Find("Text").gameObject);
			
			InvManager.Slots[i].UI = Slot.gameObject;
			
			Slot.name = "Slot "+ i.ToString();
			ItemIcon.name = i.ToString();
			
			Slot.GetComponent<SlotUI>().SlotID = i;

			ItemIcon.transform.SetParent(Panel.transform, false);
			ItemIcon.transform.localScale = EmptySlot.transform.localScale/2;
			ItemIcon.GetComponent<Image>().sprite = null;
			Destroy(ItemIcon.GetComponent<EventTrigger>());
			Destroy(ItemIcon.GetComponent<SlotUI>());

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
			
			Slot.gameObject.SetActive(true);
			ItemIcon.gameObject.SetActive(true);

			Slot.transform.localScale = EmptySlot.transform.localScale;
			ItemIcon.transform.localScale = EmptySlot.transform.localScale;

			if(UseTabs == true)
			{
				if(i >= SlotsPerTab) Slot.gameObject.SetActive(false); //mod
				
				SlotsCount++;
				if(SlotsCount == SlotsPerTab)
				{
					SlotsCount = SlotsPerTab;
					X= EmptySlot.GetComponent<RectTransform>().localPosition.x;
					Y= EmptySlot.GetComponent<RectTransform>().localPosition.y;
					CurrentSlotsInRow = 0;
				}
			}
		}
		
		Destroy(EmptySlot.gameObject);
		
		DragSlot.gameObject.SetActive(false);
		DragSlot.transform.SetParent(null, true);
		DragSlot.transform.SetParent(Panel.transform, true);

		DragSlot.transform.localScale = new Vector3(1,1,1);
	}

	public void MoveRight ()
	{
		if(CurrentTab < Tabs-1)
		{
			for(int i = CurrentTab*SlotsPerTab; i < (CurrentTab+1)*SlotsPerTab; i++)
			{
				if(i < InvManager.Slots.Length) InvManager.Slots[i].UI.gameObject.SetActive(false);
			}
			CurrentTab++;
			for(int i = CurrentTab*SlotsPerTab; i < (CurrentTab+1)*SlotsPerTab; i++)
			{
				if(i < InvManager.Slots.Length) InvManager.Slots[i].UI.gameObject.SetActive(true);
			}
		}
	}
	public void MoveLeft ()
	{
		if(CurrentTab > 0)
		{
			for(int i = CurrentTab*SlotsPerTab; i < (CurrentTab+1)*SlotsPerTab; i++)
			{
				if(i < InvManager.Slots.Length) InvManager.Slots[i].UI.gameObject.SetActive(false);
			}
			CurrentTab--;
			for(int i = CurrentTab*SlotsPerTab; i < (CurrentTab+1)*SlotsPerTab; i++)
			{
				if(i < InvManager.Slots.Length)  InvManager.Slots[i].UI.gameObject.SetActive(true);
			}
		}
	}
	
	public void LoadTab (int TabID)
	{
		if (InvID == 2)MoveToBag ();
		TabID = TabID - 1;
		if(TabID >= 0 && TabID <= Tabs-1)
		{
			for(int i = CurrentTab*SlotsPerTab; i < (CurrentTab+1)*SlotsPerTab; i++)
			{
				if(i < InvManager.Slots.Length) InvManager.Slots[i].UI.gameObject.SetActive(false);
			}
			CurrentTab = TabID;
			for(int i = CurrentTab*SlotsPerTab; i < (CurrentTab+1)*SlotsPerTab; i++)
			{
				if(i < InvManager.Slots.Length) InvManager.Slots[i].UI.gameObject.SetActive(true);
			}
		}
	}
	
	void  CreateRiches (){
		for(int i = 0; i < InvManager.Riches.Length; i++) //Starting a loop in the slots of the bag.
		{
			InvManager.Riches[i].UI.name = InvManager.Riches[i].Name;
			
			InvManager.Riches[i].UI.GetComponent<Image>().sprite = InvManager.Riches[i].Icon;
			InvManager.Riches[i].UI.transform.Find("Text").GetComponent<Text>().text = InvManager.Riches[i].Name + ": " + InvManager.Riches[i].Amount.ToString();
			
			InvManager.Riches[i].UI.gameObject.SetActive(false);
		}

		if(RichesInBag != null)
		{
			if(RichesInBagID >= 0)
			{
				RichesInBag.sprite = InvManager.Riches[RichesInBagID].Icon;
				RichesInBag.transform.Find("Text").GetComponent<Text>().text = InvManager.Riches[RichesInBagID].Name + ": " + InvManager.Riches[RichesInBagID].Amount.ToString();
			}
			else
			{
				RichesInBag.gameObject.SetActive(false);
			}
		}
	}
	
	void  Update (){
		if(Input.GetKeyDown(Key)) //Check if the player pressed the I key which enables/disables the inventory.
		{
			ToggleInventory ();
		}

		if(Input.GetKeyDown(CloseAllPanelsKey))
		{
			//Close inventory panel:
			if(ShowInv == true)
			{
				ToggleInventory ();
			}
			
			//Close container panel:
			if(InvContainer.ActiveContainer != null)
			{
				InvContainer.DesactivatePanel ();
			}
			
			//Close crafting:
			if(InvCraft.InvCraft.Panel.active == true)
			{
				InvCraft.InvCraft.ToggleCrafting ();
			}
			
			//Close equipment panel:
			if(InvEquipmentUI.Panel.active == true)
			{
				InvEquipmentUI.ToggleEquipment ();
			}
			
			if(InvItemGroup.ActiveItemGroup != null)
			{
				InvItemGroup.DesactivatePanel ();
			}
			
			if(InvVendor.ActiveVendor != null)
			{
				InvVendor.DesactivatePanel ();
			}
			
		}
		
		if(Dragging == true)
		{
			Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition;
			PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
		}
		
		if(DraggingItem)
		{
			Debug.Log("");
			Vector3 TempPos3 = Input.mousePosition - UICanvasTrans.localPosition - PanelTrans.localPosition;
			DragSlot.GetComponent<RectTransform>().localPosition = new Vector3(TempPos3.x-DragSlot.GetComponent<RectTransform>().rect.width/2,TempPos3.y-DragSlot.GetComponent<RectTransform>().rect.height,0);
		    
		}
		
		if (EventSystem.current.IsPointerOverGameObject () == false && (Input.GetMouseButton (0) || Input.GetMouseButton (1))) 
		{
			if(Action == true) CloseActionMenu();
		}

		if(Input.GetKeyDown(KeyCode.Return))
		{
			if(DropMenu.active == true)
			{
				SendItem();
			}
		}
	}
	
	public void RefreshItems ()
	{
		for(int i = 0; i < InvManager.MaxItems; i++) //Starting a loop in the slots of the bag.
		{ 
				Image ItemIcon = InvManager.Slots[i].UI.transform.Find(i.ToString()).GetComponent<Image>();
				if(InvManager.Slots[i].Dragged == false) //Check if the item inside the slot is not being dragged.
				{
					if(InvManager.Slots[i].IsTaken == true) //Check if the slot is taken.
					{
						Item ItemScript = InvManager.Slots[i].Item.GetComponent<Item>(); //Getting the item script.  
						
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

		if(WeightText != null)
		{
			WeightText.text = InvManager.Weight.ToString()+"/"+InvManager.MaxWeight.ToString()+"kg";
		}
	}
	public void RefreshRiches ()
	{
		for(int i = 0; i < InvManager.Riches.Length; i++) //Starting a loop in the slots of the bag.
		{ 
			InvManager.Riches[i].UI.transform.Find("Text").GetComponent<Text>().text = InvManager.Riches[i].Name + ": " + InvManager.Riches[i].Amount.ToString();
		}

		if(RichesInBag != null)
		{
			if(RichesInBagID >= 0)
			{
				RichesInBag.sprite = InvManager.Riches[RichesInBagID].Icon;
				RichesInBag.transform.Find("Text").GetComponent<Text>().text = InvManager.Riches[RichesInBagID].Name + ": " + InvManager.Riches[RichesInBagID].Amount.ToString();
			}
			else
			{
				RichesInBag.gameObject.SetActive(false);
			}
		}
	}

	/*public void CleanInventory ()
	{
		for(int i = 0; i < InvManager.Slots.Length; i++)
		{

		}
	}*/
	
	//Close button:
	public void  ToggleInventory (){
		ShowInv = !ShowInv; //Hide the inventory.
		
		if(ShowInv == false)
		{
			Action = false;
			Panel.SetActive(false);
			if(InvManager.ReArrangeItems == true) InvManager.ArrangeItems ();
			GetComponent<AudioSource>().PlayOneShot(CloseBag);
			
			CloseActionMenu ();

			if(CustomEvents) CustomEvents.OnPlayerCloseInventory();
		} 
		else
		{
			Panel.SetActive(true);
			GetComponent<AudioSource>().PlayOneShot(OpenBag);

			if(CustomEvents) CustomEvents.OnPlayerOpenInventory();
		}   
	}
	
	//Riches:
	public void  MoveToRiches (){
		InvID = 2; //Display the player's money with all the different currencies.
		CloseActionMenu ();
		
		for(int i = 0; i < InvManager.Riches.Length; i++) //Starting a loop in the riches:
		{
			InvManager.Riches[i].UI.gameObject.SetActive(true);
		}
		for(int j = 0; j < InvManager.MaxItems; j++) //Starting a loop in the slots of the bag.
		{
			InvManager.Slots[j].UI.gameObject.SetActive(false);
		}

		if(RichesInBag != null)
		{
			RichesInBag.gameObject.SetActive(false);
		}

		if(WeightText != null)
		{
			WeightText.gameObject.SetActive(false);
		}
	}
	
	//Bag:
	public void  MoveToBag (){
		InvID = 1; //Display the player's money with all the different currencies.
		CloseActionMenu ();
		
		for(int i = 0; i < InvManager.Riches.Length; i++) //Starting a loop in the riches:
		{
			InvManager.Riches[i].UI.gameObject.SetActive(false);
		}
		if(UseTabs == false)
		{
			for(int j = 0; j < InvManager.MaxItems; j++) //Starting a loop in the slots of the bag.
			{
				InvManager.Slots[j].UI.gameObject.SetActive(true);
			}
		}
		else
		{
			LoadTab (CurrentTab+1);
		}

		if(RichesInBag != null)
		{
			if(RichesInBagID >= 0)
			{
				RichesInBag.gameObject.SetActive(true);
			}
		}

		if(WeightText != null)
		{
			WeightText.gameObject.SetActive(true);
		}
	}
	
	//Dragging and dropping inventory window:
	public void  DragStarted (){
		if(IsMovable == true && Dragging == false)
		{
			Dragging = true;
			PanelDragPos.gameObject.SetActive(true);
			Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition;
			PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
			Panel.transform.SetParent(PanelDragPos.transform, true);
			CloseActionMenu();
		}
	}
	
	public void  DragEnded (){
		if(IsMovable == true)
		{
			Dragging = false;
			PanelDragPos.gameObject.SetActive(false);
			Panel.transform.SetParent(UICanvas.transform, true);
			
			ActionMenu.transform.SetParent(Panel.transform, true);
			DropMenu.transform.SetParent(Panel.transform, true);
			HoverItem ItemInfo = FindObjectOfType(typeof(HoverItem)) as HoverItem;
			ItemInfo.ItemInfo.transform.SetParent(Panel.transform, true);

			ActionMenu.transform.SetParent(UICanvas.transform, true);
			DropMenu.transform.SetParent(UICanvas.transform, true);
			ItemInfo.ItemInfo.transform.SetParent(UICanvas.transform, true);
			
		}
	}
	
	//Action menu:
	
	public void  CloseActionMenu (){
		Action = false;
		ActionMenu.gameObject.SetActive(false);
		DropMenu.gameObject.SetActive(false);
	}
	
	public void  SetActionType ( int Type  ){
		Item ItemScript = InvManager.Slots[ActionID].Item.GetComponent<Item>(); //Getting the item script.
		
		if(Type == 1) //Use/Equip
		{
			if(ItemScript.EquipmentSlot != "")
			{
				UseItem(InvManager.Slots[ActionID].Item, -1);
			}
			else if(ItemScript.SkillSlot != "")
			{
				MoveToSkill(InvManager.Slots[ActionID].Item, -1);
			}
			else if(ItemScript.Usable == true)
			{
				CustomEvents.OnPlayerUseInventoryItem(ItemScript);
				if(ItemScript.DestroyOnUse == true) InvManager.RemoveItem(InvManager.Slots[ActionID].Item, 1);
			}
		}
		
		if(Type == 2) //Drop Item:
		{
			if(ItemScript.Amount == 1) //If we have only 1 amount of that item, then we automaticly remove it.
			{
				CustomEvents.OnPlayerDropItem(ItemScript);
				InvManager.RemoveItem(InvManager.Slots[ActionID].Item, 1);
				CloseActionMenu ();
			}
			else if(ItemScript.Amount > 1) //If we have more than 1 of that item, then we let the player decide how many of this item to drop.
			{
				ActionMenu.gameObject.SetActive(false);
				DropMenu.transform.SetParent(UICanvas.transform, true);
				DropMenu.gameObject.SetActive(true);
				DropMenu.GetComponent<RectTransform>().localPosition = ActionMenu.GetComponent<RectTransform>().localPosition;
				DropMenu.transform.SetParent(Panel.transform, true);
				ActionType = 1;
			} 
		}
		
		if(Type == 3 && CanItemBeSold(ItemScript.Name) == true) //Sell item menu:
		{
			if(ItemScript.Amount == 1) //If we have only 1 amount of that item, then we automaticly remove it.
			{
				Transform ItemToSell = InvManager.FreeItem(InvManager.Slots[ActionID].Item, 1);
				SendItemToSell(ItemToSell);
			}
			else if(ItemScript.Amount > 1) //If we have more than 1 of that item, then we let the player decide how many of this item to drop.
			{
				DropMenu.transform.SetParent(UICanvas.transform, true);
				DropMenu.gameObject.SetActive(true);
				Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition;
				DropMenu.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x,TempPos.y,0);
				DropMenu.transform.SetParent(InvVendor.Panel.transform, true);
				ActionType = 2;
			} 
		}
	}
	
	public bool CanItemBeSold(string Name)
	{
		if(InvVendor.ActiveVendor.AcceptedItems.Length > 0)
		{
			for(int i = 0; i < InvVendor.ActiveVendor.AcceptedItems.Length; i++)
			{
				if(InvVendor.ActiveVendor.AcceptedItems[i] == Name)
				{
					return true;
				}
			}
			
			if(InvManager.MsgUI != null) InvManager.MsgUI.SendMsg("Vendor didn't accept this item!");
			return false;
		}
		else
		{
			if(InvManager.MsgUI != null) InvManager.MsgUI.SendMsg("Vendor didn't accept this item!");
			return false;
		}    
	}
	
	public void  SendItem (){
		int i = 0; //Amount to drop.
		int.TryParse(DropInput.gameObject.GetComponent<Text>().text, out i); //Converting the string to an int here.
		
		if(ActionType == 1)
		{
			//Drop the item:
			CustomEvents.OnPlayerDropItem(InvManager.Slots[ActionID].Item.GetComponent<Item>());
			InvManager.RemoveItem(InvManager.Slots[ActionID].Item, i);
			DropInput.gameObject.GetComponent<Text>().text = "";
			CloseActionMenu ();
		}
		if(ActionType == 2)
		{
			//Send item to vendor:
			Transform ItemToSell = InvManager.FreeItem(InvManager.Slots[ActionID].Item, i);
			SendItemToSell(ItemToSell);
			CloseActionMenu ();
		}
	}
	
	void SendItemToSell (Transform ItemToSell)
	{
		//Add this item to the items to sell list:
		List<Transform> TempContent = new List<Transform>(InvVendor.ActiveVendor.ItemsToSell);
		TempContent.Add(ItemToSell);
		InvVendor.ActiveVendor.ItemsToSell = TempContent.ToArray();
		
		//Changing the settings of the item and parenting it to the vendor object.
		if(ItemToSell.GetComponent<Collider>() !=null)
		{
			ItemToSell.transform.GetComponent<Collider>().isTrigger = true;
		}
		ItemToSell.transform.GetComponent<Renderer>().enabled = false;
		ItemToSell.transform.parent = InvVendor.ActiveVendor.gameObject.transform;
		ItemToSell.transform.localPosition  = Vector3.zero;
		
		ItemToSell = null;
		
		InvVendor.RefreshSellItems();
	}
	
	public void  EnablePickingUP (){
		PickingUP = true;
	}
	public void  DisablePickingUP (){
		PickingUP = false;
	}
	
	public void  ChangeItemPos (){
		if(DragTarget >= 0 && DragToContainer == false && DragToCraft == false && DragToEquipment == false && DragToSkillBar == false)
		{
			if(InvID == 1)
			{
				if(InvManager.Slots[DragTarget].Dragged == false) //Check if the item inside the slot is not being dragged.
				{
					if(InvManager.Slots[DragTarget].IsTaken == false) //Check if the slot is taken.
					{
						InvManager.MoveItem(DragID, DragTarget); //We move this item to the target slot.
						InvManager.Slots[DragID].Dragged = false; //Stop dragging the item.
						DraggingItem = false;
					}
				}    
			}
		}
		else if(DragTarget >= 0 && DragToContainer == true && DragToCraft == false) //Move to container menu:
		{
			AddToContainer(InvManager.Slots[DragID].Item, DragTarget);
			InvManager.Slots[DragID].Dragged = false; //Stop dragging the item.
			DraggingItem = false;
			DragToContainer = false;
		}
		else if(DragTarget >= 0 && DragToContainer == false && DragToCraft == true) //Move to container menu:
		{
			AddToCraft(InvManager.Slots[DragID].Item, DragTarget);
			InvManager.Slots[DragID].Dragged = false; //Stop dragging the item.
			DraggingItem = false;
			DragToCraft = false;
		}
		else if(DragTarget >= 0 && DragToEquipment == true) //Move to container menu:
		{
			UseItem(InvManager.Slots[DragID].Item, DragTarget);
			InvManager.Slots[DragID].Dragged = false; //Stop dragging the item.
			DragToEquipment = false;
		}
		else if(DragTarget >= 0 && DragToSkillBar == true) //Move to container menu:
		{
			MoveToSkill(InvManager.Slots[DragID].Item, DragTarget);
			InvManager.Slots[DragID].Dragged = false; //Stop dragging the item.
			DragToSkillBar = false;
		}
		else if(DragTarget == -10)
		{
			ActionID = DragID;
			SetActionType(3);
			DragTarget = -1;
		}
		else if (EventSystem.current.IsPointerOverGameObject () == false && DestroyOnDragToWorld == true) 
		{
			CustomEvents.OnPlayerUseInventoryItem(InvManager.Slots[DragID].Item.GetComponent<Item>());
			InvManager.RemoveItem(InvManager.Slots[DragID].Item, InvManager.Slots[DragID].Item.GetComponent<Item>().Amount);
			InvManager.Slots[DragID].Dragged = false;
			DraggingItem = false;	
		}
	}
	
	void AddToContainer(Transform Item ,   int SlotID  )
	{
		Item ItemScript = Item.GetComponent<Item>(); //Getting the item script.
		
		Transform ItemObj = InvManager.FreeItem(Item, ItemScript.Amount); //Free this item from the inventory content. 
		
		//Changing the settings of the item and parenting it to the inventory manager again.
		if(ItemObj.GetComponent<Collider>() !=null)
		{
			ItemObj.transform.GetComponent<Collider>().isTrigger = true;
		}
		ItemObj.transform.GetComponent<Renderer>().enabled = false;
		ItemObj.transform.parent = gameObject.transform;
		ItemObj.transform.localPosition  = Vector3.zero;
		ItemObj.gameObject.SetActive(false);
		
		FindObjectOfType<ContainerUI>().ActiveContainer.AddItem(ItemObj, SlotID);
	}
	
	public void  UseItem ( Transform Item ,   int n  ){
		Item EquipmentItemScript= Item.GetComponent<Item>(); //Getting the item script.
		if(n == -1) //If we didn't define the target equipment slot.
		{
			for(int t = 0; t < InvEquipment.EquipmentSlots.Length; t++) //Loop through all the equipment slots to find one.
			{
				if(InvEquipment.EquipmentSlots[t].Name == EquipmentItemScript.EquipmentSlot)
				{
					n = t;
				}
			}
		}
		if(n != -1)
		{
			if(InvEquipment.EquipmentSlots[n].Name == EquipmentItemScript.EquipmentSlot) //Check if the item equipment slots matches the current equipment slot.
			{
				/*InvManager.RemoveItem(Item, 1); //Remove this item from the inventory content. 
				CustomEvents.OnPlayerEquipItem(EquipmentItemScript.Name, EquipmentItemScript.EquipmentSlot);*/
				if(InvEquipment.EquipmentSlots[n].IsTaken == false) //Check if the slot is not taken.
				{
					
					EquippedItem = InvManager.FreeItem(Item, 1); //Free this item from the inventory content. 
					CustomEvents.OnPlayerEquipItem(EquipmentItemScript, InvEquipment.EquipmentSlots[n].GroupID);
					//Add the item to the equipment slot
					IsEquippingItem = true;
					EquippedItemSlot = n;
					
					//Changing the settings of the item and parenting it to the inventory manager again.
					if(EquippedItem.GetComponent<Collider>() !=null)
					{
						EquippedItem.transform.GetComponent<Collider>().isTrigger = true;
					}
					EquippedItem.transform.GetComponent<Renderer>().enabled = false;
					EquippedItem.transform.parent = gameObject.transform;
					EquippedItem.transform.localPosition  = Vector3.zero;
					EquippedItem.gameObject.SetActive(false);
					return;
				}
				else if(InvEquipment.EquipmentSlots[n].IsTaken == true) //Check if the slot is taken.
				{
					Item TempScript= InvEquipment.EquipmentSlots[n].Item.GetComponent<Item>();
					if(InvManager.Exist(InvEquipment.EquipmentSlots[n].Item) && TempScript.Name == EquipmentItemScript.Name) //If we already have that item in the inventory.
					{
						//No action.
						return;  
					}
					else if(InvManager.Items < InvManager.MaxItems) //If we still have room for a new item:  
					{
						InvManager.AddItem(InvEquipment.EquipmentSlots[n].Item); //Remove this item from the equipment slot and add it to the inventory.
						CustomEvents.OnPlayerUnEquipItem(TempScript, InvEquipment.EquipmentSlots[n].GroupID);
						InvEquipment.DesactivateEqObj (TempScript.Name);

						for(int x = 0; x < TempScript.Attributes.Length; x++)
						{
							for(int j = 0; j < InvEquipment.Attributes.Length; j++)
							{
								if(InvEquipment.Attributes[j].Name == TempScript.Attributes[x].Name)
								{
									InvEquipment.Attributes[j].Value -= TempScript.Attributes[x].Value;
								}
							}
						}

						EquippedItem = InvManager.FreeItem(Item, 1); //Free the item from the inventory content. 
						CustomEvents.OnPlayerEquipItem(EquipmentItemScript, InvEquipment.EquipmentSlots[n].GroupID);
						//Add the item to the equipment slot
						IsEquippingItem = true;
						EquippedItemSlot = n;
						
						//Changing the settings of the item and parenting it to the inventory manager again.
						if(EquippedItem.GetComponent<Collider>() !=null)
						{
							EquippedItem.transform.GetComponent<Collider>().isTrigger = true;
						}
						EquippedItem.transform.GetComponent<Renderer>().enabled = false;
						EquippedItem.transform.parent = gameObject.transform;
						EquippedItem.transform.localPosition  = Vector3.zero;
						EquippedItem.gameObject.SetActive(false);
						return;
					}
					else
					{
						Debug.Log("No room for a new item!");
						return;
					}
				}
			}
		}    
		
	}
	
	
	public void  MoveToSkill ( Transform Item ,   int n  ){
		Item ItemScript= Item.GetComponent<Item>(); //Getting the item script.
		if(n == -1) //If we didn't define the target skill bar slot
		{
			for(int t = 0; t < InvSkillBar.SkillSlot.Length; t++) //Loop through all the skill bar slots to find one.
			{
				if(InvSkillBar.SkillSlot[t].Name == ItemScript.SkillSlot)
				{
					n = t;
				}
			}
		}
		if(n != -1)
		{
			if(ItemScript.SkillSlot == "")
			{
				return;
			}
			if(InvSkillBar.SkillSlot[n].Name == ItemScript.SkillSlot) //Check if the item skill bar matches with the item.
			{
				if(InvSkillBar.SkillSlot[n].IsTaken == false) //Check if the slot is not taken.
				{
					if(InvSkillBar.SkillSlot[n].MultipleItems == true) //If we accept multiple items on this slot, free these items.
					{
						SkillItem = InvManager.FreeItem(Item, ItemScript.Amount);
					}
					else
					{
						SkillItem = InvManager.FreeItem(Item, 1); //Free the item from the inventory content. 
					}
					CustomEvents.OnSkillBarAdd(ItemScript);
					//Add the item to the skill bar slots
					MovingToSkillBar = true;
					SkillSlotNumber = n;
					
					//Changing the settings of the item and parenting it to the inventory manager again.
					if(SkillItem.GetComponent<Collider>() !=null)
					{
						SkillItem.transform.GetComponent<Collider>().isTrigger = true;
					}
					SkillItem.transform.GetComponent<Renderer>().enabled = false;
					SkillItem.transform.parent = gameObject.transform;
					SkillItem.transform.localPosition  = Vector3.zero;
					SkillItem.gameObject.SetActive(false);
					return;
				}
				else if(InvSkillBar.SkillSlot[n].IsTaken == true) //Check if the slot is taken.
				{
					Item TempScript= InvSkillBar.SkillSlot[n].Item.GetComponent<Item>();
					if(ItemScript.Name == TempScript.Name && InvSkillBar.SkillSlot[n].MultipleItems == true)
					{
						if(TempScript.Amount < TempScript.MaxAmount)
						{
							int AmountToAdd  = TempScript.MaxAmount - TempScript.Amount;
							if(ItemScript.Amount <= AmountToAdd)
							{
								SkillItem = InvManager.FreeItem(Item, ItemScript.Amount);
								Destroy(SkillItem.gameObject);
								TempScript.Amount += ItemScript.Amount;
							}
							else
							{
								SkillItem = InvManager.FreeItem(Item, AmountToAdd);
								Destroy(SkillItem.gameObject);
								TempScript.Amount += AmountToAdd;
							}
							
							if(InvSkillBar.SkillSlot[n].Icon.transform.Find("Text") != null && TempScript.IsStackable == true) 
							{
								InvSkillBar.SkillSlot[n].Icon.transform.Find("Text").gameObject.SetActive(true);
								InvSkillBar.SkillSlot[n].Icon.transform.Find("Text").GetComponent<Text>().text = TempScript.Amount.ToString();
							}
							else if(InvSkillBar.SkillSlot[n].Icon.transform.Find("Text") != null)
							{
								InvSkillBar.SkillSlot[n].Icon.transform.Find("Text").gameObject.SetActive(false);
							}
						}
					}
					/*if(InvManager.Exist(InvSkillBar.SkillSlot[n].Item) && TempScript.Name == ItemScript.Name) //If we already have that item in the inventory.
				{
					//No action.
					return;  
				}*/
					else if(InvManager.Items < InvManager.MaxItems) //If we still have room for a new item:  
					{
						InvManager.AddItem(InvSkillBar.SkillSlot[n].Item); //Remove this item from the equipment slot and add it to the inventory.
						if(InvSkillBar.SkillSlot[n].MultipleItems == true) //If we accept multiple items on this slot, free these items.
						{
							SkillItem = InvManager.FreeItem(Item, ItemScript.Amount);
						}
						else
						{
							SkillItem = InvManager.FreeItem(Item, 1); //Free the item from the inventory content. 
						}
						CustomEvents.OnSkillBarAdd(ItemScript);
						//Add the item to the equipment slot
						MovingToSkillBar = true;
						SkillSlotNumber = n;
						
						//Changing the settings of the item and parenting it to the inventory manager again.
						if(SkillItem.GetComponent<Collider>() !=null)
						{
							SkillItem.transform.GetComponent<Collider>().isTrigger = true;
						}
						SkillItem.transform.GetComponent<Renderer>().enabled = false;
						SkillItem.transform.parent = gameObject.transform;
						SkillItem.transform.localPosition  = Vector3.zero;
						SkillItem.gameObject.SetActive(false);
						return;
					}
					else
					{
						Debug.Log("No room for a new item!");
						return;
					}
				}
			}
		}    
		
	}
	
	void  AddToCraft ( Transform Item ,   int SlotID  ){
		Item ItemScript = Item.GetComponent<Item>(); //Getting the item script.
		
		if(InvCraft.Recipes[InvCraft.RecipeID].ItemsAmount < InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded.Length && InvCraft.Recipes[InvCraft.RecipeID].ItemReady == false)
		{
			if(ItemScript.Name == InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[SlotID].Name) //Check if this item is needed in this recipe.
			{
				if(InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[SlotID].IsTaken == false) //Check if this slot isn't already taken.
				{
					if(ItemScript.Amount >= InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[SlotID].Quantity) //If we have enough amount of this item.
					{
						Transform TempItem = InvManager.FreeItem(Item, InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[SlotID].Quantity); //Free this item from the inventory content. 
						
						//Changing the settings of the item and parenting it to the inventory manager again.
						if(TempItem.GetComponent<Collider>() !=null)
						{
							TempItem.transform.GetComponent<Collider>().isTrigger = true;
						}
						TempItem.transform.GetComponent<Renderer>().enabled = false;
						TempItem.transform.parent = gameObject.transform;
						TempItem.transform.localPosition  = Vector3.zero;
						TempItem.gameObject.SetActive(false);
						
						//Put the item in this crafting slot:
						InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[SlotID].Item = TempItem;
						InvCraft.Recipes[InvCraft.RecipeID].ItemsNeeded[SlotID].IsTaken = true;
						InvCraft.Recipes[InvCraft.RecipeID].ItemsAmount++;
						
						FindObjectOfType<CraftUI>().RefreshItems();
					}
				}
			}
		}
	}
	
}
