using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	[SerializeField]
	string version = "0.0.1ad";

	GameObject overviewCamera;

	GameObject[] respawns;

	GameObject player;
	string playerName;

	bool offlineMode = false;
	bool joinedLobby = false;

	bool respawnStarted = false;
	bool playerNameSubmitted = false;

	[SerializeField]
	float respawnDelay = 5f;

	float respawnTime = 0f;

	[SerializeField]
	GameObject RoomHUDPanelObj;
	[SerializeField]
	GameObject PlayerHUDObj;
	[SerializeField]
	GameObject PlayerNameHUDObj;

	int maxPlayersPerRoom = 20;

	// Use this for initialization
	void Start () {
		overviewCamera = GameObject.FindGameObjectWithTag ("OverviewCamera");

		respawns = GameObject.FindGameObjectsWithTag ("Respawn");
	}

	void Connect() {
		PlayerNameHUDObj.SetActive (false);
		RoomHUDPanelObj.SetActive (true);
		PlayerHUDObj.SetActive (false);

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
	}

	void OnPhotonRandomJoinFailed() {
		PhotonNetwork.CreateRoom (null);
	}

	void Update() {
		if(!playerNameSubmitted) {
			// If the game has not started (Joining room) yet, unlock the mouse and show the room HUD
			Screen.lockCursor = false;

			PlayerNameHUDObj.SetActive (true);
			RoomHUDPanelObj.SetActive (false);
			PlayerHUDObj.SetActive (false);
			
			UILabel message = RoomHUDPanelObj.transform.FindChild("Message").GetComponent<UILabel>();
			
			message.text = PhotonNetwork.connectionStateDetailed.ToString ();

			UILabel versionInfo = RoomHUDPanelObj.transform.FindChild ("VersionInfo").GetComponent<UILabel>();

			versionInfo.text = "ver." + version;

			return;
		}

		if(PhotonNetwork.room == null) return;

		if(player == null) {
			// If the player is null (not spawned yet or waiting for respawn), unlock the mouse and show the room HUD
			Screen.lockCursor = false;

			PlayerHUD.HideHP();

			PlayerNameHUDObj.SetActive (false);
			RoomHUDPanelObj.SetActive (true);
			PlayerHUDObj.SetActive (false);
			
			overviewCamera.SetActive(true);
			
			if(!respawnStarted)
				StartCoroutine("WaitToRespawn");
			else {
				UILabel message = RoomHUDPanelObj.transform.FindChild("Message").GetComponent<UILabel>();
				
				message.text = "Respawn Time: " + (int)respawnTime + " seconds remaining";
				
				respawnTime -= Time.deltaTime;
			}
		}

		else {
			// If the player is active, lock the cursor and show the player HUD
			Screen.lockCursor = true;

			PlayerNameHUDObj.SetActive (false);
			RoomHUDPanelObj.SetActive (false);
			PlayerHUDObj.SetActive(true);

			PlayerHUD.ShowHP();
		}
	}

	IEnumerator WaitToRespawn() {
		respawnStarted = true;
		respawnTime = respawnDelay;
		yield return new WaitForSeconds (respawnDelay);
		
		SpawnMyPlayer ();
		respawnStarted = false;
	}

	void SpawnMyPlayer() {
		if(respawns == null) {
			Debug.LogError("No respawn points");
			return;
		}

		overviewCamera.SetActive (false);

		GameObject spawnSpot = respawns[Random.Range (0, respawns.Length)];

		player = PhotonNetwork.Instantiate ("Player", spawnSpot.transform.position, spawnSpot.transform.localRotation, 0);

		player.GetComponent<TPSCharacter> ().enabled = true;

		MouseLook[] mouseLooks = player.GetComponentsInChildren<MouseLook>();
		foreach(MouseLook script in mouseLooks)
			script.enabled = true;

		FireProjectile[] fireProjectiles = player.GetComponentsInChildren<FireProjectile> ();
		foreach(FireProjectile script in fireProjectiles)
			script.enabled = true;

		player.transform.FindChild ("Head/Main Camera").gameObject.SetActive (true);

		player.GetComponent<NetworkHealth> ().enabled = true;
		player.GetComponent<NetworkCharacter> ().SetName (playerName);
	}

	public void OnPlayerNameSubmit() {
		string name = PlayerNameHUDObj.transform.FindChild ("Window/NameInput/Label").GetComponent<UILabel> ().text;

		if(name != null && name.Length > 0) {
			playerName = name;
			playerNameSubmitted = true;
			Connect();
		}
	}
}
