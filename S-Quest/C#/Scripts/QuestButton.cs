using UnityEngine;
using System.Collections;

namespace S_Quest
{
	public class QuestButton : MonoBehaviour {

		public int ID;

		QuestLogUI LogUI;

		void Awake ()
		{
			LogUI = FindObjectOfType (typeof(QuestLogUI)) as QuestLogUI;
		}

		public void QuestLogButton ()
		{
			LogUI.ShowQuestInfo (ID);
		}
	}
}
