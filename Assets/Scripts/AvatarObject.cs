using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AvatarObject : MonoBehaviour {

	public GameObject kittyGameObject;
	public GameObject statusImage, priceText;
		
	#region markAvatarInUI
	//To be used when doing initial setup from PlayerPrefs NOT while using AvatarStore
	public void markAvatar(){
		if (kittyGameObject.GetComponent<KittyAvatar> ().status == KittyAvatar.KittyStatuses.Selected) {
			markAsSelected ();
		} else if (kittyGameObject.GetComponent<KittyAvatar> ().status == KittyAvatar.KittyStatuses.Bought) {
			markAsBought ();
		} else if (kittyGameObject.GetComponent<KittyAvatar> ().status == KittyAvatar.KittyStatuses.Locked) {
			markAsLocked ();
		}
	}

	public void markAsSelected(){
		transform.parent.GetComponent<Image> ().sprite = AvatarSelector.instance.selectedBG;
		statusImage.GetComponent<Image>().sprite = AvatarSelector.instance.tick;
		priceText.GetComponent<Text> ().text = "SELECTED";
	}

	public void markAsBought(){
		transform.parent.GetComponent<Image>().sprite = AvatarSelector.instance.unselectedBG;
		statusImage.GetComponent<Image>().sprite = AvatarSelector.instance.unlocked;
		priceText.GetComponent<Text> ().text = "BOUGHT";
	}

	public void markAsLocked(){
		transform.parent.GetComponent<Image>().sprite = AvatarSelector.instance.unselectedBG;
		statusImage.GetComponent<Image>().sprite = AvatarSelector.instance.locked;
		priceText.GetComponent<Text> ().text = "Price : " + kittyGameObject.GetComponent<KittyAvatar>().price;
	}
	#endregion

	public void SelectAvatar(){
		//Selcet an already bought avatar
		if (kittyGameObject.GetComponent<KittyAvatar>().status == KittyAvatar.KittyStatuses.Bought) {
			//unselect previously selected avatar in UI
			AvatarSelector.instance.avatars [AvatarSelector.instance.currentAvatar].GetComponent<AvatarObject>().markAsBought();

			//unselect the previously selected avatar in player prefs
			PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_avatar"+AvatarSelector.instance.currentAvatar+"_status", 0);
			AvatarSelector.instance.avatars [AvatarSelector.instance.currentAvatar].GetComponent<AvatarObject>().kittyGameObject.GetComponent<KittyAvatar> ().status = KittyAvatar.KittyStatuses.Bought;


			//mark this avatar as selected in UI
			markAsSelected();

			//Set the currentAvatar to the avatar id that's currently selected and make it selected in player prefs
			AvatarSelector.instance.currentAvatar = kittyGameObject.GetComponent<KittyAvatar> ().avatarId;
			PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_avatar"+AvatarSelector.instance.currentAvatar+"_status", 1);
			AvatarSelector.instance.avatars [AvatarSelector.instance.currentAvatar].GetComponent<AvatarObject>().kittyGameObject.GetComponent<KittyAvatar> ().status = KittyAvatar.KittyStatuses.Selected;

			GameManager.instance.kitty = kittyGameObject;
			Debug.Log ("current avatar = " + AvatarSelector.instance.currentAvatar);
			GameManager.instance.ChangeAvatar ();

			if (Application.platform == RuntimePlatform.Android) {
				//Show success popup
				ShowAvatarChangedDialog ();
			}
			else if (Application.platform == RuntimePlatform.WindowsEditor) {
				Debug.Log ("Avatar changed successfully.");
			}
		}
		else if (kittyGameObject.GetComponent<KittyAvatar>().status == KittyAvatar.KittyStatuses.Selected) {
			if (Application.platform == RuntimePlatform.Android)
				ShowAlreadySelectedDialog ();
			else if (Application.platform == RuntimePlatform.WindowsEditor)
				Debug.Log ("Avatar already Selected");
		} 
		else if (kittyGameObject.GetComponent<KittyAvatar>().status == KittyAvatar.KittyStatuses.Locked) {
			//Buy if u can, else show not enough fishes dialog
			if (Application.platform == RuntimePlatform.Android) {
				if (GameManager.instance.fishCount >= kittyGameObject.GetComponent<KittyAvatar> ().price) { 
					ShowBuyAvatarPopUp ();
				} else {
					NotEnoughDialog ();
				}	
			}
			else if(Application.platform == RuntimePlatform.WindowsEditor) {
				if (GameManager.instance.fishCount >= kittyGameObject.GetComponent<KittyAvatar> ().price)
					BuyAvatar ();
				else
					Debug.Log ("Not enough fishes !!");
			}
		}
	}

	//Buy Avatar PopUp
	public void ShowBuyAvatarPopUp(){
		MobileNativeDialog buyAvatarDialog = new MobileNativeDialog ("Buy Avatar", "Are you sure, you want to buy this kitty ?");
		buyAvatarDialog.OnComplete += OnBuyAvatarDialogClose;
	}

	//BuyAvatarDialog PopUp: result checker
	private void OnBuyAvatarDialogClose(MNDialogResult result){
		switch (result) {
		case MNDialogResult.YES:
			BuyAvatar();
			break;
		case MNDialogResult.NO:
			break;
		}
	}

	public void BuyAvatar(){
		//Deduct the price from the fishCount var and save it to player prefs
		GameManager.instance.fishCount -= kittyGameObject.GetComponent<KittyAvatar> ().price;
		PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_fishCount", GameManager.instance.fishCount);

		//Set fish count text in avatarStoreUI
		AvatarSelector.instance.avatarStoreFishCountText.text = GameManager.instance.fishCount.ToString();

		//unselect previously selected avatar in UI
		AvatarSelector.instance.avatars [AvatarSelector.instance.currentAvatar].GetComponent<AvatarObject>().markAsBought();

		//unselect the previously selected avatar in player prefs
		PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_avatar"+AvatarSelector.instance.currentAvatar+"_status", 0);
		AvatarSelector.instance.avatars [AvatarSelector.instance.currentAvatar].GetComponent<AvatarObject>().kittyGameObject.GetComponent<KittyAvatar> ().status = KittyAvatar.KittyStatuses.Bought;


		//mark this avatar as selected in UI
		markAsSelected();

		//Set the currentAvatar to the avatar id that's currently selected and make it selected in player prefs
		AvatarSelector.instance.currentAvatar = kittyGameObject.GetComponent<KittyAvatar> ().avatarId;
		PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_avatar"+AvatarSelector.instance.currentAvatar+"_status", 1);
		AvatarSelector.instance.avatars [AvatarSelector.instance.currentAvatar].GetComponent<AvatarObject>().kittyGameObject.GetComponent<KittyAvatar> ().status = KittyAvatar.KittyStatuses.Selected;

		GameManager.instance.kitty = kittyGameObject;
		Debug.Log ("current avatar = " + AvatarSelector.instance.currentAvatar);
		GameManager.instance.ChangeAvatar ();

		//Show success popup
		if (Application.platform == RuntimePlatform.Android)
			ShowAvatarChangedDialog ();
		else if (Application.platform == RuntimePlatform.WindowsEditor)
			Debug.Log ("Avatar changed successfully");


		//Save the avatarsBought in player prefs and
		//Unlock avatar Based achievements if they has been
		AvatarSelector.instance.avatarsBought++;
		PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_avatarsBought", AvatarSelector.instance.avatarsBought);

		if (AvatarSelector.instance.avatarsBought >= 1) {
			GooglePlayManager.instance.UnlockAchievement (GPConstants.GPGSIds.achievement_spender);
		}
		if (AvatarSelector.instance.avatarsBought == AvatarSelector.instance.avatars.Length - 1) {
			GooglePlayManager.instance.UnlockAchievement (GPConstants.GPGSIds.achievement_kitty_lover);
		}

	}

	public void ShowAvatarChangedDialog(){
		new MobileNativeMessage("Kitty changed successfully", "");
	}

	public void ShowAlreadySelectedDialog(){
		new MobileNativeMessage("Kitty already slected", "");
	}

	public void NotEnoughDialog(){
		new MobileNativeMessage("Not enough fishes !", "");
	}
}
