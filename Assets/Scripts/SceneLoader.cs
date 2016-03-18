using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (LoadGameScene());
	}

	IEnumerator LoadGameScene(){
		yield return new WaitForSeconds(0);

		AsyncOperation async = SceneManager.LoadSceneAsync ("GameScene");

		//while async operation loads the scene continue waiting
		while(!async.isDone){
			yield return null;
		}
	}
}
