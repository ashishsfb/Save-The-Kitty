using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioManager: MonoBehaviour {
	public static AudioManager instance;
	public AudioSource mainMenuBGSource, backgroundMusicSource, soundEffectsSource, crusherSource;
	public AudioClip meowSound, powerPelletSound, fishSound;

	public Sprite soundOn, soundOff;
	public Image soundIcon;
	public bool isMute;

	// Use this for initialization
	void Start () {
		Debug.Log ("Audio Manager Start");
		instance = this;
		isMute = false;
		backgroundMusicSource = GetComponent<AudioSource> ();
	}

	public void doMeow(){
		soundEffectsSource.clip = meowSound;

		if(!soundEffectsSource.isPlaying)
			soundEffectsSource.Play ();
		
		//AnimationState currentState = CatController.instance.kittyAnimator.GetCurrentAnimatorStateInfo (0);
		CatController.instance.kittyAnimator.SetTrigger ("vibrate");
		CatController.instance.kittyAnimator.SetInteger ("lane", CatController.instance.lane);
	}

	public void playPowerPelletCollectionSound(){
		soundEffectsSource.clip = powerPelletSound;
		soundEffectsSource.Play ();
	}

	public void playFishCollectionSound(){
		soundEffectsSource.clip = fishSound;
		soundEffectsSource.Play ();
	}

	public void SoundControl(){
		mainMenuBGSource.enabled = isMute;
		backgroundMusicSource.enabled = isMute;
		if(isMute)
			soundIcon.GetComponent<Image> ().sprite = soundOn;
		else
			soundIcon.GetComponent<Image> ().sprite = soundOff;
		isMute = !isMute;
	}
}
