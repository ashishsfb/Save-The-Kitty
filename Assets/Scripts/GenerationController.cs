using UnityEngine;
using System.Collections;

public class GenerationController : MonoBehaviour {
	
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("obstacleGeneratorTrigger")) {
			//Debug.Log ("GenerationController OnTriggerEnter obj");
			ObstacleGenerator.instance.generateObstacle ();
		}
		if (other.gameObject.CompareTag ("obstacleGroupGeneratorTrigger")) {
			//Debug.Log ("GenerationController OnTriggerEnter grp");
			ObstacleGenerator.instance.generateObstacleGroup();
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.gameObject.CompareTag ("obstacleGeneratorTrigger")) {
			//Debug.Log ("GenerationController OnTriggerExit obj");
			Destroy (other.gameObject);
		}
		if (other.gameObject.CompareTag ("obstacleGroupGeneratorTrigger")) {
			//Debug.Log ("GenerationController OnTriggerExit grp");
			Destroy (other.gameObject);
		}
	}
}
