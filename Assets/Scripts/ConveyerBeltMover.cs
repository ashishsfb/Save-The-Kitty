using UnityEngine;
using System.Collections;

public class ConveyerBeltMover : MonoBehaviour {

	public static ConveyerBeltMover instance;
	public float speed = 0.1F;
	public Renderer beltRenderer;

	void Start() {
		instance = this;
		beltRenderer = GetComponent<Renderer>();
	}

	void Update() {
		float offset = Time.time * speed;
		beltRenderer.material.mainTextureOffset = new Vector2(0, offset);
	}
}
