/*
    S-Inventory and S-Quest commincate via this script.
*/

using UnityEngine;
using System.Collections;

namespace S_Quest
{
	public class InventoryQuest : MonoBehaviour 
	{

		public void Reward (GameObject Item) 
		{
			/*InventoryManager InvManager;
        InvManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager;
		GameObject ItemClone = (GameObject) Instantiate (Item, new Vector3(0, 0, 0), Quaternion.identity);
		InvManager.AddItem(ItemClone.transform);*/
		}
	}

	//IF YOU WANT TO REWARD THE PLAYER WITH S-INVENTORY ITEMS AFTER FINISHING A QUEST:
	//UNCOMMENT THE REWARD FUNCTION.
}