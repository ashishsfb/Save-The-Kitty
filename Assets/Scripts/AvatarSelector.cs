using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AvatarSelector: MonoBehaviour{
	public static AvatarSelector instance;
	public GameObject[] avatars;
	public int currentAvatar;

	public Text displayText;
	public Scrollbar scroller;


	public void Start(){
		Debug.Log ("Avatar Selector called");
		instance = this;
		currentAvatar = GameManager.instance.kitty.GetComponent<KittyAvatar>().avatarId;
		scroller.value = 0;

		//update prefab array (avatars) from the playerPrefs for locked status and selecting current avatar
		for(int i = 1; i <= avatars.Length; i++){
			if (!PlayerPrefs.HasKey ("avatar"+i.ToString()+"_isLocked")) {
				if(i == 1)
					PlayerPrefs.SetInt ("avatar"+i.ToString()+"_isLocked", 0);
				else
					PlayerPrefs.SetInt ("avatar"+i.ToString()+"_isLocked", 1);
			}
		}
	}

	public void BuyAvatar(){
		
	}
}
