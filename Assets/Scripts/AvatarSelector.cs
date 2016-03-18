using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AvatarSelector: MonoBehaviour{
	public static AvatarSelector instance;
	public GameObject[] avatars;
	public int currentAvatar, avatarsBought;
	public Sprite tick, locked, unlocked, selectedBG, unselectedBG;
	public Text avatarStoreFishCountText, avatarStoreLoginInfo, gameOverLoginInfo;

	public void Awake(){
		Debug.Log ("AvatarSelector : AWAKE");
		instance = this;
	}

	public void Start(){
		Debug.Log ("AvatarSelector : Start");
	}

	//Avatar Store Setup : setting up statuses of all kitties for current user from PlayerPrefs
	//Called from Google play manager as a last step of Login
	public void setUp(){
		Debug.Log ("AvatarSelector : SetUp");
		//Set avatarsBought if not already
		if (!PlayerPrefs.HasKey (GooglePlayManager.instance.currentAccount + "_avatarsBought")) {
			PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_avatarsBought", 0);
			avatarsBought = 0;
		}
		else {
			avatarsBought = PlayerPrefs.GetInt (GooglePlayManager.instance.currentAccount + "_avatarsBought");
		}

		for(int i = 0; i < avatars.Length; i++){
			if (!PlayerPrefs.HasKey (GooglePlayManager.instance.currentAccount + "_avatar" + i.ToString () + "_status")) {
				if (i == 0) {
					PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_avatar" + i.ToString () + "_status", 1);
					avatars [0].GetComponent<AvatarObject>().kittyGameObject.GetComponent<KittyAvatar> ().status = KittyAvatar.KittyStatuses.Selected;
					currentAvatar = 0;
				} 
				else {
					PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_avatar" + i.ToString () + "_status", 2);
					avatars [i].GetComponent<AvatarObject>().kittyGameObject.GetComponent<KittyAvatar> ().status = KittyAvatar.KittyStatuses.Locked;
				}
			}
			else {
				if (PlayerPrefs.GetInt (GooglePlayManager.instance.currentAccount + "_avatar" + i.ToString () + "_status") == 2) {
					avatars [i].GetComponent<AvatarObject>().kittyGameObject.GetComponent<KittyAvatar> ().status = KittyAvatar.KittyStatuses.Locked;
				}
				else if (PlayerPrefs.GetInt (GooglePlayManager.instance.currentAccount + "_avatar" + i.ToString () + "_status") == 0) {
					avatars [i].GetComponent<AvatarObject>().kittyGameObject.GetComponent<KittyAvatar> ().status = KittyAvatar.KittyStatuses.Bought;
				}
				else if (PlayerPrefs.GetInt (GooglePlayManager.instance.currentAccount + "_avatar" + i.ToString () + "_status") == 1) {
					avatars [i].GetComponent<AvatarObject>().kittyGameObject.GetComponent<KittyAvatar> ().status = KittyAvatar.KittyStatuses.Selected;
					currentAvatar = i;
				}
			}

			//marking the avatar as selected/bought/locked in the UI
			avatars [i].GetComponent<AvatarObject>().markAvatar();
		}
	}
}
