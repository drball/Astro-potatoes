using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class XPUI : MonoBehaviour {

	public RectTransform FullBar;
	public RectTransform Empty;
	public Text LevelText;
	
	[HideInInspector]
	public float BarWidth = 0;
	[HideInInspector]
	public float PosX = 0;
	[HideInInspector]
	public ExperienceManagerC XPManager;
	
	public void  SetXPBarUI (){
		if(XPManager == null)
		{
			XPManager = FindObjectOfType(typeof(ExperienceManagerC)) as ExperienceManagerC;
			
			BarWidth = FullBar.rect.width;
			PosX = FullBar.localPosition.x;
		}

		FullBar.sizeDelta = new Vector2((XPManager.XP/(XPManager.Level*XPManager.Level1XP)) * BarWidth, FullBar.sizeDelta.y);
		FullBar.localPosition = new Vector3(-(BarWidth-FullBar.rect.width)/2 + PosX,FullBar.localPosition.y,FullBar.localPosition.z);
		
		LevelText.text = "Level: " + XPManager.Level.ToString();
	}
}