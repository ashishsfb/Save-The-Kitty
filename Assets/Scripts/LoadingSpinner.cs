using UnityEngine;
using System.Collections;

public class LoadingSpinner : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine( GetComponent<Animation>().Play("spinAnimation", false, () => Debug.Log("onComplete")) );
	}
}
