using UnityEngine;
using System.Collections;

public class KittyController : MonoBehaviour {

	public static KittyController instance;

	//booleans controlling cat movement
	public bool canMoveLeft, canMoveRight, inDanzerZone, gettingPushed, eatenPowerPellet;

	// Use this for initialization
	void Start () {
		instance = this;

		canMoveLeft = true;
		canMoveRight = true;
		inDanzerZone = false;
		eatenPowerPellet = false;
		gettingPushed = false;
	}
	
	// Update is called once per frame
	void Update () {
		//check if eatenPowerPellet
		if (eatenPowerPellet && !gettingPushed) {
			float step = Mover.speed * Time.deltaTime;
			transform.Translate (-Vector3.forward * step);
		}
	}

	//pushing kitty with obstacles
	void OnTriggerStay(Collider other){
		//if kitty stays with pushers: push kitty with same speed
		if (other.tag.Equals ("Pusher")) {
			gettingPushed = true;
			float step = Mover.speed * Time.deltaTime;
			transform.Translate (Vector3.forward * step);
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag.Equals ("Left")) {
			canMoveRight = false;
		}

		if (other.gameObject.tag.Equals ("Right")) {
			canMoveLeft= false;
		}

		if (other.gameObject.tag.Equals("kittyCrusher")) {
			Destroy (this.gameObject);
			GameManager.instance.GameOver ();
		}

		if (other.gameObject.tag.Equals("fish")) {
			Destroy (other.gameObject);
			GameManager.instance.gameFishCount++;
			AudioManager.instance.playFishCollectionSound ();
		}

		if (other.gameObject.tag.Equals ("dangerZone")) {
			inDanzerZone = true;
		}

		if (other.gameObject.tag.Equals ("powerPellet")) {
			Destroy (other.gameObject);
			eatenPowerPellet = true;
			ObstacleGenerator.instance.generatedPowerPellet = false;

			AudioManager.instance.playPowerPelletCollectionSound ();

			KittyAnimationController.instance.gallopingSprite.GetComponent<SpriteRenderer> ().enabled = true;
			KittyAnimationController.instance.runningSprite.GetComponent<SpriteRenderer> ().enabled = false;

			Invoke ("ResetPowerPellet", 2);
		}

	}
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag.Equals ("Left")) {
			canMoveRight= true;
		}

		if (other.gameObject.tag.Equals ("Right")) {
			canMoveLeft= true;
		}

		if (other.gameObject.tag.Equals ("dangerZone")) {
			inDanzerZone = false;
		}

		if (other.tag.Equals ("Pusher")) {
			gettingPushed = false;
		}
	}

	void ResetPowerPellet(){
		eatenPowerPellet = false;
		KittyAnimationController.instance.gallopingSprite.GetComponent<SpriteRenderer> ().enabled = false;
		KittyAnimationController.instance.runningSprite.GetComponent<SpriteRenderer> ().enabled = true;
	}
}
