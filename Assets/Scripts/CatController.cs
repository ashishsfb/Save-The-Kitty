using UnityEngine;
using System.Collections;

public class CatController : MonoBehaviour {
	public static CatController instance;

	//To inc kitty animation speed as game progresses
	public Animator kittyAnimator;
	//public GameObject middleSprite, leftSprite, rightSprite;
	public int lane;

	//vars controlling swipe movements
	Vector2 startPos, endPos;
	bool isSwipe;
	float minSwipeDistance;

	//booleans controlling cat movement
	public bool canMoveLeft, canMoveRight, inDanzerZone, gettingPushed, eatenPowerPellet;

	// Use this for initialization
	void Start () {
		Debug.Log ("CatController Start");
		instance = this;
		lane = 0;

		canMoveLeft = true;
		canMoveRight = true;
		inDanzerZone = false;
		eatenPowerPellet = false;
		gettingPushed = false;

		/*
		middleSprite = transform.FindChild ("middleSprite").gameObject;
		leftSprite = transform.FindChild ("leftSprite").gameObject;
		rightSprite = transform.FindChild ("rightSprite").gameObject;
		*/
	}

	//For input checking and movement
	void Update(){
		if (GameManager.instance.gameState == GameManager.gameStates.Playing) {
			//Enable BG music when in play mode and disable main menu BG source
			AudioManager.instance.backgroundMusicSource.UnPause();
			AudioManager.instance.crusherSource.UnPause ();
			AudioManager.instance.mainMenuBGSource.Pause ();

			//Check for touch inputs
			if (Input.touchCount > 0) {
				foreach (Touch touch in Input.touches) {
					switch (touch.phase) {
					case TouchPhase.Began:
						isSwipe = true;
						startPos = touch.position;
						break;

					case TouchPhase.Canceled:
						isSwipe = false;
						break;

					case TouchPhase.Moved:
						endPos = touch.position;
						movePlayer ();
						isSwipe = false;
						break;

					case TouchPhase.Ended:
						endPos = touch.position;
						movePlayer ();
						isSwipe = false;
						break;
					}
				}
			}

			//Check for Keyboard inputs
			//Left Keypress
			if (Input.GetKeyUp (KeyCode.LeftArrow) && lane > -1 && canMoveLeft) {
				lane--;
				kittyAnimator.SetTrigger ("moveLeft");
				transform.position = new Vector3 (transform.position.x - 1, transform.position.y, transform.position.z);

				//To set kitty's rotation
				//transform.Rotate (new Vector3 (0, 0, transform.rotation.z-5));
			}
			else if (Input.GetKeyUp (KeyCode.LeftArrow) && !(lane > -1 && canMoveLeft)) {
				AudioManager.instance.doMeow ();
			}

			//Right Keypress
			if (Input.GetKeyUp (KeyCode.RightArrow) && lane < 1 && canMoveRight) {
				lane++;
				kittyAnimator.SetTrigger ("moveRight");
				transform.position = new Vector3 (transform.position.x + 1, transform.position.y, transform.position.z);

				//To set kitty's rotation
				//transform.Rotate (new Vector3 (0, 0, transform.rotation.z+5));
			}
			else if (Input.GetKeyUp (KeyCode.RightArrow) && !(lane < 1 && canMoveRight)){
				AudioManager.instance.doMeow ();
			}

			//check if eatenPowerPellet
			if (eatenPowerPellet && !gettingPushed && transform.position.z >= -1) {
				float step = Mover.speed * Time.deltaTime;
				transform.Translate (-Vector3.forward * step);
			}

			/*
			//SET THE SPRITE ACC TO LANE
			if(lane == -1){
				leftSprite.SetActive (true);
				middleSprite.SetActive (false);
				rightSprite.SetActive (false);
			}
			if(lane == 0){
				leftSprite.SetActive (false);
				middleSprite.SetActive (true);
				rightSprite.SetActive (false);
			}
			if(lane == 1){
				leftSprite.SetActive (false);
				middleSprite.SetActive (false);
				rightSprite.SetActive (true);
			}
			*/
		}
		else {
			AudioManager.instance.backgroundMusicSource.Pause ();
			AudioManager.instance.crusherSource.Pause ();
			AudioManager.instance.mainMenuBGSource.UnPause ();
		}
	}

	void movePlayer(){
		//horizontal swipe
		if (isSwipe && (Mathf.Abs (endPos.x - startPos.x) > Mathf.Abs (endPos.y - startPos.y))) {
			//right swipe
			if (endPos.x > startPos.x) {
				if (lane < 1 && canMoveRight) {
					lane++;
					kittyAnimator.SetTrigger ("moveRight");
					transform.position = new Vector3 (transform.position.x + 1, transform.position.y, transform.position.z);

					//To set kitty's rotation
					//transform.Rotate (new Vector3 (0, 0, transform.rotation.z+5));
				}
				else {
					AudioManager.instance.doMeow ();
				}
			}
			//left swipe
			else {
				if(lane > -1 && canMoveLeft){
					lane--;
					kittyAnimator.SetTrigger ("moveLeft");
					transform.position  = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);

					//To set kitty's rotation
					//transform.Rotate (new Vector3 (0, 0, transform.rotation.z-5));

				}
				else {
					AudioManager.instance.doMeow ();
				}
			}
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

		if (other.gameObject.tag.Equals("Crusher")) {
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
			Invoke ("ResetPowerPellet", 2);
			AudioManager.instance.playPowerPelletCollectionSound ();
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
	}
}
