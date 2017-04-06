/*

Inventory Custom Events Script Created by Oussama Bouanani (SoumiDelRio).
This script contains some helpful events to customize S-Inventory.
If you have suggestions for more events that you need in your GameObject, please contact me.

*/

using UnityEngine;
using System.Collections;

public class InventoryEvents : MonoBehaviour
{
	// Set true to log info to the console:
	public bool debug = true;
	
	// Delegate for inventory events:
	public delegate void ItemEventHandler(Item ItemObj);
	public delegate void PanelEventHandler();
	
	// You can hook into these inventory events:
	public event ItemEventHandler PlayerAddedItem = delegate {};
	public event ItemEventHandler PlayerDroppedItem = delegate {};
	public event ItemEventHandler PlayerUsedInventoryItem = delegate {};
	public event ItemEventHandler PlayerEquippedItem = delegate {};
	public event ItemEventHandler PlayerUnequippedItem = delegate {};
	public event ItemEventHandler AddedToSkillBar = delegate {};
	public event ItemEventHandler RemovedFromSkillBar = delegate {};
	public event ItemEventHandler UsedSkillBarItem = delegate {};
	public event ItemEventHandler PlayerBoughtItem = delegate {};
	public event ItemEventHandler PlayerSoldItem = delegate {};
	public event ItemEventHandler PlayerCraftedItem = delegate {};

	public event PanelEventHandler PlayerOpenedInventory = delegate {};
	public event PanelEventHandler PlayerClosedInventory = delegate {};
	public event PanelEventHandler PlayerOpenedEquipment = delegate {};
	public event PanelEventHandler PlayerClosedEquipment = delegate {};
	public event PanelEventHandler PlayerOpenedContainer = delegate {};
	public event PanelEventHandler PlayerClosedContainer = delegate {};
	public event PanelEventHandler PlayerOpenedItemGroup = delegate {};
	public event PanelEventHandler PlayerClosedItemGroup = delegate {};
	public event PanelEventHandler PlayerOpenedCrafting = delegate {};
	public event PanelEventHandler PlayerClosedCrafting = delegate {};
	public event PanelEventHandler PlayerOpenedVendor = delegate {};
	public event PanelEventHandler PlayerClosedVendor = delegate {};
	
	//================================================================================
	// Inventory Events:

	public void OnPlayerOpenInventory ()
	{
		if (debug) Debug.Log("Just opened the inventory panel");
		PlayerOpenedInventory ();
	}
	
	public void OnPlayerCloseInventory ()
	{
		if (debug) Debug.Log("Just closed the inventory panel");
		PlayerClosedInventory ();
	}
	
	public void OnPlayerAddItem (Item ItemObj)
	{
		if (debug) Debug.Log("Just added "+ItemObj.Name+" to the inventory");
		PlayerAddedItem(ItemObj);
	}
	
	public void OnPlayerDropItem (Item ItemObj)
	{
		if (debug) Debug.Log("Just removed "+ItemObj.Name+" from the inventory");
		PlayerDroppedItem(ItemObj);
	}
	
	public void OnPlayerUseInventoryItem (Item ItemObj)
	{
		if (debug) Debug.Log("Just used "+ItemObj.Name+" from the inventory");
		PlayerUsedInventoryItem(ItemObj);
	}
	
	//================================================================================
	// Equipment Events:

	public void OnPlayerOpenEquipment ()
	{
		if (debug) Debug.Log("Just opened the equipment panel");
		PlayerOpenedEquipment ();
	}
	
	public void OnPlayerCloseEquipment ()
	{
		if (debug) Debug.Log("Just closed the equipment panel");
		PlayerClosedEquipment ();
	}

	public void OnPlayerEquipItem (Item ItemObj, int GroupID)
	{
		if (debug) Debug.Log("Just equipped "+ItemObj.Name+" on "+ItemObj.EquipmentSlot+" slot.");
		PlayerEquippedItem(ItemObj);
	}
	
	public void OnPlayerUnEquipItem (Item ItemObj, int GroupID)
	{
		if (debug) Debug.Log("Just unequipped "+ItemObj.Name+" from "+ItemObj.EquipmentSlot+" slot.");
		PlayerUnequippedItem(ItemObj);
	}
	
	//================================================================================
	//Skill Bar Events:
	
	public void OnSkillBarAdd (Item ItemObj)
	{
		if (debug) Debug.Log("Just added "+ItemObj.Name+" to "+ItemObj.SkillSlot+" skill bar slot.");
		AddedToSkillBar(ItemObj);
	}
	
	public void OnSkillBarRemove (Item ItemObj)
	{
		if (debug) Debug.Log("Just removed "+ItemObj.Name+" from "+ItemObj.SkillSlot+" skill bar slot.");
		RemovedFromSkillBar(ItemObj);
	}
	
	public void OnSkillBarItemUsed (Item ItemObj)
	{
		if (debug) Debug.Log("Just used "+ItemObj.Name+" from "+ItemObj.SkillSlot+" skill bar slot.");
		UsedSkillBarItem(ItemObj);
	}
	
	//================================================================================
	//Container Events:

	public void OnPlayerOpenContainer ()
	{
		if (debug) Debug.Log("Just opened the container panel");
		PlayerOpenedContainer ();
	}
	
	public void OnPlayerCloseContainer ()
	{
		if (debug) Debug.Log("Just closed the container panel");
		PlayerClosedContainer ();
	}

	//================================================================================
	//Item Group Events:
	
	public void OnPlayerOpenItemGroup ()
	{
		if (debug) Debug.Log("Just opened the item group panel");
		PlayerOpenedItemGroup ();
	}
	
