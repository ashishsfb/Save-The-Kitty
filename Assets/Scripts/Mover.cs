using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Mover: MonoBehaviour {

	public static float speed = 1;

	void FixedUpdate () {
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}
}
