using UnityEngine;
using System.Collections;

public class KittyAvatar : MonoBehaviour {
	public string avatarName = "Avatar Name Here";
	public int avatarId = 1;
	public int cost = 100;
	public bool isLokced = true;
	public string description;

	public GameObject kittyGameObject;

	void Start(){
		kittyGameObject = this.gameObject;
	}

	public void SelectAvatar(){
		Debug.Log ("Select Avatar called");

		AvatarSelector.instance.currentAvatar = this.gameObject.GetComponent<KittyAvatar> ().avatarId;
		GameManager.instance.kitty = AvatarSelector.instance.avatars [AvatarSelector.instance.currentAvatar];
		Debug.Log ("current avatar = " + AvatarSelector.instance.currentAvatar);
		GameManager.instance.ChangeAvatar ();
	}
}
