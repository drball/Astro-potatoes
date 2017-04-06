using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace S_Quest
{
	public class QuestUIManager : MonoBehaviour {
		//Quest UI:
		public RectTransform UICanvas;
		public RectTransform Panel;
		public Image QuestGiverImage;
		public Text QuestGiverName;
		public Text QuestTitleText;
		public Text QuestDescriptionText;
		public Text QuestAwardText;
		public Image QuestAwardImage;
		public Button AcceptButton;
		public Button CloseButton;
		public Button AbandonButton;
		public Button FinishButton;

		/*//Visual and classic 2 theme variables:
	public 	Sprite EliminationIcon;
	public Sprite CollectionIcon;
	public Sprite MeetingIcon;
	public Sprite GotoIcon;*/

		public enum QuestPositions {TopRightCorner = 0, LowerRightCorner = 1, TopLeftCorner = 2, LowerLeftCorner = 3} 
		public QuestPositions QuestPosition= QuestPositions.TopRightCorner & QuestPositions.LowerRightCorner & QuestPositions.TopLeftCorner & QuestPositions.LowerLeftCorner; //Quest default position.

		//Drag and drop GUI:
		public bool  IsMovable = true; //Can the player change the position of the inventory GUI in game?
		[HideInInspector]
		public bool  Dragging = false;
		public GameObject PanelDragPos;

		[HideInInspector]
		public Quest ActiveQuest; //The current active quest.

		void  Awake (){
			CloseQuest();

			SetQuestPosition();
		}

		void  SetQuestPosition (){
			// Panel.anchorMax = new Vector2(0.5f,0.5f);
			// Panel.anchorMin = new Vector2(0.5f,0.5f);
			// Panel.pivot = new Vector2(0f,0f);

			// switch(QuestPosition)
			// {
			// case QuestPositions.TopRightCorner: //Top Right corner
			// 	Panel.localPosition = new Vector3(Screen.width/2-Panel.rect.width,-(Panel.rect.height-Screen.height/2), 0);
			// 	break;

			// case QuestPositions.LowerRightCorner: //Lower right corner
			// 	Panel.localPosition = new Vector3(Screen.width/2-Panel.rect.width,-(Screen.height/2), 0);
			// 	break;

			// case QuestPositions.TopLeftCorner: //Top Left corner
			// 	Panel.localPosition = new Vector3(-(Screen.width/2),-(Panel.rect.height-Screen.height/2), 0);
			// 	break;

			// case QuestPositions.LowerLeftCorner: //Lower Left corner
			// 	Panel.localPosition = new Vector3(-(Screen.width/2),-(Screen.height/2), 0);
			// 	break; 
			// }
		}

		public void  OpenQuest (){
			Panel.gameObject.SetActive(true);

			if(ActiveQuest.Meeting == true || ActiveQuest.ReturningToGiver == true)
			{
				FinishButton.gameObject.SetActive(true);
				AcceptButton.gameObject.SetActive(false);
				CloseButton.gameObject.SetActive(false);
				AbandonButton.gameObject.SetActive(false);
			}
			//If the quest is already finished.
			else if(ActiveQuest.QuestFinished == true)
			{
				FinishButton.gameObject.SetActive(false);
				AcceptButton.gameObject.SetActive(false);
				CloseButton.gameObject.SetActive(true);
				AbandonButton.gameObject.SetActive(false);
			}
			else if(ActiveQuest.QuestActive == false) //If the quest is still active.
			{
				FinishButton.gameObject.SetActive(false);
				AcceptButton.gameObject.SetActive(true);
				CloseButton.gameObject.SetActive(true);
				AbandonButton.gameObject.SetActive(false);
			}  
			else if(ActiveQuest.QuestActive == true) //If the quest is inactive.
			{
				FinishButton.gameObject.SetActive(false);
				AcceptButton.gameObject.SetActive(false);
				CloseButton.gameObject.SetActive(true);

				//show the abandon button only if the quest is abandonable:
				if (ActiveQuest.NonAbandonable == false) {
					AbandonButton.gameObject.SetActive (true);
				}
			} 
			//Quest giver name:
			QuestGiverName.text = ActiveQuest.QuestGiverName;
			//Quest giver image:
			if(ActiveQuest.QuestGiverSprite != null)
			{
				QuestGiverImage.sprite = ActiveQuest.QuestGiverSprite;
			}
			else
			{
				QuestGiverImage.gameObject.SetActive(false);
			}

			//Quest title:
			if(ActiveQuest.Meeting == true)
			{
				QuestTitleText.text = ActiveQuest.Sender.QuestTitle;
			}
			else
			{
				QuestTitleText.text = ActiveQuest.QuestTitle;
			}

			//Quest description:
			QuestDescriptionText.text = ActiveQuest.GetQuestString();

			SetQuestReward ();

			if(ActiveQuest.Speech) ActiveQuest.gameObject.GetComponent<AudioSource>().PlayOneShot(ActiveQuest.Speech);
		}

		public void  SetQuestReward (){
			//Quest reward:
			int GiveXP = ActiveQuest.XP;
			if(ActiveQuest.Meeting == true)
			{
				GiveXP = ActiveQuest.Sender.XP;
			}

			if(ActiveQuest.RewardPlayer == true && ActiveQuest.QuestFinished == false)
			{
				QuestAwardText.text = GiveXP.ToString() + " XP Points.\n";
			}
			else
			{
				QuestAwardText.gameObject.SetActive(false);
			}

			if(QuestAwardImage) QuestAwardImage.gameObject.SetActive(false);

			/*GameObject ItemToGive = ActiveQuest.Item;
		
		if(ActiveQuest.Meeting == true)
		{
			ItemToGive = ActiveQuest.Sender.Item;
		}
		
		if(ItemToGive != null && ActiveQuest.RewardPlayer == true && ActiveQuest.QuestFinished == false)
		{
			Item ItemScript = ItemToGive.gameObject.GetComponent<Item>();
			QuestAwardText.text = ItemScript.Name+"("+ItemScript.Amount.ToString()+") - "+GiveXP.ToString() + " XP Points.\n";
			QuestAwardImage.sprite = ItemScript.Icon;
			QuestAwardImage.gameObject.SetActive(true);
		}*/
		}

		public void  CloseQuest (){
			Panel.gameObject.SetActive(false);
			if(ActiveQuest != null) ActiveQuest.QuestOpen = false;
			ActiveQuest = null;
		}

		public void  FinishQuest (){
			if(ActiveQuest.Meeting == true)
			{
				ActiveQuest.FinishMeetingQuest();
			}
			else if(ActiveQuest.ReturningToGiver == true)
			{
				ActiveQuest.FinishQuest();
			}

			CloseQuest ();
		}

		public void  ActivateQuest (){
			ActiveQuest.Manager.AddActiveQuest(ActiveQuest);
			ActiveQuest.AcceptQuest();
			CloseQuest ();
		}

		public void  RemoveQuest (){
			ActiveQuest.AbandonQuest ();
			CloseQuest ();
		}

		void  Update (){
			if(Dragging == true)
			{
				Vector3 TempPos = Input.mousePosition - UICanvas.localPosition;
				PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
			}
		}

		//Dragging and dropping the quest window:
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