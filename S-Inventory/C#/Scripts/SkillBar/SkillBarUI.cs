using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillBarUI : MonoBehaviour 
{
	public GameObject UICanvas;
	[HideInInspector]
	public RectTransform UICanvasTrans;
	
	public GameObject Panel; //The skill bar's backdrop.
	[HideInInspector]
	public RectTransform PanelTrans;

	//Scripts:
	[HideInInspector]
	public InventoryManager InvManager;
	[HideInInspector]
	public SkillBar InvSkillBar;
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
	
	void  Awake (){
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		InvSkillBar = FindObjectOfType(typeof(SkillBar)) as SkillBar; //Get the skill bar script.
		CustomEvents = FindObjectOfType(typeof(InventoryEvents)) as InventoryEvents;

		//Setting up rect transforms:
		UICanvasTrans = UICanvas.GetComponent<RectTransform>();
		PanelTrans = Panel.GetComponent<RectTransform>();
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
	}

	public void ChangeItemPos ()
	{
		Item ItemScript = InvSkillBar.SkillSlot[DragID].Item.GetComponent<Item>(); //Getting the item script. 
		if(DragTarget >= 0 && InvManager.Slots[DragTarget].IsTaken == false)
		{
			//Moving back item to inventory:
			CustomEvents.OnSkillBarRemove(ItemScript);
			
			//we move it back to the inventory.
			InvManager.AddItemToSlot(InvSkillBar.SkillSlot[DragID].Item , DragTarget); //Add it to the inventory.
			
			InvSkillBar.SkillSlot[DragID].IsTaken = false;
			InvSkillBar.SkillSlot[DragID].Item = null;
			
			InvSkillBar.SkillSlot[DragID].Icon.sprite = null;
			InvSkillBar.SkillSlot[DragID].Icon.color = new Color(1.0f,1.0f,1.0f,0.0f);
			
			InvSkillBar.SaveSkillBar();
			
			DraggingItem = false;
			DragTarget = -1;
			
			InvManager.InvUI.RefreshItems();
			if(InvManager.SaveAndLoad == true) InvManager.SaveItems();
		}
		else if(DragTarget < 0) //Else put it back in the equipment window.
		{ 
			InvSkillBar.SkillSlot[DragID].Icon.color = new Color(1.0f,1.0f,1.0f,1.0f);
			InvSkillBar.SkillSlot[DragID].Icon.transform.Find("Text").GetComponent<Text>().text = ItemScript.Amount.ToString();

			DraggingItem = false;
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
			
			Item ItemScript = InvSkillBar.SkillSlot[ID].Item.GetComponent<Item>(); //Getting the item script.  
			CustomEvents.OnSkillBarRemove(ItemScript);
			
			//we move it back to the inventory.
			InvManager.AddItem(InvSkillBar.SkillSlot[ID].Item); //Add it to the inventory.
			
			InvSkillBar.SkillSlot[ID].IsTaken = false;
			InvSkillBar.SkillSlot[ID].Item = null;
			
			InvSkillBar.SkillSlot[ID].Icon.sprite = null;
			InvSkillBar.SkillSlot[ID].Icon.color = new Color(1.0f,1.0f,1.0f,0.0f);

			InvSkillBar.SkillSlot[ID].Icon.transform.Find("Text").GetComponent<Text>().text = "";
			
			InvSkillBar.SaveSkillBar();
		}
	}
	
}