/*

Equipment UI Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour 
{
	public GameObject UICanvas;
	[HideInInspector]
	public RectTransform UICanvasTrans;
	
	public GameObject Panel; //The equipment's backdrop.
	[HideInInspector]
	public RectTransform PanelTrans;

	public Text AttributesInfo;
	
	public enum EqPositions {TopRightCorner = 0, LowerRightCorner = 1, TopLeftCorner = 2, LowerLeftCorner = 3, Center = 4} //Inventory's starting position
	public EqPositions EqPosition= EqPositions.TopRightCorner & EqPositions.LowerRightCorner & EqPositions.TopLeftCorner & EqPositions.LowerLeftCorner & EqPositions.Center;
	
	//Scripts:
	[HideInInspector]
	public InventoryManager InvManager;
	[HideInInspector]
	public Equipment InvEquipment;
	[HideInInspector]
	public InventoryEvents CustomEvents;

	//Item Drag:
	public GameObject DragSlot;
	[HideInInspector]
	public bool DraggingItem = false;
	[HideInInspector]
	public int DragID;
	[HideInInspector]
	public int DragTarget = -1;
	[HideInInspector]
	public bool InvOpen = false;
	
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
	
	void  Start (){
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		InvEquipment = FindObjectOfType(typeof(Equipment)) as Equipment; //Get the Equipment script. 
		CustomEvents = FindObjectOfType(typeof(InventoryEvents)) as InventoryEvents;
		
		//Setting up rect transforms:
		UICanvasTrans = UICanvas.GetComponent<RectTransform>();
		PanelTrans = Panel.GetComponent<RectTransform>();
		
		SetEqPosition(); // Set the default position of the inventory backdrop.

		Panel.SetActive(false);

		RefreshAttributes ();
	}
	
	void  SetEqPosition (){
		PanelTrans.anchorMax = new Vector2(0.5f,0.5f);
		PanelTrans.anchorMin = new Vector2(0.5f,0.5f);
		PanelTrans.pivot = new Vector2(0f,0f);

		switch(EqPosition)
		{
		case EqPositions.TopRightCorner: //Top Right corner
			PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(PanelTrans.rect.height-Screen.height/2), 0);
			break;
			
		case EqPositions.LowerRightCorner: //Lower right corner
			PanelTrans.localPosition = new Vector3(Screen.width/2-PanelTrans.rect.width,-(Screen.height/2), 0);
			break;
			
		case EqPositions.TopLeftCorner: //Top Left corner
			PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(PanelTrans.rect.height-Screen.height/2), 0);
			break;
			
		case EqPositions.LowerLeftCorner: //Lower Left corner
			PanelTrans.localPosition = new Vector3(-(Screen.width/2),-(Screen.height/2), 0);
			break; 
			
		case EqPositions.Center: //Center
			PanelTrans.localPosition = new Vector3(-Panel.GetComponent<RectTransform>().rect.width/2,-Panel.GetComponent<RectTransform>().rect.height/2, 0);
			break;   
		}
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

		if(DraggingItem)
		{
			Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition - PanelTrans.localPosition;
			if(InvOpen == true) TempPos = Input.mousePosition - UICanvasTrans.localPosition - InvManager.InvUI.PanelTrans.localPosition;
			DragSlot.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-DragSlot.GetComponent<RectTransform>().rect.width/2,TempPos.y-DragSlot.GetComponent<RectTransform>().rect.height,0);
		}

		if(Dragging == true)
		{
			Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition;
			InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-InvManager.InvUI.PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
		}
	}

	public void ChangeItemPos ()
	{
		if(DragTarget >= 0 && InvManager.Slots[DragTarget].IsTaken == false)
		{
			//Moving back item to inventory:
			Item ItemScript = InvEquipment.EquipmentSlots[DragID].Item.GetComponent<Item>(); //Getting the item script.  
			CustomEvents.OnPlayerUnEquipItem(ItemScript, InvEquipment.EquipmentSlots[DragID].GroupID);
			
			//we move it back to the inventory.
			InvManager.AddItemToSlot(InvEquipment.EquipmentSlots[DragID].Item, DragTarget); //Add it to the inventory.
			
			InvEquipment.EquipmentSlots[DragID].IsTaken = false;
			InvEquipment.EquipmentSlots[DragID].Item = null;
			
			InvEquipment.EquipmentSlots[DragID].Icon.sprite = null;
			InvEquipment.EquipmentSlots[DragID].Icon.color = new Color(1.0f,1.0f,1.0f,0.0f);

			InvEquipment.DesactivateEqObj (ItemScript.Name);


			
			DraggingItem = false;
			DragTarget = -1;

			for(int x = 0; x < ItemScript.Attributes.Length; x++)
			{
				for(int j = 0; j < InvEquipment.Attributes.Length; j++)
				{
					if(InvEquipment.Attributes[j].Name == ItemScript.Attributes[x].Name)
					{
						InvEquipment.Attributes[j].Value -= ItemScript.Attributes[x].Value;
					}
				}
			}
			
			InvManager.InvUI.RefreshItems();
			if(InvManager.SaveAndLoad == true) InvManager.SaveItems();

			InvEquipment.SaveEquipments();
		}
		else if(DragTarget < 0) //Else put it back in the equipment window.
		{ 
			InvEquipment.EquipmentSlots[DragID].Icon.color = new Color(1.0f,1.0f,1.0f,1.0f);
			
			DraggingItem = false;
		}
	}

	//Close button:
	public void  ToggleEquipment (){
		Panel.SetActive(!Panel.active);
		if(Panel.active == false)
		{
			if(DraggingItem == true)
			{
				InvEquipment.EquipmentSlots[DragID].Icon.gameObject.GetComponent<SlotUI>().DisableItemDrag ();
			}

			if(CustomEvents) CustomEvents.OnPlayerCloseEquipment ();
		}
		else
		{
			if(CustomEvents) CustomEvents.OnPlayerOpenEquipment ();
		}
	}

	public void RefreshAttributes ()
	{
		if(AttributesInfo == null) return;
		AttributesInfo.text = "Equipment Attributes:";
		for(int i = 0; i < InvEquipment.Attributes.Length; i++)
		{
			AttributesInfo.text+= "\n"+InvEquipment.Attributes[i].Name+": "+InvEquipment.Attributes[i].Value.ToString();
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

			if(InvManager.Items == InvManager.MaxItems) return;

			Item ItemScript = InvEquipment.EquipmentSlots[ID].Item.GetComponent<Item>(); //Getting the item script.  
			CustomEvents.OnPlayerUnEquipItem(ItemScript, InvEquipment.EquipmentSlots[ID].GroupID);

			InvEquipment.DesactivateEqObj (ItemScript.Name);
			
			//we move it back to the inventory.
			InvManager.AddItem(InvEquipment.EquipmentSlots[ID].Item); //Add it to the inventory.
			
			InvEquipment.EquipmentSlots[ID].IsTaken = false;
			InvEquipment.EquipmentSlots[ID].Item = null;
			
			InvEquipment.EquipmentSlots[ID].Icon.sprite = null;
			InvEquipment.EquipmentSlots[ID].Icon.color = new Color(1.0f,1.0f,1.0f,0.0f);

			for(int x = 0; x < ItemScript.Attributes.Length; x++)
			{
				for(int j = 0; j < InvEquipment.Attributes.Length; j++)
				{
					if(InvEquipment.Attributes[j].Name == ItemScript.Attributes[x].Name)
					{
						InvEquipment.Attributes[j].Value -= ItemScript.Attributes[x].Value;
					}
				}
			}
			
			InvEquipment.SaveEquipments();
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