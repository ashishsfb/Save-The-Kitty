using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	//scoring
	public int crushedCount, score, multiplier, milestone;
	public int fishCount, gameFishCount, bestScore;

	//inGame UI Texts
	public Text scoreText, multiplierText;

	//GameOver UI Texts
	public Text gameOverScoreText, gameOverBestScoreText, gameOverFishCountText;

	//MainMenu UI Texts
	public Text mainMenuBestScoreText, mainMenuFishCountText;

	//game states : MainMenu, Play, Paused, GameOver
	public enum gameStates {MainMenu, Playing, Paused, GameOver};
	public gameStates gameState = gameStates.MainMenu;

	//GameCavases
	public Canvas mainMenuCanvas, inGameCanvas, pauseCanvas, gameOverCanvas, avatarStoreCanvas;

	//Kitty gameObject
	public GameObject kitty;

	public Animator crusherAnimator;

	//CONSOLE >>JUST FOR TESTING
	public Text consoleText;

	//Sets game state to MainMenu
	void Start(){
		Debug.Log ("Game Manager Start");
		instance = this;

		//set the fishCount in playerPrefs if it doesn't exists 
		//else get its value : LATER IN MAIN MENU func
		if (!PlayerPrefs.HasKey ("fishCount")) {
			fishCount = 0;
			PlayerPrefs.SetInt ("fishCount", 0);
		}

		//set the bestScore in playerPrefs if it doesn't exists 
		//else get its value : LATER IN MAIN MENU func
		if (!PlayerPrefs.HasKey ("bestScore")) {
			bestScore = 0;
			PlayerPrefs.SetInt ("bestScore", 0);
		}

		MainMenu ();
	}

	//Handles back button key-press
	void Update(){
		//If back key is presses
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Debug.Log ("Escaped");
			switch (GameManager.instance.gameState) {
			case GameManager.gameStates.Playing: 
				PauseGame ();
				break;

			case GameManager.gameStates.Paused: 
				ResumeGame ();
				break;

			case GameManager.gameStates.MainMenu: 
				ShowQuitGamePopUp ();
				break;

			case GameManager.gameStates.GameOver: 
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
	}

	//After every obstacle is crushed
	public void IncreaseScore(){
		//Debug.Log ("Game Manager Inc Score");
		if (crushedCount == milestone && multiplier < 10) {
			milestone =  milestone*2;
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
		gameState = gameStates.MainMenu;

		//set bestScore and fishCount UI Texts
		bestScore = PlayerPrefs.GetInt("bestScore");
		fishCount = PlayerPrefs.GetInt("fishCount");

		mainMenuBestScoreText.text = "Best Score " + bestScore.ToString ();
		mainMenuFishCountText.text = fishCount.ToString ();

		Time.timeScale = 0;
	}

	public void ChangeAvatar(){
		Destroy(KittyController.instance.gameObject);
		Instantiate (kitty);
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

		//Set score and fish count in UI and Vars
		gameOverScoreText.text = score.ToString ();
		fishCount += gameFishCount;
		gameOverFishCountText.text = fishCount.ToString ();
		if (score > bestScore) {
			bestScore = score;
		}
		gameOverBestScoreText.text = "Best Score " + bestScore.ToString();

		//Set BestScore and Fish Cont in Player Prefs
		PlayerPrefs.SetInt ("bestScore", bestScore);
		PlayerPrefs.SetInt ("fishCount", fishCount);

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
		mainMenuCanvas.gameObject.SetActive (false);
		inGameCanvas.gameObject.SetActive (true);
		gameState = gameStates.Playing;
		Time.timeScale = 1;

		ChangeAvatar ();
		ObstacleGenerator.instance.StartGenerator();
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
		Mover.speed = 1;
		ConveyerBeltMover.instance.speed = 0.1f;
		crusherAnimator.speed = 1;

		//start game with freezed time as we don't want things to move while in main menu state
		Time.timeScale = 0;

		//SET BACK THE GAME STATE TO MAIN MENU
		if(gameState != gameStates.GameOver)
			gameState = gameStates.MainMenu;

		//SET SCORE RELATED COUNTS BACK TO ORIGINAL VALUES
		score = 0;
		gameFishCount = 0;
		multiplier = 1;
		milestone = 5;
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
