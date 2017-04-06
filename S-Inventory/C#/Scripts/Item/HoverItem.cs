using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HoverItem : MonoBehaviour {
	public GameObject ItemInfo;
	public Text Textfield;

	//Mouse Hover:
	public bool InfoOnMouseHover = true; //If you are targeting mobile platforms, set this to false.
	[HideInInspector]
	public bool HoverActive = false; //This item returns if the mouse is hovering one of the items in the inventory GUI or not.
	[HideInInspector]
	public string Description;
	[HideInInspector]
	public Transform Item;

	//Scripts:
	[HideInInspector]
	public InventoryUI InvUI;
	
	void  Awake (){
		InvUI = FindObjectOfType(typeof(InventoryUI)) as InventoryUI; //Get the Inventory UI object/script.
		ItemInfo.gameObject.SetActive(false);
	}
	
	void  Update (){
		if(HoverActive == true && InfoOnMouseHover == true)
		{
			Vector3 TempPos2 = Input.mousePosition - InvUI.UICanvasTrans.localPosition - ItemInfo.transform.parent.GetComponent<RectTransform>().localPosition;
			ItemInfo.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(TempPos2.x+(ItemInfo.gameObject.GetComponent<RectTransform>().rect.width/2)*1.1f,TempPos2.y-(ItemInfo.gameObject.GetComponent<RectTransform>().rect.height/2)*1.1f,0);
			
			Item HoverScript = Item.GetComponent<Item>(); //Getting the item script.
			
			if(HoverScript.IsStackable == true)
			{
				Description = HoverScript.Name+"("+HoverScript.Amount+"):\n"+"Weight: "+HoverScript.Weight+"kg\n"+HoverScript.ShortDescription;
			}
			else
			{
				Description =  HoverScript.Name+":\n"+"Weight: "+HoverScript.Weight+"kg\n"+HoverScript.ShortDescription;
			}
			if(HoverScript.Attributes.Length > 0)
			{
				Description += "\nEquipment Attributes:";
				for(int i = 0; i < HoverScript.Attributes.Length; i++)
				{
					Description += "\n"+HoverScript.Attributes[i].Name+": "+HoverScript.Attributes[i].Value.ToString();
				}
			}
			Textfield.GetComponent<Text>().text = Description;   
		}
	}
}