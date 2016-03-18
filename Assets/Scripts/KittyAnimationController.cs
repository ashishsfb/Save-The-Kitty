using UnityEngine;
using System.Collections;

public class KittyAnimationController : MonoBehaviour {

	public static KittyAnimationController instance;

	//Vars controlling Kitty's Animation
	public Animator kittyAnimator, kittyRunAnimator, kittyGallopAnimator;
	public GameObject runningSprite, gallopingSprite;

	public int lane = 0;

	//vars controlling swipe movements
	Vector2 startPos, endPos;
	bool isSwipe;
	float minSwipeDistance;

	BoxCollider kittyCollider;
	AudioSource meowSource;

	void Awake(){
		Debug.Log ("KittyAnimationController: AWAKE");
		instance = this;
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("KittyAnimationController: START");
		kittyCollider = KittyController.instance.GetComponent<BoxCollider> ();
		meowSource = GetComponent<AudioSource>();

		/*
		kittyAnimator = this.GetComponent<Animator> ();
		kittyRunAnimator = transform.GetChild (0).GetComponent<Animator> ();
		kittyGallopAnimator = transform.GetChild (1).GetComponent<Animator> ();
		*/
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.instance.gameState == GameManager.gameStates.Playing){
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
			if (Input.GetKeyUp (KeyCode.LeftArrow) && lane > -1 && KittyController.instance.canMoveLeft) {
				lane--;
				kittyAnimator.SetInteger ("lane", lane);
				kittyRunAnimator.SetInteger ("lane", lane);
				kittyGallopAnimator.SetInteger ("lane", lane);
				kittyCollider.center = new Vector3(lane, 0.5f, kittyCollider.center.z);
			} 
			else if (Input.GetKeyUp (KeyCode.LeftArrow)) {
				if(!meowSource.isPlaying)
					meowSource.PlayOneShot(meowSource.clip);
			}

			if (Input.GetKeyUp (KeyCode.RightArrow) && lane < 1 && KittyController.instance.canMoveRight) {
				lane++;
				kittyAnimator.SetInteger ("lane", lane);
				kittyRunAnimator.SetInteger ("lane", lane);
				kittyGallopAnimator.SetInteger("lane", lane);
				kittyCollider.center = new Vector3(lane, 0.5f, kittyCollider.center.z);
			}
			else if (Input.GetKeyUp (KeyCode.RightArrow)) {
				if(!meowSource.isPlaying)
					meowSource.PlayOneShot(meowSource.clip);
			}
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
				if (lane < 1 && KittyController.instance.canMoveRight) {
					lane++;
					kittyRunAnimator.SetInteger ("lane", lane);
					kittyGallopAnimator.SetInteger("lane", lane);
					kittyAnimator.SetInteger ("lane", lane);
					kittyCollider.center = new Vector3(lane, 0.5f, kittyCollider.center.z);
				}
				else {
					if(!meowSource.isPlaying)
						meowSource.PlayOneShot(meowSource.clip);
				}
			}
			//left swipe
			else {
				if(lane > -1 && KittyController.instance.canMoveLeft){
					lane--;
					kittyRunAnimator.SetInteger ("lane", lane);
					kittyGallopAnimator.SetInteger("lane", lane);
					kittyAnimator.SetInteger ("lane", lane);
					kittyCollider.center = new Vector3(lane, 0.5f, kittyCollider.center.z);
				}
				else {
					if(!meowSource.isPlaying)
						meowSource.PlayOneShot(meowSource.clip);
				}
			}
		}
	}
}