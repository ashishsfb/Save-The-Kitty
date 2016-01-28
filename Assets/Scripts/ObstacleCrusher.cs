using UnityEngine;
using System.Collections;

public class ObstacleCrusher : MonoBehaviour {
	
	void OnTriggerEnter(Collider other){
		//Debug.Log ("ObstacleCrusher OnTriggerEnter");
		if (other.gameObject.CompareTag ("Pusher")) {
			Destroy(other.transform.FindChild("Model").gameObject);
			GameManager.instance.crushedCount++;
			GameManager.instance.IncreaseScore ();
		}

		if(other.gameObject.CompareTag("fish") || other.gameObject.CompareTag("powerPellet")){
			Destroy (other.gameObject);

			if (other.gameObject.CompareTag("powerPellet"))
				ObstacleGenerator.instance.generatedPowerPellet = false;
		}
	}

	void OnTriggerExit(Collider other){
		//Debug.Log ("ObstacleCrusher OnTriggerExit");
		if (other.gameObject.CompareTag ("Pusher")) {
			Destroy (other.gameObject);
		}
	}
}