	public void OnPlayerCloseItemGroup ()
	{
		if (debug) Debug.Log("Just closed the item group panel");
		PlayerClosedItemGroup ();
	}

	//================================================================================
	//Vendor Events:

	public void OnPlayerOpenVendor ()
	{
		if (debug) Debug.Log("Just opened the vendor panel");
		PlayerOpenedVendor ();
	}
	
	public void OnPlayerCloseVendor ()
	{
		if (debug) Debug.Log("Just closed the vendor panel");
		PlayerClosedVendor ();
	}

	public void OnPlayerBuyItem (Item ItemObj)
	{
		if (debug) Debug.Log("Just bought: "+ItemObj.Name);
		PlayerBoughtItem(ItemObj);
	}
	
	public void  OnPlayerSellItem (Item ItemObj)
	{
		//Your code here.
		if (debug) Debug.Log("Just sold: "+ItemObj.Name);
		PlayerSoldItem(ItemObj);
	}
	
	//================================================================================
	//Craft Events:

	public void OnPlayerOpenCrafting ()
	{
		if (debug) Debug.Log("Just opened the crafting panel");
		PlayerOpenedCrafting ();
	}
	
	public void OnPlayerCloseCrafting ()
	{
		if (debug) Debug.Log("Just closed the crafting panel");
		PlayerClosedCrafting ();
	}

	public void OnPlayerCraftItem (Item ItemObj)
	{
		if (debug) Debug.Log ("Just crafted: " + ItemObj.Name);
		PlayerCraftedItem(ItemObj);
	}
	
}

//Uncomment the code below to 
/*using UnityEngine;
using System.Collections;

public class InventoryEvents : MonoBehaviour
{
	// Set true to log info to the console:
	public bool debug = true;
	
	// Delegate for inventory events:
	public delegate void ItemEventHandler(string itemName);
	
	// You can hook into these inventory events:
	public event ItemEventHandler PlayerAddedItem = delegate {};
	public event ItemEventHandler PlayerDroppedItem = delegate {};
	public event ItemEventHandler PlayerUsedInventoryItem = delegate {};
	public event ItemEventHandler PlayerEquippedItem = delegate {};
	public event ItemEventHandler PlayerUnequippedItem = delegate {};
	public event ItemEventHandler AddedToSkillBar = delegate {};
	public event ItemEventHandler RemovedFromSkillBar = delegate {};
	public event ItemEventHandler UsedSkillBarItem = delegate {};
	public event ItemEventHandler PlayerBoughtItem = delegate {};
	public event ItemEventHandler PlayerSoldItem = delegate {};
	public event ItemEventHandler PlayerCraftedItem = delegate {};
	
	//================================================================================
	// Inventory Events:
	
	public void OnPlayerAddItem (string ItemObj.Name)
	{
		if (debug) Debug.Log("Just added "+ItemObj.Name+" to the inventory");
		PlayerAddedItem(ItemObj.Name);
	}
	
	public void OnPlayerDropItem (string ItemObj.Name)
	{
		if (debug) Debug.Log("Just removed "+ItemObj.Name+" from the inventory");
		PlayerDroppedItem(ItemObj.Name);
	}
	
	public void OnPlayerUseInventoryItem (string ItemObj.Name)
	{
		if (debug) Debug.Log("Just used "+ItemObj.Name+" from the inventory");
		PlayerUsedInventoryItem(ItemObj.Name);
	}
	
	//================================================================================
	// Equipment Events:
	
	public void OnPlayerEquipItem (string ItemObj.Name, string EquipmentSlot)
	{
		if (debug) Debug.Log("Just equipped "+ItemObj.Name+" on "+EquipmentSlot+" slot.");
		PlayerEquippedItem(ItemObj.Name);
	}
	
	public void OnPlayerUnEquipItem (string ItemObj.Name, string EquipmentSlot)
	{
		if (debug) Debug.Log("Just unequipped "+ItemObj.Name+" from "+EquipmentSlot+" slot.");
		PlayerUnequippedItem(ItemObj.Name);
	}
	
	//================================================================================
	//Skill Bar Events:
	
	public void OnSkillBarAdd (string ItemObj.Name, string SkillSlot)
	{
		if (debug) Debug.Log("Just added "+ItemObj.Name+" to "+SkillSlot+" skill bar slot.");
		AddedToSkillBar(ItemObj.Name);
	}
	
	public void OnSkillBarRemove (string ItemObj.Name, string SkillSlot)
	{
		if (debug) Debug.Log("Just removed "+ItemObj.Name+" from "+SkillSlot+" skill bar slot.");
		RemovedFromSkillBar(ItemObj.Name);
	}
	
	public void OnSkillBarItemUsed (string ItemObj.Name, string SkillSlot)
	{
		if (debug) Debug.Log("Just used "+ItemObj.Name+" from "+SkillSlot+" skill bar slot.");
		UsedSkillBarItem(ItemObj.Name);
	}
	
	//================================================================================
	//Vendor Events:
	
	public void OnPlayerBuyItem (string ItemObj.Name)
	{
		if (debug) Debug.Log("Just bought: "+ItemObj.Name);
		PlayerBoughtItem(ItemObj.Name);
	}
	
	public void  OnPlayerSellItem (string ItemObj.Name)
	{
		//Your code here.
		if (debug) Debug.Log("Just sold: "+ItemObj.Name);
		PlayerSoldItem(ItemObj.Name);
	}
	
	//================================================================================
	//Craft Events:
	
	public void OnPlayerCraftItem (string ItemObj.Name)
	{
		if (debug) Debug.Log ("Just crafted: " + ItemObj.Name);
		PlayerCraftedItem(ItemObj.Name);
	}
	
}*/