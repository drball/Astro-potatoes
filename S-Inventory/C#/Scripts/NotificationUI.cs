/*

Notification UI Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;


//Notifications are used to send the player a message when something important happens.
//For example when the inventory is full and the player tries to add an item, it should show: "Inventory is full".

public class NotificationUI : MonoBehaviour {

	public float MsgReload = 1.5f;
	
	float MsgTimer = 0.0f;
	
	public void SendMsg(string Msg)
	{
		//Show the message:
		gameObject.GetComponent<Text>().text = Msg;
		gameObject.GetComponent<Text>().enabled = true;
		//Start the timer to hide the message:
		MsgTimer = MsgReload;
	}

	void Update ()
	{
		//Message timer:
		if(MsgTimer > 0)
		{
			MsgTimer -= Time.deltaTime;
		}
		if(MsgTimer <= 0)
		{
			MsgTimer = 0;
			//Hide the timer
			gameObject.GetComponent<Text>().enabled = false;
		}
	}

}
