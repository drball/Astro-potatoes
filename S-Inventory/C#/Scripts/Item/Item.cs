/*

Item Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour 
{
	
	//Item type:
	public enum ItemDropTypes {Global = 0, Spawn = 1, Destroy = 2}
	public ItemDropTypes ItemDropType = ItemDropTypes.Global & ItemDropTypes.Spawn & ItemDropTypes.Destroy;

	public int ItemType = 0;
	
	public Sprite Icon;
	public float MinDistance = 8.0f; //Minimum distance between the item and the player to pick it up.
	public string Name; //Item's name.
	public int Amount = 1; //Item's amount.
	public float Weight = 1;
	public bool IsStackable = true; //Is the item stackable in the inventory?
	public int MaxAmount = 10; //Maximum amount of item IF it's stackable.
	
	[HideInInspector]
	public Transform MyTransform;
	
	public string ShortDescription; // A short description to this item.
	
	
	//These two vars are used in buying/selling items:
	public string Currency;
	public int CurrencyAmount;
	
	//Used in the equipment system:
	public string EquipmentSlot;
	[System.Serializable]
	public class ItemAttributes
	{
		public string Name;
		public int Value;
	}
	public ItemAttributes[] Attributes;
	
	//Skill bar system:
	public string SkillSlot;

	public bool Usable = false;
	public bool DestroyOnUse = true;
	
	[HideInInspector]
	public bool  ShowInfo = false;
	[HideInInspector]
	public Vector2 MousePos;
	
	//Other scripts:
	[HideInInspector]
	public InventoryManager InvManager;
	[HideInInspector]
	public InventoryUI InvGUI;
	
	
	void  Start (){
		InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager; //Get the Inventory Manager object/script.
		InvGUI = FindObjectOfType(typeof(InventoryUI)) as InventoryUI; //Get the Inventory Manager object/script.
		MyTransform = gameObject.transform; //Set the object tranform.
	}
	
	void  OnMouseDown (){
		float Distance;
		Distance = Vector3.Distance(MyTransform.position, InvManager.Player.transform.position); //Get the current distance between the player and the item.
		if(Distance <= MinDistance && InvManager.PickupType == InventoryManager.PickupTypes.Mouse) //Check if that distance is below or equal to the minimum distance. And the player is using the mouse to pick up items.
		{
			if(ItemType == 0)
			{
				InvManager.AddItem(MyTransform); //Add this item to the bag
			}
			if(ItemType == 1)
			{
				InvManager.AddCurrency(MyTransform); //Add this currency to the inventory
			}
		}
	}
	
	void  Update (){  
		//Always updating the mouse position.
		MousePos.x = Input.mousePosition.x; 
		MousePos.y = Screen.height - Input.mousePosition.y;
	}
	
	void  OnTriggerStay2D ( Collider2D other  ){
		Debug.Log("a trigger happened");
		if(InvManager.PickupType == InventoryManager.PickupTypes.Keyboard) //Check if the player is using the keyboard to pick up items.
		{
			if(Input.GetKey(InvManager.ActionKey)) //Checking if the player pressed the key used to pick up items.
			{
				if(other == InvManager.Player.GetComponent<Collider2D>()) 
				{
					if(ItemType == 0)
					{
						InvManager.AddItem(MyTransform); //Add this item to the bag
					}
					if(ItemType == 1)
					{
						InvManager.AddCurrency(MyTransform); //Add this currency to the inventory
					}    
				}
			}
		}     
		
		if(InventoryUI.PickingUP == true || InvManager.OnCollisionPickup == true)
		{
			if(other == InvManager.Player.GetComponent<Collider2D>()) 
			{
				if(ItemType == 0)
				{
					InvManager.AddItem(MyTransform); //Add this item to the bag
				}
				if(ItemType == 1)
				{
					InvManager.AddCurrency(MyTransform); //Add this currency to the inventory
				}  
			}
		} 
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//--DRB commented this out because not needed
		// if(InvManager.PickupType == InventoryManager.PickupTypes.Keyboard) //Check if the player is using the keyboard to pick up items.
		// {
		// 	if(Input.GetKey(InvManager.ActionKey)) //Checking if the player pressed the key used to pick up items.
		// 	{
		// 		if(other == InvManager.Player.GetComponent<Collider>()) 
		// 		{
		// 			if(ItemType == 0)
		// 			{
		// 				InvManager.AddItem(MyTransform); //Add this item to the bag
		// 			}
		// 			if(ItemType == 1)
		// 			{
		// 				InvManager.AddCurrency(MyTransform); //Add this currency to the inventory
		// 			}    
		// 		}
		// 	}
		// }     
		
		if(InventoryUI.PickingUP == true || InvManager.OnCollisionPickup == true)
		{
			if(other == InvManager.Player.GetComponent<Collider>()) 
			{
				if(ItemType == 0)
				{
					InvManager.AddItem(MyTransform); //Add this item to the bag
				}
				if(ItemType == 1)
				{
					InvManager.AddCurrency(MyTransform); //Add this currency to the inventory
				}  
			}
		} 
	}
	
	
	void  OnMouseEnter (){
		if(InvManager.PickupType == InventoryManager.PickupTypes.Mouse) //Check if the player is using the mouse to pick up items.
		{
			ShowInfo = true; //If the mouse hover over the item, we show the item's info.
		}
	}
	
	void  OnMouseExit (){
		if(InvManager.PickupType == InventoryManager.PickupTypes.Mouse) //Check if the player is using the mouse to pick up items.
		{
			ShowInfo = false; //If the mouse leaves the items, we hide the item's info.
		}    
	}
}

