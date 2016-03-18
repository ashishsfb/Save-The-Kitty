using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GooglePlayManager : MonoBehaviour {

	public static GooglePlayManager instance;
	public Text loginInfoText;

	public string currentAccount, userName;
	public GameObject loadingSpinner, playButton;

	public void Awake(){
		Debug.Log ("GooglePlayManager : AWAKE");
		GooglePlayManager.instance = this;
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate ();

		//check who was logged in earlier to set currentAccount
		if (!PlayerPrefs.HasKey ("currentAccount")) {
			PlayerPrefs.SetString ("currentAccount", "Local");
			currentAccount = "Local";
			Debug.Log ("currentAccount = " + currentAccount);
		}
		else {
			currentAccount = PlayerPrefs.GetString ("currentAccount");
			Debug.Log ("currentAccount = " + currentAccount);
		}

		if (!PlayerPrefs.HasKey ("userName")) {
			PlayerPrefs.SetString ("userName", "Local");
			userName = "Local";
		}
		else {
			userName = PlayerPrefs.GetString ("userName");
		}
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("GooglePlayManager : START");
		LogIn ();
	}

	public void Update(){
		if (!Social.localUser.authenticated) {
			if(!userName.Equals("Local"))
				loginInfoText.text = "Logged in as: " +  userName + "(Locally)";
			else	
				loginInfoText.text = "Login to connect with Google Play Services";
		}
	}

	/// <summary>
	/// Login In Into Your Google+ Account
	/// </summary>
	public void LogIn (){
		Debug.Log ("GooglePlayManager : LOGIN");
		Social.localUser.Authenticate ((bool success) =>
		{
			if (success) {
				//if previous was a Local account then transfer all that to this account
				if(currentAccount.Equals("Local"))
						GameManager.instance.transferLocalToUser(Social.localUser.id);

				//set currentAccount
				currentAccount = Social.localUser.id;
				userName = Social.localUser.userName;

				PlayerPrefs.SetString("currentAccount", currentAccount);
				PlayerPrefs.SetString("userName", userName);
				
				Debug.Log ("currentAccount = " + currentAccount);

				//Set LoginInfoText
				loginInfoText.text = "Logged in as: " +  userName;
				Debug.Log("Google Play Services : Login Success !");
			} 
			else {
				//Set LoginInfoText
				if(!userName.Equals("Local"))
					loginInfoText.text = "Logged in as: " +  userName + "(Locally)";
				else	
					loginInfoText.text = "Login to connect with Google Play Services";
				
				Debug.Log("Google Play Services : Login Failed !");
			}
			AvatarSelector.instance.setUp ();
			GameManager.instance.setUp ();

			loadingSpinner.SetActive(false);
			playButton.SetActive(true);

			//upload best score as user signs in
			OnAddScoreToLeaderBorad();
			//check for unlocked achievements as user signs in
			checkAchievements();
		});
	}

	public void OnAddScoreToLeaderBorad ()
	{
		if (Social.localUser.authenticated) {
			Social.ReportScore (GameManager.instance.bestScore, GPConstants.GPGSIds.leaderboard_best_score, (bool success) =>
				{
					if (success) {
						Debug.Log("Update Score Success");

					} else {
						Debug.Log("Update Score Fail");

					}
				});
		}
	}

	public void OnShowLeaderBoard (){
		if (!Social.Active.localUser.authenticated) {
			LogIn ();
		}
		else {
			((PlayGamesPlatform)Social.Active).ShowLeaderboardUI (GPConstants.GPGSIds.leaderboard_best_score); // Show current (Active) leaderboard
		}
	}

	public void UnlockAchievement(string achievementStr){
		if (Social.localUser.authenticated){
			Social.ReportProgress(achievementStr, 100.0f, (bool success) =>
				{
					if (success)
					{
						Debug.Log("Achievement unlocked successfully");
					}
					else
					{
						Debug.Log("Achievement unlock failed");
					}
				});
		}
	}

	public void ShowAchievements(){
		if (!Social.localUser.authenticated) {
			LogIn ();
		} else {
			Social.ShowAchievementsUI();
		}
	}

	public void checkAchievements(){
		if (GameManager.instance.gamesPlayed >= 10)
			UnlockAchievement (GPConstants.GPGSIds.achievement_10_games);
		if(GameManager.instance.gamesPlayed >= 50)
			UnlockAchievement (GPConstants.GPGSIds.achievement_50_games);
		if(GameManager.instance.gamesPlayed >= 100)
			UnlockAchievement (GPConstants.GPGSIds.achievement_100_games);

		if(AvatarSelector.instance.avatarsBought >= 1)
			UnlockAchievement (GPConstants.GPGSIds.achievement_spender);
		if(AvatarSelector.instance.avatarsBought == AvatarSelector.instance.avatars.Length - 1)
			UnlockAchievement (GPConstants.GPGSIds.achievement_kitty_lover);
	}
}
