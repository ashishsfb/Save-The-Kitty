using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KittyAvatar : MonoBehaviour {
	public int avatarId = 1;
	public int price = 100;

	public enum KittyStatuses	{Locked, Bought, Selected};
	public KittyStatuses status = KittyStatuses.Locked;

	public GameObject kittyGameObject;
}
