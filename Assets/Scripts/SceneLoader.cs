using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
	
	public Canvas LoadingCanvas;
	public Text GameOnStudiosText;
	public GameObject loadingIcon;

	// Use this for initialization
	void Start () {
		LoadingCanvas.gameObject.SetActive(false);
		loadingIcon.SetActive (false);
		GameOnStudiosText = this.GetComponent<Text> ();
	}

	/*
	 * This function runs through animation event in game on studio intro anmation
	 * This function shows loading canvas
	 * Shows loading icon
	 * Actually loads the gameScene
	 */
	public void LoadLoadingCanvas(){
		LoadingCanvas.gameObject.SetActive(true);

		//setting opacity to 0 for gameOnStudiosText
		Color newColor = GameOnStudiosText.color;
		newColor.a = 0;
		GameOnStudiosText.color = newColor;

		loadingIcon.SetActive (true);
		StartCoroutine (LoadGameScene());
	}

	IEnumerator LoadGameScene(){

		yield return new WaitForSeconds(0);

		AsyncOperation async = SceneManager.LoadSceneAsync ("gameScene");

		//while async operation loads the scene continue waiting
		while(!async.isDone){
			yield return null;
		}
	}
}
