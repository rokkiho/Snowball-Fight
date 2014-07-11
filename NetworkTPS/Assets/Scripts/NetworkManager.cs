using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	[SerializeField]
	string version = "0.0.5adprev";

	static string playerName;

	bool offlineMode = false;

	bool playerNameSubmitted = false;

	void Connect() {
		if(PhotonNetwork.connectionState != ConnectionState.Disconnected) return;

		HUDController.ShowRoomHUD ();

		if(offlineMode) {
			PhotonNetwork.offlineMode = true;
			OnJoinedLobby();
		}
		else {
			PhotonNetwork.ConnectUsingSettings (version);
		}
	}

	void OnJoinedLobby() {
		Debug.Log ("Joined Lobby");

		PhotonNetwork.JoinRandomRoom ();
	}

	void OnJoinedRoom() {
		Debug.Log ("Joined Room: " + PhotonNetwork.room.name);

		if(PhotonNetwork.isMasterClient) {
			// I'm the first to join this room. That is, I created the room. So start a new match
			NetworkRoom.StartDeathMatch();
		}
		else {
			// Get the room's match info
			NetworkRoom.ReceiveMatchInfo();
		}
	}

	void OnPhotonRandomJoinFailed() {
		RoomOptions roomOptions = new RoomOptions ();
		roomOptions.maxPlayers = 20;

		PhotonNetwork.JoinOrCreateRoom(Random.Range(0f, 1000f) + "#Room", roomOptions, TypedLobby.Default);
	}

	void Update() {
		if(!playerNameSubmitted) {
			// If the game has not started (Joining room) yet, unlock the mouse and show the room HUD
			Screen.lockCursor = false;

			HUDController.ShowPlayerNameHUD();

			GameObject.Find("UI Root").
					   transform.FindChild("RoomHUD/Message").
					   GetComponent<UILabel>().text = PhotonNetwork.connectionStateDetailed.ToString();

			GameObject.Find("UI Root").
					   transform.FindChild("RoomHUD/VersionInfo").
					   GetComponent<UILabel>().text = "ver." + version;
			
			return;
		}
	}

	public void OnPlayerNameSubmit() {
		string name = GameObject.Find ("NameInput").transform.FindChild("Label").GetComponent<UILabel> ().text;

		if(name != null && name.Length > 0) {
			playerName = name;
			playerNameSubmitted = true;
			Connect();
		}

	}

	public static string GetPlayerName() {
		return playerName;
	}
}
