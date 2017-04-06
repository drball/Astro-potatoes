using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace S_Quest
{
	public class QuestLogUI : MonoBehaviour {

		public RectTransform UICanvas;
		public RectTransform Panel;

		public Button QuestButton;
		public Transform QuestButtonsParent;
		[System.Serializable]
		public class QuestButtonVars
		{
			public Button ButtonObj;
			public Text ButtonText;
			[HideInInspector]
			public Quest ChildQuest;
		}
		[HideInInspector]
		public List<QuestButtonVars> QuestButtons = new List<QuestButtonVars>();
		[HideInInspector]
		public int CurrentQuest = -1;

		public GameObject QuestInfo;
		public Image QuestGiverImage;
		public Text QuestGiverName;
		public Text QuestTitleText;
		public Text QuestDescriptionText;
		public Text QuestAwardText;
		public Image QuestAwardImage;
		public Button AbandonButton;

		public enum QuestLogPositions {TopRightCorner = 0, LowerRightCorner = 1, TopLeftCorner = 2, LowerLeftCorner = 3} 
		public QuestLogPositions QuestLogPosition= QuestLogPositions.TopRightCorner & QuestLogPositions.LowerRightCorner & QuestLogPositions.TopLeftCorner & QuestLogPositions.LowerLeftCorner; //Quest default position.

		//Drag and drop GUI:
		public bool  IsMovable = true; //Can the player change the position of the inventory GUI in game?
		[HideInInspector]
		public bool  Dragging = false;
		public GameObject PanelDragPos;

		void  Awake ()
		{
			CloseQuestLog();

			// SetQuestLogPosition();

			QuestButtons.Clear ();

			QuestButtonVars NewButtonVars = new QuestButtonVars ();
			NewButtonVars.ButtonObj = QuestButton.GetComponent<Button> ();
			NewButtonVars.ButtonText = QuestButton.transform.GetComponentInChildren<Text> ();
			QuestButton.gameObject.GetComponent<QuestButton> ().ID = 0;

			QuestButtons.Add (NewButtonVars);
		}

		void  SetQuestLogPosition (){
			Panel.anchorMax = new Vector2(0.5f,0.5f);
			Panel.anchorMin = new Vector2(0.5f,0.5f);
			Panel.pivot = new Vector2(0f,0f);

			switch(QuestLogPosition)
			{
			case QuestLogPositions.TopRightCorner: //Top Right corner
				Panel.localPosition = new Vector3(Screen.width/2-Panel.rect.width,-(Panel.rect.height-Screen.height/2), 0);
				break;

			case QuestLogPositions.LowerRightCorner: //Lower right corner
				Panel.localPosition = new Vector3(Screen.width/2-Panel.rect.width,-(Screen.height/2), 0);
				break;

			case QuestLogPositions.TopLeftCorner: //Top Left corner
				Panel.localPosition = new Vector3(-(Screen.width/2),-(Panel.rect.height-Screen.height/2), 0);
				break;

			case QuestLogPositions.LowerLeftCorner: //Lower Left corner
				Panel.localPosition = new Vector3(-(Screen.width/2),-(Screen.height/2), 0);
				break; 
			}
		}

		public void  OpenQuestLog (){
			LoadActiveQuests();

			Panel.gameObject.SetActive(true);
		}

		public void AddQuestLogButton ()
		{
			GameObject NewButton = (GameObject)Instantiate (QuestButton.gameObject);
			NewButton.transform.SetParent (QuestButtonsParent, true);
			NewButton.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

			QuestButtonVars NewButtonVars = new QuestButtonVars ();
			NewButtonVars.ButtonObj = NewButton.GetComponent<Button> ();
			NewButtonVars.ButtonText = NewButton.transform.GetComponentInChildren<Text> ();
			NewButton.gameObject.GetComponent<QuestButton> ().ID = QuestButtons.Count;

			QuestButtons.Add (NewButtonVars);


			NewButton.gameObject.SetActive (false);
		}

		public void  LoadActiveQuests (){
			CurrentQuest = -1; QuestInfo.gameObject.SetActive(false);

			int ActiveQuests = 0;
			Quest[] Quests = FindObjectsOfType(typeof(Quest)) as Quest[];
			foreach(Quest TempQuest in Quests) 
			{
				if(TempQuest.QuestActive == true) //Check if the current quest is active.
				{
					if (ActiveQuests >= QuestButtons.Count) {
						//Add new quest button.
						AddQuestLogButton();
					}

					QuestButtons [ActiveQuests].ChildQuest = TempQuest;
					QuestButtons [ActiveQuests].ButtonText.text = TempQuest.QuestTitle;
					ActiveQuests++;
				}
			}

			for(int i = 0; i < QuestButtons.Count; i++)
			{
				if(i >= ActiveQuests)
				{
					QuestButtons[i].ButtonObj.gameObject.SetActive(false);
				}
				else
				{
					QuestButtons[i].ButtonObj.gameObject.SetActive(true);
				}
			}
		}

		public void  ShowQuestInfo (int ID){

			QuestInfo.gameObject.SetActive(true);

			//Quest giver name:
			QuestGiverName.text = QuestButtons[ID].ChildQuest.QuestGiverName;
			//Quest giver image:
			if(QuestButtons[ID].ChildQuest.QuestGiverSprite != null)
			{
				QuestGiverImage.sprite = QuestButtons[ID].ChildQuest.QuestGiverSprite;
			}
			else
			{
				QuestGiverImage.gameObject.SetActive(false);
			}

			//Quest title:
			if(QuestButtons[ID].ChildQuest.Meeting == true)
			{
				QuestTitleText.text = QuestButtons[ID].ChildQuest.Sender.QuestTitle;
			}
			else
			{
				QuestTitleText.text = QuestButtons[ID].ChildQuest.QuestTitle;
			}

			//Quest description:
			QuestDescriptionText.text = QuestButtons[ID].ChildQuest.GetQuestString();

			SetQuestReward (ID);

			CurrentQuest = ID;
		}

		public void  SetQuestReward (int ID){
			//Quest reward:
			int GiveXP = QuestButtons[ID].ChildQuest.XP;
			if(QuestButtons[ID].ChildQuest.Meeting == true)
			{
				GiveXP = QuestButtons[ID].ChildQuest.Sender.XP;
			}

			if(QuestButtons[ID].ChildQuest.RewardPlayer == true && QuestButtons[ID].ChildQuest.QuestFinished == false)
			{
				QuestAwardText.text = "You will receive: \n" + GiveXP.ToString() + " XP Points.\n \n";
			}
			else
			{
				QuestAwardText.gameObject.SetActive(false);
			}

			if(QuestAwardImage) QuestAwardImage.gameObject.SetActive(false);

			/*GameObject ItemToGive = QuestButtons[ID].ChildQuest.Item;
		
		if(QuestButtons[ID].ChildQuest.Meeting == true)
		{
			ItemToGive = QuestButtons[ID].ChildQuest.Sender.Item;
		}
		
		if(ItemToGive != null && QuestButtons[ID].ChildQuest.RewardPlayer == true && QuestButtons[ID].ChildQuest.QuestFinished == false)
		{
			Item ItemScript = ItemToGive.gameObject.GetComponent<Item>();
			QuestAwardText.text = ItemScript.Name+"("+ItemScript.Amount.ToString()+") - "+GiveXP.ToString() + " XP Points.\n";
			QuestAwardImage.sprite = ItemScript.Icon;
			QuestAwardImage.gameObject.SetActive(true);
			
		}*/
		}

		public void  RemoveQuest (){
			QuestButtons[CurrentQuest].ChildQuest.AbandonQuest ();
			LoadActiveQuests();
		}

		public void  CloseQuestLog (){
			Panel.gameObject.SetActive(false);
		}

		void  Update (){
			if(Dragging == true)
			{
				Vector3 TempPos = Input.mousePosition - UICanvas.localPosition;
				PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
			}
		}

		//Dragging and dropping the quest log window:
		public void  DragStarted (){
			if(IsMovable == true && Dragging == false)
			{
				Dragging = true;
				PanelDragPos.gameObject.SetActive(true);
				Vector3 TempPos = Input.mousePosition - UICanvas.localPosition;
				PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
				Panel.gameObject.transform.SetParent(PanelDragPos.transform, true);
			}
		}

		public void  DragEnded (){
			if(IsMovable == true)
			{
				Dragging = false;
				PanelDragPos.gameObject.SetActive(false);
				Panel.gameObject.transform.SetParent(UICanvas.transform, true);
			}
		}
	}
}