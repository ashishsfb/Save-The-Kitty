using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	//scoring
	public int crushedCount, score, multiplier, milestone;
	public int fishCount, gameFishCount, bestScore, gamesPlayed;

	//inGame UI Texts
	public Text scoreText, multiplierText;

	//GameOver UI Texts
	public Text gameOverScoreText, gameOverBestScoreText, gameOverFishCountText;

	//MainMenu UI Texts
	public Text mainMenuBestScoreText, mainMenuFishCountText;

	//game states : MainMenu, Play, Paused, GameOver
	public enum gameStates {MainMenu, AvatarStore, Credits, Tutorials, Playing, Paused, GameOver};
	public gameStates gameState = gameStates.MainMenu;

	//GameCavases
	public Canvas mainMenuCanvas, inGameCanvas, pauseCanvas, gameOverCanvas, avatarStoreCanvas, creditsCanvas, tutsCanvas;

	//Kitty gameObject
	public GameObject kitty;

	public Animator crusherAnimator, multiplierAnimator;

	//CONSOLE >>JUST FOR TESTING
	public Text consoleText;

	void Awake(){
		Debug.Log ("GameManager : AWAKE");
		instance = this;
	}

	//Sets game state to MainMenu
	void Start(){
		Debug.Log ("GameManager Start");
		//Instantiate the cuurentKittyAvatar from avatar selector itself rather from here
		//kitty = AvatarSelector.instance.avatars [AvatarSelector.instance.currentAvatar].GetComponent<AvatarObject>().kittyGameObject;
		//Instantiate (kitty);
	}

	public void setUp (){
		Debug.Log ("GameManager : SetUp");
		//set the fishCount in playerPrefs if it doesn't exists 
		//else get its value 
		if (!PlayerPrefs.HasKey (GooglePlayManager.instance.currentAccount + "_fishCount")) {
			PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_fishCount", 0);
			fishCount = 0;
		}
		else {
			fishCount = PlayerPrefs.GetInt (GooglePlayManager.instance.currentAccount + "_fishCount");
		}

		//Set fish count text of avatar store
		AvatarSelector.instance.avatarStoreFishCountText.text = fishCount.ToString();

		//set the bestScore in playerPrefs if it doesn't exists 
		//else get its value 
		if (!PlayerPrefs.HasKey (GooglePlayManager.instance.currentAccount + "_bestScore")) {
			PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_bestScore", 0);
			bestScore = 0;
		}
		else {
			bestScore = PlayerPrefs.GetInt (GooglePlayManager.instance.currentAccount + "_bestScore");
		}

		//set the gamesPlayed in playerPrefs if it doesn't exists 
		//else get its value
		if (!PlayerPrefs.HasKey (GooglePlayManager.instance.currentAccount + "_gamesPlayed")) {
			PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_gamesPlayed", 0);
			gamesPlayed = 0;
		}
		else {
			gamesPlayed = PlayerPrefs.GetInt (GooglePlayManager.instance.currentAccount + "_gamesPlayed");
		}
		MainMenu ();
	}

	public void transferLocalToUser(string currentAccount){
		//set avatars statuses
		for(int i = 0; i < AvatarSelector.instance.avatars.Length; i++){
			PlayerPrefs.SetInt (currentAccount + "_avatar" + i.ToString () + "_status", PlayerPrefs.GetInt ("Local_avatar" + i.ToString () + "_status"));
		}
		//set fishCount, bestScore, avatarsBought and gamesPlayed
		PlayerPrefs.SetInt (currentAccount + "_fishCount", PlayerPrefs.GetInt("Local_fishCount"));
		PlayerPrefs.SetInt (currentAccount + "_bestScore", PlayerPrefs.GetInt("Local_bestScore"));
		PlayerPrefs.SetInt (currentAccount + "_avatarsBought", PlayerPrefs.GetInt("Local_avatarsBought"));
		PlayerPrefs.SetInt (currentAccount + "_gamesPlayed", PlayerPrefs.GetInt("Local_gamesPlayed"));
	}

	//Handles back button key-press
	void Update(){
		//If back key is presses
		if (Input.GetKeyDown (KeyCode.Escape)) {
			switch (GameManager.instance.gameState) {
			case GameManager.gameStates.Playing: 
				PauseGame ();
				break;

			case GameManager.gameStates.Paused: 
				ResumeGame ();
				break;

			case GameManager.gameStates.MainMenu: 
				ShowQuitGamePopUp ();
				//AndroidDialogAndToastBinding.instance.dialogBoxWithTwoButtons("Exit Game", "Are you sure you want to exit the game?", "Yes", "No", "test_dialog_icon", "Tag 002");
				break;

			case GameManager.gameStates.GameOver: 
				MainMenu ();
				break;

			case GameManager.gameStates.AvatarStore: 
				MainMenu ();
				break;
			}
		}
	}

	//After every milestone completes: call IncreaseGameSpeed
	public void IncreaseGameSpeed(){
		//Debug.Log ("Game Manager Inc Game Speed");
		Mover.speed += 0.2f;
		ConveyerBeltMover.instance.speed += 0.02f;
		crusherAnimator.speed += 0.1f;
		AudioManager.instance.crusherSource.pitch += 0.05f;

		//Play multiplier inc animation
		multiplierAnimator.SetTrigger("inc");
	}

	//After every obstacle is crushed
	public void IncreaseScore(){
		//Debug.Log ("Game Manager Inc Score");
		if (crushedCount == milestone && multiplier < 30) {
			milestone =  milestone + 10;
			multiplier++;
			multiplierText.text = "x" + multiplier.ToString ();
			IncreaseGameSpeed ();
		}
		score = score + multiplier;
		scoreText.text = score.ToString ();

		//ONLY FOR TESTING
		string consoleString = "Next milestone = " + milestone;
		consoleString += "       speed = " + Mover.speed;
		consoleString += "       count = " + crushedCount;
		consoleText.text = consoleString;
	}

	//Sets game state to MainMenu
	public void MainMenu(){
		if (gameState == gameStates.Paused) {
			//Destroy(CatController.instance.gameObject);
			Destroy(KittyController.instance.gameObject);
			Instantiate (kitty);
		}
		resetGame ();

		//Debug.Log ("Game Manager MainMenu");
		gameOverCanvas.gameObject.SetActive (false);
		pauseCanvas.gameObject.SetActive (false);
		avatarStoreCanvas.gameObject.SetActive (false);
		mainMenuCanvas.gameObject.SetActive (true);
		creditsCanvas.gameObject.SetActive (false);
		gameState = gameStates.MainMenu;

		//set bestScore and fishCount UI Texts
		bestScore = PlayerPrefs.GetInt(GooglePlayManager.instance.currentAccount + "_bestScore");
		fishCount = PlayerPrefs.GetInt(GooglePlayManager.instance.currentAccount + "_fishCount");

		mainMenuBestScoreText.text = "Best Score " + bestScore.ToString ();
		mainMenuFishCountText.text = fishCount.ToString ();
		AvatarSelector.instance.avatarStoreFishCountText.text = fishCount.ToString ();
		Time.timeScale = 0;
	}

	public void ChangeAvatar(){
		if(KittyController.instance != null)
			Destroy(KittyController.instance.gameObject);

		kitty = AvatarSelector.instance.avatars [AvatarSelector.instance.currentAvatar].GetComponent<AvatarObject>().kittyGameObject;
		Instantiate (kitty);
	}

	public void CreditsPage(){
		mainMenuCanvas.gameObject.SetActive (false);
		creditsCanvas.gameObject.SetActive (true);
		gameState = gameStates.Credits;
	}

	public void TutsPage(){
		mainMenuCanvas.gameObject.SetActive (false);
		tutsCanvas.gameObject.SetActive (true);
		gameState = gameStates.Tutorials;
	}

	//As kitty gets destroyed when crushed thus reinstantiate it after game over
	//Sets game state to GameOver
	public void GameOver(){
		//Debug.Log ("Game Manager GameOver");
		Debug.Log("Game score = " + score);
		Debug.Log("Multiplier = " + multiplier);
		Debug.Log("Next milestone = " + milestone);
		Debug.Log("crushed count = " + crushedCount);

		inGameCanvas.gameObject.SetActive (false);
		gameOverCanvas.gameObject.SetActive (true);
		gameState = gameStates.GameOver;

		//update the number of games played in var and PlayerPrefs THEN
		//Check for gamesPlayed based achievements
		gamesPlayed++;
		PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_gamesPlayed", gamesPlayed);
		if (gamesPlayed >= 10) {
			GooglePlayManager.instance.UnlockAchievement (GPConstants.GPGSIds.achievement_10_games);
		}
		if (gamesPlayed >= 50) {
			GooglePlayManager.instance.UnlockAchievement (GPConstants.GPGSIds.achievement_50_games);
		}
		if (gamesPlayed >= 100) {
			GooglePlayManager.instance.UnlockAchievement (GPConstants.GPGSIds.achievement_100_games);
		}

		//Set score and fish count in UI and Vars
		gameOverScoreText.text = score.ToString ();
		fishCount += gameFishCount;
		gameOverFishCountText.text = fishCount.ToString ();

		//if high score is made : update bestScore and add it to leaderboards
		if (score > bestScore) {
			bestScore = score;

			GooglePlayManager.instance.OnAddScoreToLeaderBorad ();
		}
		gameOverBestScoreText.text = "Best Score " + bestScore.ToString();

		//Set BestScore and Fish Cont in Player Prefs
		PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_bestScore", bestScore);
		PlayerPrefs.SetInt (GooglePlayManager.instance.currentAccount + "_fishCount", fishCount);

		resetGame ();
		Time.timeScale = 0;
		Instantiate (kitty);
	}

	//Sets game state to Playing
	public void RestartGame(){
		//Debug.Log ("Game Manager RestartGame");
		gameOverCanvas.gameObject.SetActive (false);
		inGameCanvas.gameObject.SetActive (true);
		Time.timeScale = 1;
		gameState = gameStates.Playing;
		ObstacleGenerator.instance.StartGenerator();
	}

	//Sets game state to Paused
	public void PauseGame(){
		//Debug.Log ("Game Manager PauseGame");
		inGameCanvas.gameObject.SetActive (false);
		pauseCanvas.gameObject.SetActive (true);
		gameState = gameStates.Paused;
		Time.timeScale = 0;
	}

	//Sets game state to Playing
	public void StartGame(){
		//Debug.Log ("Game Manager StartGame");
		avatarStoreCanvas.gameObject.SetActive(false);
		mainMenuCanvas.gameObject.SetActive (false);
		inGameCanvas.gameObject.SetActive (true);
		gameState = gameStates.Playing;
		Time.timeScale = 1;

		ChangeAvatar ();
		ObstacleGenerator.instance.StartGenerator();

		AudioManager.instance.backgroundMusicSource.Play ();
		AudioManager.instance.crusherSource.Play ();
	}

	//Sets game state to Playing
	public void ResumeGame(){
		//Debug.Log ("Game Manager RestartGame");
		pauseCanvas.gameObject.SetActive (false);
		inGameCanvas.gameObject.SetActive (true);
		gameState = gameStates.Playing;
		Time.timeScale = 1;
	}

	//Sets game state to MainMenu
	public void resetGame(){
		//Debug.Log ("Game Manager resetGame");
		//RESET SPEEDS: obstacles, belt, crusher
		Mover.speed = 2;
		ConveyerBeltMover.instance.speed = 0.2f;
		crusherAnimator.speed = 1.5f;
		AudioManager.instance.crusherSource.pitch = 1.25f;

		//start game with freezed time as we don't want things to move while in main menu state
		Time.timeScale = 0;

		//SET BACK THE GAME STATE TO MAIN MENU
		if(gameState != gameStates.GameOver)
			gameState = gameStates.MainMenu;

		//SET SCORE RELATED COUNTS BACK TO ORIGINAL VALUES
		score = 0;
		gameFishCount = 0;
		multiplier = 1;
		milestone = 10;
		crushedCount = 0;

		if(AudioManager.instance != null)
			AudioManager.instance.crusherSource.pitch = 1;

		//SET THE IN GAME UI TEXTS
		scoreText.text = score.ToString ();
		multiplierText.text = "x" + multiplier.ToString ();

		//DESTROY ANY REMAINING OBSTACLES
		if (ObstacleGenerator.instance != null && ObstacleGenerator.instance.generatedObstacles != null && ObstacleGenerator.instance.generatedObstacles.Count > 0) {
			foreach(GameObject go in ObstacleGenerator.instance.generatedObstacles) {
				Destroy (go);
			}
		}

		//ONLY FOR TESTING
		string consoleString = "Next milestone = " + milestone;
		consoleString += "       speed = " + Mover.speed;
		consoleString += "       count = " + crushedCount;
		consoleText.text = consoleString;
	}

	public void OpenAvatarStore(){
		mainMenuCanvas.gameObject.SetActive (false);
		avatarStoreCanvas.gameObject.SetActive (true);

		gameState = gameStates.AvatarStore;

		//Set Avatar Store login info and GameOver login info
		if (Social.localUser.authenticated) {
			AvatarSelector.instance.avatarStoreLoginInfo.text = "Logged in as: " + GooglePlayManager.instance.userName;
			AvatarSelector.instance.gameOverLoginInfo.text = "Logged in as: " + GooglePlayManager.instance.userName;
		}
		else {
			if (GooglePlayManager.instance.userName.Equals ("Local")) {
				AvatarSelector.instance.avatarStoreLoginInfo.text = "Login to connect with Google Play Services";
				AvatarSelector.instance.gameOverLoginInfo.text = "Login to connect with Google Play Services";
			} else {
				AvatarSelector.instance.avatarStoreLoginInfo.text = "Logged in as: " +  GooglePlayManager.instance.userName + "(Locally)";
				AvatarSelector.instance.gameOverLoginInfo.text = "Logged in as: " +  GooglePlayManager.instance.userName + "(Locally)";
			}
		}
	}

	public void ComingSoon(){
		new MobileNativeMessage("Coming Soon...", "This feature is not yet complete.");
	}

	//Runs before game is quit
	void OnApplicationQuit(){
		Debug.Log ("Quiting application");
	}

	//Home PopUp
	public void ShowHomePopUp(){
		MobileNativeDialog homeDialog = new MobileNativeDialog ("Main Menu", "Are you sure? Your progress will be lost !");
		homeDialog .OnComplete += OnhomeDialogClose;
	}

	//Home PopUp: result checker
	private void OnhomeDialogClose(MNDialogResult result){
		switch (result) {
		case MNDialogResult.YES:
			MainMenu();
			break;
		case MNDialogResult.NO:
			break;
		}
	}

	//QuitGame PopUp
	public void ShowQuitGamePopUp(){
		MobileNativeDialog quitGameDialog = new MobileNativeDialog ("Exit Game", "Are you sure you want to exit the game?");
		quitGameDialog.OnComplete += OnQuitGameDialogClose;
	}

	//QuitGame PopUp: result checker
	private void OnQuitGameDialogClose(MNDialogResult result){
		switch (result) {
		case MNDialogResult.YES:
			QuitGame ();
			break;
		case MNDialogResult.NO:
			break;
		}
	}

	public void QuitGame(){
		if (gameState.Equals (gameStates.GameOver)) {
			PlayerPrefs.SetInt ("fishCount", fishCount);
			PlayerPrefs.SetInt ("bestScore", bestScore);
		}
		Application.Quit ();
	}
}
