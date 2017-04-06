using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DemoSettings : MonoBehaviour {

	InventoryManager InvManager;
	InventoryUI InvUI;
	HoverItem ItemInfo;
	Equipment InvEq;

	public Text PickupTypeText;
	public Text DropTypeText;

	public Text EqAtt;

	// Use this for initialization
	void Awake () 
	{
		InvManager = FindObjectOfType (typeof(InventoryManager)) as InventoryManager;
		InvUI = FindObjectOfType (typeof(InventoryUI)) as InventoryUI;
		ItemInfo = FindObjectOfType (typeof(HoverItem)) as HoverItem;
		InvEq = FindObjectOfType (typeof(Equipment)) as Equipment;

		if(InvManager.PickupType == InventoryManager.PickupTypes.Mouse)
		{
			PickupTypeText.text = "Item pick up type: <color=red>Mouse</color>.\n(<color=red>P</color> to swtich to keyboard).";
		}
		else if(InvManager.PickupType == InventoryManager.PickupTypes.Keyboard)
		{
			PickupTypeText.text = "Item pick up type: <color=red>E key</color>.\n(<color=red>P</color> to swtich to mouse).";
		}

		if(InvManager.DropType == InventoryManager.DropTypes.Spawn)
		{
			DropTypeText.text = "Item drop type: <color=red>Spawn items</color>.\n(<color=red>L</color> to swtich).";
		}
		else if(InvManager.DropType == InventoryManager.DropTypes.Destroy)
		{
			DropTypeText.text = "Item drop type: <color=red>Destroy items</color>.\n(<color=red>L</color> to swtich).";
		}
	}

	void Update () 
	{
	    if(Input.GetKeyDown(KeyCode.P))
		{
			if(InvManager.PickupType == InventoryManager.PickupTypes.Mouse)
			{
				InvManager.PickupType = InventoryManager.PickupTypes.Keyboard;
				PickupTypeText.text = "Item pick up type: <color=red>E key</color>.\n(<color=red>P</color> to swtich to mouse).";
			}
			else if(InvManager.PickupType == InventoryManager.PickupTypes.Keyboard)
			{
				InvManager.PickupType = InventoryManager.PickupTypes.Mouse;
				PickupTypeText.text = "Item pick up type: <color=red>Mouse</color>.\n(<color=red>P</color> to swtich to keyboard).";
			}
		}

		if(Input.GetKeyDown(KeyCode.L))
		{
			if(InvManager.DropType == InventoryManager.DropTypes.Spawn)
			{
				InvManager.DropType = InventoryManager.DropTypes.Destroy;
				DropTypeText.text = "Item drop type: <color=red>Destroy items</color>.\n(<color=red>L</color> to swtich).";
			}
			else if(InvManager.DropType == InventoryManager.DropTypes.Destroy)
			{
				InvManager.DropType = InventoryManager.DropTypes.Spawn;
				DropTypeText.text = "Item drop type: <color=red>Spawn items</color>.\n(<color=red>L</color> to swtich).";
			}
		}

		EqAtt.text = "Equipment Attributes:\n\n";

		for(int i = 0; i < InvEq.Attributes.Length; i++)
		{
			EqAtt.text +=InvEq.Attributes[i].Name+": "+InvEq.Attributes[i].Value.ToString()+".\n\n";
		}
	}

	public void ToggleCollisionPickup ()
	{
		InvManager.OnCollisionPickup = !InvManager.OnCollisionPickup;
	}

	public void ReArrangeItems ()
	{
		InvManager.ReArrangeItems = !InvManager.ReArrangeItems;
	}

	public void ToggleItemInfo ()
	{
		ItemInfo.InfoOnMouseHover = !ItemInfo.InfoOnMouseHover;
	}

	public void ToggleDestroyOnDrag ()
	{
		InvUI.DestroyOnDragToWorld = !InvUI.DestroyOnDragToWorld;
	}
}
