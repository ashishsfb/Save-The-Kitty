using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class Share : MonoBehaviour
{
	public Texture2D saveTheKittyIcon;
	private bool isProcessing = false;

	public void ShareGame(){
		if (!isProcessing) {
			StartCoroutine (ShareScreenshot ());
		}
	}

	public IEnumerator ShareScreenshot(){
		isProcessing = true;

		// wait for graphics to render
		yield return new WaitForEndOfFrame();

		//save your image on a designated path
		byte[] bytes = saveTheKittyIcon.EncodeToPNG ();
		string path = Application.persistentDataPath + "/SaveTheCat.png";
		File.WriteAllBytes (path, bytes);

		AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent");

		Debug.Log ("intent class, intentobject done");

		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		Debug.Log ("setAction done");

		intentObject.Call<AndroidJavaObject>("setType", "image/*");
		Debug.Log ("setType done");

		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Download and compete with your friends !!");
		Debug.Log ("extra_sub done");

		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Save The Cat");
		Debug.Log ("extra_title done");

		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Download Save the cat https://play.google.com/apps/testing/com.GameOnStudio.KittyTrouble and beat my Best-score : " + GameManager.instance.bestScore);
		Debug.Log ("extra_text done");

		Debug.Log ("put extra FULLY done :)");

		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		//AndroidJavaClass fileClass = new AndroidJavaClass("java.io.File");

		Debug.Log ("uri class done");

		AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", path);// Set Image Path Here
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);

		Debug.Log ("file object, uri object done");

		bool fileExist = fileObject.Call<bool>("exists");
		Debug.Log("File exist : " + fileExist);

		// Attach image to intent
		if (fileExist)
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

		Debug.Log ("extra_steam done");

		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

		Debug.Log ("extra_unity currentActivity done");

		//currentActivity.Call("startActivity", intentObject);
		AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share via");
		currentActivity.Call("startActivity", jChooser);
		isProcessing = false;

		this.gameObject.GetComponent<Button> ().interactable = true;
	}
}
