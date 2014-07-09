using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {

	[SerializeField]
	static GameObject RoomHUD;
	[SerializeField]
	static GameObject PlayerHUD;
	[SerializeField]
	static GameObject PlayerNameHUD;

	void Start() {
		RoomHUD = GameObject.Find ("RoomHUD");
		PlayerHUD = GameObject.Find ("PlayerHUD");
		PlayerNameHUD = GameObject.Find ("PlayerNameHUD");
	}

	public static void ShowRoomHUD() {
		RoomHUD.SetActive (true);
		PlayerHUD.SetActive (false);
		PlayerNameHUD.SetActive (false);
	}

	public static void ShowPlayerHUD() {
		RoomHUD.SetActive (false);
		PlayerHUD.SetActive (true);
		PlayerNameHUD.SetActive (false);
	}

	public static void ShowPlayerNameHUD() {
		RoomHUD.SetActive (false);
		PlayerHUD.SetActive (false);
		PlayerNameHUD.SetActive (true);
	}
}
